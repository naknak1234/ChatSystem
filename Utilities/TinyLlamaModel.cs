using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChatSystem.Utilities
{
    public class TinyLlamaModel : IAIModel
    {
        public string Name => "TinyLlama";
        private readonly string _pythonPath = @"D:\Python3.12\python.exe";
        private readonly string _scriptPath;
        private readonly int _contextWindowSize = 128;
        private readonly Queue<string> _contextQueue;
        public TinyLlamaModel()
        {
            _scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "tinyllama_inference.py");
            System.Diagnostics.Debug.WriteLine($"Looking for script at: {_scriptPath}");
            _contextQueue = new Queue<string>(_contextWindowSize);
        }
        public async Task Initialize(string modelPath)
        {
            await Task.CompletedTask;
        }

        public async Task<string> GetResponse(string input)
        {
            try
            {
                if (!File.Exists(_pythonPath))
                    throw new FileNotFoundException("Python executable not found.", _pythonPath);
                if (!File.Exists(_scriptPath))
                    throw new FileNotFoundException("TinyLlama inference script not found.", _scriptPath);
                if (_contextQueue.Count >= _contextWindowSize)
                {
                    _contextQueue.Dequeue();
                }
                _contextQueue.Enqueue(input);
                string context = string.Join(" ", _contextQueue);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _pythonPath,
                    Arguments = $"\"{_scriptPath}\" \"{context}\" \"TinyLlama-1.1B-Chat-v1.0\" \"{ConfigManager.GetModelsDirectory()}\"",
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
                throw new Exception($"TinyLlama error: {ex.Message}");
            }
        }
    }
}