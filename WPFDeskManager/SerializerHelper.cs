using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

namespace WPFDeskManager
{
    internal class SerializerHelper
    {
        public static void SaveToFile(string path, Serialization data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }

        public static Serialization LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            return JsonSerializer.Deserialize<Serialization>(json, options)!;
        }
    }
}
