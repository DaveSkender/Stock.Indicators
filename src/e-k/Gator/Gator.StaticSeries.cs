namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Gator Oscillator indicator.
/// </summary>
public static partial class Gator
{
    /// <summary>
    /// Converts a list of time-series values to Gator Oscillator results.
    /// </summary>
    /// <typeparam name="T">The type of the time-series values, which must implement <see cref="IReusable"/> or <see cref="IQuote"/>.</typeparam>
    /// <param name="source">The list of time-series values to transform.</param>
    /// <returns>A list of Gator Oscillator results.</returns>
    [Series("GATOR", "Gator Oscillator", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<GatorResult> ToGator<T>(
        this IReadOnlyList<T> source)
        where T : IReusable
        => source
            .ToAlligator()
            .ToGator();

    /// <summary>
    /// Converts a list of Alligator results to Gator Oscillator results.
    /// </summary>
    /// <param name="alligator">The list of Alligator results.</param>
    /// <returns>A list of Gator Oscillator results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the alligator list is null.</exception>
    [ExcludeFromCatalog]
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
