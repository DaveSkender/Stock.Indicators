namespace StreamHubs;

[TestClass]
public class FractalHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        FractalHub observer = quoteHub
            .ToFractalHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<FractalResult> actuals
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrivals
        quoteHub.Insert(Quotes[30]);  // rebuilds complete series
        quoteHub.Insert(Quotes[80]);  // rebuilds from insertion point

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // Ensure all fractals are calculated with full context
        observer.Rebuild(0);

        // time-series, for comparison
        IReadOnlyList<FractalResult> expected = RevisedQuotes.ToFractal();

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyClose()
    {
        // simple test, just to check Close variant

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        FractalHub observer = quoteHub
            .ToFractalHub(endType: EndType.Close);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // Trigger rebuild to calculate all fractals with complete future context
        observer.Rebuild(0);

        // stream results
        IReadOnlyList<FractalResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<FractalResult> seriesList
           = Quotes
            .ToFractal(endType: EndType.Close);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyDifferentSpans()
    {
        // test with different left and right spans

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with different spans
        FractalHub observer = quoteHub
            .ToFractalHub(3, 4);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // Trigger rebuild to calculate all fractals with complete future context
        observer.Rebuild(0);

        // stream results
        IReadOnlyList<FractalResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<FractalResult> seriesList
           = Quotes
            .ToFractal(3, 4);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        FractalHub hub1 = new(new QuoteHub(), 2, EndType.HighLow);
        hub1.ToString().Should().Be("FRACTAL(2,2,HIGHLOW)");

        FractalHub hub2 = new(new QuoteHub(), 3, 4, EndType.Close);
        hub2.ToString().Should().Be("FRACTAL(3,4,CLOSE)");
    }
}
