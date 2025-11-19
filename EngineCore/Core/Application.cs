using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using EngineCore.Graphics;
using EngineCore.ECS;
using EngineCore.Physics;
using EngineCore.Platform.OpenGL;
using System.Diagnostics.CodeAnalysis;
using EngineCore.Physics.Dynamic;
using EngineCore.Utils;
using EngineCore.Physics.Dynamic.Constaints;
using EngineCore.Graphics.GraphicsManagers;
using System.Linq.Expressions;
using EngineCore.Assets;
using EngineCore.Assets.Assets;
using EngineCore.Assets.AssetTypes;
using EngineCore.Logging.Profiling;
using EngineCore.Logging;
using EngineCore.Logging.LogOutputs;


namespace EngineCore.Core
{
    public class Application : GameWindow
    {
        public Application(string title, int width, int height) : base(GameWindowSettings.Default, new NativeWindowSettings() { Title = title, ClientSize = new Vector2i(width, height) })
        {
        }

        public AssetManager AssetManager { get; private set; }

        public GraphicsSystem GraphicsSystem { get; private set; }


        public SceneSystem SceneSystem { get; private set; }


        public Renderer renderer;
        public Scene Scene { get; set; }

        public FrameBuffer fbo;

        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;
            UpdateFrequency = 60;

            Logger.AddLogOutput(new ConsoleLogOutput());

            AssetManager = new AssetManager();

            GraphicsSystem = new GraphicsSystem(this);

            GraphicsSystem.GraphicsContext.ShaderManager.LoadShader("DefaultShader", "Shaders/DefaultVertex.vert", "Shaders/DefaultFragment.frag");
            GraphicsSystem.GraphicsContext.ShaderManager.LoadShader("TextureShader", "Shaders/TextureVertex.vert", "Shaders/TextureFragment.frag");

            SceneSystem = new SceneSystem(this);


            renderer = new Renderer(GraphicsSystem.GraphicsContext);
            Scene = new Scene();
            Scene.Application = this;
            Scene.Name = "Scene1";

            


            fbo = new FrameBuffer(ClientSize.X, ClientSize.Y);

            

        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            AssetManager?.Dispose();
        }

        protected virtual void DrawEditor()
        { }
        
       
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);            

            Title = $"Editor {MathF.Round(1f / (float)args.Time)} FPS";

            Scene.Update((float)args.Time);
        }
        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);




            fbo.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            

            renderer.Begin(GraphicsSystem.Camera);
            DrawEditor();
            SceneSystem.CurrentScene.Render();
            FrameBuffer.Unbind();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);







            //editor code
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GraphicsSystem.SetViewport(ClientSize.X, ClientSize.Y);
        }
    }
}
