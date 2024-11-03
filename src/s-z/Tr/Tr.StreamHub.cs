namespace Skender.Stock.Indicators;

// TRUE RANGE (STREAM HUB)

#region initializer

public static partial class Tr
{
    public static TrHub<TIn> ToTr<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
        => new(quoteProvider);
}
#endregion

public class TrHub<TIn>
    : ChainProvider<TIn, TrResult>
    where TIn : IQuote
{
    #region constructors

    private const string hubName = "TRUE RANGE";

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
        int i = indexHint ?? ProviderCache.GetIndex(item, true);

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
