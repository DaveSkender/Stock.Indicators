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
                    values[x] = l != 0 ? Math.Log(c / l) : double.NaN;

                    l = c;
                    x++;
                }

                // calculate hurst exponent
                (double h, double hAl) = CalcHurstWindow(values);
                r.HurstExponent = h.NaN2Null();
                r.HurstExponentAL = hAl.NaN2Null();
            }
        }

        return results;
    }

    private static (double H, double HAL) CalcHurstWindow(double[] values)
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
        double[] logRsAL = new double[setQty];
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
                double maxY = double.MinValue;
                double minY = double.MaxValue;

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

            // average R/S for this chunk size
            double avgRs = sumChunkRs / chunkQty;

            // Anis-Lloyd finite-sample expected R/S
            double eRs = HurstExpectedRS(chunkSize);

            // asymptotic expected R/S: √(π×n/2)
            double eRsAsymptotic = Math.Sqrt(Math.PI * chunkSize / 2.0);

            // Anis-Lloyd corrected R/S:
            // subtract finite-sample bias, add back asymptotic expectation
            // RS_corrected = RS_observed − E[R/S]_AL + √(π×n/2)
            double rsAL = avgRs - eRs + eRsAsymptotic;

            // set results
            logSize[setNum] = Math.Log10(chunkSize);
            logRs[setNum] = Math.Log10(avgRs);

            // use NaN if corrected R/S is non-positive
            // (rare edge case: only occurs when avgRs is near zero, e.g. bad data)
            logRsAL[setNum] = rsAL > 0
                ? Math.Log10(rsAL)
                : double.NaN;

            // increment set
            setNum++;
        }

        // hurst exponents: raw and Anis-Lloyd corrected
        return (
            Numerix.Slope(logSize, logRs),
            Numerix.Slope(logSize, logRsAL));
    }

    // Anis-Lloyd expected R/S for a random series of length n
    // Reference: Anis and Lloyd (1976), Peters (1994)
    private static double HurstExpectedRS(int n)
    {
        // inner sum: Σ_{j=1}^{n-1} √((n-j)/j)
        double innerSum = 0;
        for (int j = 1; j < n; j++)
        {
            innerSum += Math.Sqrt((double)(n - j) / j);
        }

        double gammaFactor;
        if (n <= 340)
        {
            // exact: Γ((n-1)/2) / (√π × Γ(n/2))
            gammaFactor = Math.Exp(
                Numerix.LogGamma((n - 1.0) / 2.0)
                - 0.5 * Math.Log(Math.PI)
                - Numerix.LogGamma(n / 2.0));
        }
        else
        {
            // Stirling approximation: √(2 / (π × n))
            gammaFactor = Math.Sqrt(2.0 / (Math.PI * n));
        }

        return (n - 0.5) / n * gammaFactor * innerSum;
    }

    // parameter validation
    private static void ValidateHurst(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 20)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be at least 20 for Hurst Exponent.");
        }
    }
}
