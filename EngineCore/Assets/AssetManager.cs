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
    public class AssetManager : IAssetLoader, IDisposable
    {
        private Dictionary<string, AssetReference> _assets;
        private List<IAssetImporter> _importers;
        private IFileSystem _fileSystem;
        private string _resourcesPath;

        public string ResourcesPath => _resourcesPath;
        public IEnumerable<string> LoadedAssetPaths => _assets.Keys;

        public AssetManager(IFileSystem fileSystem = null)
        {
            _assets = new Dictionary<string, AssetReference>();
            _importers = new List<IAssetImporter>();
            _fileSystem = fileSystem ?? new PhysicalFileSystem();

            // Определяем путь к ресурсам
            string exedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _resourcesPath = _fileSystem.CombinePath(exedir, "Resources");

            // Регистрируем импортеры
            _importers.Add(new TextureImporter(_fileSystem));
            _importers.Add(new ShaderImporter(_fileSystem));
        }

        public T LoadAsset<T>(string assetPath) where T : Asset
        {
            // Проверяем, существует ли файл
            string fullPath = GetFullPath(assetPath);
            if (!_fileSystem.FileExists(fullPath))
            {
                Logger.Log($"Asset file does not exist: {fullPath}", LogLevel.Error);
                return null;
            }

            // Проверяем, загружен ли ассет
            if (_assets.TryGetValue(assetPath, out var existingRef))
            {
                existingRef.AddReference();
                return (T)existingRef.Asset;
            }

            // Загружаем новый ассет
            IAssetImporter importer = _importers.FirstOrDefault(i => i.CanImport(_fileSystem.GetExtension(fullPath)));
            if (importer != null)
            {
                try
                {
                    Asset asset = importer.Import(fullPath);
                    if (asset != null)
                    {
                        // Устанавливаем путь ассета как путь к файлу (относительно ресурсов)
                        asset.Path = assetPath;
                        asset.Name = _fileSystem.GetFileNameWithoutExtension(assetPath);

                        var assetRef = new AssetReference(asset, assetPath);
                        _assets[assetPath] = assetRef;

                        Logger.Log($"Asset {assetPath} loaded successfully", LogLevel.Info);
                        return (T)asset;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to load {fullPath} - {ex.Message}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Log($"No importer found for asset: {assetPath}", LogLevel.Error);
            }

            return null;
        }

        public Asset LoadAsset(string assetPath, Type assetType)
        {
            // Проверяем, существует ли файл
            string fullPath = GetFullPath(assetPath);
            if (!_fileSystem.FileExists(fullPath))
            {
                Logger.Log($"Asset file does not exist: {fullPath}", LogLevel.Error);
                return null;
            }

            // Проверяем, загружен ли ассет
            if (_assets.TryGetValue(assetPath, out var existingRef))
            {
                existingRef.AddReference();
                return existingRef.Asset;
            }

            // Загружаем новый ассет
            IAssetImporter importer = _importers.FirstOrDefault(i => i.CanImport(_fileSystem.GetExtension(fullPath)));
            if (importer != null)
            {
                try
                {
                    Asset asset = importer.Import(fullPath);
                    if (asset != null && assetType.IsAssignableFrom(asset.GetType()))
                    {
                        // Устанавливаем путь ассета как путь к файлу (относительно ресурсов)
                        asset.Path = assetPath;
                        asset.Name = _fileSystem.GetFileNameWithoutExtension(assetPath);

                        var assetRef = new AssetReference(asset, assetPath);
                        _assets[assetPath] = assetRef;

                        Logger.Log($"Asset {assetPath} loaded successfully", LogLevel.Info);
                        return asset;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to load {fullPath} - {ex.Message}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Log($"No importer found for asset: {assetPath}", LogLevel.Error);
            }

            return null;
        }

        public T GetAsset<T>(string assetPath) where T : Asset
        {
            if (_assets.TryGetValue(assetPath, out var assetRef))
            {
                return (T)assetRef.Asset;
            }
            
            Logger.Log($"Asset {assetPath} is not loaded", LogLevel.Warning);
            return null;
        }

        public Asset GetAsset(string assetPath)
        {
            if (_assets.TryGetValue(assetPath, out var assetRef))
            {
                return assetRef.Asset;
            }
            
            Logger.Log($"Asset {assetPath} is not loaded", LogLevel.Warning);
            return null;
        }

        public void PreloadAsset(string assetPath)
        {
            // Загружает ассет без увеличения счетчика ссылок
            // Полезно для предварительной загрузки
            string fullPath = GetFullPath(assetPath);
            if (!_fileSystem.FileExists(fullPath))
            {
                Logger.Log($"Asset file does not exist: {fullPath}", LogLevel.Error);
                return;
            }

            if (_assets.ContainsKey(assetPath))
            {
                // Ассет уже загружен
                return;
            }

            IAssetImporter importer = _importers.FirstOrDefault(i => i.CanImport(_fileSystem.GetExtension(fullPath)));
            if (importer != null)
            {
                try
                {
                    Asset asset = importer.Import(fullPath);
                    if (asset != null)
                    {
                        asset.Path = assetPath;
                        asset.Name = _fileSystem.GetFileNameWithoutExtension(assetPath);

                        // Создаем ссылку с нулевым счетчиком, чтобы можно было выгрузить при необходимости
                        var assetRef = new AssetReference(asset, assetPath);
                        // Не увеличиваем счетчик, т.к. это предварительная загрузка
                        _assets[assetPath] = assetRef;

                        Logger.Log($"Asset {assetPath} preloaded successfully", LogLevel.Info);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to preload {fullPath} - {ex.Message}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Log($"No importer found for asset: {assetPath}", LogLevel.Error);
            }
        }

        public bool IsAssetLoaded(string assetPath)
        {
            return _assets.ContainsKey(assetPath);
        }

        public void UnloadAsset(string assetPath)
        {
            if (_assets.TryGetValue(assetPath, out var assetRef))
            {
                if (assetRef.RemoveReference())
                {
                    // Удаляем ассет, если больше нет ссылок
                    _assets.Remove(assetPath);
                    // Освобождаем ресурсы ассета, если он реализует IDisposable
                    if (assetRef.Asset is IDisposable disposableAsset)
                    {
                        disposableAsset.Dispose();
                    }
                    Logger.Log($"Asset {assetPath} unloaded", LogLevel.Info);
                }
            }
        }

        public void UnloadAllAssets()
        {
            foreach (var assetRef in _assets.Values)
            {
                if (assetRef.Asset is IDisposable disposableAsset)
                {
                    disposableAsset.Dispose();
                }
            }
            _assets.Clear();
            Logger.Log("All assets unloaded", LogLevel.Info);
        }

        public IEnumerable<string> GetLoadedAssetPaths()
        {
            return _assets.Keys.ToList();
        }

        private string GetFullPath(string assetPath)
        {
            // Если путь абсолютный, возвращаем как есть
            if (Path.IsPathRooted(assetPath))
            {
                return assetPath;
            }

            // Иначе считаем, что путь относительный и строим от Resources
            return _fileSystem.CombinePath(_resourcesPath, assetPath);
        }

        public void Dispose()
        {
            UnloadAllAssets();
        }
    }
}