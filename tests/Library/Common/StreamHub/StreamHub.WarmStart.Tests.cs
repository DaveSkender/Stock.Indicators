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

        // other results (scaled closes change the differences), under a state that cannot decode
        (_, RsiHub other) = Chain(Bars.Take(60).Select(static b => new Bar(
            b.Timestamp, b.Open, b.High, b.Low, b.Close * b.Close / 100m, b.Volume)));
        HubSnapshot<RsiResult> bad = other.CaptureSnapshot() with { State = [2] };
        bad.Results.Should().NotEqual(good.Results);

        Action restore = () => rsi.RestoreSnapshot(bad);

        restore.Should().Throw<InvalidDataException>();
        rsi.Cache.Should().Equal(good.Results);
        rsi.CaptureSnapshot().State.Should().Equal(good.State);

        // and it still computes correctly afterwards
        barHub.Add(Bars.Skip(60).Take(20).ToList());
        rsi.Results.IsExactly(Bars.Take(80).ToList().ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_BadRingEntries_ThrowInvalidData()
    {
        (_, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();
        int capacity = new RollbackRing<int>().Capacity;

        byte[] Edit(Action<BinaryWriter> tail)
        {
            // version, avgGain, avgLoss, then the ring
            using MemoryStream stream = new();
            using (BinaryWriter w = new(stream))
            {
                w.Write((byte)2);
                w.Write(1.0);
                w.Write(1.0);
                tail(w);
            }

            return stream.ToArray();
        }

        void Entry(BinaryWriter w, long ticks)
        {
            w.Write(ticks);
            w.Write(1.0);
            w.Write(1.0);
        }

        (string Case, byte[] State, string Message)[] cases = [
            ("negative count", Edit(static w => w.Write(-1)), "*count -1 is out of range*"),
            ("count over capacity, complete payload", Edit(w => {
                w.Write(capacity + 1);
                for (int i = 0; i <= capacity; i++)
                {
                    Entry(w, DateTime.UnixEpoch.Ticks + i);
                }
            }), "*count*out of range*"),
            ("ticks below range", Edit(w => {
                w.Write(1);
                Entry(w, -1L);
            }), "*timestamp is out of range*"),
            ("ticks above range", Edit(w => {
                w.Write(1);
                Entry(w, long.MaxValue);
            }), "*timestamp is out of range*"),
        ];

        foreach ((string label, byte[] state, string message) in cases)
        {
            Action restore = () => rsi.RestoreSnapshot(snapshot with { State = state });
            restore.Should().ThrowExactly<InvalidDataException>(label).WithMessage(message, label);
        }
    }

    [TestMethod]
    public void Capture_ResultsAreReadOnly_AndRootHubIsNamed()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(10).ToList());

        HubSnapshot<IBar> snapshot = barHub.CaptureSnapshot();

        snapshot.Results.Should().NotBeAssignableTo<IBar[]>();
        snapshot.HubName.Should().Be("BARS");
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

    [TestMethod]
    public void Capture_RsiState_IsVersionTwo_AndOlderShapeIsRejected()
    {
        (_, RsiHub rsi) = Chain(Bars.Take(60));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();

        snapshot.State[0].Should().Be(2);

        // version 1 held only the two averages
        byte[] v1 = [1, .. BitConverter.GetBytes(1.0), .. BitConverter.GetBytes(1.0)];

        Action restore = () => rsi.RestoreSnapshot(snapshot with { State = v1 });

        restore.Should().Throw<InvalidDataException>().WithMessage("*State version 1*");
    }

    [TestMethod]
    public void Restore_GoldenVersionTwoState_DecodesEachField()
    {
        // distinct values in every field, so a reordered layout reads different averages
        List<Bar> bars = [.. Bars.Take(61)];
        (BarHub source, RsiHub sourceRsi) = Chain(bars.Take(60));
        HubSnapshot<RsiResult> snapshot = sourceRsi.CaptureSnapshot();

        const double avgGain = 2.0, avgLoss = 0.5, ringGain = 3.0, ringLoss = 0.25;
        using MemoryStream stream = new();
        using (BinaryWriter w = new(stream))
        {
            w.Write((byte)2);
            w.Write(avgGain);
            w.Write(avgLoss);
            w.Write(1);
            w.Write(bars[59].Timestamp.Ticks);
            w.Write(ringGain);
            w.Write(ringLoss);
        }

        byte[] golden = stream.ToArray();

        BarHub barHub = new();
        RsiHub rsi = barHub.ToRsiHub(Lookback);
        barHub.RestoreSnapshot(source.CaptureSnapshot());
        rsi.RestoreSnapshot(snapshot with { State = golden });

        rsi.CaptureSnapshot().State.Should().Equal(golden);

        // the next bar continues from the top-level averages
        barHub.Add(bars[60]);
        rsi.Results[^1].Rsi.Should().Be(NextRsi(avgGain, avgLoss, bars[59], bars[60]));

        // correcting it rolls back to the ring entry
        Bar revised = Revise(bars[60]);
        barHub.Add(revised);
        rsi.Results[^1].Rsi.Should().Be(NextRsi(ringGain, ringLoss, bars[59], revised));

        static double NextRsi(double gain, double loss, Bar prior, Bar bar)
        {
            double change = (double)bar.Close - (double)prior.Close;
            double g = ((gain * (Lookback - 1)) + Math.Max(change, 0)) / Lookback;
            double l = ((loss * (Lookback - 1)) + Math.Max(-change, 0)) / Lookback;
            return 100 - (100 / (1 + (g / l)));
        }
    }

    [TestMethod]
    public void Capture_AfterMidTailCorrection_RestoresAndContinues()
    {
        // a correction behind the newest bar replays later bars, so the ring is no longer ascending
        List<Bar> bars = [.. Bars.Take(200)];
        const int split = 150;

        (BarHub barHub, RsiHub rsi) = Chain(bars.Take(split));
        Bar revised = Revise(bars[split - 5]);
        barHub.Add(revised);
        bars[split - 5] = revised;

        HubSnapshot<IBar> barSnapshot = barHub.CaptureSnapshot();
        HubSnapshot<RsiResult> rsiSnapshot = rsi.CaptureSnapshot();

        BarHub barHub2 = new();
        RsiHub rsi2 = barHub2.ToRsiHub(Lookback);
        barHub2.RestoreSnapshot(barSnapshot);
        rsi2.RestoreSnapshot(rsiSnapshot);

        rsi2.CaptureSnapshot().State.Should().Equal(rsiSnapshot.State);

        barHub2.Add(bars.Skip(split).ToList());

        rsi2.Results.IsExactly(bars.ToRsi(Lookback));
    }

    [TestMethod]
    public void Restore_IntoUsedHub_ReplacesRingEntriesNotJustOverwritesThem()
    {
        // a short snapshot ring leaves slots a restore must not inherit from the used hub
        List<Bar> bars = [.. Bars.Take(60)];
        (_, RsiHub source) = Chain(bars.Take(20));
        HubSnapshot<RsiResult> sourceSnapshot = source.CaptureSnapshot();

        // same timestamps, different prices, and more bars than the ring holds
        (BarHub used, RsiHub usedRsi) = Chain(bars.Take(50).Select(static b => new Bar(
            b.Timestamp, b.Open, b.High, b.Low, b.Close * b.Close / 100m, b.Volume)));
        HubSnapshot<IBar> sourceBars = Chain(bars.Take(20)).Bars.CaptureSnapshot();

        used.RestoreSnapshot(sourceBars);
        usedRsi.RestoreSnapshot(sourceSnapshot);

        usedRsi.CaptureSnapshot().State.Should().Equal(sourceSnapshot.State);
    }

    [TestMethod]
    public void Restore_NeverPrunedSnapshot_DoesNotReopenAPrunedSpan()
    {
        List<Bar> bars = [.. Bars.Take(260)];
        BarHub pruned = new(150);
        pruned.Add(bars);

        BarHub fresh = new(150);
        fresh.Add(bars.Skip(200).Take(20).ToList());
        HubSnapshot<IBar> neverPruned = fresh.CaptureSnapshot();
        neverPruned.PrunedThrough.Should().BeNull();

        pruned.RestoreSnapshot(neverPruned);

        List<BarRejectionReason> reasons = [];
        pruned.BarRejected += (_, e) => reasons.Add(e.Reason);
        pruned.Add(bars[10]);

        reasons.Should().Equal(BarRejectionReason.PrunedHistory);
    }

    [TestMethod]
    public void Capture_ChainedBarHub_IsNamedLikeARootOne()
    {
        BarHub root = new();
        BarHub chained = new(root);

        chained.Name.Should().Be("BARS");
        chained.CaptureSnapshot().HubName.Should().Be("BARS");
    }
}
