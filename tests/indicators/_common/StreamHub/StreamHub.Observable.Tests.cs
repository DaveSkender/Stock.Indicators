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

    [TestMethod]
    public void StandaloneQuoteHub_CannotBeSubscribedToProvider()
    {
        // create two standalone quote hubs
        QuoteHub standaloneObserver = new();
        QuoteHub provider = new();

        // attempting to subscribe a standalone hub to another provider should throw
        Action act = () => provider.Subscribe(standaloneObserver);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*standalone QuoteHub*");

        provider.EndTransmission();
    }

    [TestMethod]
    public void StandaloneTickHub_CannotBeSubscribedToProvider()
    {
        // create two standalone tick hubs
        TickHub standaloneObserver = new();
        TickHub provider = new();

        // attempting to subscribe a standalone hub to another provider should throw
        Action act = () => provider.Subscribe(standaloneObserver);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*standalone TickHub*");

        provider.EndTransmission();
    }

    [TestMethod]
    public void NonStandaloneQuoteHub_CanBeSubscribedAsObserver()
    {
        // create a standalone provider
        QuoteHub provider = new();

        // create a non-standalone observer using the constructor
        QuoteHub observer = new(provider);

        // assert: subscribed to provider
        provider.ObserverCount.Should().Be(1);
        observer.IsSubscribed.Should().BeTrue();

        // unsubscribe
        observer.Unsubscribe();
        provider.ObserverCount.Should().Be(0);
        observer.IsSubscribed.Should().BeFalse();

        // can manually resubscribe to the same provider
        provider.Subscribe(observer);

        // assert: subscribed again
        provider.ObserverCount.Should().Be(1);
        observer.IsSubscribed.Should().BeTrue();

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
