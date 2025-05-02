using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Windows.Media.Imaging;
using System.Buffers.Text;
using System.Xml.Linq;

namespace WPFDeskManager
{
    internal class SerializerHelper
    {
        public static void SaveIconToFile(string json)
        {
            File.WriteAllText("cache/save.json", json);
        }

        public static string? LoadIconFromFile()
        {
            return ReadFile("cache/save.json");
        }

        public static string ImageToCache(BitmapSource bitmap)
        {
            using MemoryStream memStream = new MemoryStream();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memStream);

            string uuid = Guid.NewGuid().ToString();
            string base64 = Convert.ToBase64String(memStream.ToArray());

            WriteFile("cache/" + uuid, base64);

            return uuid;
        }

        private static void WriteFile(string path, string content)
        {
            string? direc = Path.GetDirectoryName(path);

            if (!Directory.Exists(direc))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            }

            File.WriteAllText(path, content);
        }

        private static string? ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllText(path);
        }
    }
}
