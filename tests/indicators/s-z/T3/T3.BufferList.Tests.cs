namespace BufferLists;

[TestClass]
public class T3 : BufferListTestBase
{
    private const int lookbackPeriods = 5;
    private const double volumeFactor = 0.7;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<T3Result> series
       = Quotes.ToT3(lookbackPeriods, volumeFactor);

    [TestMethod]
    public void FromReusableSplit()
    {
        T3List sut = new(lookbackPeriods, volumeFactor);

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
        T3List sut = new(lookbackPeriods, volumeFactor);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        T3List sut = new(lookbackPeriods, volumeFactor) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        T3List sut = new(lookbackPeriods, volumeFactor);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        T3List sut = new(lookbackPeriods, volumeFactor) { Quotes };

        IReadOnlyList<T3Result> series
            = Quotes.ToT3(lookbackPeriods, volumeFactor);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        T3List sut = new(lookbackPeriods, volumeFactor);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<T3Result> expected = subset.ToT3(lookbackPeriods, volumeFactor);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
