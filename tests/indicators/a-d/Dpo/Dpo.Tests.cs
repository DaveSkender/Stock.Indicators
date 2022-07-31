using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Dpo : TestBase
{
    [TestMethod]
    public void Standard()
    {
        // get expected data
        List<Quote> qot = new();
        List<DpoResult> exp = new();

        List<string> csvData = File.ReadAllLines("a-d/Dpo/Dpo.Data.csv")
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

            exp.Add(new DpoResult(date)
            {
                Sma = csv[6].ToDoubleNull(),
                Dpo = csv[7].ToDoubleNull()
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
            Assert.AreEqual(e.Sma, NullMath.Round(a.Sma, 5), $"at index {i}");
            Assert.AreEqual(e.Dpo, NullMath.Round(a.Dpo, 5), $"at index {i}");
        }
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<DpoResult> results = quotes
            .Use(CandlePart.Close)
            .GetDpo(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(489, results.Count(x => x.Dpo != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<DpoResult> r = tupleNanny.GetDpo(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Dpo is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<DpoResult> results = quotes
            .GetSma(2)
            .GetDpo(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(488, results.Count(x => x.Dpo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetDpo(14)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(480, results.Count(x => x.Sma is not null and not double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<DpoResult> r = badQuotes.GetDpo(5);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Dpo is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<DpoResult> r0 = noquotes.GetDpo(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<DpoResult> r1 = onequote.GetDpo(5);
        Assert.AreEqual(1, r1.Count());
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetDpo(0));
}
