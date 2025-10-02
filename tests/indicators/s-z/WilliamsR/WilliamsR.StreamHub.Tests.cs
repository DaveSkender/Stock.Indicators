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

        // verify against static series calculation with tolerance
        IReadOnlyList<WilliamsResult> expected = Quotes.ToWilliamsR(14);
        observer.Cache.Should().HaveCount(expected.Count);

        for (int i = 0; i < observer.Cache.Count; i++)
        {
            WilliamsResult actual = observer.Cache[i];
            WilliamsResult exp = expected[i];

            actual.Timestamp.Should().Be(exp.Timestamp);

            if (exp.WilliamsR.HasValue && actual.WilliamsR.HasValue)
            {
                actual.WilliamsR.Should().BeApproximately(exp.WilliamsR.Value, 0.001);
            }
            else
            {
                actual.WilliamsR.Should().Be(exp.WilliamsR);
            }
        }
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

        // verify consistency with tolerance
        IReadOnlyList<WilliamsResult> expected = Quotes.ToWilliamsR(lookbackPeriods);
        observer.Cache.Should().HaveCount(expected.Count);

        for (int i = 0; i < observer.Cache.Count; i++)
        {
            WilliamsResult actual = observer.Cache[i];
            WilliamsResult exp = expected[i];

            actual.Timestamp.Should().Be(exp.Timestamp);

            if (exp.WilliamsR.HasValue && actual.WilliamsR.HasValue)
            {
                actual.WilliamsR.Should().BeApproximately(exp.WilliamsR.Value, 0.001);
            }
            else
            {
                actual.WilliamsR.Should().Be(exp.WilliamsR);
            }
        }
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

        // compare results with reasonable precision
        streamObserver.Cache.Should().HaveCount(batchResults.Count);

        for (int i = 0; i < streamObserver.Cache.Count; i++)
        {
            WilliamsResult streamResult = streamObserver.Cache[i];
            WilliamsResult batchResult = batchResults[i];

            streamResult.Timestamp.Should().Be(batchResult.Timestamp);

            if (streamResult.WilliamsR.HasValue && batchResult.WilliamsR.HasValue)
            {
                streamResult.WilliamsR.Should().BeApproximately(batchResult.WilliamsR.Value, 0.001);
            }
            else
            {
                streamResult.WilliamsR.Should().Be(batchResult.WilliamsR);
            }
        }
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
