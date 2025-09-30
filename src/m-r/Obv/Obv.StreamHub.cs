namespace Skender.Stock.Indicators;

// ON-BALANCE VOLUME (STREAM HUB)

/// <summary>
/// Provides methods for creating OBV hubs.
/// </summary>
public static partial class Obv
{
    /// <summary>
    /// Converts the quote provider to an OBV hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>An OBV hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    public static ObvHub<TIn> ToObv<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider);
    }
}

/// <summary>
/// Represents an On-Balance Volume (OBV) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class ObvHub<TIn> : ChainProvider<TIn, ObvResult>, IObv
    where TIn : IQuote
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObvHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    internal ObvHub(
        IQuoteProvider<TIn> provider) : base(provider)
    {
    }

    /// <inheritdoc/>
    protected override (ObvResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Handle volume direction changes in streaming mode
        double obvValue;
        if (i > 0)
        {
            double currentClose = (double)item.Close;
            double previousClose = (double)ProviderCache[i - 1].Close;
            
            double previousObv = Cache[i - 1].Obv;
            
            if (currentClose > previousClose)
            {
                obvValue = previousObv + (double)item.Volume;
            }
            else if (currentClose < previousClose)
            {
                obvValue = previousObv - (double)item.Volume;
            }
            else
            {
                obvValue = previousObv; // No change if prices are equal
            }
        }
        else
        {
            obvValue = 0; // First period starts at 0
        }

        // Create result
        ObvResult r = new(
            Timestamp: item.Timestamp,
            Obv: obvValue);

        return (r, i);
    }

    /// <inheritdoc/>
    public override string ToString() => "OBV";
}