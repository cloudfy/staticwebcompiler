using StaticWebCompiler.Framework;

namespace StaticWebCompiler.Compilers;

internal interface IPageCompiler : ICompiler
{
    Task<Page> Compile(PageSlug fileInfo);
}
