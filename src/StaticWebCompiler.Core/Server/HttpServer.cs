using Microsoft.Extensions.Logging;
using System.Net;

namespace StaticWebCompiler.Server;

public sealed class HttpServer : IDisposable
{
    private readonly string _rootDirectory;
    private readonly ILogger _logger;
    private readonly HttpListener _listener = new ();
    private bool _run { get; set; } = true;

    public HttpServer(string rootDirectory, ILogger logger)
    {
        _rootDirectory = rootDirectory;
        _logger = logger;
    }
    public async Task Start()
    {
        string baseDirectory = _rootDirectory;
        string prefix = "http://localhost:8944/";
        _listener.Prefixes.Add(prefix);

        _logger.LogWarning($"Starting server at {prefix}");
        _listener.Start();

        while (_run)
        {
            try
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                await Task.Factory.StartNew(() => ProcessRequest(context, baseDirectory), TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
    public void Stop()
    {
        _run = false;

        if (_listener.IsListening)
            _listener.Stop();
    }
    public void Dispose()
    {
        Stop();
    }

    private async Task ProcessRequest(HttpListenerContext context, string baseDirectory)
    {
        string urlPath = context.Request.Url.AbsolutePath.TrimStart('/');
        string filePath = Path.Combine(baseDirectory, urlPath);

        if (File.Exists(filePath))
        {
            try
            {
                string contentType = GetContentType(filePath);
                byte[] fileBytes = File.ReadAllBytes(filePath);

                context.Response.ContentType = contentType;
                context.Response.ContentLength64 = fileBytes.Length;
                await context.Response.OutputStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                context.Response.OutputStream.Close();

                Console.WriteLine($"Served: {urlPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serving file: {ex.Message}");
                //RespondWithError(context, HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
        else
        {
            Console.WriteLine($"File not found: {urlPath}");
            //RespondWithError(context, HttpStatusCode.NotFound, "File Not Found");
        }
    }
    static string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".html" => "text/html",
            ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".png" => "image/png",
            ".jpeg" => "image/jpeg",
            ".jpg" => "image/jpeg",
            _ => "application/octet-stream",
        };
    }
}
