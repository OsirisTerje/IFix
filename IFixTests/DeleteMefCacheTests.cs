using NUnit.Framework;
using IFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTest;

namespace IFix.Tests
{
   
    public class DeleteMefCacheTests
    {
        [LocalOnly]
        [Test()]
        public void CheckCacheForVS2012()
        {
            var sut = new Vs2012Cache();
            var locations = sut.Locations;
            foreach ( var loc in locations)
                Assert.That(loc.Contains("VisualStudio"),Is.True);
        }
    }
}