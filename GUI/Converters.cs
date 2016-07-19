using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwinSnakesTools.Data;

namespace TwinSnakesTools.GUI
{

    public class TextObjectConverter : IValueConverter
    {
        private FontFamily _font = new FontFamily("MS Gothic");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter as string)
            {
                case "Font":
                    return OnConvertFont(value as string);
                case "TimeTag":
                    return OnConvertTimeTag(System.Convert.ToUInt32(value));
                case "SubtitleName":
                    return OnConvertSubtitleName(value as SubtitleSection);
                default:
                    return null;
            }

        }

        private object OnConvertTimeTag(uint v)
        {
            return ToTimeTag(v);
        }

        private object OnConvertSubtitleName(SubtitleSection subtitleSection)
        {
            string language = "?";

            switch (subtitleSection.LanguageID)
            {
                case 1: language = "EN"; break;
                case 2: language = "FR"; break;
                case 3: language = "DE"; break;
                case 4: language = "IT"; break;
                case 5: language = "ES"; break;
                case 7: language = "JP"; break;
            }

            var timeTag = ToTimeTag(subtitleSection.BaseTime);
            var result = $"[{language}] {timeTag}";
            System.Diagnostics.Debug.WriteLine($"{result} {subtitleSection.Length:X4}");

            return result;
        }

        private static string ToTimeTag(uint value)
        {
            var tag = TimeSpan.FromMilliseconds(value);

            return $"{tag.Minutes:00}:{tag.Seconds:00}:{tag.Milliseconds:000}";
        }

        private object OnConvertFont(string text)
        {
            var element = new TextBlock();

            var hasFontLetter = false;

            RenderOptions.SetBitmapScalingMode(element, BitmapScalingMode.HighQuality);
            var buffer = new StringBuilder();
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    var letter = text[i];
                    if (letter == '\\')
                    {
                        if (buffer.Length > 0)
                        {
                            element.Inlines.Add(new Run(buffer.ToString()));
                            buffer.Clear();
                        }

                        var code = text.Substring(i + 2, 4);
                        var codeValue = ushort.Parse(code, NumberStyles.HexNumber);


                        if ((codeValue & 0xFC00) == 0x8C00 || (codeValue & 0xFC00) == 0x9000)
                        {
                            var index = codeValue & 0x3FF;

                            hasFontLetter |= true;

                            element.Inlines.Add(new InlineUIContainer(new Rectangle
                            {
                                Margin = new Thickness(0, 2, 0, -2),
                                UseLayoutRounding = true,
                                SnapsToDevicePixels = true,
                                Width = 24 / 1.5,
                                Height = 23 / 1.5,
                                Fill = new ImageBrush
                                {

                                    ViewboxUnits = BrushMappingMode.Absolute,
                                    Viewbox = GetViewBox(index),
                                    ImageSource = FontData
                                }
                            }));
                        }
                        else
                        {
                            buffer.Append($"[{code}]");
                        }

                        i += 5;
                    }
                    else if (letter == '[')
                    {
                        if (text[i + 1] == 'N' && text[i + 2] == 'L' && text[i + 3] == ']')
                        {
                            buffer.Append(Environment.NewLine);
                            i += 3;
                        }
                        else if (text[i + 1] == 'N' && text[i + 2] == ']')
                        {
                            buffer.Append(Environment.NewLine);
                            i += 2;
                        }
                        else
                        {
                            buffer.Append(letter);
                        }
                    }
                    else if (letter == '|')
                    {
                        buffer.Append(Environment.NewLine);
                    }
                    else
                    {
                        buffer.Append(letter);
                    }
                }
            }
            element.Inlines.Add(new Run(buffer.ToString()));

            if (hasFontLetter)
            {
                //element.FontFamily = _font;
            }

            return element;
        }

        private Rect GetViewBox(int index)
        {
            index = index - index / 255 - 1;
            var column = index % 20;
            var row = index / 20;

            return new Rect(column * 24, row * 24, 24, 23);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public BitmapImage FontData { get; set; }
    }

    public class SourceRegroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as List<SubtitleSection>;

            return source.Where(x => x.LanguageID == 1 || x.LanguageID == 7).OrderBy(x => x.LanguageID).ThenBy(x => x.Offset);

            //return source.GroupBy(x => x.LanguageID).Select(x =>
            //{
            //    var first = x.OrderBy(y => y.BaseTime).First();
            //    return new SubtitleSection
            //    {
            //        Offset = first.Offset,
            //        BaseTime = first.BaseTime,
            //        LanguageID = x.First().LanguageID,
            //        Texts = x.SelectMany(y => y.Texts).OrderBy(y => y.Start).ToList()
            //    };
            //});
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
