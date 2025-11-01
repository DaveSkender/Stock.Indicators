namespace BufferLists;

[TestClass]
public class Ultimate : BufferListTestBase
{
    private const int shortPeriods = 7;
    private const int middlePeriods = 14;
    private const int longPeriods = 28;

    private static readonly IReadOnlyList<UltimateResult> series
       = Quotes.ToUltimate(shortPeriods, middlePeriods, longPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<UltimateResult> expected = subset.ToUltimate(shortPeriods, middlePeriods, longPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods) {
            MaxListSize = maxListSize
        };

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        IReadOnlyList<UltimateResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
