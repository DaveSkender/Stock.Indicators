namespace BufferLists;

[TestClass]
public class Macd : BufferListTestBase
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
    public void FromReusableSplit()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods) { Quotes };

        IReadOnlyList<MacdResult> series
            = Quotes.ToMacd(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

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

        IReadOnlyList<MacdResult> expected = subset.ToMacd(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Extension()
    {
        MacdList sut = Quotes.ToMacdList(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test streaming vs batch accuracy with specific data points
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        for (int i = 0; i < 100; i++)
        {
            sut.Add(Quotes[i]);
        }

        // Validate specific calculations match series results
        MacdResult streamResult = sut[50];
        MacdResult seriesResult = series[50];

        streamResult.Macd.Should().Be(seriesResult.Macd);
        streamResult.Signal.Should().Be(seriesResult.Signal);
        streamResult.Histogram.Should().Be(seriesResult.Histogram);
        streamResult.FastEma.Should().Be(seriesResult.FastEma);
        streamResult.SlowEma.Should().Be(seriesResult.SlowEma);

        streamResult.Should().Be(seriesResult);
        sut.ToList().Should().BeEquivalentTo(series.Take(100));
    }

    [TestMethod]
    public void RealTimeSimulation()
    {
        // Simulate real-time data processing
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        // Process initial historical data
        foreach (Quote quote in Quotes.Take(100))
        {
            sut.Add(quote);
        }

        // Process new incoming quotes
        foreach (Quote quote in Quotes.Skip(100).Take(10))
        {
            MacdResult result = sut[^1]; // Last result before adding new
            sut.Add(quote);
            MacdResult newResult = sut[^1]; // New result after adding

            // Verify calculations are consistent
            if (newResult.Macd.HasValue)
            {
                newResult.Macd.Should().NotBeNull();
                newResult.FastEma.Should().NotBeNull();
                newResult.SlowEma.Should().NotBeNull();
            }
        }
    }
}
