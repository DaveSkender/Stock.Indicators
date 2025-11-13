namespace StreamHub;

[TestClass]
public class ChopHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 25; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        ChopHub observer = quoteHub
            .ToChopHub(14);

        // fetch initial results (early)
        IReadOnlyList<ChopResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 25; i < length; i++)
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
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<ChopResult> seriesList = quotesList.ToChop(14);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 8;
        const int chopPeriods = 14;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToChopHub(chopPeriods)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .ToChop(chopPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        ChopHub observer = quoteHub.ToChopHub(14);

        observer.ToString().Should().Be("CHOP(14)");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
