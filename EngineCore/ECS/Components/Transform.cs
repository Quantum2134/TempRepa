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
using Vec2 = System.Numerics.Vector2;


namespace EngineCore.ECS.Components
{
    public class Transform
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public Matrix4 Matrix { get; private set; }

        public Transform()
        {
            Position = Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;

            Matrix = Matrix4.Identity;
        }

        public void Update()
        {
            Matrix = Matrix4.CreateTranslation(Position.X, Position.Y, 0) *
                     Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation)) *
                     Matrix4.CreateScale(Scale.X, Scale.Y, 1);

        }
    }
}
