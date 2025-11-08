using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using EngineCore.Platform.OpenGL;
using EngineCore.Graphics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using EngineCore.Core;
using EngineCore.ECS;
using EngineCore.ECS.Components;
using ImGuiNET;
using System.Runtime.InteropServices;

using Platform.Imgui;
using EngineEditor.EditorPanels;

using EngineCore.Logging;
using EngineCore.Logging.LogOutputs;
using EngineCore.Logging.Profiling;
using EngineCore.Assets.Assets;
using System.IO;
using EngineEditor.Platform.Windows;



namespace EngineEditor
{
    internal class Editor : Application
    {
        public Editor(string title, int width, int height) : base(title, width, height)
        {
        }
        Entity C;
        Entity A;
        Entity B;
        float t;
        bool onSim;
        Vector2 vb;
        Vector2 ab;
        
        ImGuiController controller;

        private List<EditorPanel> editorPanels;
        private HierarchyPanel hierarchyPanel;
        private InspectorPanel inspectorPanel;
        private ConsolePanel consolePanel;
        private MenuBarPanel menuBarPanel;
        private PlaybackControls panelControls;

        private bool var;
        private float tim;
        private float scale;

        Texture texture;

        Scene scene;

        protected override void OnLoad()
        {
            base.OnLoad();
                   
            controller = new ImGuiController(ClientSize.X, ClientSize.Y);


            Init();
            InitGui();
            
        }
        private void InitGui()
        {
            editorPanels = new List<EditorPanel>();
            hierarchyPanel = new HierarchyPanel(scene);
            inspectorPanel = new InspectorPanel();
            consolePanel = new ConsolePanel();
            menuBarPanel = new MenuBarPanel();
            menuBarPanel.OnSaveScene += SaveScene;
            menuBarPanel.OnOpenScene += OpenScene;
            panelControls = new PlaybackControls();
            panelControls.OnStep += PanelControls_OnStep;
            panelControls.OnResume += PanelControls_OnResume;
            panelControls.OnStop += PanelControls_OnStop;
            panelControls.OnPause += PanelControls_OnPause;
            panelControls.OnPlay += PanelControls_OnPlay;

            editorPanels.Add(hierarchyPanel);
            editorPanels.Add(inspectorPanel);
            editorPanels.Add(consolePanel);
        }

        private void PanelControls_OnPlay()
        {
            onSim = true;
        }

        private void PanelControls_OnPause()
        {
            onSim = false;
        }

        private void PanelControls_OnStop()
        {
            onSim = false;
        }

        private void PanelControls_OnResume()
        {
            onSim = true;
        }

        private void PanelControls_OnStep()
        {
            
        }

        private void Init()
        {
            texture = GraphicsSystem.GraphicsContext.TextureManager.LoadTexture("Red-Circle-Transparent");
            Sprite sprite = new Sprite(texture);
            SpriteRenderer sr = new SpriteRenderer(sprite);

            scene = SceneSystem.NewScene("Scene1");


            C = new Entity("C");
            C.transform.Scale = new Vector2(3, 3);
            C.AddComponent(sr);
            scene.AddEntity(C);


            A = new Entity("A");
            A.transform.Scale = new Vector2(3, 3);
            A.AddComponent(sr);
            scene.AddEntity(A);


            B = new Entity("B");
            B.transform.Scale = new Vector2(3, 3);
            B.AddComponent(sr);
            scene.AddEntity(B);
            
            t = 0;
            tim = 1f;
            scale = 4f;
            onSim = false;

            GraphicsSystem.Camera.Size = 360f;

            GraphicsSystem.GraphicsContext.StateManager.SetClearColor(new Color4(150, 150, 150, 255));

        }

        protected override void DrawEditor()
        {
            base.DrawEditor();

            //shared
            renderer.DrawWidthLine(scene.entities.Find(e => e.Name == "C").transform.Position, scene.entities.Find(e => e.Name == "B").transform.Position, 0.5f, Color4.Gray, -2);
            renderer.DrawWidthLine(scene.entities.Find(e => e.Name == "A").transform.Position, scene.entities.Find(e => e.Name == "B").transform.Position, 0.5f, Color4.Gray, -2);
            renderer.DrawLine(new Vector2(0, -500), new Vector2(0, 500), Color4.Black, -100);
            renderer.DrawLine(new Vector2(-5000, 0), new Vector2(5000, 0), Color4.Black, -100);

            //egor
            if(!var)
            {
                renderer.DrawLine(new Vector2(0, 0), new Vector2(MathF.Cos(MathHelper.DegreesToRadians(60)), MathF.Sin(MathHelper.DegreesToRadians(60))) * 70, Color4.Black, -100);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "C").transform.Position, EgorkaVariant.Cvel(t) * 100, Color4.Red, -5);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "A").transform.Position, EgorkaVariant.Avel(t) * 100, Color4.Red, -5);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "B").transform.Position, vb * 100, Color4.Red, -5);
            }
            else
            {
                renderer.DrawLine(new Vector2(-150, 50), new Vector2(50, 50), Color4.Black, -100);
                renderer.DrawCircle(new Vector2(-50, -50), 50f, Color4.Black, -100, 45);

                renderer.DrawArrow(scene.entities.Find(e => e.Name == "C").transform.Position, QuantumVariant.Cvel(t) * 100, Color4.Red, -5);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "A").transform.Position, QuantumVariant.Avel(t) * 100, Color4.Red, -5);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "B").transform.Position, vb * 100, Color4.Red, -5);

                renderer.DrawArrow(scene.entities.Find(e => e.Name == "C").transform.Position, QuantumVariant.Cacc(t) * 100, Color4.Blue, -5);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "A").transform.Position, QuantumVariant.Aacc(t) * 100, Color4.Blue, -5);
                renderer.DrawArrow(scene.entities.Find(e => e.Name == "B").transform.Position, ab * 100, Color4.Blue, -5);


                QuantumVariant.Mcs(QuantumVariant.Cpoint(t), QuantumVariant.Cvel(t), QuantumVariant.Bpoint(t), vb, out Vector2 p);
                renderer.DrawTexture(texture, Color4.Red, p * 100, new Vector2(5, 5), 0);
                //Logger.Log((p*100).ToString(), LogLevel.Trace);

                QuantumVariant.Mcu(QuantumVariant.Cpoint(t), QuantumVariant.Cacc(t), QuantumVariant.Bpoint(t), ab, out Vector2 q);
                renderer.DrawTexture(texture, Color4.Blue, q * 100, new Vector2(5, 5), 0);
                Logger.Log((q * 100).ToString(), LogLevel.Trace);
            }

        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);            

            
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            

            base.OnRenderFrame(args);

            if(!var)
            {
                scene.entities.Find(e => e.Name == "C").transform.Position = EgorkaVariant.Cpoint(t) * 100;
                scene.entities.Find(e => e.Name == "A").transform.Position = EgorkaVariant.Apoint(t) * 100;
                scene.entities.Find(e => e.Name == "B").transform.Position = EgorkaVariant.Bpoint(t) * 100;
                Vector2 b = EgorkaVariant.Bpoint(t);
                Vector2 b2 = EgorkaVariant.Bpoint(t + (float)args.Time);
                vb = (b2 - b) / (float)args.Time;
            }
            else
            {
                scene.entities.Find(e => e.Name == "C").transform.Position = QuantumVariant.Cpoint(t) * 100;
                scene.entities.Find(e => e.Name == "A").transform.Position = QuantumVariant.Apoint(t) * 100;
                scene.entities.Find(e => e.Name == "B").transform.Position = QuantumVariant.Bpoint(t) * 100;             
                vb = QuantumVariant.Bvel(t, (float)args.Time);
                ab = QuantumVariant.Bacc(t, (float)args.Time);
            }



            if (t <= tim && onSim)
            {
                t += (float)args.Time / scale;
            }                      


            controller.Update(this, (float)args.Time);

            ImGui.DockSpaceOverViewport();


            hierarchyPanel.Render();
            inspectorPanel.Render();
            inspectorPanel.SetSelectedEntity(hierarchyPanel.GetSelectedEntity());
            consolePanel.Render();
            menuBarPanel.Render();
            ViewportPanel();

            panelControls.Render();

            if(panelControls.IsStopped())
            {
                ImGui.Checkbox("Variant", ref var);
            }
            if(panelControls.IsPlaying())
            {
                

            }
            ImGui.SliderFloat("Time", ref t, 0, tim);
            ImGui.InputFloat("Max time", ref tim);
            ImGui.InputFloat("Time scale", ref scale);


            ImGui.End();



            controller.Render();

            SwapBuffers();
        }


        private void ViewportPanel()
        {
            ImGui.Begin("Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.MenuBar);
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

        private void SaveScene()
        {
            SceneSystem.SaveCurrentScene();
        }
        private void OpenScene()
        {
            var path = FileDialogWindows.OpenFile(
                title: "Select scene",
                filter: "JSON files (*.json)\0*.json\0All files (*.*)\0*.*\0"
            );

            if (path != null)
            {
                scene = SceneSystem.LoadScene(path);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            controller.WindowResized(e.Width, e.Height);
            
            Logger.Log($"Resized {ClientSize.X}, {ClientSize.Y}", LogLevel.Trace);
        }        
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            controller.PressChar((char)e.Unicode);        
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            controller.MouseScroll(e.Offset);
        }       
    }
}
  