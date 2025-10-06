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
    public void FromQuotesCtor()
    {
        EpmaList sut = new(lookbackPeriods, Quotes);

        ValidateResults(sut, series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        EpmaList sut = new(lookbackPeriods, subset);

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
        const int maxListSize = 100;

        // Test that EPMA's result list auto-pruning (from base class)
        // works correctly alongside its internal cache pruning

        // Generate a continuous dataset to test pruning and calculation accuracy
        List<Quote> testQuotes = [];
        DateTime startDate = new(2020, 1, 1);

        // Create 1250 quotes for testing both pruning mechanisms
        for (int i = 0; i < 1250; i++)
        {
            Quote quote = Quotes[i % Quotes.Count];
            testQuotes.Add(new Quote
            {
                Timestamp = startDate.AddDays(i),
                Open = quote.Open,
                High = quote.High,
                Low = quote.Low,
                Close = quote.Close,
                Volume = quote.Volume
            });
        }

        // Calculate expected results for ALL quotes using static series
        // This gives us the baseline for comparison
        IReadOnlyList<EpmaResult> fullExpectedResults = testQuotes.ToEpma(lookbackPeriods);

        // Create buffer list with small MaxListSize for testing result list pruning
    EpmaList sut = new(lookbackPeriods) { MaxListSize = maxListSize };

        // Add first 1200 quotes to trigger both:
        // 1. Internal cache pruning (happens at 1000 items)
        // 2. Result list pruning (happens at MaxListSize = 100)
        for (int i = 0; i < 1200; i++)
        {
            sut.Add(testQuotes[i]);
        }

    // Verify result list was pruned to stay at MaxListSize
    sut.Count.Should().Be(maxListSize);

        // Add the next 50 quotes after pruning and verify accuracy
        List<EpmaResult> actualFinalResults = [];
        for (int i = 1200; i < 1250; i++)
        {
            sut.Add(testQuotes[i]);
            actualFinalResults.Add(sut[^1]);
        }

        // Compare the final 50 results against static series calculations
        // This ensures pruning didn't affect mathematical accuracy
        actualFinalResults.Count.Should().Be(50);

        for (int i = 0; i < actualFinalResults.Count; i++)
        {
            EpmaResult actual = actualFinalResults[i];
            EpmaResult expected = fullExpectedResults[1200 + i];

            actual.Timestamp.Should().Be(expected.Timestamp);

            if (expected.Epma.HasValue)
            {
                actual.Epma.Should().NotBeNull();
                actual.Epma.Should().BeApproximately(expected.Epma!.Value, 0.0001);
            }
            else
            {
                actual.Epma.Should().BeNull();
            }
        }
    }
}
