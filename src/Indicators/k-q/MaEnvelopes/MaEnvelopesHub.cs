namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Streaming hub for Moving Average Envelopes.
/// </summary>
public static partial class MaEnvelopes
{
    /// <summary>
    /// Creates a Moving Average Envelopes streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">Chain provider.</param>
    /// <param name="lookbackPeriods">Number of periods for the moving average.</param>
    /// <param name="percentOffset">Percentage offset for the envelopes. Default is 2.5.</param>
    /// <param name="movingAverageType">Type of moving average to use. Default is SMA.</param>
    /// <returns>A Moving Average Envelopes hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid or the moving average type is not supported.</exception>
    public static MaEnvelopesHub ToMaEnvelopesHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        => new(chainProvider, lookbackPeriods, percentOffset, movingAverageType);
}

/// <summary>
/// Streaming hub for Moving Average Envelopes calculations.
/// </summary>
/// <remarks>
/// The centerline comes from the moving average's own hub, built on the provider and
/// released with this hub. That hub matches its Series results through late arrival,
/// removal, and pruning, and this hub only offsets its values. Both hubs cache the
/// series, so each envelope hub holds two caches of the provider's length.
/// </remarks>
public class MaEnvelopesHub
    : StreamHub<IReusable, MaEnvelopeResult>
{
    // ALMA settings the Series style uses for its centerline
    private const double AlmaOffset = 0.85;
    private const double AlmaSigma = 6;

    private readonly double offsetRatio;

    internal MaEnvelopesHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType)
        : base(CenterlineHub(provider, lookbackPeriods, percentOffset, movingAverageType))
    {
        LookbackPeriods = lookbackPeriods;
        PercentOffset = percentOffset;
        MovingAverageType = movingAverageType;

        offsetRatio = percentOffset / 100d;
        Name = $"MAENV({lookbackPeriods},{percentOffset},{Enum.GetName(movingAverageType)})";

        OwnProvider();
        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double PercentOffset { get; init; }

    /// <inheritdoc/>
    public MaType MovingAverageType { get; init; }

    /// <summary>
    /// Builds the moving-average hub this hub reads its centerline from.
    /// </summary>
    /// <remarks>
    /// Arguments are validated before the inner hub subscribes, so a bad argument
    /// cannot leave it subscribed to the provider.
    /// </remarks>
    /// <param name="provider">Chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="percentOffset">Percentage offset for the envelopes.</param>
    /// <param name="movingAverageType">Type of moving average for the centerline.</param>
    /// <returns>The moving-average hub, subscribed to the provider.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The moving average type is not supported.</exception>
    private static IChainProvider<IReusable> CenterlineHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType)
    {
        ArgumentNullException.ThrowIfNull(provider);
        MaEnvelopes.Validate(percentOffset);

        return movingAverageType switch {
            MaType.ALMA => provider.ToAlmaHub(lookbackPeriods, AlmaOffset, AlmaSigma),
            MaType.DEMA => provider.ToDemaHub(lookbackPeriods),
            MaType.EMA => provider.ToEmaHub(lookbackPeriods),
            MaType.EPMA => provider.ToEpmaHub(lookbackPeriods),
            MaType.HMA => provider.ToHmaHub(lookbackPeriods),
            MaType.SMA => provider.ToSmaHub(lookbackPeriods),
            MaType.SMMA => provider.ToSmmaHub(lookbackPeriods),
            MaType.TEMA => provider.ToTemaHub(lookbackPeriods),
            MaType.WMA => provider.ToWmaHub(lookbackPeriods),
            _ => throw new ArgumentOutOfRangeException(
                nameof(movingAverageType), movingAverageType,
                $"Moving Average Envelopes does not support {Enum.GetName(movingAverageType)}.")
        };
    }

    /// <inheritdoc/>
    protected override (MaEnvelopeResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? ma = item.Value.NaN2Null();

        return (new MaEnvelopeResult(
            Timestamp: item.Timestamp,
            Centerline: ma,
            UpperEnvelope: ma + (ma * offsetRatio),
            LowerEnvelope: ma - (ma * offsetRatio)), i);
    }
}
