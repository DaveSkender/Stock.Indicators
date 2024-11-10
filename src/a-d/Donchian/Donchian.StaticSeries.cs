namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for calculating Donchian Channels.
/// </summary>
public static partial class Donchian
{
    /// <summary>
    /// Converts a list of quotes to Donchian Channel results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of Donchian Channel results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<DonchianResult> ToDonchian<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
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
            TQuote q = quotes[i];

            if (i >= lookbackPeriods)
            {
                decimal highHigh = 0;
                decimal lowLow = decimal.MaxValue;

                // high/low over prior periods
                for (int p = i - lookbackPeriods; p < i; p++)
                {
                    TQuote d = quotes[p];

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
                    LowerBand: l,
                    Centerline: c,
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
