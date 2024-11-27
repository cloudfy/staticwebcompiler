using Microsoft.Extensions.Logging;

namespace StaticWebCompiler.Cli.Commands;

internal static class BuildCommand
{
    public static async Task HandleAsync(string sourcePath, string? output, ILogger logger)
    {
        var builder = WebsiteCompiler.CreateBuilder(
            sourcePath
            , logger)
            .SetBuildPath(output ?? Path.Combine(Environment.CurrentDirectory, "build"));

        await builder.Build();
    }
}
