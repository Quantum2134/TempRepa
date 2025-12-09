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

        public Dictionary<string, Texture> Textures => textures;

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

        public Texture LoadTexture(TextureAsset textureAsset)
        {
            if (textures.ContainsKey(textureAsset.Name)) return textures[textureAsset.Name];

            Texture texture = new Texture(textureAsset.Width, textureAsset.Height, textureAsset.PixelData);
            texture.TextureName = textureAsset.Name;
            textures.Add(textureAsset.Name, texture);

            Logger.Log($"Texture {textureAsset.Name} created successfully", LogLevel.Info);

            return texture;
        }           
    }
}
