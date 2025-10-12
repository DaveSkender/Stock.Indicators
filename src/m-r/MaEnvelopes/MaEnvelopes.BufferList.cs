using System.Globalization;

namespace Skender.Stock.Indicators;

#pragma warning disable IDE0072 // Missing cases in switch statement

/// <summary>
/// Moving Average Envelopes from incremental reusable values.
/// </summary>
public class MaEnvelopesList : BufferList<MaEnvelopeResult>, IIncrementFromChain
{
    private readonly object _maList;
    private readonly double _offsetRatio;
    private readonly MaType _movingAverageType;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaEnvelopesList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the moving average.</param>
    /// <param name="percentOffset">The percentage offset for the envelopes.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    public MaEnvelopesList(
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
    {
        MaEnvelopes.Validate(percentOffset);
        LookbackPeriods = lookbackPeriods;
        PercentOffset = percentOffset;
        MovingAverageType = movingAverageType;
        _movingAverageType = movingAverageType;

        _offsetRatio = percentOffset / 100d;

        // Create the appropriate MA BufferList based on type
        _maList = movingAverageType switch {
            MaType.ALMA => new AlmaList(lookbackPeriods, offset: 0.85, sigma: 6),
            MaType.DEMA => new DemaList(lookbackPeriods),
            MaType.EMA => new EmaList(lookbackPeriods),
            MaType.EPMA => new EpmaList(lookbackPeriods),
            MaType.HMA => new HmaList(lookbackPeriods),
            MaType.SMA => new SmaList(lookbackPeriods),
            MaType.SMMA => new SmmaList(lookbackPeriods),
            MaType.TEMA => new TemaList(lookbackPeriods),
            MaType.WMA => new WmaList(lookbackPeriods),

            _ => throw new ArgumentOutOfRangeException(
                    nameof(movingAverageType), movingAverageType,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Moving Average Envelopes does not support {0}.",
                        Enum.GetName(movingAverageType)))
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaEnvelopesList"/> class with initial values.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the moving average.</param>
    /// <param name="percentOffset">The percentage offset for the envelopes.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public MaEnvelopesList(
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType,
        IReadOnlyList<IReusable> values)
        : this(lookbackPeriods, percentOffset, movingAverageType)
        => Add(values);

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

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add value to the underlying MA BufferList using switch expression
        double? ma = _movingAverageType switch {
            MaType.ALMA => AddToList<AlmaList, AlmaResult>((AlmaList)_maList, timestamp, value, r => r.Alma),
            MaType.DEMA => AddToList<DemaList, DemaResult>((DemaList)_maList, timestamp, value, r => r.Dema),
            MaType.EMA => AddToList<EmaList, EmaResult>((EmaList)_maList, timestamp, value, r => r.Ema),
            MaType.EPMA => AddToList<EpmaList, EpmaResult>((EpmaList)_maList, timestamp, value, r => r.Epma),
            MaType.HMA => AddToList<HmaList, HmaResult>((HmaList)_maList, timestamp, value, r => r.Hma),
            MaType.SMA => AddToList<SmaList, SmaResult>((SmaList)_maList, timestamp, value, r => r.Sma),
            MaType.SMMA => AddToList<SmmaList, SmmaResult>((SmmaList)_maList, timestamp, value, r => r.Smma),
            MaType.TEMA => AddToList<TemaList, TemaResult>((TemaList)_maList, timestamp, value, r => r.Tema),
            MaType.WMA => AddToList<WmaList, WmaResult>((WmaList)_maList, timestamp, value, r => r.Wma),
            _ => null
        };

        // Calculate envelopes
        double? upperEnvelope = ma.HasValue ? ma + (ma * _offsetRatio) : null;
        double? lowerEnvelope = ma.HasValue ? ma - (ma * _offsetRatio) : null;

        AddInternal(new MaEnvelopeResult(
            Timestamp: timestamp,
            Centerline: ma,
            UpperEnvelope: upperEnvelope,
            LowerEnvelope: lowerEnvelope));
    }

    private static double? AddToList<TList, TResult>(
        TList list,
        DateTime timestamp,
        double value,
        Func<TResult, double?> selector)
        where TList : BufferList<TResult>, IIncrementFromChain
        where TResult : IReusable
    {
        list.Add(timestamp, value);
        return selector(list[^1]);
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();

        // Clear the underlying MA list using switch expression
        switch (_movingAverageType)
        {
            case MaType.ALMA:
                ((AlmaList)_maList).Clear();
                break;
            case MaType.DEMA:
                ((DemaList)_maList).Clear();
                break;
            case MaType.EMA:
                ((EmaList)_maList).Clear();
                break;
            case MaType.EPMA:
                ((EpmaList)_maList).Clear();
                break;
            case MaType.HMA:
                ((HmaList)_maList).Clear();
                break;
            case MaType.SMA:
                ((SmaList)_maList).Clear();
                break;
            case MaType.SMMA:
                ((SmmaList)_maList).Clear();
                break;
            case MaType.TEMA:
                ((TemaList)_maList).Clear();
                break;
            case MaType.WMA:
                ((WmaList)_maList).Clear();
                break;
        }
    }
}

public static partial class MaEnvelopes
{
    /// <summary>
    /// Creates a buffer list for Moving Average Envelopes calculations.
    /// </summary>
    /// <typeparam name="T">The type of the reusable value.</typeparam>
    /// <param name="source">The source list of reusable values.</param>
    /// <param name="lookbackPeriods">The number of periods for the moving average.</param>
    /// <param name="percentOffset">The percentage offset for the envelopes.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <returns>A new <see cref="MaEnvelopesList"/> instance.</returns>
    public static MaEnvelopesList ToMaEnvelopesList<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        where T : IReusable
        => new(lookbackPeriods, percentOffset, movingAverageType)
        {
            (IReadOnlyList<IReusable>)source
        };
}
