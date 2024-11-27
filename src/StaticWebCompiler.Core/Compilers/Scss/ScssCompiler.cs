using Microsoft.Extensions.Logging;
using SharpScss;

namespace StaticWebCompiler.Compilers.Scss;

internal sealed class ScssCompiler : IContentCompiler
{
    private readonly ILogger _logger;

    internal ScssCompiler(ILogger logger) => _logger = logger;

    public ILogger Logger => _logger;

    public Task<(string Content, string? FileName)> Compile(FileInfo file)
    {
        try
        {
            var result = SharpScss.Scss.ConvertFileToCss(file.FullName, new ScssOptions
            {
                OutputStyle = ScssOutputStyle.Compact
            });

            return Task.FromResult<(string, string?)>((result.Css, Path.GetFileNameWithoutExtension(file.Name) + ".css"));
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message);
            return Task.FromResult<(string, string?)>(("", null));
        }
    }
}
