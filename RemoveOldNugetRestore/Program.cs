using System;

namespace RemoveOldNugetRestore
{
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options) && (options.Execute || options.Check))
            {
                var oldNugetRestore = new RemoveOldNugetRestore(options);
                oldNugetRestore.Execute();
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
        }


    }
}
