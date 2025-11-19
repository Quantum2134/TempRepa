using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EngineCore.Assets
{
    public interface IFileSystem
    {
        Stream OpenRead(string path);
        string ReadAllText(string path);
        byte[] ReadAllBytes(string path);
        bool FileExists(string path);
        bool DirectoryExists(string path);
        string[] GetFiles(string directory, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);
        string[] GetDirectories(string directory);
        string GetFileName(string path);
        string GetFileNameWithoutExtension(string path);
        string GetExtension(string path);
        string CombinePath(params string[] paths);
        string GetDirectory(string path);
    }
}