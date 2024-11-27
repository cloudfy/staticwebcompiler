using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace StaticWebCompiler.Compilers.Html;

internal class HandlebarCompiler
{
    private readonly IHandlebars _handlebars;

    internal HandlebarCompiler()
    {
        //var config = new HandlebarsConfiguration()
        //{
        //};
        //config.BlockHelpers.AddOrReplace("if", new IfHelper());
       
        _handlebars = Handlebars.Create();
      //  _handlebars.Configuration.BlockHelpers.AddOrReplace("if", new IfHelper());
        _handlebars.RegisterHelper("when", (output,options,context,arguments) => {

            if (arguments.Length < 2)
            {
                throw new HandlebarsException("Helper 'greaterThan' requires two arguments");
            }

            // Check if the arguments are not equal
            var arg1 = arguments[0]?.ToString();
            var arg2 = arguments[1]?.ToString();

            if (arg1 == arg2)
            {
                // Render the inner block if not equal
                options.Template(output, context);
            }
            else
            {
                // Render the "else" block if equal
                options.Inverse(output, context);
            }

        }); //.Configuration.Helpers.AddOrReplace("if", new IfHelper2());
    }

    internal string Compile(string template, object content)
    {
        var compiler = _handlebars.Compile(template);
        return compiler(content);
    }
}

public class IfHelper : IHelperDescriptor<BlockHelperOptions>
{
    public PathInfo Name => throw new NotImplementedException();

    public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
    {
        throw new NotImplementedException();
    }

    public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
    {
        // Ensure the correct number of arguments
        if (arguments.Length < 2)
        {
            throw new HandlebarsException("Helper 'greaterThan' requires two arguments");
        }

        // Check if the arguments are not equal
        var arg1 = arguments[0]?.ToString();
        var arg2 = arguments[1]?.ToString();

        if (arg1 == arg2)
        {
            // Render the inner block if not equal
            options.Template(output, context);
        }
        else
        {
            // Render the "else" block if equal
            options.Inverse(output, context);
        }
    }
}