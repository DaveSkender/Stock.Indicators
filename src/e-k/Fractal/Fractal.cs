namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // WILLIAMS FRACTAL
    /// <include file='./info.xml' path='indicator/type[@name="standard"]/*' />
    ///
    public static IEnumerable<FractalResult> GetFractal<TQuote>(
        this IEnumerable<TQuote> quotes,
        int windowSpan = 2,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote
    {
        return GetFractal(quotes, windowSpan, windowSpan, endType);
    }

    // more configurable version (undocumented)
    /// <include file='./info.xml' path='indicator/type[@name="alt"]/*' />
    ///
    public static IEnumerable<FractalResult> GetFractal<TQuote>(
        IEnumerable<TQuote> quotes,
        int leftSpan,
        int rightSpan,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateFractal(Math.Min(leftSpan, rightSpan));

        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // initialize
        List<FractalResult> results = new(quotesList.Count);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];
            int index = i + 1;

            FractalResult r = new()
            {
                Date = q.Date
            };

            if (index > leftSpan && index <= quotesList.Count - rightSpan)
            {
                bool isHigh = true;
                bool isLow = true;

                decimal evalHigh = (endType == EndType.Close) ?
                   q.Close : q.High;

                decimal evalLow = (endType == EndType.Close) ?
                    q.Close : q.Low;

                // compare today with wings
                for (int p = i - leftSpan; p <= i + rightSpan; p++)
                {
                    // skip center eval period
                    if (p == i)
                    {
                        continue;
                    }

                    // evaluate wing periods
                    TQuote wing = quotesList[p];

                    decimal wingHigh = (endType == EndType.Close) ?
                        wing.Close : wing.High;

                    decimal wingLow = (endType == EndType.Close) ?
                        wing.Close : wing.Low;

                    if (evalHigh <= wingHigh)
                    {
                        isHigh = false;
                    }

                    if (evalLow >= wingLow)
                    {
                        isLow = false;
                    }
                }

                // bearish signal
                if (isHigh)
                {
                    r.FractalBear = evalHigh;
                }

                // bullish signal
                if (isLow)
                {
                    r.FractalBull = evalLow;
                }
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateFractal(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for Fractal.");
        }
    }
}
