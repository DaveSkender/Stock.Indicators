namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CHAIKIN MONEY FLOW
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<CmfResult> GetCmf<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.Volume);

        // check parameter arguments
        ValidateCmf(lookbackPeriods);

        // initialize
        List<CmfResult> results = new(bdList.Count);
        List<AdlResult> adlResults = GetAdl(quotes).ToList();

        // roll through quotes
        for (int i = 0; i < adlResults.Count; i++)
        {
            AdlResult r = adlResults[i];

            CmfResult result = new()
            {
                Date = r.Date,
                MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                MoneyFlowVolume = r.MoneyFlowVolume
            };

            if (i + 1 >= lookbackPeriods)
            {
                double? sumMfv = 0;
                double? sumVol = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    BasicData q = bdList[p];
                    sumVol += q.Value;

                    AdlResult d = adlResults[p];
                    sumMfv += (double)d.MoneyFlowVolume;
                }

                double? avgMfv = sumMfv / lookbackPeriods;
                double? avgVol = sumVol / lookbackPeriods;

                if (avgVol != 0)
                {
                    result.Cmf = avgMfv / avgVol;
                }
            }

            results.Add(result);
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
