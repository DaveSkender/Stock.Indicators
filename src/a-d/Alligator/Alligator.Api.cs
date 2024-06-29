namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    /// <summary>
    /// Williams Alligator is an indicator that transposes multiple moving averages,
    /// showing chart patterns that creator Bill Williams compared to an alligator's
    /// feeding habits when describing market movement.
    /// <para>
    /// See
    /// <see href="https://dotnet.StockIndicators.dev/indicators/Alligator/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// <c>T</c> must be <see cref="IReusable"/> or <see cref="IQuote"/> type
    /// </typeparam>
    /// <param name="source">Time-series values to transform.</param>
    /// <param name="jawPeriods">Lookback periods for the Jaw line.</param>
    /// <param name="jawOffset">Offset periods for the Jaw line.</param>
    /// <param name="teethPeriods">Lookback periods for the Teeth line.</param>
    /// <param name="teethOffset">Offset periods for the Teeth line.</param>
    /// <param name="lipsPeriods">Lookback periods for the Lips line.</param>
    /// <param name="lipsOffset">Offset periods for the Lips line.</param>
    /// <returns>Time series of Alligator values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Invalid parameter value provided.
    /// </exception>
    public static IEnumerable<AlligatorResult> GetAlligator<T>(
        this IEnumerable<T> source,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        where T : IReusable
        => source
            .ToSortedList()
            .CalcAlligator(
                jawPeriods,
                jawOffset,
                teethPeriods,
                teethOffset,
                lipsPeriods,
                lipsOffset);

    // OBSERVER, from Chain Provider
    public static Alligator<TIn> ToAlligator<TIn>(
        this IChainProvider<TIn> chainProvider,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        where TIn : struct, IReusable
        => new(
            chainProvider,
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);
}
