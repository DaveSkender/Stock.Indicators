namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (SERIES)

public static partial class Dpo
{
    public static IReadOnlyList<DpoResult> ToDpo<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<DpoResult> results = new(length);

        int offset = (lookbackPeriods / 2) + 1;

        IReadOnlyList<SmaResult> sma
            = source.ToSma(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T src = source[i];

            double? dpoSma = null;
            double? dpoVal = null;

            if (i >= lookbackPeriods - offset - 1 && i < length - offset)
            {
                SmaResult s = sma[i + offset];
                dpoSma = s.Sma;
                dpoVal = s.Sma is null ? null : src.Value - s.Sma;
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
