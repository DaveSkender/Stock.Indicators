namespace Skender.Stock.Indicators;

/// <summary>
/// Vortex indicator.
/// </summary>
public static partial class Vortex
{
    /// <summary>
    /// Calculates the Vortex indicator for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of VortexResult containing the Vortex indicator values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    public static IReadOnlyList<VortexResult> ToVortex(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => quotes
            .ToQuoteDList()
            .CalcVortex(lookbackPeriods);

    /// <summary>
    /// Calculates the Vortex indicator for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of VortexResult containing the Vortex indicator values.</returns>
    private static List<VortexResult> CalcVortex(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<VortexResult> results = new(length);

        double[] tr = new double[length];
        double[] pvm = new double[length];
        double[] nvm = new double[length];

        double prevHigh = 0;
        double prevLow = 0;
        double prevClose = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            // skip first period
            if (i == 0)
            {
                prevHigh = q.High;
                prevLow = q.Low;
                prevClose = q.Close;

                results.Add(new(q.Timestamp));
                continue;
            }

            // trend information
            double highMinusPrevClose = Math.Abs(q.High - prevClose);
            double lowMinusPrevClose = Math.Abs(q.Low - prevClose);

            tr[i] = Math.Max(q.High - q.Low, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            pvm[i] = Math.Abs(q.High - prevLow);
            nvm[i] = Math.Abs(q.Low - prevHigh);

            prevHigh = q.High;
            prevLow = q.Low;
            prevClose = q.Close;

            double pvi = double.NaN;
            double nvi = double.NaN;

            // vortex indicator
            if (i + 1 > lookbackPeriods)
            {
                double sumTr = 0;
                double sumPvm = 0;
                double sumNvm = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    sumTr += tr[p];
                    sumPvm += pvm[p];
                    sumNvm += nvm[p];
                }

                if (sumTr is not 0)
                {
                    pvi = sumPvm / sumTr;
                    nvi = sumNvm / sumTr;
                }
            }

            results.Add(new VortexResult(
                Timestamp: q.Timestamp,
                Pvi: pvi.NaN2Null(),
                Nvi: nvi.NaN2Null()));
        }

        return results;
    }
}
