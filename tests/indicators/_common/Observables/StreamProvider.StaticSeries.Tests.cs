namespace Observables;

[TestClass]
public class StreamProviders : TestBase, ITestChainProvider
{
    [TestMethod]
    public void Prefill()
    {
        IReadOnlyList<Quote> quotesList = Quotes
            .Take(50)
            .ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        provider.Add(quotesList);

        // initialize observer
        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        // assert: prefilled
        provider.Cache.Should().HaveCount(50);
        observer.Cache.Should().HaveCount(50);

        // assert: same dates
        for (int i = 0; i < 50; i++)
        {
            IReusable r = observer.Cache[i];
            IReusable q = provider.Cache[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Subscription()
    {
        // setup quote provider, observer
        QuoteHub<Quote> provider = new();

        QuotePartHub<Quote> observer
            = provider.ToQuotePart(CandlePart.OHLC4);

        // assert: subscribed
        provider.ObserverCount.Should().Be(1);
        provider.HasObservers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: unsubscribe
        observer.Unsubscribe();

        // assert: not subscribed
        provider.ObserverCount.Should().Be(0);
        provider.HasObservers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();

        // act: resubscribe
        provider.Subscribe(observer);

        // assert: subscribed
        provider.ObserverCount.Should().Be(1);
        provider.HasObservers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: end all subscriptions
        provider.EndTransmission();

        // assert: not subscribed
        provider.ObserverCount.Should().Be(0);
        provider.HasObservers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public void ChainProvider()
    {
        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<QuotePart> observer = provider
            .ToQuotePart(CandlePart.HL2)
            .ToEma(11);

        // emulate adding quotes to provider
        provider.Add(Quotes);
        provider.EndTransmission();

        // stream results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = Quotes
            .Use(CandlePart.HL2)
            .ToEma(11);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
