namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the previous and current results for a hub.
/// </summary>
public readonly record struct BackPair<T>(T Previous, T Current)
    where T : ISeries;

internal static class StrategyGroupUtilities
{
    /// <summary>
    /// Attempts to retrieve the last two results from the specified stream observable as a back pair.
    /// </summary>
    /// <remarks>
    /// If the stream observable contains fewer than two results, the method returns
    /// <see langword="false"/> and <paramref name="pair"/> is set to its default value.
    /// </remarks>
    /// <typeparam name="TResult">
    /// The type of series elements contained in the stream observable. Must implement <see cref="ISeries"/>.
    /// </typeparam>
    /// <param name="hub">
    /// The stream observable from which to retrieve the last two results.
    /// </param>
    /// <param name="pair">
    /// When this method returns, contains a <see cref="BackPair{TResult}"/> representing
    /// the last two results if available; otherwise, the default value.
    /// </param>
    /// <param name="offset">Offset periods for <see cref="BackPair{T}.Previous"/>. Default is 1.</param>
    /// <returns>
    /// <see langword="true"/> if the stream observable contains at least two results and
    /// the back pair was retrieved; otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool TryGetBackPair<TResult>(
        this IStreamObservable<TResult> hub,
        out BackPair<TResult> pair,
        int offset = 1)
        where TResult : ISeries
    {
        IReadOnlyList<TResult> cache = hub.Results;
        int n = 1 - offset;

        if (cache.Count < n)
        {
            pair = default;
            return false;
        }

        pair = new(cache[^n], cache[^1]);
        return true;
    }

    /// <summary>
    /// Determines whether a set of <see cref="BackPair{T}"/> instances are temporally aligned
    /// by comparing their current and previous
    /// timestamps.
    /// </summary>
    /// <remarks>
    /// Use this method to verify that two series pairs are synchronized at both their current and
    /// previous positions. This is useful when processing time-aligned data from multiple sources.
    /// </remarks>
    /// <typeparam name="T1">The type of the 1st series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <typeparam name="T2">The type of the 2nd series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <param name="pair1">The 1st <see cref="BackPair{T}"/> instance to compare.</param>
    /// <param name="pair2">The 2nd <see cref="BackPair{T}"/> instance to compare.</param>
    /// <returns>
    /// <see langword="true"/> if all pairs current and previous timestamps are equal, otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool AreAligned<T1, T2>(
        BackPair<T1> pair1,
        BackPair<T2> pair2)
        where T1 : ISeries
        where T2 : ISeries
            => pair1.Current.Timestamp == pair2.Current.Timestamp
            && pair1.Previous.Timestamp == pair2.Previous.Timestamp;

    /// <inheritdoc cref="AreAligned{T1, T2}(BackPair{T1}, BackPair{T2})" />
    /// <typeparam name="T1">The type of the 1st series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <typeparam name="T2">The type of the 2nd series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <typeparam name="T3">The type of the 3rd series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <param name="pair1">The 1st <see cref="BackPair{T}"/> instance to compare.</param>
    /// <param name="pair2">The 2nd <see cref="BackPair{T}"/> instance to compare.</param>
    /// <param name="pair3">The 3rd <see cref="BackPair{T}"/> instance to compare.</param>

    internal static bool AreAligned<T1, T2, T3>(
        BackPair<T1> pair1,
        BackPair<T2> pair2,
        BackPair<T3> pair3)
        where T1 : ISeries
        where T2 : ISeries
        where T3 : ISeries
            => AreAligned(pair1, pair2)
            && pair1.Current.Timestamp == pair3.Current.Timestamp
            && pair1.Previous.Timestamp == pair3.Previous.Timestamp;

    /// <inheritdoc cref="AreAligned{T1, T2}(BackPair{T1}, BackPair{T2})" />
    /// <typeparam name="T1">The type of the 1st series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <typeparam name="T2">The type of the 2nd series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <typeparam name="T3">The type of the 3rd series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <typeparam name="T4">The type of the 4th series, which must implement the <see cref="ISeries"/> interface.</typeparam>
    /// <param name="pair1">The 1st <see cref="BackPair{T}"/> instance to compare.</param>
    /// <param name="pair2">The 2nd <see cref="BackPair{T}"/> instance to compare.</param>
    /// <param name="pair3">The 3rd <see cref="BackPair{T}"/> instance to compare.</param>
    /// <param name="pair4">The 4th <see cref="BackPair{T}"/> instance to compare.</param>
    internal static bool AreAligned<T1, T2, T3, T4>(
        BackPair<T1> pair1,
        BackPair<T2> pair2,
        BackPair<T3> pair3,
        BackPair<T4> pair4)
        where T1 : ISeries
        where T2 : ISeries
        where T3 : ISeries
        where T4 : ISeries
            => AreAligned(pair1, pair2, pair3)
            && pair1.Current.Timestamp == pair4.Current.Timestamp
            && pair1.Previous.Timestamp == pair4.Previous.Timestamp;
}

