using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;

namespace IFix
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                DisplayVersion();
                Console.WriteLine(GetUsage());
                return -1;
            }
            var result = Parser.Default.ParseArguments<
                GoToBlog,
                NuGetRestoreCommand,
                GitIgnoreCommand,
                VSTestCacheCommands,
                MefCacheCommand,
                CreateSln,
                NugetConsolidateCommands,
                DiagnosticsCommands
            >(args)
                    .WithParsed<GoToBlog>(options=>options.Execute())
                    .WithParsed<NuGetRestoreCommand>(options => options.Execute())
                    .WithParsed<GitIgnoreCommand>(options => options.Execute())
                    .WithParsed<VSTestCacheCommands>(options => options.Execute())
                    .WithParsed<MefCacheCommand>(options => options.Execute())
                    .WithParsed<CreateSln>(options => options.Execute())
                    .WithParsed<NugetConsolidateCommands>(options => options.Execute())
                    .WithParsed<DiagnosticsCommands>(options => options.Execute())
                    .WithNotParsed(HandleErrors)
                ;
            
            
            
            return 0;
        }

        public static void HandleErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
            Environment.Exit(1);
        }

        public static string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Usage: IFix  <command> [command_specific_options]  [-c](Check only) [-f](Fix)  [-v](Verbose mode");
            usage.AppendLine("where <command> is one of :  ");
            usage.AppendLine("    gitignore");
            usage.AppendLine("    diagnostics");
            usage.AppendLine("    mefcache");
            usage.AppendLine("    vstestcache");
            usage.AppendLine("    nugetconsolidate");
            usage.AppendLine("    createsln");
            usage.AppendLine("    nugetrestore");
            usage.AppendLine("    ca0053");
            usage.AppendLine("    info");
            usage.AppendLine("For more instructions and information run 'IFix info', or for specific commands 'IFix <command> -?' , or 'IFix info -? ");
            usage.AppendLine("(c) Terje Sandstrom (https://hermit.no) , 2015-2024");

            return usage.ToString();
        }

        private static void DisplayVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("IFix  " + version);
        }
    }
}
