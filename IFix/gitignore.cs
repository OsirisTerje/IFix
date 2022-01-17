using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;


namespace IFix
{


    public class Repository
    {
        private readonly string gitdirectory;

        public DirectoryInfo GitRepo => Directory.GetParent(gitdirectory);

        public string File => GitRepo + @"/.gitignore";

        public Repository(string repo)
        {
            gitdirectory = repo;
        }
    }

    public class GitIgnore
    {
        public GitIgnoreCommand GitIgnoreCommand { get; private set; }

        private List<string> stdGitIgnore;
        public IEnumerable<Repository> Repositories { get; }


        /// <summary>
        /// For testing purposes only
        /// </summary>
        //public GitIgnore()
        //    : this(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"../../../")))
        //{

        //}

        public GitIgnore(string currentdirectory)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            RetrieveStdGitIgnore();
            var reponames = new List<string>(Directory.GetDirectories(currentdirectory, ".git", SearchOption.AllDirectories));
            Repositories = reponames.Select(r => new Repository(r));
        }

        /// <summary>
        /// For use within IFix, from other commands, only works on current directory. Requires that the Fix option is set
        /// </summary>
        /// <param name="gitIgnoreCommand"></param>
        public GitIgnore(GitIgnoreCommand gitIgnoreCommand)
        {
            GitIgnoreCommand = gitIgnoreCommand;
            RetrieveStdGitIgnore();
            
        }

        public void WriteGitIgnore()
        {
            if (!GitIgnoreCommand.Fix)
                return;
            File.WriteAllLines(".gitignore", stdGitIgnore);
        }


        public int Execute(GitIgnoreCommand command)
        {
#if DEBUG
            Console.Write("A chance to attach the debugger");
            Console.ReadKey();
#endif
            GitIgnoreCommand = command;
            int retval = 0;

            if (!Repositories.Any())
                Writer.Write("No git subfolders found. Run IFix from either above your git repos or from within root folder of your git repo.");
            foreach (var repository in Repositories)
            {
                int thisrepo = 0;
                Writer.Write("Checking: " + repository.GitRepo);
                if (GitIgnoreCommand.Replace)
                {
                    ExecuteReplace(repository);
                }
                else if (GitIgnoreCommand.Merge)
                {
                    thisrepo += ExecuteMerge(repository);
                }
                else if ((GitIgnoreCommand.Check || GitIgnoreCommand.Fix || GitIgnoreCommand.Add))
                {
                    thisrepo += ExecuteFixAdd(command, repository);
                }
                if (thisrepo == 0 && !command.Verbose)
                    Writer.WriteGreen("-->is Ok<--");
                retval += thisrepo;
            }
            return retval;
        }

        private int ExecuteFixAdd(GitIgnoreCommand command, Repository repo)
        {
            int retval = 0;
            var filetocheck = repo.File;
            if (File.Exists(filetocheck))
            {
                var lines = File.ReadAllLines(filetocheck).ToList();
                retval = FixNuGet(command, lines, filetocheck, retval);
            }
            else
            {
                Writer.WriteRed("No .gitignore in " + repo.GitRepo+"\nUse IFix gitignore -r -f to retrieve the standard gitignore");
                if (GitIgnoreCommand.Fix || GitIgnoreCommand.Add)
                {
                    File.WriteAllLines(filetocheck, stdGitIgnore);
                    Writer.Write("Added " + filetocheck);
                }
                retval++;
            }
            return retval;
        }

        private int ExecuteMerge(Repository repo)
        {
            int retval = 0;
            if (File.Exists(repo.File))
            {

                var lines = File.ReadAllLines(repo.File).ToList();
                var missing = CheckIfOurContainsStd(lines, stdGitIgnore).ToList();
                if (missing.Any())
                {
                    if (GitIgnoreCommand.Fix)
                    {
                        lines.AddRange(missing);
                        File.WriteAllLines(repo.File, lines);
                    }
                    Writer.Write(GitIgnoreCommand.Fix
                        ? "Added " + missing.Count + " lines to .gitignore for " + repo.GitRepo
                        : "Missing " + missing.Count + " lines to gitignore for " + repo.GitRepo);
                    retval++;
                }
                else
                    Writer.Write("No change needed for " + repo.GitRepo);
            }
            else
            {
                if (GitIgnoreCommand.Fix)
                    File.WriteAllLines(repo.File, stdGitIgnore);
                Writer.Write($"{(GitIgnoreCommand.Fix ? "Added" : "Missing")} gitignore for {repo.GitRepo}");
                retval++;
            }
            return retval;
        }

        private void ExecuteReplace(Repository repository)
        {
            var exist = File.Exists(repository.File);
            File.WriteAllLines(repository.File, stdGitIgnore);
            string msg = $"{(exist ? "Replaced" : "Added")} gitignore with standard for {repository.GitRepo}";
            Writer.Write(msg);
        }

        private int FixNuGet(GitIgnoreCommand command, IReadOnlyCollection<string> lines, string filetocheck, int retval)
        {
            var outlines = new List<string>();
            bool fix = false;
            bool green = true;
            if (!CheckIfNuGetPackages(lines,GitIgnoreCommand.Strict,GitIgnoreCommand.LatestGitVersion))
            {
                if (!GitIgnoreCommand.Strict)
                    Writer.WriteRed(
                        "Missing 'packages' or 'packages/' or 'packages/*' or '**/packages/*' in ignorelist for " + filetocheck);
                else
                {
                    if (!GitIgnoreCommand.LatestGitVersion)
                        Writer.WriteRed("Missing either/or both '**/packages/*' and 'packages/*'  in ignorelist for " + filetocheck);
                    else 
                        Writer.WriteRed("Missing  '**/packages/*'  in ignorelist for " + filetocheck);
                }
                if (GitIgnoreCommand.Fix && !GitIgnoreCommand.Add)
                {
                    outlines.AddRange(AddOnlyMissingInfo(lines, command.LatestGitVersion));
                    lines = outlines;
                    fix = true;
                   
                }
                retval++;
                green = false;
            }
            else
            {
                var reincludes = CheckIfNuGetPackagesAllowReincludes(lines);
                if (!reincludes && command.Verbose)
                {
                    Writer.Write("Warning: Reincludes does not work with this pattern '" + GetNuGetPackageCommand(lines) +"'");
                    Writer.Write("See http://geekswithblogs.net/terje/archive/2014/07/03/gitignorendashhow-to-exclude-nuget-packages-at-any-level-and-make.aspx for more information");
               
                }
            }

            if (!CheckIfVS2015Files(lines))
            {
                Writer.WriteRed("Missing node_modules  in "+filetocheck);
                if (GitIgnoreCommand.Fix && !GitIgnoreCommand.Add)
                {
                    outlines.AddRange(new List<string> { "node_modules/"});
                    fix = true;
                }
                green = false;
                retval++;

            } 
            if (green)
                Writer.WriteGreen("Ok : " + filetocheck);
            if (!fix) 
                return retval;
            File.WriteAllLines(filetocheck, outlines);
            Writer.Write("Fixed " + filetocheck);
            return retval;
        }

        public bool CheckIfVS2015Files(IReadOnlyCollection<string> lines) => lines.Any(line => line.Contains("node_modules/"));

        private void RetrieveStdGitIgnore()
        {
            var temp = Path.GetTempPath();
            var tempgitignore = temp + "/VisualStudio.gitignore";
            if (!File.Exists(tempgitignore) || (DateTime.Now.Subtract(File.GetLastWriteTime(tempgitignore)).Hours > 24))
                DownloadGitIgnore(tempgitignore);
            stdGitIgnore = new List<string>(File.ReadAllLines(tempgitignore));

        }

        /// <summary>
        /// Adds the missing info as a block at the end of the file
        /// </summary>
        //public void AddMissingInfo(ICollection<string> lines, bool latest = false)
        //{
        //    lines.Add(@"# NuGet Packages");
        //    lines.Add(@"*.nupkg");
        //    lines.Add(@"**/packages/*");
        //    if (!latest)
        //        lines.Add(@"packages/*");
        //    if (!latest)
        //        lines.Add(@"!**/packages/build/");
        //    lines.Add(@"!packages/build/");

        //}

        /*
                enum GitIgnoreParsing { Start, FoundNuGet, FoundNupkg, FoundPackagesFirst, FoundPackagesSecond, FoundReincludeBuild };
        */

        /// <summary>
        /// Adds the missing info line by line in-place, removes commented lines for NuGet which are no longer needed
        /// </summary>
        public IReadOnlyCollection<string> AddOnlyMissingInfo(IReadOnlyCollection<string> lines, bool latest = false)
        {
            const string nuGetStart = @"# NuGet Packages";
            const string nuGetPackageFull = @"**/packages/*";
            const string nuGetPackage194 = @"packages/*";
            const string nuGetPkg = @"*.nupkg";
            const string nuGetReincludeBuild = @"!packages/build/";
            const string nuGetReincludeBuild2 = @"!**/packages/build/";
            var outlines = new Collection<string>();
            foreach (var line in lines)
                outlines.Add(line);
            var patterns = new List<Tuple<string, bool>>
            {
                new Tuple<string, bool>(nuGetStart,true),
                new Tuple<string, bool>(nuGetPkg,false),
                new Tuple<string, bool>(nuGetPackageFull,false),
                new Tuple<string, bool>(nuGetPackage194,false),
                new Tuple<string, bool>(nuGetReincludeBuild,false),
                new Tuple<string, bool>(nuGetReincludeBuild2,false)
            };
            string lastpattern = "";
            foreach (var pattern in patterns)
            {
                string line = pattern.Item2 ?
                    outlines.FirstOrDefault(l => l.Trim().Contains(pattern.Item1)) :
                    outlines.FirstOrDefault(l => l.Trim() == pattern.Item1);
                if (string.IsNullOrEmpty(line))
                {
                    if (string.IsNullOrEmpty(lastpattern))
                        outlines.Add(pattern.Item1);
                    else
                    {
                        int index = outlines.IndexOf(lastpattern);
                        if (index == outlines.Count())
                            outlines.Add(pattern.Item1);
                        else
                            outlines.Insert(index + 1, pattern.Item1);
                        line = pattern.Item1;
                    }
                }
                lastpattern = line;
            }
            // Removed commented lines, no longer needed
            outlines.Remove("## TODO: If you have NuGet Package Restore enabled, uncomment the next line");
            outlines.Remove("#packages/");
            return outlines;
        }

        public bool CheckIfNuGetPackages(IReadOnlyCollection<string> lines, bool strict, bool latest)
        {
            if (!strict)
                return lines.Any(line => line.Trim() == "packages/" || line.Trim() == "packages/*" || line.Trim() == "packages" || line.Trim() == "**/packages/*");
            return lines.Any(line => line.Trim() == "**/packages/*") &&
                    (latest || lines.Any(line => line.Trim() == "packages/*"));

        }

        public bool CheckIfNuGetPackagesAllowReincludes(IEnumerable<string> lines)
        {
            return lines.Any(line => line.Trim() == "packages/*");
        }

        /// <summary>
        /// Use to find which command doesnt allow reincludes.  It returns only the first one. 
        /// </summary>
        public string GetNuGetPackageCommand(IEnumerable<string> lines)
        {
            var cmd = lines.FirstOrDefault(line => line.Trim().Contains("packages"));
            return cmd;
        }


        public bool CheckIfNDepend(IEnumerable<string> lines)
        {
            return lines.Any(line => line.Trim() == "NDependOut/");
        }

        public void DownloadGitIgnore(string path)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/github/gitignore/raw/main/VisualStudio.gitignore",
                    path);
            }
        }

        public bool CheckIfNotEqual(IEnumerable<string> ourGitIgnore, IEnumerable<string> stdgitIgnore)
        {
            return ourGitIgnore.Count() == stdgitIgnore.Count();
        }

        public IEnumerable<string> CheckIfOurContainsStd(IEnumerable<string> ourGitIgnore, IEnumerable<string> stdgitIgnore)
        {
            return stdgitIgnore.Where(line => !ourGitIgnore.Contains(line)).ToList();
        }


    }
}
