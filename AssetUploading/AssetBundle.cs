using LZ4;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using SevenZip.Compression.LZMA;

namespace PendulumClient.AssetUploading
{
    public class AssetBundle
    {
        public string Signature { get; }
        public int FormatVersion { get; }
        public string UnityVersion { get; }
        public string GeneratorVersion { get; }
        public long FileSize { get; }
        public uint CompressedBlockSize { get; }
        public uint UncompressedBlockSize { get; }
        public uint Flags { get; }
        public CompressionType Compression => (CompressionType)(Flags & 0x3F);

        private List<AssetBlock> _assetBlocks = new List<AssetBlock>();
        public ReadOnlyCollection<AssetBlock> AssetBlocks => new ReadOnlyCollection<AssetBlock>(_assetBlocks);

        private List<AssetNode> _assetNodes = new List<AssetNode>();
        public ReadOnlyCollection<AssetNode> AssetNodes => new ReadOnlyCollection<AssetNode>(_assetNodes);

        private byte[] UncompressedBlock { get; }
        private byte[] DecoderProperties { get; } = new byte[5];
        private byte[] UncompressedBlockNodeHeader { get; }

        public AssetBundle(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            using (BinaryReader br = new BinaryReader(fs))
            {

                Signature = br.ReadCString();
                if (Signature != "UnityFS")
                    throw new NotSupportedException("Unsupported file signature.");

                FormatVersion = br.ReadInt32BE();
                UnityVersion = br.ReadCString();
                GeneratorVersion = br.ReadCString();
                FileSize = br.ReadInt64BE();
                CompressedBlockSize = br.ReadUInt32BE();
                UncompressedBlockSize = br.ReadUInt32BE();
                Flags = br.ReadUInt32BE();

                BinaryReader decompressedReader = br;
                if (Compression != CompressionType.None)
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] compressedBuffer = new byte[CompressedBlockSize];
                    byte[] decompressedBuffer = new byte[UncompressedBlockSize];

                    br.Read(compressedBuffer, 0, compressedBuffer.Length);
                    LZ4Codec.Decode(compressedBuffer, 0, compressedBuffer.Length, decompressedBuffer, 0, decompressedBuffer.Length, true);
                    ms.Write(decompressedBuffer, 0, decompressedBuffer.Length);
                    ms.Position = 0;

                    decompressedReader = new BinaryReader(ms);
                }

                decompressedReader.ReadBytes(16);

                int blockCount = decompressedReader.ReadInt32BE();
                for (int i = 0; i < blockCount; i++)
                    _assetBlocks.Add(AssetBlock.FromReader(decompressedReader));

                int nodeCount = decompressedReader.ReadInt32BE();
                for (int i = 0; i < nodeCount; i++)
                    _assetNodes.Add(AssetNode.FromReader(decompressedReader));

                if (decompressedReader != br)
                    decompressedReader.Close();

                AssetBlock block = _assetBlocks.First();
                UncompressedBlock = new byte[block.UncompressedSize];

                Console.WriteLine(block.Compression);

                br.Read(DecoderProperties, 0, DecoderProperties.Length);

                if (block.Compression == CompressionType.None)
                    br.Read(UncompressedBlock, 0, block.UncompressedSize);
                else if (block.Compression == CompressionType.LZMA)
                {
                    byte[] compressedData = new byte[block.CompressedSize];
                    br.Read(compressedData, 0, compressedData.Length);

                    using (MemoryStream inStream = new MemoryStream(compressedData))
                    using (MemoryStream outStream = new MemoryStream(UncompressedBlock))
                    {
                        /*SevenZip.Compression.LZMA.Decoder lzmaDecoder = new SevenZip.Compression.LZMA.Decoder();

                        lzmaDecoder.SetDecoderProperties(DecoderProperties);
                        lzmaDecoder.Code(inStream, outStream, block.CompressedSize, block.UncompressedSize, null); might have to fix this eventually?*/
                    }
                }
                else if (block.Compression == CompressionType.LZ4HC)
                {
                    br.BaseStream.Position -= DecoderProperties.Length;
                    byte[] compressedData = new byte[block.CompressedSize];
                    br.Read(compressedData, 0, compressedData.Length);

                    LZ4Codec.Decode(compressedData, 0, compressedData.Length, UncompressedBlock, 0, UncompressedBlock.Length, true);
                }
                else throw new NotSupportedException("Compression type not supported.");
            }
        }

        public void SetAvatarId(string avatarId)
        {
            for (int i = 0; i < UncompressedBlock.Length - 41; i++)
            {
                if (UncompressedBlock[i] == 'a' && UncompressedBlock[i + 1] == 'v' && UncompressedBlock[i + 2] == 't' && UncompressedBlock[i + 3] == 'r')
                {
                    byte[] avatarIdChars = Encoding.ASCII.GetBytes(avatarId);
                    Array.Copy(avatarIdChars, 0, UncompressedBlock, i, 41);
                    i += 41;
                }
            }
        }

        public void SaveTo(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                SaveTo(fs);
        }

        public void SaveTo(Stream stream)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                uint newCompressedBlockSize = 0, newUncompressedBlockSize = 0;

                bw.WriteCString(Signature);
                bw.WriteInt32BE(FormatVersion);
                bw.WriteCString(UnityVersion);
                bw.WriteCString(GeneratorVersion);

                long fileSizePosition = bw.BaseStream.Position;
                bw.BaseStream.Write(new byte[16], 0, 16);

                bw.WriteUInt32BE(Flags | (uint)CompressionType.LZ4HC);

                using (MemoryStream inStream = new MemoryStream(UncompressedBlock))
                using (MemoryStream compressedOutStream = new MemoryStream())
                {
                    byte[] encodedBlockBuffer = LZ4Codec.EncodeHC(UncompressedBlock, 0, UncompressedBlock.Length);

                    compressedOutStream.Position = 0;
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (BinaryWriter tempWriter = new BinaryWriter(memoryStream))
                    {
                        tempWriter.Write(new byte[16], 0, 16);

                        tempWriter.WriteInt32BE(_assetBlocks.Count);
                        foreach (AssetBlock block in _assetBlocks)
                            block.ToWriter(tempWriter, (int)inStream.Length, (int)encodedBlockBuffer.Length);

                        tempWriter.WriteInt32BE(_assetNodes.Count);
                        foreach (AssetNode node in _assetNodes)
                            node.ToWriter(tempWriter);

                        byte[] compressedBlockNodeHeader = LZ4Codec.EncodeHC(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                        newCompressedBlockSize = (uint)compressedBlockNodeHeader.Length;
                        newUncompressedBlockSize = (uint)memoryStream.Length;
                        bw.Write(compressedBlockNodeHeader);
                    }

                    stream.Write(encodedBlockBuffer, 0, encodedBlockBuffer.Length);
                }

                bw.BaseStream.Seek(fileSizePosition, SeekOrigin.Begin);
                bw.WriteInt64BE(bw.BaseStream.Length);
                bw.WriteUInt32BE(newCompressedBlockSize);
                bw.WriteUInt32BE(newUncompressedBlockSize);
            }
        }

        public override string ToString()
        {
            return string.Format("Signature: {0}\nFormatVersion: {1}\nUnityVersion: {2}\nGeneratorVersion: {3}\nFileSize: {4}\nCompressedBlockSize: {5}\nUncompressedBlockSize: {6}\nFlags: {7}",
                Signature, FormatVersion, UnityVersion, GeneratorVersion, FileSize, CompressedBlockSize, UncompressedBlockSize, Flags);
        }
    }
}
