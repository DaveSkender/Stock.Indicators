namespace StreamHub;

[TestClass]
public class WilliamsR : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = Quotes.Count;

        // setup quote provider and observer BEFORE adding data
        QuoteHub<Quote> provider = new();
        WilliamsRHub<Quote> observer = provider.ToWilliamsR(14);

        // add base quotes (batch)
        provider.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // close observations
        provider.EndTransmission();

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
        WilliamsRHub<Quote> hub = new(new QuoteHub<Quote>(), 14);
        hub.ToString().Should().Be("WILLR(14)");
    }

    [TestMethod]
    public void IncrementalUpdates()
    {
        const int lookbackPeriods = 14;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider with incremental updates
        QuoteHub<Quote> provider = new();
        WilliamsRHub<Quote> observer = provider.ToWilliamsR(lookbackPeriods);

        // add quotes one by one
        foreach (Quote quote in quotesList)
        {
            provider.Add(quote);
        }

        // close observations
        provider.EndTransmission();

        // verify consistency
        IReadOnlyList<WilliamsResult> expected = Quotes.ToWilliamsR(lookbackPeriods);
        observer.Cache.Should().HaveCount(expected.Count);
        observer.Cache.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Properties()
    {
        const int lookbackPeriods = 21;

        QuoteHub<Quote> provider = new();
        WilliamsRHub<Quote> observer = provider.ToWilliamsR(lookbackPeriods);

        // verify properties
        observer.LookbackPeriods.Should().Be(lookbackPeriods);
        observer.ToString().Should().Be($"WILLR({lookbackPeriods})");
    }

    [TestMethod]
    public void DefaultParameters()
    {
        QuoteHub<Quote> provider = new();
        WilliamsRHub<Quote> observer = provider.ToWilliamsR();

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
        QuoteHub<Quote> provider = new();
        WilliamsRHub<Quote> streamObserver = provider.ToWilliamsR(lookbackPeriods);

        foreach (Quote quote in quotesList)
        {
            provider.Add(quote);
        }

        provider.EndTransmission();

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
        QuoteHub<Quote> provider = new();
        WilliamsRHub<Quote> observer = provider.ToWilliamsR(14);

        provider.Add(Quotes);
        provider.EndTransmission();

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
        QuoteHub<Quote> provider = new();

        // Test parameter validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => provider.ToWilliamsR(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => provider.ToWilliamsR(-1));
    }
}
