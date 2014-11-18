using System.IO;
using System.Linq;
using IFix;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture, PathDependent]
    public class VerifyProcess
    {


        NuGetRestoreCommand Command { get; set; }

        private const string subpath = "/../../../TestProject/TestProject";


        [SetUp]
        public void Init()
        {
            Command = new NuGetRestoreCommand {Check = true};
        }


        [Test, Integration]
        public void CheckDeletingNugetExe()
        {
            var v = new RemoveOldNugetRestore(Command);
            string path = Directory.GetCurrentDirectory() + subpath ;
            string fileisat = path + "/.nuget/nuget.exe";
            bool exist = File.Exists(fileisat);
            Assert.IsTrue(exist);
            v.RemoveAllNugetExeFiles(path);
            exist = File.Exists(fileisat);
            Assert.IsFalse(exist);
        }

        [Test]
        public void VerifyCorrectDirectory()
        {
            string path = Directory.GetCurrentDirectory() + subpath;
            var exist = File.Exists(path + "/TestProject.csproj");
            Assert.IsTrue(exist);
        }


        [Test]
        public void VerifyReadingCsproj()
        {
            string path = Directory.GetCurrentDirectory() + subpath;
            var lines = File.ReadAllLines(path + "/TestProject.csproj");
            var sut = new RemoveOldNugetRestore(Command);
            var outlines = sut.FixImportAndRestorePackagesInProj(lines, path + "/TestProject.csproj");
            
            Assert.IsTrue(outlines.Count()<lines.Count());
            Assert.IsTrue(RemoveOldNugetRestore.ALineContains(lines, "RestorePackages"), "1");
            Assert.IsFalse(RemoveOldNugetRestore.ALineContains(outlines, "RestorePackages"), "2");
            Assert.IsTrue(RemoveOldNugetRestore.ALineContains(lines, "Import Project", "NuGet.targets"), "3");
            Assert.IsFalse(RemoveOldNugetRestore.ALineContains(outlines, "Import Project", "NuGet.targets"), "4");
        }


        [Test]
        public void VerifyReadingTargetsInCsproj()
        {
            string path = Directory.GetCurrentDirectory() + subpath;
            var lines = File.ReadAllLines(path + "/TestProject.csproj");

            var sut = new RemoveOldNugetRestore(Command);
            var outlines = sut.FixTargetInProj(lines);
            Assert.IsTrue(lines.Count()==outlines.Count()+6);

        }

        [Test]
        public void VerifyCopyingTargetPathsToConfig()
        {

            string path = Directory.GetCurrentDirectory() + subpath;
            string file = path + "/.nuget/nuget.targets";
            Assert.IsTrue(File.Exists(file),"Target file doesnt exist");
            var sut = new RemoveOldNugetRestore(Command);
            var configlinesBefore = File.ReadAllLines(path + "/.nuget/nuget.config");
            var outlines = sut.CheckAndCopyNugetPaths(file);
            Assert.IsNotNull(outlines,"Didnt find anything");
            Assert.IsTrue(outlines.Lines.Count()==configlinesBefore.Count()+4,"Number of new lines incorrect");

        }

        [Test]
        public void VerifyNotCopyingTargetPathsToConfigWhenOnlyComments()
        {

            string path = Directory.GetCurrentDirectory() + subpath;
            string file = path + "/.nuget/nuget2.targets";
            Assert.IsTrue(File.Exists(file), "Target file doesnt exist");
            Command.Fix = true;
            var sut = new RemoveOldNugetRestore(Command);
            var configlinesBefore = File.ReadAllLines(path + "/.nuget/nuget.config");
            var outlines = sut.CheckAndCopyNugetPaths(file);
            Assert.IsNull(outlines);
        }



        

    }


    public class IntegrationAttribute : CategoryAttribute
    { }

    public class PathDependentAttribute : CategoryAttribute
    { }
}
