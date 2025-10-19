namespace BufferList;

[TestClass]
public class VolatilityStop : BufferListTestBase, ITestQuoteBufferList
{
    private const int lookbackPeriods = 14;
    private const double multiplier = 3;
    private static readonly IReadOnlyList<VolatilityStopResult> expected
        = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier);

        foreach (Quote q in Quotes)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        VolatilityStopList sut = Quotes.ToVolatilityStopList(lookbackPeriods, multiplier);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<VolatilityStopResult> expected = subset.ToVolatilityStop(lookbackPeriods, multiplier);

        VolatilityStopList sut = new(lookbackPeriods, multiplier, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier) {
            MaxListSize = 100
        };

        sut.Add(Quotes);

        sut.Should().HaveCount(100);

        // Verify the last 100 items match series results
        IReadOnlyList<VolatilityStopResult> expectedLast100 = expected.Skip(expected.Count - 100).ToList();

        sut.Should().BeEquivalentTo(expectedLast100, options => options.WithStrictOrdering());
    }
}
