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
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using Vec2 = System.Numerics.Vector2;


namespace EngineCore.ECS.Components
{
    public class TextureRenderer : Component
    {
        private bool flipX;
        private bool flipY;

        private Vector2[] uv;

        public Texture Texture { get; set; }
        public Color4 Color { get; set; }
        public int Layer {  get; set; }
        public Vector2[] UV { get => uv; set => uv = value; }

        public bool FlipX
        {
            get => flipX;
            set
            {
                if(value ==  flipX) return;
                flipX = value;

                (UV[0], UV[3]) = (uv[3], uv[0]);
                (UV[1], UV[2]) = (uv[2], uv[1]);

                

            }
        }

        public bool FlipY
        {
            get => flipY;
            set
            {
                flipY = value;
                (UV[0], UV[1]) = (uv[1], UV[0]);
                (UV[3], UV[2]) = (uv[2], UV[3]);
            }
        }

        public TextureRenderer()
        {
            Color = Color4.White;
            Layer = 0;

            uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);

            UV = uv;
        }

        
    }
}