namespace StaticSeries;

[TestClass]
public class Dpo : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // get expected data
        List<Quote> qot = [];
        List<DpoResult> exp = [];

        List<string> csvData = File.ReadAllLines("a-d/Dpo/Dpo.Data.csv")
            .Skip(1)
            .ToList();

        for (int i = 0; i < csvData.Count; i++)
        {
            string[] csv = csvData[i].Split(",");
            DateTime date = Convert.ToDateTime(csv[1], invariantCulture);

            qot.Add(new Quote(date, 0, 0, 0, Close: csv[5].ToDecimal(), 0));
            exp.Add(new(date, csv[7].ToDoubleNull(), csv[6].ToDoubleNull()));
        }

        // calculate actual data
        IReadOnlyList<DpoResult> act = qot.ToDpo(14);

        // assertions
        Assert.AreEqual(exp.Count, act.Count);

        // compare all values
        for (int i = 0; i < exp.Count; i++)
        {
            DpoResult e = exp[i];
            DpoResult a = act[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            Assert.AreEqual(e.Sma, a.Sma.Round(5), $"at index {i}");
            Assert.AreEqual(e.Dpo, a.Dpo.Round(5), $"at index {i}");
        }
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<DpoResult> results = Quotes
            .Use(CandlePart.Close)
            .ToDpo(14);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.Dpo != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DpoResult> results = Quotes
            .ToSma(2)
            .ToDpo(14);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Dpo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToDpo(14)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma is not null and not double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<DpoResult> r = BadQuotes
            .ToDpo(5);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dpo is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<DpoResult> r0 = Noquotes
            .ToDpo(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<DpoResult> r1 = Onequote
            .ToDpo(5);

        Assert.AreEqual(1, r1.Count);
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(()
            => Quotes.ToDpo(0));
}
