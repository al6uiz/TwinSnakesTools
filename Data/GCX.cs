using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace TwinSnakesTools.Data
{
    public class GCX
    {
        [XmlIgnore]
        public int Magic { get; set; }

        [XmlAttribute("Magic")]
        public string MagicHex
        {
            get { return Magic.ToString("X8"); }
            set { Magic = int.Parse(value, NumberStyles.HexNumber); }
        }

        public List<Header> Headers { get; set; } = new List<Header>();
        public List<TextEntity> Entities { get; set; } = new List<TextEntity>();

        [XmlAttribute]
        public int Length { get; set; }

        [XmlAttribute]
        public long Offset { get; set; }

        public byte[] FontData { get; set; }

        public static void Unpack(Stream stream, int length, string output, long position)
        {
            var gcx = new GCX();
            gcx.Offset = position;
            gcx.Length = length;
            gcx.Magic = stream.ReadInt32BE();

            while (true)
            {
                var value1 = stream.ReadUInt32BE();
                var value2 = stream.ReadUInt32BE();

                if (value1 == 0 && value2 == 0)
                {
                    break;
                }
                else
                {
                    gcx.Headers.Add(new Header { Unknown1 = value1, Unknown2 = value2 });
                }
            }

            var start = stream.Position;
            var length2 = stream.ReadInt32();
            var endian = stream.ReadInt32();

            var textStart = stream.ReadInt32() + start;
            var fontStart = stream.ReadInt32() + start;

            while (stream.Position < textStart)
            {
                gcx.Entities.Add(new TextEntity { OffsetValue = stream.ReadUInt32() });
            }



            foreach (var item in gcx.Entities)
            {
                var offset = item.OffsetValue & 0xFFFFFF;
                stream.Position = textStart + offset;
                item.Text = TextTable.GetString(stream.ReadToNull(), TextTable.Empty);
            }

            stream.Position = fontStart;
            var fontLength = stream.ReadInt32();
            if (fontLength > 0)
            {
                gcx.FontData = stream.ReadBytes(fontLength);
            }


            Directory.CreateDirectory(Path.GetDirectoryName(output));

            if (stream.Position < stream.Length)
            {
                File.WriteAllBytes(output + ".seq", stream.ReadBytes(stream.ReadInt32()));
            }

            if (stream.Position < stream.Length)
            {
                File.WriteAllBytes(output + ".unk", stream.ReadBytes(stream.ReadInt32()));
            }

            SerializationHelper.Save(gcx, output + ".xml");
        }



        public class TextEntity
        {
            [XmlIgnore]
            public uint OffsetValue { get; set; }

            [XmlAttribute]
            public uint Offset
            {
                get { return OffsetValue & 0xFFFFFF; }
                set
                {
                    unchecked
                    {
                        OffsetValue = (OffsetValue & 0xFF000000) | (value & 0xFFFFFF);
                    }
                }
            }

            [XmlAttribute]
            public byte Flag
            {
                get { return (byte)(OffsetValue >> 24); }
                set
                {
                    OffsetValue = (uint)(value << 24 | ((int)OffsetValue & 0xFFFFFF));
                }
            }


            [XmlText]
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public class Header
        {
            [XmlIgnore]
            public uint Unknown1 { get; set; }

            [XmlAttribute]
            public string Hash
            {
                get { return HashValue.ToString("X6"); }
                set { HashValue = uint.Parse(value, NumberStyles.HexNumber); }
            }

            [XmlIgnore]
            public uint HashValue
            {
                get { return Unknown1 & 0xFFFFFF; }
                set
                {
                    Unknown1 = (Unknown1 & 0xFF000000) | (value & 0xFFFFFF);

                }
            }
            [XmlAttribute]
            public byte Flag
            {
                get { return (byte)(Unknown1 >> 24); }
                set
                {
                    Unknown1 = (uint)((int)Unknown1 & 0xFFFFFF | (value << 24));
                }
            }


            [XmlAttribute]
            public uint Unknown2 { get; set; }


            public override string ToString()
            {
                return $"{Unknown1:X8} {Unknown2:X8}";
            }
        }
    }
}
