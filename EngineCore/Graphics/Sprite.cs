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
using Vec4 = System.Numerics.Vector4;
using EngineCore.ECS;
using EngineCore.ECS.Components;
using System.Reflection;
using System.Runtime.Loader;
using System.IO;

namespace EngineCore.Graphics
{
    public class Sprite
    {
        //Replace to SpriteRenderer
        private Vector2[] uv;

        public Texture Texture { get; private set; }
        public Color4 Color { get; set; }
        public int Layer { get; set; }

        public Vector2[] UV => uv;
     
        public Sprite(Texture texture)
        {
            Texture = texture;
            uv = new Vector2[4];
            Color = Color4.White;
            Layer = 0;
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);           
        }     
    }
}
