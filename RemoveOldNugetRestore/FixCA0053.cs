using System;
using System.IO;

namespace IFix
{
    public class FixCA0053
    {
        private bool changed;
        public int Execute(CommonOptions options)
        {
            int skipped = 0;
            int fixedup = 0;
            int nowrite = 0;

            string here = Directory.GetCurrentDirectory();
            string[] filePaths = Directory.GetFiles(here, "*.csproj",
                                         SearchOption.AllDirectories);
            var carsd = new SearchTerms("<CodeAnalysisRuleSetDirectories>", "</CodeAnalysisRuleSetDirectories>", @"$(DevEnvDir)\..\..\Team Tools\Static Analysis Tools\Rule Sets");
            var card = new SearchTerms("<CodeAnalysisRuleDirectories>", "</CodeAnalysisRuleDirectories>", @"$(DevEnvDir)\..\..\Team Tools\Static Analysis Tools\FxCop\Rules");
            bool fix = (options.Fix);
            foreach (var file in filePaths)
            {
                changed = false;
                string text = File.ReadAllText(file);

                text = Change2(text, carsd);
                text = Change2(text, card);

                try
                {
                    
                    if (changed)
                    {
                        if (fix)
                            File.WriteAllText(file, text);
                        Console.ForegroundColor=ConsoleColor.Red;
                        Console.WriteLine((fix)?"Fixed   :":"Found in   :" + file);
                        Console.ResetColor();
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
            Console.WriteLine((fix) ? "Fixed   :" : "Found   :" + fixedup);
            Console.WriteLine("Skipped : " + skipped);
            if (nowrite > 0)
                Console.WriteLine("Unable to write :" + nowrite);
            int total = fixedup + skipped;
            Console.WriteLine("Total files checked : " + total);
            return fixedup;
        }

        private string Change2(string text, SearchTerms terms)
        {
            const int notFound = -1;
            int index = 0;
            do
            {
                index = text.IndexOf(terms.Start, index, StringComparison.CurrentCultureIgnoreCase);
                if (index != notFound)
                {
                    int indexend = text.IndexOf(terms.Stop, index, StringComparison.CurrentCultureIgnoreCase);
                    string tobechecked = text.Substring(index, indexend - index);
                    if (tobechecked.IndexOf(@"Microsoft Visual Studio 10.0", StringComparison.CurrentCultureIgnoreCase) != notFound)
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
            } while (index != notFound);
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
