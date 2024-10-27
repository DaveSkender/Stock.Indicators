namespace Skender.Stock.Indicators;

// DONCHIAN CHANNEL (SERIES)

public static partial class Donchian
{
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
