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
        List<TQuote>? evalQuotesList = quotesEval.SortToList();
        List<TQuote>? mrktQuotesList = quotesMarket.SortToList();

        // check parameter arguments
        ValidateBeta(mrktQuotesList, evalQuotesList, lookbackPeriods);

        // initialize
        int size = evalQuotesList.Count;
        List<BetaResult> results = new(size);

        bool calcSd = type is BetaType.All or BetaType.Standard;
        bool calcUp = type is BetaType.All or BetaType.Up;
        bool calcDn = type is BetaType.All or BetaType.Down;

        // convert quotes to returns
        double?[] evalReturns = new double?[size];
        double?[] mrktReturns = new double?[size];
        decimal? prevE = 0;
        decimal? prevM = 0;

        for (int i = 0; i < size; i++)
        {
            TQuote? e = evalQuotesList[i];
            TQuote? m = mrktQuotesList[i];

            if (e.Date != m.Date)
            {
                throw new InvalidQuotesException(nameof(quotesEval), e.Date,
                    "Date sequence does not match.  Beta requires matching dates in provided quotes.");
            }

            evalReturns[i] = (double?)(prevE != 0 ? (e.Close / prevE) - 1m : 0);
            mrktReturns[i] = (double?)(prevM != 0 ? (m.Close / prevM) - 1m : 0);

            prevE = e.Close;
            prevM = m.Close;
        }

        // roll through quotes
        for (int i = 0; i < size; i++)
        {
            TQuote? q = evalQuotesList[i];

            BetaResult r = new()
            {
                Date = q.Date,
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
                r.CalcBeta(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Standard);
            }

            if (calcDn)
            {
                r.CalcBeta(
                i, lookbackPeriods, mrktReturns, evalReturns, BetaType.Down);
            }

            if (calcUp)
            {
                r.CalcBeta(
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
    private static void CalcBeta(
        this BetaResult r,
        int i,
        int lookbackPeriods,
        double?[] mrktReturns,
        double?[] evalReturns,
        BetaType type)
    {
        // note: BetaType.All is ineligible for this method

        // initialize
        CorrResult c = new();

        List<double?> dataA = new(lookbackPeriods);
        List<double?> dataB = new(lookbackPeriods);

        for (int p = i - lookbackPeriods + 1; p <= i; p++)
        {
            double? a = mrktReturns[p];
            double? b = evalReturns[p];

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
        List<TQuote> quotesMarket,
        List<TQuote> quotesEval,
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
        if (quotesEval.Count != quotesMarket.Count)
        {
            throw new InvalidQuotesException(
                nameof(quotesEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }
    }
}
