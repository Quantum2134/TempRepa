using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngineCore.Core
{
    public class Project
    {
        public string Name { get; set; } = "Untitled";
        public string Author { get; set; } = "";
        public string Version { get; set; } = "1.0.0";

        public string ProjectFilePath { get; set; } 
        public string RootDirectory => Path.GetDirectoryName(ProjectFilePath);

        public string EntryScenePath { get; set; } = "Scenes/main.qscene";

        public string ScenesDirectory { get; set; } = "Scenes";
        public string AssetsDirectory { get; set; } = "Assets";
        //public string ScriptsDirectory { get; set; } = "scripts";
        //public string SettingsDirectory { get; set; } = "settings";

        //public BuildSettings BuildSettings { get; set; } = new BuildSettings();

        public string GetFullPath(string relativePath)
        {
            return Path.GetFullPath(Path.Combine(RootDirectory, relativePath));
        }

        public string GetScenesPath(string sceneName)
        {
            return GetFullPath(Path.Combine(ScenesDirectory, sceneName));
        }

        public string GetAssetPath(string assetName)
        {
            return GetFullPath(Path.Combine(AssetsDirectory, assetName));
        }

        public static Project Load(string projectFilePath)
        {
            var json = File.ReadAllText(projectFilePath);
            var project = JsonSerializer.Deserialize<Project>(json);
            project.ProjectFilePath = Path.GetFullPath(projectFilePath);
            return project;
        }

        public static void Save(Project project)
        {
            var json = JsonSerializer.Serialize(project, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(project.ProjectFilePath, json);
        }
    }
}
