using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Cmf : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<CmfResult> results = quotes.GetCmf(20).ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Cmf != null).Count());

            // sample values
            CmfResult r1 = results[49];
            Assert.AreEqual(0.5468, Math.Round(r1.MoneyFlowMultiplier, 4));
            Assert.AreEqual(55609259, Math.Round(r1.MoneyFlowVolume, 2));
            Assert.AreEqual(0.350596, Math.Round((double)r1.Cmf, 6));

            CmfResult r2 = results[249];
            Assert.AreEqual(0.7778, Math.Round(r2.MoneyFlowMultiplier, 4));
            Assert.AreEqual(36433792.89, Math.Round(r2.MoneyFlowVolume, 2));
            Assert.AreEqual(-0.040226, Math.Round((double)r2.Cmf, 6));

            CmfResult r3 = results[501];
            Assert.AreEqual(0.8052, Math.Round(r3.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25, Math.Round(r3.MoneyFlowVolume, 2));
            Assert.AreEqual(-0.123754, Math.Round((double)r3.Cmf, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<CmfResult> r = Indicator.GetCmf(badQuotes, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void BigData()
        {
            IEnumerable<CmfResult> r = Indicator.GetCmf(bigQuotes, 150);
            Assert.AreEqual(1246, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<CmfResult> results = quotes.GetCmf(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            CmfResult last = results.LastOrDefault();
            Assert.AreEqual(0.8052, Math.Round(last.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25, Math.Round(last.MoneyFlowVolume, 2));
            Assert.AreEqual(-0.123754, Math.Round((double)last.Cmf, 6));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetCmf(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetCmf(TestData.GetDefault(20), 20));
        }
    }
}
