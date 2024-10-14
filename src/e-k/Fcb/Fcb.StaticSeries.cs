namespace Skender.Stock.Indicators;

// FRACTAL CHAOS BANDS (SERIES)

public static partial class Fcb
{
    public static IReadOnlyList<FcbResult> ToFcb<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int windowSpan = 2)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(windowSpan);

        // initialize
        int length = quotes.Count;
        List<FcbResult> results = new(length);

        IReadOnlyList<FractalResult> fractals = quotes
            .ToFractal(windowSpan, windowSpan, EndType.HighLow);

        decimal? upperLine = null;
        decimal? lowerLine = null;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            FractalResult f = fractals[i];

            if (i >= 2 * windowSpan)
            {
                FractalResult fp = fractals[i - windowSpan];

                upperLine = fp.FractalBear ?? upperLine;
                lowerLine = fp.FractalBull ?? lowerLine;
            }

            results.Add(new(
                Timestamp: f.Timestamp,
                UpperBand: upperLine,
                LowerBand: lowerLine));
        }

        return results;
    }
}
