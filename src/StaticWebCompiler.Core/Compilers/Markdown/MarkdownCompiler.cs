using Microsoft.Extensions.Logging;
using StaticWebCompiler.Framework;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using System.Text.RegularExpressions;
using StaticWebCompiler.Extensions;

namespace StaticWebCompiler.Compilers.Markdown;

internal class MarkdownCompiler : IPageCompiler
{
    private readonly ILogger _logger;

    internal MarkdownCompiler(ILogger logger) => _logger = logger;

    public async Task<Page> Compile(PageSlug pageSlug)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .Build();
        var context = new MarkdownParserContext();

        var mdxContent = await File.ReadAllTextAsync(pageSlug.FullPath);

        var yamlContent = ExtractYamlFrontmatter(mdxContent);
        var keyValuePairs = ParseYamlToDictionary(yamlContent);

        var html = Markdig.Markdown.ToHtml(mdxContent, pipeline, context);
        
        return new Page(
            keyValuePairs.GetValueOrDefault("title", "Untitled")
            , html
            , keyValuePairs.GetValueOrNull("layout")
            , keyValuePairs.GetValueOrNull("keywords")
            , keyValuePairs.GetValueOrNull("description")
            , pageSlug.Slug);
    }

    public ILogger Logger => _logger;

    static string ExtractYamlFrontmatter(string content)
    {
        // Regex to match content between ---
        var regex = new Regex(@"^---\s*([\s\S]*?)\s*---", RegexOptions.Multiline);
        var match = regex.Match(content);

        if (!match.Success)
        {
            throw new Exception("YAML frontmatter not found!");
        }

        return match.Groups[1].Value.Trim();
    }
    static Dictionary<string, string> ParseYamlToDictionary(string yamlContent)
    {
        var dictionary = new Dictionary<string, string>();
        var lines = yamlContent.Split('\n');

        foreach (var line in lines)
        {
            // Match "key: value" format
            var match = Regex.Match(line, @"^(?<key>\w+):\s*(?<value>.+)?$");
            if (match.Success)
            {
                var key = match.Groups["key"].Value.Trim();
                var value = match.Groups["value"].Value.Trim();
                dictionary[key] = value;
            }
        }

        return dictionary;
    }
}
