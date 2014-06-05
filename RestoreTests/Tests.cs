using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFix;
using NuGet;
using NUnit.Framework;

namespace RestoreTests
{
    [TestFixture]
    public class Tests
    {
        [TestCase("<PackageSource Include=\"https://www.nuget.org/api/v2/\" />",false)]
        [TestCase("        <!--",true)]
        [TestCase("        <!-- The official NuGet package source (https://www.nuget.org/api/v2/) will be excluded if package sources are specified and it does not appear in the list -->",false)]
        [Test]
        public void LineHasComment(string line,bool expected)
        {

            var sut = new RemoveOldNugetRestore();

            var result = sut.IsCommentLine(line,false);
            Assert.AreEqual(expected,result,"Fails "+line);
        }

        [TestCase("<RestoreCommand>$(NuGetCommand) install \"$(PackagesConfig)\" -source \"$(PackageSources)\"  $(NonInteractiveSwitch) $(RequireConsentSwitch) -solutionDir $(PaddedSolutionDir)</RestoreCommand>","")]
        [TestCase("            <Using Namespace=\"Microsoft.Build.Utilities\" />","")]
        [TestCase("<!-- The official NuGet package source (https://www.nuget.org/api/v2/) will be excluded if package sources are specified and it does not appear in the list -->","")]
        [TestCase("            <PackageSource Include=\"https://www.nuget.org/api/v2/\" />","https://www.nuget.org/api/v2/")]
        public void LineHasExtractableUrl(string line, string expected)
        {
            var sut = new RemoveOldNugetRestore();

            var result = sut.ExtractUrl(line);
            Assert.AreEqual(expected,result,"Failed: "+line);
           
        }
    }
}
