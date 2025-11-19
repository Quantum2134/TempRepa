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
        private IFileSystem _fileSystem;

        public ShaderImporter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool CanImport(string extension)
        {
            return extension == ".vert" || extension == ".frag";
        }

        public Asset Import(string fullPath)
        {
            string shaderSrc = _fileSystem.ReadAllText(fullPath);

            return new ShaderAsset
            {
                Name = _fileSystem.GetFileNameWithoutExtension(fullPath),
                Path = fullPath,
                ShaderSource = shaderSrc
            };
        }
    }
}
