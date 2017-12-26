using System.Linq;
using System.Linq.Expressions;
using Fclp;
using IFix;
using NUnit.Framework;

namespace IFixTests
{
    public class OptionsTests
    {
        [Test]
        public void ThatDiagnosticOptionsWorksDumpDisable()
        {
            var args = new[] {"diagnostics","-d","0","-f"};

            var fclp = new FluentCommandLineParser {IsCaseSensitive = true};
            var setup = new SetupCommands(fclp)
            {
                DoExecute = false
            };

            var result = fclp.Parse(args);
            Assert.That(result.HasErrors,Is.Not.True);
            Assert.That(!result.AdditionalOptionsFound.Any());
            //var obj = fclp.
            //var di = new DiagnosticsCommands();
            //var res = CommandLine.Parser.Default.ParseArguments(args, di);
            //Assert.That(res);
            //Assert.That(di.EnableDisableDump,Is.EqualTo(0));
            //Assert.That(di.Fix);
        }

        [Test]
        public void ThatDiagnosticOptionsWorksDumpEnable()
        {
            var args = new[] { "diagnostics", "-d", "1", "-f" };
            var di = new DiagnosticsCommands();
            //var res = CommandLine.Parser.Default.ParseArguments(args, di);
            //Assert.That(res);
            //Assert.That(di.EnableDisableDump, Is.EqualTo(1));
            //Assert.That(di.Fix);
        }


        [Test]
        public void ThatDiagnosticOptionsForDumpFolderWorks()
        {
            var args = new[] { "diagnostics", "-D", @"C:\CrashDumps", "-f" };
            var di = new DiagnosticsCommands();
            //var res = CommandLine.Parser.Default.ParseArguments(args, di);
            //Assert.That(res);
            //Assert.That(di.EnableDisableDump, Is.EqualTo(null));
            //Assert.That(di.Fix);
            //Assert.That(di.DumpFolder,Is.EqualTo(@"C:\CrashDumps"));
        }
    }
}
