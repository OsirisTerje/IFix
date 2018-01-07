using System.Linq;
using System.Linq.Expressions;
using Fclp;
using Fclp.Internals.Errors;
using IFix;
using NUnit.Framework;

namespace IFixTests
{
    public class OptionsTests
    {
        private FluentCommandLineParser fclp;
        private SetupCommands setup;

        [Test]
        public void ThatDiagnosticOptionsWorksDumpDisable()
        {
            var args = new[] {"diagnostics","-d","0","-f"};
            
            var result = fclp.Parse(args);

            Assert.That(result.HasErrors,Is.Not.True);
            Assert.That(!result.AdditionalOptionsFound.Any());
            var di = setup.ParsedOptions as DiagnosticsCommands;
            Assert.IsNotNull(di);
            Assert.That(di.EnableDisableDump,Is.EqualTo(0));
            Assert.That(di.Fix);
        }

        [SetUp]
        public void Setup()
        {
            fclp = new FluentCommandLineParser {IsCaseSensitive = true};
            setup = new SetupCommands(fclp)
            {
                DoExecute = false
            };
        }

        [Test]
        public void ThatDiagnosticOptionsWorksDumpEnable()
        {
            var args = new[] { "diagnostics", "-d", "1", "-f" };
            var result = fclp.Parse(args);
            Assert.That(result.HasErrors,Is.Not.True);
            var di = setup.ParsedOptions as DiagnosticsCommands;
            Assert.IsNotNull(di);
            Assert.That(di.EnableDisableDump, Is.EqualTo(1));
            Assert.That(di.Fix);
        }

        [TestCase("diagnostics", "-f", "-u", "1")]
        [TestCase("diagnostics", "-u", "1", "-f")]
        [TestCase("diagnostics", "-u", "1", "-f")]
        public void ThatFuslogOptionsWorks(string a, string b, string c, string d)
        {
            var args = new[] { a,b,c,d};
            var result = fclp.Parse(args);
            Assert.That(result.HasErrors, Is.Not.True);
            var di = setup.ParsedOptions as DiagnosticsCommands;
            Assert.IsNotNull(di);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.EnableDisableFuslog,Is.EqualTo(1));
            Assert.That(di.Fix);
        }

        [TestCase("diagnostics", "-f", "-u", "1",1)]
        [TestCase("diagnostics", "-f", "-u", "0",0)]
        [TestCase("diagnostics", "-u", "0", "-f",0)]
        public void ThatFuslogOptionsWorks4Disable(string a, string b, string c, string d,int expected)
        {
            var args = new[] { a, b, c, d };
            var result = fclp.Parse(args);
            Assert.That(result.HasErrors, Is.Not.True);
            var di = setup.ParsedOptions as DiagnosticsCommands;
            Assert.IsNotNull(di);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.EnableDisableFuslog, Is.EqualTo(expected));
            Assert.That(di.Fix);
        }

        [TestCase("diagnostics", "--fix", "--fuslog", "1", 1)]
        [TestCase("diagnostics", "--fix", "--fuslog", "0", 0)]
        [TestCase("diagnostics", "-u", "0", "-f", 0)]
        public void ThatFuslogOptionsWorks4DisableWithFullOptions(string a, string b, string c, string d, int expected)
        {
            var args = new[] { a, b, c, d };
            var result = fclp.Parse(args);
            Assert.That(result.HasErrors, Is.Not.True);
            var di = setup.ParsedOptions as DiagnosticsCommands;
            Assert.IsNotNull(di);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.EnableDisableFuslog, Is.EqualTo(expected));
            Assert.That(di.Fix);
        }


        [Test]
        public void ThatDiagnosticOptionsForDumpFolderWorks()
        {
            var args = new[] { "diagnostics", "-D", @"C:\CrashDumps", "-f" };
            var result = fclp.Parse(args);
            Assert.That(result.HasErrors, Is.Not.True);
            var di = setup.ParsedOptions as DiagnosticsCommands;
            Assert.IsNotNull(di);
            Assert.That(di.EnableDisableDump, Is.EqualTo(-1));
            Assert.That(di.Fix);
            Assert.That(di.DumpFolder,Is.EqualTo(@"C:\CrashDumps"));
        }
    }
}
