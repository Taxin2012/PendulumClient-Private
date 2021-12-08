using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PendulumClient.AssetUploading
{
    public class AssetNode
    {
                public long Offset { get; }
        public long Size { get; }
        public int Flags { get; }
        public string Name { get; }

        public AssetNode(long offset, long size, int flags, string name)
        {
            Offset = offset;
            Size = size;
            Flags = flags;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("Offset: {0}, Size: {1}, Flags: {2}, Name: {3}", Offset, Size, Flags, Name);
        }

        public static AssetNode FromReader(BinaryReader reader) => new AssetNode(reader.ReadInt64BE(), reader.ReadInt64BE(), reader.ReadInt32BE(), reader.ReadCString());

        public void ToWriter(BinaryWriter writer, long offset = -1, long size = 0)
        {
            size = size <= 0 ? Size : size;
            offset = offset <= -1 ? Offset : offset;

            writer.WriteInt64BE(offset);
            writer.WriteInt64BE(size);
            writer.WriteInt32BE(Flags | (int)CompressionType.LZ4HC);
            writer.WriteCString(Name);
        }
    }
}
