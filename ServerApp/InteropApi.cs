namespace BlazorLib.Interop
{
    public interface IInteropApi
    {
        List<ScriptFile> CobolFiles { get; }
        List<ScriptFile> PythonFiles { get; }

        List<ScriptFile> GetCobolFiles();
        List<ScriptFile> GetPythonFiles();

        Task LoadCobolFilesAsync();
        Task LoadPythonFilesAsync();
    }

    public class InteropApi : IInteropApi
    {
        private bool _cobolInitialized;
        private bool _pythonInitialized;

        public List<ScriptFile> CobolFiles { get; } = new();
        public List<ScriptFile> PythonFiles { get; } = new();

        public List<ScriptFile> GetCobolFiles()
        {
            LoadCobolFiles();
            return CobolFiles;
        }

        public List<ScriptFile> GetPythonFiles()
        {
            LoadPythonFiles();
            return PythonFiles;
        }

        public void LoadCobolFiles()
        {
            if (_cobolInitialized)
            {
                return;
            }

            _cobolInitialized = true;
            CobolFiles.Clear();

            var cobolPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "cobol");
            if (!Directory.Exists(cobolPath))
            {
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(cobolPath, "*.cobol"))
            {
                var fileName = Path.GetFileName(filePath);
                var content = File.ReadAllText(filePath);
                CobolFiles.Add(new ScriptFile { FileName = fileName, Content = content });
            }
        }

        public void LoadPythonFiles()
        {
            if (_pythonInitialized)
            {
                return;
            }

            _pythonInitialized = true;
            PythonFiles.Clear();
            PythonFiles.Add(new ScriptFile { FileName = "data_process.py", Content = "import pandas as pd\nprint('Processing data...')" });
            PythonFiles.Add(new ScriptFile { FileName = "scraper.py", Content = "import requests\nresponse = requests.get('url')\nprint(response.text)" });
            PythonFiles.Add(new ScriptFile { FileName = "utils.py", Content = "def add(a, b):\n    return a + b" });
            PythonFiles.Add(new ScriptFile { FileName = "main.py", Content = "if __name__ == '__main__':\n    print('Hello World')" });
            PythonFiles.Add(new ScriptFile { FileName = "config.py", Content = "DB_HOST = 'localhost'\nDB_PORT = 5432" });
        }

        public Task LoadCobolFilesAsync()
        {
            LoadCobolFiles();
            return Task.CompletedTask;
        }

        public Task LoadPythonFilesAsync()
        {
            LoadPythonFiles();
            return Task.CompletedTask;
        }
    }
}
