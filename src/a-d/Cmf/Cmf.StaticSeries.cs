namespace Skender.Stock.Indicators;

// CHAIKIN MONEY FLOW (SERIES)

public static partial class Cmf
{
    public static IReadOnlyList<CmfResult> ToCmf<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcCmf(lookbackPeriods);

    private static List<CmfResult> CalcCmf<TQuote>(
        this IReadOnlyList<TQuote> source,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // get volume array
        double[] volume
            = source.Select(v => (double)v.Volume).ToArray();

        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = volume.Length;
        List<CmfResult> results = new(length);
        IReadOnlyList<AdlResult> adlResults = source.ToAdl();

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            AdlResult adl = adlResults[i];
            double? cmf = null;

            if (i >= lookbackPeriods - 1)
            {
                double? sumMfv = 0;
                double? sumVol = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    sumVol += volume[p];

                    AdlResult d = adlResults[p];
                    sumMfv += d.MoneyFlowVolume;
                }

                double? avgMfv = sumMfv / lookbackPeriods;
                double? avgVol = sumVol / lookbackPeriods;

                if (avgVol != 0)
                {
                    cmf = avgMfv / avgVol;
                }
            }

            results.Add(new(
                Timestamp: adl.Timestamp,
                MoneyFlowMultiplier: adl.MoneyFlowMultiplier,
                MoneyFlowVolume: adl.MoneyFlowVolume,
                Cmf: cmf));
        }

        return results;
    }
}