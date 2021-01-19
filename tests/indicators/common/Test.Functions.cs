using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class FunctionsTests : TestBase
    {

        private readonly double[] closePrice = HistoryTestData.GetLong()
            .Select(x => (double)x.Close)
            .ToArray();


        [TestMethod]
        public void StdDev()
        {
            double sd = Functions.StdDev(closePrice);

            // assertions
            Assert.AreEqual(633.932098287, Math.Round(sd, 9));
        }
    }
}
