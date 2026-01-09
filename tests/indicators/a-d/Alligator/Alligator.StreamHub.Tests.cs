namespace StreamHubs;

[TestClass]
public class AlligatorHubTests : StreamHubTestBase, ITestChainObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        AlligatorHub observer = quoteHub.ToAlligatorHub();

        // fetch initial results (early)
        IReadOnlyList<AlligatorResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<AlligatorResult> expectedOriginal = Quotes.ToAlligator();
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<AlligatorResult> expectedRevised = RevisedQuotes.ToAlligator();
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        AlligatorHub observer = quoteHub
            .ToSmaHub(10)
            .ToAlligatorHub();

        // emulate adding quotes out of order
        // note: this works when graceful order
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
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

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<AlligatorResult> sut
            = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<AlligatorResult> expected
           = RevisedQuotes
            .ToSma(10)
            .ToAlligator();

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        AlligatorHub hub = new(new QuoteHub(), 13, 8, 7, 5, 4, 3);
        hub.ToString().Should().Be("ALLIGATOR(13,8,7,5,4,3)");
    }
}
