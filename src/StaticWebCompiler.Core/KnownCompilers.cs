using Microsoft.Extensions.Logging;
using StaticWebCompiler.Compilers;

namespace StaticWebCompiler;

internal sealed class KnownCompilers
{
    private static Dictionary<string, ICompiler> _compilers = [];

    internal static IPageCompiler GetPageCompilerForExtension(FileInfo file)
    {
        if (_compilers.TryGetValue(file.Extension.ToLower(), out var compiler) == false)
        {
            throw new Exception($"No compiler found for {file.Extension} extension");
        }

        return (IPageCompiler)compiler;
    }
    internal static IContentCompiler? GetCompilerForExtension(FileInfo file)
    {
        if (_compilers.TryGetValue(file.Extension.ToLower(), out var compiler) == false)
        {
            //throw new Exception($"No compiler found for {file.Extension} extension");
            return null;
        }
        if (compiler is not IContentCompiler)
        {
            return null;
        }
        return (IContentCompiler)compiler;
    }

    internal static void Initialize(ILogger logger)
    {
        // pages
        _compilers.Add(".mdx", new Compilers.Markdown.MarkdownCompiler(logger));
        _compilers.Add(".md", new Compilers.Markdown.MarkdownCompiler(logger));
        _compilers.Add(".html", new Compilers.Html.HtmlCompiler(logger));
        _compilers.Add(".htm", new Compilers.Html.HtmlCompiler(logger));

        // assets
        _compilers.Add(".scss", new Compilers.Scss.ScssCompiler(logger));
    }
}
