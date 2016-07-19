using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TwinSnakesTools.Data
{
    using FontUtility = TwinSnakesTools.Data.FontData;

    public class SubtitleSectionText
    {
        [XmlAttribute]
        public uint Start { get; set; }

        [XmlAttribute]
        public uint End { get; set; }

        [XmlAttribute]
        public uint Unknown1 { get; set; }

        [XmlAttribute]
        public ushort Length { get; set; }
        [XmlAttribute]
        public ushort Unknown2 { get; set; }

        public string Text { get; set; }

        public string TranslatedText { get; set; }


        public static SubtitleSectionText FromStream(Stream reader)
        {
            var section = new SubtitleSectionText();

            section.Start = reader.ReadUInt32();
            section.End = reader.ReadUInt32();

            section.Unknown1 = reader.ReadUInt32();
            section.Length = reader.ReadUInt16();

            Debug.Assert(section.Length % 4 == 0);

            section.Unknown2 = reader.ReadUInt16();

            byte[] textBuffer = new byte[section.Length - 0x10];
            reader.Read(textBuffer, 0, textBuffer.Length);
            section.Text = TextTable.GetString(textBuffer);

            return section;
        }

        public static void ToStream(Stream writer, SubtitleSectionText section, bool useDouble)
        {
            var textBuffer = new byte[0];

            if (section.Text != null)
            {
                textBuffer = TextTable.GetBytes(section.Text, useDouble);
            }

            int length = textBuffer.Length;
            if ((length & 3) != 0) length = (length & ~3) + 4;

            writer.Write(section.Start);
            writer.Write(section.End);
            writer.Write(section.Unknown1);
            writer.Write((ushort)(length + 0x10));
            writer.Write(section.Unknown2);

            writer.Write(textBuffer);

            writer.WriteOffset(4);
        }
    }


    public class SubtitleSection : StreamSection
    {
        public override string ToString()
        {
            return $"[{LanguageID}] {Offset} {Length}";
        }


        [XmlAttribute]
        public uint BaseTime { get; set; }

        [XmlArrayItem("Text")]
        public List<SubtitleSectionText> Texts { get; set; }

        public byte[] FontData { get; set; }

        [XmlAttribute]
        public uint Length { get; set; }

        public byte[] RawData { get; set; }

        public SubtitleSection()
        {
            Type = SectionType.Subtitle;
            Texts = new List<SubtitleSectionText>();
        }

        public new static SubtitleSection FromStream(Stream reader)
        {
            var section = new SubtitleSection();
            var basePosition = reader.Position;

            section.Offset = reader.Position;

            section.LanguageID = reader.ReadUInt16BE();
            section.StreamID = reader.ReadUInt16BE();

            section.Length = reader.ReadUInt32BE();
            section.BaseTime = reader.ReadUInt32BE();

            var endPosition = basePosition + section.Length;


            if (section.LanguageID == 1 || section.LanguageID == 7)
            {

                var dummy = reader.ReadUInt32();
                Debug.Assert(dummy == 0);

                var dataLength = reader.ReadUInt32();

                var textStart = reader.Position;

                while (reader.Position < textStart + dataLength)
                {
                    var text = SubtitleSectionText.FromStream(reader);

                    section.Texts.Add(text);
                }

                Debug.Assert(reader.Position == textStart + dataLength);

                var fontLength = reader.ReadUInt32();
                section.FontData = reader.ReadBytes((int)fontLength);

                if (endPosition != reader.Position)
                {
                    var rem = reader.ReadBytes((int)(endPosition - reader.Position));
                    Debug.Assert(rem.Count(x => x == 0) == rem.Length);

                }
                Debug.Assert(endPosition % 0x10 == 0);
                reader.Seek(endPosition, SeekOrigin.Begin);
            }
            else
            {
                section.RawData = reader.ReadBytes((int)(endPosition - reader.Position));
            }

            return section;
        }

        public static void ToStreamTranslated(Stream writer, SubtitleSection section)
        {
            byte[] rawText = null;

            ApplyNewFont(section);

            using (var content = new MemoryStream())
            {

                foreach (var text in section.Texts)
                {
                    SubtitleSectionText.ToStream(content, text, section.LanguageID == 7);
                }

                rawText = content.ToArray();
            }
            writer.WriteBE(section.LanguageID);
            writer.WriteBE(section.StreamID);

            if (section.LanguageID == 1 || section.LanguageID == 7)
            {
                var total = 16 + rawText.Length + 4 + 4 + section.FontData?.Length ?? 0;
                total = ((total + 16 - 1) / 16) * 16;
                writer.WriteBE(total);


                writer.WriteBE(section.BaseTime);

                writer.Write(0u);
                writer.Write((uint)rawText.Length);

                writer.Write(rawText);


                if (section.FontData == null)
                {
                    writer.Write(0);
                }
                else
                {
                    writer.Write(section.FontData.Length);
                    writer.Write(section.FontData);
                }

                writer.WriteOffset(16);
            }
            else
            {
                writer.WriteBE(section.Length);
                writer.WriteBE(section.BaseTime);

                writer.Write(section.RawData);
            }
        }

        public static void ToStream(Stream writer, SubtitleSection section)
        {
            byte[] rawText = null;

            using (var content = new MemoryStream())
            {
                foreach (var text in section.Texts)
                {
                    SubtitleSectionText.ToStream(content, text, section.LanguageID == 7);
                }

                rawText = content.ToArray();
            }

            writer.WriteBE(section.LanguageID);
            writer.WriteBE(section.StreamID);

            if (section.LanguageID == 1 || section.LanguageID == 7)
            {
                var total = 16 + rawText.Length + 4 + 4 + section.FontData?.Length ?? 0;
                total = ((total + 16 - 1) / 16) * 16;
                writer.WriteBE(total);


                writer.WriteBE(section.BaseTime);

                writer.Write(0u);
                writer.Write((uint)rawText.Length);

                writer.Write(rawText);

                //if (section.LanguageID == 7)
                {
                    if (section.FontData == null)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(section.FontData.Length);
                        writer.Write(section.FontData);
                    }
                }

                writer.WriteOffset(16);
            }
            else
            {
                writer.WriteBE(section.Length);
                writer.WriteBE(section.BaseTime);

                writer.Write(section.RawData);
            }
        }

        private static void ApplyNewFont(SubtitleSection section)
        {
            var letters = section.Texts.SelectMany(x => x.TranslatedText).Where(x => x >= '가' && x <= '힣').Distinct().ToArray();

            var start = section.FontData.Length / 144;
            start = 0x9000 + start + 1 + start / 255;

            start = 0x9001;

            var rs = new List<string>();
            var rt = new List<string>();

            foreach (var item in letters)
            {
                if ((start & 0xFF) == 0)
                {
                    start++;
                }

                rs.Add(new string(item, 1));
                rt.Add($"\\x{start++:X4}");
            }

            for (int i = 0; i < section.Texts.Count; i++)
            {
                var buffer = new StringBuilder(section.Texts[i].TranslatedText);
                for (int j = 0; j < letters.Length; j++)
                {
                    buffer.Replace(rs[j], rt[j]);
                }
                section.Texts[i].Text = " "+buffer.Replace(" ", "  ").Replace("[NL]","[NL] ").ToString();
            }

            using (var bitmap = FontUtility.GenerateFontBitmap(letters))
            {
                section.FontData = FontUtility.GetBytes(letters.Length, bitmap);
            }
        }

    }
}