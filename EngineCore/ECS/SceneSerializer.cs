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
        public string TextureName { get; set; }
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

        foreach (var entity in scene.Entities)
        {
            var entityDto = new EntityDto
            {
                Name = entity.Name,
                Transform = new TransformDto
                {
                    Pos = new float[] { entity.Transform.Position.X, entity.Transform.Position.Y },
                    Rotation = entity.Transform.Rotation,
                    Scale = new float[] { entity.Transform.Scale.X, entity.Transform.Scale.Y }
                }
            };

            TextureRenderer[] tr = entity.GetComponents<TextureRenderer>();
            foreach (var textureRenderer in tr)
            {
                entityDto.SpriteRenderer = new SpriteRendererDto
                {
                    Color = new float[]
                    {
                        textureRenderer.Color.R,
                        textureRenderer.Color.G,
                        textureRenderer.Color.B,
                        textureRenderer.Color.A
                    },
                    Z = textureRenderer.Layer,
                    TextureName = textureRenderer.Texture.TextureName
                };
            }

            //ScriptComponent[] sc = entity.GetComponents<ScriptComponent>();
            //foreach(var scriptComponent in sc)
            //{
            //    entityDto.ScriptComponent = new ScriptComponentDto
            //    {
            //        ScriptName = scriptComponent.name
            //    };
            //}

            ScriptComponent sc = entity.GetComponent<ScriptComponent>();
            entityDto.ScriptComponent = new ScriptComponentDto
            {
                ScriptName = sc?.name
            };
                 
            dto.Entities.Add(entityDto);
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(dto, options);
        if (!Directory.Exists(filePath))
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
            entity.Transform = new Transform
            {
                Position = new Vector2(entityDto.Transform.Pos[0], entityDto.Transform.Pos[1]),
                Rotation = entityDto.Transform.Rotation,
                Scale = new Vector2(entityDto.Transform.Scale[0], entityDto.Transform.Scale[1])
            };

            // SpriteRenderer
            if (entityDto.SpriteRenderer != null)
            {
                TextureRenderer sr = new TextureRenderer();
                sr.Texture = Application.GraphicsSystem.GraphicsContext.TextureManager.GetTexture(entityDto.SpriteRenderer.TextureName);
                sr.Color = new Color4(
                        entityDto.SpriteRenderer.Color[0],
                        entityDto.SpriteRenderer.Color[1],
                        entityDto.SpriteRenderer.Color[2],
                        entityDto.SpriteRenderer.Color[3]);
                sr.Layer = entityDto.SpriteRenderer.Z;

                entity.AddComponent(sr);
            }

            // ScriptComponent
            if (entityDto.ScriptComponent != null && !string.IsNullOrEmpty(entityDto.ScriptComponent.ScriptName))
            {
                var script = Application.ScriptSystem.GetScript(entityDto.ScriptComponent.ScriptName);
                if (script != null)
                {
                    ScriptComponent scriptComponent = (ScriptComponent)Activator.CreateInstance(script);

                    scriptComponent.name = entityDto.ScriptComponent.ScriptName;
                    scriptComponent.Application = Application;
                    entity.AddComponent(scriptComponent);
                }
                else
                {
                    // Опционально: логирование ошибки
                    Console.WriteLine($"Warning: Script '{entityDto.ScriptComponent.ScriptName}' not registered.");
                }
            }

            scene.Entities.Add(entity);
        }

        return scene;
    }
}