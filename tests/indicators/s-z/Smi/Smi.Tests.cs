using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Smi : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<SmiResult> results = quotes.GetSmi(14, 20, 5, 3)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(489, results.Where(x => x.Smi != null).Count());
            Assert.AreEqual(489, results.Where(x => x.Signal != null).Count());

            // sample values
            SmiResult r12 = results[12];
            Assert.IsNull(r12.Smi);
            Assert.IsNull(r12.Signal);

            SmiResult r13 = results[13];
            Assert.AreEqual(17.2603m, Math.Round((decimal)r13.Smi, 4));
            Assert.AreEqual(17.2603m, Math.Round((decimal)r13.Signal, 4));

            SmiResult r14 = results[14];
            Assert.AreEqual(18.6086m, Math.Round((decimal)r14.Smi, 4));
            Assert.AreEqual(17.9344m, Math.Round((decimal)r14.Signal, 4));

            SmiResult r28 = results[28];
            Assert.AreEqual(51.0417m, Math.Round((decimal)r28.Smi, 4));
            Assert.AreEqual(47.1207m, Math.Round((decimal)r28.Signal, 4));

            SmiResult r150 = results[150];
            Assert.AreEqual(65.6692m, Math.Round((decimal)r150.Smi, 4));
            Assert.AreEqual(66.3292m, Math.Round((decimal)r150.Signal, 4));

            SmiResult r250 = results[250];  // also testing aliases here
            Assert.AreEqual(67.2534m, Math.Round((decimal)r250.Smi, 4));
            Assert.AreEqual(67.6261m, Math.Round((decimal)r250.Signal, 4));

            SmiResult r501 = results[501];
            Assert.AreEqual(-52.6560m, Math.Round((decimal)r501.Smi, 4));
            Assert.AreEqual(-54.1903m, Math.Round((decimal)r501.Signal, 4));
        }

        [TestMethod]
        public void NoSignal()
        {

            List<SmiResult> results = quotes.GetSmi(5, 20, 20, 1)
                .ToList();

            // signal equals oscillator
            SmiResult r1 = results[487];
            Assert.AreEqual(r1.Smi, r1.Signal);

            SmiResult r2 = results[501];
            Assert.AreEqual(r2.Smi, r2.Signal);
        }

        [TestMethod]
        public void SmallPeriods()
        {

            List<SmiResult> results = quotes.GetSmi(1, 1, 1, 5)
                .ToList();

            // sample values
            SmiResult r51 = results[51];
            Assert.AreEqual(-100m, Math.Round((decimal)r51.Smi, 4));
            Assert.AreEqual(-20.8709m, Math.Round((decimal)r51.Signal, 4));

            SmiResult r81 = results[81];
            Assert.AreEqual(0m, Math.Round((decimal)r81.Smi, 4));
            Assert.AreEqual(-14.7101m, Math.Round((decimal)r81.Signal, 4));

            SmiResult r88 = results[88];
            Assert.AreEqual(100m, Math.Round((decimal)r88.Smi, 4));
            Assert.AreEqual(47.2291m, Math.Round((decimal)r88.Signal, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmiResult> r = badQuotes.GetSmi(5, 5, 1, 5);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {

            List<SmiResult> results = quotes.GetSmi(14, 20, 5, 3)
                    .RemoveWarmupPeriods()
                    .ToList();

            // assertions
            Assert.AreEqual(501 - (14 + 100), results.Count);

            SmiResult last = results.LastOrDefault();
            Assert.AreEqual(-52.6560m, Math.Round((decimal)last.Smi, 4));
            Assert.AreEqual(-54.1903m, Math.Round((decimal)last.Signal, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmi(quotes, 0, 5, 5, 5));

            // bad first smooth period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmi(quotes, 14, 0, 5, 5));

            // bad second smooth period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmi(quotes, 14, 3, 0, 5));

            // bad signal
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetSmi(9, 3, 1, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetSmi(TestData.GetDefault(129), 30, 3, 3, 3));
        }
    }
}
