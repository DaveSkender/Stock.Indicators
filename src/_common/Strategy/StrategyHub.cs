using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators;

/// <summary>
/// Represents an aligned pair of prior and current results for a hub.
/// </summary>
/// <typeparam name="T">Result type.</typeparam>
public readonly record struct AlignedPair<T>(T Previous, T Current)
    where T : ISeries;

/// <summary>
/// Aligns latest pairs across two hubs.
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

    public bool TryGetAligned(out AlignedPair<T1> pair1, out AlignedPair<T2> pair2)
    {
        if (!StrategyGroupUtilities.TryGetLatestPair(Hub1, out pair1)
            || !StrategyGroupUtilities.TryGetLatestPair(Hub2, out pair2))
        {
            pair1 = default;
            pair2 = default;
            return false;
        }

        return pair1.Current.Timestamp == pair2.Current.Timestamp;
    }
}

/// <summary>
/// Aligns latest pairs across three hubs.
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
    public bool TryGetAligned(
        out AlignedPair<T1> pair1,
        out AlignedPair<T2> pair2,
        out AlignedPair<T3> pair3)
    {
        if (!StrategyGroupUtilities.TryGetLatestPair(Hub1, out pair1)
            || !StrategyGroupUtilities.TryGetLatestPair(Hub2, out pair2)
            || !StrategyGroupUtilities.TryGetLatestPair(Hub3, out pair3))
        {
            pair1 = default;
            pair2 = default;
            pair3 = default;
            return false;
        }

        DateTime timestamp = pair1.Current.Timestamp;
        return timestamp == pair2.Current.Timestamp
            && timestamp == pair3.Current.Timestamp;
    }
}

/// <summary>
/// Aligns latest pairs across four hubs.
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
    public bool TryGetAligned(
        out AlignedPair<T1> pair1,
        out AlignedPair<T2> pair2,
        out AlignedPair<T3> pair3,
        out AlignedPair<T4> pair4)
    {
        if (!StrategyGroupUtilities.TryGetLatestPair(Hub1, out pair1)
            || !StrategyGroupUtilities.TryGetLatestPair(Hub2, out pair2)
            || !StrategyGroupUtilities.TryGetLatestPair(Hub3, out pair3)
            || !StrategyGroupUtilities.TryGetLatestPair(Hub4, out pair4))
        {
            pair1 = default;
            pair2 = default;
            pair3 = default;
            pair4 = default;
            return false;
        }

        DateTime timestamp = pair1.Current.Timestamp;
        return timestamp == pair2.Current.Timestamp
            && timestamp == pair3.Current.Timestamp
            && timestamp == pair4.Current.Timestamp;
    }
}

internal static class StrategyGroupUtilities
{
    internal static bool TryGetLatestPair<TResult>(
        IStreamObservable<TResult> hub,
        out AlignedPair<TResult> pair)
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
}
