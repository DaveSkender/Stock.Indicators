namespace Skender.Stock.Indicators;

// KELTNER CHANNELS (UTILITIES)

public static partial class Keltner
{
    // remove empty (null) periods
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<KeltnerResult> Condense(
        this IReadOnlyList<KeltnerResult> results)
    {
        List<KeltnerResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<KeltnerResult> RemoveWarmupPeriods(
        this IReadOnlyList<KeltnerResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Width != null) + 1;

        return results.Remove(Math.Max(2 * n, n + 100));
    }

    // parameter validation
    internal static void Validate(
        int emaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        if (emaPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(emaPeriods), emaPeriods,
                "EMA periods must be greater than 1 for Keltner Channel.");
        }

        if (atrPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                "ATR periods must be greater than 1 for Keltner Channel.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for Keltner Channel.");
        }
    }
}
