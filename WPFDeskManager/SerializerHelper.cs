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
        /// <summary>
        /// 保存图标信息
        /// </summary>
        /// <param name="json">图标信息JSON</param>
        public static void SaveIconToFile(string json)
        {
            File.WriteAllText("cache/save.json", json);
        }

        /// <summary>
        /// 加载图标信息
        /// </summary>
        /// <returns>图标信息JSON</returns>
        public static string? LoadIconFromFile()
        {
            return ReadFile("cache/save.json");
        }

        /// <summary>
        /// 将BitmapSource转换为Base64字符串并保存到文件
        /// </summary>
        /// <param name="bitmap">BitmapSource</param>
        /// <returns>文件名字</returns>
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

        /// <summary>
        /// 从缓存中读取图像
        /// </summary>
        /// <param name="name">文件名字</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage? ImageFromCache(string name)
        {
            string? base64 = ReadFile("cache/" + name);
            if (base64 == null)
            {
                return null;
            }

            using MemoryStream memStream = new MemoryStream(Convert.FromBase64String(base64));
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = memStream;
            bitmap.EndInit();

            return bitmap;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        private static void WriteFile(string path, string content)
        {
            string? direc = Path.GetDirectoryName(path);

            if (!Directory.Exists(direc))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            }

            File.WriteAllText(path, content);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>内容</returns>
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
