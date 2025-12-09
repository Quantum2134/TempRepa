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
using EngineCore.Assets;
using EngineCore.Logging;
using EngineCore.Logging.LogOutputs;
using EngineCore.Assets.Assets;
using EngineCore.Platform.OpenGL;
using EngineCore.ECS.Components;
using System.Linq.Expressions;
using System.IO;
using EngineCore.Assets.AssetTypes;
using EngineCore.Scripts;
using EngineCore.Input;


namespace EngineCore.Core
{
    public class Application : GameWindow
    {
        public Application(string title, int width, int height) : base(GameWindowSettings.Default, new NativeWindowSettings() { Title = title, ClientSize = new Vector2i(width, height) })
        {
        }

        public AssetSystem AssetSystem { get; set; }
        public GraphicsSystem GraphicsSystem { get; set; }
        public SceneSystem SceneSystem { get; set; }
        public ScriptSystem ScriptSystem { get; set; }

        public Renderer Renderer { get; set; }
        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;
            UpdateFrequency = 60f;

            Logger.AddLogOutput(new ConsoleLogOutput());

            AssetSystem = new AssetSystem();
            GraphicsSystem = new GraphicsSystem(this);          
            SceneSystem = new SceneSystem(this);
            ScriptSystem = new ScriptSystem();

            ScriptSystem.LoadScriptAssembly(@"C:\Users\malts\Desktop\GameTestScripts\GameScripts\bin\Debug\net8.0\GameScripts.dll");
            InputSystem.Application = this;
        }

        public void InitEngineContext(Project project)
        {
            AssetSystem.ResourcesPath = Path.Combine(project.RootDirectory, project.AssetsDirectory);
            AssetSystem.LoadAssets();

            GraphicsSystem = new GraphicsSystem(this);
            GraphicsSystem.GraphicsContext.ShaderManager.LoadShader("DefaultShader", AssetSystem.GetAsset<ShaderAsset>("DefaultVertex"),
                                                                                     AssetSystem.GetAsset<ShaderAsset>("DefaultFragment"));
            GraphicsSystem.GraphicsContext.ShaderManager.LoadShader("TextureShader", AssetSystem.GetAsset<ShaderAsset>("TextureVertex"),
                                                                                     AssetSystem.GetAsset<ShaderAsset>("TextureFragment"));

            foreach (Asset asset in AssetSystem.Assets)
            {
                if (asset.Type == AssetType.Texture)
                {
                    GraphicsSystem.GraphicsContext.TextureManager.LoadTexture((TextureAsset)asset);
                }
            }

            Renderer = new Renderer(GraphicsSystem.GraphicsContext);


            SceneSystem.LoadScene(Path.Combine(project.RootDirectory, project.AssetsDirectory, project.ScenesDirectory, project.EntryScenePath));
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }       
        
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            SceneSystem.CurrentScene?.Update((float)args.Time);
        }
        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Renderer?.Begin(GraphicsSystem.Camera);
            SceneSystem.CurrentScene?.Render();
           
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GraphicsSystem.SetViewport(e.Width, e.Height);
        }
    }
}
