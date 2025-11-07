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
        
        ImGuiController controller;

        private List<EditorPanel> editorPanels;
        private HierarchyPanel hierarchyPanel;
        private InspectorPanel inspectorPanel;
        private ConsolePanel consolePanel;
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
            hierarchyPanel = new HierarchyPanel(Scene);
            inspectorPanel = new InspectorPanel();
            consolePanel = new ConsolePanel();

            editorPanels.Add(hierarchyPanel);
            editorPanels.Add(inspectorPanel);
            editorPanels.Add(consolePanel);
        }
        private void Init()
        {
            Texture texture = new Texture(@"D:\Projects\GameEngine\EngineEditor\Resources\Textures\Red-Circle-Transparent.png");           
            Sprite sprite = new Sprite(texture);
            sprite.Color = Color4.Red;
            SpriteRenderer sr = new SpriteRenderer(sprite);

            C = new Entity("C");
            C.transform.Scale = new Vector2(3, 3);
            C.AddComponent(sr);
            Scene.AddEntity(C);


            A = new Entity("A");
            A.transform.Scale = new Vector2(3, 3);
            A.AddComponent(sr);
            Scene.AddEntity(A);


            B = new Entity("B");
            B.transform.Scale = new Vector2(3, 3);
            B.AddComponent(sr);
            Scene.AddEntity(B);
            
            t = 0;
            onSim = true;

            graphicsSystem.Camera.Size = 360f;

            graphicsSystem.GraphicsContext.StateManager.SetClearColor(new Color4(150, 150, 150, 255));
        }

        protected override void DrawEditor()
        {
            base.DrawEditor();

            renderer.DrawWidthLine(C.transform.Position, B.transform.Position, 0.5f, Color4.Gray, -2);
            renderer.DrawWidthLine(A.transform.Position, B.transform.Position, 0.5f, Color4.Gray, -2);

            renderer.DrawLine(new Vector2(0, -500), new Vector2(0, 500), Color4.Black, -100);
            renderer.DrawLine(new Vector2(-5000, 0), new Vector2(5000, 0), Color4.Black, -100);
            renderer.DrawLine(new Vector2(0, 0), new Vector2(MathF.Cos(MathHelper.DegreesToRadians(60)), MathF.Sin(MathHelper.DegreesToRadians(60))) * 70, Color4.Black, -100);
            renderer.DrawArrow(C.transform.Position, Cvel(t) * 100, Color4.Red, -5);
            renderer.DrawArrow(A.transform.Position, Avel(t) * 100, Color4.Red, -5);
            renderer.DrawArrow(B.transform.Position, vb * 100, Color4.Red, -5);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);            
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            

            base.OnRenderFrame(args);
            

            

            if (t <= 1f && onSim)
            {
                C.transform.Position = Cpoint(t) * 100;
                A.transform.Position = Apoint(t) * 100;
                B.transform.Position = Bpoint(t) * 100;
                t += (float)args.Time / 4;

                Vector2 b = Bpoint(t);
                Vector2 b2 = Bpoint(t + (float)args.Time);
                vb = (b2 - b) / (float)args.Time;
            }

            if (KeyboardState.IsKeyPressed(Keys.Q))
            {
                onSim = !onSim;
                Logger.Log("Trace", LogLevel.Trace);
                Logger.Log("Info", LogLevel.Info);
                Logger.Log("Warning", LogLevel.Warning);
                Logger.Log("Error", LogLevel.Error);
            }






            controller.Update(this, (float)args.Time);

            ImGui.DockSpaceOverViewport();


            hierarchyPanel.Render();
            inspectorPanel.Render();
            inspectorPanel.SetSelectedEntity(hierarchyPanel.GetSelectedEntity());
            consolePanel.Render();
            ViewportPanel();

            

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
                    graphicsSystem.Camera.Position -= new Vector2(MouseState.Delta.X, -MouseState.Delta.Y) * graphicsSystem.Camera.Size / 1061;

                }
                graphicsSystem.Camera.Size -= MouseState.ScrollDelta.Y * 20;

                if (MouseState.IsButtonPressed(MouseButton.Right))
                {
                    Logger.Log($"{ClientSize.ToString()}, {graphicsSystem.Camera.Size}", LogLevel.Trace);

                }

            }
            ImGui.End();
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



        private Vector2 Cpoint(float t)
        {
            float x = 0.075f * (t * t + 1);
            float y = 0.15f * MathF.Sqrt(3) / 2 * (t * t + 1);

            

            return new Vector2(x, y); //100 pixels = 1m
        }
        private Vector2 Cvel(float t)
        {
            return new Vector2(0.15f * t, 0.15f * MathF.Sqrt(3) * t);
        }

        private Vector2 Apoint(float t)
        {
            float x = 0.05f * t * t * t + 0.25f;
            float y = 0;

            return new Vector2(x, y); //100 pixels = 1m
        }
        private Vector2 Avel(float t)
        {
            return new Vector2(0.15f * t * t, 0);
        }

        private Vector2 Bpoint(float t)
        {
            Vector2 c = Cpoint(t);
            Vector2 a = Apoint(t);

            Vector2 d = a - c;
            float dlength = d.Length;

            Vector2 center = (a + c) / 2;

            Vector2 ex = d / dlength;
            Vector2 ey = new Vector2(-ex.Y, ex.X);

            float hsqr = 0.3f * 0.3f - (dlength * dlength * 0.25f);
            float h = MathF.Sqrt(MathF.Max(0, hsqr));



            return center + h * ey;
        }       
    }
}
  