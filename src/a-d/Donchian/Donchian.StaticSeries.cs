namespace Skender.Stock.Indicators;

/// <summary>
/// Donchian Channels indicator.
/// </summary>
public static partial class Donchian
{
    /// <summary>
    /// Converts a list of quotes to Donchian Channel results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of Donchian Channel results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static IReadOnlyList<DonchianResult> ToDonchian(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<DonchianResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IQuote q = quotes[i];

            if (i >= lookbackPeriods)
            {
                double highHigh = 0;
                double lowLow = double.MaxValue;

                // high/low over prior periods
                for (int p = i - lookbackPeriods; p < i; p++)
                {
                    IQuote d = quotes[p];

                    if ((double)d.High > highHigh)
                    {
                        highHigh = (double)d.High;
                    }

                    if ((double)d.Low < lowLow)
                    {
                        lowLow = (double)d.Low;
                    }
                }

                double u = highHigh;
                double l = lowLow;
                double c = (u + l) / 2d;

                results.Add(new DonchianResult(
                    Timestamp: q.Timestamp,
                    UpperBand: u,
                    Centerline: c,
                    LowerBand: l,
                    Width: c == 0 ? null : (double?)((u - l) / c)));
            }
            else
            {
                results.Add(new(q.Timestamp));
            }
        }

        return results;
    }
}
