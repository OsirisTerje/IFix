using System;
using NuGet;

namespace IFix
{
    using System.Reflection;

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
                if (invokedverb == "nugetrestore")
                {
                    var nugetoptions = invokedverbinstance as NuGetRestoreOptions;
                    if (nugetoptions!=null && (nugetoptions.Fix || nugetoptions.Check))
                    {
                        var oldNugetRestore = new RemoveOldNugetRestore(nugetoptions);
                        oldNugetRestore.Execute();
                    }
                    else
                    {
                        var msg = nugetoptions.Help();
                        Console.WriteLine(msg);
                    }
                }
                else if (invokedverb == "ca0053")
                {
                    var coptions = invokedverbinstance as FixCA0053Options;
                    if (coptions!=null && (coptions.Fix || coptions.Check))
                    {
                        var fixer = new FixCA0053();
                        fixer.Execute(coptions);
                    }
                    else
                    {
                        var msg = options.GetUsage();
                        Console.WriteLine(msg);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command");
                }
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
        }


    }
}
