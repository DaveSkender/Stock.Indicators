namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (SERIES)

public static partial class Indicator
{
    // NOTE: sequence swapped from API
    internal static List<BetaResult> CalcBeta<T>(
        List<T> sourceEval,
        List<T> sourceMrkt,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where T : IReusable
    {
        // check parameter arguments
        Beta.Validate(sourceEval, sourceMrkt, lookbackPeriods);

        // initialize
        int length = sourceEval.Count;
        List<BetaResult> results = new(length);

        bool calcSd = type is BetaType.All or BetaType.Standard;
        bool calcUp = type is BetaType.All or BetaType.Up;
        bool calcDn = type is BetaType.All or BetaType.Down;

        // convert quotes to returns
        double[] evalReturns = new double[length];
        double[] mrktReturns = new double[length];
        double prevE = 0;
        double prevM = 0;

        for (int i = 0; i < length; i++)
        {
            T eval = sourceEval[i];
            T mrkt = sourceMrkt[i];

            if (eval.Timestamp != mrkt.Timestamp)
            {
                throw new InvalidQuotesException(nameof(sourceEval), eval.Timestamp,
                    "Timestamp sequence does not match.  Beta requires matching dates in provided quotes.");
            }

            evalReturns[i] = prevE != 0 ? (eval.Value / prevE) - 1d : 0;
            mrktReturns[i] = prevM != 0 ? (mrkt.Value / prevM) - 1d : 0;

            prevE = eval.Value;
            prevM = mrkt.Value;
        }

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T eval = sourceEval[i];

            // skip warmup periods
            if (i < lookbackPeriods)
            {
                results.Add(
                new BetaResult(
                Timestamp: eval.Timestamp,
                ReturnsEval: evalReturns[i],
                ReturnsMrkt: mrktReturns[i]));

                continue;
            }

            double? beta = null;
            double? betaUp = null;
            double? betaDown = null;
            double? ratio = null;
            double? convexity = null;

            // calculate beta variants
            if (calcSd)
            {
                beta = CalcBetaWindow(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Standard);
            }

            if (calcDn)
            {
                betaDown = CalcBetaWindow(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Down);
            }

            if (calcUp)
            {
                betaUp = CalcBetaWindow(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Up);
            }

            // ratio and convexity
            if (type == BetaType.All && betaUp != null && betaDown != null)
            {
                ratio = (betaDown != 0) ? betaUp / betaDown : null;
                convexity = (betaUp - betaDown) * (betaUp - betaDown);
            }

            results.Add(
            new BetaResult(
                Timestamp: eval.Timestamp,
                ReturnsEval: evalReturns[i],
                ReturnsMrkt: mrktReturns[i],
                Beta: beta,
                BetaUp: betaUp,
                BetaDown: betaDown,
                Ratio: ratio,
                Convexity: convexity));
        }

        return results;
    }

    // calculate beta
    private static double? CalcBetaWindow(
        int i,
        int lookbackPeriods,
        double[] mrktReturns,
        double[] evalReturns,
        BetaType type)
    {
        // note: BetaType.All is ineligible for this method

        // initialize
        double? beta = null;
        List<double> dataA = new(lookbackPeriods);
        List<double> dataB = new(lookbackPeriods);

        for (int p = i - lookbackPeriods + 1; p <= i; p++)
        {
            double a = mrktReturns[p];
            double b = evalReturns[p];

            if (type is BetaType.Standard
            || (type is BetaType.Down && a < 0)
            || (type is BetaType.Up && a > 0))
            {
                dataA.Add(a);
                dataB.Add(b);
            }
        }

        if (dataA.Count > 0)
        {
            // calculate correlation, covariance, and variance
            CorrResult c = PeriodCorrelation(
                default,
                [.. dataA],
                [.. dataB]);

            // calculate beta
            if (c.Covariance != null && c.VarianceA != null && c.VarianceA != 0)
            {
                beta = (c.Covariance / c.VarianceA).NaN2Null();
            }
        }

        return beta;
    }
}
