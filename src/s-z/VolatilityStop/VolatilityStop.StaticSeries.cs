namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Volatility Stop indicator.
/// </summary>
public static partial class VolatilityStop
{
    /// <summary>
    /// Calculates the Volatility Stop for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the Average True Range. Default is 3.</param>
    /// <returns>A list of VolatilityStopResult containing the Volatility Stop values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<VolatilityStopResult> ToVolatilityStop(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 7,
        double multiplier = 3)
        => quotes
            .ToQuoteDList()
            .CalcVolatilityStop(lookbackPeriods, multiplier);

    /// <summary>
    /// Calculates the Volatility Stop for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the Average True Range.</param>
    /// <returns>A list of VolatilityStopResult containing the Volatility Stop values.</returns>
    private static List<VolatilityStopResult> CalcVolatilityStop(
        this List<QuoteD> quotes,
        int lookbackPeriods,
        double multiplier)
    {
        //convert quotes
        List<IReusable> reList = quotes
            .Cast<IReusable>()
            .ToList();

        // check parameter arguments
        Validate(lookbackPeriods, multiplier);

        // initialize
        int length = quotes.Count;
        List<VolatilityStopResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        List<AtrResult> atrList = quotes.CalcAtr(lookbackPeriods);

        // initial trend (guess)
        int initPeriods = Math.Min(length, lookbackPeriods);
        double sic = reList[0].Value;
        bool isLong = reList[initPeriods - 1].Value > sic;

        for (int i = 0; i < initPeriods; i++)
        {
            IReusable init = reList[i];
            sic = isLong ? Math.Max(sic, init.Value) : Math.Min(sic, init.Value);
            results.Add(new(init.Timestamp));
        }

        // roll through source values
        for (int i = lookbackPeriods; i < length; i++)
        {
            IReusable s = reList[i];

            // average true range × multiplier constant
            double? arc = atrList[i - 1].Atr * multiplier;

            // stop and reverse threshold
            double? sar = isLong ? sic - arc : sic + arc;

            // add SAR as separate bands
            double? lowerBand = null;
            double? upperBand = null;

            if (isLong)
            {
                lowerBand = sar;
            }
            else
            {
                upperBand = sar;
            }

            // evaluate stop and reverse
            bool? isStop;

            if ((isLong && s.Value < sar)
            || (!isLong && s.Value > sar))
            {
                isStop = true;
                sic = s.Value;
                isLong = !isLong;
            }
            else
            {
                isStop = false;

                // significant close adjustment
                // extreme favorable close (value) while in trade
                sic = isLong ? Math.Max(sic, s.Value) : Math.Min(sic, s.Value);
            }

            results.Add(new VolatilityStopResult(
                Timestamp: s.Timestamp,
                Sar: sar,
                IsStop: isStop,
                UpperBand: upperBand,
                LowerBand: lowerBand));
        }

        // remove trend to first stop, since it is a guess
        int cutIndex = results.FindIndex(static x => x.IsStop ?? false);

        for (int d = 0; d <= cutIndex; d++)
        {
            VolatilityStopResult r = results[d];

            results[d] = r with {
                Sar = null,
                UpperBand = null,
                LowerBand = null,
                IsStop = null
            };
        }

        return results;
    }
}
