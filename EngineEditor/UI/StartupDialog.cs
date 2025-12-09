using EngineCore.ECS;
using EngineEditor.EditorConfiguration;
using EngineEditor.Platform.Windows;
using EngineEditor.UI.EditorPanels;
using EngineEditor.Utils;
using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.UI
{
    internal class StartupDialog
    {
        private string _newProjectName = "MyGame";
        private string _newProjectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MyGame");

        public bool Show(EditorConfig config, Action<string> onOpenProject, Action<string, string> onCreateProject)
        {
            ImGui.SetNextWindowSize(new Vector2(520, 420).ToNumerics(), ImGuiCond.FirstUseEver);
            ImGui.Begin(" Quantum Editor — Welcome", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            ImGui.Text("Start a new project or open an existing one");

            // === Создание нового проекта ===
            ImGui.SeparatorText("New Project");
            ImGui.InputText("Name", ref _newProjectName, 64);
            //ImGui.InputText("Path", ref _newProjectPath, 256);
            if (ImGui.Button("Create Project", new Vector2(150, 30).ToNumerics()))
            {
                if (!string.IsNullOrWhiteSpace(_newProjectName) && !string.IsNullOrWhiteSpace(_newProjectPath))
                {
                    var sourceFiles = FileDialogWindows.SelectFolder(
                    title: "Select project directory"
                    );

                    if (sourceFiles != null)
                    {

                        onCreateProject(_newProjectName, sourceFiles);

                    }
                }
            }

            ImGui.Separator();

            // === Последний проект ===
            //if (!string.IsNullOrEmpty(config.LastOpenedProject) && File.Exists(config.LastOpenedProject))
            //{
            //    string name = Path.GetFileNameWithoutExtension(config.LastOpenedProject);
            //    if (ImGui.Button($"Continue: {name}", new Vector2(200, 30).ToNumerics()))
            //    {
            //        onOpenProject(config.LastOpenedProject);
            //    }
            //}

            // === Недавние проекты ===
            if (config.RecentProjects.Count > 0)
            {
                ImGui.SeparatorText("Recent Projects");
                var recent = config.RecentProjects.ToList();
                foreach (var path in recent)
                {
                    if (!File.Exists(path)) continue;
                    string name = Path.GetFileNameWithoutExtension(path);
                    if (ImGui.Button($"{name}", new Vector2(200, 25).ToNumerics()))
                    {
                        onOpenProject(path);
                    }
                }
            }

            bool keepOpen = true;
            ImGui.End();

            // Закрываем окно, если проект открыт (но это управляет EditorApp)
            return keepOpen;
        }
    }
}
