using System.Linq;
using CommandLine;
using IFix;
using NUnit.Framework;

namespace IFixTests
{
    public class OptionsTests
    {
        [Test]
        public void ThatDiagnosticOptionsWorksDumpDisable()
        {
            var args = new[] { "diagnostics", "-d", "0", "-f" };
            var result = Parsing(args);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.EnableDisableDump, Is.EqualTo(0));
            Assert.That(result.Fix);
        }

        private DiagnosticsCommands Parsing(string[] args)
        {
            var sut = new Parser();
            var parsed = sut.ParseArguments<DiagnosticsCommands>(args);
            var result = ((Parsed<DiagnosticsCommands>)parsed).Value;
            return result;
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ThatDiagnosticOptionsWorksDumpEnable()
        {
            var args = new[] { "diagnostics", "-d", "1", "-f" };
            var di = Parsing(args);
            Assert.That(di, Is.Not.Null);
            Assert.That(di.EnableDisableDump, Is.EqualTo(1));
            Assert.That(di.Fix);
        }

        [TestCase("diagnostics", "-f", "-u", "1")]
        [TestCase("diagnostics", "-u", "1", "-f")]
        [TestCase("diagnostics", "-u", "1", "-f")]
        public void ThatFuslogOptionsWorks(string a, string b, string c, string d)
        {
            var args = new[] { a, b, c, d };
            var di = Parsing(args);
            Assert.That(di, Is.Not.Null);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.EnableDisableFuslog, Is.EqualTo(1));
            Assert.That(di.Fix);
        }

        [TestCase("diagnostics", "-f", "-u", "1", 1)]
        [TestCase("diagnostics", "-f", "-u", "0", 0)]
        [TestCase("diagnostics", "-u", "0", "-f", 0)]
        public void ThatFuslogOptionsWorks4Disable(string a, string b, string c, string d, int expected)
        {
            var args = new[] { a, b, c, d };
            var di = Parsing(args);
            Assert.That(di, Is.Not.Null);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.EnableDisableFuslog, Is.EqualTo(expected));
            Assert.That(di.Fix);
        }

        [TestCase("diagnostics", "--fix", "--Fuslog", "1", 1)]
        [TestCase("diagnostics", "--fix", "--Fuslog", "0", 0)]
        [TestCase("diagnostics", "-u", "0", "-f", 0)]
        public void ThatFuslogOptionsWorks4DisableWithFullOptions(string a, string b, string c, string d, int expected)
        {
            var args = new[] { a, b, c, d };
            var di = Parsing(args);
            Assert.That(di, Is.Not.Null);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.EnableDisableFuslog, Is.EqualTo(expected));
            Assert.That(di.Fix);
        }


        [Test]
        public void ThatDiagnosticOptionsForDumpFolderWorks()
        {
            var args = new[] { "diagnostics", "-D", @"C:\CrashDumps", "-f" };
            var di = Parsing(args);
            Assert.That(di, Is.Not.Null);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.Fix);
            Assert.That(di.DumpFolder, Is.EqualTo(@"C:\CrashDumps"));
        }
    }
}
