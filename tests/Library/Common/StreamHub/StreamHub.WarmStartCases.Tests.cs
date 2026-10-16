namespace Observables;

/// <summary>
/// Warm-start behaviour every snapshot-capable bar hub must keep. The snapshot contract itself
/// is covered once, in <see cref="WarmStart"/>; this checks each hub's own state encoding.
/// </summary>
/// <typeparam name="TResult">Hub result type.</typeparam>
public abstract class WarmStartCases<TResult> : TestBase
    where TResult : ISeries
{
    private const int Split = 120;

    private protected abstract StreamHub<IBar, TResult> Subscribe(BarHub barHub);

    private protected abstract IReadOnlyList<TResult> Series(IReadOnlyList<Bar> bars);

    /// <summary>
    /// Gets restore points inside the hub's warmup, where its state is partly unset.
    /// </summary>
    private protected abstract int[] WarmupSplits { get; }

    private (BarHub Bars, StreamHub<IBar, TResult> Hub) Chain(IEnumerable<Bar> bars, int? maxCacheSize = null)
    {
        BarHub barHub = new(maxCacheSize);
        StreamHub<IBar, TResult> hub = Subscribe(barHub);
        barHub.Add(bars.ToList());
        return (barHub, hub);
    }

    private (BarHub Bars, StreamHub<IBar, TResult> Hub, HubSnapshot<TResult> Snapshot) StartFromSnapshot(
        IReadOnlyList<Bar> bars, int split = Split, int? maxCacheSize = null)
    {
        (BarHub source, StreamHub<IBar, TResult> sourceHub) = Chain(bars.Take(split), maxCacheSize);
        HubSnapshot<IBar> barSnapshot = source.CaptureSnapshot();
        HubSnapshot<TResult> snapshot = sourceHub.CaptureSnapshot();

        BarHub barHub = new(maxCacheSize);
        StreamHub<IBar, TResult> hub = Subscribe(barHub);
        barHub.RestoreSnapshot(barSnapshot);
        hub.RestoreSnapshot(snapshot);

        return (barHub, hub, snapshot);
    }

    private static Bar Revise(Bar bar, decimal by = 1m)
        => new(bar.Timestamp, bar.Open, bar.High + by, bar.Low, bar.Close + (by / 2), bar.Volume);

    [TestMethod]
    public void Restore_ResumesBitExact()
    {
        IReadOnlyList<Bar> bars = Bars.Take(200).ToList();

        (BarHub barHub, StreamHub<IBar, TResult> hub, _) = StartFromSnapshot(bars);
        barHub.Add(bars.Skip(Split).ToList());

        hub.Results.IsExactly(Series(bars));
    }

    [TestMethod]
    public void Restore_DuringWarmup_ContinuesBitExact()
    {
        IReadOnlyList<Bar> bars = Bars.Take(100).ToList();

        foreach (int split in WarmupSplits)
        {
            (BarHub barHub, StreamHub<IBar, TResult> hub, HubSnapshot<TResult> snapshot) = StartFromSnapshot(bars, split);

            hub.CaptureSnapshot().State.Should().Equal(snapshot.State, "split {0} re-encodes unchanged", split);

            barHub.Add(bars.Skip(split).ToList());
            hub.Results.IsExactly(Series(bars), "split {0}", split);
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(150)]
    public void Restore_ThenCorrectNewest_MatchesAnUninterruptedChain(int cacheLimit)
    {
        // a pruned provider is where a replay from the provider differs from the restored ring
        int? maxCacheSize = cacheLimit == 0 ? null : cacheLimit;
        List<Bar> bars = [.. Bars.Take(300)];
        const int split = 220;

        (BarHub uninterruptedBars, StreamHub<IBar, TResult> uninterrupted) = Chain(bars.Take(split), maxCacheSize);
        (BarHub restoredBars, StreamHub<IBar, TResult> restored, _) = StartFromSnapshot(bars, split, maxCacheSize);

        Bar revised = Revise(bars[split - 1]);
        foreach (BarHub hub in new[] { uninterruptedBars, restoredBars })
        {
            hub.Add(revised);
            hub.Add(bars.Skip(split).ToList());
        }

        restored.Results.IsExactly(uninterrupted.Results);
    }

    [TestMethod]
    public void Restore_RepeatedNewestUpdates_KeepRollingBack()
    {
        List<Bar> bars = [.. Bars.Take(200)];

        (BarHub barHub, StreamHub<IBar, TResult> hub, _) = StartFromSnapshot(bars);

        for (int tick = 1; tick <= 5; tick++)
        {
            bars[Split - 1] = Revise(bars[Split - 1], tick);
            barHub.Add(bars[Split - 1]);
        }

        barHub.Add(bars.Skip(Split).ToList());

        hub.Results.IsExactly(Series(bars));
    }

    [TestMethod]
    public void Restore_DeepCorrection_FallsBackToReplay()
    {
        List<Bar> bars = [.. Bars.Take(200)];

        (BarHub barHub, StreamHub<IBar, TResult> hub, _) = StartFromSnapshot(bars);
        barHub.Add(bars.Skip(Split).ToList());

        bars[Split - 30] = Revise(bars[Split - 30], 2m);
        barHub.Add(bars[Split - 30]);

        hub.Results.IsExactly(Series(bars));
    }

    [TestMethod]
    public void Restore_IntoUsedHub_ReplacesItsRollbackState()
    {
        // same timestamps, different prices: a stale rollback entry would restore the wrong state
        List<Bar> bars = [.. Bars.Take(200)];
        List<Bar> other = bars.Take(Split)
            .Select(static b => new Bar(b.Timestamp, b.Open + 7m, b.High + 7m, b.Low + 7m, b.Close * 1.3m, b.Volume * 2))
            .ToList();
        (BarHub barHub, StreamHub<IBar, TResult> hub) = Chain(other);

        (BarHub source, StreamHub<IBar, TResult> sourceHub) = Chain(bars.Take(Split));
        barHub.RestoreSnapshot(source.CaptureSnapshot());
        hub.RestoreSnapshot(sourceHub.CaptureSnapshot());

        bars[Split - 1] = Revise(bars[Split - 1]);
        barHub.Add(bars[Split - 1]);
        barHub.Add(bars.Skip(Split).ToList());

        hub.Results.IsExactly(Series(bars));
    }

    [TestMethod]
    public void Restore_TruncatedOrOtherVersionState_Throws()
    {
        (_, StreamHub<IBar, TResult> source) = Chain(Bars.Take(Split));
        HubSnapshot<TResult> snapshot = source.CaptureSnapshot();
        byte[] state = [.. snapshot.State];

        byte[] otherVersion = [.. state];
        otherVersion[0]++;

        foreach (byte[] bad in new[] { state[..^1], [.. state, 0], otherVersion })
        {
            (_, StreamHub<IBar, TResult> target) = Chain(Bars.Take(Split));
            Action act = () => target.RestoreSnapshot(snapshot with { State = bad });
            act.Should().ThrowExactly<InvalidDataException>();
        }
    }
}

[TestClass]
public class AdxWarmStart : WarmStartCases<AdxResult>
{
    private const int Lookback = 14;

    private protected override StreamHub<IBar, AdxResult> Subscribe(BarHub barHub) => barHub.ToAdxHub(Lookback);

    private protected override IReadOnlyList<AdxResult> Series(IReadOnlyList<Bar> bars) => bars.ToAdx(Lookback);

    // first bar, smoothing seed, first ADX, first ADXR
    private protected override int[] WarmupSplits => [0, 1, Lookback, (2 * Lookback) - 1, (3 * Lookback) - 1];
}
