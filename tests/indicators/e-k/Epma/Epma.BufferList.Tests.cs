namespace BufferList;

[TestClass]
public class EpmaBufferListTests : BufferListTestBase
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<EpmaResult> series
       = Quotes.ToEpma(lookbackPeriods);

    private static void ValidateResults(IReadOnlyList<EpmaResult> actual, IReadOnlyList<EpmaResult> expected)
    {
        actual.Should().HaveCount(expected.Count);
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void FromReusableSplit()
    {
        EpmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        ValidateResults(sut, series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        EpmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        ValidateResults(sut, series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        EpmaList sut = new(lookbackPeriods) { reusables };

        ValidateResults(sut, series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        EpmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        ValidateResults(sut, series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        EpmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<EpmaResult> expected = Quotes.ToEpma(lookbackPeriods);

        ValidateResults(sut, expected);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        EpmaList sut = new(lookbackPeriods);

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

        IReadOnlyList<EpmaResult> expected = subset.ToEpma(lookbackPeriods);

        ValidateResults(sut, expected);
    }

    [TestMethod]
    public void LargeDatasetWithPruning()
    {
        // Create a dataset larger than the pruning threshold (1000)
        // to verify that pruning doesn't affect accuracy
        const int dataSize = 1500;
        EpmaList sut = new(lookbackPeriods);

        // Generate test data
        List<Quote> largeDataset = [];
        DateTime startDate = new(2020, 1, 1);
        Random random = new(42); // Fixed seed for reproducibility

        for (int i = 0; i < dataSize; i++)
        {
            largeDataset.Add(new Quote {
                Timestamp = startDate.AddDays(i),
                Close = (decimal)(200 + (random.NextDouble() * 20) - 10)
            });
        }

        // Add all data to buffer list
        foreach (Quote quote in largeDataset)
        {
            sut.Add(quote);
        }

        // Verify we have all results
        sut.Should().HaveCount(dataSize);

        // Calculate expected results using static series
        IReadOnlyList<EpmaResult> expected = largeDataset.ToEpma(lookbackPeriods);

        // Verify the buffer list results match static series
        // This confirms pruning doesn't affect accuracy
        ValidateResults(sut, expected);
    }

    [TestMethod]
    public void AutoPruning()
    {
        // Test that EPMA's result list auto-pruning (from base class)
        // works correctly alongside its internal cache pruning
        
        // Create buffer list with small MaxListSize for testing result list pruning
        EpmaList sut = new(lookbackPeriods) { MaxListSize = 100 };

        // Add enough quotes to trigger both:
        // 1. Internal cache pruning (happens at 1000 items)
        // 2. Result list pruning (happens at MaxListSize = 100)
        for (int i = 0; i < 1200; i++)
        {
            Quote quote = Quotes[i % Quotes.Count];
            sut.Add(quote.Timestamp.AddDays(i), (double)quote.Close);
        }

        // Verify result list was pruned to stay under MaxListSize
        sut.Count.Should().BeLessThan(100);
        
        // Verify most recent results are retained and calculations still work
        EpmaResult lastResult = sut[^1];
        lastResult.Should().NotBeNull();
        lastResult.Epma.Should().NotBeNull();
        
        // Store the last result value for comparison
        double? lastEpma = lastResult.Epma;
        
        // Verify that the indicator still produces valid results after pruning
        // (both cache pruning and result list pruning)
        Quote newQuote = Quotes[0];
        sut.Add(newQuote.Timestamp.AddDays(1200), (double)newQuote.Close);
        
        EpmaResult finalResult = sut[^1];
        finalResult.Epma.Should().NotBeNull();
        
        // Verify the new result is numerically valid and reasonable
        // (should be different from previous but within reasonable bounds)
        finalResult.Epma.Should().NotBe(lastEpma);
        
        // Verify values are within reasonable ranges (not NaN, infinity, or extreme values)
        finalResult.Epma.Should().BeInRange(100, 300);
        
        // Add one more quote to verify continuous operation
        Quote anotherQuote = Quotes[1];
        sut.Add(anotherQuote.Timestamp.AddDays(1201), (double)anotherQuote.Close);
        
        EpmaResult nextResult = sut[^1];
        nextResult.Epma.Should().NotBeNull();
        nextResult.Epma.Should().BeInRange(100, 300);
    }
}
