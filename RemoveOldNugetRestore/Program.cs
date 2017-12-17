using System;

namespace IFix
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = new Options();
            string invokedverb="";
            IOptions invokedverbinstance=null;
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(options.GetUsage());
                return -1;
            }
            int retval = 0;     
            if (CommandLine.Parser.Default.ParseArguments(args, options,(verb,subOptions)=>
            {
                invokedverb = verb;
                invokedverbinstance = (IOptions)subOptions;
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
                        if (invokedverb == "info")
                        {
                            retval = invokedverbinstance.Execute();
                            return retval;
                        }
                        var msg = invokedverbinstance.Help();
                        Console.WriteLine(msg);
                        retval = -1;
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
