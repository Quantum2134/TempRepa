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

        public string ResourcesPath { get; set; }

        public List<Asset> Assets => assets.Values.ToList();

        public AssetSystem()
        {
            assets = new Dictionary<string, Asset>();
            importers = new List<IAssetImporter>();

            string exedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //resourcesPath = Path.Combine(exedir, "Resources");


            importers.Add(new TextureImporter());
            importers.Add(new ShaderImporter());          
        }

        

        public void LoadAssets()
        {
            Logger.Log("Loading shaders", LogLevel.Info);
            LoadAssetsFromDirectory("Shaders", ".vert");
            LoadAssetsFromDirectory("Shaders", ".frag");

            Logger.Log("Loading textures", LogLevel.Info);
            LoadAssetsFromDirectory("Textures", ".png");
            LoadAssetsFromDirectory("Textures", ".jpg");
        }

        public void LoadAssetsFromDirectory(string subDir, string extension)
        {
            string dirPath = Path.Combine(ResourcesPath, subDir);
            if (!Directory.Exists(dirPath))
            {
                Logger.Log($"Path {dirPath} does not exist", LogLevel.Error);
                return;
            }

            foreach(string fileName in Directory.GetFiles(dirPath, $"*{extension}", SearchOption.AllDirectories))
            {             
                IAssetImporter importer = importers.FirstOrDefault(i => i.CanImport(Path.GetExtension(fileName)));
                if (importer != null)
                {
                    try
                    {
                        var asset = importer.Import(fileName);
                        assets[asset.Name] = asset;
                        Logger.Log($"Asset {asset.Name} loaded successfully", LogLevel.Info);
                    }
                    catch(Exception ex)
                    {
                        Logger.Log($"Failed to load {fileName} - {ex.Message}", LogLevel.Error);
                    }
                }
            }
        }

        public string CopyFileToResources(string sourceFullPath, string relativeDestinationDir)
        {
            if (string.IsNullOrWhiteSpace(sourceFullPath))
                throw new ArgumentException("Source file path is null or empty.", nameof(sourceFullPath));

            if (string.IsNullOrWhiteSpace(relativeDestinationDir))
                throw new ArgumentException("Relative destination directory is null or empty.", nameof(relativeDestinationDir));

            if (!File.Exists(sourceFullPath))
            {
                Logger.Log($"Source file does not exist: {sourceFullPath}", LogLevel.Error);
                return null;
            }

            // Полный путь к папке назначения внутри ресурсов
            string destDirFullPath = Path.Combine(ResourcesPath, relativeDestinationDir);
            Directory.CreateDirectory(destDirFullPath); // создаст, если не существует

            string fileName = Path.GetFileName(sourceFullPath);
            string destFullPath = Path.Combine(destDirFullPath, fileName);

            try
            {
                File.Copy(sourceFullPath, destFullPath, overwrite: true);
                Logger.Log($"File copied to resources: {destFullPath}", LogLevel.Info);

                // Возвращаем относительный путь (для последующей загрузки)
                return Path.Combine(relativeDestinationDir, fileName);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to copy file {sourceFullPath} to {destFullPath} - {ex.Message}", LogLevel.Error);
                return null;
            }
        }

        public void LoadAssetByRelativePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                Logger.Log("Relative path is null or empty.", LogLevel.Error);
                return;
            }

            string fullPath = Path.Combine(ResourcesPath, relativePath);

            if (!File.Exists(fullPath))
            {
                Logger.Log($"Asset file not found in resources: {fullPath}", LogLevel.Error);
                return;
            }

            string extension = Path.GetExtension(fullPath).ToLowerInvariant();

            IAssetImporter importer = importers.FirstOrDefault(i => i.CanImport(extension));
            if (importer == null)
            {
                Logger.Log($"No importer found for extension '{extension}' (file: {relativePath})", LogLevel.Error);
                return;
            }

            try
            {
                Asset asset = importer.Import(fullPath);
                assets[asset.Name] = asset; // или assets[asset.AssetId] = asset;
                Logger.Log($"Asset '{asset.Name}' loaded successfully from {relativePath}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to load asset from {relativePath} - {ex.Message}", LogLevel.Error);
            }
        }


        public T LoadAsset<T>(string relativePath) where T : Asset
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Relative path cannot be null or empty.", nameof(relativePath));

            string filePath = Path.GetFullPath(Path.Combine(ResourcesPath, relativePath));

            if (!File.Exists(filePath))
            {
                Logger.Log($"Asset file not found: {filePath}", LogLevel.Error);
                return null; // или выбросить исключение, в зависимости от твоей стратегии обработки ошибок
            }

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            IAssetImporter importer = importers.FirstOrDefault(i => i.CanImport(extension));
            if (importer == null)
            {
                Logger.Log($"No importer found for extension: {extension} (file: {filePath})", LogLevel.Error);
                return null;
            }

            try
            {
                Asset asset = importer.Import(filePath);

                // Дополнительно: можно проверить, что загруженный ассет действительно приводится к T
                if (asset is not T typedAsset)
                {
                    Logger.Log($"Loaded asset is not of expected type {typeof(T).Name}. Actual type: {asset.GetType().Name}", LogLevel.Error);
                    return null;
                }

                assets[asset.Name] = asset;
                Logger.Log($"Asset '{asset.Name}' ({asset.Name}) loaded successfully from {filePath}", LogLevel.Info);
                return typedAsset;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to load asset from {filePath} - {ex.Message}\n{ex.StackTrace}", LogLevel.Error);
                return null;
            }
        }

        public T GetAsset<T>(string name) where T : Asset
        {
            if(assets.TryGetValue(name, out var asset))
            {
                return (T)asset;
            }
            Logger.Log($"No asset was found", LogLevel.Error);
            return null;
        }
    }
}
