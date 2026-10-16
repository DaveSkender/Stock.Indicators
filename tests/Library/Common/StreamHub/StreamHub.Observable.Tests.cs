namespace Observables;

[TestClass]
public class StreamObservables : TestBase, ITestChainProvider
{
    [TestMethod]
    public void Prefill()
    {
        IReadOnlyList<Bar> barsList = Bars
            .Take(50)
            .ToList();

        int length = barsList.Count;

        // setup bar provider hub
        BarHub barHub = new();

        // prefill bars at provider
        barHub.Add(barsList);

        // initialize observer
        BarPartHub observer = barHub
            .ToBarPartHub(CandlePart.Close);

        // assert: prefilled
        barHub.Cache.Should().HaveCount(50);
        observer.Cache.Should().HaveCount(50);

        // assert: same dates
        for (int i = 0; i < 50; i++)
        {
            IReusable r = observer.Cache[i];
            IReusable q = barHub.Cache[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    public void Subscription()
    {
        // setup bar provider hub, observer
        BarHub barHub = new();

        BarPartHub observer
            = barHub.ToBarPartHub(CandlePart.OHLC4);

        // assert: subscribed
        barHub.ObserverCount.Should().Be(1);
        barHub.HasObservers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: unsubscribe
        observer.Unsubscribe();

        // assert: not subscribed
        barHub.ObserverCount.Should().Be(0);
        barHub.HasObservers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();

        // act: resubscribe
        barHub.Subscribe(observer);

        // assert: subscribed
        barHub.ObserverCount.Should().Be(1);
        barHub.HasObservers.Should().BeTrue();
        observer.IsSubscribed.Should().BeTrue();

        // act: end all subscriptions
        barHub.EndTransmission();

        // assert: not subscribed
        barHub.ObserverCount.Should().Be(0);
        barHub.HasObservers.Should().BeFalse();
        observer.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // setup bar provider hub
        BarHub barHub = new();

        // initialize observer
        EmaHub observer = barHub
            .ToBarPartHub(CandlePart.HL2)
            .ToEmaHub(11);

        // emulate adding bars to provider hub
        barHub.Add(Bars);
        barHub.EndTransmission();

        // stream results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = Bars
            .Use(CandlePart.HL2)
            .ToEma(11);

        // assert, should equal series
        streamList.Should().HaveCount(Bars.Count);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    public void EndTransmissionCompletesChainedHubs()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50).ToList());

        SmaHub sma = barHub.ToSmaHub(10);
        EmaHub ema = sma.ToEmaHub(5);
        EpmaHub epma = barHub.ToEpmaHub(10);
        MaEnvelopesHub env = barHub.ToMaEnvelopesHub(10, 2.5);

        barHub.EndTransmission();

        sma.IsSubscribed.Should().BeFalse();
        ema.IsSubscribed.Should().BeFalse();
        epma.IsSubscribed.Should().BeFalse();
        env.IsSubscribed.Should().BeFalse();
    }
}
