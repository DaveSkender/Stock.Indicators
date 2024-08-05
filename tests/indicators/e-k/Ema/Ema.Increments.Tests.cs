namespace Increments;

[TestClass]
public class Ema : IncrementsTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly double[] primatives
       = Quotes
        .Select(x => (double)x.Close)
        .ToArray();

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<EmaResult> series
       = Quotes
        .ToEma(lookbackPeriods);

    private static readonly List<double?> seriesArray = series
        .Select(x => x.Ema)
        .ToList();

    [TestMethod]
    public void FromReusableSplit()
    {
        EmaInc sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        EmaInc sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        EmaInc sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        EmaInc sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        EmaInc sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromPrimitive()
    {
        EmaIncPrimitive sut = new(lookbackPeriods);

        foreach (double p in primatives) { sut.Add(p); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(seriesArray);
    }

    [TestMethod]
    public void FromPrimitiveBatch()
    {
        EmaIncPrimitive sut = new(lookbackPeriods) { primatives };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(seriesArray);
    }
}
