namespace StaticWebCompiler.Framework;

internal record SiteItem(
    string Text
    , string? Parent
    , string PhysicalPath
    , string Slug)
{
}
