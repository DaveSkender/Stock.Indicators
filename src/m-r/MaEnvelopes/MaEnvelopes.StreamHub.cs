using System.Globalization;

namespace Skender.Stock.Indicators;

#pragma warning disable IDE0072 // Missing cases in switch statement

/// <summary>
/// Provides methods for calculating the Moving Average Envelopes indicator.
/// </summary>
public static partial class MaEnvelopes
{
    /// <summary>
    /// Creates a Moving Average Envelopes streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the moving average.</param>
    /// <param name="percentOffset">The percentage offset for the envelopes. Default is 2.5.</param>
    /// <param name="movingAverageType">The type of moving average to use. Default is SMA.</param>
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
public class MaEnvelopesHub
    : StreamHub<IReusable, MaEnvelopeResult>
{
    private readonly string hubName;
    private readonly double offsetRatio;
    private readonly MaType movingAverageType;
    private readonly double k; // for EMA-based types
    private readonly int lookbackPeriods;

    private double lastEma1 = double.NaN; // for DEMA
    private double lastEma2 = double.NaN; // for DEMA
    private double lastEma3 = double.NaN; // for TEMA
    private double lastEma4 = double.NaN; // for TEMA
    private double lastEma5 = double.NaN; // for TEMA

    /// <summary>
    /// Initializes a new instance of the <see cref="MaEnvelopesHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the moving average.</param>
    /// <param name="percentOffset">The percentage offset for the envelopes.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid or the moving average type is not supported.</exception>
    internal MaEnvelopesHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType) : base(provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        MaEnvelopes.Validate(percentOffset);

        // Validate lookbackPeriods based on MA type
        switch (movingAverageType)
        {
            case MaType.ALMA:
                throw new NotImplementedException(
                    "ALMA-based Moving Average Envelopes are not yet supported in streaming mode. " +
                    "Use Series-style calculation instead.");
            case MaType.DEMA:
                Dema.Validate(lookbackPeriods);
                break;
            case MaType.EMA:
                Ema.Validate(lookbackPeriods);
                break;
            case MaType.EPMA:
                throw new NotImplementedException(
                    "EPMA-based Moving Average Envelopes are not yet supported in streaming mode. " +
                    "Use Series-style calculation instead.");
            case MaType.HMA:
                throw new NotImplementedException(
                    "HMA-based Moving Average Envelopes are not yet supported in streaming mode. " +
                    "Use Series-style calculation instead.");
            case MaType.SMA:
                Sma.Validate(lookbackPeriods);
                break;
            case MaType.SMMA:
                Smma.Validate(lookbackPeriods);
                break;
            case MaType.TEMA:
                Tema.Validate(lookbackPeriods);
                break;
            case MaType.WMA:
                Wma.Validate(lookbackPeriods);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(movingAverageType), movingAverageType,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Moving Average Envelopes does not support {0}.",
                        Enum.GetName(movingAverageType)));
        }

        this.lookbackPeriods = lookbackPeriods;
        LookbackPeriods = lookbackPeriods;
        PercentOffset = percentOffset;
        MovingAverageType = movingAverageType;

        this.movingAverageType = movingAverageType;
        offsetRatio = percentOffset / 100d;
        k = 2d / (lookbackPeriods + 1); // for EMA-based types
        hubName = $"MAENV({lookbackPeriods},{percentOffset},{Enum.GetName(movingAverageType)})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods for the moving average.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the percentage offset for the envelopes.
    /// </summary>
    public double PercentOffset { get; init; }

    /// <summary>
    /// Gets the type of moving average used.
    /// </summary>
    public MaType MovingAverageType { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (MaEnvelopeResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate MA value based on type
        double? ma = movingAverageType switch {
            MaType.DEMA => CalculateDema(i),
            MaType.EMA => CalculateEma(i),
            MaType.SMA => CalculateSma(i),
            MaType.SMMA => CalculateSmma(i),
            MaType.TEMA => CalculateTema(i),
            MaType.WMA => CalculateWma(i),
            _ => throw new ArgumentOutOfRangeException(nameof(item))
        };

        // Calculate envelopes
        double? upperEnvelope = ma.HasValue && !double.IsNaN(ma.Value) ? ma + (ma * offsetRatio) : null;
        double? lowerEnvelope = ma.HasValue && !double.IsNaN(ma.Value) ? ma - (ma * offsetRatio) : null;

        MaEnvelopeResult r = new(
            Timestamp: item.Timestamp,
            Centerline: ma,
            UpperEnvelope: upperEnvelope,
            LowerEnvelope: lowerEnvelope);

        return (r, i);
    }

    private double? CalculateSma(int index)
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double sum = 0;
        int count = 0;
        for (int p = index - lookbackPeriods + 1; p <= index; p++)
        {
            double val = ProviderCache[p].Value;
            if (double.IsNaN(val))
            {
                return null; // Not enough valid data yet
            }
            sum += val;
            count++;
        }

        return count == lookbackPeriods ? sum / lookbackPeriods : null;
    }

    private double? CalculateEma(int index)
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double val = ProviderCache[index].Value;
        if (double.IsNaN(val))
        {
            return null;
        }

        double ema = index >= lookbackPeriods - 1
            ? Cache[index - 1].Centerline is not null
                // normal
                ? Ema.Increment(k, Cache[index - 1].Centerline!.Value, val)
                // re/initialize as SMA
                : Sma.Increment(ProviderCache, lookbackPeriods, index)
            // warmup periods are never calculable
            : double.NaN;

        return ema.NaN2Null();
    }

    private double? CalculateDema(int index)
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double val = ProviderCache[index].Value;
        if (double.IsNaN(val))
        {
            return null;
        }

        // if out-of-order change (insertion/deletion before current index) occurred
        // invalidate state and backfill from previous cached EMA layers
        if (index > 0 && Cache.Count > index && Cache[index - 1].Centerline is not null && double.IsNaN(lastEma1))
        {
            // Backfill from cache if available - for now, reinitialize
            lastEma1 = lastEma2 = double.NaN;
        }

        double dema = index >= lookbackPeriods - 1
            ? Cache[index - 1].Centerline is not null
                // normal
                ? CalculateDemaIncrement(val)
                // re/initialize as SMA
                : InitializeDema(index)
            // warmup periods are never calculable
            : double.NaN;

        return dema.NaN2Null();
    }

    private double InitializeDema(int index)
    {
        double sum = 0;
        for (int p = index - lookbackPeriods + 1; p <= index; p++)
        {
            double val = ProviderCache[p].Value;
            if (double.IsNaN(val))
            {
                lastEma1 = lastEma2 = double.NaN;
                return double.NaN;
            }
            sum += val;
        }

        lastEma1 = lastEma2 = sum / lookbackPeriods;
        return (2 * lastEma1) - lastEma2;
    }

    private double CalculateDemaIncrement(double value)
    {
        if (double.IsNaN(value))
        {
            return double.NaN;
        }
        lastEma1 = Ema.Increment(k, lastEma1, value);
        lastEma2 = Ema.Increment(k, lastEma2, lastEma1);
        return (2 * lastEma1) - lastEma2;
    }

    private double? CalculateTema(int index)
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double val = ProviderCache[index].Value;
        if (double.IsNaN(val))
        {
            return null;
        }

        double tema = index >= lookbackPeriods - 1
            ? Cache[index - 1].Centerline is not null
                // normal
                ? CalculateTemaIncrement(val)
                // re/initialize as SMA
                : InitializeTema(index)
            // warmup periods are never calculable
            : double.NaN;

        return tema.NaN2Null();
    }

    private double InitializeTema(int index)
    {
        double sum = 0;
        for (int p = index - lookbackPeriods + 1; p <= index; p++)
        {
            double val = ProviderCache[p].Value;
            if (double.IsNaN(val))
            {
                lastEma3 = lastEma4 = lastEma5 = double.NaN;
                return double.NaN;
            }
            sum += val;
        }

        lastEma3 = lastEma4 = lastEma5 = sum / lookbackPeriods;
        return (3 * lastEma3) - (3 * lastEma4) + lastEma5;
    }

    private double CalculateTemaIncrement(double value)
    {
        if (double.IsNaN(value))
        {
            return double.NaN;
        }
        lastEma3 = Ema.Increment(k, lastEma3, value);
        lastEma4 = Ema.Increment(k, lastEma4, lastEma3);
        lastEma5 = Ema.Increment(k, lastEma5, lastEma4);
        return (3 * lastEma3) - (3 * lastEma4) + lastEma5;
    }

    private double? CalculateSmma(int index)
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double val = ProviderCache[index].Value;
        if (double.IsNaN(val))
        {
            return null;
        }

        double smma = index >= lookbackPeriods - 1
            ? Cache[index - 1].Centerline is not null
                // normal
                ? ((Cache[index - 1].Centerline!.Value * (lookbackPeriods - 1)) + val) / lookbackPeriods
                // re/initialize as SMA
                : Sma.Increment(ProviderCache, lookbackPeriods, index)
            // warmup periods are never calculable
            : double.NaN;

        return smma.NaN2Null();
    }

    private double? CalculateWma(int index)
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double sumWeight = 0;
        double sumValue = 0;
        for (int p = index - lookbackPeriods + 1, weight = 1; p <= index; p++, weight++)
        {
            double val = ProviderCache[p].Value;
            if (double.IsNaN(val))
            {
                return null; // Not enough valid data yet
            }
            sumValue += val * weight;
            sumWeight += weight;
        }

        return sumValue / sumWeight;
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > lookbackPeriods)
        {
            MaEnvelopeResult prior = Cache[i - 1];
            // Reset state variables based on MA type
            if (movingAverageType == MaType.DEMA)
            {
                lastEma1 = lastEma2 = double.NaN;
            }
            else if (movingAverageType == MaType.TEMA)
            {
                lastEma3 = lastEma4 = lastEma5 = double.NaN;
            }
        }
        else
        {
            lastEma1 = lastEma2 = lastEma3 = lastEma4 = lastEma5 = double.NaN;
        }
    }
}
