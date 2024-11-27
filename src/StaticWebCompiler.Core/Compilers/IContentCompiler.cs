namespace StaticWebCompiler.Compilers;

internal interface IContentCompiler : ICompiler
{
    Task<(string Content, string? FileName)> Compile(FileInfo file);
}