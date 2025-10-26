namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (STREAM HUB)

/// <summary>
/// Represents a stream hub for calculating the Gator Oscillator indicator.
/// </summary>
public class GatorHub
   : StreamHub<AlligatorResult, GatorResult>
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="GatorHub"/> class.
    /// </summary>
    /// <param name="alligatorHub">The alligator hub.</param>
    internal GatorHub(AlligatorHub alligatorHub)
        : base(alligatorHub)
    {
        ArgumentNullException.ThrowIfNull(alligatorHub);
        hubName = "GATOR()";
        Reinitialize();
    }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (GatorResult result, int index)
        ToIndicator(AlligatorResult item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        if (i == 0)
        {
            // First result is always null values
            GatorResult r = new(item.Timestamp);
            return (r, i);
        }

        GatorResult p = Cache[i - 1];

        double? upper = (item.Jaw - item.Teeth).Abs();
        double? lower = -(item.Teeth - item.Lips).Abs();

        GatorResult result = new(
            Timestamp: item.Timestamp,
            Upper: upper,
            Lower: lower,
            UpperIsExpanding: p.Upper is not null ? upper > p.Upper : null,
            LowerIsExpanding: p.Lower is not null ? lower < p.Lower : null);

        return (result, i);
    }
}

public static partial class Gator
{
    /// <summary>
    /// Converts an Alligator hub to a Gator hub.
    /// </summary>
    /// <param name="alligatorHub">The Alligator hub.</param>
    /// <returns>A Gator hub.</returns>
    public static GatorHub ToGatorHub(
        this AlligatorHub alligatorHub)
        => new(alligatorHub);

    /// <summary>
    /// Creates a Gator hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <returns>A Gator hub.</returns>
    public static GatorHub ToGatorHub(
        this IChainProvider<IReusable> chainProvider)
    {
        AlligatorHub alligatorHub = chainProvider.ToAlligatorHub();
        return new GatorHub(alligatorHub);
    }

    /// <summary>
    /// Creates a Gator hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <returns>An instance of <see cref="GatorHub"/>.</returns>
    public static GatorHub ToGatorHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToGatorHub();
    }
}
