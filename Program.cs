using System.CommandLine;
using erd_dotnet;

var inputArgument = new Argument<string>("inputFile", "Text file (er format).");
var outputArgument = new Argument<string>("outputFile", "Output file (png or txt).");
var optionOutputFormat = new Option<string>(name: "--format", description: "Output format (png or txt)", getDefaultValue: () => "png");
optionOutputFormat.AddAlias("-f");
var rootCommand = new RootCommand("Dotnet Erd to png")
{
    inputArgument,
    outputArgument
};
rootCommand.AddGlobalOption(optionOutputFormat);

rootCommand.SetHandler((optionFormatValue, inputArgumentValue, outputArgumentValue) =>
{
    GenerateFile(inputArgumentValue, outputArgumentValue, optionFormatValue);
}, optionOutputFormat, inputArgument, outputArgument);

await rootCommand.InvokeAsync(args);


void GenerateFile(string input, string output, string format)
{
    var text = File.ReadAllLines(input);
    var parser = new ErdParser();
    var erd = parser.Parse(text);

    var writer = new ErdDotWriter(erd);

    if (format == "png")
    {
        var tempDot = Path.GetTempFileName();
        writer.WriteFile(tempDot);
        System.Diagnostics.Process.Start("dot", $"-Tpng {tempDot} -o {output}");
    }
    else 
    {
        writer.WriteFile(output);
    }
}
