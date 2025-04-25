ChatSystem
ChatSystem is a WPF-based chat application created by naknak1234(Sam Sothyratanak). It integrates AI models (TinyLlama and Qwen2-7B-Instruct) for conversational responses. Users can interact with AI, copy chat messages, and manage chat sessions.
Features

Chat with AI models (TinyLlama and Qwen2-7B-Instruct).
Select and copy chat messages.
Responsive UI with a sidebar for chat sessions.
Support for multiple chat sessions with rename/delete options.

Prerequisites

Windows OS (due to WPF dependency).
Visual Studio 2022 with .NET desktop development workload.
Python 3.8+ installed.
Git installed for cloning the repository.
Internet connection for NuGet package restore and model downloads.

Setup Instructions
1. Clone the Repository
git clone https://github.com/naknak1234/ChatSystem.git
cd ChatSystem

2. Restore .NET Dependencies (NuGet Packages)
Open ChatSystem.sln in Visual Studio 2022. The project uses NuGet for dependency management. The following NuGet packages are required:

Newtonsoft.Json: Used for JSON parsing (e.g., handling AI model responses).

To install the NuGet packages:

Right-click the solution in Solution Explorer and select Manage NuGet Packages for Solution.
Search for Newtonsoft.Json and install version 13.0.3 (or the latest compatible version) for the ChatSystem project.
Alternatively, use the Package Manager Console:Install-Package Newtonsoft.Json -Version 13.0.3


Build the solution (Ctrl+Shift+B) to ensure all dependencies are restored.

If you encounter NuGet restore issues:

Verify your internet connection.
Go to Tools > NuGet Package Manager > Package Manager Console and run:dotnet restore


Alternatively, right-click the solution in Solution Explorer and select Restore NuGet Packages.

3. Install Python Dependencies
The project uses a Python script (tinyllama_inference.py) for AI model inference. Install the required Python packages:
pip install torch transformers>=4.37.0 huggingface_hub bitsandbytes psutil

4. Download AI Models
The project uses two AI models, which are not included in the repository due to their size. Download them manually:

TinyLlama-1.1B:huggingface-cli download TinyLlama/TinyLlama-1.1B-Chat-v1.0 --local-dir ./models/tinyllama-1.1b


Qwen2-7B-Instruct:huggingface-cli download Qwen/Qwen2-7B-Instruct --local-dir ./models/qwen2-7b-instruct



Ensure the models are placed in the models directory under the project’s bin/Debug folder (e.g., ChatSystem/bin/Debug/models/tinyllama-1.1b and ChatSystem/bin/Debug/models/qwen2-7b-instruct).
5. Configure the Project

Ensure Python is in your system PATH.
Verify that the tinyllama_inference.py script is located in the models directory (e.g., ChatSystem/bin/Debug/models/tinyllama_inference.py).

Running the Application

Open ChatSystem.sln in Visual Studio 2022.
Build the solution (Ctrl+Shift+B).
Run the application (F5).
Select an AI model from the dropdown (TinyLlama or Qwen2-7B-Instruct).
Start chatting! You can copy messages by selecting text and using Ctrl+C.

Troubleshooting

NuGet Restore Fails: Ensure Visual Studio is configured to allow NuGet downloads (Tools > Options > NuGet Package Manager). Try restoring manually via dotnet restore in the Package Manager Console.
Python Script Errors: Ensure Python dependencies are installed and tinyllama_inference.py is in the correct location.
Model Not Found: Verify the model directories exist in bin/Debug/models/.
Memory Issues: Qwen2-7B-Instruct requires significant RAM (up to 15GB). Ensure your system has enough memory, or use TinyLlama instead.
Build Errors: Ensure you’re targeting a compatible .NET version (e.g., .NET Framework 4.8 or .NET 6+).

Contributing
Feel free to fork the repository, make improvements, and submit pull requests. For major changes, please open an issue first to discuss.
License
This project is licensed under the MIT License. See the LICENSE file for details.
Acknowledgments

Built with WPF and Transformers.
Models: TinyLlama and Qwen2-7B-Instruct.

