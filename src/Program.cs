using System.CommandLine;
using System.Diagnostics;
using erd_dotnet;

var inputArgument = new Argument<string>("erdFile", "Erd text file (er format).");
var outputArgument = new Argument<string>("outputFile", "Output file (png or dot).");
var optionOutputFormat = new Option<string>(name: "--format", description: "Output format (png or dot)", getDefaultValue: () => "png");
optionOutputFormat.AddAlias("-f");
var optionReadDatabase = new Option<string>(name: "--database", description: "Database connection string (Postgres)");
optionReadDatabase.AddAlias("-d");
var rootCommand = new RootCommand("Dotnet Erd tool")
{
    inputArgument,
    outputArgument
};
rootCommand.AddGlobalOption(optionOutputFormat);
rootCommand.AddGlobalOption(optionReadDatabase);

rootCommand.SetHandler(async (optionDbValue, optionFormatValue, inputArgumentValue, outputArgumentValue) =>
{
    await Execute(inputArgumentValue, outputArgumentValue, optionFormatValue, optionDbValue);
}, optionReadDatabase, optionOutputFormat, inputArgument, outputArgument);

await rootCommand.InvokeAsync(args);

static async Task Execute(string input, string output, string format, string databaseConnection)
{
    if (!string.IsNullOrEmpty(databaseConnection))
    {
        await GenerateErdFromDatabase(databaseConnection, input);
    }
    GenerateFile(input, output, format);
}

static async Task GenerateErdFromDatabase(string connectionString, string erdFile)
{
    Erd erd = new Erd();
    if (File.Exists(erdFile))
    {
        var parser = new ErdParser();
        erd = parser.ParseFromFile(erdFile);
    }
    var reader = new PostgresReader(connectionString);
    erd = await reader.FetchErdEntities(erd);
    var erdWriter = new ErdWriter(erd);

    File.WriteAllLines(erdFile, erdWriter.GetStrings());
}

static void GenerateFile(string input, string output, string format)
{
    if (!File.Exists(input))
    {
        Console.WriteLine($"File {input} not found.");
        return;
    }

    var parser = new ErdParser();
    var erd = parser.ParseFromFile(input);

    var writer = new ErdDotWriter(erd);

    if (format.ToLowerInvariant() == "png")
    {
        var tempDot = Path.GetTempFileName();
        var tempPng = tempDot + ".png";
        writer.WriteFile(tempDot);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dot",
            Arguments = $"-Tpng {tempDot} -o {tempPng}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using (Process process = new Process())
        {
            process.StartInfo = startInfo;

            process.Start();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine(process.StandardError.ReadToEnd());
                return;
            }
            File.Move(tempPng, output, true);
            File.Delete(tempDot);
        }
    }
    else
    {
        writer.WriteFile(output);
    }
}
