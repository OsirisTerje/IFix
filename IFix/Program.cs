﻿using System;
using System.Collections.Generic;
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
                    var msg = $"Short: {c.ShortName}, Long: {c.LongName}: Descr: {c.Description}";
                    Console.WriteLine(msg);
                }

                return 0;
            }
            if (setup.ParsedOptions == null)
            {
                Console.WriteLine("Unrecognized command");
            }
            return 0;
        }

        public static string GetUsage()
        {
            var usage = new StringBuilder();
            DisplayVersion();
            usage.AppendLine("Usage: IFix  <command> [-c](Check only) [-f](Fix)  [-v](Verbose mode");
            usage.AppendLine("where <command> is one of :  gitignore, diagnostics, mefcache, vstestcache, nugetconsolidate, createsln, nugetrestore,  ca0053, info");
            usage.AppendLine("For more instructions and information run 'IFix info -c'");
            usage.AppendLine("or one of IFix info --gitignore/--nugetrestore/--ca0053/--mefcache/--nugetconsolidate -c");
            usage.AppendLine("by Terje Sandstrom, 2015-2018");

            return usage.ToString();
        }

        private static void DisplayVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("IFix  " + version);
        }
    }
}