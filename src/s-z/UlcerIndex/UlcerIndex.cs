namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ULCER INDEX (UI)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ToBasicD(CandlePart.Close);

        // check parameter arguments
        ValidateUlcer(lookbackPeriods);

        // initialize
        List<UlcerIndexResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicD q = bdList[i];
            int index = i + 1;

            UlcerIndexResult result = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double? sumSquared = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    BasicD d = bdList[p];
                    int dIndex = p + 1;

                    double maxClose = 0;
                    for (int s = index - lookbackPeriods; s < dIndex; s++)
                    {
                        BasicD dd = bdList[s];
                        if (dd.Value > maxClose)
                        {
                            maxClose = dd.Value;
                        }
                    }

                    double? percentDrawdown = (maxClose == 0) ? null
                        : 100 * (double)((d.Value - maxClose) / maxClose);

                    sumSquared += percentDrawdown * percentDrawdown;
                }

                result.UI = (sumSquared == null) ? null
                    : Math.Sqrt((double)sumSquared / lookbackPeriods);
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateUlcer(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Ulcer Index.");
        }
    }
}
