namespace BufferLists;

[TestClass]
public class Slope : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<SlopeResult> series
       = Quotes.ToSlope(lookbackPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        SlopeList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        SlopeList sut = new(lookbackPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        SlopeList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<SlopeResult> expected = subset.ToSlope(lookbackPeriods);

        SlopeList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        SlopeList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        SlopeList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        SlopeList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        SlopeList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<SlopeResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void AddSingleValue_ValidatesHistoricalRepaint()
    {
        // Test that Line values are correctly updated as new data arrives
        // This verifies the legitimate historical repaint behavior

        SlopeList sut = new(lookbackPeriods);

        // Add first set of values
        foreach (Quote quote in Quotes.Take(lookbackPeriods + 5))
        {
            sut.Add(quote);
        }

        // Get Line value from a middle point
        const int midIndex = lookbackPeriods + 2;
        decimal? lineBefore = sut[midIndex].Line;

        // Add one more value - this should update Line values for the last lookbackPeriods results
        sut.Add(Quotes[lookbackPeriods + 5]);

        // The Line value at midIndex may have changed because it's within the window
        // that gets updated when new data arrives
        decimal? lineAfter = sut[midIndex].Line;

        // Verify the final result matches series implementation
        List<Quote> expectedBatch = Quotes.Take(lookbackPeriods + 6).ToList();
        IReadOnlyList<SlopeResult> expected = expectedBatch.ToSlope(lookbackPeriods);
        sut.IsExactly(expected);
    }
}
