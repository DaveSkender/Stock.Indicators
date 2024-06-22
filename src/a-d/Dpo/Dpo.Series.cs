namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<DpoResult> CalcDpo<T>(
        this List<T> tpList,
        int lookbackPeriods)
        where T : IReusableResult
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
            T src = tpList[i];

            double? dpoSma = default;
            double? dpoVal = default;

            if (i >= lookbackPeriods - offset - 1 && i < length - offset)
            {
                SmaResult s = sma[i + offset];
                dpoSma = s.Sma;
                dpoVal = s.Sma is null ? null : (src.Value - s.Sma);
            }

            DpoResult r = new(
                Timestamp: src.Timestamp,
                Dpo: dpoVal,
                Sma: dpoSma);

            results.Add(r);
        }

        return results;
    }
}
