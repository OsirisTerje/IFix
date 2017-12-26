using System;
using Fclp;

namespace IFix
{
    public class SetupCommands
    {
        private FluentCommandLineParser parser;
        public bool DoExecute { get; set; } = true;

        public string HelpText { get; private set; }

        public SetupCommands(FluentCommandLineParser parser)
        {
            this.parser = parser;
          
            SetupGitIgnore();
            SetupMefCache();
            SetupNugetRestore();
            SetupCa0053();
            SetupNugetConsolidate();
            SetupCreateSln();
            SetupDiagnostics();
            SetupVSTestCache();
            parser.SetupHelp("?", "help").Callback(text => HelpText = text);  //Console.WriteLine(text));
        }
        public void SetupGitIgnore()
        {
            var gi = parser.SetupCommand<GitIgnoreCommand>("gitignore").OnSuccess(Execute);
            gi.Setup(args => args.Replace).As('r', "replace").SetDefault(false).WithDescription("Replace the existing instead of merging in the latest, applies to all gitignore files");
            gi.Setup(args => args.Add).As('a', "add").SetDefault(false).WithDescription("Only add latest standard public .gitignore when missing, don't fix up the others");
            gi.Setup(args => args.Merge).As('m', "merge").SetDefault(false).WithDescription("Get and merge information from the  standard public gitignore file");
            gi.Setup(args => args.LatestGitVersion).As('l', "latest").SetDefault(false).WithDescription("Use for compatibility with the latest git version 2.0.1. If you have git < 2.0.1 leave this out");
            gi.Setup(args => args.Strict).As('s', "strict").SetDefault(false).WithDescription("Ensure that the most extensive pattern is used");
            Setup(gi);
        }

        public void SetupMefCache()
        {
            var gi = parser.SetupCommand<MefCacheCommand>("mefcache").OnSuccess(Execute);
            gi.Setup(args => args.Vs2012).As('2', "VS2012").SetDefault(false).WithDescription("Delete VS2012 cache");
            gi.Setup(args => args.Vs2013).As('3', "VS2013").SetDefault(false).WithDescription("Delete VS2013 cache");
            gi.Setup(args => args.Vs2015).As('5', "VS2015").SetDefault(false).WithDescription("Delete VS2015 cache");
            gi.Setup(args => args.Vs2017).As('7', "VS2017").SetDefault(false).WithDescription("Delete VS2017 cache");
            gi.Setup(args => args.All).As('a', "All").SetDefault(false).WithDescription("Delete cache for all VS instances");
            Setup(gi);

        }

        public void SetupNugetRestore()
        {
            var gi = parser.SetupCommand<NuGetRestoreCommand>("nugetrestore").OnSuccess(Execute);
            Setup(gi);
        }

        public void SetupCa0053()
        {
            var gi = parser.SetupCommand<FixCA0053Command>("ca0053").OnSuccess(Execute);
            Setup(gi);
        }

        public void SetupNugetConsolidate()
        {
            var gi = parser.SetupCommand<NugetConsolidateCommands>("nugetconsolidate").OnSuccess(Execute);
            gi.Setup(args => args.Consolidate).As('o', "consolidate").SetDefault(false).WithDescription("Only list packages that needs consolidation");
            gi.Setup(args => args.Short).As('s', "short").SetDefault(false).WithDescription("Only list packages and versions, skip the project information");
            Setup(gi);
        }

        public void SetupCreateSln()
        {
            var gi = parser.SetupCommand<CreateSln>("createsln").OnSuccess(Execute);
            gi.Setup(args => args.Blank).As('b', "blank").SetDefault(false).WithDescription("Create a complete blank solution");
            gi.Setup(args => args.Name).As('n', "name").SetDefault("BlankSln").WithDescription("Enter a name for the new sln");
            Setup(gi);
        }

        public void SetupDiagnostics()
        {
            var gi = parser.SetupCommand<DiagnosticsCommands>("diagnostics").OnSuccess(Execute);
            gi.Setup(args => args.Show).As('s', "show").SetDefault(false).WithDescription("Show all diagnostics settings");
            gi.Setup(args => args.EnableDisableDump).As('d', "dump").SetDefault(-1).WithDescription("Enable or Disable dump");
            gi.Setup(args => args.EnableDisableFuslog).As('u', "fuslog").SetDefault(-1).WithDescription("Enable or Disable fuslog");
            gi.Setup(args => args.DumpFolder).As('D', "dumpfolder").SetDefault(@"C:\dumps").WithDescription("Set dumpfolder");
            gi.Setup(args => args.FuslogFolder).As('U', "fuslogfolder").SetDefault(@"C:\fuslog").WithDescription("Set fuslog logfolder");
            Setup(gi);
        }

        public void SetupVSTestCache()
        {
            var gi = parser.SetupCommand<VSTestCacheCommands>("vstestcache").OnSuccess(Execute);
            Setup(gi);
        }



        public Options ParsedOptions { get; private set; }

        public void Execute(Options args)
        {
            ParsedOptions = args;
            if (DoExecute)
                args.Execute();
        }


        private void Setup<T>(Fclp.ICommandLineCommandFluent<T> cmd) where T : Options
        {
            cmd.Setup(args => args.Fix).As('f', "fix").SetDefault(false).WithDescription("Fix the issue");
            cmd.Setup(args => args.Check).As('c', "check").SetDefault(false).WithDescription("Check only");
            cmd.Setup(args => args.Verbose).As('v', "verbose").SetDefault(false).WithDescription("Enable verbose mode");
        }

    }
}