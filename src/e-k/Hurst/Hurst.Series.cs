namespace Skender.Stock.Indicators;

// HURST EXPONENT (SERIES)
public static partial class Indicator
{
    internal static List<HurstResult> CalcHurst(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateHurst(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<HurstResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double _) = tpList[i];

            HurstResult r = new(date);
            results.Add(r);

            if (i + 1 > lookbackPeriods)
            {
                // get evaluation batch
                double[] values = new double[lookbackPeriods];

                int x = 0;
                double l = tpList[i - lookbackPeriods].Item2;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double c) = tpList[p];

                    // return values
                    values[x] = l != 0 ? (c / l) - 1 : double.NaN;

                    l = c;
                    x++;
                }

                // calculate hurst exponent
                r.HurstExponent = CalcHurstWindow(values).NaN2Null();
            }
        }

        return results;
    }

    private static double CalcHurstWindow(double[] values)
    {
        int totalSize = values.Length;
        int maxChunks = 0;
        int setQty = 0;

        // determine max chunk quantity so chunks are
        // not smaller than 8 observations
        // and not to exceed 32 total chunks
        for (int chunkQty = 1; chunkQty <= 32; chunkQty *= 2)
        {
            if (totalSize / chunkQty >= 8)
            {
                maxChunks = chunkQty;
                setQty++;
                continue;
            }
            else
            {
                break;
            }
        }

        // initialize result sets
        double[] logRs = new double[setQty];
        double[] logSize = new double[setQty];
        int setNum = 0;

        // roll through sets
        for (int chunkQty = 1; chunkQty <= maxChunks; chunkQty *= 2)
        {
            // initialize set and chunks
            int chunkSize = totalSize / chunkQty;
            double sumChunkRs = 0;

            // starting index position used to skip
            // observations to enforce same-sized chunks
            int startIndex = totalSize - (chunkSize * chunkQty);

            // analyze chunks in set
            for (int chunkNum = 1; chunkNum <= chunkQty; chunkNum++)
            {
                // chunk mean
                double sum = 0;
                for (int i = startIndex; i < startIndex + chunkSize; i++)
                {
                    sum += values[i];
                }

                double chunkMean = sum / chunkSize;

                // chunk mean diff
                double sumY = 0;
                double sumSq = 0;
                double maxY = values[startIndex] - chunkMean;
                double minY = values[startIndex] - chunkMean;
                for (int i = startIndex; i < startIndex + chunkSize; i++)
                {
                    double y = values[i] - chunkMean;
                    sumY += y;
                    minY = (sumY < minY) ? sumY : minY;
                    maxY = (sumY > maxY) ? sumY : maxY;

                    sumSq += y * y;
                }

                // chunk rescaled range
                double r = maxY - minY;
                double s = Math.Sqrt(sumSq / chunkSize);
                double rs = s != 0 ? r / s : 0;

                sumChunkRs += rs;

                // increment starting index
                startIndex += chunkSize;
            }

            // set results
            logSize[setNum] = Math.Log10(chunkSize);
            logRs[setNum] = Math.Log10(sumChunkRs / chunkQty);

            // increment set
            setNum++;
        }

        // hurst exponent
        // TODO: apply Anis-Lloyd corrected R/S Hurst?
        return Functions.Slope(logSize, logRs);
    }

    // parameter validation
    private static void ValidateHurst(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 100)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be at least 100 for Hurst Exponent.");
        }
    }
}
