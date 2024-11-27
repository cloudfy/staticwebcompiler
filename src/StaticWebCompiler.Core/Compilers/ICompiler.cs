using Microsoft.Extensions.Logging;

namespace StaticWebCompiler.Compilers;

internal interface ICompiler 
{
    ILogger Logger { get; }
}
