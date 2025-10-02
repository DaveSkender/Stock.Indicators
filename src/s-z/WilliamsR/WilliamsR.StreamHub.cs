namespace Skender.Stock.Indicators;

// WILLIAMS %R (STREAM HUB)

/// <summary>
/// Provides methods for creating Williams %R hubs.
/// </summary>
public static partial class WilliamsR
{
    /// <summary>
    /// Converts the quote provider to a Williams %R hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for Williams %R.</param>
    /// <returns>A Williams %R hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static WilliamsRHub<TIn> ToWilliamsR<TIn>(
        this IStreamObservable<TIn> quoteProvider,
        int lookbackPeriods = 14)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods);
}

/// <summary>
/// Represents a Williams %R stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class WilliamsRHub<TIn>
    : StreamHub<TIn, WilliamsResult>, IWilliamsR
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for Williams %R.</param>
    internal WilliamsRHub(
        IStreamObservable<TIn> provider,
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
        ToIndicator(TIn item, int? indexHint)
    {
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
                TIn x = ProviderCache[p];

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
                 ? -100 * (highHigh - (double)item.Close) / (highHigh - lowLow)
                 : 0;
        }

        WilliamsResult result = new(
            Timestamp: item.Timestamp,
            WilliamsR: williamsR.NaN2Null());

        return (result, i);
    }

    #endregion
}
