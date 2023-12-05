namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<DpoResult> CalcDpo(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Dpo.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        int offset = (lookbackPeriods / 2) + 1;
        List<SmaResult> sma = tpList.GetSma(lookbackPeriods).ToList();
        List<DpoResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            DpoResult r = new() { Date = date };
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
}
