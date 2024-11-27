using Microsoft.Extensions.Logging;

namespace StaticWebCompiler.IO;

public sealed class ReCompileHandler : IDisposable
{
    private readonly WebsiteCompiler _compiler;
    private readonly ILogger _logger;
    private static bool _isCompiling = false;
    internal FileSystemWatcher _watcher;

    public ReCompileHandler(WebsiteCompiler compiler, ILogger logger)
    {
        _watcher = new(compiler.SourcePath)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            IncludeSubdirectories = true
        };
        _watcher.Changed += Watcher_Changed;

        _compiler = compiler;
        _logger = logger;
    }

    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
    }
    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (_isCompiling == false)
        {
            _logger.LogInformation("Re-compiling website...");

            _isCompiling = true;
            Task.Factory.StartNew(async () => await _compiler.Build(), TaskCreationOptions.PreferFairness);
            //_compiler.Build().Wait();
            _isCompiling = false;
        }
    }

    public void Dispose()
    {
        Stop();
        _watcher.Dispose();
    }
}
