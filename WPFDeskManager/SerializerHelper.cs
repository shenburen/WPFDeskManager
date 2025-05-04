using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

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
            File.WriteAllText(GetAbsolutePath("cache/save.json"), json);
        }

        /// <summary>
        /// 加载图标信息
        /// </summary>
        /// <returns>图标信息JSON</returns>
        public static string? LoadIconFromFile()
        {
            return ReadFile(GetAbsolutePath("cache/save.json"));
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

            WriteFile(GetAbsolutePath("cache/" + uuid), base64);

            return uuid;
        }

        /// <summary>
        /// 从缓存中读取图像
        /// </summary>
        /// <param name="name">文件名字</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage? ImageFromCache(string name)
        {
            string? base64 = ReadFile(GetAbsolutePath("cache/" + name));
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
        /// 删除缓存文件
        /// </summary>
        /// <param name="name">文件名字</param>
        public static void DeleteCache(string name)
        {
            string path = GetAbsolutePath("cache/" + name);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
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

        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns>绝对路径</returns>
        private static string GetAbsolutePath(string path)
        {
            string? exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exeDir == null)
            {
                return path;
            }

            return Path.Combine(exeDir, path);
        }
    }
}
