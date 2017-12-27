using System;
using System.IO;
using System.Linq;

namespace IFix
{
    public class DeleteVSTestCache
    {
        public int Execute(VSTestCacheCommands vsTestCacheCommands)
        {

            var location =
                Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp",
                    "VisualStudioTestExplorerExtensions");
            var dirs = Directory.GetDirectories(location);
            if (vsTestCacheCommands.Check)
            {
                if (!dirs.Any())
                {
                    Console.WriteLine("No cache directories found. All is fine!");
                    return 0;
                }
                foreach (var dir in dirs)
                {
                    var name = new DirectoryInfo(dir).Name;
                    var msg = IsValid(dir) ? " Looks ok" : " Is corrupt";
                    Console.WriteLine($"Found vstest cache {name} at: {dir}, state: {msg}");
                }
            } else if (vsTestCacheCommands.Fix)
            {
                try
                {
                    foreach (var dir in dirs)
                    {
                        Directory.Delete(dir, true);
                        Console.WriteLine($"Deleted vstest cache at: {dir}");
                    }
                }
                catch (System.UnauthorizedAccessException)
                {
                    Console.WriteLine("Not able to delete all. Note that some caches now may be corrupt. Please close all instances of Visual Studio (at least vstest executables) and try again");
                    return 1;
                }
               
            }


            return 0;
        }

        public bool IsValid(string location)
        {
            var nupkgs = Directory.EnumerateFiles(location, "*.nupkg");
            if (nupkgs.Count() != 1)
                return false;
            var dlls = Directory.EnumerateFiles(location, "*.dll", SearchOption.AllDirectories);
            if (!dlls.Any())
                return false;
            return true;
        }


    }
}