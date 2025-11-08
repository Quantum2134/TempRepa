using EngineCore.Core;
using EngineCore.ECS;
using EngineCore.ECS.Components;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SceneSerializer
{
    // DTO для сериализации — проще и безопаснее, чем пытаться сериализовать runtime-объекты напрямую
    private class SceneDto
    {
        public string Name { get; set; } = "";
        public List<EntityDto> Entities { get; set; } = new();
    }

    private class EntityDto
    {
        public string Name { get; set; } = "Entity";
        public TransformDto Transform { get; set; } = new();
        public SpriteRendererDto? SpriteRenderer { get; set; }
        public ScriptComponentDto? ScriptComponent { get; set; }
    }

    private class TransformDto
    {
        public float[] Pos { get; set; } = new float[2];
        public float Rotation { get; set; }
        public float[] Scale { get; set; } = new float[2];
    }

    private class SpriteRendererDto
    {
        public float[] Color { get; set; } = new float[4] { 1, 1, 1, 1 };
        public int Z { get; set; }
        public string TextureName { get; set; } = "";
    }

    private class ScriptComponentDto
    {
        public string ScriptName { get; set; } = "";
    }

    public Application Application { get; private set; }

    public SceneSerializer(Application application)
    {
        Application = application;
    }

    // -- Сохранение --

    public void SaveScene(string filePath, Scene scene)
    {
        var dto = new SceneDto
        {
            Name = scene.Name,
            Entities = new List<EntityDto>()
        };

        foreach (var entity in scene.entities)
        {
            var entityDto = new EntityDto
            {
                Name = entity.Name,
                Transform = new TransformDto
                {
                    Pos = new float[] { entity.transform.Position.X, entity.transform.Position.Y },
                    Rotation = entity.transform.Rotation,
                    Scale = new float[] { entity.transform.Scale.X, entity.transform.Scale.Y }
                }
            };

            SpriteRenderer sr = entity.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                entityDto.SpriteRenderer = new SpriteRendererDto
                {
                    Color = new float[]
                    {
                        sr.Color.R,
                        sr.Color.G,
                        sr.Color.B,
                        sr.Color.A
                    },
                    Z = sr.Layer,
                    TextureName = Application.GraphicsSystem.GraphicsContext.TextureManager.GetName(sr.Sprite.Texture)
                };
            }

            ScriptComponent sc = entity.GetComponent<ScriptComponent>();
            if (sc != null)
            {
                entityDto.ScriptComponent = new ScriptComponentDto
                {
                    ScriptName = sc.name
                };
            }

            dto.Entities.Add(entityDto);
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(dto, options);
        if(!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        File.WriteAllText(Path.Combine(filePath, $"{scene.Name}.scene.json"), json);
    }

    // -- Загрузка --

    public Scene LoadScene(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Scene file not found: {filePath}");

        string json = File.ReadAllText(filePath);
        var dto = JsonSerializer.Deserialize<SceneDto>(json)
            ?? throw new InvalidOperationException("Failed to deserialize scene.");

        var scene = new Scene { Name = dto.Name };

        foreach (var entityDto in dto.Entities)
        {
            var entity = new Entity(entityDto.Name);

            // Transform
            entity.transform = new Transform
            {
                Position = new Vector2(entityDto.Transform.Pos[0], entityDto.Transform.Pos[1]),
                Rotation = entityDto.Transform.Rotation,
                Scale = new Vector2(entityDto.Transform.Scale[0], entityDto.Transform.Scale[1])
            };

            // SpriteRenderer
            if (entityDto.SpriteRenderer != null)
            {
                SpriteRenderer sr = new SpriteRenderer(new EngineCore.Graphics.Sprite(Application.GraphicsSystem.GraphicsContext.TextureManager.GetTexture(entityDto.SpriteRenderer.TextureName)));
                sr.Color = new Color4(
                        entityDto.SpriteRenderer.Color[0],
                        entityDto.SpriteRenderer.Color[1],
                        entityDto.SpriteRenderer.Color[2],
                        entityDto.SpriteRenderer.Color[3]);
                sr.Layer = entityDto.SpriteRenderer.Z;

                //entity.SpriteRenderer = new SpriteRenderer
                //{
                //    Color = new Color(
                //        entityDto.SpriteRenderer.Color[0],
                //        entityDto.SpriteRenderer.Color[1],
                //        entityDto.SpriteRenderer.Color[2],
                //        entityDto.SpriteRenderer.Color[3]
                //    ),
                //    Z = entityDto.SpriteRenderer.Z,
                //    TextureName = entityDto.SpriteRenderer.TextureName
                //};
                entity.AddComponent(sr);
            }

            //// ScriptComponent
            //if (entityDto.ScriptComponent != null && !string.IsNullOrEmpty(entityDto.ScriptComponent.ScriptName))
            //{
            //    var script = ScriptRegistry.Create(entityDto.ScriptComponent.ScriptName);
            //    if (script != null)
            //    {
            //        script.ScriptName = entityDto.ScriptComponent.ScriptName;
            //        entity.ScriptComponent = script;
            //    }
            //    else
            //    {
            //        // Опционально: логирование ошибки
            //        Console.WriteLine($"Warning: Script '{entityDto.ScriptComponent.ScriptName}' not registered.");
            //    }
            //}

            scene.entities.Add(entity);
        }

        return scene;
    }
}