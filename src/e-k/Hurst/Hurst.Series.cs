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

        // precompute Anis-Lloyd bias corrections per chunk size:
        // these depend only on lookbackPeriods, not on the data, so
        // they are constant across every result index.
        double[] alCorrections = PrecomputeAlCorrections(lookbackPeriods);

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

                    // log returns require strictly positive prices on both
                    // ends; non-positive inputs propagate as NaN
                    values[x] = (l > 0 && c > 0) ? Math.Log(c / l) : double.NaN;

                    l = c;
                    x++;
                }

                // calculate hurst exponent
                (double h, double hAl) = CalcHurstWindow(values, alCorrections);
                r.HurstExponent = h.NaN2Null();
                r.HurstExponentAL = hAl.NaN2Null();
            }
        }

        return results;
    }

    private static (double H, double HAL) CalcHurstWindow(
        double[] values,
        double[] alCorrections)
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

            // Anis-Lloyd corrected R/S:
            // RS_corrected = avgRs - E[R/S]_AL + √(π·n/2)
            // The bias-correction term (√(π·n/2) - E[R/S]_AL) is precomputed
            // per chunk size since it depends only on n.
            double rsAL = avgRs + alCorrections[setNum];

            // set results
            logSize[setNum] = Math.Log10(chunkSize);
            logRs[setNum] = Math.Log10(avgRs);

            // NaN-guards: rsAL is non-negative for any finite avgRs (the
            // bias-correction term is positive across all chunk sizes used),
            // so this branch fires only when avgRs is NaN.
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

    // Precomputes the Anis-Lloyd bias-correction term (√(π·n/2) - E[R/S]_AL)
    // for each chunk size derived from totalSize. The chunk sizes and the
    // correction depend only on lookbackPeriods, not on the data, so we do
    // this once per indicator call rather than per result index.
    private static double[] PrecomputeAlCorrections(int totalSize)
    {
        int setQty = 0;
        int maxChunks = 0;
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

        double[] corrections = new double[setQty];
        int setIdx = 0;
        for (int chunkQty = 1; chunkQty <= maxChunks; chunkQty *= 2)
        {
            int chunkSize = totalSize / chunkQty;
            double eRsAsymptotic = Math.Sqrt(Math.PI * chunkSize / 2.0);
            corrections[setIdx++] = eRsAsymptotic - HurstExpectedRS(chunkSize);
        }

        return corrections;
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

        // Branch threshold n=340: below this, LogGamma is fast and exact;
        // above, the Stirling approximation introduces <0.05% error and
        // avoids LogGamma overflow risk for very large n.
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
            // Stirling Variant A (Peters 1994): √(2 / (π × (n-1)))
            // More accurate than (n-0.5)/n · √(2/(π·n)) — 5× lower error
            // vs exact Γ((n-1)/2)/(√π·Γ(n/2)) across all n > 340.
            gammaFactor = Math.Sqrt(2.0 / (Math.PI * (n - 1.0)));
        }

        return gammaFactor * innerSum;
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
