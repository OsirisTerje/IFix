using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IFix
{
    public class NugetConsolidate
    {
        private List<NugetPackage> NugetPackages { get; } = new List<NugetPackage>();

        public int Execute(NugetConsolidateCommands commands)
        {
            var path = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(path, "packages.config", SearchOption.AllDirectories);
            var res = LocalExecute(commands, files);
            if (commands.Check)
            {
                foreach (var pack in NugetPackages)
                {
                    Console.WriteLine(pack.Name);
                    foreach (var version in pack.Versions)
                    {
                        Console.WriteLine($"   {version.Version}");
                        foreach (var project in version.Projects)
                        {
                            Console.WriteLine($"      {project.Name}");
                        }
                    }
                }
            }
            return res;
        }
        private string FindCsProjFileName(string packageconfigFile)
        {
            var dirpath = Path.GetDirectoryName(packageconfigFile);
            var csprojs = Directory.GetFiles(dirpath, "*.csproj", SearchOption.TopDirectoryOnly).ToList();
            if (csprojs.Count == 1)
            {
                var s = csprojs.Single();
                s = s.Substring(0, s.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase));
                return s;
            }
            return "";
        }
        public int LocalExecute(NugetConsolidateCommands command, IEnumerable<string> files)
        {
            foreach (var packageconfigFile in files)
            {
                var content = File.ReadAllLines(packageconfigFile);
                var csprojFileName = FindCsProjFileName(packageconfigFile);
                foreach (var line in content.Where(l => l.Contains("<package ")))
                {
                    var parts = line.Trim().Split(' ').ToList();
                    if (parts.Count < 3)
                        continue;
                    var id = parts[1].Split('=')[1].Trim('"');
                    var version = parts[2].Split('=')[1].Trim('"');
                    Update(csprojFileName,id,version);
                }
            }
            return 0;
        }

        void Update(string project, string package, string version)
        {
            var pack = NugetPackages.SingleOrDefault(o => o.Name == package);
            if (pack == null)
            {
                NugetPackages.Add(new NugetPackage(package, version, project));
            }
            else
            {
                pack.Update(version,project);
            }

        }
    }




    

}

public class NugetVersion
{
    public List<Project> Projects { get; } = new List<Project>();

    public string Version { get; set; }

    public NugetVersion(string version, string project)
    {
        Version = version;
        Projects.Add(new Project(project));
    }


    public void Update(string project)
    {
        var proj = Projects.SingleOrDefault(o => o.Name == project);
        if (proj == null)
        {
            Projects.Add(new Project(project));
        }
    }


}

public class NugetPackage
{
    public List<NugetVersion> Versions { get; } = new List<NugetVersion>();
    public string Name { get; }

    public NugetPackage(string name, string version, string project)
    {
        Name = name;
        Versions.Add(new NugetVersion(version, project));
    }

    public void Update(string version, string project)
    {
        var vers = Versions.SingleOrDefault(o => o.Version == version);
        if (vers == null)
        {
            Versions.Add(new NugetVersion(version, project));
        }
        else
        {
            vers.Update(project);
        }
    }

}

public class Project
{
    public string Name { get; }

    public Project(string name)
    {
        Name = name;
    }

}




