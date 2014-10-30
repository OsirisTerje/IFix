using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLine;

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
        public virtual string Help()
        {
            var usage = new StringBuilder();
            usage.AppendLine(
                "Use :  -c or --check to only check without fixing, or -f or --fix to check and fix the errors");
            usage.AppendLine(
                "Use option -v or --verbose in addition to get more information.  Default for this is false.");
            return usage.ToString();
        }

        public abstract int Execute();

        public virtual bool ValidOptions()
        {
            return Check || Fix;
        }
    }

    public class Options
    {
       

        [VerbOption("nugetrestore")]
        public NuGetRestoreCommand NuGetRestore { get; set; }

        [VerbOption("ca0053")]
        public FixCA0053Command FixCa0053Command { get; set; }

        [VerbOption("gitignore")]
        public GitIgnoreCommand GitIgnoreCommand { get; set; }

        [VerbOption("blog")]
        public GoToBlog BlogCommand { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            usage.AppendLine("IFix  "+version);
            usage.AppendLine("Usage: IFix  <command> [-c](Check only) [-f](Fix)  [-v](Verbose mode");
            usage.AppendLine("where <command> is one of :  nugetrestore,  ca0053, gitignore, blog");
            usage.AppendLine("For more instructions and information see blogpost at http://geekswithblogs.net/terje/archive/2014/06/13/fixing-up-visual-studiorsquos-gitignore--using-ifix.aspx");
            usage.AppendLine("or use: IFix blog -c");
            usage.AppendLine("by Terje Sandstrom, Inmeta Consulting, 2014");
            
            return usage.ToString();
        }
    }

    public class GoToBlog : CommonOptions
    {
        public override int Execute()
        {
            var b = new Process
            {
                StartInfo =
                    new ProcessStartInfo(
                        "http://geekswithblogs.net/terje/archive/2014/06/13/fixing-up-visual-studiorsquos-gitignore--using-ifix.aspx")
            };
            b.Start();
            return 0;
        }
    }

    public class NuGetRestoreCommand : CommonOptions
    {
        public override int Execute()
        {
            var fixer = new RemoveOldNugetRestore(this);
            return fixer.Execute();
        }
    }

    public class FixCA0053Command : CommonOptions
    {
        public override int Execute()
        {
            var fixer = new FixCA0053();
            return fixer.Execute(this);
        }
    }

    public class GitIgnoreCommand : CommonOptions
    {
        [Option('a', "add", HelpText = "Only add latest standard public .gitignore when missing, don't fix up the others.")]
        public bool Add { get; set; }

        [Option('m', "merge", HelpText = "Get and merge information from the  standard public gitignore file")]
        public bool Merge { get; set; }

        [Option('r', "replace", HelpText = "Replace the existing instead of merging in the latest, applies to all gitignore files")]
        public bool Replace { get; set; }

        [Option('l',"latestgit",HelpText = "Use for compatibility with the latest git version 2.0.1. If you have git < 2.0.1 leave this out.")]
        public bool LatestGitVersion { get; set; }

        [HelpOption]
        public override string Help()
        {
            var sb = new StringBuilder(base.Help());
            sb.AppendLine("use -a or --add to only add in missing gitignores. Use instead of fix");
            sb.AppendLine("use -m or --merge to check or fix comparing/merging with latest visual studio gitignore file. Works with check and fix options");
            sb.AppendLine(
                "use -r or --replace to replace existing gitignores, or add where missing, with the latest visual studio gitignore file");
            sb.AppendLine("use -l or --latestgit if you have version >= 2.0.1 of git, then the gitignore will be a few lines shorter. Latest Windows git is currently 1.9.4");
            return sb.ToString();
        }

        public override int Execute()
        {
            var fixer = new GitIgnore(Directory.GetCurrentDirectory());
            return fixer.Execute(this);
        }

        public override bool ValidOptions()
        {
            var ok = base.ValidOptions();
            ok |= (Add || Replace);
            return ok;
        }
    }
}
