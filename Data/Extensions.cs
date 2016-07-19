using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinSnakesTools.Data
{

    //[DebuggerStepThrough]
    static class Extensions
    {
        public static byte[] ReadBytes(this Stream stream, int length)
        {
            var data = new byte[length];

            stream.Read(data, 0, length);

            return data;
        }

        public static int ReadInt32(this Stream stream)
        {
            return stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16 | stream.ReadByte() << 24;
        }

        public static int ReadInt32BE(this Stream stream)
        {
            return stream.ReadByte() << 24 | stream.ReadByte() << 16 | stream.ReadByte() << 8 | stream.ReadByte();
        }

        public static uint ReadUInt32BE(this Stream stream)
        {
            unchecked
            {
                return (uint)ReadInt32BE(stream);
            }
        }

        public static short ReadInt16(this Stream stream)
        {
            unchecked
            {
                return (short)(stream.ReadByte() | stream.ReadByte() << 8);
            }
        }


        public static ushort ReadUInt16BE(this Stream stream)
        {
            unchecked
            {
                return (ushort)(stream.ReadByte() << 8 | stream.ReadByte());
            }
        }
        public static short ReadInt16BE(this Stream stream)
        {
            unchecked
            {
                return (short)(stream.ReadByte() << 8 | stream.ReadByte());
            }
        }
        public static uint ReadUInt32(this Stream stream)
        {
            unchecked
            {
                return (uint)(stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16 | stream.ReadByte() << 24);
            }
        }

        public static ushort ReadUInt16(this Stream stream)
        {
            unchecked
            {
                return (ushort)(stream.ReadByte() | stream.ReadByte() << 8);
            }
        }
        public static byte[] ReadToNull(this Stream stream)
        {
            var list = new List<byte>();
            var read = 0;
            while ((read = stream.ReadByte()) > 0)
            {
                list.Add((byte)read);
            }

            return list.ToArray();
        }

        public static void Write(this Stream stream, byte value)
        {
            stream.WriteByte(value);
        }
        public static void Write(this Stream stream, byte[] value)
        {
            stream.Write(value, 0, value.Length);
        }

        public static void Write(this Stream stream, short value)
        {
            var raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, 2);
        }

        public static void Write(this Stream stream, ushort value)
        {
            var raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, 2);
        }
        public static void Write(this Stream stream, int value)
        {
            var raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, 4);
        }

        public static void Write(this Stream stream, uint value)
        {
            var raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, 4);
        }

        public static void Write(this Stream stream, long value)
        {
            var raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, 8);
        }

        public static void Write(this Stream stream, ulong value)
        {
            var raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, 8);
        }


        public static void WriteBE(this Stream stream, short value)
        {
            var raw = BitConverter.GetBytes(value);
            Array.Reverse(raw);
            stream.Write(raw, 0, 2);
        }

        public static void WriteBE(this Stream stream, ushort value)
        {
            var raw = BitConverter.GetBytes(value);
            Array.Reverse(raw);
            stream.Write(raw, 0, 2);
        }
        public static void WriteBE(this Stream stream, int value)
        {
            var raw = BitConverter.GetBytes(value);
            Array.Reverse(raw);
            stream.Write(raw, 0, 4);
        }

        public static void WriteBE(this Stream stream, uint value)
        {
            var raw = BitConverter.GetBytes(value);
            Array.Reverse(raw);
            stream.Write(raw, 0, 4);
        }

        public static void WriteBE(this Stream stream, long value)
        {
            var raw = BitConverter.GetBytes(value);
            Array.Reverse(raw);
            stream.Write(raw, 0, 8);
        }

        public static void WriteBE(this Stream stream, ulong value)
        {
            var raw = BitConverter.GetBytes(value);
            Array.Reverse(raw);
            stream.Write(raw, 0, 8);
        }

        public static string ReadString(this Stream stream)
        {
            return Encoding.Default.GetString(ReadToNull(stream));
        }

        public static void Offset(this Stream stream, int offset)
        {
            stream.Position = ((stream.Position + offset - 1) / offset) * offset;
        }


        public static void WriteOffset(this Stream stream, int offset)
        {
            var count = ((stream.Position + offset - 1) / offset) * offset - stream.Position;


            while (count > 0)
            {
                if (count >= 8)
                {
                    stream.Write(0L);
                    count -= 8;
                }
                else if (count >= 4)
                {
                    stream.Write(0);
                    count -= 4;
                }
                else if (count >= 2)
                {
                    stream.Write((short)0);
                    count -= 2;
                }
                else if (count >= 1)
                {
                    stream.Write((byte)0);
                    count -= 1;
                }
            }
        }
    }
}
