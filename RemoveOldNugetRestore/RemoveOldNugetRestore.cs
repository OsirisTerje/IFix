using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveOldNugetRestore
{
    public class RemoveOldNugetRestore
    {
        private bool changed;

        Options Options { get; set; }

        public RemoveOldNugetRestore(Options options)
        {
            Options = options;
        }

        /// <summary>
        /// For testing
        /// </summary>
        public RemoveOldNugetRestore()
        {
            Options = new Options();
        }

        public void Execute()
        {


            string here = Directory.GetCurrentDirectory();

            RemoveAllNugetTargetFiles(here);
            RemoveAllNugetExeFiles(here);
            FixSolutionFiles(here);

            FixingCsprojFiles(here);
        }

        public void RemoveAllNugetExeFiles(string here)
        {
            Console.WriteLine("Checking Nuget.exe which is under a .nuget folder");
            var filePaths = Directory.GetFiles(here, "nuget.exe", SearchOption.AllDirectories).Where(item => item.Contains(".nuget"));
            var enumerable = filePaths as IList<string> ?? filePaths.ToList();
            foreach (var file in enumerable)
            {

                if (Options.Fix)
                    File.Delete(file);
                Console.WriteLine("Deleting " + file);
            }
            int count = enumerable.Count();
            string msg = ((Options.Fix) ? "Removed :" : "Found :") + count+" nuget.exe file(s)";
            if (count == 0)
                msg = "No nuget.exe files found";
            Console.ForegroundColor = (count > 0) ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();

        }

        public void FixingCsprojFiles(string here)
        {
            int skipped = 0;
            int fixedup = 0;
            int nowrite = 0;
            string fixOrCheck = (Options.Fix) ? "Fixed" : "Found";
            Console.WriteLine("{0} csproj files", Options.Fix ? "Fixing" : "Checking");
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
                        if (Options.Fix)
                            File.WriteAllLines(file, output2);
                        fixedup++;
                    }
                    else
                    {
                        skipped++;
                    }
                    if (changed || Options.Verbose)
                        Console.WriteLine("{0} : {1}", (changed) ? fixOrCheck : "Skipped", file);
                }
                catch
                {
                    Console.WriteLine("Unable to write to :" + file);
                    nowrite++;
                }
            }
            Console.ForegroundColor = (fixedup > 0) ? ConsoleColor.Red : ConsoleColor.Green;
            string msg = fixOrCheck + " : " + fixedup+" csproj file(s)";
            if (fixedup == 0)
                msg = "No csproj files with target info found";
            Console.WriteLine(msg);
            Console.ResetColor();
            Console.WriteLine("Skipped : " + skipped);
            if (nowrite > 0)
                Console.WriteLine("Unable to write :" + nowrite);
            int total = fixedup + skipped;
            Console.WriteLine("Total files checked : " + total);
            Console.WriteLine("Finished {0} csproj files", Options.Fix ? "fixing" : "checking");
        }

        public IEnumerable<string> FixImportAndRestorePackagesInCsproj(IEnumerable<string> lines, string file)
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
                    var msg = string.Format("{2} {0} in {1}",
                        line.Contains("Import") ? "Import Project line" : "RestorePackages line", file, Options.Fix ? "Removed " : "To be removed ");
                    Console.WriteLine(msg);
                    changed = true;
                }
            }
            return output;
        }

        public IEnumerable<string> FixTargetInCsproj(IEnumerable<string> output)
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
        public void RemoveAllNugetTargetFiles(string here)
        {
            Console.WriteLine("Checking for nuget.target files");
            string[] filePaths = Directory.GetFiles(here, "nuget.targets",
                SearchOption.AllDirectories);

            foreach (var file in filePaths)
            {
                if (file.Contains(".nuget")) // Means we have found the file in a known location
                {
                    var cf = CheckAndCopyNugetPaths(file);
                    if (cf != null)
                    {
                        if (Options.Fix)
                            File.WriteAllLines(cf.Name, cf.Lines);
                        Console.WriteLine("{2} info from target file {0} to config file {1})", file, cf.Name, Options.Fix ? "Copied" : "Found to copy");
                    }
                }
                if (Options.Fix)
                    File.Delete(file);
                Console.WriteLine("{1} file: {0}", file, Options.Fix ? "Deleted" : "Found to delete");
            }
            Console.ForegroundColor = filePaths.Any() ? ConsoleColor.Red : ConsoleColor.Green;
            string msg = "No nuget.target files found";
            if (filePaths.Any())
                msg = string.Format("{1} : {0} nuget.targets file(s)", filePaths.Count(),Options.Fix?"Fixed":"Found");
            Console.WriteLine(msg);
            Console.ResetColor();
        }



        public ConfigFile CheckAndCopyNugetPaths(string file)
        {
            var lines = File.ReadAllLines(file);
            var nugetpaths = new List<string>();
            bool comment = false;
            foreach (var line in lines)
            {
                if (line.Contains("PackageSource"))
                {
                    if (!comment)
                    {
                        var url = ExtractUrl(line);
                        if (url.Length > 0)
                            nugetpaths.Add(url);
                    }
                }
                comment = IsCommentLine(line);  // Extract comment if previous line is a starting comment line with no ending comment
            }
            // Does there exist a nuget.config file ?
            var pathconfig = Path.GetDirectoryName(file);
            var configname = pathconfig + "/nuget.config";
            if (File.Exists(configname) && nugetpaths.Any())
            {
                var configlines = new List<string>(File.ReadAllLines(configname));
                var packageSourceExistAtLine = ContainsPackageSource(configlines);
                bool needToAddPackagSourcesTag = packageSourceExistAtLine == -1;
                bool haveItAdded = false;
                foreach (var nugetpath in nugetpaths)
                {
                    if (!NugetPathAlreadyThere(configlines, nugetpath))
                    {
                        var lineno = packageSourceExistAtLine;
                        if (packageSourceExistAtLine == -1) // Not there, insert at line 2
                        {
                            if (!haveItAdded)
                            {
                                configlines.Insert(2, "   <packageSources>");
                                haveItAdded = true;
                            }
                            lineno = 2;
                        }
                        var key = ExtractKey(nugetpath);
                        string line = string.Format("        <add key=\"{0}\" value=\"{1}\"/>", key, nugetpath);
                        configlines.Insert(lineno + 1, line);
                    }
                }
                if (needToAddPackagSourcesTag)
                {
                    int lineno = 2 + nugetpaths.Count() + 1;
                    configlines.Insert(lineno, "    </packageSources>");
                }
                return new ConfigFile(configlines, configname);
            }
            return null;
        }

        private string ExtractKey(string nugetpath)
        {
            var array = nugetpath.Split('.');
            if (array.Count() < 2)
                return "Don't know";
            return array[1];
        }

        private bool NugetPathAlreadyThere(IEnumerable<string> configlines, string nugetpath)
        {
            return ALineContains(configlines, nugetpath);
        }



        private int ContainsPackageSource(IEnumerable<string> configlines)
        {
            int i = 0;
            foreach (var configline in configlines)
            {
                if (configline.Contains("<packageSources"))
                    return i;
                i++;
            }
            return -1;
        }

        public string ExtractUrl(string line)
        {
            if (line.Contains("PackageSource"))
            {
                int index = line.IndexOf("http", StringComparison.Ordinal);
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

        private void FixSolutionFiles(string here)
        {

            Console.WriteLine("{0} solution files", Options.Fix ? "Fixing" : "Checking");
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
                        Console.WriteLine("Found nuget.target in " + file);
                    }
                }
                if (found)
                {
                    if (Options.Fix)
                        File.WriteAllLines(file, outlines);
                    count++;
                }
                if ((found && Options.Fix)|| Options.Verbose)
                {
                    string msg = string.Format("{0} checked. {1}", file,
                        found ? "Nuget.target" + (Options.Fix ? " removed" : " found") : "Skipped, nothing found");
                    Console.WriteLine(msg);
                }
            }
            Console.ForegroundColor = count > 0 ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine("{0} {1} solution file(s), out of {2}", Options.Fix ? "Fixed : " : "Found : ",count, slnFilePaths.Length);
            Console.ResetColor();
        }

        static public bool ALineContains(IEnumerable<string> lines, string pattern1, string pattern2 = "")
        {
            return lines.Any(line => line.Contains(pattern1) && line.Contains(pattern2));
        }
    }

    public class ConfigFile
    {
        public ConfigFile(IEnumerable<string> lines, string name)
        {
            Lines = lines;
            Name = name;
        }
        public IEnumerable<string> Lines { get; private set; }
        public string Name { get; private set; }
    }
}