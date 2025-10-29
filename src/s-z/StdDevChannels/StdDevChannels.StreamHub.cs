namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (STREAM HUB)

/// <summary>
/// Interface for Standard Deviation Channels Hub.
/// </summary>
public interface IStdDevChannels
{
    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    int LookbackPeriods { get; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    double StandardDeviations { get; }
}

/// <summary>
/// Provides methods for creating Standard Deviation Channels hubs.
/// </summary>
/// <remarks>
/// Standard Deviation Channels uses a reverse window algorithm that recalculates
/// ALL values when new data arrives. This is different from typical streaming
/// indicators where each value is calculated once. As the cache grows, window
/// boundaries shift, causing values to "repaint" (change retrospectively).
/// </remarks>
public class StdDevChannelsHub
    : StreamHub<IReusable, StdDevChannelsResult>, IStdDevChannels
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevChannelsHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    internal StdDevChannelsHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double standardDeviations) : base(provider)
    {
        StdDevChannels.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        StandardDeviations = standardDeviations;
        hubName = $"STDEV-CHANNELS({lookbackPeriods},{standardDeviations})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    public double StandardDeviations { get; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    /// <remarks>
    /// Overrides the default behavior to recalculate ALL cached values
    /// because Standard Deviation Channels uses reverse windows that shift
    /// as new data arrives.
    /// </remarks>
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        // Clear existing cache and recalculate all values
        // because window boundaries shift with each new item
        Cache.Clear();
        
        // Recalculate all values based on current provider cache size
        int cacheSize = ProviderCache.Count;
        for (int i = 0; i < cacheSize; i++)
        {
            StdDevChannelsResult r = StdDevChannels.Increment(
                ProviderCache,
                LookbackPeriods,
                StandardDeviations,
                i);
            
            Cache.Add(r);
        }
    }

    /// <inheritdoc/>
    protected override (StdDevChannelsResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result using Increment method
        StdDevChannelsResult r = StdDevChannels.Increment(
            ProviderCache,
            LookbackPeriods,
            StandardDeviations,
            i);

        return (r, i);
    }

    #endregion methods
}


public static partial class StdDevChannels
{
    /// <summary>
    /// Converts the chain provider to a Standard Deviation Channels hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    /// <returns>A Standard Deviation Channels hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static StdDevChannelsHub ToStdDevChannelsHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
             => new(chainProvider, lookbackPeriods, standardDeviations);

    /// <summary>
    /// Creates a Standard Deviation Channels hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">Number of standard deviations for the channels.</param>
    /// <returns>An instance of <see cref="StdDevChannelsHub"/>.</returns>
    public static StdDevChannelsHub ToStdDevChannelsHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStdDevChannelsHub(lookbackPeriods, standardDeviations);
    }
}
