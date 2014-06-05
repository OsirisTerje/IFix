using System;

namespace IFix
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            string invokedverb="";
            CommonOptions invokedverbinstance=null;
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(options.GetUsage());
                return;
            }
                
            if (CommandLine.Parser.Default.ParseArguments(args, options,(verb,subOptions)=>
            {
                invokedverb = verb;
                invokedverbinstance = (CommonOptions)subOptions;
            }) )
            {
                if (invokedverbinstance != null)
                {
                    if (invokedverbinstance.Fix || invokedverbinstance.Check)
                    {
                        invokedverbinstance.Execute();
                    }
                    else
                    {
                        var msg = invokedverbinstance.Help();
                        Console.WriteLine(msg);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command : " + invokedverb);
                }
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
        }


    }
}
