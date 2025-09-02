namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for STARC Band indicator calculations.
/// </summary>
public static partial class StarcBands
{
    /// <summary>
    /// Converts a series of quotes to STARC Bands.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source series of quotes.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <returns>A list of <see cref="StarcBandsResult"/> containing the STARC Bands values.</returns>
    public static IReadOnlyList<StarcBandsResult> ToStarcBands<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int smaPeriods,
        double multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcStarcBands(smaPeriods, multiplier, atrPeriods);

    /// <summary>
    /// Calculates the STARC Bands for a series of quotes.
    /// </summary>
    /// <param name="source">The source series of quotes.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <returns>A list of <see cref="StarcBandsResult"/> containing the STARC Bands values.</returns>
    private static List<StarcBandsResult> CalcStarcBands(
        this List<QuoteD> source,
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        Validate(smaPeriods, multiplier, atrPeriods);

        // initialize
        int length = source.Count;
        List<StarcBandsResult> results = new(length);
        List<AtrResult> atrResults = source.CalcAtr(atrPeriods);
        IReadOnlyList<SmaResult> smaResults = source.ToSma(smaPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            SmaResult s = smaResults[i];
            AtrResult a = atrResults[i];

            results.Add(new(
                Timestamp: s.Timestamp,
                Centerline: s.Sma,
                UpperBand: s.Sma + (multiplier * a.Atr),
                LowerBand: s.Sma - (multiplier * a.Atr)));
        }

        return results;
    }
}
