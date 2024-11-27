using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace StaticWebCompiler.Framework;

internal class Site
{
    private readonly string _sourcePath;
    private readonly ILogger _logger;

    private readonly Dictionary<string, string> _layouts = [];
    private readonly List<SiteItem> _siteItems = [];
    private readonly List<Page> _pages = [];

    internal Site(string sourcePath, ILogger logger)
    {
        _sourcePath = sourcePath.ToLower();
        _logger = logger;
    }

    private DirectoryInfo PagesDirectory => new (Path.Combine(_sourcePath, "pages"));
    private DirectoryInfo ThemeDirectory => new(Path.Combine(_sourcePath, "_theme"));

    internal async Task<Site> Inspect()
    {
        // load all files
        var pageFiles = PagesDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories);

        foreach (var file in pageFiles)
        {
            var logicalPath = file.FullName.Remove(0, PagesDirectory.FullName.Length + 1);

            _logger.LogDebug("Inspecting {0}", file.FullName);
            var compiler = KnownCompilers.GetPageCompilerForExtension(file);

            var slug = CreateSlug(file);
            Page page = await compiler.Compile(slug);
            _pages.Add(page);

            if (page.HideFromMenu == false)
            {
                _siteItems.Add(
                    new SiteItem(
                        page.MenuTitle ?? page.Title, GetParent(logicalPath), logicalPath, slug.Slug));
            }
        }

        // load all templates
        var themeFiles = ThemeDirectory.EnumerateFiles("*.htm?", SearchOption.TopDirectoryOnly);
        foreach (var themeFile in themeFiles)
        {
            _logger.LogDebug("Inspecting {file}", themeFile.FullName);
            var content = await File.ReadAllTextAsync(themeFile.FullName);

            _layouts.Add(themeFile.Name[..themeFile.Name.IndexOf(".")], content);
        }

        // return
        return this;
    }

    internal string GetLayoutOrDefault(string? name)
    {
        if (string.IsNullOrEmpty(name) == false && _layouts.TryGetValue(name, out var layout))
            return layout;

        if (_layouts.TryGetValue("_layout", out layout))
            return layout;

        return "";
    }

    internal IReadOnlyList<Page> Pages => _pages;
    internal IReadOnlyList<SiteItem> SiteItems => _siteItems;

    private string? GetParent(string physicalFile)
    {
        var parts = physicalFile.Split('\\');
        if (parts.Length == 1)
            return null;
        var parent = parts[^2];
        return parent;
    }
    private PageSlug CreateSlug(FileInfo fileInfo)
    {
        var file = fileInfo.FullName;
        var slug = file.Remove(0, PagesDirectory.FullName.Length+1);
        slug = slug.Replace("\\", "-");
        slug = slug[..slug.IndexOf(".")];

        return new PageSlug(file, slug + ".html");
    }
}
