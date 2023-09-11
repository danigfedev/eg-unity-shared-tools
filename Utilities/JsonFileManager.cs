using System.IO;
using UnityEngine;

namespace eg_unity_shared_tools.Utilities
{
    public static class JsonFileManager
    {
        public static TParsedType LoadJson<TParsedType>(string absoluteFilePath) where TParsedType : class
        {
            if (!File.Exists(absoluteFilePath))
            {
                Debug.LogWarning($"JSON file not found at {absoluteFilePath}. Returning NULL.");
                return null;
            }

            string jsonString = File.ReadAllText(absoluteFilePath);
            var parsedData = JsonUtility.FromJson<TParsedType>(jsonString);

            return parsedData;
        }

        public static void SaveJson<TParsedType>(TParsedType data, string absoluteFilePath)
        {
            var jsonString = JsonUtility.ToJson(data);
            File.WriteAllText(absoluteFilePath, jsonString);
        }
    }
}