namespace StreamHub;

[TestClass]
public class AwesomeHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 40; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        AwesomeHub<Quote> observer = provider
            .ToAwesome(5, 34);

        // fetch initial results (early)
        IReadOnlyList<AwesomeResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 40; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<AwesomeResult> seriesList = quotesList.ToAwesome(5, 34);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int emaPeriods = 12;
        int fastPeriods = 5;
        int slowPeriods = 34;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        AwesomeHub<EmaResult> observer = provider
            .ToEma(emaPeriods)
            .ToAwesome(fastPeriods, slowPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<AwesomeResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<AwesomeResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToAwesome(fastPeriods, slowPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int fastPeriods = 5;
        int slowPeriods = 34;
        int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<AwesomeResult> observer = provider
            .ToAwesome(fastPeriods, slowPeriods)
            .ToEma(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToAwesome(fastPeriods, slowPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        AwesomeHub<Quote> observer = provider.ToAwesome(5, 34);

        observer.ToString().Should().Be("AWESOME(5,34)");

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
