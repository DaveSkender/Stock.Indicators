namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Simple Moving Average (SMA).
/// </summary>
public class SmaHub
    : ChainHub<IReusable, SmaResult>, ISma
{
    private readonly Queue<double> buffer;

    internal SmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        buffer = new Queue<double>(lookbackPeriods);
        Name = $"SMA({lookbackPeriods})";

        // Validate cache size for warmup requirements
        ValidateCacheSize(lookbackPeriods, Name);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Update the rolling buffer
        buffer.Update(LookbackPeriods, item.Value);

        // Calculate SMA when we have enough values
        double sma = buffer.Count == LookbackPeriods ? buffer.Average() : double.NaN;

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma.NaN2Null());

        return (r, i);
    }

    /// <summary>
    /// Restores the buffer state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(int restoreIndex)
    {
        // Clear buffer
        buffer.Clear();

        if (restoreIndex < 0)
        {
            return;
        }

        // Rebuild buffer from cache
        // We need at most the last LookbackPeriods values
        int startIdx = Math.Max(0, restoreIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= restoreIndex; p++)
        {
            IReusable item = ProviderCache[p];
            buffer.Update(LookbackPeriods, item.Value);
        }
    }
}

public static partial class Sma
{
    /// <summary>
    /// Creates an SMA streaming hub with a chain provider source.
    /// </summary>
    /// <remarks>If providers contain historical data, this hub will fast-forward its cache.</remarks>
    /// <param name="chainProvider">Chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A chain-sourced instance of <see cref="SmaHub"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="chainProvider"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static SmaHub ToSmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);
}
