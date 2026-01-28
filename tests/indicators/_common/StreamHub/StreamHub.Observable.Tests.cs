namespace Observables;

[TestClass]
public class StreamObservables : TestBase, ITestChainProvider
{
    [TestMethod]
    public void Prefill()
    {
        IReadOnlyList<Quote> quotesList = Quotes
            .Take(50)
            .ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(quotesList);

        // initialize observer
        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        // assert: prefilled
        quoteHub.Cache.Should().HaveCount(50);
        observer.Cache.Should().HaveCount(50);

        // assert: same dates
        for (int i = 0; i < 50; i++)
        {
            IReusable r = observer.Cache[i];
            IReusable q = quoteHub.Cache[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Subscription()
    {
        // setup quote provider hub, observer
        QuoteHub quoteHub = new();

        QuotePartHub observer
            = quoteHub.ToQuotePartHub(CandlePart.OHLC4);

        // assert: subscribed
        quoteHub.ObserverCount.Should().Be(1);
        quoteHub.HasObservers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: unsubscribe
        observer.Unsubscribe();

        // assert: not subscribed
        quoteHub.ObserverCount.Should().Be(0);
        quoteHub.HasObservers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();

        // act: resubscribe
        quoteHub.Subscribe(observer);

        // assert: subscribed
        quoteHub.ObserverCount.Should().Be(1);
        quoteHub.HasObservers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: end all subscriptions
        quoteHub.EndTransmission();

        // assert: not subscribed
        quoteHub.ObserverCount.Should().Be(0);
        quoteHub.HasObservers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToQuotePartHub(CandlePart.HL2)
            .ToEmaHub(11);

        // emulate adding quotes to provider hub
        quoteHub.Add(Quotes);
        quoteHub.EndTransmission();

        // stream results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = Quotes
            .Use(CandlePart.HL2)
            .ToEma(11);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
