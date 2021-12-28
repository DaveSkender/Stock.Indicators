using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ParabolicSar : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            decimal acclerationStep = 0.02m;
            decimal maxAccelerationFactor = 0.2m;

            List<ParabolicSarResult> results =
                quotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                    .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Sar != null).Count());

            // sample values
            ParabolicSarResult r14 = results[14];
            Assert.AreEqual(212.83m, r14.Sar);
            Assert.AreEqual(true, r14.IsReversal);

            ParabolicSarResult r16 = results[16];
            Assert.AreEqual(212.9924m, Math.Round((decimal)r16.Sar, 4));
            Assert.AreEqual(false, r16.IsReversal);

            ParabolicSarResult r94 = results[94];
            Assert.AreEqual(228.36m, r94.Sar);
            Assert.AreEqual(false, r94.IsReversal);

            ParabolicSarResult r501 = results[501];
            Assert.AreEqual(229.7662m, Math.Round((decimal)r501.Sar, 4));
            Assert.AreEqual(false, r501.IsReversal);
        }

        [TestMethod]
        public void Extended()
        {
            decimal acclerationStep = 0.02m;
            decimal maxAccelerationFactor = 0.2m;
            decimal initialStep = 0.01m;

            List<ParabolicSarResult> results =
                quotes.GetParabolicSar(
                    acclerationStep, maxAccelerationFactor, initialStep)
                    .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Sar != null).Count());

            // sample values
            ParabolicSarResult r14 = results[14];
            Assert.AreEqual(212.83m, r14.Sar);
            Assert.AreEqual(true, r14.IsReversal);

            ParabolicSarResult r16 = results[16];
            Assert.AreEqual(212.9518m, Math.Round((decimal)r16.Sar, 4));
            Assert.AreEqual(false, r16.IsReversal);

            ParabolicSarResult r94 = results[94];
            Assert.AreEqual(228.36m, r94.Sar);
            Assert.AreEqual(false, r94.IsReversal);

            ParabolicSarResult r486 = results[486];
            Assert.AreEqual(273.4148m, r486.Sar);
            Assert.AreEqual(false, r486.IsReversal);

            ParabolicSarResult r501 = results[501];
            Assert.AreEqual(246.73m, r501.Sar);
            Assert.AreEqual(false, r501.IsReversal);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ParabolicSarResult> r = Indicator.GetParabolicSar(badQuotes);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            decimal acclerationStep = 0.02m;
            decimal maxAccelerationFactor = 0.2m;

            List<ParabolicSarResult> results =
                quotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                    .RemoveWarmupPeriods()
                    .ToList();

            // assertions
            Assert.AreEqual(488, results.Count);

            ParabolicSarResult last = results.LastOrDefault();
            Assert.AreEqual(229.7662m, Math.Round((decimal)last.Sar, 4));
            Assert.AreEqual(false, last.IsReversal);
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad acceleration step
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetParabolicSar(quotes, 0, 1));

            // insufficient acceleration step
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetParabolicSar(quotes, 0.02m, 0));

            // step larger than factor
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetParabolicSar(quotes, 6, 2));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetParabolicSar(TestData.GetDefault(1), 0.02m, 0.2m));
        }
    }
}
