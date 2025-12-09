using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace EngineEditor.EditorConfiguration
{
    internal class EditorConfigManager
    {
        private static JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
      
        public static EditorConfig LoadOrCreate()
        {
            string path = EditorPaths.EditorConfigPath;

            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var config = JsonSerializer.Deserialize<EditorConfig>(json, _options) ?? new EditorConfig();
                    // Оставляем только существующие проекты
                    config.RecentProjects = config.RecentProjects
                        .Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
                        .Distinct()
                        .ToList();
                    return config;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Config load error: {ex.Message}. Creating new config.");
                }
            }

            // Создаём новый
            var newConfig = new EditorConfig();
            Save(newConfig);
            return newConfig;
        }

        public static void Save(EditorConfig config)
        {
            string json = JsonSerializer.Serialize(config, _options);
            File.WriteAllText(EditorPaths.EditorConfigPath, json);
        }
    }
}
