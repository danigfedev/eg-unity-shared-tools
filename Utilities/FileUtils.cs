using System.IO;
using UnityEngine;

namespace eg_unity_shared_tools.Utilities
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

        public static (bool, string[]) DirectoryHasSubDirectories(string directoryPath)
        {
            var subdirectories = Directory.GetDirectories(directoryPath);

            return (subdirectories.Length > 0, subdirectories);
        }
        
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool recursive = true)
        {
            var sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
            
            if (!sourceDirectoryInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectoryInfo.FullName}");
            }

            destinationDirectory = Path.Combine(destinationDirectory, sourceDirectoryInfo.Name);
            DirectoryInfo[] subdirectories = sourceDirectoryInfo.GetDirectories();

            Directory.CreateDirectory(destinationDirectory); //If directory already exists, it won´t do anything
            
            foreach (FileInfo file in sourceDirectoryInfo.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(targetFilePath);
            }
            
            if (recursive)
            {
                foreach (DirectoryInfo subDirectory in subdirectories)
                {
                    string newDestinationDir = Path.Combine(destinationDirectory, subDirectory.Name);
                    CopyDirectory(subDirectory.FullName, newDestinationDir, true);
                }
            }
        }
    }
}