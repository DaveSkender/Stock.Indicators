namespace StreamHubs;

[TestClass]
public class MaEnvelopesHubTests : StreamHubTestBase, ITestChainObserver
{
    private const int lookbackPeriods = 20;
    private const double percentOffset = 2.5;
    private readonly IReadOnlyList<MaEnvelopeResult> expectedOriginal = Bars.ToMaEnvelopes(lookbackPeriods, percentOffset);

    [TestMethod]
    public void BarObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Bars.Count;

        // setup bar provider hub
        BarHub barHub = new();

        // prefill bars at provider
        barHub.Add(Bars.Take(20));

        // initialize observer
        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset);

        // fetch initial results (early)
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

        // emulate adding bars to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Bar q = Bars[i];
            barHub.Add(q);

            // resend duplicate bars
            if (i is > 100 and < 105) { barHub.Add(q); }
        }

        // late arrival, should equal series
        barHub.Add(Bars[80]);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        barHub.RemoveAt(removeAtIndex);

        IReadOnlyList<MaEnvelopeResult> expectedRevised = RevisedBars.ToMaEnvelopes(lookbackPeriods, percentOffset);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    public void WithCachePruning_MatchesSeriesExactly()
    {
        const int maxCacheSize = 50;
        const int totalBars = 100;

        IReadOnlyList<Bar> bars = Bars.Take(totalBars).ToList();
        IReadOnlyList<MaEnvelopeResult> expected = bars
            .ToMaEnvelopes(lookbackPeriods, percentOffset)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        BarHub barHub = new(maxCacheSize);
        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset);

        // Stream more bars than cache can hold
        barHub.Add(bars);

        // Verify cache was pruned
        barHub.Bars.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // Streaming results should match last N from full series (original series with front chopped off)
        // NOT recomputation on just the cached bars (which would have different warmup)
        observer.Results.IsExactly(expected);

        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 12;
        const int maPeriods = 20;
        int length = Bars.Count;

        // setup bar provider hub
        BarHub barHub = new();

        // initialize observer
        MaEnvelopesHub observer = barHub
            .ToSmaHub(smaPeriods)
            .ToMaEnvelopesHub(maPeriods, percentOffset);

        // emulate bar stream
        for (int i = 0; i < length; i++) { barHub.Add(Bars[i]); }

        // final results
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MaEnvelopeResult> expected = Bars
            .ToSma(smaPeriods)
            .ToMaEnvelopes(maPeriods, percentOffset);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    public void WithDemaType_AsMaType_MatchesSeriesExactly()
    {
        int length = Bars.Count;

        // setup bar provider hub
        BarHub barHub = new();

        // initialize observer with DEMA type
        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, MaType.DEMA);

        // emulate bar stream
        for (int i = 0; i < length; i++) { barHub.Add(Bars[i]); }

        // final results
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MaEnvelopeResult> expected = Bars.ToMaEnvelopes(lookbackPeriods, percentOffset, MaType.DEMA);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    [DataRow(MaType.ALMA)]
    [DataRow(MaType.DEMA)]
    [DataRow(MaType.EMA)]
    [DataRow(MaType.EPMA)]
    [DataRow(MaType.HMA)]
    [DataRow(MaType.SMA)]
    [DataRow(MaType.SMMA)]
    [DataRow(MaType.TEMA)]
    [DataRow(MaType.WMA)]
    public void EveryMaType_WithLateArrivalAndRemoval_MatchesSeriesExactly(MaType maType)
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(20));

        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, maType);

        for (int i = 20; i < Bars.Count; i++)
        {
            if (i == 80) { continue; }

            barHub.Add(Bars[i]);

            if (i is > 100 and < 105) { barHub.Add(Bars[i]); }
        }

        barHub.Add(Bars[80]);
        observer.Results.IsExactly(Bars.ToMaEnvelopes(lookbackPeriods, percentOffset, maType));

        barHub.RemoveAt(removeAtIndex);
        observer.Results.Should().HaveCount(501);
        observer.Results.IsExactly(RevisedBars.ToMaEnvelopes(lookbackPeriods, percentOffset, maType));

        // same public surface for every type
        observer.Should().BeOfType<MaEnvelopesHub>();
        observer.MovingAverageType.Should().Be(maType);
        observer.IsSubscribed.Should().BeTrue();

        observer.Unsubscribe();
        barHub.ObserverCount.Should().Be(0);
        barHub.EndTransmission();
    }

    [TestMethod]
    [DataRow(MaType.ALMA)]
    [DataRow(MaType.DEMA)]
    [DataRow(MaType.EMA)]
    [DataRow(MaType.EPMA)]
    [DataRow(MaType.HMA)]
    [DataRow(MaType.SMA)]
    [DataRow(MaType.SMMA)]
    [DataRow(MaType.TEMA)]
    [DataRow(MaType.WMA)]
    public void EveryMaType_WithCachePruning_MatchesSeriesExactly(MaType maType)
    {
        const int maxCacheSize = 80;  // TEMA warms up over 3 × lookback

        IReadOnlyList<Bar> bars = Bars.Take(150).ToList();
        IReadOnlyList<MaEnvelopeResult> expected = bars
            .ToMaEnvelopes(lookbackPeriods, percentOffset, maType)
            .TakeLast(maxCacheSize)
            .ToList();

        BarHub barHub = new(maxCacheSize);
        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, maType);

        barHub.Add(bars);

        observer.Results.Should().HaveCount(maxCacheSize);
        observer.Results.IsExactly(expected);

        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    [DataRow(MaType.ALMA)]
    [DataRow(MaType.DEMA)]
    [DataRow(MaType.EMA)]
    [DataRow(MaType.EPMA)]
    [DataRow(MaType.HMA)]
    [DataRow(MaType.SMA)]
    [DataRow(MaType.SMMA)]
    [DataRow(MaType.TEMA)]
    [DataRow(MaType.WMA)]
    public void EveryMaType_AfterProviderRebuild_MatchesSeriesExactly(MaType maType)
    {
        BarHub barHub = new();
        barHub.Add(Bars);

        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, maType);
        IReadOnlyList<MaEnvelopeResult> expected = Bars.ToMaEnvelopes(lookbackPeriods, percentOffset, maType);

        // rebuild from the provider re-seeds the inner hub, then this hub
        barHub.Rebuild(Bars[200].Timestamp);
        observer.Results.IsExactly(expected);

        barHub.Rebuild();
        observer.Results.IsExactly(expected);

        observer.Rebuild();
        observer.Results.IsExactly(expected);

        observer.Unsubscribe();
        barHub.EndTransmission();
    }

    [TestMethod]
    [DataRow(MaType.ALMA)]
    [DataRow(MaType.DEMA)]
    [DataRow(MaType.EMA)]
    [DataRow(MaType.EPMA)]
    [DataRow(MaType.HMA)]
    [DataRow(MaType.SMA)]
    [DataRow(MaType.SMMA)]
    [DataRow(MaType.TEMA)]
    [DataRow(MaType.WMA)]
    public void EveryMaType_Unsubscribe_ReleasesInnerHub(MaType maType)
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50));
        SmaHub other = barHub.ToSmaHub(5);

        MaEnvelopesHub observer = barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, maType);

        // the provider sees the inner hub, not this one
        barHub.ObserverCount.Should().Be(2);
        barHub.HasSubscriber(observer).Should().BeFalse();

        observer.Unsubscribe();

        // EPMA releases two levels: its EpmaHub and that hub's SlopeHub
        barHub.ObserverCount.Should().Be(1);
        other.IsSubscribed.Should().BeTrue();

        barHub.EndTransmission();
    }

    [TestMethod]
    [DataRow(MaType.ALMA)]
    [DataRow(MaType.DEMA)]
    [DataRow(MaType.EMA)]
    [DataRow(MaType.EPMA)]
    [DataRow(MaType.HMA)]
    [DataRow(MaType.SMA)]
    [DataRow(MaType.SMMA)]
    [DataRow(MaType.TEMA)]
    [DataRow(MaType.WMA)]
    public void InvalidLookbackPeriods_LeavesNoInnerHubSubscribed(MaType maType)
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50));

        Action act = () => barHub.ToMaEnvelopesHub(0, percentOffset, maType);

        act.Should().Throw<ArgumentOutOfRangeException>();
        barHub.ObserverCount.Should().Be(0);
    }

    [TestMethod]
    public void InvalidPercentOffset_LeavesNoInnerHubSubscribed()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50));

        Action act = () => barHub.ToMaEnvelopesHub(lookbackPeriods, 0, MaType.SMA);

        act.Should().Throw<ArgumentOutOfRangeException>();
        barHub.ObserverCount.Should().Be(0);
    }

    [TestMethod]
    public void UnsupportedMaType_LeavesNoInnerHubSubscribed()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50));

        Action act = () => barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, (MaType)999);

        act.Should().Throw<ArgumentOutOfRangeException>();
        barHub.ObserverCount.Should().Be(0);
    }

    [TestMethod]
    public void InsufficientCacheSize_LeavesNoInnerHubSubscribed()
    {
        BarHub barHub = new(50);  // TEMA(20) needs 60
        barHub.Add(Bars.Take(50));

        Action act = () => barHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, MaType.TEMA);

        act.Should().Throw<ArgumentOutOfRangeException>();
        barHub.ObserverCount.Should().Be(0);
    }

    [TestMethod]
    public void Unsubscribe_KeepsCallerSuppliedProvider()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50));

        SmaHub smaHub = barHub.ToSmaHub(5);
        MaEnvelopesHub observer = smaHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, MaType.HMA);

        observer.Unsubscribe();

        // only the inner HmaHub is released; the caller's SmaHub keeps running
        smaHub.ObserverCount.Should().Be(0);
        smaHub.IsSubscribed.Should().BeTrue();

        barHub.EndTransmission();
    }

    public override void ToStringOverride_ReturnsExpectedName()
    {
        MaEnvelopesHub hub = new(new BarHub(), lookbackPeriods, percentOffset, MaType.SMA);
        hub.ToString().Should().Be($"MAENV({lookbackPeriods},{percentOffset},{MaType.SMA})");
    }
}
