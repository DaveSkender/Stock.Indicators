namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for STARC Band indicator calculations.
/// </summary>
public static partial class StarcBands
{
    /// <summary>
    /// Converts a series of quotes to STARC Bands.
    /// </summary>
    /// <param name="quotes">The source series of quotes.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <returns>A list of <see cref="StarcBandsResult"/> containing the STARC Bands values.</returns>
    public static IReadOnlyList<StarcBandsResult> ToStarcBands(
        this IReadOnlyList<IQuote> quotes,
        int smaPeriods = 5,
        double multiplier = 2,
        int atrPeriods = 10)
        => quotes
            .ToQuoteDList()
            .CalcStarcBands(smaPeriods, multiplier, atrPeriods);

    /// <summary>
    /// Calculates the STARC Bands for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <returns>A list of <see cref="StarcBandsResult"/> containing the STARC Bands values.</returns>
    private static List<StarcBandsResult> CalcStarcBands(
        this List<QuoteD> quotes,
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        Validate(smaPeriods, multiplier, atrPeriods);

        // initialize
        int length = quotes.Count;
        List<StarcBandsResult> results = new(length);
        List<AtrResult> atrResults = quotes.CalcAtr(atrPeriods);
        IReadOnlyList<SmaResult> smaResults = quotes.ToSma(smaPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            SmaResult s = smaResults[i];
            AtrResult a = atrResults[i];

            results.Add(new(
                Timestamp: s.Timestamp,
                UpperBand: s.Sma + (multiplier * a.Atr),
                Centerline: s.Sma,
                LowerBand: s.Sma - (multiplier * a.Atr)));
        }

        return results;
    }
}
