namespace BufferLists;

[TestClass]
public class MaEnvelopes : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 20;
    private const double percentOffset = 2.5;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MaEnvelopeResult> series
       = Quotes.ToMaEnvelopes(lookbackPeriods, percentOffset);

    [TestMethod]
    public override void AddQuotes()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        MaEnvelopesList sut = Quotes.ToMaEnvelopesList(lookbackPeriods, percentOffset);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset, MaType.SMA, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithEmaType()
    {
        IReadOnlyList<MaEnvelopeResult> expected = Quotes.ToMaEnvelopes(
            lookbackPeriods, percentOffset, MaType.EMA);

        MaEnvelopesList sut = Quotes.ToMaEnvelopesList(
            lookbackPeriods, percentOffset, MaType.EMA);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<MaEnvelopeResult> expected = subset.ToMaEnvelopes(lookbackPeriods, percentOffset);

        MaEnvelopesList sut = new(lookbackPeriods, percentOffset, MaType.SMA, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        MaEnvelopesList sut = new(lookbackPeriods, percentOffset) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<MaEnvelopeResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
