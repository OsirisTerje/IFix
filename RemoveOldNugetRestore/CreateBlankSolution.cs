using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace IFix
{
    public class CreateBlankSolution
    {
        public int Execute(CreateSln createSln)
        {
            var name = "blank.sln";
            if (!string.IsNullOrEmpty(createSln.Name))
                name = createSln.Name;
            if (!name.ToLower(CultureInfo.InvariantCulture).EndsWith(".sln"))
                name += ".sln";
            if (createSln.Fix)
            {
                var content = new List<string> { "Microsoft Visual Studio Solution File, Format Version 12.00", "# Visual Studio 14", "VisualStudioVersion = 14.0.24720.0", "MinimumVisualStudioVersion = 10.0.40219.1" };
                if (!createSln.Blank)
                {
                    var items = new SolutionItems();
                    AddGitIgnore(items);
                    AddReadme(items, name);
                    content.AddRange(items.Content);
                }
                File.WriteAllLines(name, content);
                Console.WriteLine($"{name} file created");
            }
            return 0;
        }

        private void AddGitIgnore(SolutionItems items)
        {
            const string gitignore = ".gitignore";
            if (!File.Exists(gitignore))
            {
                var gi = new GitIgnore(new GitIgnoreCommand { Fix = true });
                gi.WriteGitIgnore();
            }
            items.Add(gitignore);
        }

        private void AddReadme(SolutionItems items, string name)
        {
            const string readme = "readme.md";
            if (!File.Exists(readme))
            {
                var content = $"### Readme file for {name}" + Environment.NewLine + Environment.NewLine;
                content += "Add information for the developer about things that are helpful to know";
                File.WriteAllText(readme, content);
            }
            items.Add(readme);
        }
    }

    class SolutionItems
    {
        private readonly List<string> files = new List<string>();
        private IEnumerable<string> Files => files;

        public void Add(string filename)
        {
            files.Add($"{filename} = {filename}");
        }

        public IEnumerable<string> Content
        {
            get
            {
                var list = new List<string>();
                var s1 =
                    "Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"";
                s1 += Guid.NewGuid().ToString("B").ToUpper();
                s1 += "\"";
                list.Add(s1);
                list.Add("ProjectSection(SolutionItems) = preProject");
                list.AddRange(Files);
                list.Add("EndProjectSection");
                list.Add("EndProject");
                list.Add("Global");
                list.Add("GlobalSection(SolutionProperties) = preSolution");
                list.Add("HideSolutionNode = FALSE");
                list.Add("EndGlobalSection");
                list.Add("EndGlobal");
                return list;
            }
        }


    }
}