namespace BufferLists;

[TestClass]
public class Prs : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<PrsResult> series
       = OtherQuotes.ToPrs(Quotes, lookbackPeriods);

    [TestMethod]
    public void AddReusableItems()
    {
        PrsList sut = new(lookbackPeriods);

        for (int i = 0; i < OtherQuotes.Count; i++)
        {
            sut.Add(OtherQuotes[i], Quotes[i]);
        }

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        PrsList sut = new(lookbackPeriods) {
            { OtherQuotes, Quotes }
        };

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        PrsList sut = new(lookbackPeriods);

        for (int i = 0; i < OtherQuotes.Count; i++)
        {
            sut.Add(OtherQuotes[i].Timestamp, (double)OtherQuotes[i].Close, (double)Quotes[i].Close);
        }

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotes()
    {
        // PRS uses paired series, so this test uses OtherQuotes and Quotes as series
        PrsList sut = new(lookbackPeriods);

        for (int i = 0; i < OtherQuotes.Count; i++)
        {
            sut.Add(OtherQuotes[i], Quotes[i]);
        }

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        // PRS uses paired series, so this test uses OtherQuotes and Quotes as series
        PrsList sut = new(lookbackPeriods) {
            { OtherQuotes, Quotes }
        };

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        // PRS uses paired series, so this test uses OtherQuotes and Quotes as series
        PrsList sut = new(lookbackPeriods, OtherQuotes, Quotes);

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithSeriesCtor()
    {
        PrsList sut = new(lookbackPeriods, OtherQuotes, Quotes);

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ToPrsListExtension()
    {
        PrsList sut = OtherQuotes.ToPrsList(Quotes, lookbackPeriods);

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> evalSubset = OtherQuotes.Take(80).ToList();
        List<Quote> baseSubset = Quotes.Take(80).ToList();
        IReadOnlyList<PrsResult> expected = evalSubset.ToPrs(baseSubset, lookbackPeriods);

        PrsList sut = new(lookbackPeriods, evalSubset, baseSubset);

        sut.Should().HaveCount(evalSubset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(evalSubset, baseSubset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        PrsList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(OtherQuotes, Quotes);

        IReadOnlyList<PrsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ThrowsOnMismatchedTimestamps()
    {
        PrsList sut = new(lookbackPeriods);

        Quote evalItem = OtherQuotes[0];
        Quote baseItem = Quotes[1]; // Different timestamp

        Action act = () => sut.Add(evalItem, baseItem);

        act.Should().Throw<InvalidQuotesException>()
            .WithMessage("*Timestamp sequence does not match*");
    }

    [TestMethod]
    public void ThrowsOnDifferentSeriesLengths()
    {
        PrsList sut = new(lookbackPeriods);

        List<Quote> shortSeriesBase = Quotes.Take(OtherQuotes.Count - 1).ToList();

        Action act = () => sut.Add(OtherQuotes, shortSeriesBase);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*must have the same number of items*");
    }

    [TestMethod]
    public void NoLookbackPeriods()
    {
        // Test with int.MinValue (no lookback calculation)
        IReadOnlyList<PrsResult> expectedNoLookback = OtherQuotes.ToPrs(Quotes);
        PrsList sut = new(int.MinValue, OtherQuotes, Quotes);

        sut.Should().HaveCount(OtherQuotes.Count);
        sut.Should().BeEquivalentTo(expectedNoLookback, options => options.WithStrictOrdering());
    }
}
