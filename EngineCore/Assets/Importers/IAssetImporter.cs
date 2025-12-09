using EngineCore.Assets.AssetTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Assets.Importers
{
    internal interface IAssetImporter
    {
        public bool CanImport(string extension);

        public Asset Import(string fullPath);
    }
}
