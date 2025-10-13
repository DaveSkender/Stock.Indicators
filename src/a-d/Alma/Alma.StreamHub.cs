namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Arnaud Legoux Moving Average (ALMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class AlmaHub<TIn>
    : ChainProvider<TIn, AlmaResult>, IAlma
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="offset">The offset for the ALMA calculation.</param>
    /// <param name="sigma">The sigma for the ALMA calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    internal AlmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods,
        double offset,
        double sigma) : base(provider)
    {
        Alma.Validate(lookbackPeriods, offset, sigma);
        LookbackPeriods = lookbackPeriods;
        Offset = offset;
        Sigma = sigma;
        hubName = $"ALMA({lookbackPeriods},{offset},{sigma})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double Offset { get; init; }

    /// <inheritdoc/>
    public double Sigma { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AlmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double alma = i >= LookbackPeriods - 1
            ? Alma.Increment(ProviderCache, LookbackPeriods, Offset, Sigma, i)
            : double.NaN;

        // candidate result
        AlmaResult r = new(
            Timestamp: item.Timestamp,
            Alma: alma.NaN2Null());

        return (r, i);
    }
}

/// <summary>
/// Provides methods for calculating the Arnaud Legoux Moving Average (ALMA) indicator.
/// </summary>
public static partial class Alma
{
    /// <summary>
    /// Creates an ALMA streaming hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <returns>An ALMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static AlmaHub<T> ToAlmaHub<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where T : IReusable
        => new(chainProvider, lookbackPeriods, offset, sigma);

    /// <summary>
    /// Creates an ALMA hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <returns>An instance of <see cref="AlmaHub{TQuote}"/>.</returns>
    public static AlmaHub<TQuote> ToAlmaHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAlmaHub(lookbackPeriods, offset, sigma);
    }
}
