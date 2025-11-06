using EngineCore.Assets.AssetTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Assets.Importers
{
    internal class ShaderImporter : IAssetImporter
    {
        public bool CanImport(string extension)
        {
            return extension == ".vert" || extension == ".frag";
        }

        public Asset Import(string fullPath)
        {
            string path = Path.GetFileNameWithoutExtension(fullPath);

            string shaderSrc = File.ReadAllText(fullPath);

            return new ShaderAsset
            {
                Name = Path.GetFileNameWithoutExtension(fullPath),
                Path = fullPath,
                ShaderSource = shaderSrc
            };
        }
    }
}
