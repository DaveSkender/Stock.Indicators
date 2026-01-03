namespace Tests.Indicators;

[TestClass]
public class DpoTests : TestBase
{
    [TestMethod]
    public void Standard()
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
            DateTime date = Convert.ToDateTime(csv[1], EnglishCulture);

            qot.Add(new Quote {
                Date = date,
                Close = csv[5].ToDecimal()
            });

            exp.Add(new DpoResult(date) {
                Sma = csv[6].ToDoubleNull(),
                Dpo = csv[7].ToDoubleNull()
            });
        }

        // calculate actual data
        List<DpoResult> act = qot.GetDpo(14)
            .ToList();

        // assertions
        Assert.HasCount(exp.Count, act);

        // compare all values
        for (int i = 0; i < exp.Count; i++)
        {
            DpoResult e = exp[i];
            DpoResult a = act[i];

            Assert.AreEqual(e.Date, a.Date);
            Assert.AreEqual(e.Sma, a.Sma.Round(5), $"at index {i}");
            Assert.AreEqual(e.Dpo, a.Dpo.Round(5), $"at index {i}");
        }
    }

    [TestMethod]
    public void UseTuple()
    {
        List<DpoResult> results = quotes
            .Use(CandlePart.Close)
            .GetDpo(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(static x => x.Dpo != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<DpoResult> r = tupleNanny
            .GetDpo(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(static x => x.Dpo is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<DpoResult> results = quotes
            .GetSma(2)
            .GetDpo(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Dpo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetDpo(14)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Sma is not null and not double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        List<DpoResult> r = badQuotes
            .GetDpo(5)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Dpo is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<DpoResult> r0 = noquotes
            .GetDpo(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<DpoResult> r1 = onequote
            .GetDpo(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetDpo(0));
}
