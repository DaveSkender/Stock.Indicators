
namespace Skender.Stock.Indicators;

/// <summary>
/// Groups two stream observable hubs for coordinated access and alignment.
/// </summary>
public sealed class StrategyGroup<T1, T2>(IStreamObservable<T1> hub1, IStreamObservable<T2> hub2)
    where T1 : ISeries
    where T2 : ISeries
{
    /// <summary>
    /// Gets the 1st stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T1> Hub1 { get; } = hub1 ?? throw new ArgumentNullException(nameof(hub1));
    /// <summary>
    /// Gets the 2nd stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T2> Hub2 { get; } = hub2 ?? throw new ArgumentNullException(nameof(hub2));

    /// <summary>
    /// Attempts to retrieve and align the most recent pairs from both hubs.
    /// </summary>
    public bool TryGetBackPair(out BackPair<T1> pair1, out BackPair<T2> pair2)
    {
        if (!Hub1.TryGetBackPair(out pair1)
         || !Hub2.TryGetBackPair(out pair2))
        {
            pair1 = default;
            pair2 = default;
            return false;
        }

        return StrategyGroupUtilities.AreAligned(pair1, pair2);
    }
}

/// <summary>
/// Groups three stream observable hubs for coordinated access and alignment.
/// </summary>
public sealed class StrategyGroup<T1, T2, T3>(
    IStreamObservable<T1> hub1,
    IStreamObservable<T2> hub2,
    IStreamObservable<T3> hub3)
    where T1 : ISeries
    where T2 : ISeries
    where T3 : ISeries
{
    /// <summary>
    /// Gets the 1st stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T1> Hub1 { get; } = hub1 ?? throw new ArgumentNullException(nameof(hub1));
    /// <summary>
    /// Gets the 2nd stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T2> Hub2 { get; } = hub2 ?? throw new ArgumentNullException(nameof(hub2));
    /// <summary>
    /// Gets the 3rd stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T3> Hub3 { get; } = hub3 ?? throw new ArgumentNullException(nameof(hub3));

    /// <summary>
    /// Attempts to retrieve and align the most recent pairs from all hubs.
    /// </summary>
    public bool TryGetBackPair(
        out BackPair<T1> pair1,
        out BackPair<T2> pair2,
        out BackPair<T3> pair3)
    {
        if (!Hub1.TryGetBackPair(out pair1)
         || !Hub2.TryGetBackPair(out pair2)
         || !Hub3.TryGetBackPair(out pair3))
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
/// Groups four stream observable hubs for coordinated access and alignment.
/// </summary>
public sealed class StrategyGroup<T1, T2, T3, T4>(
    IStreamObservable<T1> hub1,
    IStreamObservable<T2> hub2,
    IStreamObservable<T3> hub3,
    IStreamObservable<T4> hub4)
    where T1 : ISeries
    where T2 : ISeries
    where T3 : ISeries
    where T4 : ISeries
{
    /// <summary>
    /// Gets the 1st stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T1> Hub1 { get; } = hub1 ?? throw new ArgumentNullException(nameof(hub1));
    /// <summary>
    /// Gets the 2nd stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T2> Hub2 { get; } = hub2 ?? throw new ArgumentNullException(nameof(hub2));
    /// <summary>
    /// Gets the 3rd stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T3> Hub3 { get; } = hub3 ?? throw new ArgumentNullException(nameof(hub3));
    /// <summary>
    /// Gets the 4th stream observable hub the <see cref="StrategyGroup{T1, T2, T3, T4}"/>.
    /// </summary>
    public IStreamObservable<T4> Hub4 { get; } = hub4 ?? throw new ArgumentNullException(nameof(hub4));

    /// <summary>
    /// Attempts to retrieve and align the most recent pairs from all hubs.
    /// </summary>
    public bool TryGetBackPair(
        out BackPair<T1> pair1,
        out BackPair<T2> pair2,
        out BackPair<T3> pair3,
        out BackPair<T4> pair4)
    {
        if (!Hub1.TryGetBackPair(out pair1)
         || !Hub2.TryGetBackPair(out pair2)
         || !Hub3.TryGetBackPair(out pair3)
         || !Hub4.TryGetBackPair(out pair4))
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
