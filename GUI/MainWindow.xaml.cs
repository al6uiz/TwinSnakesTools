using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

using TwinSnakesTools.Data;

namespace TwinSnakesTools.GUI
{
    using Rectangle = System.Windows.Shapes.Rectangle;

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AllowDrop = true;
            Drop += MainWindow_Drop;

            DataContext = this;

            _converter = Resources["_converter"] as TextObjectConverter;

            ResetSource();
        }

        private string _tilte = "Twin Snakes Tool";

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            var formats = e.Data.GetFormats();

            var files = e.Data.GetData("FileNameW") as string[];
            if (files.Length != 1)
            {
                return;
            }

            var path = files.First();

            if (Path.GetExtension(path) == "xml")
            {
                return;
            }

            try
            {
                var fileName = Path.GetFileName(path);
                if (fileName.StartsWith("Subtitle"))
                {
                    ReadSubtitle(path);
                }
                else
                {
                    ReadGCX(path);
                }
                _lastPath = path;
                Title = $"{_tilte} - {fileName}";

            }
            catch
            {
                ResetSource();
            }
        }

        private void ResetSource()
        {
            _lastPath = null;
            _gcxEditor.DataContext = null;
            _subTitleEditor.DataContext = null;

            _gcxEditor.Visibility = Visibility.Collapsed;
            _subTitleEditor.Visibility = Visibility.Collapsed;

            Title = _tilte;

            _converter.FontData = null;
        }

        private string _lastPath;

        private void ReadSubtitle(string fileName)
        {
            _gcxEditor.Visibility = Visibility.Collapsed;
            _subTitleEditor.Visibility = Visibility.Visible;

            _subTitleEditor.DataContext = SerializationHelper<MovieSubtitle>.Read(fileName);
        }

        private void ReadGCX(string fileName)
        {
            _gcxEditor.Visibility = Visibility.Visible;
            _subTitleEditor.Visibility = Visibility.Collapsed;

            var gcx = SerializationHelper<GCX>.Read(fileName);
            _gcxEditor.DataContext = gcx;


            SetConverterFont(gcx.FontData);
        }

        private TextObjectConverter _converter;

        private void _subtitles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }

            var item = e.AddedItems[0] as SubtitleSection;

            SetConverterFont(item.FontData);

        }

        private void Save()
        {
            try
            {
                if (_subtitles.Visibility == Visibility.Visible)
                {
                    SerializationHelper.Save(_subtitles.DataContext as MovieSubtitle, _lastPath);
                }
                else if (_gcxEditor.Visibility == Visibility.Visible)
                {
                    SerializationHelper.Save(_gcxEditor.DataContext as GCX, _lastPath);
                }

                MessageBox.Show("저장 완료");
            }
            catch
            {
                MessageBox.Show("저장 실패");
            }
        }

        private void SetConverterFont(byte[] fontData)
        {
            if (fontData == null || fontData.Length == 0)
            {
                _converter.FontData = null;
                return;
            }

            var brush = Foreground as SolidColorBrush;
            var color = brush.Color;


            using (var ms = new MemoryStream())
            {
                using (var bitmap = FontData.GetBitmap(fontData))
                {
                    var bits = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    var length = bits.Stride * bits.Height;
                    var data = new byte[length];
                    Marshal.Copy(bits.Scan0, data, 0, data.Length);
                    for (int i = 0; i < data.Length; i += 4)
                    {
                        if (data[i] == 0)
                        {
                            data[i + 3] = 0;
                        }
                        else
                        {
                            data[i + 0] = color.R;
                            data[i + 1] = color.G;
                            data[i + 2] = color.B;
                        }
                    }
                    Marshal.Copy(data, 0, bits.Scan0, data.Length);

                    bitmap.UnlockBits(bits);

                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                ms.Position = 0;

                var src = new BitmapImage();
                src.BeginInit();
                src.StreamSource = ms;
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();

                _converter.FontData = src;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
    }
}
