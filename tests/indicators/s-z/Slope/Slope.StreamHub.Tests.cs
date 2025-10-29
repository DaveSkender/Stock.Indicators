namespace StreamHub;

[TestClass]
public class SlopeHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;
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

        // late arrival, should equal series (excluding Line values which are not maintained in streaming)
        quoteHub.Insert(Quotes[80]);
        actuals.Should().BeEquivalentTo(expectedOriginal, static options => options
            .Excluding(x => x.Line)
            .WithStrictOrdering());

        // delete, should equal series (revised, excluding Line)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<SlopeResult> expectedRevised = RevisedQuotes.ToSlope(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options
            .Excluding(x => x.Line)
            .WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SlopeHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToSlopeHub(lookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<SlopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SlopeResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToSlope(lookbackPeriods);

        // assert, should equal series (excluding Line)
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, static options => options
            .Excluding(x => x.Line)
            .WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToSlopeHub(lookbackPeriods)
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
            .ToSlope(lookbackPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.Should().BeEquivalentTo(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Standard()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SlopeHub observer = quoteHub.ToSlopeHub(lookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // final results
        IReadOnlyList<SlopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SlopeResult> expected = Quotes.ToSlope(lookbackPeriods);

        // assert, should equal series (excluding Line)
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, static options => options
            .Excluding(x => x.Line)
            .WithStrictOrdering());

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
