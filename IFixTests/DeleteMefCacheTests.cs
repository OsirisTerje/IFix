using NUnit.Framework;
using IFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFix.Tests
{
    [TestFixture()]
    public class DeleteMefCacheTests
    {
        [Test()]
        public void CheckCacheForVS2012()
        {
            var sut = new Vs2012Cache();
            Assert.That(sut.Location.Contains("VisualStudio"),Is.True);
        }
    }
}