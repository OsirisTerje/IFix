using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace IFix
{
    public class GitIgnore
    {
        private GitIgnoreCommand Command { get; set; }

        public int Execute(GitIgnoreCommand command)
        {
            Command = command;
            int retval = 0;
            string here = Directory.GetCurrentDirectory();
            var stdGitIgnore = RetrieveStdGitIgnore().ToList();
            string[] repositories = Directory.GetDirectories(here, ".git", SearchOption.AllDirectories);
            foreach (var repository in repositories)
            {
                var dir = Directory.GetParent(repository);
                var filetocheck = dir + @"/.gitignore";
                if ((Command.Check || Command.Fix || Command.Add) && !Command.Merge && !Command.Replace)
                {
                    if (File.Exists(filetocheck))
                    {
                        var lines = File.ReadAllLines(filetocheck).ToList();
                        if (!CheckIfNuGetPackages(lines))
                        {
                            Writer.WriteRed("Missing 'packages' or 'packages/' or 'packages/*' or '**/packages/*' in ignorelist for " + filetocheck);
                            if (Command.Fix && !Command.Add)
                            {
                                AddMissingInfo(lines, command.LatestGitVersion);
                                File.WriteAllLines(filetocheck, lines);
                                Writer.Write("Fixed " + filetocheck);
                            }
                            retval++;

                        }
                        else
                        {
                            if (Command.Verbose)
                            {
                                Writer.WriteGreen("Ok : " + filetocheck);
                            }
                        }
                    }
                    else
                    {
                        Writer.WriteRed("No .gitignore in " + repository);
                        if (Command.Fix || Command.Add)
                        {
                            File.WriteAllLines(filetocheck, stdGitIgnore);
                            Writer.Write("Added " + filetocheck);
                        }
                        retval++;
                    }
                }
                else // Commands replace or merge
                {
                    if (Command.Replace)
                    {
                        var exist = File.Exists(filetocheck);
                        File.WriteAllLines(filetocheck, stdGitIgnore);
                        string msg = string.Format("{0} gitignore with standard for {1}", exist ? "Replaced" : "Added",
                            dir);
                        Writer.Write(msg);
                        
                    }
                    else  // merge
                    {
                        if (File.Exists(filetocheck))
                        {
                            var lines = File.ReadAllLines(filetocheck).ToList();
                            var missing = CheckIfOurContainsStd(lines, stdGitIgnore).ToList();
                            if (missing.Any())
                            {
                                if (Command.Fix)
                                {
                                    lines.AddRange(missing);
                                    File.WriteAllLines(filetocheck, lines);
                                }
                                Writer.Write(Command.Fix?"Added "+missing.Count()+" lines to .gitignore for "+dir:"Missing "+missing.Count()+" lines to gitignore for "+dir);
                                retval++;
                            }
                            else 
                                Writer.Write("No change needed for "+dir);
                        }
                        else
                        {
                            if (Command.Fix)
                                File.WriteAllLines(filetocheck, stdGitIgnore);
                            Writer.Write(string.Format("{0} gitignore for {1}",Command.Fix?"Added":"Missing",dir));
                            retval++;
                        }
                    }
                }
                
            }
            return retval;
        }

        private IEnumerable<string> RetrieveStdGitIgnore()
        {
            var temp = Path.GetTempPath();
            var tempgitignore = temp + "/VisualStudio.gitignore";
            DownloadGitIgnore(tempgitignore);
            var stdGitIgnore = File.ReadAllLines(tempgitignore);
            return stdGitIgnore;
        }

        public void AddMissingInfo(ICollection<string> lines,bool latest=false)
        {
            lines.Add(@"# NuGet Packages");
            lines.Add(@"**/packages/*");
            if (!latest)
                lines.Add(@"packages/*");
            lines.Add(@"*.nupkg");
            if (!latest)
                lines.Add(@"!**/packages/build/");
            lines.Add(@"!packages/build/");
            
        }

        public bool CheckIfNuGetPackages(IEnumerable<string> lines)
        {
            return lines.Any(line => line.Trim()=="packages/" || line.Trim()=="packages/*" || line.Trim()=="packages");
        }

        public bool CheckIfNDepend(IEnumerable<string> lines)
        {
            return lines.Any(line => line.Trim() == "NDependOut/");
        }

        public void DownloadGitIgnore(string path)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/github/gitignore/raw/master/VisualStudio.gitignore",
                    path);
            }
        }

        public bool CheckIfNotEqual(IEnumerable<string> ourGitIgnore, IEnumerable<string> stdGitIgnore)
        {
            return ourGitIgnore.Count() == stdGitIgnore.Count();
        }

        public IEnumerable<string> CheckIfOurContainsStd(IEnumerable<string> ourGitIgnore, IEnumerable<string> stdGitIgnore)
        {
            return stdGitIgnore.Where(line => !ourGitIgnore.Contains(line)).ToList();
        }

        
    }
}
