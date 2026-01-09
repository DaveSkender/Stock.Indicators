namespace StreamHubs;

[TestClass]
public class WilliamsRHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<WilliamsResult> sut = Quotes.ToWilliamsRHub(14).Results;
        sut.IsBetween(static x => x.WilliamsR, -100, 0);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        const int lookbackPeriods = 14;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        WilliamsRHub observer = quoteHub.ToWilliamsRHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<WilliamsResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);

        IReadOnlyList<WilliamsResult> expectedOriginal = Quotes.ToWilliamsR(lookbackPeriods);

        actuals.Should().HaveCount(length);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<WilliamsResult> expectedRevised = RevisedQuotes.ToWilliamsR(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
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
        observer.Cache.IsExactly(expected);
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
        streamObserver.Cache.IsExactly(batchResults);
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
