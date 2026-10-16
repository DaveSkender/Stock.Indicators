namespace Observables;

[TestClass]
public class WarmStart : TestBase
{
    private const int Lookback = 14;
    private const int Split = 120;

    private static (BarHub Bars, RsiHub Rsi) Chain(IEnumerable<Bar> bars, int lookback = Lookback, int? maxCacheSize = null)
    {
        BarHub barHub = new(maxCacheSize);
        RsiHub rsi = barHub.ToRsiHub(lookback);
        barHub.Add(bars.ToList());
        return (barHub, rsi);
    }

    /// <summary>
    /// Builds a warm-started chain: restored from snapshots of the first <paramref name="split"/> bars,
    /// with no history replay through the indicator.
    /// </summary>
    private static (BarHub Bars, RsiHub Rsi, HubSnapshot<RsiResult> RsiSnapshot) StartFromSnapshot(
        IReadOnlyList<Bar> bars, int split = Split, int? maxCacheSize = null)
    {
        (BarHub source, RsiHub sourceRsi) = Chain(bars.Take(split), maxCacheSize: maxCacheSize);
        HubSnapshot<IBar> barSnapshot = source.CaptureSnapshot();
        HubSnapshot<RsiResult> rsiSnapshot = sourceRsi.CaptureSnapshot();

        BarHub barHub = new(maxCacheSize);
        RsiHub rsi = barHub.ToRsiHub(Lookback);

        barHub.RestoreSnapshot(barSnapshot);
        rsi.RestoreSnapshot(rsiSnapshot);

        return (barHub, rsi, rsiSnapshot);
    }

    private static Bar Revise(Bar bar)
        => new(bar.Timestamp, bar.Open, bar.High + 1m, bar.Low, bar.Close + 0.5m, bar.Volume);

    [TestMethod]
    public void Restore_ResumesBitExact_WithoutReplayingHistory()
    {
        IReadOnlyList<Bar> bars = Bars.Take(200).ToList();

        (BarHub barHub, RsiHub rsi, HubSnapshot<RsiResult> snapshot) = StartFromSnapshot(bars);

        // restored results are the snapshot's own, not recomputed
        rsi.Cache.Should().HaveCount(Split);
        ReferenceEquals(rsi.Cache[0], snapshot.Results[0]).Should().BeTrue();

        barHub.Add(bars.Skip(Split).ToList());

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_NewestItemCorrection_StillMatchesSeries()
    {
        List<Bar> bars = [.. Bars.Take(200)];

        (BarHub barHub, RsiHub rsi, _) = StartFromSnapshot(bars);

        // revise the newest restored bar, then continue
        Bar last = bars[Split - 1];
        Bar revised = new(
            last.Timestamp, last.Open, last.High + 1m, last.Low, last.Close + 0.5m, last.Volume);
        barHub.Add(revised);
        bars[Split - 1] = revised;

        barHub.Add(bars.Skip(Split).ToList());

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_DeepCorrection_FallsBackToReplayAndMatchesSeries()
    {
        List<Bar> bars = [.. Bars.Take(200)];

        (BarHub barHub, RsiHub rsi, _) = StartFromSnapshot(bars);
        barHub.Add(bars.Skip(Split).ToList());

        // a correction older than the restored point forces a replay from the provider
        Bar old = bars[Split - 30];
        Bar revised = new(
            old.Timestamp, old.Open, old.High + 2m, old.Low, old.Close + 1m, old.Volume);
        barHub.Add(revised);
        bars[Split - 30] = revised;

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    public void Capture_IsACopy_LaterAddsDoNotChangeIt()
    {
        (BarHub barHub, RsiHub rsi) = Chain(Bars.Take(60));

        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();
        byte[] state = [.. snapshot.State];

        barHub.Add(Bars.Skip(60).Take(20).ToList());

        snapshot.Results.Should().HaveCount(60);
        snapshot.State.Should().Equal(state);
    }

    [TestMethod]
    public void Restore_WrongHubName_Throws()
    {
        (_, RsiHub source) = Chain(Bars.Take(60), 14);
        HubSnapshot<RsiResult> snapshot = source.CaptureSnapshot();

        (_, RsiHub other) = Chain(Bars.Take(60), 10);

        Action restore = () => other.RestoreSnapshot(snapshot);

        restore.Should().Throw<InvalidDataException>().WithMessage("*RSI(14)*RSI(10)*");
    }

    [TestMethod]
    public void Restore_OtherFormatVersion_Throws()
    {
        (_, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot() with { FormatVersion = 99 };

        Action restore = () => rsi.RestoreSnapshot(snapshot);

        restore.Should().Throw<InvalidDataException>().WithMessage("*format 99*");
    }

    [TestMethod]
    public void Restore_OtherStateVersion_Throws()
    {
        (_, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();
        byte[] state = [.. snapshot.State];
        state[0] = 7;

        Action restore = () => rsi.RestoreSnapshot(snapshot with { State = state });

        restore.Should().Throw<InvalidDataException>().WithMessage("*State version 7*");
    }

    [TestMethod]
    public void Restore_TruncatedOrLongState_Throws()
    {
        (_, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();

        Action truncated = () => rsi.RestoreSnapshot(snapshot with { State = snapshot.State[..5] });
        Action extended = () => rsi.RestoreSnapshot(snapshot with { State = [.. snapshot.State, 0] });

        truncated.Should().Throw<InvalidDataException>().WithMessage("*shorter*");
        extended.Should().Throw<InvalidDataException>().WithMessage("*longer*");
    }

    [TestMethod]
    public void Restore_ProviderNotRestored_Throws()
    {
        (_, RsiHub source) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = source.CaptureSnapshot();

        BarHub barHub = new();
        RsiHub rsi = barHub.ToRsiHub(Lookback);

        Action restore = () => rsi.RestoreSnapshot(snapshot);

        restore.Should().Throw<InvalidDataException>().WithMessage("*provider*");
        rsi.Cache.Should().BeEmpty();
    }

    [TestMethod]
    public void Capture_UnsupportedHub_Throws()
    {
        BarHub barHub = new();
        SmaHub sma = barHub.ToSmaHub(10);

        Action capture = () => sma.CaptureSnapshot();

        capture.Should().Throw<NotSupportedException>().WithMessage("*SMA(10)*");
    }

    [TestMethod]
    public void Restore_MoreResultsThanCacheHolds_Throws()
    {
        BarHub source = new();
        source.Add(Bars.Take(100).ToList());

        BarHub small = new(maxCacheSize: 50);

        Action restore = () => small.RestoreSnapshot(source.CaptureSnapshot());

        restore.Should().Throw<InvalidDataException>().WithMessage("*cache limit*");
        small.Cache.Should().BeEmpty();
    }

    [TestMethod]
    public void Restore_IntoUsedHub_DropsItsOldLastItem()
    {
        IReadOnlyList<Bar> bars = Bars.Take(200).ToList();

        // the target has already computed the very next bar, so its last item
        // equals the result the restored hub is about to produce
        (BarHub barHub, RsiHub rsi) = Chain(bars.Take(Split + 1));

        (BarHub source, RsiHub sourceRsi) = Chain(bars.Take(Split));
        barHub.RestoreSnapshot(source.CaptureSnapshot());
        rsi.RestoreSnapshot(sourceRsi.CaptureSnapshot());

        barHub.Add(bars[Split]);

        rsi.Results.Should().HaveCount(Split + 1, "the next bar must not be suppressed as a repeat");
        rsi.Results.IsExactly(bars.Take(Split + 1).ToList().ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_IntoUsedHub_DropsItsOldRollbackState()
    {
        List<Bar> bars = [.. Bars.Take(200)];

        // same timestamps, different prices: stale rollback entries would restore the wrong averages
        List<Bar> other = bars.Take(Split)
            .Select(b => new Bar(b.Timestamp, b.Open + 7m, b.High + 7m, b.Low + 7m, b.Close * 1.3m, b.Volume))
            .ToList();
        (BarHub barHub, RsiHub rsi) = Chain(other);

        (BarHub source, RsiHub sourceRsi) = Chain(bars.Take(Split));
        barHub.RestoreSnapshot(source.CaptureSnapshot());
        rsi.RestoreSnapshot(sourceRsi.CaptureSnapshot());

        Bar last = bars[Split - 1];
        Bar revised = new(
            last.Timestamp, last.Open, last.High + 1m, last.Low, last.Close + 0.5m, last.Volume);
        barHub.Add(revised);
        bars[Split - 1] = revised;
        barHub.Add(bars.Skip(Split).ToList());

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(13)]
    [DataRow(14)]
    public void Restore_DuringWarmup_ContinuesBitExact(int split)
    {
        // before the first calculable position the averages are NaN, and must survive as NaN
        IReadOnlyList<Bar> bars = Bars.Take(100).ToList();

        (BarHub barHub, RsiHub rsi, HubSnapshot<RsiResult> snapshot) = StartFromSnapshot(bars, split);

        rsi.CaptureSnapshot().State.Should().Equal(snapshot.State);

        barHub.Add(bars.Skip(split).ToList());

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(150)]
    public void Restore_ThenCorrectNewest_MatchesAnUninterruptedChain(int cacheLimit)
    {
        int? maxCacheSize = cacheLimit == 0 ? null : cacheLimit;
        // a pruned provider is where a replay from the provider differs from the live path
        List<Bar> bars = [.. Bars.Take(300)];
        const int split = 220;

        (BarHub uninterruptedBars, RsiHub uninterrupted) = Chain(bars.Take(split), maxCacheSize: maxCacheSize);
        (BarHub restoredBars, RsiHub restored, _) = StartFromSnapshot(bars, split, maxCacheSize);

        Bar revised = Revise(bars[split - 1]);
        foreach (BarHub hub in new[] { uninterruptedBars, restoredBars })
        {
            hub.Add(revised);
            hub.Add(bars.Skip(split).ToList());
        }

        restored.Results.IsExactly(uninterrupted.Results);
        restoredBars.Cache.Should().Equal(uninterruptedBars.Cache);
    }

    [TestMethod]
    public void Restore_RepeatedNewestUpdates_KeepRollingBack()
    {
        // a forming bar is revised on every tick; each revision must resolve from the restored ring
        List<Bar> bars = [.. Bars.Take(200)];

        (BarHub barHub, RsiHub rsi, _) = StartFromSnapshot(bars);

        for (int tick = 1; tick <= 5; tick++)
        {
            Bar last = bars[Split - 1];
            bars[Split - 1] = new Bar(
                last.Timestamp, last.Open, last.High + tick, last.Low, last.Close + (tick * 0.1m), last.Volume);
            barHub.Add(bars[Split - 1]);
        }

        barHub.Add(bars.Skip(Split).ToList());

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_SameCountOtherLastTimestamp_Throws()
    {
        List<Bar> bars = [.. Bars.Take(61)];
        (_, RsiHub source) = Chain(bars.Take(60));
        HubSnapshot<RsiResult> snapshot = source.CaptureSnapshot();

        // same count, but the newest bar is a different day
        (_, RsiHub target) = Chain(bars.Take(59).Append(bars[60]));

        Action restore = () => target.RestoreSnapshot(snapshot);

        restore.Should().Throw<InvalidDataException>().WithMessage("*line up*");
    }

    [TestMethod]
    public void Restore_SameCountOtherFirstTimestamp_Throws()
    {
        List<Bar> bars = [.. Bars.Take(60)];
        (_, RsiHub source) = Chain(bars);
        HubSnapshot<RsiResult> snapshot = source.CaptureSnapshot();

        Bar shifted = new(bars[0].Timestamp.AddHours(1), bars[0].Open, bars[0].High, bars[0].Low, bars[0].Close, bars[0].Volume);
        (_, RsiHub target) = Chain(bars.Skip(1).Prepend(shifted));

        Action restore = () => target.RestoreSnapshot(snapshot);

        restore.Should().Throw<InvalidDataException>().WithMessage("*line up*");
    }

    [TestMethod]
    public void Restore_PrunedHistory_StillRefusesOlderBars()
    {
        List<Bar> bars = [.. Bars.Take(260)];
        BarHub source = new(150);
        source.Add(bars);

        BarHub target = new(150);
        target.RestoreSnapshot(source.CaptureSnapshot());

        List<BarRejectionReason> reasons = [];
        target.BarRejected += (_, e) => reasons.Add(e.Reason);

        // older than the restored head, and inside the pruned span
        target.Add(bars[10]);

        reasons.Should().Equal(BarRejectionReason.PrunedHistory);
    }

    [TestMethod]
    public void Restore_ClearsAStaleFault()
    {
        BarHub source = new();
        source.Add(Bars.Take(10).ToList());

        BarHub target = new();
        Bar bar = Bars[0];
        target.Add(bar);

        // repeat one bar until the hub faults
        for (int i = 0; i < 150 && !target.IsFaulted; i++)
        {
            try
            {
                target.Add(bar);
            }
            catch (OverflowException)
            {
                break;
            }
        }

        target.IsFaulted.Should().BeTrue("the setup must fault the hub");

        target.RestoreSnapshot(source.CaptureSnapshot());

        target.IsFaulted.Should().BeFalse();
    }

    [TestMethod]
    public void Restore_FailedDecode_LeavesTheHubUntouched()
    {
        (BarHub barHub, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> good = rsi.CaptureSnapshot();
        (_, RsiHub other) = Chain(Bars.Take(60).Select(Revise));
        HubSnapshot<RsiResult> bad = other.CaptureSnapshot() with { State = [1] };

        Action restore = () => rsi.RestoreSnapshot(bad);

        restore.Should().Throw<InvalidDataException>();
        rsi.Cache.Should().Equal(good.Results);

        // and it still computes correctly afterwards
        barHub.Add(Bars.Skip(60).Take(20).ToList());
        rsi.Results.IsExactly(Bars.Take(80).ToList().ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_NullSnapshot_Throws()
    {
        BarHub barHub = new();

        Action restore = () => barHub.RestoreSnapshot(null!);

        restore.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Restore_EmptyState_Throws()
    {
        (_, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();

        Action restore = () => rsi.RestoreSnapshot(snapshot with { State = [] });

        restore.Should().Throw<InvalidDataException>().WithMessage("*empty*");
    }

    [TestMethod]
    public void Restore_RootHubWithOtherStateVersion_Throws()
    {
        BarHub source = new();
        source.Add(Bars.Take(10).ToList());
        HubSnapshot<IBar> snapshot = source.CaptureSnapshot();
        byte[] state = [.. snapshot.State];
        state[0] = 9;

        BarHub target = new();

        Action restore = () => target.RestoreSnapshot(snapshot with { State = state });

        restore.Should().Throw<InvalidDataException>().WithMessage("*State version 9*");
    }

    [TestMethod]
    public void Restore_AfterProviderRestore_UnsupportedSubscriberNeedsRebuild()
    {
        IReadOnlyList<Bar> bars = Bars.Take(100).ToList();
        (BarHub source, _) = Chain(bars.Take(60));

        BarHub barHub = new();
        SmaHub sma = barHub.ToSmaHub(10);
        barHub.RestoreSnapshot(source.CaptureSnapshot());

        // the subscriber was not told; it rebuilds from the restored provider
        sma.Cache.Should().BeEmpty();
        sma.Rebuild();
        barHub.Add(bars.Skip(60).ToList());

        sma.Results.IsExactly(bars.ToSma(10));
    }

    [TestMethod]
    public void Restore_ThenRebuild_StillMatchesSeries()
    {
        IReadOnlyList<Bar> bars = Bars.Take(200).ToList();
        (BarHub barHub, RsiHub rsi, _) = StartFromSnapshot(bars);

        rsi.Rebuild();
        barHub.Add(bars.Skip(Split).ToList());

        rsi.Results.IsExactly(bars.ToRsi(Lookback));
    }
}
