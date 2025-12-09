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
using EngineCore.Logging;

using GameScripts;

namespace EngineEditor
{
    internal class RuntimeGame : Application
    {
        public RuntimeGame(string title, int width, int height) : base(title, width, height)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // [AUTOGEN_SCRIPTS]
            //var script1 = new GameScripts.Class1();
            //ScriptSystem.AddScript(script1);



            InitEngineContext(Project.Load(@"C:\Users\malts\Desktop\testproj\MyGame\project.json"));


            


            
            
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
