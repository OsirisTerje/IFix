using System.Linq;
using System.Linq.Expressions;
using Fclp;
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
