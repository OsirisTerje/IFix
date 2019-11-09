using NUnit.Framework;

namespace IFix.DiagnosticsTests
{
    public class RegistryTests
    {
        [Test]
        public void ThatRegistryDumpPointsRight()
        {
            var sut = new RegistryDump();
            Assert.That(sut.SubKeyName,Is.EqualTo(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps"));
            var exist = sut.ExistKey();
            Assert.That(exist);

        }
    }
}
