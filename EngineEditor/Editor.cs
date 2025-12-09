using EngineCore.Assets.Assets;
using EngineCore.Assets.AssetTypes;
using EngineCore.Core;
using EngineCore.ECS;
using EngineCore.ECS.Components;
using EngineCore.Graphics;
using EngineCore.Logging;
using EngineCore.Logging.LogOutputs;
using EngineCore.Logging.Profiling;
using EngineCore.Platform.OpenGL;
using EngineEditor.EditorConfiguration;
using EngineEditor.Platform.Windows;
using EngineEditor.ProjectSystem;
using EngineEditor.UI;
using EngineEditor.UI.EditorPanels;
using EngineEditor.Utils;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Platform.Imgui;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EngineEditor.GameBuild;

using Microsoft.Win32;



namespace EngineEditor
{
    internal class Editor : Application
    {       
        public Editor(string title, int width, int height) : base(title, width, height)
        {
        }

        private EditorConfig EditorConfig = null;
        private Project? CurrentProject;
        private bool showStartup = true;

        StartupDialog StartupDialog;

        public ImGuiController ImGuiController;

        public AssetsPanel AssetsPanel;
        public ConsolePanel ConsolePanel;
        public HierarchyPanel HierarchyPanel;
        public InspectorPanel InspectorPanel;
        public ViewportPanel ViewportPanel;
        public MenuBarPanel MenuBarPanel;

        public FrameBuffer fbo;

        GameBuilder GameBuilder;

        protected override void OnLoad()
        {
            StartupDialog = new StartupDialog();

            EditorConfig = EditorConfigManager.LoadOrCreate();            
            showStartup = true;


            InitGui();
            //AssetSystem.ResourcesPath = CurrentProject.ProjectFilePath; //проекта пока нет!
            base.OnLoad();   //к этому моменту уже должен быть известен путь!        

            GameBuilder = new GameBuilder(@"D:\Projects\GameEngine\EngineRuntime\EngineRuntime.csproj", "C:\\Users\\malts\\Desktop\\testproj\\MyGame\\Assets");

            fbo = new FrameBuffer(ClientSize.X, ClientSize.Y);
        }
        public void OpenProject(string projectPath)
        {
            var project = Project.Load(projectPath); // из EngineCore
            UpdateRecentProjects(projectPath);
            CurrentProject = project;
            showStartup = false;
            Console.WriteLine($"Project opened: {project.Name}");


            InitEngineContext(project);

            HierarchyPanel.SetScene(SceneSystem.CurrentScene);
            try
            {
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open project: {ex.Message}");
            }

        }

        private void UpdateRecentProjects(string projectPath)
        {
            EditorConfig.RecentProjects.RemoveAll(p => p == projectPath);
            EditorConfig.RecentProjects.Insert(0, projectPath);
            if (EditorConfig.RecentProjects.Count > 10)
                EditorConfig.RecentProjects.RemoveAt(10);
            EditorConfig.LastOpenedProject = projectPath;
            EditorConfigManager.Save(EditorConfig);
        }

        public void CreateAndOpenProject(string name, string destinationDir)
        {
            if (ProjectCreator.CreateFromTemplate(name, destinationDir))
            {
                string projectFile = Path.Combine(destinationDir, name, "project.json");
                OpenProject(projectFile);
            }
        }


        private void InitGui()
        {
            ImGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);

            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;


            AssetsPanel = new AssetsPanel(this);
            ConsolePanel = new ConsolePanel();
            HierarchyPanel = new HierarchyPanel();
            InspectorPanel = new InspectorPanel(this);
            MenuBarPanel = new MenuBarPanel();

            HierarchyPanel.OnEntitySelected += HierarchyPanel_OnEntitySelected;          

            MenuBarPanel.OnSaveScene += MenuBarPanel_OnSaveScene;
            MenuBarPanel.OnOpenScene += MenuBarPanel_OnOpenScene;
        }

        private void MenuBarPanel_OnOpenScene()
        {
            var sourceFiles = FileDialogWindows.OpenFile(
                    title: "Select scene",
                    filter: "All files (*.*)\0*.*\0"
                );

            if( sourceFiles != null )
            {
                SceneSystem.LoadScene(sourceFiles);
                HierarchyPanel.SetScene(SceneSystem.CurrentScene);

            }

        }

        private void MenuBarPanel_OnSaveScene()
        {
            SceneSystem.SaveCurrentScene();
        }      

        private void HierarchyPanel_OnEntitySelected(Entity obj)
        {
            InspectorPanel.SetSelectedEntity(obj);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);          
        }
        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            fbo.Bind();
            base.OnRenderFrame(args);
            FrameBuffer.Unbind();

            ImGuiController.Update(this, (float)args.Time);
            ImGui.DockSpaceOverViewport();

            if (showStartup)
            {
                StartupDialog.Show(EditorConfig, OpenProject, CreateAndOpenProject);
            }
            else
            {
                AssetsPanel.Render();
                ConsolePanel.Render();
                InspectorPanel.Render();
                HierarchyPanel.Render();
                MenuBarPanel.Render();
                Viewport();
            }

            ImGui.Begin("Builder");
            if(ImGui.Button("Build"))
            {
                SceneSystem.SaveCurrentScene();

                GameBuilder.Build("C:\\Users\\malts\\Desktop\\game", ScriptSystem.Scripts.Values.ToArray());
            }
            ImGui.End();

            ImGuiController.Render();

            SwapBuffers();
            
        }

        private void Viewport()
        {
            ImGui.Begin("Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            ImGui.Image(fbo.ColorTexture.Handle, new System.Numerics.Vector2(ClientSize.X, ClientSize.Y), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            if (ImGui.IsWindowHovered())
            {
                if (MouseState.IsButtonDown(MouseButton.Left))
                {
                    GraphicsSystem.Camera.Position -= new Vector2(MouseState.Delta.X, -MouseState.Delta.Y) * GraphicsSystem.Camera.Size / 1061;

                }
                GraphicsSystem.Camera.Size -= MouseState.ScrollDelta.Y * 20;

            }
            ImGui.End();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            ImGuiController.WindowResized(e.Width, e.Height);
        }        
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            ImGuiController.PressChar((char)e.Unicode);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            ImGuiController.MouseScroll(e.Offset);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            SceneSystem.SaveCurrentScene();
        }
    }
}
  