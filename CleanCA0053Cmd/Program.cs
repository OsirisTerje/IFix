using System;

namespace RemoveOldNugetRestore
{
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
}
