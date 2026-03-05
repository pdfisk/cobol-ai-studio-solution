using BlazorLib.Interop;
using BlazorLib.Server;

var server = new SimpleHttpServer("http://localhost:8080/", new InteropApi());
server.Start();

using (server)
{
    Console.WriteLine("Server running at http://localhost:8080/. Press Ctrl+C to stop.");
    var exitEvent = new ManualResetEventSlim(false);
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        exitEvent.Set();
    };
    exitEvent.Wait();
}
