using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class FunctionsTests : TestBase
    {

        private readonly double[] closePrice = History.GetHistoryLong()
            .Select(x => (double)x.Close)
            .ToArray();


        [TestMethod()]
        public void StdDevTest()
        {
            double sd = Functions.StdDev(closePrice);

            // assertions
            Assert.AreEqual(633.932098287, Math.Round(sd, 9));

        }
    }
}