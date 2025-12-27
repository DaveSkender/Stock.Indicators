namespace StreamHub;

[TestClass]
public class SlopeHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private readonly IReadOnlyList<SlopeResult> expectedOriginal = Quotes.ToSlope(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
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
        actuals.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<SlopeResult> expectedRevised = RevisedQuotes.ToSlope(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
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
        actuals.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
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
        IReadOnlyList<SmaResult> actuals
            = observer.Results;

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
    public override void ToStringOverride_ReturnsExpectedName()
    {
        SlopeHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("SLOPE(14)");
    }

    [TestMethod]
    public void Standard()
    {
        // Arrange
        QuoteHub quoteHub = new();
        SlopeHub sut = quoteHub.ToSlopeHub(lookbackPeriods);

        // Act
        foreach (Quote q in Quotes)
        {
            quoteHub.Add(q);
        }

        IReadOnlyList<SlopeResult> results = sut.Results;

        // Assert
        results.Should().HaveCount(Quotes.Count);
        results.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void VerifyLineRepaintBehavior()
    {
        // This test verifies that Line values are recalculated for the last lookbackPeriods results
        // as new data arrives, matching the Series implementation's behavior

        QuoteHub quoteHub = new();
        SlopeHub sut = quoteHub.ToSlopeHub(lookbackPeriods);

        // Add initial batch
        foreach (Quote q in Quotes.Take(lookbackPeriods + 5))
        {
            quoteHub.Add(q);
        }

        // Get Line value from a position within the window
        int midIndex = lookbackPeriods + 2;
        decimal? lineBefore = sut.Results[midIndex].Line;

        // Add one more value - this should update Line values for the last lookbackPeriods results
        quoteHub.Add(Quotes[lookbackPeriods + 5]);

        decimal? lineAfter = sut.Results[midIndex].Line;

        // The Line value may have changed because it's recalculated using the new slope/intercept
        // Verify final result matches series implementation
        List<Quote> expectedBatch = Quotes.Take(lookbackPeriods + 6).ToList();
        IReadOnlyList<SlopeResult> expected = expectedBatch.ToSlope(lookbackPeriods);

        sut.Results.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void InsufficientData()
    {
        // Arrange
        QuoteHub quoteHub = new();
        SlopeHub sut = quoteHub.ToSlopeHub(lookbackPeriods);

        // Act - Add fewer quotes than lookback period
        foreach (Quote q in Quotes.Take(lookbackPeriods - 1))
        {
            quoteHub.Add(q);
        }

        IReadOnlyList<SlopeResult> results = sut.Results;

        // Assert - All results should have null values
        results.Should().HaveCount(lookbackPeriods - 1);
        results.Should().AllSatisfy(
            r => {
                r.Slope.Should().BeNull();
                r.Intercept.Should().BeNull();
                r.StdDev.Should().BeNull();
                r.RSquared.Should().BeNull();
                r.Line.Should().BeNull();
            });

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
