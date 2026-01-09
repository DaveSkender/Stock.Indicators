namespace StreamHubs;

[TestClass]
public class PivotPointsHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<PivotPointsResult> actuals
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

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> expected = RevisedQuotes.ToPivotPoints();

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithWeekly()
    {
        // Test with weekly window size

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub(PeriodSize.Week);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<PivotPointsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> seriesList
           = Quotes
            .ToPivotPoints(PeriodSize.Week);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithCamarilla()
    {
        // Test with Camarilla pivot point type

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub(pointType: PivotPointType.Camarilla);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<PivotPointsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> seriesList
           = Quotes
            .ToPivotPoints(pointType: PivotPointType.Camarilla);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithDemark()
    {
        // Test with Demark pivot point type

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub(pointType: PivotPointType.Demark);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<PivotPointsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> seriesList
           = Quotes
            .ToPivotPoints(pointType: PivotPointType.Demark);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithFibonacci()
    {
        // Test with Fibonacci pivot point type

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub(pointType: PivotPointType.Fibonacci);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<PivotPointsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> seriesList
           = Quotes
            .ToPivotPoints(pointType: PivotPointType.Fibonacci);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithWoodie()
    {
        // Test with Woodie pivot point type

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub(pointType: PivotPointType.Woodie);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<PivotPointsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> seriesList
           = Quotes
            .ToPivotPoints(pointType: PivotPointType.Woodie);

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
        PivotPointsHub hub1 = new(new QuoteHub(), PeriodSize.Month, PivotPointType.Standard);
        hub1.ToString().Should().Be("PIVOT-POINTS(Month,Standard)");

        PivotPointsHub hub2 = new(new QuoteHub(), PeriodSize.Week, PivotPointType.Camarilla);
        hub2.ToString().Should().Be("PIVOT-POINTS(Week,Camarilla)");
    }
}
