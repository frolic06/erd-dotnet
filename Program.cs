using System;
using System.IO;

namespace erd_dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: erd-csharp [path-to-input-file] [path-to-output-file]");
                Console.WriteLine("Where:");
                Console.WriteLine("  - [path-to-input-file]     Text file (er format)");
                Console.WriteLine("  - [path-to-output-file]    Output file (.png)");
                return;
            }
            var text = File.ReadAllLines(args[0]);
            var parser = new ErdParser();
            var erd = parser.Parse(text);
            
            var writer = new ErdDotWriter(erd);
            var tempDot = Path.GetTempFileName();
            writer.WriteFile(tempDot);
            System.Diagnostics.Process.Start("dot", $"-Tpng {tempDot} -o {args[1]}");
        }
    }
}

