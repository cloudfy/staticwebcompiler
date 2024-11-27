using Microsoft.Extensions.Logging;
using StaticWebCompiler.Compilers;
using StaticWebCompiler.Framework;

namespace StaticWebCompiler;

public sealed class WebsiteCompiler
{
    private readonly string _sourcePath;
    private string _buildPath { get; set; }
    private readonly ILogger _logger;

    private readonly Dictionary<string, ICompiler> _compilers = [];

    private WebsiteCompiler(string sourcePath, ILogger logger)
    {
        _sourcePath = sourcePath;
        _buildPath = "build";
        _logger = logger;

        // setup builders
        KnownCompilers.Initialize(logger);
    }

    internal string SourcePath => _sourcePath;

    public static WebsiteCompiler CreateBuilder(string sourcePath, ILogger logger)
        => new (sourcePath, logger);
  
    public async Task Build()
    {
        _logger.LogInformation("StaticWebCompiler: Building website from {sourcePath} to {buildPath}", _sourcePath, _buildPath);
        _logger.LogWarning("Starting build...");

        // remove build
        ReadyBuild();

        // collect site
        var site = new Site(_sourcePath, _logger);
        await site.Inspect();

        // handlebars
        var outputCompiler = new Compilers.Html.HandlebarCompiler();

        // build pages
        foreach (var page in site.Pages)
        {
            _logger.LogInformation($"Compiling {page.Slug}");

            // merge template and compile content
            var layoutContent = site.GetLayoutOrDefault(page.Layout);
            var pagePreCompileContent = layoutContent.Replace("{{content}}", page.Content);
            var compiledContent = outputCompiler.Compile(pagePreCompileContent, page);

            // store file in build
            await WriteFile(page.Slug, compiledContent);
        }

        // compile assets
        var assetsFolder = new DirectoryInfo(Path.Combine(_sourcePath, "assets"));
        foreach (var assetFile in assetsFolder.EnumerateFiles("*.*", SearchOption.AllDirectories))
        {
            var compiler = KnownCompilers.GetCompilerForExtension(assetFile);
            if (compiler is not null)
            {
                _logger.LogInformation($"Compiling asset: {assetFile.Name}");

                var compileResult = await compiler.Compile(assetFile);

                var outputAssetFilePath = Path.Combine(_buildPath, assetFile.FullName.Remove(0, _sourcePath.Length + 1));
                var outputAssetPath = Path.GetDirectoryName(outputAssetFilePath)!;
                if (compileResult.FileName is not null)
                {
                    outputAssetFilePath = Path.Combine(outputAssetPath, compileResult.FileName);
                }
                if (Directory.Exists(outputAssetPath) == false)
                    Directory.CreateDirectory(outputAssetPath);
                await File.WriteAllTextAsync(outputAssetFilePath, compileResult.Content);
            }
            else
            {
                var outputAssetFilePath = Path.Combine(_buildPath, assetFile.FullName.Remove(0, _sourcePath.Length + 1));
                var outputAssetPath = Path.GetDirectoryName(outputAssetFilePath)!;
                if (Directory.Exists(outputAssetPath) == false)
                    Directory.CreateDirectory(outputAssetPath);

                File.Copy(assetFile.FullName, outputAssetFilePath, true);
            }
        }

        // copy content
        string[] copyContents = ["images", "content", "lib", "scripts"];

        foreach (var asset in copyContents)
        {
            string assetPath = Path.Combine(_sourcePath, asset);
            if (Directory.Exists(assetPath))
            {
                _logger.LogInformation($"Copying asset: {asset}");

                IO.IOHelper.CopyDirectory(assetPath, Path.Combine(_buildPath, asset));
            }
        }
    }

    private void ReadyBuild()
    {
        if (Directory.Exists(_buildPath))
        {
            Directory.Delete(_buildPath, true);
        }
        Directory.CreateDirectory(_buildPath);
    }

    public async Task WriteFile(string filePath, string content)
    {
        var fullPath = Path.Combine(_buildPath, filePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
        await File.WriteAllTextAsync(fullPath, content);
    }

    public WebsiteCompiler SetBuildPath(string buildPath)
    {
        _buildPath = buildPath;
        return this;
    }
}
