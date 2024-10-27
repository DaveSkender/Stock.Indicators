namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (UTILITIES)

public static partial class Vwap
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VwapResult> RemoveWarmupPeriods(
        this IReadOnlyList<VwapResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwap != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        IReadOnlyList<QuoteD> quotes,
        DateTime? startDate)
    {
        // nothing to do for 0 length
        if (quotes.Count == 0)
        {
            return;
        }

        // check parameter arguments (intentionally after quotes check)
        if (startDate < quotes[0].Timestamp)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                "Start Timestamp must be within the quotes range for VWAP.");
        }
    }
}
