using EngineCore.Assets.AssetTypes;
using EngineCore.Assets.Importers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace EngineCore.Assets
{
    public class AssetManager
    {
        private Dictionary<string, Asset> assets;
        private List<IAssetImporter> importers;
        private string resourcesPath;

        public AssetManager()
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
            if (!Directory.Exists(dirPath)) throw new Exception();

            foreach(string fileName in  Directory.GetFiles(dirPath, $"*{extension}", SearchOption.AllDirectories))
            {             
                IAssetImporter importer = importers.FirstOrDefault(i => i.CanImport(Path.GetExtension(fileName)));
                if (importer != null)
                {
                    try
                    {
                        var asset = importer.Import(fileName);
                        assets[asset.Name] = asset;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine($"FileName: {Path.GetFileNameWithoutExtension(fileName)}; Loaded successfully");
            }
        }
        
        public T GetAsset<T>(string name) where T : Asset
        {
            if(assets.TryGetValue(name, out var asset))
            {
                return (T)asset;
            }
            return null;
        }
    }
}
