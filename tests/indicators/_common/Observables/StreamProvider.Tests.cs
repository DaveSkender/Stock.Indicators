namespace Observables;

[TestClass]
public class ProviderTests : TestBase, ITestChainProvider
{
    [TestMethod]
    public void Prefill()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(50)
            .ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

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
        provider.SubscriberCount.Should().Be(1);
        provider.HasSubscribers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: unsubscribe
        observer.Unsubscribe();

        // assert: not subscribed
        provider.SubscriberCount.Should().Be(0);
        provider.HasSubscribers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();

        // act: resubscribe
        provider.Subscribe(observer);

        // assert: subscribed
        provider.SubscriberCount.Should().Be(1);
        provider.HasSubscribers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: end all subscriptions
        provider.EndTransmission();

        // assert: not subscribed
        provider.SubscriberCount.Should().Be(0);
        provider.HasSubscribers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public void ChainProvider()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<QuotePart> observer = provider
            .ToQuotePart(CandlePart.HL2)
            .ToEma(11);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // stream results
        IReadOnlyList<EmaResult> streamEma
            = observer.Results;

        // time-series, for comparison
        List<EmaResult> staticEma = Quotes
            .Use(CandlePart.HL2)
            .GetEma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            EmaResult s = staticEma[i];
            EmaResult r = streamEma[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Ema.Should().Be(s.Ema);
            r.Should().Be(s);
        }

        // confirm public interface
        Assert.AreEqual(observer.Cache.Count, observer.Results.Count);

        // confirm same length as provider cache
        Assert.AreEqual(observer.Cache.Count, provider.Quotes.Count);
    }
}
