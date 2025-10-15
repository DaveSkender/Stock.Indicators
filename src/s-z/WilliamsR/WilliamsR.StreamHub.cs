namespace Skender.Stock.Indicators;


/// <summary>
/// Represents a Williams %R stream hub.
/// </summary>
public class WilliamsRHub
    : StreamHub<IQuote, WilliamsResult>, IWilliamsR
 {
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for Williams %R.</param>
    internal WilliamsRHub(
        IStreamObservable<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        WilliamsR.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;

        hubName = $"WILLR({lookbackPeriods})";

        Reinitialize();
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the lookback periods for Williams %R calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (WilliamsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate Williams %R
        double williamsR = double.NaN;
        if (i >= LookbackPeriods - 1)
        {
            double highHigh = double.MinValue;
            double lowLow = double.MaxValue;
            bool isViable = true;

            // Get lookback window
            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                IQuote x = (IQuote)ProviderCache[p];

                if (double.IsNaN((double)x.High) ||
                    double.IsNaN((double)x.Low) ||
                    double.IsNaN((double)x.Close))
                {
                    isViable = false;
                    break;
                }

                if ((double)x.High > highHigh)
                {
                    highHigh = (double)x.High;
                }

                if ((double)x.Low < lowLow)
                {
                    lowLow = (double)x.Low;
                }
            }

            williamsR = !isViable
                 ? double.NaN
                 : highHigh - lowLow != 0
                 ? (100 * ((double)item.Close - lowLow) / (highHigh - lowLow)) - 100
                 : 0;
        }

        WilliamsResult result = new(
            Timestamp: item.Timestamp,
            WilliamsR: williamsR.NaN2Null());

        return (result, i);
    }

    #endregion

}


public static partial class WilliamsR
{
    /// <summary>
    /// Converts the quote provider to a Williams %R hub.
    /// </summary>
        /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for Williams %R.</param>
    /// <returns>A Williams %R hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static WilliamsRHub ToWilliamsRHub(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates a WilliamsR hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods"></param>
    /// <returns>An instance of <see cref="WilliamsRHub"/>.</returns>
    public static WilliamsRHub ToWilliamsRHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWilliamsRHub(lookbackPeriods);
    }
}

