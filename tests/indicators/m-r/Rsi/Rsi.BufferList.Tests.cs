namespace BufferLists;

[TestClass]
public class Rsi : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<RsiResult> series
       = Quotes.ToRsi(lookbackPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        RsiList sut = new(lookbackPeriods);

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
        RsiList sut = Quotes.ToRsiList(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        RsiList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        RsiList sut = new(lookbackPeriods);

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
        RsiList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        RsiList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        RsiList sut = new(14, Quotes);
        sut.IsBetween(static x => x.Rsi, 0, 100);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<RsiResult> expected = subset.ToRsi(lookbackPeriods);

        RsiList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        RsiList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<RsiResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void DuplicateTimestamp_UpdatesLastResult()
    {
        // Arrange: Add first 50 quotes
        RsiList sut = new(lookbackPeriods);
        for (int i = 0; i < 50; i++)
        {
            sut.Add(Quotes[i]);
        }

        // Verify initial state
        sut.Should().HaveCount(50);
        RsiResult? originalLast = sut[49];

        // Act: Add duplicate timestamp with different value
        DateTime lastTimestamp = Quotes[49].Timestamp;
        double newValue = (double)Quotes[49].Close + 5.0;  // Different close price
        sut.Add(lastTimestamp, newValue);

        // Assert: Should still have 50 results (update, not add)
        sut.Should().HaveCount(50);

        // Last result should be updated based on new value
        RsiResult? updatedLast = sut[49];
        updatedLast.Timestamp.Should().Be(lastTimestamp);
        updatedLast.Rsi.Should().NotBe(originalLast.Rsi);  // Value should have changed

        // Verify mathematical correctness by rebuilding from scratch
        List<IReusable> recomputedData = Quotes.Take(49).Cast<IReusable>().ToList();
        recomputedData.Add(new Quote(lastTimestamp, 0, 0, 0, (decimal)newValue, 0));
        IReadOnlyList<RsiResult> expected = recomputedData.ToRsi(lookbackPeriods);

        sut[49].Rsi.Should().BeApproximately(expected[49].Rsi, 0.00001);
    }

    [TestMethod]
    public void QuoteCorrection_MaintainsMathematicalPrecision()
    {
        // Arrange: Build full list
        RsiList sut = Quotes.ToRsiList(lookbackPeriods);
        int lastIndex = Quotes.Count - 1;

        // Act: Correct the last quote with new values
        DateTime lastTimestamp = Quotes[lastIndex].Timestamp;
        double correctedValue = (double)Quotes[lastIndex].Close * 1.1;  // 10% correction
        sut.Add(lastTimestamp, correctedValue);

        // Assert: Count unchanged
        sut.Should().HaveCount(Quotes.Count);

        // Verify correctness by comparing to Series with corrected data
        List<IReusable> correctedData = Quotes.Take(lastIndex).Cast<IReusable>().ToList();
        correctedData.Add(new Quote(lastTimestamp, 0, 0, 0, (decimal)correctedValue, 0));
        IReadOnlyList<RsiResult> expected = correctedData.ToRsi(lookbackPeriods);

        // Verify the corrected value matches exactly
        sut[lastIndex].Rsi.Should().BeApproximately(expected[lastIndex].Rsi, 0.00001);
    }
}
