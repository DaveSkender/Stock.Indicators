namespace Skender.Stock.Indicators;

// TRUE RANGE (STREAM HUB)

#region initializer

public static partial class Tr
{
    /// <summary>
    /// Converts a quote provider to a True Range (TR) hub.
    /// </summary>
    /// <typeparam name="TIn">The type of quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>A True Range (TR) hub.</returns>
    [Stream("TR", "True Range", Category.PriceTrend, ChartType.Overlay)]
    public static TrHub<TIn> ToTr<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
        => new(quoteProvider);
}
#endregion

/// <summary>
/// Represents a True Range (TR) hub for streaming data.
/// </summary>
/// <typeparam name="TIn">The type of quote.</typeparam>
public class TrHub<TIn>
    : ChainProvider<TIn, TrResult>
    where TIn : IQuote
{
    #region constructors

    private const string hubName = "TRUE RANGE";

    /// <summary>
    /// Initializes a new instance of the <see cref="TrHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    internal TrHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TrResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period
        if (i == 0)
        {
            return (new TrResult(item.Timestamp, null), i);
        }

        TIn prev = ProviderCache[i - 1];

        // candidate result
        TrResult r = new(
            item.Timestamp,
            Tr.Increment(
                (double)item.High,
                (double)item.Low,
                (double)prev.Close));

        return (r, i);
    }
}
