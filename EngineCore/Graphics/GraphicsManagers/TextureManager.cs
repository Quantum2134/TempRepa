using EngineCore.Assets.Assets;
using EngineCore.Assets.AssetTypes;
using EngineCore.Logging;
using EngineCore.Platform.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics.GraphicsManagers
{
    public class TextureManager
    {
        private GraphicsContext graphicsContext;

        private Dictionary<string, Texture> textures;

        public TextureManager(GraphicsContext graphicsContext)
        {
            this.graphicsContext = graphicsContext;

            textures = new Dictionary<string, Texture>();
        }

        public Texture GetTexture(string name)
        {
            if (textures.TryGetValue(name, out var texture)) return texture;

            return null;
        }

        public string GetName(Texture texture)
        {
            return textures.FirstOrDefault(x => x.Value == texture).Key;
        }

        public Texture LoadTexture(string name)
        {
            if (textures.ContainsKey(name)) return textures[name];

            TextureAsset textureAsset = graphicsContext.GraphicsSystem.Application.AssetManager.LoadAsset<TextureAsset>(name);

            if (textureAsset == null)
            {
                Logger.Log($"Could not load texture asset for {name}", LogLevel.Error);
                return null;
            }

            Texture texture = new Texture(textureAsset.Width, textureAsset.Height, textureAsset.PixelData);
            textures.Add(name, texture);

            Logger.Log($"Texture {name} created successfully", LogLevel.Info);

            return texture;
        }           
    }
}
