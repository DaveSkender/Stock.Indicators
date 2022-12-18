namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (SERIES)
public static partial class Indicator
{
    // NOTE: sequence swapped from API
    internal static List<BetaResult> CalcBeta(
        List<(DateTime, double)> tpListEval,
        List<(DateTime, double)> tpListMrkt,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
    {
        // check parameter arguments
        ValidateBeta(tpListEval, tpListMrkt, lookbackPeriods);

        // initialize
        int length = tpListEval.Count;
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
            (DateTime eDate, double eValue) = tpListEval[i];
            (DateTime mDate, double mValue) = tpListMrkt[i];

            if (eDate != mDate)
            {
                throw new InvalidQuotesException(nameof(tpListEval), eDate,
                    "Date sequence does not match.  Beta requires matching dates in provided quotes.");
            }

            evalReturns[i] = prevE != 0 ? (eValue / prevE) - 1d : 0;
            mrktReturns[i] = prevM != 0 ? (mValue / prevM) - 1d : 0;

            prevE = eValue;
            prevM = mValue;
        }

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double _) = tpListEval[i];

            BetaResult r = new(date)
            {
                ReturnsEval = evalReturns[i],
                ReturnsMrkt = mrktReturns[i]
            };
            results.Add(r);

            // skip warmup periods
            if (i < lookbackPeriods)
            {
                continue;
            }

            // calculate beta variants
            if (calcSd)
            {
                r.CalcBetaWindow(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Standard);
            }

            if (calcDn)
            {
                r.CalcBetaWindow(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Down);
            }

            if (calcUp)
            {
                r.CalcBetaWindow(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Up);
            }

            // ratio and convexity
            if (type == BetaType.All && r.BetaUp != null && r.BetaDown != null)
            {
                r.Ratio = (r.BetaDown != 0) ? r.BetaUp / r.BetaDown : null;
                r.Convexity = (r.BetaUp - r.BetaDown) * (r.BetaUp - r.BetaDown);
            }
        }

        return results;
    }

    // calculate beta
    private static void CalcBetaWindow(
        this BetaResult r,
        int i,
        int lookbackPeriods,
        double[] mrktReturns,
        double[] evalReturns,
        BetaType type)
    {
        // note: BetaType.All is ineligible for this method

        // initialize
        CorrResult c = new(r.Date);

        List<double> dataA = new(lookbackPeriods);
        List<double> dataB = new(lookbackPeriods);

        for (int p = i - lookbackPeriods + 1; p <= i; p++)
        {
            double a = mrktReturns[p];
            double b = evalReturns[p];

            if (type is BetaType.Standard)
            {
                dataA.Add(a);
                dataB.Add(b);
            }
            else if (type is BetaType.Down && a < 0)
            {
                dataA.Add(a);
                dataB.Add(b);
            }
            else if (type is BetaType.Up && a > 0)
            {
                dataA.Add(a);
                dataB.Add(b);
            }
        }

        if (dataA.Count > 0)
        {
            // calculate correlation, covariance, and variance
            c.PeriodCorrelation(dataA.ToArray(), dataB.ToArray());

            // calculate beta
            if (c.Covariance != null && c.VarianceA != null && c.VarianceA != 0)
            {
                double? beta = (c.Covariance / c.VarianceA).NaN2Null();

                if (type == BetaType.Standard)
                {
                    r.Beta = beta;
                }
                else if (type == BetaType.Down)
                {
                    r.BetaDown = beta;
                }
                else if (type == BetaType.Up)
                {
                    r.BetaUp = beta;
                }
            }
        }
    }

    // parameter validation
    private static void ValidateBeta(
        List<(DateTime, double)> tpListEval,
        List<(DateTime, double)> tpListMrkt,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        // check quotes
        if (tpListEval.Count != tpListMrkt.Count)
        {
            throw new InvalidQuotesException(
                nameof(tpListEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }
    }
}
