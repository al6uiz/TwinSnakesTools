using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ionic.Zlib;


namespace TwinSnakesTools.Data
{
    class StageData
    {
        private static void Unpack(string path)
        {
            var directories = new List<DirectoryEntity>();

            using (var stream = File.OpenRead(path))
            {
                var magic = stream.ReadInt32();
                var unknown1 = stream.ReadInt16BE();
                var unknown2 = stream.ReadInt16BE();
                var entryCount = stream.ReadInt16BE();
                var unknown3 = stream.ReadInt16BE();
                var unknown4 = stream.ReadInt32BE();

                for (int i = 0; i < entryCount; i++)
                {
                    var name = stream.ReadString();
                    stream.Offset(4);
                    var offset = stream.ReadInt32BE();
                    directories.Add(new DirectoryEntity
                    {
                        Offset = offset,
                        Name = name
                    });
                }

                if (!Directory.Exists("stage"))
                {
                    Directory.CreateDirectory("stage");
                }

                for (int i = 0; i < entryCount; i++)
                {
                    var entry = directories[i];
                    var position = entry.Offset * 0x800;
                    stream.Position = position;

                    var next = i == entryCount - 1 ? stream.Length : directories[i + 1].Offset * 0x800;
                    var length = (int)(next - position);

                    var data = stream.ReadBytes(length);

                    ExtractFile(data, entry.Name);
                }
            }
        }

        private static void ExtractFile(byte[] data, string key)
        {
            var list = new List<FileEntity>();
            using (var ms = new MemoryStream(data))
            {
                var count = ms.ReadInt32BE();

                for (int i = 0; i < count; i++)
                {
                    var entity = new FileEntity
                    {
                        Key = ms.ReadUInt32BE(),
                        Offset = ms.ReadInt32BE()
                    };
                    list.Add(entity);
                }

                string location = null;
                byte[] lastData = null;

                for (int i = 0; i < count; i++)
                {
                    var item = list[i];
                    switch (item.Type)
                    {
                        case 0x7E:
                            var compressedSize = item.Hash;
                            var offset = item.Offset + 0x800;

                            ms.Position = offset;
                            using (var zs = new ZlibStream(new MemoryStream(ms.ReadBytes((int)compressedSize)), CompressionMode.Decompress))
                            {
                                zs.Read(lastData, 0, lastData.Length);
                            }
                            break;
                        case 0x7F:
                            if (item.Hash > 0)
                            {
                                lastData = new byte[item.Offset];
                                location = Path.Combine("stage", key, item.Hash.ToString("X6"));
                                if (!Directory.Exists(location))
                                {
                                    Directory.CreateDirectory(location);
                                }
                            }
                            break;
                        case 0x00: break;
                        default:
                            var size = list[i + 1].Offset - item.Offset;
                            var content = new byte[size];
                            Array.Copy(lastData, item.Offset, content, 0, content.Length);
                            var path = Path.Combine(location, $"{item.Hash:X6}.{item.Type:X2}");
                            File.WriteAllBytes(path, content);
                            break;
                    }
                }
            }
        }
    }

    class FileEntity
    {
        public byte Type { get { return (byte)(Key >> 24); } }
        public uint Hash { get { return Key & 0xFFFFFF; } }
        public uint Key { get; set; }
        public int Offset { get; set; }

        public override string ToString()
        {
            return $"Type:{Type:X2} Key:{Hash:X6} Offset:{Offset:n0}";
        }
    }
    class DirectoryEntity
    {
        public int Offset { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Offset:X8} {Name}";
        }
    }
}
