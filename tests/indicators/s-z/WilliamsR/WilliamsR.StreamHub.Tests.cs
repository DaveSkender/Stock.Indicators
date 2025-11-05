namespace StreamHub;

[TestClass]
public class WilliamsR : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = Quotes.Count;

        // setup quote provider hub and observer BEFORE adding data
        QuoteHub quoteHub = new();
        WilliamsRHub observer = quoteHub.ToWilliamsRHub(14);

        // add base quotes (batch)
        quoteHub.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);
        }

        // close observations
        quoteHub.EndTransmission();

        // assert results
        observer.Cache.Should().HaveCount(length);

        // verify against static series calculation
        IReadOnlyList<WilliamsResult> expected = Quotes.ToWilliamsR(14);
        observer.Cache.Should().HaveCount(expected.Count);
        observer.Cache.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public override void CustomToString()
    {
        WilliamsRHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("WILLR(14)");
    }

    [TestMethod]
    public void IncrementalUpdates()
    {
        const int lookbackPeriods = 14;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub with incremental updates
        QuoteHub quoteHub = new();
        WilliamsRHub observer = quoteHub.ToWilliamsRHub(lookbackPeriods);

        // add quotes one by one
        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        // close observations
        quoteHub.EndTransmission();

        // verify consistency
        IReadOnlyList<WilliamsResult> expected = Quotes.ToWilliamsR(lookbackPeriods);
        observer.Cache.Should().HaveCount(expected.Count);
        observer.Cache.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Properties()
    {
        const int lookbackPeriods = 21;

        QuoteHub quoteHub = new();
        WilliamsRHub observer = quoteHub.ToWilliamsRHub(lookbackPeriods);

        // verify properties
        observer.LookbackPeriods.Should().Be(lookbackPeriods);
        observer.ToString().Should().Be($"WILLR({lookbackPeriods})");
    }

    [TestMethod]
    public void DefaultParameters()
    {
        QuoteHub quoteHub = new();
        WilliamsRHub observer = quoteHub.ToWilliamsRHub();

        // verify default properties
        observer.LookbackPeriods.Should().Be(14);
        observer.ToString().Should().Be("WILLR(14)");
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test that streaming produces accurate results compared to batch processing
        const int lookbackPeriods = 14;

        List<Quote> quotesList = Quotes.ToList();

        // streaming calculation
        QuoteHub quoteHub = new();
        WilliamsRHub streamObserver = quoteHub.ToWilliamsRHub(lookbackPeriods);

        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        quoteHub.EndTransmission();

        // batch calculation
        IReadOnlyList<WilliamsResult> batchResults = Quotes.ToWilliamsR(lookbackPeriods);

        // compare results
        streamObserver.Cache.Should().HaveCount(batchResults.Count);
        streamObserver.Cache.Should().BeEquivalentTo(batchResults);
    }

    [TestMethod]
    public void BoundaryValues()
    {
        // Test Williams %R stays within [-100, 0] bounds
        QuoteHub quoteHub = new();
        WilliamsRHub observer = quoteHub.ToWilliamsRHub(14);

        quoteHub.Add(Quotes);
        quoteHub.EndTransmission();

        foreach (WilliamsResult result in observer.Cache)
        {
            if (result.WilliamsR.HasValue)
            {
                result.WilliamsR.Value.Should().BeInRange(-100d, 0d);
            }
        }
    }

    [TestMethod]
    public void ParameterValidation()
    {
        QuoteHub quoteHub = new();

        // Test parameter validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => quoteHub.ToWilliamsRHub(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => quoteHub.ToWilliamsRHub(-1));
    }
}
