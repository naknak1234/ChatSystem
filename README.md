# ChatSystem 💬  
A modern WPF-based chat application powered by AI, created by **naknak1234**.  

ChatSystem uses **TinyLlama-1.1B** and **Qwen2-7B-Instruct** models to provide seamless AI conversations in a sleek, professional interface.

---

## 🌟 Features
- **AI-Powered Chat:** Use TinyLlama-1.1B or Qwen2-7B-Instruct for dynamic conversations.
- **Select and Copy:** Highlight chat messages and press **Ctrl+C** to copy.
- **Session Management:** Create, rename, and delete multiple chat sessions.
- **Polished Modern UI:** Gradient backgrounds, shadow effects, and rounded corners for a clean, premium look.

---

## 📋 Table of Contents
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
  - [Clone the Repository](#clone-the-repository)
  - [Restore .NET Dependencies](#restore-net-dependencies)
  - [Install Python Dependencies](#install-python-dependencies)
  - [Download AI Models](#download-ai-models)
  - [Configure the Project](#configure-the-project)
- [Running the Application](#running-the-application)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [Acknowledgments](#acknowledgments)

---

## 📋 Prerequisites
Before you begin, make sure you have:

- 🖥️ **Windows OS** (Required for WPF)
- 🛠️ **Visual Studio 2022** with **.NET Desktop Development** workload
- 🐍 **Python 3.8+** installed and added to `PATH`
- 🌐 **Git** installed for cloning the repository
- 📡 **Internet connection** for NuGet and Huggingface downloads

---

## ⚙️ Setup Instructions

### 1. Clone the Repository
```
git clone https://github.com/naknak1234/ChatSystem.git
cd ChatSystem
```

---

### 2. Restore .NET Dependencies
Open `ChatSystem.sln` with Visual Studio 2022.  
Install these **NuGet Packages**:

| Package | Version |
|:--------|:--------|
| Microsoft.Win32.SystemEvents | Latest |
| Newtonsoft.Json | Latest |
| Microsoft.VisualBasic | Latest |
| WindowsAPICodePack-Core | Latest |
| WindowsAPICodePack-Shell | Latest |
| Microsoft.Extensions.Configuration | 6.0.0 |
| Microsoft.Extensions.Configuration.Json | 6.0.0 |
| Microsoft.Extensions.Configuration.Binder | 6.0.0 |

#### Install via NuGet GUI:
- Right-click the solution in **Solution Explorer** → **Manage NuGet Packages for Solution** → Install the packages.

#### Or via Package Manager Console:
```bash
Install-Package Microsoft.Win32.SystemEvents
Install-Package Newtonsoft.Json
Install-Package Microsoft.VisualBasic
Install-Package WindowsAPICodePack-Core
Install-Package WindowsAPICodePack-Shell
Install-Package Microsoft.Extensions.Configuration -Version 6.0.0
Install-Package Microsoft.Extensions.Configuration.Json -Version 6.0.0
Install-Package Microsoft.Extensions.Configuration.Binder -Version 6.0.0
```

> 🔥 **Important:**  
> After installing, build the solution (`Ctrl+Shift+B`) to ensure everything compiles.

---

### 3. Install Python Dependencies
Install the required Python packages for AI inference:
```
pip install torch transformers>=4.37.0 huggingface_hub bitsandbytes psutil
```

---

### 4. Download AI Models
Use the **Hugging Face CLI** to download models:

**TinyLlama-1.1B**:
```
huggingface-cli download TinyLlama/TinyLlama-1.1B-Chat-v1.0 --local-dir ./models/tinyllama-1.1b
```

**Qwen2-7B-Instruct**:
```
huggingface-cli download Qwen/Qwen2-7B-Instruct --local-dir ./models/qwen2-7b-instruct
```

Move the downloaded models into:
```
ChatSystem/bin/Debug/models/
```

✅ **Example Structure**:
```
ChatSystem/
 └── bin/
      └── Debug/
           └── models/
                ├── tinyllama-1.1b/
                └── qwen2-7b-instruct/
```

> ⚠️ **Note:**  
> Qwen2-7B requires **at least ~15GB of RAM**. If your machine can't handle it, use TinyLlama instead.

---

### 5. Configure the Project
- Confirm Python is installed:
  ```
  python --version
  ```
- Ensure `tinyllama_inference.py` exists under your project and will copy during build.

---

## 🚀 Running the Application
1. Open the solution in Visual Studio.
2. Build (`Ctrl+Shift+B`).
3. Run (`F5`).
4. Select an AI model.
5. Chat away!

🔑 **To copy a chat message:**  
Highlight it and press **Ctrl+C**.

---

## 🛠️ Troubleshooting

| Issue | Solution |
|:------|:---------|
| **NuGet Restore Fails** | Try `dotnet restore` or check Visual Studio > Tools > NuGet Package Manager settings. |
| **Python Errors** | Ensure all Python dependencies are installed and correct. |
| **Model Not Found** | Verify the models are placed correctly under `bin/Debug/models/`. |
| **Out of Memory** | Use TinyLlama instead of Qwen2-7B if RAM is insufficient. |
| **Build Errors** | Double-check that your NuGet packages are installed and correct target framework is selected. |

---

## 🤝 Contributing
Contributions are welcome!  
Steps:

1. Fork the repository.
2. Create a new branch:
    ```
    git checkout -b feature/your-feature-name
    ```
3. Commit changes:
    ```
    git commit -m "Description of feature"
    ```
4. Push branch:
    ```
    git push origin feature/your-feature-name
    ```
5. Open a **Pull Request**.

---

## 🙌 Acknowledgments
- Built with **WPF**, **Transformers**, and **PyTorch**.
- AI Models: **TinyLlama-1.1B** and **Qwen2-7B-Instruct**.
- Special thanks to the open-source community.

---

⭐ **If you enjoy this project, star it on GitHub to show your support!** ⭐
