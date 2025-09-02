namespace Tests.Indicators;

[TestClass]
public class CmoTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CmoResult> results = quotes
            .GetCmo(14)
            .ToList();

        foreach (CmoResult r in results)
        {
            Console.WriteLine($"{r.Date:d},{r.Cmo:N4}");
        }

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(488, results.Count(x => x.Cmo != null));

        // sample values
        CmoResult r13 = results[13];
        Assert.IsNull(r13.Cmo);

        CmoResult r14 = results[14];
        Assert.AreEqual(24.1081, r14.Cmo.Round(4));

        CmoResult r249 = results[249];
        Assert.AreEqual(48.9614, r249.Cmo.Round(4));

        CmoResult r501 = results[501];
        Assert.AreEqual(-26.7502, r501.Cmo.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<CmoResult> results = quotes
            .Use(CandlePart.Close)
            .GetCmo(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(488, results.Count(x => x.Cmo != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<CmoResult> r = tupleNanny
            .GetCmo(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.Cmo is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<CmoResult> results = quotes
            .GetSma(2)
            .GetCmo(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(481, results.Count(x => x.Cmo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetCmo(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<CmoResult> r = badQuotes
            .GetCmo(35)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Cmo is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CmoResult> r0 = noquotes
            .GetCmo(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<CmoResult> r1 = onequote
            .GetCmo(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<CmoResult> results = quotes
            .GetCmo(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(488, results);

        CmoResult last = results.LastOrDefault();
        Assert.AreEqual(-26.7502, last.Cmo.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetCmo(0));
}
