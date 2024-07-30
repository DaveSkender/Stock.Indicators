namespace Increments;

[TestClass]
public class Ema : IncrementsTestBase
{
    private static readonly double[] primatives
       = Quotes
        .Select(x => (double)x.Close)
        .ToArray();

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    [TestMethod]
    public void FromReusable()
    {
        EmaInc sut = new(14);

        foreach (IReusable item in reusables)
        {
            sut.AddValue(item.Timestamp, item.Value);
        }

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(14);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        EmaInc sut = new(14);

        foreach (IReusable item in reusables)
        {
            sut.AddValue(item);
        }

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(14);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        EmaInc sut = new(14);

        sut.AddValues(reusables);

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(14);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        EmaInc sut = new(14);

        foreach (Quote q in Quotes)
        {
            sut.AddValue(q);
        }

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(14);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        EmaInc sut = new(14);

        sut.AddValues(Quotes);

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(14);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromPrimitive()
    {
        EmaIncPrimitive sut = new(14);

        foreach (double p in primatives)
        {
            sut.AddValue(p);
        }

        List<double?> series = Quotes
            .ToEma(14)
            .Select(x => x.Ema)
            .ToList();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromPrimitiveBatch()
    {
        EmaIncPrimitive sut = new(14);

        sut.AddValues(primatives);

        List<double?> series = Quotes
            .ToEma(14)
            .Select(x => x.Ema)
            .ToList();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }
}
