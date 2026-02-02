namespace Skender.Stock.Indicators;

/// <summary>
/// Hurst Exponent indicator.
/// </summary>
public static partial class Hurst
{
    /// <summary>
    /// Converts a list of time-series values to Hurst Exponent results.
    /// </summary>
    /// <param name="source">The list of time-series values to transform.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of Hurst Exponent results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 1.</exception>
    public static IReadOnlyList<HurstResult> ToHurst(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 100)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<HurstResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = source[i];
            double? h = null;

            if (i + 1 > lookbackPeriods)
            {
                // get evaluation batch
                double[] values = new double[lookbackPeriods];

                int x = 0;
                double l = source[i - lookbackPeriods].Value;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    IReusable ps = source[p];

                    // return values
                    values[x] = l != 0 ? (ps.Value / l) - 1 : double.NaN;

                    l = ps.Value;
                    x++;
                }

                // calculate hurst exponent
                h = CalcHurstWindow(values).NaN2Null();
            }

            results.Add(new(
                Timestamp: s.Timestamp,
                HurstExponent: h));
        }

        return results;
    }

    /// <summary>
    /// Calculates the Hurst Exponent for a given window of values.
    /// </summary>
    /// <param name="values">The array of values to evaluate.</param>
    /// <returns>The calculated Hurst Exponent.</returns>
    internal static double CalcHurstWindow(double[] values)
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
                    minY = sumY < minY ? sumY : minY;
                    maxY = sumY > maxY ? sumY : maxY;

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
            logSize[setNum] = DeMath.Log10(chunkSize);
            logRs[setNum] = DeMath.Log10(sumChunkRs / chunkQty);

            // increment set
            setNum++;
        }

        // hurst exponent
        // TODO: apply Anis-Lloyd corrected R/S Hurst?
        return Numerical.Slope(logSize, logRs);
    }
}
