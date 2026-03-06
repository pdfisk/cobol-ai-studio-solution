using System.Net;
using System.Text;
using BlazorLib.Interop;
using Newtonsoft.Json;

namespace BlazorLib.Server
{
    public class SimpleHttpServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly IInteropApi _interopApi;
        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsListening => _listener.IsListening;

        public SimpleHttpServer(string[] prefixes, IInteropApi interopApi)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this operating system.");
            }

            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("At least one prefix must be specified.", nameof(prefixes));
            }

            _interopApi = interopApi ?? throw new ArgumentNullException(nameof(interopApi));

            _listener = new HttpListener();
            foreach (var prefix in prefixes)
            {
                _listener.Prefixes.Add(prefix);
                _listener.Prefixes.Add(prefix.EndsWith("/") ? prefix : prefix + "/");
            }
        }

        public SimpleHttpServer(string prefix, IInteropApi interopApi)
            : this(new[] { prefix }, interopApi)
        {
        }

        public void Start()
        {
            if (_listener.IsListening)
            {
                return;
            }

            _listener.Start();
            _cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Run(() => RunServer(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }

        private async Task RunServer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync().ConfigureAwait(false);
                    _ = ProcessRequestAsync(context);
                }
                catch (HttpListenerException)
                {
                    // Listener stopped
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Listener disposed
                    break;
                }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var url = request.RawUrl?.TrimStart('/');

            AddCorsHeaders(response);

            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
                return;
            }

            try
            {
                var responseString = HandleRequest(url ?? string.Empty);
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            }
            finally
            {
                response.Close();
            }
        }

        private static void AddCorsHeaders(HttpListenerResponse response)
        {
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
        }

        private string HandleRequest(string url)
        {
            List<ScriptFile>? reply = null;
            switch (url)
            {
                case "cobol":
                    reply = _interopApi.GetCobolFiles();
                    break;
                case "python":
                    reply = _interopApi.GetPythonFiles();
                    break;
            }

            if (reply != null)
            {
                return JsonConvert.SerializeObject(reply);
            }

            return JsonConvert.SerializeObject(new { status = "error", message = "Unknown endpoint", url });
        }

        public void Dispose()
        {
            Stop();
            _listener.Close();
            _cancellationTokenSource?.Dispose();
        }
    }
}

