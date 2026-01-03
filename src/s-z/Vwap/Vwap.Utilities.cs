namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the VWAP (Volume Weighted Average Price) indicator.
/// </summary>
public static partial class Vwap
{
    // parameter validation
    /// <summary>
    /// Validates the parameters for the VWAP calculation.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="startDate">The optional start date for the VWAP calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the start date is earlier than the first quote's timestamp.
    /// </exception>
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
