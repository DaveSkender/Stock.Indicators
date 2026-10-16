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

        CompletionSpy emaSpy = new();
        CompletionSpy epmaSpy = new();
        ema.Subscribe(emaSpy);
        epma.Subscribe(epmaSpy);

        barHub.EndTransmission();
        barHub.EndTransmission();

        foreach (bool subscribed in new[] { sma.IsSubscribed, ema.IsSubscribed, epma.IsSubscribed, env.IsSubscribed })
        {
            subscribed.Should().BeFalse();
        }

        foreach (int count in new[] { barHub.ObserverCount, sma.ObserverCount, ema.ObserverCount, epma.ObserverCount })
        {
            count.Should().Be(0);
        }

        emaSpy.Completed.Should().Be(1);
        epmaSpy.Completed.Should().Be(1);
    }

    [TestMethod]
    public void EndTransmissionIsolatesFaultingObserver()
    {
        BarHub barHub = new();
        SmaHub sma = barHub.ToSmaHub(10);
        EmaHub ema = sma.ToEmaHub(5);
        CompletionSpy emaSpy = new();
        ema.Subscribe(emaSpy);

        barHub.Subscribe(new CompletionSpy { Throws = true });

        Action end = barHub.EndTransmission;

        end.Should().NotThrow();
        emaSpy.Completed.Should().Be(1);
    }

    private sealed class CompletionSpy : IStreamObserver<IReusable>
    {
        public int Completed { get; private set; }
        public bool Throws { get; init; }
        public bool IsSubscribed => true;
        public void Unsubscribe() { }
        public void OnAdd(IReusable item, bool notify, int? indexHint) { }
        public void OnRebuild(DateTime fromTimestamp) { }
        public void OnPrune(DateTime toTimestamp) { }
        public void OnError(Exception exception) { }

        public void OnCompleted()
        {
            Completed++;
            if (Throws)
            {
                throw new InvalidOperationException("boom");
            }
        }
    }
}
