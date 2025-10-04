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
}
