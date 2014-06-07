﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IFix
{
    public class GitIgnore
    {
        private GitIgnoreCommand Command { get; set; }

        public void Execute(GitIgnoreCommand command)
        {
            Command = command;
            string here = Directory.GetCurrentDirectory();
            var temp = Path.GetTempPath();
            var tempgitignore = temp + "/VisualStudio.gitignore";
            DownloadGitIgnore(tempgitignore);
            var stdGitIgnore = File.ReadAllLines(tempgitignore);
            string[] repositories = Directory.GetDirectories(here, ".git", SearchOption.AllDirectories);
            foreach (var repository in repositories)
            {
                var dir = Directory.GetParent(repository);
                var filetocheck = dir + @"/.gitignore";
                if (Command.Check || Command.Fix || Command.Add)
                {
                    if (File.Exists(filetocheck))
                    {
                        var lines = File.ReadAllLines(filetocheck).ToList();
                        if (!CheckIfPackages(lines))
                        {
                            Writer.WriteRed("Missing 'packages/' in ignorelist for " + filetocheck);
                            if (Command.Fix && !Command.Add)
                            {
                                AddMissingInfo(lines);
                                File.WriteAllLines(filetocheck, lines);
                                Writer.Write("Fixed " + filetocheck);
                            }

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
                    }
                }
                else // Commands replace or latest
                {
                    if (Command.Replace)
                    {
                        File.WriteAllLines(filetocheck, stdGitIgnore);
                        Writer.Write("Replaced gitignore with standard for " + dir);
                    }
                    else
                    {
                        if (File.Exists(filetocheck))
                        {
                            var lines = File.ReadAllLines(filetocheck).ToList();
                            var missing = CheckIfOurContainsStd(lines, stdGitIgnore).ToList();
                            if (missing.Any())
                            {
                                lines.AddRange(missing);
                                File.WriteAllLines(filetocheck, lines);
                                Writer.Write("Added "+missing.Count()+" lines to .gitignore for "+dir);
                            }
                            else 
                                Writer.Write("No change needed for "+dir);
                        }
                        else
                        {
                            File.WriteAllLines(filetocheck, stdGitIgnore);
                            Writer.Write("Added gitignore for "+dir);
                        }
                    }
                }
            }

        }

        public void AddMissingInfo(ICollection<string> lines)
        {
            lines.Add(@"# NuGet Packages");
            lines.Add(@"packages/*");
            lines.Add(@"*.nupkg");
            lines.Add(@"!packages/build/");
        }

        public bool CheckIfPackages(IEnumerable<string> lines)
        {
            return lines.Any(line => line.Trim()=="packages/" || line.Trim()=="packages/*");
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
            if (ourGitIgnore.Count() != stdGitIgnore.Count())
                return false;
            return true;
        }

        public IEnumerable<string> CheckIfOurContainsStd(IEnumerable<string> ourGitIgnore, IEnumerable<string> stdGitIgnore)
        {
            return stdGitIgnore.Where(line => !ourGitIgnore.Contains(line)).ToList();
        }

        
    }
}
