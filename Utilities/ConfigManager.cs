using System;
using System.IO;

namespace ChatSystem.Utilities
{
    public static class ConfigManager
    {
        private static string _modelsDirectory;
        static ConfigManager()
        {
            _modelsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");

            if (!Directory.Exists(_modelsDirectory))
            {
                Directory.CreateDirectory(_modelsDirectory);
            }
        }
        public static string GetModelsDirectory()
        {
            return _modelsDirectory;
        }
        public static void SetModelsDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path cannot be empty.");
            _modelsDirectory = directoryPath;
            if (!Directory.Exists(_modelsDirectory))
            {
                Directory.CreateDirectory(_modelsDirectory);
            }
        }
    }
}