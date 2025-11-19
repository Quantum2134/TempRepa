using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EngineCore.Assets
{
    public class PhysicalFileSystem : IFileSystem
    {
        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetFiles(string directory, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.GetFiles(directory, searchPattern, searchOption);
        }

        public string[] GetDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string CombinePath(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            string combinedPath = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                combinedPath = Path.Combine(combinedPath, paths[i]);
            }
            return combinedPath;
        }

        public string GetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}