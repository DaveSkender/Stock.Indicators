namespace StreamHubs;

[TestClass]
public class HubCollectionTests : TestBase
{
    [TestMethod]
    public void HubCollection_Ctor_Defaults()
    {
        // arrange - empty collection
        HubCollection emptyHubs = new();

        // assert
        emptyHubs.Should().NotBeNull();
        emptyHubs.Should().BeEmpty();
    }

    [TestMethod]
    public void HubCollection_InitializeAsCollection_MixedHubTypes()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaFastHub = quoteHub.ToEmaHub(50);
        EmaHub emaSlowHub = quoteHub.ToEmaHub(200);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);
        DonchianHub donchianHub = quoteHub.ToDonchianHub(20);

        // act - collection initializer syntax with mixed hub types
        HubCollection hubs =
        [
            emaFastHub,
            emaSlowHub,
            rsiHub,
            quoteHub,   // QuoteProvider type
            donchianHub // Non-reusable types
        ];

        // assert - verify hub references are the same (not copied)
        hubs.Should().NotBeNull();
        hubs.Should().HaveCount(5);
        quoteHub.ObserverCount.Should().Be(4);
        hubs[0].Should().BeSameAs(emaFastHub);
        hubs[1].Should().BeSameAs(emaSlowHub);
        hubs[2].Should().BeSameAs(rsiHub);
        hubs[3].Should().BeSameAs(quoteHub);
        hubs[4].Should().BeSameAs(donchianHub);
    }

    [TestMethod]
    public void HubCollection_UsingCtorWithEnumerable_HasHubRefs()
    {
        // arrange
        QuoteHub quoteHub = new();

        EmaHub emaHub = quoteHub.ToEmaHub(200);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);

        IEnumerable<IStreamObservable<IReusable>> hubEnumerable =
        [
            emaHub,
            rsiHub
        ];

        // act
        HubCollection hubs = new(hubEnumerable);

        // assert - verify hub references are the same (not copied)
        hubs.Should().HaveCount(2);
        quoteHub.ObserverCount.Should().Be(2);
        hubs[0].Should().BeSameAs(emaHub);
        hubs[1].Should().BeSameAs(rsiHub);
    }

    [TestMethod]
    public void HubCollection_UseCtorWithList_HasHubRefs()
    {
        // arrange
        QuoteHub quoteHub = new();

        List<IStreamObservable<IReusable>> hubList =
        [
            quoteHub.ToEmaHub(20),
            quoteHub.ToRsiHub(14)
        ];

        // act
        HubCollection hubs = new(hubList);

        // assert - verify hub references are the same (not copied)
        hubs.Should().HaveCount(2);
        quoteHub.ObserverCount.Should().Be(2);
        hubs[0].Should().BeSameAs(hubList[0]);
        hubs[1].Should().BeSameAs(hubList[1]);
    }

    [TestMethod]
    public void HubCollection_WithQuoteIncrements_HasCorrectResults()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);

        HubCollection hubs = [emaHub, rsiHub];

        // assert - initially empty
        emaHub.Results.Should().BeEmpty();
        rsiHub.Results.Should().BeEmpty();

        // act - add quotes AFTER collection creation
        foreach (Quote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // assert - verify exact same instance
        hubs[0].Should().BeSameAs(emaHub);
        hubs[1].Should().BeSameAs(rsiHub);
        quoteHub.ObserverCount.Should().Be(2);

        // assert - contain equivalent results
        emaHub.Results.Should().HaveCount(502);
        rsiHub.Results.Should().HaveCount(502);
        hubs[0].Results.Should().HaveCount(502);
        hubs[1].Results.Should().HaveCount(502);

        hubs[0].Results.Should().BeEquivalentTo(emaHub.Results);
        hubs[1].Results.Should().BeEquivalentTo(rsiHub.Results);
    }

    [TestMethod]
    public void HubCollection_ResultsSnapshot_ReturnsLiveView()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);

        HubCollection hubs = [emaHub, rsiHub];

        // act - add initial quotes
        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // act - store results snapshot
        List<IReadOnlyList<ISeries>> resultsSnapshot
            = hubs.Select(h => h.Results).ToList();

        // assert - initial results count
        int initialCount = resultsSnapshot[0].Count;
        initialCount.Should().Be(100);
        resultsSnapshot[0].Should().HaveCount(initialCount);
        resultsSnapshot[1].Should().HaveCount(initialCount);

        // act - add MORE quotes after getting results
        for (int i = 100; i < 200; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // assert - final results count updated in snapshot
        int finalCount = resultsSnapshot[0].Count;
        finalCount.Should().Be(200);
        resultsSnapshot[0].Should().HaveCount(finalCount);
        resultsSnapshot[1].Should().HaveCount(finalCount);

        // assert - last element is from the newly added quotes
        resultsSnapshot[0][^1].Timestamp.Should().Be(Quotes[199].Timestamp);

        // assert - provider shows correct observer count
        quoteHub.ObserverCount.Should().Be(2);
    }

    [TestMethod]
    public void HubCollection_Results_ReturnsAllHubResults()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);
        DonchianHub donchianHub = quoteHub.ToDonchianHub(20);

        HubCollection hubs = [emaHub, rsiHub, donchianHub];

        // act - add quotes
        foreach (Quote q in Quotes.Take(100))
        {
            quoteHub.Add(q);
        }

        // act - get results enumerable
        IEnumerable<IReadOnlyList<ISeries>> results = hubs.Results;

        // assert - results count matches hub count
        results.Should().HaveCount(3);

        // assert - each result set has correct count
        results.ElementAt(0).Should().HaveCount(100);
        results.ElementAt(1).Should().HaveCount(100);
        results.ElementAt(2).Should().HaveCount(100);

        // assert - results are live views (same reference)
        results.ElementAt(0).Should().BeSameAs(emaHub.Results);
        results.ElementAt(1).Should().BeSameAs(rsiHub.Results);
        results.ElementAt(2).Should().BeSameAs(donchianHub.Results);
    }

    [TestMethod]
    public void HubCollection_LastValues_ReturnsReusableValues()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);

        HubCollection hubs = [emaHub, rsiHub];

        // act - add quotes
        foreach (Quote q in Quotes.Take(100))
        {
            quoteHub.Add(q);
        }

        // act - get last values
        List<double> lastValues = hubs.LastValues.ToList();

        // assert - count matches hub count
        lastValues.Should().HaveCount(2);

        // assert - values match last result values
        lastValues[0].Should().Be(emaHub.Results[^1].Value);
        lastValues[1].Should().Be(rsiHub.Results[^1].Value);

        // assert - values are not NaN
        lastValues[0].Should().NotBe(double.NaN);
        lastValues[1].Should().NotBe(double.NaN);
    }

    [TestMethod]
    public void HubCollection_LastValues_WithNonReusable_ReturnsNaN()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        DonchianHub donchianHub = quoteHub.ToDonchianHub(20);  // Non-reusable result

        HubCollection hubs = [emaHub, donchianHub];

        // act - add quotes
        foreach (Quote q in Quotes.Take(100))
        {
            quoteHub.Add(q);
        }

        // act - get last values
        List<double> lastValues = hubs.LastValues.ToList();

        // assert - count matches hub count
        lastValues.Should().HaveCount(2);

        // assert - reusable hub has value
        lastValues[0].Should().Be(emaHub.Results[^1].Value);
        lastValues[0].Should().NotBe(double.NaN);

        // assert - non-reusable hub returns NaN
        lastValues[1].Should().Be(double.NaN);
    }

    [TestMethod]
    public void HubCollection_LastValues_WithEmptyResults_ReturnsNaN()
    {
        // arrange
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);

        HubCollection hubs = [emaHub, rsiHub];

        // act - get last values WITHOUT adding quotes
        List<double> lastValues = hubs.LastValues.ToList();

        // assert - count matches hub count
        lastValues.Should().HaveCount(2);

        // assert - empty results return NaN
        lastValues[0].Should().Be(double.NaN);
        lastValues[1].Should().Be(double.NaN);
    }
}
