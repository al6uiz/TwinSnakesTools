using System;
using System.Diagnostics;
using System.IO;

namespace TwinSnakesTools.Data
{
    /// <summary>
    ///     Represents the Endianess of a Stream (Big = AA BB CC DD or Little = DD CC BB AA).
    /// </summary>
    public enum Endian
    {
        Little,
        Big,
        Default
    }

    /// <summary>
    ///     Represents a Binary Reader that can be used to read Big or Little endian data.
    /// </summary>
     //[DebuggerStepThrough]
    //public class EndianBinaryReader
    //{
    //    /// <summary>
    //    ///     The Base Stream of this Reader.
    //    /// </summary>
    //    public Stream BaseStream { get; set; }

    //    /// <summary>
    //    ///     The current Endian of the Reader.
    //    /// </summary>
    //    public Endian Endian;

    //    /// <summary>
    //    ///     Creates a new instace of the Endian Binary Reader.
    //    /// </summary>
    //    /// <param name="Input">The Input Stream used to read the data</param>
    //    /// <param name="Endian">The Endian used on this Stream</param>
    //    public EndianBinaryReader(Stream Input, Endian Endian)
    //    {
    //        BaseStream = Input;
    //        this.Endian = Endian;
    //    }

    //    /// <summary>
    //    ///     Reads a byte from the Stream.
    //    /// </summary>
    //    /// <returns>The byte at the current position</returns>
    //    public byte ReadByte()
    //    {
    //        return (byte)BaseStream.ReadByte();
    //    }

    //    /// <summary>
    //    ///     Reads a unsigned 16-bits integer from the Stream.
    //    /// </summary>
    //    /// <returns>The integer at the current position</returns>
    //    public ushort ReadUInt16()
    //    {
    //        if (Endian == Endian.Little)
    //            return (ushort)
    //                (BaseStream.ReadByte() |
    //                (BaseStream.ReadByte() << 8));
    //        else
    //            return (ushort)
    //                ((BaseStream.ReadByte() << 8) |
    //                BaseStream.ReadByte());
    //    }

    //    /// <summary>
    //    ///     Reads a signed 16-bits integer from the Stream.
    //    /// </summary>
    //    /// <returns>The integer at the current position</returns>
    //    public short ReadInt16()
    //    {
    //        return (short)ReadUInt16();
    //    }

    //    /// <summary>
    //    ///     Reads a unsigned 24-bits integer from the Stream.
    //    /// </summary>
    //    /// <returns>The integer at the current position</returns>
    //    public uint ReadUInt24()
    //    {
    //        if (Endian == Endian.Little)
    //            return (uint)
    //                (BaseStream.ReadByte() |
    //                (BaseStream.ReadByte() << 8) |
    //                (BaseStream.ReadByte() << 16));
    //        else
    //            return (uint)
    //                ((BaseStream.ReadByte() << 16) |
    //                (BaseStream.ReadByte() << 8) |
    //                BaseStream.ReadByte());
    //    }

    //    /// <summary>
    //    ///     Reads a unsigned 32-bits integer from the Stream.
    //    /// </summary>
    //    /// <returns>The integer at the current position</returns>
    //    public uint ReadUInt32()
    //    {
    //        if (Endian == Endian.Little)
    //            return (uint)
    //                (BaseStream.ReadByte() |
    //                (BaseStream.ReadByte() << 8) |
    //                (BaseStream.ReadByte() << 16) |
    //                (BaseStream.ReadByte() << 24));
    //        else
    //            return (uint)
    //                ((BaseStream.ReadByte() << 24) |
    //                (BaseStream.ReadByte() << 16) |
    //                (BaseStream.ReadByte() << 8) |
    //                BaseStream.ReadByte());
    //    }

    //    /// <summary>
    //    ///     Reads a signed 32-bits integer from the Stream.
    //    /// </summary>
    //    /// <returns>The integer at the current position</returns>
    //    public int ReadInt32()
    //    {
    //        return (int)ReadUInt32();
    //    }

    //    /// <summary>
    //    ///     Reads a 32-bits, single precision floating point value from the Stream.
    //    /// </summary>
    //    /// <returns>The float at the current position</returns>
    //    public float ReadSingle()
    //    {
    //        return BitConverter.ToSingle(BitConverter.GetBytes(ReadUInt32()), 0);
    //    }

    //    /// <summary>
    //    ///     Reads a Block of bytes from the Stream to a Buffer.
    //    /// </summary>
    //    /// <param name="Buffer">The Byte Array to store the data into</param>
    //    /// <param name="Index">Start Index of the Array (zero based)</param>
    //    /// <param name="Length">Number of bytes to read</param>
    //    public void Read(byte[] Buffer, int Index, int Length)
    //    {
    //        BaseStream.Read(Buffer, Index, Length);
    //    }

    //    /// <summary>
    //    ///     Seeks to a given position of the Stream.
    //    /// </summary>
    //    /// <param name="Offset">The Offset to Seek to, based on the Origin</param>
    //    /// <param name="Origin">From where to start counting the Offset</param>
    //    public void Seek(long Offset, SeekOrigin Origin)
    //    {
    //        BaseStream.Seek(Offset, Origin);
    //    }

    //    /// <summary>
    //    ///     Closes the underlying Stream.
    //    /// </summary>
    //    public void Close()
    //    {
    //        BaseStream.Close();
    //    }
    //}
    /// <summary>
    ///     Represents a Binary Writer that can be used to write Big or Little endian data.
    /// </summary>
    public class EndianBinaryWriter
    {
        /// <summary>
        ///     The Base Stream of this Writer.
        /// </summary>
        public Stream BaseStream { get; set; }

        /// <summary>
        ///     The current Endian of the Writer.
        /// </summary>
        public Endian Endian;

        /// <summary>
        ///     Creates a new instace of the Endian Binary Writer.
        /// </summary>
        /// <param name="Input">The Input Stream used to write the data</param>
        /// <param name="Endian">The Endian used on this Stream</param>
        public EndianBinaryWriter(Stream Input, Endian Endian)
        {
            BaseStream = Input;
            this.Endian = Endian;
        }

        /// <summary>
        ///     Writes a byte to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write(byte Value)
        {
            BaseStream.WriteByte(Value);
        }

        /// <summary>
        ///     Writes a unsigned 16-bits integer to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write(ushort Value)
        {
            if (Endian == Endian.Little)
            {
                BaseStream.WriteByte((byte)Value);
                BaseStream.WriteByte((byte)(Value >> 8));
            }
            else
            {
                BaseStream.WriteByte((byte)(Value >> 8));
                BaseStream.WriteByte((byte)Value);
            }
        }

        /// <summary>
        ///     Writes a signed 16-bits integer to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write(short Value)
        {
            Write((ushort)Value);
        }

        /// <summary>
        ///     Writes a unsigned 24-bits integer to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write24(uint Value)
        {
            if (Endian == Endian.Little)
            {
                BaseStream.WriteByte((byte)Value);
                BaseStream.WriteByte((byte)(Value >> 8));
                BaseStream.WriteByte((byte)(Value >> 16));
            }
            else
            {
                BaseStream.WriteByte((byte)(Value >> 16));
                BaseStream.WriteByte((byte)(Value >> 8));
                BaseStream.WriteByte((byte)Value);
            }
        }

        /// <summary>
        ///     Writes a unsigned 32-bits integer to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write(uint Value)
        {
            if (Endian == Endian.Little)
            {
                BaseStream.WriteByte((byte)Value);
                BaseStream.WriteByte((byte)(Value >> 8));
                BaseStream.WriteByte((byte)(Value >> 16));
                BaseStream.WriteByte((byte)(Value >> 24));
            }
            else
            {
                BaseStream.WriteByte((byte)(Value >> 24));
                BaseStream.WriteByte((byte)(Value >> 16));
                BaseStream.WriteByte((byte)(Value >> 8));
                BaseStream.WriteByte((byte)Value);
            }
        }

        /// <summary>
        ///     Writes a signed 32-bits integer to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write(int Value)
        {
            Write((uint)Value);
        }

        /// <summary>
        ///     Writes a 32-bits, single precision floating point value to the Stream.
        /// </summary>
        /// <param name="Value">The value to be written</param>
        public void Write(float Value)
        {
            Write(BitConverter.ToUInt32(BitConverter.GetBytes(Value), 0));
        }

        /// <summary>
        ///     Writes a Block of bytes to the Stream.
        /// </summary>
        /// <param name="Buffer">The Buffer to be written</param>
        public void Write(byte[] Buffer)
        {
            BaseStream.Write(Buffer, 0, Buffer.Length);
        }

        /// <summary>
        ///     Writes a Block of bytes to the Stream.
        /// </summary>
        /// <param name="Buffer">The Buffer to be written</param>
        /// <param name="Index">Start Index of the Array (zero based)</param>
        /// <param name="Length">Number of bytes to write</param>
        public void Write(byte[] Buffer, int Index, int Length)
        {
            BaseStream.Write(Buffer, Index, Length);
        }

        /// <summary>
        ///     Seeks to a given position of the Stream.
        /// </summary>
        /// <param name="Offset">The Offset to Seek to, based on the Origin</param>
        /// <param name="Origin">From where to start counting the Offset</param>
        public void Seek(long Offset, SeekOrigin Origin)
        {
            BaseStream.Seek(Offset, Origin);
        }

        /// <summary>
        ///     Closes the underlying Stream.
        /// </summary>
        public void Close()
        {
            BaseStream.Close();
        }
    }
}
