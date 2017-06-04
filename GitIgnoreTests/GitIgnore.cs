using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using IntegrationTest;

namespace GitIgnoreTests
{
    [TestFixture, Category(nameof(GitIgnore))]
    public class GitIgnore
    {

        private const string W = "gitignoreWPackage.tst";
        private const string WO = "gitignoreWOPackage.tst";
        private const string STD = "VisualStudio.gitignore";
        private const string GitHubVS = "github.visualstudio.gitignore";

        private string gitDirectory;
        public string GitDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(gitDirectory))
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    var uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    gitDirectory = Path.Combine(Path.GetDirectoryName(path), @"..\..\..\");
                }
                return gitDirectory;
            }
        }

        /// <summary>
        /// Checks that the gitignore get the extra 194 line added.
        /// </summary>
        [Test,LocalOnly]
        public void VerifyGitHubIgnore()
        {
            var testdata = ResourceReader.Read(GitHubVS);
            Assert.That(testdata.Any(l => l.Trim() == @"packages/*"), Is.False, "githubignore contained the packages/*");
            var sut = new IFix.GitIgnore(GitDirectory);

            var result = sut.CheckIfNuGetPackages(testdata,false,false);
            Assert.IsTrue(result);
            var outlines = sut.AddOnlyMissingInfo(testdata);
            Assert.That(outlines.Any(l => l.Trim() == @"packages/*"), "packages/* was not added");

        }


        [Test]
        public void VerifyWithPackages()
        {
            var testdata = ResourceReader.Read(W);

            var sut = new IFix.GitIgnore(GitDirectory);

            var result = sut.CheckIfNuGetPackages(testdata,false,false);
            Assert.IsTrue(result);

        }

        [Test]
        public void VerifyWithNoPackages()
        {
            var testdata = ResourceReader.Read(WO);

            var sut = new IFix.GitIgnore(GitDirectory);

            var result = sut.CheckIfNuGetPackages(testdata, false, false);
            Assert.IsFalse(result);

        }

        [Test]
        public void VerifyWithAddingInfo()
        {
            var testdata = ResourceReader.Read(WO);

            var sut = new IFix.GitIgnore(GitDirectory);

            var result = sut.CheckIfNuGetPackages(testdata, false, false);
            Assert.IsFalse(result, "Testdata contains packages or CheckIfPackages failed");
            var outlines = sut.AddOnlyMissingInfo(testdata);
            result = sut.CheckIfNuGetPackages(outlines, false, false);
            Assert.IsTrue(result, "Add missing info failed");

        }

        [Test, Integration]
        public void CheckDownload()
        {
            var sut = new IFix.GitIgnore();

            var temp = Path.GetTempPath();
            var tempgitignore = temp + "/VisualStudio.gitignore";
            sut.DownloadGitIgnore(tempgitignore);
            var lines = File.ReadAllLines(tempgitignore);
            Assert.IsTrue(lines.Any());
            Assert.IsTrue(lines[0].StartsWith("##"));
        }

        [Test]
        public void VerifyMissing()
        {
            var gitignorelines = ResourceReader.Read(W);
            var stdlines = ResourceReader.Read(STD);

            var sut = new IFix.GitIgnore(GitDirectory);

            var result = sut.CheckIfOurContainsStd(gitignorelines, stdlines);

            Assert.IsTrue(result.Any());


        }


        [Test]
        public void VerifyAddMissing()
        {
            var lines = new List<string> { "Somestart" };

            var sut = new IFix.GitIgnore(GitDirectory);

            var outlines = sut.AddOnlyMissingInfo(lines);

            Assert.That(outlines.Count() == 7, "Count was " + outlines.Count());
        }

        [Test]
        public void VerifyAddMissing2()
        {
            var lines = new List<string> { "Somestart", @"**/packages/*" };

            var sut = new IFix.GitIgnore(GitDirectory);

            var outlines = sut.AddOnlyMissingInfo(lines);

            Assert.That(outlines.Count() == 7, "Count was " + outlines.Count());
        }


        [Test]
        public void FindMyOwnGitRepo()
        {
            var sut = new IFix.GitIgnore(GitDirectory);
            Assert.That(sut.Repositories.Count() == 1, "Can't find my own repo");
            var file = sut.Repositories.First().File;
            Assert.That(File.Exists(file), "No gitignore file");
        }

        [Test]
        public void VerifyThatGitHubIgnoreFileWasDownloaded()
        {
            var sut = new IFix.GitIgnore(GitDirectory); // Make sure we create it and thus also download the file. 
            var temp = Path.GetTempPath();
            var tempgitignore = temp + "/VisualStudio.gitignore";
            Assert.That(File.Exists(tempgitignore),"No temporary gitignore found at "+tempgitignore);
        }


    }

    public class Integration : CategoryAttribute
    { }
}
