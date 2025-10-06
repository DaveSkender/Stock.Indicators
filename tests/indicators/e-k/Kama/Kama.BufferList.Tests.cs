namespace BufferLists;

[TestClass]
public class Kama : BufferListTestBase
{
    private const int erPeriods = 10;
    private const int fastPeriods = 2;
    private const int slowPeriods = 30;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<KamaResult> series
       = Quotes.ToKama(erPeriods, fastPeriods, slowPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        KamaList sut = new(erPeriods, fastPeriods, slowPeriods);

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
        KamaList sut = new(erPeriods, fastPeriods, slowPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        KamaList sut = new(erPeriods, fastPeriods, slowPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        KamaList sut = new(erPeriods, fastPeriods, slowPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        KamaList sut = new(erPeriods, fastPeriods, slowPeriods) { Quotes };

        IReadOnlyList<KamaResult> series
            = Quotes.ToKama(erPeriods, fastPeriods, slowPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        KamaList sut = new(erPeriods, fastPeriods, slowPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        KamaList sut = new(erPeriods, fastPeriods, slowPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<KamaResult> expected = subset.ToKama(erPeriods, fastPeriods, slowPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
