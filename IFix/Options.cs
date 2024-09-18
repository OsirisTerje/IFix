using System.Diagnostics;
using System.IO;
using CommandLine;

namespace IFix
{


    public abstract class Options
    {
        [Option('c', "check", HelpText = "Only check if we have an issue. No changes done")]
        public bool Check { get; set; }


         [Option('f', "fix", HelpText = "Fix all files with issues")]
        public bool Fix { get; set; }

        [Option('v', "verbose", HelpText = "More verbose output")]
        public bool Verbose { get; set; }

        // [HelpOption]
        // string Help();
        //{
        //    var usage = new StringBuilder();
        //    usage.AppendLine(
        //        "Use :  -c or --check to only check without fixing, or -f or --fix to check and fix the errors");
        //    usage.AppendLine(
        //        "Use option -v or --verbose in addition to get more information.  Default for this is false.");
        //    return usage.ToString();
        //}

        public abstract int Execute();

        public virtual bool ValidOptions()
        {
            return Check || Fix;
        }
    }

    [Verb("info")]
    public class GoToBlog : Options
    {
        private const string UrlIFix = @"https://visualstudiogallery.msdn.microsoft.com/b8ba97b0-bb89-4c21-a1e2-53ef335fd9cb";

        private const string Urlgitignore = @"https://hermit.no/fixing-up-visual-studio-rsquo-s-gitignore-using-ifix/";

        private const string UrlNugetrestore = @"https://hermit.no/converting-projects-to-use-automatic-nuget-restore-using-ifix/";

        private const string UrlCa0053 = @"https://hermit.no/how-to-fix-the-ca0053-error-in-code-analysis-in-visual-studio-2012/";

        private const string UrlMefcache = @"https://hermit.no/how-to-fix-visual-studio-loading-errors-using-ifix/";

        private const string UrlCreateSln = @"https://hermit.no/ifix-create-solution-skeleton-file/";
        public override int Execute()
        {
            string url = UrlIFix;
            if (GitIgnore)
                url = Urlgitignore;
            else if (NuGetRestore)
                url = UrlNugetrestore;
            else if (Ca0053)
                url = UrlCa0053;
            else if (MefCache)
                url = UrlMefcache;
            else if (CreateSln)
                url = UrlCreateSln;

            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            return 0;
        }

        [Option('g', "gitignore", HelpText = "gitignore blog")]
        public bool GitIgnore { get; set; }

        [Option('n', "nugetrestore", HelpText = "nugetrestore blog")]
        public bool NuGetRestore { get; set; }
        [Option('a', "ca0053", HelpText = "ca0053 blog")]
        public bool Ca0053 { get; set; }
        [Option('e', "mefcache")]
        public bool MefCache { get; set; }
        [Option('s', "createsln", HelpText = "Creates a blank solution file")]
        public bool CreateSln { get; set; }

    }

    [Verb("nugetrestore")]
    public class NuGetRestoreCommand : Options
    {
        public override int Execute()
        {
            var fixer = new RemoveOldNugetRestore(this);
            return fixer.Execute();
        }
    }

    [Verb("fixCa0053")]
    public class FixCA0053Command : Options
    {
        public override int Execute()
        {
            var fixer = new FixCA0053();
            return fixer.Execute(this);
        }
    }

    [Verb("CreateSln")]
    public class CreateSln : Options
    {
        public override int Execute()
        {
            var fixer = new CreateBlankSolution();
            return fixer.Execute(this);
        }

        [Value(0)]
        public string Name { get; set; }

        [Option('b',"blank",HelpText="Create a complete blank solution")]
        public bool Blank { get; set; }
    }

    [Verb("MefCache")]
    public class MefCacheCommand : Options
    {
        public override int Execute()
        {
            var fixer = new DeleteMefCache();
            return fixer.Execute(this);
        }
        [Option('2',"vs2012",HelpText="Delete cache for VS2012")]
        public bool Vs2012 { get; set; }
        [Option('3', "vs2013", HelpText = "Delete cache for VS2013")]
        public bool Vs2013 { get; set; }
        [Option('5', "vs2015", HelpText = "Delete cache for VS2015")]
        public bool Vs2015 { get; set; }

        [Option('7', "vs2017", HelpText = "Delete cache for VS2017")]
        public bool Vs2017 { get; set; }

        [Option('a', "all", HelpText = "Delete cache for all VS instances")]
        public bool All { get; set; }

        public bool NotSpecific => !Vs2012 && !Vs2013 && !Vs2015;

        //public  string Help()
        //{
        //    var msg = base.Help();
        //    msg += "Use options --vs2012/--vs2013/--vs2015/--vs2017 to limit cache deletion to one version, --all to clear all";
        //    return msg;
        //}
    }

    [Verb("VSTestcache")]
    public class VSTestCacheCommands : Options
    {
        public override int Execute()
        {
            var fixer = new DeleteVSTestCache();
            return fixer.Execute(this);
        }
    }

    [Verb("gitignore")]
    public class GitIgnoreCommand : Options
    {
        [Option('a', "add", HelpText = "Only add latest standard public .gitignore when missing, don't fix up the others.")]
        public bool Add { get; set; }

        [Option('m', "merge", HelpText = "Get and merge information from the  standard public gitignore file")]
        public bool Merge { get; set; }

        [Option('r', "replace", HelpText = "Replace the existing instead of merging in the latest, applies to all gitignore files")]
        public bool Replace { get; set; }

        [Option('l',"latestgit",HelpText = "Use for compatibility with the latest git version 2.0.1. If you have git < 2.0.1 leave this out.")]
        public bool LatestGitVersion { get; set; } = true;

        [Option('s',"strict",HelpText="Ensure that the most extensive pattern is used")]
        public bool Strict { get; set; }

        //[HelpOption]
        //public override string Help()
        //{
        //    var sb = new StringBuilder(base.Help());
        //    sb.AppendLine("use -a or --add to only add in missing gitignores. Use instead of fix");
        //    sb.AppendLine("use -m or --merge to check or fix comparing/merging with latest visual studio gitignore file. Works with check and fix options");
        //    sb.AppendLine("use -s or --strict to ensure the most extensive pattern is used.  Combine with -f");
        //    sb.AppendLine(
        //        "use -r or --replace to replace existing gitignores, or add where missing, with the latest visual studio gitignore file");
        //    sb.AppendLine("use -l or --latestgit if you have version >= 2.0.1 of git, then the gitignore will be a few lines shorter. Latest Windows git is currently 1.9.4");
        //    return sb.ToString();
        //}

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

    [Verb("NugetConsolidate")]
    public class NugetConsolidateCommands : Options
    {


        [Option('s', "short", HelpText = "Only list packages and versions, skip the project information")]
        public bool Short { get; set; }

        [Option('o', "consolidate", HelpText = "Only list packages that needs consolidation")]
        public bool Consolidate { get; set; }

        [Option('p', "packageconfig", HelpText = "Use for when you have package.config based projects")]
        public bool PackageConfig { get; set; } = false;
        public override int Execute()
        {
            INugetConsolidate fixer = PackageConfig ? new NugetConsolidateOld() : new NugetConsolidate();
            return fixer.Execute(this);
        }
    }

    [Verb("Diagnostics")]
    public class DiagnosticsCommands : Options
    {

        [Option('s',"show",HelpText = "Show all diagnostics settings")]
        public bool Show { get; set; }

        [Option('d', "Dump", Default = -1, HelpText = "Enable or Disable dump")]
        public int EnableDisableDump { get; set; }

        [Option('D', "Dumpfolder", Default = "",HelpText = "Set dumpfolder")]
        public string DumpFolder { get; set; }

        [Option('u', "Fuslog", Default = -1, HelpText = "Enable or Disable fuslog")]
        public int EnableDisableFuslog { get; set; }

        [Option('U', "Fuslogfolder", Default = "", HelpText = "Set fuslog logfolder")]
        public string FuslogFolder { get; set; }


        [Option('t', "Vstest", Default = -1, HelpText =
        "Enable or Disable vstest tracing, 0 = disable, 1 = enable discovery 2= enable execution, 3 = enable both")]
        public int VSTestTracing { get; set; }


        public bool HasDumpCommand => EnableDisableDump != -1;
        public bool HasFuslogCommand => EnableDisableFuslog != -1;
        public bool HasVSTestCommand => VSTestTracing != -1;


        public override int Execute()
        {
            var diag = new Diagnostics();
            return diag.Execute(this);
        }
    }
}
