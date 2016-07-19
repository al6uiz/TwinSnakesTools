using System.Diagnostics;
using System.IO;

namespace TwinSnakesTools.Data
{
    public class EndOfStreamSection : StreamSection
    {
        public uint Value { get; set; }
        public uint Length { get; set; }

        public EndOfStreamSection()
        {
            Type = SectionType.EndOfStream;
        }


        public static new EndOfStreamSection FromStream(Stream reader)
        {
            var packet = new EndOfStreamSection();

            packet.LanguageID = reader.ReadUInt16BE();
            packet.StreamID = reader.ReadUInt16BE();
            packet.Length = reader.ReadUInt32BE();

            packet.Value = reader.ReadUInt32BE();
            uint dummy = reader.ReadUInt32BE();

            Debug.Assert(dummy == 0);

            reader.Offset(0x800);

            return packet;
        }

        public static void ToStream(Stream writer, EndOfStreamSection packet)
        {
            writer.WriteBE(packet.LanguageID);
            writer.WriteBE(packet.StreamID);

            writer.WriteBE(packet.Length);

            writer.WriteBE(packet.Value);
            writer.Write(0u);

            writer.WriteOffset(0x800);

        }
    }

}
