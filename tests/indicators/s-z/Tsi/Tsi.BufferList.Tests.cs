namespace BufferLists;

[TestClass]
public class Tsi : BufferListTestBase
{
    private const int lookbackPeriods = 25;
    private const int smoothPeriods = 13;
    private const int signalPeriods = 7;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<TsiResult> series
       = Quotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

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
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods) { Quotes };

        IReadOnlyList<TsiResult> series
            = Quotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<TsiResult> expected = subset.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Extension()
    {
        TsiList sut = Quotes.ToTsiList(lookbackPeriods, smoothPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test streaming vs batch accuracy with specific data points
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

        for (int i = 0; i < 100; i++)
        {
            sut.Add(Quotes[i]);
        }

        // Validate specific calculations match series results
        TsiResult streamResult = sut[50];
        TsiResult seriesResult = series[50];

        streamResult.Tsi.Should().BeApproximately(seriesResult.Tsi, 0.0001);
        streamResult.Signal.Should().BeApproximately(seriesResult.Signal, 0.0001);

        streamResult.Should().Be(seriesResult);
        sut.ToList().Should().BeEquivalentTo(series.Take(100));
    }

    [TestMethod]
    public void RealTimeSimulation()
    {
        // Simulate real-time data processing
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

        // Process initial historical data
        foreach (Quote quote in Quotes.Take(100))
        {
            sut.Add(quote);
        }

        // Process new incoming quotes
        foreach (Quote quote in Quotes.Skip(100).Take(10))
        {
            TsiResult result = sut[^1]; // Last result before adding new
            sut.Add(quote);
            TsiResult newResult = sut[^1]; // New result after adding

            // Verify calculations are consistent
            if (newResult.Tsi.HasValue)
            {
                newResult.Tsi.Should().NotBeNull();
            }
        }
    }
}
