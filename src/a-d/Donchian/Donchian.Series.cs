namespace Skender.Stock.Indicators;

// DONCHIAN CHANNEL (SERIES)
public static partial class Indicator
{
    internal static IEnumerable<DonchianResult> CalcDonchian<TQuote>(
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

            DonchianResult result = new()
            {
                Date = q.Date
            };

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

                result.UpperBand = highHigh;
                result.LowerBand = lowLow;
                result.Centerline = (result.UpperBand + result.LowerBand) / 2m;
                result.Width = (result.Centerline == 0) ? null
                    : (result.UpperBand - result.LowerBand) / result.Centerline;
            }

            results.Add(result);
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
