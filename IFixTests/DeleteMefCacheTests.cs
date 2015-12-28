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
            var sut = new Cache("11.0");
            Assert.That(sut.Location.Contains("VisualStudio"),Is.True);
        }
    }
}