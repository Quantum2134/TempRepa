using EngineCore.Assets.AssetTypes;
using EngineCore.Assets.Importers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using EngineCore.Logging;

namespace EngineCore.Assets
{
    public interface IAssetLoader
    {
        T LoadAsset<T>(string assetPath) where T : Asset;
        Asset LoadAsset(string assetPath, Type assetType);
        void UnloadAsset(string assetPath);
        void UnloadAllAssets();
        bool IsAssetLoaded(string assetPath);
        T GetAsset<T>(string assetPath) where T : Asset;
        Asset GetAsset(string assetPath);
        void PreloadAsset(string assetPath);
        IEnumerable<string> GetLoadedAssetPaths();
    }
}