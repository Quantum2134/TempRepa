using EngineCore.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class GraphicsSystem
    {
        public Application Application { get; private set; }

        public GraphicsContext GraphicsContext { get; private set; }

        public int ViewportWidth { get; private set; }
        public int ViewportHeight { get; private set; }

        public Camera Camera { get; set; }

        public GraphicsSystem(Application application)
        {
            GraphicsContext = new GraphicsContext(this);

            Application = application;

            ViewportWidth = application.ClientSize.X;
            ViewportHeight = application.ClientSize.Y;

            GL.Viewport(0, 0, ViewportWidth, ViewportHeight);

            Camera = new Camera()
            {
                Position = new Vector2(0, 0),
                AspectRatio = (float)ViewportWidth / ViewportHeight,
                Size = ViewportHeight
            };
        }

        public void SetViewport(int width, int height)
        {

            Camera.AspectRatio = (float)ViewportWidth / ViewportHeight;

            GL.Viewport(0, 0, ViewportWidth, ViewportHeight);
        }
    }
}
