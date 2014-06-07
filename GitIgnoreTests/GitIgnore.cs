using System.IO;
using System.Linq;
using NUnit.Framework;

namespace GitIgnoreTests
{
    [TestFixture,Category("GitIgnore")]
    public class GitIgnore
    {

        private const string W = "gitignoreWPackage.tst";
        private const string WO = "gitignoreWOPackage.tst";
        private const string STD = "VisualStudio.gitignore";

        [Test]
        public void VerifyWithPackages()
        {
            var testdata = ResourceReader.Read(W);

            var sut = new IFix.GitIgnore();

            var result = sut.CheckIfPackages(testdata);
            Assert.IsTrue(result);

        }

        [Test]
        public void VerifyWithNoPackages()
        {
            var testdata = ResourceReader.Read(WO);

            var sut = new IFix.GitIgnore();

            var result = sut.CheckIfPackages(testdata);
            Assert.IsFalse(result);

        }

        [Test]
        public void VerifyWithAddingInfo()
        {
            var testdata = ResourceReader.Read(WO);

            var sut = new IFix.GitIgnore();

            var result = sut.CheckIfPackages(testdata);
            Assert.IsFalse(result,"Testdata contains packages or CheckIfPackages failed");
            sut.AddMissingInfo(testdata);
            result = sut.CheckIfPackages(testdata);
            Assert.IsTrue(result,"Add missing info failed");

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

            var sut = new IFix.GitIgnore();

            var result = sut.CheckIfOurContainsStd(gitignorelines, stdlines);

            Assert.IsTrue(result.Any());


        }
    }

    public class Integration:CategoryAttribute
    {}
}
