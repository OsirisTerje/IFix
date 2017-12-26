using System;
using Fclp;

namespace IFix
{
    class Program
    {
        static int Main(string[] args)
        {
            var fclp = new FluentCommandLineParser();
            var setup = new SetupCommands(fclp);
            if (args == null || args.Length == 0)
            {
                Console.WriteLine();
                return -1;
            }
            var result = fclp.Parse(args);
            if (result.HasErrors)
            {
                Console.WriteLine("Errors in parsing commands");
                return 1;
            }
            return 0;
        }


    }
}
