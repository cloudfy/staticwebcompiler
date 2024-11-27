namespace StaticWebCompiler.Framework;

/// <summary>
/// 
/// </summary>
/// <param name="Title">Title of the page.</param>
/// <param name="Content">Content. Pre-compiled (ex. Markdown > HTML).</param>
/// <param name="Layout">Layout file to use.</param>
/// <param name="Keywords">Key words.</param>
/// <param name="Description">Description.</param>
/// <param name="Slug">Virtual path aka slug.</param>
internal record Page
    (string Title
    , string? Content
    , string? Layout
    , string? Keywords
    , string? Description
    , string Slug
    , string? MenuTitle = null
    , bool HideFromMenu = false)
{
}
