using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Gator : TestBase
    {
        [TestMethod]
        public void Standard()
        {
            List<GatorResult> results = Indicator.GetGator(history)
                .ToList();

            // assertions

            foreach (GatorResult r in results)
            {
                Console.WriteLine("{0:d},{1:N4},{2:N4},{3},{4}",
                    r.Date, r.Upper, r.Lower, r.UpperIsExpanding, r.LowerIsExpanding);
            }

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.Upper != null).Count());
            Assert.AreEqual(490, results.Where(x => x.Lower != null).Count());
            Assert.AreEqual(481, results.Where(x => x.UpperIsExpanding != null).Count());
            Assert.AreEqual(489, results.Where(x => x.LowerIsExpanding != null).Count());

            // sample values
            GatorResult r11 = results[11];
            Assert.IsNull(r11.Upper);
            Assert.IsNull(r11.Lower);
            Assert.IsNull(r11.UpperIsExpanding);
            Assert.IsNull(r11.LowerIsExpanding);

            GatorResult r12 = results[12];
            Assert.IsNull(r12.Upper);
            Assert.AreEqual(-0.1402m, Math.Round(r12.Lower.Value, 4));
            Assert.IsNull(r12.UpperIsExpanding);
            Assert.IsNull(r12.LowerIsExpanding);

            GatorResult r13 = results[13];
            Assert.IsNull(r13.Upper);
            Assert.AreEqual(-0.0406m, Math.Round(r13.Lower.Value, 4));
            Assert.IsNull(r13.UpperIsExpanding);
            Assert.IsFalse(r13.LowerIsExpanding);

            GatorResult r19 = results[19];
            Assert.IsNull(r19.Upper);
            Assert.AreEqual(-1.0018m, Math.Round(r19.Lower.Value, 4));
            Assert.IsNull(r19.UpperIsExpanding);
            Assert.IsTrue(r19.LowerIsExpanding);

            GatorResult r20 = results[20];
            Assert.AreEqual(0.4004m, Math.Round(r20.Upper.Value, 4));
            Assert.AreEqual(-1.0130m, Math.Round(r20.Lower.Value, 4));
            Assert.IsNull(r20.UpperIsExpanding);
            Assert.IsTrue(r20.LowerIsExpanding);

            GatorResult r21 = results[21];
            Assert.AreEqual(0.7298m, Math.Round(r21.Upper.Value, 4));
            Assert.AreEqual(-0.6072m, Math.Round(r21.Lower.Value, 4));
            Assert.IsTrue(r21.UpperIsExpanding);
            Assert.IsFalse(r21.LowerIsExpanding);

            GatorResult r99 = results[99];
            Assert.AreEqual(0.5159m, Math.Round(r99.Upper.Value, 4));
            Assert.AreEqual(-0.2320m, Math.Round(r99.Lower.Value, 4));
            Assert.IsFalse(r99.UpperIsExpanding);
            Assert.IsTrue(r99.LowerIsExpanding);

            GatorResult r249 = results[249];
            Assert.AreEqual(3.1317m, Math.Round(r249.Upper.Value, 4));
            Assert.AreEqual(-1.8058m, Math.Round(r249.Lower.Value, 4));
            Assert.IsTrue(r249.UpperIsExpanding);
            Assert.IsFalse(r249.LowerIsExpanding);

            GatorResult r501 = results[501];
            Assert.AreEqual(7.4538m, Math.Round(r501.Upper.Value, 4));
            Assert.AreEqual(-9.2399m, Math.Round(r501.Lower.Value, 4));
            Assert.IsTrue(r501.UpperIsExpanding);
            Assert.IsTrue(r501.LowerIsExpanding);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<GatorResult> r = Indicator.GetGator(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetGator(HistoryTestData.Get(114)));
        }
    }
}
