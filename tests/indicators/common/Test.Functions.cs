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

        [TestMethod]
        public void RoundDownDate()
        {
            TimeSpan interval = PeriodSize.OneHour.ToTimeSpan();
            DateTime evDate = DateTime.Parse("2020-12-15 09:35:45", englishCulture);

            DateTime rnDate = evDate.RoundDownDate(interval);
            DateTime exDate = DateTime.Parse("2020-12-15 09:00:00", englishCulture);

            // assertions
            Assert.AreEqual(exDate, rnDate);
        }

        [TestMethod]
        public void ToTimeSpan()
        {
            Assert.AreEqual(
                PeriodSize.OneHour.ToTimeSpan(),
                TimeSpan.FromHours(1));
        }
    }
}
