using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class FunctionsTests : TestBase
    {

        private readonly double[] closePrice = longishQuotes
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

            DateTime rnDate = evDate.RoundDown(interval);
            DateTime exDate = DateTime.Parse("2020-12-15 09:00:00", englishCulture);

            // assertions
            Assert.AreEqual(exDate, rnDate);
        }

        [TestMethod]
        public void ToTimeSpan()
        {
            Assert.AreEqual(PeriodSize.OneMinute.ToTimeSpan(), TimeSpan.FromMinutes(1));
            Assert.AreEqual(PeriodSize.TwoMinutes.ToTimeSpan(), TimeSpan.FromMinutes(2));
            Assert.AreEqual(PeriodSize.ThreeMinutes.ToTimeSpan(), TimeSpan.FromMinutes(3));
            Assert.AreEqual(PeriodSize.FiveMinutes.ToTimeSpan(), TimeSpan.FromMinutes(5));
            Assert.AreEqual(PeriodSize.FifteenMinutes.ToTimeSpan(), TimeSpan.FromHours(0.25));
            Assert.AreEqual(PeriodSize.ThirtyMinutes.ToTimeSpan(), TimeSpan.FromHours(0.5));
            Assert.AreEqual(PeriodSize.OneHour.ToTimeSpan(), TimeSpan.FromMinutes(60));
            Assert.AreEqual(PeriodSize.TwoHours.ToTimeSpan(), TimeSpan.FromHours(2));
            Assert.AreEqual(PeriodSize.FourHours.ToTimeSpan(), TimeSpan.FromHours(4));
            Assert.AreEqual(PeriodSize.Day.ToTimeSpan(), TimeSpan.FromHours(24));
            Assert.AreEqual(PeriodSize.Week.ToTimeSpan(), TimeSpan.FromDays(7));

            Assert.AreEqual(PeriodSize.Month.ToTimeSpan(), TimeSpan.Zero);
        }

        [TestMethod]
        public void ToPeriodSize()
        {
            //POSITIVE TEST
            Assert.AreEqual(TimeSpan.FromMinutes(1).ToPeriodSize(), PeriodSize.OneMinute);
            Assert.AreEqual(TimeSpan.FromMinutes(2).ToPeriodSize(), PeriodSize.TwoMinutes);
            Assert.AreEqual(TimeSpan.FromMinutes(3).ToPeriodSize(), PeriodSize.ThreeMinutes);
            Assert.AreEqual(TimeSpan.FromMinutes(5).ToPeriodSize(), PeriodSize.FiveMinutes);
            Assert.AreEqual(TimeSpan.FromMinutes(15).ToPeriodSize(), PeriodSize.FifteenMinutes);
            Assert.AreEqual(TimeSpan.FromMinutes(30).ToPeriodSize(), PeriodSize.ThirtyMinutes);
            Assert.AreEqual(TimeSpan.FromHours(1).ToPeriodSize(), PeriodSize.OneHour);
            Assert.AreEqual(TimeSpan.FromHours(2).ToPeriodSize(), PeriodSize.TwoHours);
            Assert.AreEqual(TimeSpan.FromHours(4).ToPeriodSize(), PeriodSize.FourHours);
            Assert.AreEqual(TimeSpan.FromDays(1).ToPeriodSize(), PeriodSize.Day);
            Assert.AreEqual(TimeSpan.FromDays(7).ToPeriodSize(), PeriodSize.Week);

            //NEGATIVE TEST - NOT MATCH --> NULL           
            Assert.IsNull(TimeSpan.FromMinutes(4).ToPeriodSize());
        }
    }
}
