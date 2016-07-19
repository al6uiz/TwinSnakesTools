using System;
using System.IO;
using System.Xml.Serialization;

namespace TwinSnakesTools.Data
{
    public class SerializationHelper
    {
        public static void Save<T>(T contents, string fileName) where T : class
        {
            SerializationHelper<T>.Save(contents, fileName);
        }


        public static void Save<T>(T contents, TextWriter writer) where T : class
        {
            SerializationHelper<T>.Save(contents, writer);
        }
    }

    public class SerializationHelper<T> where T : class
    {
        private static XmlSerializer _serializer;

        static SerializationHelper()
        {
            try
            {
                _serializer = new XmlSerializer(typeof(T));
            }
            catch
            {

            }
        }

        public static T Read(string fileName)
        {
            using (TextReader reader = File.OpenText(fileName))
            {
                return _serializer.Deserialize(reader) as T;
            }
        }

        public static T Read(TextReader reader)
        {
            return _serializer.Deserialize(reader) as T;
        }


        public static void SaveDefault(string fileName)
        {
            var instance = Activator.CreateInstance<T>();

            var fields = typeof(T).GetProperties();

            foreach (var field in fields)
            {
                if (field.PropertyType == typeof(string))
                {
                    if (field.GetValue(instance, null) == null)
                    {
                        field.SetValue(instance, string.Empty, null);
                    }
                }
            }

            Save(instance, fileName);
        }

        public static void Save(T contents, string fileName)
        {
            var location = Path.GetDirectoryName(Path.GetFullPath(fileName));

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            using (var writer = File.CreateText(fileName))
            {
                _serializer.Serialize(writer, contents);
            }
        }

        public static void Save(T contents, TextWriter writer)
        {
            _serializer.Serialize(writer, contents);
        }
    }
}
