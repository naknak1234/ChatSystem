using ChatSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ChatSystem.Utilities
{
    public static class AIModelFactory
    {
        public static IAIModel CreateAIModel(string modelName, string modelPath = null)
        {
            if (!string.IsNullOrEmpty(modelPath))
            {
                var localModel = new LocalAIModel(modelName);
                localModel.Initialize(modelPath).GetAwaiter().GetResult();
                return localModel;
            }
            switch (modelName)
            {
                case "TinyLlama":
                    return new TinyLlamaModel();
                case "Select more":
                    throw new ArgumentException("Select more is not a valid model. Please select a directory to load additional models.");
                default:
                    throw new ArgumentException("Unknown AI model");
            }
        }
        public static List<string> GetAvailableModels()
        {
            var models = new List<string> { "TinyLlama", "Select more" };
            string modelsDirectory = ConfigManager.GetModelsDirectory();

            if (Directory.Exists(modelsDirectory))
            {
                var looseFiles = Directory.GetFiles(modelsDirectory, "*.safetensors");
                if (looseFiles.Length > 0)
                {
                    Console.WriteLine("Warning: Found loose .safetensors files in the models directory. Models should be placed in subdirectories (e.g., models/gemma-2b/).");
                }
                var modelDirs = Directory.GetDirectories(modelsDirectory);
                foreach (var modelDir in modelDirs)
                {
                    var safetensorsFiles = Directory.GetFiles(modelDir, "*.safetensors");
                    if (safetensorsFiles.Length > 0) 
                    {
                        var modelName = Path.GetFileName(modelDir);
                        if (!models.Contains(modelName))
                        {
                            models.Insert(models.Count - 1, modelName); 
                        }
                    }
                }
            }

            return models;
        }
    }
}