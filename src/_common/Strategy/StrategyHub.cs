using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the back pair (previous and current) of results for a hub.
/// </summary>
/// <typeparam name="T">Result type.</typeparam>
public readonly record struct BackPair<T>(T Previous, T Current)
    where T : ISeries;

/// <summary>
/// Aligns the most recent back pairs across two hubs.
/// </summary>
public sealed class StrategyGroup<T1, T2>
    where T1 : ISeries
    where T2 : ISeries
{
    public StrategyGroup(IStreamObservable<T1> hub1, IStreamObservable<T2> hub2)
    {
        Hub1 = hub1 ?? throw new ArgumentNullException(nameof(hub1));
        Hub2 = hub2 ?? throw new ArgumentNullException(nameof(hub2));
    }

    public IStreamObservable<T1> Hub1 { get; }

    public IStreamObservable<T2> Hub2 { get; }

    public bool TryGetBackPair(out BackPair<T1> pair1, out BackPair<T2> pair2)
    {
        if (!StrategyGroupUtilities.TryGetBackPair(Hub1, out pair1)
            || !StrategyGroupUtilities.TryGetBackPair(Hub2, out pair2))
        {
            pair1 = default;
            pair2 = default;
            return false;
        }

        return StrategyGroupUtilities.AreAligned(pair1, pair2);
    }
}

/// <summary>
/// Aligns the most recent back pairs across three hubs.
/// </summary>
public sealed class StrategyGroup<T1, T2, T3>
    where T1 : ISeries
    where T2 : ISeries
    where T3 : ISeries
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyGroup{T1, T2, T3}"/> class.
    /// </summary>
    /// <param name="hub1">First hub.</param>
    /// <param name="hub2">Second hub.</param>
    /// <param name="hub3">Third hub.</param>
    public StrategyGroup(
        IStreamObservable<T1> hub1,
        IStreamObservable<T2> hub2,
        IStreamObservable<T3> hub3)
    {
        Hub1 = hub1 ?? throw new ArgumentNullException(nameof(hub1));
        Hub2 = hub2 ?? throw new ArgumentNullException(nameof(hub2));
        Hub3 = hub3 ?? throw new ArgumentNullException(nameof(hub3));
    }

    public IStreamObservable<T1> Hub1 { get; }

    public IStreamObservable<T2> Hub2 { get; }

    public IStreamObservable<T3> Hub3 { get; }

    /// <summary>
    /// Attempts to align the latest two results from all hubs at the same timestamp.
    /// </summary>
    /// <param name="pair1">Aligned pair for the first hub.</param>
    /// <param name="pair2">Aligned pair for the second hub.</param>
    /// <param name="pair3">Aligned pair for the third hub.</param>
    /// <returns>True when all hubs have at least two results and the most recent timestamps match.</returns>
    public bool TryGetBackPair(
        out BackPair<T1> pair1,
        out BackPair<T2> pair2,
        out BackPair<T3> pair3)
    {
        if (!StrategyGroupUtilities.TryGetBackPair(Hub1, out pair1)
            || !StrategyGroupUtilities.TryGetBackPair(Hub2, out pair2)
            || !StrategyGroupUtilities.TryGetBackPair(Hub3, out pair3))
        {
            pair1 = default;
            pair2 = default;
            pair3 = default;
            return false;
        }

        return StrategyGroupUtilities.AreAligned(pair1, pair2, pair3);
    }
}

/// <summary>
/// Aligns the most recent back pairs across four hubs.
/// </summary>
public sealed class StrategyGroup<T1, T2, T3, T4>
    where T1 : ISeries
    where T2 : ISeries
    where T3 : ISeries
    where T4 : ISeries
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyGroup{T1, T2, T3, T4}"/> class.
    /// </summary>
    /// <param name="hub1">First hub.</param>
    /// <param name="hub2">Second hub.</param>
    /// <param name="hub3">Third hub.</param>
    /// <param name="hub4">Fourth hub.</param>
    public StrategyGroup(
        IStreamObservable<T1> hub1,
        IStreamObservable<T2> hub2,
        IStreamObservable<T3> hub3,
        IStreamObservable<T4> hub4)
    {
        Hub1 = hub1 ?? throw new ArgumentNullException(nameof(hub1));
        Hub2 = hub2 ?? throw new ArgumentNullException(nameof(hub2));
        Hub3 = hub3 ?? throw new ArgumentNullException(nameof(hub3));
        Hub4 = hub4 ?? throw new ArgumentNullException(nameof(hub4));
    }

    public IStreamObservable<T1> Hub1 { get; }

    public IStreamObservable<T2> Hub2 { get; }

    public IStreamObservable<T3> Hub3 { get; }

    public IStreamObservable<T4> Hub4 { get; }

    /// <summary>
    /// Attempts to align the latest two results from all hubs at the same timestamp.
    /// </summary>
    /// <param name="pair1">Aligned pair for the first hub.</param>
    /// <param name="pair2">Aligned pair for the second hub.</param>
    /// <param name="pair3">Aligned pair for the third hub.</param>
    /// <param name="pair4">Aligned pair for the fourth hub.</param>
    /// <returns>True when all hubs have at least two results and the most recent timestamps match.</returns>
    public bool TryGetBackPair(
        out BackPair<T1> pair1,
        out BackPair<T2> pair2,
        out BackPair<T3> pair3,
        out BackPair<T4> pair4)
    {
        if (!StrategyGroupUtilities.TryGetBackPair(Hub1, out pair1)
            || !StrategyGroupUtilities.TryGetBackPair(Hub2, out pair2)
            || !StrategyGroupUtilities.TryGetBackPair(Hub3, out pair3)
            || !StrategyGroupUtilities.TryGetBackPair(Hub4, out pair4))
        {
            pair1 = default;
            pair2 = default;
            pair3 = default;
            pair4 = default;
            return false;
        }

        return StrategyGroupUtilities.AreAligned(pair1, pair2, pair3, pair4);
    }
}

internal static class StrategyGroupUtilities
{
    internal static bool TryGetBackPair<TResult>(
        IStreamObservable<TResult> hub,
        out BackPair<TResult> pair)
        where TResult : ISeries
    {
        IReadOnlyList<TResult> cache = hub.Results;

        if (cache.Count < 2)
        {
            pair = default;
            return false;
        }

        pair = new(cache[^2], cache[^1]);
        return true;
    }

    internal static bool AreAligned<T1, T2>(BackPair<T1> pair1, BackPair<T2> pair2)
        where T1 : ISeries
        where T2 : ISeries
    {
        return pair1.Current.Timestamp == pair2.Current.Timestamp
            && pair1.Previous.Timestamp == pair2.Previous.Timestamp;
    }

    internal static bool AreAligned<T1, T2, T3>(
        BackPair<T1> pair1,
        BackPair<T2> pair2,
        BackPair<T3> pair3)
        where T1 : ISeries
        where T2 : ISeries
        where T3 : ISeries
    {
        return AreAligned(pair1, pair2)
            && pair1.Current.Timestamp == pair3.Current.Timestamp
            && pair1.Previous.Timestamp == pair3.Previous.Timestamp;
    }

    internal static bool AreAligned<T1, T2, T3, T4>(
        BackPair<T1> pair1,
        BackPair<T2> pair2,
        BackPair<T3> pair3,
        BackPair<T4> pair4)
        where T1 : ISeries
        where T2 : ISeries
        where T3 : ISeries
        where T4 : ISeries
    {
        return AreAligned(pair1, pair2, pair3)
            && pair1.Current.Timestamp == pair4.Current.Timestamp
            && pair1.Previous.Timestamp == pair4.Previous.Timestamp;
    }
}
