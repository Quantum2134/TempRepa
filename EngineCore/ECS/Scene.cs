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
using EngineCore.ECS;
using EngineCore.ECS.Components;
using System.IO;
using MathNet.Numerics.Statistics.Mcmc;
using EngineCore.Core;

namespace EngineCore.ECS
{
    public class Scene
    {
        public string Name { get; set; }
        public Application Application { get; set; }

        public List<Entity> entities;


        public Scene()
        {
            entities = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }       

        public void Render()
        {
            foreach(Entity entity in entities)
            {
                SpriteRenderer sr = entity.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Application.renderer.DrawTexture(sr.Sprite.Texture, sr.Color, entity.transform.Position, entity.transform.Scale, sr.Layer);
                }
            }
        }

        public void Update(float dt)
        {
            foreach (Entity entity in entities)
            {
                entity.transform.Update();
            }
        }       
    }
}
