using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace ChatSystem.Utilities
{
    public class LocalAIModel : IAIModel
    {
        private string _modelPath;
        private bool _isInitialized;
        private readonly int _contextWindowSize = 512;
        private readonly Queue<string> _contextQueue;
        private readonly string _pythonPath = @"D:\Python3.12\python.exe";
        private string _scriptPath;
        public LocalAIModel(string modelName)
        {
            Name = modelName;
            _isInitialized = false;
            _contextQueue = new Queue<string>(_contextWindowSize);
            _scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "tinyllama_inference.py");
            System.Diagnostics.Debug.WriteLine($"Looking for script at: {_scriptPath}");
        }
        public string Name { get; }

        public async Task Initialize(string modelPath)
        {
            _modelPath = modelPath;
            _isInitialized = true;
            Console.WriteLine($"Model {Name} initialized for path {modelPath}");
        }
        public async Task<string> GetResponse(string input)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Model not initialized. Call Initialize first.");
            if (_contextQueue.Count >= _contextWindowSize)
            {
                _contextQueue.Dequeue();
            }
            _contextQueue.Enqueue(input);
            string context = string.Join(" ", _contextQueue);
            try
            {
                if (!File.Exists(_pythonPath))
                    throw new FileNotFoundException("Python executable not found.", _pythonPath);
                if (!File.Exists(_scriptPath))
                    throw new FileNotFoundException("TinyLlama inference script not found.", _scriptPath);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _pythonPath,
                    Arguments = $"\"{_scriptPath}\" \"{context}\" \"{Name}\" \"{ConfigManager.GetModelsDirectory()}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode != 0)
                        throw new Exception($"Python script error: {error}");

                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(output);
                    if (jsonResponse.error != null)
                        throw new Exception(jsonResponse.error.ToString());

                    return jsonResponse.response.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Inference error for {Name}: {ex.Message}";
            }
        }
    }
}