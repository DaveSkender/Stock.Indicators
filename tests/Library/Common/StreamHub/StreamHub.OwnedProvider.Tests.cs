namespace Observables;

/// <summary>
/// A hub that builds its own inner hub must release it on unsubscribe, and must
/// never release an inner hub the caller supplied.
/// </summary>
[TestClass]
public class OwnedProviderRelease : TestBase
{
    public static IEnumerable<object[]> HubsWithOwnedProvider =>
    [
        ["StochRsi", (Func<BarHub, object>)(b => b.ToStochRsiHub())],
        ["ConnorsRsi", (Func<BarHub, object>)(b => b.ToConnorsRsiHub())],
        ["Epma", (Func<BarHub, object>)(b => b.ToEpmaHub(10))],
        ["Gator", (Func<BarHub, object>)(b => b.ToGatorHub())],
        ["Stc", (Func<BarHub, object>)(b => b.ToStcHub())],
        ["Pvo", (Func<BarHub, object>)(b => b.ToPvoHub())],
    ];

    [TestMethod]
    [DynamicData(nameof(HubsWithOwnedProvider))]
    public void Unsubscribe_ReleasesOwnedProvider(string name, Func<BarHub, object> subscribe)
    {
        ArgumentNullException.ThrowIfNull(subscribe);

        BarHub barHub = new();
        barHub.Add(Bars.Take(100));
        SmaHub other = barHub.ToSmaHub(5);  // unrelated observer must survive
        int baseline = barHub.ObserverCount;

        object hub = subscribe(barHub);
        barHub.ObserverCount.Should().Be(baseline + 1, name);

        // the added observer is the inner hub: an outer hub that observes bars is checked
        // on barHub, and one that observes another result type cannot be on it at all
        bool outerOnBarHub = hub is IStreamObserver<IBar> direct && barHub.HasSubscriber(direct);
        outerOnBarHub.Should().BeFalse(name);

        ((IReleasableHub)hub).Unsubscribe();

        barHub.ObserverCount.Should().Be(baseline, $"{name} left its inner hub subscribed");
        other.IsSubscribed.Should().BeTrue();

        barHub.EndTransmission();
    }

    public static IEnumerable<object[]> InvalidArguments =>
    [
        ["StochRsi", (Action<BarHub>)(b => b.ToStochRsiHub(14, 0))],
        ["ConnorsRsi", (Action<BarHub>)(b => b.ToConnorsRsiHub(3, 2, 1))],
        ["Epma", (Action<BarHub>)(b => b.ToEpmaHub(0))],  // control: SlopeHub validates before it subscribes
        ["Stc", (Action<BarHub>)(b => b.ToStcHub(-1))],
        ["Pvo", (Action<BarHub>)(b => b.ToPvoHub(0))],
    ];

    [TestMethod]
    [DynamicData(nameof(InvalidArguments))]
    public void InvalidArgument_LeavesNoInnerHubSubscribed(string name, Action<BarHub> subscribe)
    {
        ArgumentNullException.ThrowIfNull(subscribe);

        BarHub barHub = new();
        barHub.Add(Bars.Take(50));

        Action act = () => subscribe(barHub);

        act.Should().Throw<ArgumentOutOfRangeException>(name);
        barHub.ObserverCount.Should().Be(0, $"{name} stranded its inner hub");
    }

    [TestMethod]
    public void Unsubscribe_EpmaReleasesItsSlopeHub()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(50));

        EpmaHub epma = barHub.ToEpmaHub(10);
        barHub.ObserverCount.Should().Be(1);  // the inner SlopeHub, not EpmaHub

        epma.Unsubscribe();

        barHub.ObserverCount.Should().Be(0);
        barHub.HasObservers.Should().BeFalse();
    }

    [TestMethod]
    public void Unsubscribe_KeepsCallerSuppliedRsiHub()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(100));

        RsiHub rsiHub = barHub.ToRsiHub(14);
        StochRsiHub first = rsiHub.ToStochRsiHub(14, 3, 1);
        StochRsiHub second = rsiHub.ToStochRsiHub(10, 3, 1);

        first.Unsubscribe();
        second.Unsubscribe();

        // the caller still holds rsiHub, so it keeps receiving bars
        rsiHub.IsSubscribed.Should().BeTrue();
        barHub.ObserverCount.Should().Be(1);

        int before = rsiHub.Results.Count;
        barHub.Add(Bars[100]);
        rsiHub.Results.Should().HaveCount(before + 1);

        barHub.EndTransmission();
    }

    [TestMethod]
    public void Unsubscribe_KeepsCallerSuppliedMacdAndAlligatorHubs()
    {
        BarHub barHub = new();
        barHub.Add(Bars.Take(100));

        MacdHub macdHub = barHub.ToMacdHub(23, 50, 0);
        AlligatorHub alligatorHub = barHub.ToAlligatorHub();

        macdHub.ToStcHub(10).Unsubscribe();
        alligatorHub.ToGatorHub().Unsubscribe();

        macdHub.IsSubscribed.Should().BeTrue();
        alligatorHub.IsSubscribed.Should().BeTrue();
        barHub.ObserverCount.Should().Be(2);

        barHub.EndTransmission();
    }
}
