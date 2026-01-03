namespace Skender.Stock.Indicators;

/// <summary>
/// TDI-GM indicator.
/// </summary>
public static partial class TdiGm
{
    /// <summary>
    /// Calculates the TDI-GM for a series of data.
    /// </summary>
    /// <param name="source">The source list of data.</param>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    /// <returns>A list of TdiGmResult containing the TDI-GM values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static IReadOnlyList<TdiGmResult> ToTdiGm(
        this IReadOnlyList<IReusable> source,
        int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(rsiPeriod, bandLength, fastLength, slowLength);

        // initialize
        int length = source.Count;
        List<TdiGmResult> results = new(length);

        return results;
    }
}
