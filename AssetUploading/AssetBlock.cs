using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PendulumClient.AssetUploading
{
    public class AssetBlock
    {
        public int UncompressedSize { get; }
        public int CompressedSize { get; }
        public short Flags { get; }
        public CompressionType Compression => (CompressionType)(Flags & 0x3F);

        public AssetBlock(int uncompressedSize, int compressedSize, short flags)
        {
            UncompressedSize = uncompressedSize;
            CompressedSize = compressedSize;
            Flags = flags;
        }

        public override string ToString()
        {
            return string.Format("Uncompressed size: {0}, Compressed size: {1}, Flags: {2}", UncompressedSize, CompressedSize, Flags);
        }

        public static AssetBlock FromReader(BinaryReader reader) => new AssetBlock(reader.ReadInt32BE(), reader.ReadInt32BE(), reader.ReadInt16BE());

        public void ToWriter(BinaryWriter writer, int uncompressedSize = 0, int compressedSize = 0)
        {
            uncompressedSize = uncompressedSize <= 0 ? UncompressedSize : uncompressedSize;
            compressedSize = compressedSize <= 0 ? CompressedSize : compressedSize;

            writer.WriteInt32BE(uncompressedSize);
            writer.WriteInt32BE(compressedSize);
            writer.WriteInt16BE((short)(Flags | (uint)CompressionType.LZ4HC));
        }
    }
}
