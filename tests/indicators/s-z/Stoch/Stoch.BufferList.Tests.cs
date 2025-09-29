namespace BufferLists;

[TestClass]
public class Stoch : BufferListTestBase
{
    private const int lookbackPeriods = 14;
    private const int signalPeriods = 3;
    private const int smoothPeriods = 3;

    private static readonly IReadOnlyList<StochResult> series
       = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        StochList sut = new(lookbackPeriods, signalPeriods, smoothPeriods);

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

        IReadOnlyList<StochResult> expected = subset.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
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
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void IncrementalConsistency()
    {
        // Test that incremental addition produces same results as batch
        StochList incremental = new(lookbackPeriods, signalPeriods, smoothPeriods);
        StochList batch = new(lookbackPeriods, signalPeriods, smoothPeriods) { Quotes };

        foreach (Quote quote in Quotes)
        {
            incremental.Add(quote);
        }

        incremental.Should().HaveCount(batch.Count);

        // Compare specific values to ensure accuracy
        for (int i = 0; i < incremental.Count; i++)
        {
            StochResult inc = incremental[i];
            StochResult bat = batch[i];

            inc.Timestamp.Should().Be(bat.Timestamp);

            if (inc.Oscillator.HasValue && bat.Oscillator.HasValue)
            {
                inc.Oscillator.Should().BeApproximately(bat.Oscillator.Value, 0.0001);
            }
            else
            {
                inc.Oscillator.Should().Be(bat.Oscillator);
            }

            if (inc.Signal.HasValue && bat.Signal.HasValue)
            {
                inc.Signal.Should().BeApproximately(bat.Signal.Value, 0.0001);
            }
            else
            {
                inc.Signal.Should().Be(bat.Signal);
            }

            if (inc.PercentJ.HasValue && bat.PercentJ.HasValue)
            {
                inc.PercentJ.Should().BeApproximately(bat.PercentJ.Value, 0.0001);
            }
            else
            {
                inc.PercentJ.Should().Be(bat.PercentJ);
            }
        }
    }

    [TestMethod]
    public void ParameterValidation()
    {
        // Test parameter validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StochList(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StochList(14, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StochList(14, 3, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StochList(14, 3, 3, 0, 2, MaType.SMA));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StochList(14, 3, 3, 3, 0, MaType.SMA));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StochList(14, 3, 3, 3, 2, MaType.ALMA));
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
        StochList fromExtension = Quotes.ToStochBufferList(lookbackPeriods, signalPeriods, smoothPeriods);
        StochList fromConstructor = new(lookbackPeriods, signalPeriods, smoothPeriods) { Quotes };

        fromExtension.Should().BeEquivalentTo(fromConstructor);
    }
}
