namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Fractal Chaos Bands (FCB) indicator.
/// </summary>
public static partial class Fcb
{
    /// <summary>
    /// Converts a list of quotes to FCB results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote data.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="windowSpan">The window span for the calculation. Default is 2.</param>
    /// <returns>A list of FCB results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the window span is invalid.</exception>
    [Series("FCB", "Fractal Chaos Bands", Category.PriceChannel, ChartType.Overlay)]
    public static IReadOnlyList<FcbResult> ToFcb<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [Param("Window Span", 2, 30, 2)]
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
