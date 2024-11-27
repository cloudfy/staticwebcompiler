using Microsoft.Extensions.Logging;
using System.CommandLine.Binding;

namespace StaticWebCompiler.Cli;

public class LoggerBinder : BinderBase<ILogger>
{
    protected override ILogger GetBoundValue(
        BindingContext bindingContext) => GetLogger(bindingContext);

    ILogger GetLogger(BindingContext bindingContext)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder => builder.AddConsole());
        ILogger logger = loggerFactory.CreateLogger("LoggerCategory");
        return logger;
    }
}