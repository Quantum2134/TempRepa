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


namespace EngineCore.Core
{
    public class Application : GameWindow
    {
        public Application(string title, int width, int height) : base(GameWindowSettings.Default, new NativeWindowSettings() { Title = title, ClientSize = new Vector2i(width, height) })
        {
        }

        public GraphicsSystem graphicsSystem;
        public Renderer renderer;
        public Scene Scene { get; private set; }
        public FrameBuffer fbo;

        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;
            UpdateFrequency = 60;

            graphicsSystem = new GraphicsSystem(new Vector2i(0, 0), new Vector2i(ClientSize.X, ClientSize.Y));

            graphicsSystem.GraphicsContext.ShaderManager.LoadShader("DefaultShader", "D:\\Projects\\GameEngine\\EngineEditor\\Resources\\DefaultVertex.vert",
                                                                                     "D:\\Projects\\GameEngine\\EngineEditor\\Resources\\DefaultFragment.frag");
            graphicsSystem.GraphicsContext.ShaderManager.LoadShader("TextureShader", "D:\\Projects\\GameEngine\\EngineEditor\\Resources\\TextureVertex.vert",
                                                                                     "D:\\Projects\\GameEngine\\EngineEditor\\Resources\\TextureFragment.frag");


            renderer = new Renderer(graphicsSystem.GraphicsContext);
            Scene = new Scene(this);



            AssetManager assetmanager = new AssetManager();
            assetmanager.LoadAssets();
            //assetmanager.LoadAssetsFromDirectory("Shaders", ".vert");

            Console.WriteLine(assetmanager.GetAsset<ShaderAsset>("Shader").ShaderSource);


            fbo = new FrameBuffer(ClientSize.X, ClientSize.Y);

            

        }

        protected override void OnUnload()
        {
            base.OnUnload();
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

            

            renderer.Begin(graphicsSystem.Camera);
            DrawEditor();
            Scene.Render();
            FrameBuffer.Unbind();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);







            //editor code
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            graphicsSystem.SetViewport(new Vector2i(0, 0), new Vector2i(ClientSize.X, ClientSize.Y));
        }
    }
}
