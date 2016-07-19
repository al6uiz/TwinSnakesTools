using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Ionic.Zlib;

namespace TwinSnakesTools.Data
{
    class Program
    {
        static void Main(string[] args)
        {
            var dest = @"D:\Desktop\TC2\root";

            var src = @"E:\Games\Metal Gear Solid\TS\DISC1";

            MovieSubtitle.ApplyTranslation(Path.Combine(src, @"shared\vox.dat"), Path.Combine(dest, @"shared\vox.dat"));
            MovieSubtitle.ApplyTranslation(Path.Combine(src, @"shared\movie.dat"), Path.Combine(dest, @"shared\movie.dat"));
            MovieSubtitle.ApplyTranslation(Path.Combine(src, @"demo.dat"), Path.Combine(dest, @"demo.dat"));

            MovieSubtitle.Unpack(Path.Combine(dest, @"shared\vox.dat"), Path.Combine(dest, @"shared\vox"));
            MovieSubtitle.Unpack(Path.Combine(dest, @"shared\movie.dat"), Path.Combine(dest, @"shared\movie"));
            MovieSubtitle.Unpack(Path.Combine(dest, @"demo.dat"), Path.Combine(dest, @"demo"));

            Repack();


            //MovieSubtitle.Unpack(Path.Combine(src, @"shared\vox.dat"), Path.Combine(src, @"shared\vox"));
            //MovieSubtitle.Unpack(Path.Combine(src, @"shared\movie.dat"), Path.Combine(src, @"shared\movie"));
            //MovieSubtitle.Unpack(Path.Combine(src, @"demo.dat"), Path.Combine(src, @"demo"));

            //TestFont();

            //DumpFileList();
            //return;
            //ExtractAllGCX();
            //return;
            //ExtractAllSubtitles();
            return;
            //return;
            //foreach (var file in Directory.GetFiles(output))
            //{
            //    GCX.Unpack(file, TextTable.ToTable(string.Empty));
            //}

            //return;
            //var location = @"E:\Games\Metal Gear Solid\TS\DISC1\stage.dat.original";
            //UnpackStageData(location);



            //var path = @"E:\Games\Metal Gear Solid\TS\DISC1\stage";

            //foreach (var file in Directory.GetFiles(path, "*.gcx", SearchOption.AllDirectories))
            //{
            //    GCX.Unpack(file, TextTable.Empty);
            //    //if (file.Contains("title"))
            //    //{
            //    //    GCX.Unpack(file, TextTable.ToTable(_table_title));
            //    //}
            //    //else
            //    //{

            //    //    GCX.Unpack(file, TextTable.ToTable(_table_comm));
            //    //}
            //}



            //return;


            //var korean = File.ReadAllText(@"E:\TranslateMGS\TwinSnakes\Korean.txt", Encoding.Default);

            //var result = korean.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).Where(x => x.Key >= '가').OrderBy(x => x.Value).Reverse().Take(150).OrderBy(x => x.Key).ToArray();


            //var sum = 0;
            //using (var fs = File.OpenWrite("All2"))
            //{
            //    foreach (var file in Directory.GetFiles(@"E:\Games\Metal Gear Solid\TS\DISC1\state.dat", "*.*", SearchOption.AllDirectories))
            //    {
            //        var data = File.ReadAllBytes(file);
            //        Debug.WriteLine(file);
            //        if (sum > 0x18E2DDC0)
            //        {

            //        }
            //        sum += data.Length;
            //        //fs.Write(data, 0, data.Length);

            //    }
            //    //18E2DDC0
            //    //18F43AA0
            //    //1911A778
            //}


            //var path = @"E:\Games\Metal Gear Solid\TS\DISC1\shared\codec.dat";
            //using (var reader = new BigEndianBinaryReader(File.OpenRead(path)))
            //{
            //    reader.BaseStream.Position = 0x0002B67E;
            //    var text = ReadJapaneseString(reader.BaseStream);


            //}

        }

        private static void Repack()
        {
            var info = new ProcessStartInfo(@"E:\TranslateMGS\TwinSnakes\gcit.exe", @"""D:\Desktop\TC2"" -Q -Flush -D ""D:\Desktop\METAL GEAR SOLID THE TWIN SNAKES [GGSJA4].iso""");


            var process = Process.Start(info);
            process.WaitForExit();
        }

        private static void TestFont()
        {
            var sample = @"알래스카 폭스 군도의 독도, 섀도우 모세스 섬에 있는 핵병기 폐기소…. 폭스하운드 부대와 그들의 차세대 특수부대가 돌연 봉기하여 섬을 점령했다. 그들이 정부에게 요구한 것은 빅보스의 유체다. 그것이 24시간 이내에 받아들여지지 않을 경우, 그들은 핵을 발사하겠다고 통고해 왔다. 너에게 맡겨진 임무는 2가지. 폐기소에 잠입하여 인질로 잡혀있는 DARPA 국장 도널드 앤더슨과 암즈테크사 사장 케네스 베이커를 구출하는 것. 그리고 테러리스트의 핵 발사능력 유무를 조사하여, 사실이라면 그것을 저지하는 일이다.";

            var src = sample.Where(x => x > 0x80).Distinct().ToArray();
            using (var bitmap = FontData.GenerateFontBitmap(src))
            {
                var data = FontData.GetBytes(src.Length, bitmap);
                using (var newFont = FontData.GetBitmap(data))
                {
                    newFont.Save("fontOut.png", ImageFormat.Png);
                }
            }
        }

        private static void DumpFileList()
        {
            var location = @"E:\Games\Metal Gear Solid\TS\DISC1\";
            foreach (var file in ListFiles(location))
            {
                Debug.WriteLine($"{file.Substring(location.Length)}\t{new FileInfo(file).Length}");
            }
            return;
        }

        private static string[] ListFiles(string location)
        {
            var files = Directory.GetFiles(location, "*.dat", SearchOption.AllDirectories);

            return files;
        }

        private static void ExtractAllSubtitles()
        {
            var path = @"E:\Games\Metal Gear Solid\TS\DISC1\shared\vox.dat";
            MovieSubtitle.Unpack(path, @"E:\Games\Metal Gear Solid\TS\DISC1\shared\vox");

            path = @"E:\Games\Metal Gear Solid\TS\DISC1\demo.dat";
            MovieSubtitle.Unpack(path, @"E:\Games\Metal Gear Solid\TS\DISC1\shared\demo");

            path = @"E:\Games\Metal Gear Solid\TS\DISC1\shared\movie.dat";
            MovieSubtitle.Unpack(path, @"E:\Games\Metal Gear Solid\TS\DISC1\shared\movie");


            path = @"E:\Games\Metal Gear Solid\TS\DISC2\shared\vox.dat";
            MovieSubtitle.Unpack(path, @"E:\Games\Metal Gear Solid\TS\DISC2\shared\vox");

            path = @"E:\Games\Metal Gear Solid\TS\DISC2\demo.dat";
            MovieSubtitle.Unpack(path, @"E:\Games\Metal Gear Solid\TS\DISC2\shared\demo");

            path = @"E:\Games\Metal Gear Solid\TS\DISC2\shared\movie.dat";
            MovieSubtitle.Unpack(path, @"E:\Games\Metal Gear Solid\TS\DISC2\shared\movie");

            //    path = @"E:\Games\Metal Gear Solid\TS\DISC1\shared\codec.dat";

            //    Codec.Unpack(path);

            //    path = @"E:\Games\Metal Gear Solid\TS\DISC2\shared\codec.dat";

            //    Codec.Unpack(path);
        }

        private static void ExtractAllGCX()
        {
            var path = @"E:\Games\Metal Gear Solid\TS\DISC1\stage";
            foreach (var file in Directory.GetFiles(path, "*.gcx", SearchOption.AllDirectories))
            {
                using (var fs = File.OpenRead(file))
                {
                    GCX.Unpack(fs, (int)fs.Length, file, 0);
                }
            }

            path = @"E:\Games\Metal Gear Solid\TS\DISC2\stage";
            foreach (var file in Directory.GetFiles(path, "*.gcx", SearchOption.AllDirectories))
            {
                using (var fs = File.OpenRead(file))
                {
                    GCX.Unpack(fs, (int)fs.Length, file, 0);
                }
            }


            return;
        }

    }
}
