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
        ["StochRsi", (Func<BarHub, Action>)(b => b.ToStochRsiHub().Unsubscribe)],
        ["ConnorsRsi", (Func<BarHub, Action>)(b => b.ToConnorsRsiHub().Unsubscribe)],
        ["Epma", (Func<BarHub, Action>)(b => b.ToEpmaHub(10).Unsubscribe)],
        ["Gator", (Func<BarHub, Action>)(b => b.ToGatorHub().Unsubscribe)],
        ["Stc", (Func<BarHub, Action>)(b => b.ToStcHub().Unsubscribe)],
        ["Pvo", (Func<BarHub, Action>)(b => b.ToPvoHub().Unsubscribe)],
    ];

    [TestMethod]
    [DynamicData(nameof(HubsWithOwnedProvider))]
    public void Unsubscribe_ReleasesOwnedProvider(string name, Func<BarHub, Action> subscribe)
    {
        ArgumentNullException.ThrowIfNull(subscribe);

        BarHub barHub = new();
        barHub.Add(Bars.Take(100));
        SmaHub other = barHub.ToSmaHub(5);  // unrelated observer must survive
        int baseline = barHub.ObserverCount;

        Action unsubscribe = subscribe(barHub);
        barHub.ObserverCount.Should().Be(baseline + 1, name);

        unsubscribe();

        barHub.ObserverCount.Should().Be(baseline, $"{name} left its inner hub subscribed");
        other.IsSubscribed.Should().BeTrue();

        barHub.EndTransmission();
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
