namespace Skender.Stock.Indicators;

// DONCHIAN CHANNEL (UTILITIES)

public static partial class Donchian
{
    // remove empty (null) periods
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DonchianResult> Condense(
        this IReadOnlyList<DonchianResult> results)
    {
        List<DonchianResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DonchianResult> RemoveWarmupPeriods(
        this IReadOnlyList<DonchianResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Donchian Channel.");
        }
    }
}
