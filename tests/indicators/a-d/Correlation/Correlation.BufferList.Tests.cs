namespace BufferLists;

[TestClass]
public class Correlation : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> quotesA
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<IReusable> quotesB
       = Data.GetCompare()
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<CorrResult> series
       = quotesA.ToCorrelation(quotesB, lookbackPeriods);

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        CorrelationList sut = new(lookbackPeriods) {
            { quotesA, quotesB }
        };

        sut.IsBetween(x => x.Correlation, -1, 1);
        sut.IsBetween(x => x.RSquared, 0, 1);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        CorrelationList sut = new(lookbackPeriods);

        for (int i = 0; i < quotesA.Count; i++)
        {
            sut.Add(quotesA[i], quotesB[i]);
        }

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        CorrelationList sut = new(lookbackPeriods) {
            { quotesA, quotesB }
        };

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        CorrelationList sut = new(lookbackPeriods);

        for (int i = 0; i < quotesA.Count; i++)
        {
            sut.Add(quotesA[i].Timestamp, quotesA[i].Value, quotesB[i].Value);
        }

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        // Correlation uses paired series, so this test uses quotesA and quotesB as series
        CorrelationList sut = new(lookbackPeriods);

        for (int i = 0; i < quotesA.Count; i++)
        {
            sut.Add(quotesA[i], quotesB[i]);
        }

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        // Correlation uses paired series, so this test uses quotesA and quotesB as series
        CorrelationList sut = new(lookbackPeriods) {
            { quotesA, quotesB }
        };

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        // Correlation uses paired series, so this test uses quotesA and quotesB as series
        CorrelationList sut = new(lookbackPeriods, quotesA, quotesB);

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithSeriesCtor()
    {
        CorrelationList sut = new(lookbackPeriods, quotesA, quotesB);

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ToCorrelationListExtension()
    {
        CorrelationList sut = quotesA.ToCorrelationList(quotesB, lookbackPeriods);

        sut.Should().HaveCount(quotesA.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<IReusable> subsetA = quotesA.Take(80).ToList();
        List<IReusable> subsetB = quotesB.Take(80).ToList();
        IReadOnlyList<CorrResult> expected = subsetA.ToCorrelation(subsetB, lookbackPeriods);

        CorrelationList sut = new(lookbackPeriods, subsetA, subsetB);

        sut.Should().HaveCount(subsetA.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subsetA, subsetB);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        CorrelationList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(quotesA, quotesB);

        IReadOnlyList<CorrResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ThrowsOnMismatchedTimestamps()
    {
        CorrelationList sut = new(lookbackPeriods);

        IReusable itemA = quotesA[0];
        IReusable itemB = quotesB[1]; // Different timestamp

        Action act = () => sut.Add(itemA, itemB);

        act.Should().Throw<InvalidQuotesException>()
            .WithMessage("*Timestamp sequence does not match*");
    }

    [TestMethod]
    public void ThrowsOnDifferentSeriesLengths()
    {
        CorrelationList sut = new(lookbackPeriods);

        List<IReusable> shortSeriesB = quotesB.Take(quotesA.Count - 1).ToList();

        Action act = () => sut.Add(quotesA, shortSeriesB);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*must have the same number of items*");
    }
}
