using EngineCore.Assets.Assets;
using EngineCore.Assets.AssetTypes;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Assets.Importers
{
    internal class TextureImporter : IAssetImporter
    {
        public TextureImporter()
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
        }

        public bool CanImport(string extension)
        {
            return extension == ".png" || extension == ".jpg";
        }

        public Asset Import(string fullPath)
        {
            using (FileStream stream = File.OpenRead(fullPath))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                return new TextureAsset
                {
                    Name = Path.GetFileNameWithoutExtension(fullPath),
                    Path = fullPath,
                    Width = image.Width,
                    Height = image.Height,
                    PixelData = image.Data,
                    Type = AssetType.Texture
                };
            }
        }
    }
}
