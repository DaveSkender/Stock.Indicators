namespace Skender.Stock.Indicators;

// DONCHIAN CHANNEL (SERIES)
public static partial class Indicator
{
    internal static List<DonchianResult> CalcDonchian<TQuote>(
        this List<TQuote> quotesList,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateDonchian(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        List<DonchianResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];

            DonchianResult r = new(q.Date);
            results.Add(r);

            if (i >= lookbackPeriods)
            {
                decimal highHigh = 0;
                decimal lowLow = decimal.MaxValue;

                // high/low over prior periods
                for (int p = i - lookbackPeriods; p < i; p++)
                {
                    TQuote d = quotesList[p];

                    if (d.High > highHigh)
                    {
                        highHigh = d.High;
                    }

                    if (d.Low < lowLow)
                    {
                        lowLow = d.Low;
                    }
                }

                r.UpperBand = highHigh;
                r.LowerBand = lowLow;
                r.Centerline = (r.UpperBand + r.LowerBand) / 2m;
                r.Width = (r.Centerline == 0) ? null
                    : (r.UpperBand - r.LowerBand) / r.Centerline;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateDonchian(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Donchian Channel.");
        }
    }
}
