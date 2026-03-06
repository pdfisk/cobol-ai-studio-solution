using Shared;
using Newtonsoft.Json;

namespace BlazorLib.Interop
{
    public interface IInteropApi
    {
        List<ScriptFile> GetCobolFiles();
        List<ScriptFile> GetPythonFiles();

        Task LoadCobolFilesAsync();
        Task LoadPythonFilesAsync();
    }

    public class InteropApi : IInteropApi
    {
        private readonly HttpClient _httpClient;
        private bool _cobolInitialized;
        private bool _pythonInitialized;

        private List<ScriptFile> CobolFiles { get; } = new();
        private List<ScriptFile> PythonFiles { get; } = new();

        public InteropApi(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public List<ScriptFile> GetCobolFiles()
        {
            return CobolFiles;
        }

        public List<ScriptFile> GetPythonFiles()
        {
            return PythonFiles;
        }

        public async Task LoadCobolFilesAsync()
        {
            if (_cobolInitialized)
            {
                return;
            }

            _cobolInitialized = true;
            CobolFiles.Clear();

            try
            {
                var response = await _httpClient.GetAsync(Constants.CobolEndpoint);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var files = JsonConvert.DeserializeObject<List<ScriptFile>>(json);
                if (files != null)
                {
                    CobolFiles.AddRange(files);
                }
            }
            catch
            {
                _cobolInitialized = false;
                throw;
            }
        }

        public async Task LoadPythonFilesAsync()
        {
            if (_pythonInitialized)
            {
                return;
            }

            _pythonInitialized = true;
            PythonFiles.Clear();

            try
            {
                var response = await _httpClient.GetAsync(Constants.PythonEndpoint);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var files = JsonConvert.DeserializeObject<List<ScriptFile>>(json);
                if (files != null)
                {
                    PythonFiles.AddRange(files);
                }
            }
            catch
            {
                _pythonInitialized = false;
                throw;
            }
        }
    }
}
