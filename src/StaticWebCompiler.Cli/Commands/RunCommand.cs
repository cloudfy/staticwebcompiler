using Microsoft.Extensions.Logging;
using StaticWebCompiler.IO;
using StaticWebCompiler.Server;

namespace StaticWebCompiler.Cli.Commands;

internal static class RunCommand
{
    public static async Task HandleAsync(string srcDirectory, string buildDirectory, ILogger logger)
    {
        WebsiteCompiler compiler = WebsiteCompiler
            .CreateBuilder(srcDirectory, logger)
            .SetBuildPath(buildDirectory);

        ReCompileHandler reCompileHandler = new(compiler, logger);
        reCompileHandler.Start();

        using var server = new HttpServer(buildDirectory, logger);
        await server.Start();

    }
}