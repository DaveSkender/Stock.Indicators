using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Dpo : TestBase
    {
        [TestMethod]
        public void Standard()
        {
            // get expected data
            List<Quote> qot = new();
            List<DpoResult> exp = new();

            List<string> csvData = File.ReadAllLines("A-D/Dpo/Dpo.Data.csv")
                .Skip(1)
                .ToList();

            for (int i = 0; i < csvData.Count; i++)
            {
                string[] csv = csvData[i].Split(",");
                DateTime date = Convert.ToDateTime(csv[1], EnglishCulture);

                qot.Add(new Quote
                {
                    Date = date,
                    Close = csv[5].ToDecimal()
                });

                exp.Add(new DpoResult
                {
                    Date = date,
                    Sma = csv[6].ToDecimalNull(),
                    Dpo = csv[7].ToDecimalNull()
                });
            }

            // calculate actual data
            List<DpoResult> act = qot.GetDpo(14)
                .ToList();

            // assertions
            Assert.AreEqual(exp.Count, act.Count);

            // compare all values
            for (int i = 0; i < exp.Count; i++)
            {
                DpoResult e = exp[i];
                DpoResult a = act[i];

                Assert.AreEqual(e.Date, a.Date);
                Assert.AreEqual(e.Sma, a.Sma.NullRound(5), $"at index {i}");
                Assert.AreEqual(e.Dpo, a.Dpo.NullRound(5), $"at index {i}");
            }
        }

        [TestMethod]
        public void ConvertToQuotes()
        {
            List<Quote> newQuotes = quotes.GetDpo(14)
                .ConvertToQuotes()
                .ToList();

            Assert.AreEqual(489, newQuotes.Count);

            Quote q = newQuotes.LastOrDefault();
            Assert.AreEqual(2.18214m, Math.Round(q.Close, 5));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<DpoResult> r = badQuotes.GetDpo(5);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetDpo(0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetDpo(TestData.GetDefault(10), 11));
        }
    }
}
