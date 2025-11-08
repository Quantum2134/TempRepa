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
    public class AssetSystem
    {
        private Dictionary<string, Asset> assets;
        private List<IAssetImporter> importers;
        private string resourcesPath;

        public string ResourcesPath => resourcesPath;

        public List<Asset> Assets => assets.Values.ToList();

        public AssetSystem()
        {
            assets = new Dictionary<string, Asset>();
            importers = new List<IAssetImporter>();

            string exedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            resourcesPath = Path.Combine(exedir, "Resources");


            importers.Add(new TextureImporter());
            importers.Add(new ShaderImporter());          
        }

        public void LoadAssets()
        {
            LoadAssetsFromDirectory("Shaders", ".vert");
            LoadAssetsFromDirectory("Shaders", ".frag");

            LoadAssetsFromDirectory("Textures", ".png");
            LoadAssetsFromDirectory("Textures", ".jpg");
        }

        public void LoadAssetsFromDirectory(string subDir, string extension)
        {
            string dirPath = Path.Combine(resourcesPath, subDir);
            if (!Directory.Exists(dirPath)) Logger.Log($"Path {dirPath} does not exist", LogLevel.Error);

            foreach(string fileName in  Directory.GetFiles(dirPath, $"*{extension}", SearchOption.AllDirectories))
            {             
                IAssetImporter importer = importers.FirstOrDefault(i => i.CanImport(Path.GetExtension(fileName)));
                if (importer != null)
                {
                    try
                    {
                        var asset = importer.Import(fileName);
                        assets[asset.Name] = asset;
                        Logger.Log($"Asset {asset.Path} loaded successfully", LogLevel.Info);
                    }
                    catch(Exception ex)
                    {
                        Logger.Log($"Failed to load {fileName} - {ex.Message}", LogLevel.Error);
                    }
                }
            }
        }
        
        public T GetAsset<T>(string name) where T : Asset
        {
            if(assets.TryGetValue(name, out var asset))
            {
                return (T)asset;
            }
            Logger.Log($"No {name} was found", LogLevel.Error);
            return null;
        }
    }
}
