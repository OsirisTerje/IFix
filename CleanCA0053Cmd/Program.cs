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


    class RemoveOldNugetRestore
    {
        private bool changed;
        public void Execute()
        {
            int skipped = 0;
            int fixedup = 0;
            int nowrite = 0;

            string here = Directory.GetCurrentDirectory();


            FixSolutionFiles(here);


            string[] filePaths = Directory.GetFiles(here, "*.csproj",
                                         SearchOption.AllDirectories);
            foreach (var file in filePaths)
            {

                var lines = File.ReadAllLines(file);
                var output = new List<string>();

                changed = false;
                foreach (var line in lines)
                {
                    if (!((line.Contains(@"<Import Project") && line.Contains("NuGet.targets")) || line.Contains("<RestorePackages>true</RestorePackages>")))
                    {
                        output.Add(line);
                    }
                    else
                    {
                        changed = true;
                    }

                }

                var output2 = new List<string>();
                bool foundTarget = false;
                foreach (var line in output)
                {
                    if (line.Contains("<Target Name=") && line.Contains("EnsureNuGetPackageBuildImports"))
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


                try
                {

                    if (changed)
                    {
                        File.WriteAllLines(file, output2);
                        Console.WriteLine("Fixed   :" + file);
                        fixedup++;
                    }
                    else
                    {
                        Console.WriteLine("Skipped :" + file);
                        skipped++;
                    }
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
        }

        private static void FixSolutionFiles(string here)
        {
            string[] slnFilePaths = Directory.GetFiles(here, "*.sln");
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
#if !DEBUG
                File.WriteAllLines(file);
#endif
                string msg = string.Format("{0} checked. {1}", file, found ? "Nuget.target removed" : "Nothing found");
                Console.WriteLine(msg);
            }
        }

        private string Change2(string text, SearchTerms terms)
        {
            const int NotFound = -1;
            int index = 0;
            do
            {
                index = text.IndexOf(terms.Start, index, StringComparison.CurrentCultureIgnoreCase);
                if (index != NotFound)
                {
                    int indexend = text.IndexOf(terms.Stop, index, StringComparison.CurrentCultureIgnoreCase);
                    string tobechecked = text.Substring(index, indexend - index);
                    if (tobechecked.IndexOf(@"Microsoft Visual Studio 10.0", StringComparison.CurrentCultureIgnoreCase) != NotFound)
                    {
                        int start = index + terms.Start.Length;
                        int length = indexend - start;
                        text = text.Remove(start, length);
                        text = text.Insert(start, terms.Content);
                        index = indexend;
                        changed = true;
                    }
                    else
                        index = indexend;
                }
            } while (index != NotFound);
            return text;
        }
    }


    struct SearchTerms
    {
        public string Start { get; private set; }

        public string Stop { get; private set; }

        public string Content { get; private set; }

        public SearchTerms(string start, string stop, string content)
            : this()
        {
            Start = start;
            Stop = stop;
            Content = content;
        }
    }


}
