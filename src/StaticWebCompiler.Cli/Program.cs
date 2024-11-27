using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace StaticWebCompiler.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        /*
         * swccli build --output
         * 
        */

        var buildCommand = new Command("build", "Compile and build the static website.");
        var srcOption = new Option<string>(
            "--src"
            , () => Path.Combine(Environment.CurrentDirectory, "src")
            , "Source directory of the build. Defaults to 'src'.");
        var outputOption = new Option<string?>("--output", "Output directory of the build. Defaults to 'build'.");
        buildCommand.Add(srcOption);
        buildCommand.Add(outputOption);

        buildCommand.SetHandler<string, string?, ILogger>(
            async (src, output, logger) => {
                await Commands.BuildCommand.HandleAsync(src, output, logger);
            }
            , srcOption
            , outputOption
            , new LoggerBinder());

        var runCommand = new Command("run", "Compile, build and run the static website");
        runCommand.AddOption(srcOption);
        runCommand.AddOption(outputOption);

        var portOption = new Option<int?>("--port", "Port to run the server on. Defaults to 8944.");
        runCommand.AddOption(portOption);

        var noWatchOption = new Option<bool?>("--nowatch", "Do not watch for changes and rebuild the website.");
        runCommand.AddOption(noWatchOption);

        runCommand.SetHandler<string, string?, int?, ILogger>(
            async (src, output, port, logger) =>
            {
                await Commands.RunCommand.HandleAsync(src, output, port, logger);
            }
            , srcOption
            , outputOption
            , portOption
            , new LoggerBinder());

        var initCommand = new Command("init", "Initialize a new workspace");
        initCommand.AddOption(outputOption);
        initCommand.SetHandler<string, ILogger>(
            async (output, logger) => {
                await Commands.InitCommand.HandleAsync(output, logger);
            }
            , outputOption
            , new LoggerBinder());

        var rootCommand = new RootCommand("Build and compile static websites.");
        rootCommand.AddCommand(buildCommand);
        rootCommand.AddCommand(runCommand);
        // rootCommand.AddCommand(initCommand);

        return await rootCommand.InvokeAsync(args);
    }
}