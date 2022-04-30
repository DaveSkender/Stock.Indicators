namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BOLLINGER BANDS
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<BollingerBandsResult> GetBollingerBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicData(CandlePart.Close);

        // check parameter arguments
        ValidateBollingerBands(lookbackPeriods, standardDeviations);

        // initialize
        List<BollingerBandsResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicData q = bdList[i];
            decimal close = (decimal)q.Value;

            BollingerBandsResult r = new()
            {
                Date = q.Date
            };

            if (i + 1 >= lookbackPeriods)
            {
                double[] periodClose = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    BasicData d = bdList[p];
                    periodClose[n] = d.Value;
                    sum += d.Value;
                    n++;
                }

                double periodAvg = sum / lookbackPeriods;
                double stdDev = Functions.StdDev(periodClose);

                r.Sma = (decimal)periodAvg;
                r.UpperBand = (decimal)(periodAvg + (standardDeviations * stdDev));
                r.LowerBand = (decimal)(periodAvg - (standardDeviations * stdDev));

                r.PercentB = (r.UpperBand == r.LowerBand) ? null
                    : (double)((close - r.LowerBand) / (r.UpperBand - r.LowerBand));

                r.ZScore = (stdDev == 0) ? null : (double)(close - r.Sma) / stdDev;
                r.Width = (periodAvg == 0) ? null : (double)(r.UpperBand - r.LowerBand) / periodAvg;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateBollingerBands(
        int lookbackPeriods,
        double standardDeviations)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Bollinger Bands.");
        }

        if (standardDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                "Standard Deviations must be greater than 0 for Bollinger Bands.");
        }
    }
}
