namespace Skender.Stock.Indicators;

// CHAIKIN MONEY FLOW (SERIES)

public static partial class Indicator
{
    private static List<CmfResult> CalcCmf<TQuote>(
        this List<TQuote> source,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // get volume array
        double[] volume
            = source.Select(v => (double)v.Volume).ToArray();

        // check parameter arguments
        Cmf.Validate(lookbackPeriods);

        // initialize
        int length = volume.Length;
        List<CmfResult> results = new(length);
        List<AdlResult> adlResults = source.CalcAdl();

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
