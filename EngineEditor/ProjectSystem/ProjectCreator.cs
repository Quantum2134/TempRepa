using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineEditor.EditorConfiguration;

using EngineCore.Core;

namespace EngineEditor.ProjectSystem
{
    internal static class ProjectCreator
    {
        public static bool CreateFromTemplate(string projectName, string destinationDirectory)
        {
            string templateDir = Path.Combine(EditorPaths.ProjectTemplatesPath, "DefaultProjectTemplate");
            if (!Directory.Exists(templateDir))
            {
                Console.WriteLine($"Template not found: {templateDir}");
                return false;
            }

            string projectDir = Path.Combine(destinationDirectory, projectName);
            if (Directory.Exists(projectDir))
            {
                Console.WriteLine($"Project directory already exists: {projectDir}");
                return false;
            }

            try
            {
                CopyDirectory(templateDir, projectDir);

                // Обновляем project.qproj
                string projectFile = Path.Combine(projectDir, "project.json");
                if (!File.Exists(projectFile))
                {
                    Console.WriteLine("project.qproj not found in template!");
                    return false;
                }

                var project = Project.Load(projectFile); // твой метод из EngineCore
                project.Name = projectName;
                project.ProjectFilePath = projectFile;
                Project.Save(project);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create project: {ex.Message}");
                return false;
            }
        }

        // Простая реализация копирования директории
        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, targetFile, true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string targetDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, targetDir);
            }
        }
    }
}
