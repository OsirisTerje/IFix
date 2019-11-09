using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Fclp;

namespace IFix
{
    class Program
    {
        static int Main(string[] args)
        {
            var fclp = new FluentCommandLineParser();
            var setup = new SetupCommands(fclp);
            if (args == null || args.Length == 0)
            {
                DisplayVersion();
                Console.WriteLine(GetUsage());
                return -1;
            }
            var result = fclp.Parse(args);
            if (result.HasErrors)
            {
                Console.WriteLine($"Errors in parsing commands for {setup.ParsedOptions}");
                return 1;
            }
            if (result.HelpCalled)
            {
                DisplayVersion();
                var cmdstring = result.RawResult.Command;
                var cmd = fclp.Commands.FirstOrDefault(o => o.Name == cmdstring);
                Console.WriteLine($"Options for command: {cmdstring}");
                foreach (var c in cmd.Options)
                {
                    var msg = $"Short: -{c.ShortName}, Long: --{c.LongName}: Descr: {c.Description}";
                    Console.WriteLine(msg);
                }

                return 0;
            }
            if (setup.ParsedOptions == null)
            {
                Console.WriteLine("Unrecognized command");
                Console.WriteLine(GetUsage());
            }
            return 0;
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
            usage.AppendLine("(c) Terje Sandstrom (http://hermit.no) , 2015-2018");

            return usage.ToString();
        }

        private static void DisplayVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("IFix  " + version);
        }
    }
}
