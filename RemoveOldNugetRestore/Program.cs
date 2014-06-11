using System;

namespace IFix
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = new Options();
            string invokedverb="";
            CommonOptions invokedverbinstance=null;
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(options.GetUsage());
                return -1;
            }
            int retval = 0;     
            if (CommandLine.Parser.Default.ParseArguments(args, options,(verb,subOptions)=>
            {
                invokedverb = verb;
                invokedverbinstance = (CommonOptions)subOptions;
            }) )
            {
                if (invokedverbinstance != null)
                {
                    if (invokedverbinstance.ValidOptions())
                    {
                        retval = invokedverbinstance.Execute();
                    }
                    else
                    {
                        var msg = invokedverbinstance.Help();
                        Console.WriteLine(msg);
                    }
                }
                else
                {
                    retval = -1;
                    Console.WriteLine("Unknown command : " + invokedverb);
                }
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
            return retval;
        }


    }
}
