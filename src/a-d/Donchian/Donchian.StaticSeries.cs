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
                decimal highHigh = 0;
                decimal lowLow = decimal.MaxValue;

                // high/low over prior periods
                for (int p = i - lookbackPeriods; p < i; p++)
                {
                    IQuote d = quotes[p];

                    if (d.High > highHigh)
                    {
                        highHigh = d.High;
                    }

                    if (d.Low < lowLow)
                    {
                        lowLow = d.Low;
                    }
                }

                decimal u = highHigh;
                decimal l = lowLow;
                decimal c = (u + l) / 2m;

                results.Add(new DonchianResult(
                    Timestamp: q.Timestamp,
                    UpperBand: u,
                    Centerline: c,
                    LowerBand: l,
                    Width: c == 0 ? null : (u - l) / c));
            }
            else
            {
                results.Add(new(q.Timestamp));
            }
        }

        return results;
    }
}
