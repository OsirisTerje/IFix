using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IFix
{
    public class DeleteMefCache
    {
        private readonly List<Cache> caches=new List<Cache>();


        public int Execute(MefCacheCommand command)
        {
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
                Directory.Delete(cache.Location,true);
                Console.WriteLine($"Cache for {cache.Name} at {cache.Location} deleted");
            }
        }

        private void VerifyExistenceOfCache()
        {
            foreach (var cache in caches)
            {
                var exist = Directory.Exists(cache.Location);
                cache.Exist = exist;
                Console.WriteLine($"Cache for {cache.Name} at {cache.Location} {(exist ? " do exist" : " do not exist")}");
                if (exist)
                {
                    bool hascontent = (Directory.EnumerateDirectories(cache.Location).Count() + Directory.EnumerateFiles(cache.Location).Count()) > 0;
                    Console.WriteLine(!hascontent ? @"Cache is empty" : @"Cache is not empty");
                }
            }
        }

        private  void CreateCaches(MefCacheCommand command)
        {
            if (command.All || (command.NotSpecific && command.Check))
            {
                caches.AddRange(new List<Cache> { new Vs2012Cache(), new Vs2013Cache(), new Vs2015Cache() });
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
            }
        }
    }

    public abstract class Cache
    {
        public string Location { get; private set; }

        public bool Exist { get; set; }

        public abstract string Name { get; }

        protected Cache(string version)
        {
            Location = Path.Combine(Environment.GetEnvironmentVariable("localappdata"), "Microsoft", "VisualStudio",
                version, "ComponentModelCache");
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
}
