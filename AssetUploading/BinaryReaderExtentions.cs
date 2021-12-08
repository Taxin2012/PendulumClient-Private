using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PendulumClient.AssetUploading
{
    public static class BinaryReaderExtentions
    {
        public static string ReadCString(this BinaryReader reader)
        {
            int length = 0;
            char[] strBytes = new char[1024];
            for (int i = 0; i < strBytes.Length && (reader.BaseStream.Position != reader.BaseStream.Length); i++)
            {
                char c = reader.ReadChar();
                if (c == 0)
                    break;

                length++;
                strBytes[i] = c;
            }

            return new string(strBytes, 0, length);
        }

        public static void WriteCString(this BinaryWriter writer, string str)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(str);
            writer.Write(buffer);
            writer.Write('\0');
        }

        public static int ReadInt32BE(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static void WriteInt32BE(this BinaryWriter writer, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            writer.Write(buffer);
        }

        public static short ReadInt16BE(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static void WriteInt16BE(this BinaryWriter writer, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            writer.Write(buffer);
        }

        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static void WriteUInt32BE(this BinaryWriter writer, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            writer.Write(buffer);
        }

        public static long ReadInt64BE(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt64(bytes, 0);
        }

        public static void WriteInt64BE(this BinaryWriter writer, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            writer.Write(buffer);
        }
    }
}
