namespace StreamHub;

[TestClass]
public class SlopeHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private readonly IReadOnlyList<SlopeResult> expectedOriginal = Quotes.ToSlope(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        SlopeHub observer = quoteHub.ToSlopeHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<SlopeResult> actuals = observer.Results;

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

        // Compare - exclude Line property as it differs in streaming vs batch
        actuals.Should().HaveCount(expectedOriginal.Count);
        for (int i = 0; i < actuals.Count; i++)
        {
            SlopeResult actual = actuals[i];
            SlopeResult expected = expectedOriginal[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Slope.Should().BeApproximately(expected.Slope, 0.00001);
            actual.Intercept.Should().BeApproximately(expected.Intercept, 0.00001);
            actual.StdDev.Should().BeApproximately(expected.StdDev, 0.00001);
            actual.RSquared.Should().BeApproximately(expected.RSquared, 0.00001);
            // Note: Line values differ between streaming and batch (batch fills last window)
        }

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<SlopeResult> expectedRevised = RevisedQuotes.ToSlope(lookbackPeriods);

        actuals.Should().HaveCount(501);

        // Compare revised results - exclude Line property
        for (int i = 0; i < actuals.Count; i++)
        {
            SlopeResult actual = actuals[i];
            SlopeResult expected = expectedRevised[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Slope.Should().BeApproximately(expected.Slope, 0.00001);
            actual.Intercept.Should().BeApproximately(expected.Intercept, 0.00001);
            actual.StdDev.Should().BeApproximately(expected.StdDev, 0.00001);
            actual.RSquared.Should().BeApproximately(expected.RSquared, 0.00001);
        }

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int slopePeriods = 14;
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SlopeHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToSlopeHub(slopePeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<SlopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SlopeResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToSlope(slopePeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length);

        // Compare - exclude Line property
        for (int i = 0; i < actuals.Count; i++)
        {
            SlopeResult actual = actuals[i];
            SlopeResult exp = expected[i];

            actual.Timestamp.Should().Be(exp.Timestamp);
            actual.Slope.Should().BeApproximately(exp.Slope, 0.00001);
            actual.Intercept.Should().BeApproximately(exp.Intercept, 0.00001);
            actual.StdDev.Should().BeApproximately(exp.StdDev, 0.00001);
            actual.RSquared.Should().BeApproximately(exp.RSquared, 0.00001);
        }

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int slopePeriods = 20;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToSlopeHub(slopePeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<SmaResult> actuals = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = RevisedQuotes
            .ToSlope(slopePeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.Should().BeEquivalentTo(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        SlopeHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("SLOPE(14)");
    }
}
