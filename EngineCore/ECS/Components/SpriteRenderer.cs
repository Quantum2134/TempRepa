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
    public class SpriteRenderer : Component
    {
        public Sprite Sprite { get; set; }
        public int Layer { get; set; }
        public Color4 Color { get; set; }

        public SpriteRenderer(Sprite sprite)
        {
            Sprite = sprite;
            Color = Color4.White;
        }
    }
}
