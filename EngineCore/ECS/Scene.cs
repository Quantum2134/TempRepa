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
using EngineCore.Core;

namespace EngineCore.ECS
{
    public class Scene
    {
        public string Name { get; set; }

        private List<Entity> entities;

        private List<Entity> entitiesToAdd;
        private List<Entity> entitiesToRemove;


        public Application application;

        public List<Entity> Entities => entities;

        public Scene()
        {
            entities = new List<Entity>();
            entitiesToAdd = new List<Entity>();
            entitiesToRemove = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            entitiesToAdd.Add(entity);
        }
        public void RemoveEntity(Entity entity)
        {
            entitiesToRemove.Add(entity);
        }
        public Entity FindEntityByName(string name)
        {
            return entities.Find(x => x.Name == name);
        }

        private void Execute()
        {
            foreach(Entity entity in entitiesToAdd)
            {
                entities.Add(entity);
            }
            foreach (Entity entity in entitiesToRemove)
            {
                entities.Remove(entity);
            }
        }
        private void Pop()
        {
            entitiesToAdd.Clear();
            entitiesToRemove.Clear();
        }

        public void Update(float dt)
        {

            Execute();
            Pop();

            foreach (Entity entity in entities)
            {
                entity.Transform.Update();

                var scripts = entity.GetComponents<ScriptComponent>();
                foreach(var script in scripts)
                {
                    script.Update(dt);
                }
            }
        }

        public void Render()
        {
            foreach(Entity entity in entities)
            {
                TextureRenderer[] tr = entity.GetComponents<TextureRenderer>();
                foreach (TextureRenderer renderer in tr)
                {
                    if (renderer != null && renderer.Texture != null)
                    {
                        application.Renderer.DrawTexture(renderer.Texture, renderer.Color, entity.Transform.Position, entity.Transform.Scale, renderer.UV, renderer.Layer);
                    }
                }
            }
        }
    }
}
