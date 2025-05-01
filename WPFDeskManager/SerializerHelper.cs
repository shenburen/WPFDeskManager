using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

namespace WPFDeskManager
{
    internal class SerializerHelper
    {
        public static bool SaveToFile()
        {
            Serialization serialization = Build();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            string json = JsonSerializer.Serialize(serialization, options);
            File.WriteAllText("icons.json", json);

            return true;
        }

        public static Serialization LoadFromFile(string path)
        {
            string json = File.ReadAllText(path);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            return JsonSerializer.Deserialize<Serialization>(json, options)!;
        }

        private static Serialization Build()
        {
            Dictionary<int, IconBoxInfo> infos = new Dictionary<int, IconBoxInfo>(Global.IconBoxInfos);
            Serialization serialization = new Serialization();

            Dictionary<int, IconBoxInfo> list = infos.Where(info => info.Value.IsRoot).ToDictionary(info => info.Key, info => info.Value);
            foreach (var item in list)
            {

                IconSerialization icon = new IconSerialization
                {
                    id = item.Key,
                    CenterX = item.Value.CenterX,
                    CenterY = item.Value.CenterY,
                    IconType = item.Value.IconType,
                    SvgName = item.Value.SvgName,
                    TargetPath = item.Value.TargetPath,
                    IsRoot = item.Value.IsRoot,
                    Image = item.Value.IconImage?.Source,
                };
                serialization.Icons.Add(icon);
            }

            return serialization;
        }
    }
}
