using System.IO;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor.Utilities
{
    public static class FileUtils
    {
        public static string BuildAbsolutePathInProject(string relativePath)
        {
            return Path.Combine(Application.dataPath, relativePath);
        }

        public static bool FileExists(string filePath) => File.Exists(filePath);
        
        public static bool DirectoryExists(string directoryPath) => Directory.Exists(directoryPath);

        public static void TryCreateDirectory(string path)
        {
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new DirectoryNotFoundException($"Couldn´t get the directory path from original path {path}");
            }
            
            if (!DirectoryExists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static bool DirectoryIsEmpty(string directoryPath) =>
            Directory.GetDirectories(directoryPath).Length == 0;

        public static bool DirectoryIsEmpty(string directoryPath, ref string[] directories)
        {
            directories = Directory.GetDirectories(directoryPath);

            return directories.Length == 0;
        }
    }
}