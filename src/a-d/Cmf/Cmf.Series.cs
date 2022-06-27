namespace Skender.Stock.Indicators;

// CHAIKIN MONEY FLOW (SERIES)
public static partial class Indicator
{
    internal static List<CmfResult> CalcCmf(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // convert quotes
        List<(DateTime, double)> tpList = qdList.ToBasicTuple(CandlePart.Volume);

        // check parameter arguments
        ValidateCmf(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<CmfResult> results = new(length);
        List<AdlResult> adlResults = qdList.CalcAdl(null).ToList();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            AdlResult adl = adlResults[i];

            CmfResult r = new(adl.Date)
            {
                MoneyFlowMultiplier = adl.MoneyFlowMultiplier,
                MoneyFlowVolume = adl.MoneyFlowVolume
            };
            results.Add(r);

            if (i >= lookbackPeriods - 1)
            {
                double? sumMfv = 0;
                double? sumVol = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sumVol += pValue;

                    AdlResult d = adlResults[p];
                    sumMfv += d.MoneyFlowVolume;
                }

                double? avgMfv = sumMfv / lookbackPeriods;
                double? avgVol = sumVol / lookbackPeriods;

                if (avgVol != 0)
                {
                    r.Cmf = avgMfv / avgVol;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateCmf(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Chaikin Money Flow.");
        }
    }
}
