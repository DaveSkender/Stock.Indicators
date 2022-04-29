namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // FRACTAL CHAOS BANDS
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<FcbResult> GetFcb<TQuote>(
        this IEnumerable<TQuote> quotes,
        int windowSpan = 2)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateFcb(windowSpan);

        // initialize
        List<FractalResult> fractals = GetFractal(quotes, windowSpan).ToList();
        int length = fractals.Count;
        List<FcbResult> results = new(length);
        decimal? upperLine = null, lowerLine = null;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            FractalResult f = fractals[i];

            FcbResult r = new()
            {
                Date = f.Date
            };

            if (i >= 2 * windowSpan)
            {
                FractalResult fp = fractals[i - windowSpan];

                upperLine = (fp.FractalBear != null) ? fp.FractalBear : upperLine;
                lowerLine = (fp.FractalBull != null) ? fp.FractalBull : lowerLine;

                r.UpperBand = upperLine;
                r.LowerBand = lowerLine;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateFcb(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for FCB.");
        }
    }
}
