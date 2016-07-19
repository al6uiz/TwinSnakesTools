using System.IO;
using System.Xml.Serialization;

namespace TwinSnakesTools.Data
{
    public class StreamSection
    {
        [XmlIgnore]
        public SectionType Type { get; set; }

        [XmlAttribute]
        public ushort StreamID { get; set; }


        [XmlAttribute]
        public ushort LanguageID { get; set; }

        [XmlAttribute]
        public long Offset { get; set; }

        public static StreamSection FromStream(Stream reader)
        {
            
            var id = reader.ReadUInt32BE() & 0xff;
            reader.Seek(-4, SeekOrigin.Current);

            switch (id)
            {
                case 0x04: return SubtitleSection.FromStream(reader);
                case 0xF0: return EndOfStreamSection.FromStream(reader);
                default: return RawSection.FromStream(reader);
            }
        }

        public static void ToStream(Stream writer, StreamSection packet)
        {
            switch (packet.Type)
            {
                case SectionType.Subtitle: SubtitleSection.ToStream(writer, (SubtitleSection)packet); break;
                case SectionType.EndOfStream: EndOfStreamSection.ToStream(writer, (EndOfStreamSection)packet); break;
                case SectionType.Raw: RawSection.ToStream(writer, (RawSection)packet); break;
            }
        }

    }


    public enum SectionType
    {
        Raw,
        EndOfStream,
        Subtitle
    }
}
