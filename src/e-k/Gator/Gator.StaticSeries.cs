namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (SERIES)

public static partial class Gator
{
    /// <summary>
    /// Gator Oscillator is an expanded view of Williams Alligator.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <typeparam name="T">
    /// <c>T</c> must be <see cref="IReusable"/> or <see cref="IQuote"/> type
    /// </typeparam>
    /// <param name="source">Time-series values to transform.</param>
    /// <returns>Time series of Gator values.</returns>
    public static IReadOnlyList<GatorResult> ToGator<T>(
        this IReadOnlyList<T> source)
        where T : IReusable
        => source
            .ToAlligator()
            .ToGator();

    // from [custom] Alligator
    public static IReadOnlyList<GatorResult> ToGator(
        this IReadOnlyList<AlligatorResult> alligator)
    {
        ArgumentNullException.ThrowIfNull(alligator);

        // initialize
        int length = alligator.Count;
        List<GatorResult> results = [];

        if (length > 0)
        {
            results.Add(new(alligator[0].Timestamp));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            AlligatorResult a = alligator[i];
            GatorResult p = results[i - 1];

            double? upper = (a.Jaw - a.Teeth).Abs();
            double? lower = -(a.Teeth - a.Lips).Abs();

            results.Add(new GatorResult(

                Timestamp: a.Timestamp,

                // gator
                Upper: upper,
                Lower: lower,

                // directional information
                UpperIsExpanding: p.Upper is not null
                    ? upper > p.Upper
                    : null,

                LowerIsExpanding: p.Lower is not null
                    ? lower < p.Lower
                    : null));
        }

        return results;
    }
}
