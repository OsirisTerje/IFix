using System;
using System.Reflection;
using System.Text;
using CommandLine;
using NuGet;

namespace IFix
{


    public abstract class CommonOptions
    {
        [Option('c', "check", HelpText = "Only check if we have an issue. No changes done")]
        public bool Check { get; set; }


        [Option('f', "fix", HelpText = "Fix all files with issues")]
        public bool Fix { get; set; }

        [Option('v', "verbose", HelpText = "More verbose output")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string Help()
        {
            var usage = new StringBuilder();
            usage.AppendLine(
                "Use :  -c or --check to only check without fixing, or -f or --fix to check and fix the errors");
            usage.AppendLine(
                "Use option -v or --verbose in addition to get more information.  Default for this is false.");
            return usage.ToString();
        }

        public abstract void Execute();
    }

    public class Options
    {
       

        [VerbOption("nugetrestore")]
        public NuGetRestoreCommand NuGetRestore { get; set; }

        [VerbOption("ca0053")]
        public FixCA0053Command FixCa0053Command { get; set; }

        [VerbOption("gitignore")]
        public GitIgnoreCommand GitIgnoreCommand { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            usage.AppendLine("IFix  "+version);
            usage.AppendLine("IFix: Fixes common solution and project file issues");
            usage.AppendLine("Verbs:  NuGetRestore: Converts all projects in a directory tree to remove old nuget restore, by removing nuget.target and exe files and converts all csproj and sln files.");
            usage.AppendLine("Usage: IFix  <command> [-c](Check only) [-f](Fix)  [-v](Verbose mode");
            usage.AppendLine("where <command> is one of :  nugetrestore,  ca0053, gitignore");
            usage.AppendLine("For more instructions and information see blogpost at http://geekswithblogs.net/Terje");
            usage.AppendLine("by Terje Sandstrom, Inmeta Consulting, 2014");
            
            return usage.ToString();
        }
    }

    public class NuGetRestoreCommand : CommonOptions
    {
        public override void Execute()
        {
            var fixer = new RemoveOldNugetRestore(this);
            fixer.Execute();
        }
    }

    public class FixCA0053Command : CommonOptions
    {
        public override void Execute()
        {
            var fixer = new FixCA0053();
            fixer.Execute(this);
        }
    }

    public class GitIgnoreCommand : CommonOptions
    {
        [Option('a', "add", HelpText = "Only add latest standard public .gitignore when missing, don't fix up the others.")]
        public bool Add { get; set; }

        [Option('m', "merge", HelpText = "Get and merge information from the  standard public gitnore file")]
        public bool Merge { get; set; }

        [Option('r', "replace", HelpText = "Replace the existing instead of merging in the latest, applies to all gitignore files")]
        public bool Replace { get; set; }

        

        public override void Execute()
        {
            var fixer = new GitIgnore();
            fixer.Execute(this);
        }
    }
}
