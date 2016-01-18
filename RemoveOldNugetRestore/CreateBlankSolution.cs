using System;
using System.IO;

namespace IFix
{
    public class CreateBlankSolution
    {
        public int Execute(CreateSln createSln)
        {
            if (createSln.Fix)
            {
                var content = new[] {"Microsoft Visual Studio Solution File, Format Version 12.00", "# Visual Studio 14", "VisualStudioVersion = 14.0.24720.0", "MinimumVisualStudioVersion = 10.0.40219.1"};
                File.WriteAllLines("blank.sln",content);
                Console.WriteLine("Blank.sln file created");
            }
            return 0;
        }
    }
}