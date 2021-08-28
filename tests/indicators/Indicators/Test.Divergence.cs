using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Divergence : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            IEnumerable<PivotsResult> pricePivots = quotes.GetPivots();
            IEnumerable<RsiResult> rsi = quotes.GetRsi(14);

            //foreach (RsiResult r in rsi)
            //{
            //    Console.WriteLine($"{r.Date:d},{r.Rsi:N4}");
            //}

            IEnumerable<PivotsResult> rsiPivots = rsi
                .ConvertToQuotes()
                .GetPivots();

            //foreach (PivotsResult r in rsiPivots)
            //{
            //    Console.WriteLine($"{r.Date:d},{r.HighPoint:N2},{r.HighTrend},{r.HighLine:N4},"
            //                     + $"{r.LowPoint:N2},{r.LowTrend},{r.LowLine:N4}");
            //}

            List<DivergenceResult> results
               = Indicator.CalcDivergence(pricePivots, rsiPivots)
                .ToList();

            //foreach (DivergenceResult r in results)
            //{
            //    Console.WriteLine($"{r.Date:d},{r.HighPoint:N2},{r.HighTrend},{r.HighLine:N4},"
            //                     + $"{r.LowPoint:N2},{r.LowTrend},{r.LowLine:N4}");
            //}

            // assertions

            // proper quantities
            Assert.AreEqual(488, results.Count);
            Assert.AreEqual(0, results.Where(x => x.BullishRegular != null).Count());
            Assert.AreEqual(0, results.Where(x => x.BullishHidden != null).Count());
            Assert.AreEqual(0, results.Where(x => x.BearishRegular != null).Count());
            Assert.AreEqual(0, results.Where(x => x.BearishHidden != null).Count());

            // sample values
            DivergenceResult r3 = results[3];
            Assert.AreEqual(null, r3.BullishRegular);
            Assert.AreEqual(null, r3.BullishHidden);
            Assert.AreEqual(null, r3.BearishRegular);
            Assert.AreEqual(null, r3.BearishHidden);

            DivergenceResult r7 = results[7];

            DivergenceResult r120 = results[120];

            DivergenceResult r180 = results[180];

            DivergenceResult r250 = results[250];

            DivergenceResult r472 = results[472];

            DivergenceResult r497 = results[497];

            DivergenceResult r498 = results[498];
        }


        [TestMethod]
        public void BadData()
        {
            // TO DO: Test bad data in caller
        }

        [TestMethod]
        public void Exceptions()
        {
            Assert.Fail();
            //// different lengths
            //Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            //    Indicator.GetDivergence());

            //// different dates
            //Assert.ThrowsException<BadQuotesException>(() =>
            //    Indicator.GetDivergence());
        }
    }
}
