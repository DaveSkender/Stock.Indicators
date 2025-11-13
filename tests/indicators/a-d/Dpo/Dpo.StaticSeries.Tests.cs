namespace StaticSeries;

[TestClass]
public class Dpo : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
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
        Assert.HasCount(exp.Count, act);

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

        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(static x => x.Dpo != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DpoResult> results = Quotes
            .ToSma(2)
            .ToDpo(14);

        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Dpo != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToDpo(14)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Sma is not null and not double.NaN));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<DpoResult> r = BadQuotes
            .ToDpo(5);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Dpo is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<DpoResult> r0 = Noquotes
            .ToDpo(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<DpoResult> r1 = Onequote
            .ToDpo(5);

        Assert.HasCount(1, r1);
    }

    /// <summary>
    /// bad SMA period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToDpo(0));
}
