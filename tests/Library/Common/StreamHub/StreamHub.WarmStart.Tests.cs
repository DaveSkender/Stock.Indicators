namespace Observables;

[TestClass]
public class WarmStart : TestBase
{
    private const int Lookback = 14;
    private const int Split = 120;

    private static (BarHub Bars, RsiHub Rsi) Chain(IEnumerable<Bar> bars, int lookback = Lookback)
    {
        BarHub barHub = new();
        RsiHub rsi = barHub.ToRsiHub(lookback);
        barHub.Add(bars.ToList());
        return (barHub, rsi);
    }

    /// <summary>
    /// Builds a warm-started chain: restored from the first <see cref="Split"/> bars' snapshots,
    /// with no history replay through the indicator.
    /// </summary>
    private static (BarHub Bars, RsiHub Rsi, HubSnapshot<RsiResult> RsiSnapshot) StartFromSnapshot(IReadOnlyList<Bar> bars)
    {
        (BarHub source, RsiHub sourceRsi) = Chain(bars.Take(Split));
        HubSnapshot<IBar> barSnapshot = source.CaptureSnapshot();
        HubSnapshot<RsiResult> rsiSnapshot = sourceRsi.CaptureSnapshot();

        BarHub barHub = new();
        RsiHub rsi = barHub.ToRsiHub(Lookback);

        barHub.RestoreSnapshot(barSnapshot);
        rsi.RestoreSnapshot(rsiSnapshot);

        return (barHub, rsi, rsiSnapshot);
    }

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
    public void Restore_PreservesNaNState()
    {
        // a NaN input contaminates the averages; restore must keep the exact bits
        List<Bar> bars = [.. Bars.Take(60)];
        (BarHub barHub, RsiHub rsi) = Chain(bars);

        Bar last = bars[^1];
        barHub.Add(new Bar(last.Timestamp.AddDays(1), 0m, 0m, 0m, 0m, 0m));
        HubSnapshot<RsiResult> snapshot = rsi.CaptureSnapshot();

        BarHub barHub2 = new();
        RsiHub rsi2 = barHub2.ToRsiHub(Lookback);
        barHub2.RestoreSnapshot(barHub.CaptureSnapshot());
        rsi2.RestoreSnapshot(snapshot);

        rsi2.CaptureSnapshot().State.Should().Equal(snapshot.State);
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
}
