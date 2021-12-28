using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Smma : TestBase
    {
        [TestMethod]
        public void Standard()
        {

            List<SmmaResult> results = quotes.GetSmma(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Smma != null).Count());

            // starting calculations at proper index
            Assert.IsNull(results[18].Smma);
            Assert.IsNotNull(results[19].Smma);

            // sample values
            Assert.AreEqual(214.52500m, Math.Round(results[19].Smma.Value, 5));
            Assert.AreEqual(214.55125m, Math.Round(results[20].Smma.Value, 5));
            Assert.AreEqual(214.58319m, Math.Round(results[21].Smma.Value, 5));
            Assert.AreEqual(225.78071m, Math.Round(results[100].Smma.Value, 5));
            Assert.AreEqual(255.67462m, Math.Round(results[501].Smma.Value, 5));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmmaResult> r = Indicator.GetSmma(badQuotes, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<SmmaResult> results = quotes.GetSmma(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (20 + 100), results.Count);
            Assert.AreEqual(255.67462m, Math.Round(results.LastOrDefault().Smma.Value, 5));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmma(quotes, 0));

            // insufficient quotes for N+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetSmma(TestData.GetDefault(129), 30));

            // insufficient quotes for 2×N
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetSmma(TestData.GetDefault(499), 250));
        }
    }
}
