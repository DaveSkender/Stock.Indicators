namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BETA COEFFICIENT
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<BetaResult> GetBeta<TQuote>(
        IEnumerable<TQuote> quotesMarket,
        IEnumerable<TQuote> quotesEval,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdListEval = quotesEval.ConvertToBasic(CandlePart.Close);
        List<BasicD> bdListMrkt = quotesMarket.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateBeta(quotesMarket, quotesEval, lookbackPeriods);

        // initialize
        List<BetaResult> results = new(bdListEval.Count);
        bool calcSd = type is BetaType.All or BetaType.Standard;
        bool calcUp = type is BetaType.All or BetaType.Up;
        bool calcDn = type is BetaType.All or BetaType.Down;

        // roll through quotes
        for (int i = 0; i < bdListEval.Count; i++)
        {
            BasicD e = bdListEval[i];

            BetaResult r = new()
            {
                Date = e.Date
            };
            results.Add(r);

            // skip warmup periods
            if (i < lookbackPeriods - 1)
            {
                continue;
            }

            // calculate standard beta
            if (calcSd)
            {
                r.CalcBeta(
                i, lookbackPeriods, bdListMrkt, bdListEval, BetaType.Standard);
            }

            // calculate up/down betas
            if (i >= lookbackPeriods)
            {
                if (calcDn)
                {
                    r.CalcBeta(
                    i, lookbackPeriods, bdListMrkt, bdListEval, BetaType.Down);
                }

                if (calcUp)
                {
                    r.CalcBeta(
                    i, lookbackPeriods, bdListMrkt, bdListEval, BetaType.Up);
                }
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

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<BetaResult> RemoveWarmupPeriods(
        this IEnumerable<BetaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Beta != null);

        return results.Remove(removePeriods);
    }

    // calculate beta
    private static void CalcBeta(
        this BetaResult r,
        int index,
        int lookbackPeriods,
        List<BasicD> bdListMrkt,
        List<BasicD> bdListEval,
        BetaType type)
    {
        // note: BetaType.All is ineligible for this method

        // initialize
        CorrResult c = new();

        List<double> dataA = new(lookbackPeriods);
        List<double> dataB = new(lookbackPeriods);

        for (int p = index - lookbackPeriods + 1; p <= index; p++)
        {
            double a = bdListMrkt[p].Value;
            double b = bdListEval[p].Value;

            if (type is BetaType.Standard)
            {
                dataA.Add(a);
                dataB.Add(b);
            }
            else if (type is BetaType.Down
                && a < bdListMrkt[p - 1].Value)
            {
                dataA.Add(a);
                dataB.Add(b);
            }
            else if (type is BetaType.Up
                && a > bdListMrkt[p - 1].Value)
            {
                dataA.Add(a);
                dataB.Add(b);
            }
        }

        if (dataA.Count > 0)
        {
            // calculate correlation, covariance, and variance
            c.CalcCorrelation(dataA.ToArray(), dataB.ToArray());

            // calculate beta
            if (c.Covariance != null && c.VarianceA != null && c.VarianceA != 0)
            {
                double? beta = c.Covariance / c.VarianceA;

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
    private static void ValidateBeta<TQuote>(
    IEnumerable<TQuote> quotesMarket,
    IEnumerable<TQuote> quotesEval,
    int lookbackPeriods)
    where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        // check quotes
        if (quotesEval.Count() < quotesMarket.Count())
        {
            throw new InvalidQuotesException(
                nameof(quotesEval),
                "Eval quotes should have at least as many records as Market quotes for Beta.");
        }
    }
}
