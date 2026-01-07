namespace BufferLists;

[TestClass]
public class Macd : BufferListTestBase, ITestChainBufferList
{
    private const int fastPeriods = 12;
    private const int slowPeriods = 26;
    private const int signalPeriods = 9;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MacdResult> series
       = Quotes.ToMacd(fastPeriods, slowPeriods, signalPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

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
        MacdList sut = Quotes.ToMacdList(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

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
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<MacdResult> expected = subset.ToMacd(fastPeriods, slowPeriods, signalPeriods);

        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void Extension()
    {
        MacdList sut = Quotes.ToMacdList(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (Quote quote in Quotes.Take(100))
        {
            sut.Add(quote);
        }

        MacdResult streamResult = sut[50];
        MacdResult seriesResult = series[50];

        streamResult.Should().Be(seriesResult);
        sut.Should().HaveCount(100);
        sut.IsExactly(series.Take(100));
    }

    [TestMethod]
    public void RealTimeSimulation()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (Quote quote in Quotes.Take(100))
        {
            sut.Add(quote);
        }

        foreach (Quote quote in Quotes.Skip(100).Take(10))
        {
            MacdResult previous = sut[^1];

            sut.Add(quote);

            MacdResult current = sut[^1];

            if (current.Macd.HasValue)
            {
                current.Macd.Should().NotBeNull();
                current.FastEma.Should().NotBeNull();
                current.SlowEma.Should().NotBeNull();
            }

            current.Timestamp.Should().BeAfter(previous.Timestamp);
        }
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<MacdResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void DuplicateTimestamp_UpdatesLastResult()
    {
        // Arrange: Add first 50 quotes
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);
        for (int i = 0; i < 50; i++)
        {
            sut.Add(Quotes[i]);
        }

        // Verify initial state
        sut.Should().HaveCount(50);
        MacdResult? originalLast = sut[49];

        // Act: Add duplicate timestamp with different value
        DateTime lastTimestamp = Quotes[49].Timestamp;
        double newValue = (double)Quotes[49].Close + 5.0;  // Different close price
        sut.Add(lastTimestamp, newValue);

        // Assert: Should still have 50 results (update, not add)
        sut.Should().HaveCount(50);

        // Last result should be updated based on new value
        MacdResult? updatedLast = sut[49];
        updatedLast.Timestamp.Should().Be(lastTimestamp);
        updatedLast.Macd.Should().NotBe(originalLast.Macd);  // Value should have changed

        // Verify mathematical correctness by rebuilding from scratch
        List<IReusable> recomputedData = Quotes.Take(49).Cast<IReusable>().ToList();
        recomputedData.Add(new Quote(lastTimestamp, 0, 0, 0, (decimal)newValue, 0));
        IReadOnlyList<MacdResult> expected = recomputedData.ToMacd(fastPeriods, slowPeriods, signalPeriods);

        sut[49].Macd.Should().BeApproximately(expected[49].Macd, 0.00001);
    }

    [TestMethod]
    public void QuoteCorrection_MaintainsMathematicalPrecision()
    {
        // Arrange: Build full list
        MacdList sut = Quotes.ToMacdList(fastPeriods, slowPeriods, signalPeriods);
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
        IReadOnlyList<MacdResult> expected = correctedData.ToMacd(fastPeriods, slowPeriods, signalPeriods);

        // Verify the corrected value matches exactly
        sut[lastIndex].Macd.Should().BeApproximately(expected[lastIndex].Macd, 0.00001);
    }
}
