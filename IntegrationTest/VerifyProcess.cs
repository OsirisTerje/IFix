﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using IFix;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture, PathDependent,LocalOnly]
    public class VerifyProcess
    {


        NuGetRestoreCommand Command { get; set; }

        private const string subpath = @"..\..\..\..\TestProject/TestProject";


        private string currentDirectory;
        public string CurrentDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(currentDirectory))
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    var uri = new UriBuilder(codeBase);
                    var path  = Uri.UnescapeDataString(uri.Path);
                    var path2 = Path.GetDirectoryName(path);
                    currentDirectory = Path.Combine(path2, subpath);
                }
                return currentDirectory;
            }
        }

        [SetUp]
        public void Init()
        {
            Command = new NuGetRestoreCommand {Check = true};
        }


        //[Test, Integration, LocalOnly, Category("Integration")]
        //public void CheckDeletingNugetExe()
        //{
        //    var v = new RemoveOldNugetRestore(Command);
        //    string path = CurrentDirectory;
        //    string fileisat = path + "/.nuget/nuget.exe";
        //    bool exist = File.Exists(fileisat);
        //    Assert.IsTrue(exist);
        //    v.RemoveAllNugetExeFiles(path);
        //    exist = File.Exists(fileisat);
        //    Assert.IsFalse(exist);
        //}

        [Test]
        public void VerifyCorrectDirectory()
        {
            var path = Path.Combine(CurrentDirectory, "TestProject.csproj");
           var exist = File.Exists(path);
            Assert.That(exist, $"Can locate {path}");
        }


        [Test]
        public void VerifyReadingCsproj()
        {
            string path = CurrentDirectory;
            var lines = File.ReadAllLines(path + "/TestProject.csproj");
            var sut = new RemoveOldNugetRestore(Command);
            var outlines = sut.FixImportAndRestorePackagesInProj(lines, path + "/TestProject.csproj").ToList();
            
            Assert.That(outlines.Count(),Is.LessThan(lines.Count()));
            Assert.That(RemoveOldNugetRestore.ALineContains(lines, "RestorePackages"), Is.True, "1");
            Assert.That(RemoveOldNugetRestore.ALineContains(outlines, "RestorePackages"), Is.False, "2");
            Assert.That(RemoveOldNugetRestore.ALineContains(lines, "Import Project", "NuGet.targets"), Is.True, "3");
            Assert.That(RemoveOldNugetRestore.ALineContains(outlines, "Import Project", "NuGet.targets"), Is.False, "4");
        }


        [Test]
        public void VerifyReadingTargetsInCsproj()
        {
            var lines = File.ReadAllLines(CurrentDirectory + "/TestProject.csproj");

            var sut = new RemoveOldNugetRestore(Command);
            var outlines = sut.FixTargetInProj(lines);
            Assert.That(lines.Length, Is.EqualTo(outlines.Count()+6));

        }

        [Test]
        public void VerifyCopyingTargetPathsToConfig()
        {

            string path = CurrentDirectory;
            string file = path + "/.nuget/nuget.targets";
            Assert.That(File.Exists(file), "Target file doesn't exist");
            var sut = new RemoveOldNugetRestore(Command);
            var configlinesBefore = File.ReadAllLines(path + "/.nuget/nuget.config");
            var outlines = sut.CheckAndCopyNugetPaths(file);
            Assert.That(outlines,Is.Not.Null,"Didnt find anything");
            Assert.That(outlines.Lines.Count(),Is.EqualTo(configlinesBefore.Count()+4), "Number of new lines incorrect");

        }

        [Test]
        public void VerifyNotCopyingTargetPathsToConfigWhenOnlyComments()
        {

            string path = CurrentDirectory;
            string file = path + "/.nuget/nuget2.targets";
            Assert.That(File.Exists(file), "Target file doesn't exist");
            Command.Fix = true;
            var sut = new RemoveOldNugetRestore(Command);
            var configlinesBefore = File.ReadAllLines(path + "/.nuget/nuget.config");
            var outlines = sut.CheckAndCopyNugetPaths(file);
            Assert.That(outlines,Is.Null);
        }



        

    }


    public class IntegrationAttribute : CategoryAttribute
    { }

    public class PathDependentAttribute : CategoryAttribute
    { }

    public class LocalOnlyAttribute : CategoryAttribute
    { }
}
