namespace StreamHub;

[TestClass]
public class AwesomeHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 40; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        AwesomeHub<Quote> awesomeHub = quoteHub
            .ToAwesomeHub(5, 34);

        // fetch initial results (early)
        IReadOnlyList<AwesomeResult> streamList
            = awesomeHub.Results;

        // emulate adding quotes to provider
        for (int i = 40; i < length; i++)
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
        IReadOnlyList<AwesomeResult> seriesList = quotesList.ToAwesome(5, 34);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        awesomeHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int emaPeriods = 12;
        const int fastPeriods = 5;
        const int slowPeriods = 34;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        AwesomeHub<EmaResult> awesomeHub = quoteHub
            .ToEmaHub(emaPeriods)
            .ToAwesomeHub(fastPeriods, slowPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<AwesomeResult> streamList
            = awesomeHub.Results;

        // time-series, for comparison
        IReadOnlyList<AwesomeResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToAwesome(fastPeriods, slowPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        awesomeHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int fastPeriods = 5;
        const int slowPeriods = 34;
        const int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        EmaHub<AwesomeResult> emaHub = quoteHub
            .ToAwesomeHub(fastPeriods, slowPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = emaHub.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToAwesome(fastPeriods, slowPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        AwesomeHub<Quote> awesomeHub = quoteHub.ToAwesomeHub(5, 34);

        awesomeHub.ToString().Should().Be("AWESOME(5,34)");

        awesomeHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
