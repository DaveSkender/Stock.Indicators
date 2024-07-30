namespace Skender.Stock.Indicators;

public static partial class Sma
{
    /// <summary>
    /// Simple moving average calculation
    /// </summary>
    /// <param name="values">List of chainable values</param>
    /// <param name="lookbackPeriods">
    /// Window to evaluate, prior to 'endIndex'
    /// </param>
    /// <param name="endIndex">
    /// Index position to evaluate or last position when <see langword="null"/>.
    /// </param>
    /// <typeparam name="T">IReusable (chainable) type</typeparam>
    /// <returns>
    /// Simple moving average or <see langword="null"/>
    /// if incalculable <see langword="double.NaN"/>
    /// values are in range.
    /// </returns>
    internal static double? Average<T>(
        this IReadOnlyList<T> values,
        int lookbackPeriods,
        int? endIndex = null)
        where T : IReusable

        // TODO: unused SMA utility, either make public or remove

        => Increment(
            values,
            lookbackPeriods,
            endIndex ?? values.Count - 1)
           .NaN2Null();

    /// <summary>
    /// Simple moving average calculation
    /// </summary>
    /// <param name="values">List of chainable values</param>
    /// <param name="lookbackPeriods">Window to evaluate, prior to 'endIndex'</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <typeparam name="T">IReusable (chainable) type</typeparam>
    /// <returns>
    /// Simple moving average or <see langword="double.NaN"/>
    /// when incalculable.
    /// </returns>
    internal static double Increment<T>(
        IReadOnlyList<T> values,
        int lookbackPeriods,
        int endIndex)
        where T : IReusable
    {
        int offset = lookbackPeriods - 1;

        if (endIndex < offset || endIndex >= values.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        int startIndex = endIndex - offset;

        for (int i = startIndex; i <= endIndex; i++)
        {
            sum += values[i].Value;
        }

        return sum / lookbackPeriods;

        // TODO: apply this SMA increment method more widely in other indicators (see EMA example)
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
