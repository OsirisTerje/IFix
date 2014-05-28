using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CleanCA0053Cmd;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture]
    public class VerifyProcess
    {
        private const string subpath = "/../../../TestProject/TestProject";
        [Test]
        public void CheckDeletingNugetExe()
        {
            var v = new RemoveOldNugetRestore.RemoveOldNugetRestore();
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

    }
}
