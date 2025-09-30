namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (STREAM HUB)

/// <summary>
/// Provides methods for creating Bollinger Bands hubs.
/// </summary>
public static partial class BollingerBands
{
    /// <summary>
    /// Converts the chain provider to a Bollinger Bands hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    /// <returns>A Bollinger Bands hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static BollingerBandsHub<TIn> ToBollingerBands<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods, standardDeviations);
}

/// <summary>
/// Represents a Bollinger Bands stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class BollingerBandsHub<TIn>
    : ChainProvider<TIn, BollingerBandsResult>, IBollingerBands
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;
    private readonly double standardDeviations;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    internal BollingerBandsHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods,
        double standardDeviations) : base(provider)
    {
        BollingerBands.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        this.standardDeviations = standardDeviations;
        hubName = $"BB({lookbackPeriods},{standardDeviations})";
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    public double StandardDeviations => standardDeviations;

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (BollingerBandsResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result using Increment method like SMA
        BollingerBandsResult r = BollingerBands.Increment(
            ProviderCache, 
            LookbackPeriods, 
            standardDeviations, 
            i);

        return (r, i);
    }

    #endregion
}
