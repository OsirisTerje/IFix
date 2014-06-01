using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace RemoveOldNugetRestore
{
    public class Options
    {
        [Option('c',"check",HelpText="Only check if we have an issue. No changes done")]
        public bool Check { get; set; }


        [Option('f',"fix",HelpText="Fix all files with issues")]
        public bool Fix { get; set; }

        [Option('v',"verbose",HelpText="More verbose output")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            usage.AppendLine("RemoveOldNugetRestore  "+version);
            usage.AppendLine("RemoveOldNugetRestore: Converts all projects in a directory tree to remove nuget.target files and converts all csproj files.");
            usage.AppendLine("by Terje Sandstrom, Inmeta Consulting, 2014");
            usage.AppendLine("Usage: RemoveOldNugetRestore [-c](Check only) [-x](Perform correction)");
            usage.AppendLine("For more instructions and information see blogpost at");
            return usage.ToString();
        }
    }
}
