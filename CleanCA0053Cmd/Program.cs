using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace CleanCA0053Cmd
{
    using System.IO;
    using System.Reflection;

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Version: " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("RemoveOldNugetRestore: Converts all projects in a directory tree to remove nuget.target files and converts all csproj files.");
            Console.WriteLine("by Terje Sandstrom, Inmeta Consulting, 2014");
            Console.WriteLine("For instructions see blogpost at");
            Console.WriteLine();
            var oldNugetRestore = new RemoveOldNugetRestore();
            oldNugetRestore.Execute();
        }


    }


    public class RemoveOldNugetRestore
    {
        private bool changed;
        public void Execute()
        {


            string here = Directory.GetCurrentDirectory();

            RemoveAllNugetTargetFiles(here);
            FixSolutionFiles(here);

            FixingCsprojFiles(here);
        }

        private void FixingCsprojFiles(string here)
        {
            int skipped = 0;
            int fixedup = 0;
            int nowrite = 0;
            Console.WriteLine("Fixing csproj files");
            var filePaths = Directory.GetFiles(here, "*.csproj",
                SearchOption.AllDirectories);
            foreach (var file in filePaths)
            {
                changed = false;
                var lines = File.ReadAllLines(file);
                var output = FixImportAndRestorePackagesInCsproj(lines, file);
                var output2 = FixTargetInCsproj(output);
                try
                {
                    if (changed)
                    {
                        File.WriteAllLines(file, output2);
                        fixedup++;
                    }
                    else
                    {
                        skipped++;
                    }
                    Console.WriteLine("{0} : {1}", (changed) ? "Fixed" : "Skipped", file);
                }
                catch
                {
                    Console.WriteLine("Unable to write to :" + file);
                    nowrite++;
                }
            }
            Console.WriteLine("Fixed : " + fixedup);
            Console.WriteLine("Skipped : " + skipped);
            if (nowrite > 0)
                Console.WriteLine("Unable to write :" + nowrite);
            int total = fixedup + skipped;
            Console.WriteLine("Total files checked : " + total);
            Console.WriteLine("Finished fixing csproj files");
        }

        private IEnumerable<string> FixImportAndRestorePackagesInCsproj(IEnumerable<string> lines, string file)
        {
            var output = new List<string>();


            foreach (var line in lines)
            {
                if (
                    !((line.Contains(@"<Import Project") && line.Contains("NuGet.targets")) ||
                      line.Contains("<RestorePackages>true</RestorePackages>")))
                {
                    output.Add(line);
                }
                else
                {
                    var msg = string.Format("Removed {0} in {1}",
                        line.Contains("Import") ? "Import Project line" : "RestorePackages line", file);
                    Console.WriteLine(msg);
                    changed = true;
                }
            }
            return output;
        }

        private static IEnumerable<string> FixTargetInCsproj(IEnumerable<string> output)
        {
            var output2 = new List<string>();
            bool foundTarget = false;
            foreach (var line in output)
            {
                if (line.Contains("<Target Name=\"EnsureNuGetPackageBuildImports"))
                {
                    foundTarget = true;
                }
                else
                {
                    if (foundTarget)
                    {
                        if (line.Contains("</Target>"))
                            foundTarget = false;
                    }
                    else
                    {
                        output2.Add(line);
                    }
                }
            }
            return output2;
        }

        /// <summary>
        /// Delete all the nuget.target files found.  Optionally copy the relevant info on external nuget repositories to the nuget.config file.  
        /// </summary>
        /// <param name="here"></param>
        private void RemoveAllNugetTargetFiles(string here)
        {
            string[] filePaths = Directory.GetFiles(here, "nuget.targets",
                                         SearchOption.AllDirectories);
            foreach (var file in filePaths)
            {
                CheckAndCopyNugetPaths(file);
                File.Delete(file);
                Console.WriteLine("Deleted file: {0}", file);
            }
        }

        private void CheckAndCopyNugetPaths(string file)
        {
            var lines = File.ReadAllLines(file);
            var nugetpaths = new List<string>();
            string previousline="";
            bool comment = false;
            foreach (var line in lines)
            {
                if (line.Contains("PackageSource"))
                {
                    if (!comment)
                    {
                        var url = ExtractUrl(line);
                        nugetpaths.Add(url);
                    }
                }
                comment = IsCommentLine(line);  // Extract comment if previous line is a starting comment line with no ending comment
            }
        }

        public string ExtractUrl(string line)
        {
            if (line.Contains("PackageSource"))
            {
                int index = line.IndexOf("http", System.StringComparison.Ordinal);
                if (index < 0)
                    return "";
                var path = line.Substring(index);
                int count = path.IndexOf('"');
                path = path.Substring(0, count);
                return path;
            }
            return "";
        }

        public bool IsCommentLine(string line)
        {
            var line2 = line.Trim();
            return line2.StartsWith("<!--") && !line2.EndsWith("-->");
        }

        private static void FixSolutionFiles(string here)
        {
            Console.WriteLine("Fixing solution files");
            int count = 0;
            string[] slnFilePaths = Directory.GetFiles(here, "*.sln", SearchOption.AllDirectories);
            foreach (var file in slnFilePaths)
            {
                var text = File.ReadAllLines(file);
                var outlines = new List<string>();
                bool found = false;
                foreach (var line in text)
                {
                    if (!line.Contains(@".nuget\NuGet.targets"))
                        outlines.Add(line);
                    else
                    {
                        found = true;
                    }
                }
                if (found)
                {
                    File.WriteAllLines(file, outlines);
                    count++;
                }
                string msg = string.Format("{0} checked. {1}", file, found ? "Nuget.target removed" : "Skipped, nothing found");
                Console.WriteLine(msg);
            }
            Console.WriteLine("Fixing {0} solution files finished", count);
        }


    }





}
