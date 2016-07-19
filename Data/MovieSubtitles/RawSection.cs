using System;
using System.IO;


namespace TwinSnakesTools.Data
{
    public class RawSection : StreamSection
    {
        public override string ToString()
        {
            return $"{StreamID} {Length} {First}";
        }

        public uint First
        {
            get
            {
                return (uint)(Data[0] << 24 | Data[1] << 16 | Data[2] << 8 | Data[3] << 0);

            }
        }

        public byte[] Data { get; set; }

        public uint Length { get; set; }

        public RawSection()
        {
            Type = SectionType.Raw;
        }

        public static new RawSection FromStream(Stream reader)
        {
            var section = new RawSection();

            section.LanguageID = reader.ReadUInt16BE();
            section.StreamID = reader.ReadUInt16BE();
            section.Length = reader.ReadUInt32BE();
            section.Data = new byte[section.Length - 8];
            reader.Read(section.Data, 0, section.Data.Length);

            return section;
        }

        public static void ToStream(Stream writer, RawSection packer)
        {
            writer.WriteBE(packer.LanguageID);
            writer.WriteBE(packer.StreamID);
            writer.WriteBE(packer.Data.Length + 8);
            writer.Write(packer.Data);
        }
    }
}
