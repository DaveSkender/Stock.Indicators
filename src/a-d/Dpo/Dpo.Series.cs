using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static Collection<DpoResult> CalcDpo(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateDpo(lookbackPeriods);

        // initialize
        int length = tpColl.Count;
        int offset = (lookbackPeriods / 2) + 1;
        List<SmaResult> sma = tpColl.GetSma(lookbackPeriods).ToList();
        Collection<DpoResult> results = new();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpColl[i];

            DpoResult r = new(date);
            results.Add(r);

            if (i >= lookbackPeriods - offset - 1 && i < length - offset)
            {
                SmaResult s = sma[i + offset];
                r.Sma = s.Sma;
                r.Dpo = s.Sma is null ? null : (value - s.Sma).NaN2Null();
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateDpo(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DPO.");
        }
    }
}
