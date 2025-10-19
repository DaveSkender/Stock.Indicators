namespace BufferLists;

[TestClass]
public class WilliamsR : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<WilliamsResult> series
       = Quotes.ToWilliamsR(lookbackPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        WilliamsRList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        WilliamsRList sut = new(lookbackPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        WilliamsRList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        WilliamsRList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<WilliamsResult> expected = subset.ToWilliamsR(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void IncrementalConsistency()
    {
        // Test that incremental addition produces same results as batch
        WilliamsRList incremental = new(lookbackPeriods);
        WilliamsRList batch = new(lookbackPeriods) { Quotes };

        foreach (Quote quote in Quotes)
        {
            incremental.Add(quote);
        }

        incremental.Should().HaveCount(batch.Count);
        incremental.Should().BeEquivalentTo(batch, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ParameterValidation()
    {
        // Test parameter validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new WilliamsRList(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new WilliamsRList(-1));
    }

    [TestMethod]
    public void BoundaryConditions()
    {
        // Test with minimal data
        WilliamsRList sut = new(5);
        List<Quote> minimal = Quotes.Take(5).ToList();

        foreach (Quote quote in minimal)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(minimal.Count);

        // Should have null values for initial periods
        for (int i = 0; i < 4; i++)
        {
            sut[i].WilliamsR.Should().BeNull();
        }

        // Should have a value at the lookback period boundary
        sut[4].WilliamsR.Should().NotBeNull();
    }

    [TestMethod]
    public void BufferListExtension()
    {
        // Test extension method
        WilliamsRList fromExtension = Quotes.ToWilliamsRList(lookbackPeriods);
        WilliamsRList fromConstructor = new(lookbackPeriods) { Quotes };

        fromExtension.Should().HaveCount(fromConstructor.Count);
        fromExtension.Should().BeEquivalentTo(fromConstructor, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        WilliamsRList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        IReadOnlyList<WilliamsResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void BoundaryValues()
    {
        // Test that Williams %R stays within expected range [-100, 0]
        WilliamsRList sut = new(lookbackPeriods) { Quotes };

        foreach (WilliamsResult result in sut)
        {
            if (result.WilliamsR.HasValue)
            {
                result.WilliamsR.Value.Should().BeInRange(-100d, 0d);
            }
        }
    }
}
