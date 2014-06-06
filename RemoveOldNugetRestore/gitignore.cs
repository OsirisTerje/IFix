using System;
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
        private GitIgnoreOptions Options { get; set; }

        public void Execute(GitIgnoreOptions options)
        {
            Options = options;
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
                if (File.Exists(filetocheck))
                {
                    var lines = File.ReadAllLines(filetocheck).ToList();
                    if (!CheckIfPackages(lines))
                    {
                        Writer.WriteRed("Missing packages in ignorelist");
                        if (Options.Fix)
                        {
                            AddMissingInfo(lines);
                            File.WriteAllLines(filetocheck, lines);
                            Writer.Write("Fixed " + filetocheck);
                        }

                    }
                    else
                    {
                        if (Options.Verbose)
                        {
                            Writer.WriteGreen("Ok : " + filetocheck);
                        }
                    }
                }
                else
                {
                    Writer.WriteRed("No .gitignore in " + repository);
                    if (Options.Fix)
                    {
                        DownloadGitIgnore(filetocheck);
                        Writer.Write("Added " + filetocheck);
                    }
                }
            }

        }

        private void AddMissingInfo(ICollection<string> lines)
        {
            lines.Add(@"# NuGet Packages");
            lines.Add(@"packages/*");
            lines.Add(@"*.nupkg");
        }

        private bool CheckIfPackages(IEnumerable<string> lines)
        {
            return lines.Any(line => line.Trim() == "packages/");
        }

        private void DownloadGitIgnore(string path)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/github/gitignore/blob/master/VisualStudio.gitignore",
                    path);
            }
        }
    }
}
