using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IFix
{
    public class DeleteMefCache
    {
        private readonly List<Cache> caches=new List<Cache>();

        public MefCacheCommand MefCacheCommand { get; private set; }

        public int Execute(MefCacheCommand command)
        {
            MefCacheCommand = command;
            CreateCaches(command);
            if (command.Check || command.Fix)
                VerifyExistenceOfCache();
            if (command.Fix)
                DeleteCaches();
            return 0;
        }

        private void DeleteCaches()
        {
            foreach (var cache in caches.Where(d=>d.Exist))
            {
                int no = 1;
                foreach (var location in cache.Locations)
                {
                    Directory.Delete(location, true);
                    Console.WriteLine($"Cache {no} for {cache.Name} at {location} deleted");
                    no++;
                }
            }
        }

        private void VerifyExistenceOfCache()
        {
            foreach (var cache in caches)
            {
                int no = 1;
                foreach (var location in cache.Locations)
                {
                    var exist = Directory.Exists(location);
                    cache.Exist |= exist;
                    Console.Write(
                        $"Cache {no} for {cache.Name} at {location} {(exist ? " do exist." : " do not exist.")}");
                    if (exist)
                    {
                        bool hascontent = (Directory.EnumerateDirectories(location).Count() +
                                           Directory.EnumerateFiles(location).Count()) > 0;
                        Console.WriteLine(!hascontent ? @" Cache is empty" : @" Cache is not empty");
                    }
                    else Console.WriteLine("");
                    no++;
                }
            }
        }

        private  void CreateCaches(MefCacheCommand command)
        {
            if (command.All || command.NotSpecific )
            {
                caches.AddRange(new List<Cache> { new Vs2012Cache(), new Vs2013Cache(), new Vs2015Cache(), new Vs2017Cache() });
            }
            else
            {
                if (command.Vs2012)
                {
                    caches.Add(new Vs2012Cache());
                }
                if (command.Vs2013)
                {
                    caches.Add(new Vs2013Cache());
                }
                if (command.Vs2015)
                {
                    caches.Add(new Vs2015Cache());
                }
                if (command.Vs2017)
                {
                    caches.Add(new Vs2017Cache());
                }
            }
        }
    }

    public abstract class Cache
    {
        public List<string> Locations { get; } = new List<string>();

        public bool Exist { get; set; }

        public abstract string Name { get; }

        protected Cache(string version)
        {
            const string componentmodelcache = "ComponentModelCache";
            var baselocation = Path.Combine(Environment.GetEnvironmentVariable("localappdata"), "Microsoft", "VisualStudio");
            var dirs = Directory.EnumerateDirectories(baselocation, $"{version}*");
            foreach (var dir in dirs)
            {
                var checkPath = Path.Combine(dir, componentmodelcache);
                if (Directory.Exists(checkPath))
                    Locations.Add(checkPath);
            }
            
        }
    }


    public class Vs2012Cache : Cache
    {
        public Vs2012Cache() : base("11.0")
        {
        }
        
        public override string Name => "VS2012";
    }
    public class Vs2013Cache : Cache
    {
        public Vs2013Cache() : base("12.0")
        {
        }

        public override string Name => "VS2013";
    }

    public class Vs2015Cache : Cache
    {
        public Vs2015Cache() : base("14.0")
        {
        }

        public override string Name => "VS2015";
    }

    public class Vs2017Cache : Cache
    {
        public Vs2017Cache() : base("15.0")
        {
        }

        public override string Name => "VS2017";
    }
}
