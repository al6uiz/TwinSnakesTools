using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TwinSnakesTools.Data
{
    public class MovieSubtitle
    {
        public static void Unpack(string path, string output)
        {
            Directory.CreateDirectory(output);

            using (var input = File.OpenRead(path))
            {
                var outBuffer = new MovieSubtitle();

                int index = 0;

                while (input.Position < input.Length)
                {
                    var section = StreamSection.FromStream(input);

                    switch (section.Type)
                    {
                        case SectionType.Subtitle:
                            var subtitleSection = section as SubtitleSection;

                            outBuffer.Subtitles.Add(subtitleSection);
                            break;
                        case SectionType.EndOfStream:
                            if (outBuffer.Subtitles.Count > 0)
                            {
                                var fileName = string.Format("Subtitle_{0:D5}.xml", index);
                                var outputPath = Path.Combine(output, fileName);
                                SerializationHelper.Save(outBuffer, outputPath);
                                outBuffer.Subtitles.Clear();
                            }
                            outBuffer.Length = input.Position - outBuffer.Position;
                            outBuffer.Position = input.Position;
                            index++;
                            break;
                        case SectionType.Raw:
                            var rawSection = section as RawSection;
                            break;
                    }
                }
            }
        }

        public static void ApplyTranslation(string originalPath, string outputPath)
        {
            var index = 0;
            var subtitleLocation = originalPath.Remove(originalPath.LastIndexOf('.'));

            var set = LoadSubtitle(index, subtitleLocation);
            var buffer = new Dictionary<long, byte[]>();

            using (var output = File.Create(outputPath))
            using (var input = File.OpenRead(originalPath))
            {
                while (input.Position < input.Length)
                {
                    var section = StreamSection.FromStream(input);

                    switch (section.Type)
                    {
                        case SectionType.Raw:
                            var rawSection = section as RawSection;

                            using (var ms = new MemoryStream())
                            {
                                RawSection.ToStream(ms, rawSection);

                                AppendToBuffer(buffer, rawSection.First, ms.ToArray());
                            }
                            break;
                        case SectionType.EndOfStream:

                            if (buffer.Count > 0)
                            {
                                var source = buffer.ToArray().OrderBy(x => x.Key);
                               
                                foreach (var item in source)
                                {
                                    output.Write(item.Value);
                                }

                                buffer.Clear();
                            }


                            EndOfStreamSection.ToStream(output, section as EndOfStreamSection);
                            index++;

                            set = LoadSubtitle(index, subtitleLocation);

                            break;
                        case SectionType.Subtitle:
                            var subtitleSection = section as SubtitleSection;

                            SubtitleSection loaded;
                            using (var ms = new MemoryStream())
                            {
                                if (set != null && set.TryGetValue(GetKey(subtitleSection), out loaded))
                                {
                                    if (loaded.Texts.Where(x => x.TranslatedText != null).Count() > 0)
                                    {
                                        var copy = loaded.Texts.ToArray();

                                        for (int i = 0; i < copy.Length; i++)
                                        {
                                            loaded.Texts.Clear();
                                            loaded.Texts.Add(copy[i]);
                                            loaded.BaseTime = copy[i].Start;

                                            SubtitleSection.ToStreamTranslated(ms, loaded);
                                            AppendToBuffer(buffer, loaded.BaseTime, ms.ToArray());
                                        }
                                    }
                                    else
                                    {
                                        SubtitleSection.ToStream(ms, subtitleSection);
                                        AppendToBuffer(buffer, subtitleSection.BaseTime, ms.ToArray());
                                    }
                                }
                                else
                                {
                                    SubtitleSection.ToStream(ms, subtitleSection);
                                    AppendToBuffer(buffer, subtitleSection.BaseTime, ms.ToArray());
                                }

                            }
                            break;
                    }
                }

            }
        }

        private static void AppendToBuffer(Dictionary<long, byte[]> buffer, uint key, byte[] data)
        {
            byte[] existing;
            if (buffer.TryGetValue(key, out existing))
            {
                buffer[key] = Merge(existing, data);
            }
            else
            {
                buffer.Add(key, data);
            }
        }

        private static byte[] Merge(params byte[][] data)
        {
            var total = data.Sum(x => x.Length);

            var output = new byte[total];

            var offset = 0;
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(data[i], 0, output, offset, data[i].Length);
                offset += data[i].Length;
            }

            return output;
        }

        private static Dictionary<long, SubtitleSection> LoadSubtitle(int index, string location)
        {
            var path = Path.Combine(location, $"Subtitle_{index:00000}.xml");

            if (File.Exists(path))
            {
                var section = SerializationHelper<MovieSubtitle>.Read(path);

                return section.Subtitles.Where(x => x.LanguageID == 7).ToDictionary(x => GetKey(x));
            }
            else
            {
                return null;
            }
        }

        private static long GetKey(SubtitleSection x)
        {
            return (x.BaseTime << 8) | x.LanguageID;
        }

        public List<SubtitleSection> Subtitles { get; set; } = new List<SubtitleSection>();

        [XmlAttribute]
        public long Position { get; set; }
        [XmlAttribute]
        public long Length { get; set; }
    }

}
