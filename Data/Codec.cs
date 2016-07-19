using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinSnakesTools.Data
{
    class Codec
    {
        public static void Unpack(string path)
        {
            var output = path.Remove(path.LastIndexOf('.'));

            using (var reader = File.OpenRead(path))
            {
                var data = new byte[4];
                var buffer = new List<byte>();
                var offset = 0;

                while (reader.Position < reader.Length)
                {
                    reader.Read(data, 0, 4);

                    if (data[0] == 0x44 && data[1] == 0xAA && data[2] == 0x21 && data[3] == 0x40)
                    {
                        if (buffer.Count > 0)
                        {
                            Unpack(output, buffer, offset);
                            buffer.Clear();
                            offset = (int)(reader.Position - 4);
                        }
                    }

                    buffer.AddRange(data);
                }
                if (buffer.Count > 0)
                {
                    Unpack(output, buffer, offset);
                }
            }
        }

        private static void Unpack(string output, List<byte> buffer, long offset)
        {
            var data = buffer.ToArray();
            using (var ms = new MemoryStream(data))
            {
                var name = output + $@"\{offset:X8}";

                GCX.Unpack(ms, data.Length, name, offset);
            }
        }
    }
}
