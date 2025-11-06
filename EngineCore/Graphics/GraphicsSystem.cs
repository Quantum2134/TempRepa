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
        public GraphicsContext GraphicsContext { get; private set; }

        public Vector2i ViewportPos { get; private set; }
        public Vector2i ViewportSize { get; private set; }

        public Camera Camera { get; set; }

        public GraphicsSystem(Vector2i viewportPos, Vector2i viewportSize)
        {
            GraphicsContext = new GraphicsContext();

            ViewportPos = viewportPos;
            ViewportSize = viewportSize;

            GL.Viewport(ViewportPos.X, ViewportPos.Y, ViewportSize.X, ViewportSize.Y);

            Camera = new Camera()
            {
                Position = new Vector2(0, 0),
                AspectRatio = (float)viewportSize.X / viewportSize.Y,
                Size = viewportSize.Y
            };
        }

        public void SetViewport(Vector2i viewportPos, Vector2i viewportSize)
        {
            ViewportPos = viewportPos;
            ViewportSize = viewportSize;

            Camera.AspectRatio = (float)ViewportSize.X / ViewportSize.Y;

            GL.Viewport(ViewportPos.X, ViewportPos.Y, ViewportSize.X, ViewportSize.Y);
        }
    }
}
