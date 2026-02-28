namespace BufferLists;

[TestClass]
public class Stoch : BufferListTestBase
{
    private const int lookbackPeriods = 14;
    private const int signalPeriods = 3;
    private const int smoothPeriods = 3;
    private const double kFactor = 3;
    private const double dFactor = 2;
    private const MaType movingAverageType = MaType.SMA;

    private static readonly IReadOnlyList<StochResult> series
       = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<StochResult> expected = subset.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void ExtendedParameters()
    {
        const double kFactor = 5;
        const double dFactor = 4;
        const MaType movingAverageType = MaType.SMMA;

        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType) { Quotes };

        IReadOnlyList<StochResult> expected = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        StochList sut = new(14, 3, 3, 1, 1, MaType.SMA, Quotes);
        sut.IsBetween(static x => x.Oscillator, 0, 100);
        sut.IsBetween(static x => x.Signal, 0, 100);
    }

    [TestMethod]
    public void IncrementalConsistency()
    {
        // Test that incremental addition produces same sut as batch
        StochList incremental = new(lookbackPeriods, signalPeriods, smoothPeriods);
        StochList batch = new(lookbackPeriods, signalPeriods, smoothPeriods) { Quotes };

        foreach (Quote quote in Quotes)
        {
            incremental.Add(quote);
        }

        incremental.Should().HaveCount(batch.Count);
        incremental.IsExactly(batch);
    }

    [TestMethod]
    public void ParameterValidation()
    {
        // Test parameter validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(static () => new StochList(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(static () => new StochList(14, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(static () => new StochList(14, 3, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(static () => new StochList(14, 3, 3, 0, 2, MaType.SMA));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(static () => new StochList(14, 3, 3, 3, 0, MaType.SMA));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(static () => new StochList(14, 3, 3, 3, 2, MaType.ALMA));
    }

    [TestMethod]
    public void BoundaryConditions()
    {
        // Test with minimal data
        StochList sut = new(5, 3, 3);
        List<Quote> minimal = Quotes.Take(5).ToList();

        foreach (Quote quote in minimal)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(minimal.Count);

        // Should have null values for initial periods
        for (int i = 0; i < 4; i++)
        {
            sut[i].Oscillator.Should().BeNull();
        }
    }

    [TestMethod]
    public void BufferListExtension()
    {
        // Test extension method
        StochList fromExtension = Quotes.ToStochList(lookbackPeriods, signalPeriods, smoothPeriods);
        StochList fromConstructor = new(lookbackPeriods, signalPeriods, smoothPeriods) { Quotes };

        fromExtension.IsExactly(fromConstructor);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods) {
            MaxListSize = maxListSize
        };

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        IReadOnlyList<StochResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
