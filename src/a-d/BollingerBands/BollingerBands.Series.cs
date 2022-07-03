namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (SERIES)
public static partial class Indicator
{
    internal static List<BollingerBandsResult> CalcBollingerBands(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double standardDeviations)
    {
        // check parameter arguments
        ValidateBollingerBands(lookbackPeriods, standardDeviations);

        // initialize
        List<BollingerBandsResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double value) = tpList[i];

            BollingerBandsResult r = new(date);
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double[] window = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    window[n] = pValue;
                    sum += pValue;
                    n++;
                }

                double? periodAvg = (sum / lookbackPeriods).NaN2Null();
                double? stdDev = Functions.StdDev(window).NaN2Null();

                r.Sma = periodAvg;
                r.UpperBand = periodAvg + (standardDeviations * stdDev);
                r.LowerBand = periodAvg - (standardDeviations * stdDev);

                r.PercentB = (r.UpperBand == r.LowerBand) ? null
                    : (value - r.LowerBand) / (r.UpperBand - r.LowerBand);

                r.ZScore = (stdDev == 0) ? null : (value - r.Sma) / stdDev;
                r.Width = (periodAvg == 0) ? null : (r.UpperBand - r.LowerBand) / periodAvg;
            }
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
