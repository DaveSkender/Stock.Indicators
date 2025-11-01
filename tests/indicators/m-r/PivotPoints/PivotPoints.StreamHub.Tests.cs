namespace StreamHub;

[TestClass]
public class PivotPointsHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(20));

        // initialize observer
        PivotPointsHub observer = quoteHub
            .ToPivotPointsHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<PivotPointsResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrivals
        quoteHub.Insert(quotesList[30]);  // rebuilds complete series
        quoteHub.Insert(quotesList[80]);  // rebuilds from insertion point

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<PivotPointsResult> seriesList
           = quotesList
            .ToPivotPoints();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithWeekly()
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
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithCamarilla()
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
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithDemark()
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
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithFibonacci()
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
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithWoodie()
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
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        PivotPointsHub hub1 = new(new QuoteHub(), PeriodSize.Month, PivotPointType.Standard);
        hub1.ToString().Should().Be("PIVOT-POINTS(Month,Standard)");

        PivotPointsHub hub2 = new(new QuoteHub(), PeriodSize.Week, PivotPointType.Camarilla);
        hub2.ToString().Should().Be("PIVOT-POINTS(Week,Camarilla)");
    }
}
