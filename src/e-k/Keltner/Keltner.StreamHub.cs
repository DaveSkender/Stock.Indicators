namespace Skender.Stock.Indicators;

// KELTNER CHANNELS (STREAM HUB)

/// <summary>
/// Represents a stream hub for calculating Keltner Channels.
/// </summary>
public class KeltnerHub
    : StreamHub<IQuote, KeltnerResult>, IKeltner
{
    #region constructors

    private readonly string hubName;
    private readonly int _lookbackPeriods;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeltnerHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="emaPeriods">The number of periods for the EMA.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="atrPeriods">The number of periods for the ATR.</param>
    internal KeltnerHub(
        IQuoteProvider<IQuote> provider,
        int emaPeriods,
        double multiplier,
        int atrPeriods) : base(provider)
    {
        Keltner.Validate(emaPeriods, multiplier, atrPeriods);

        EmaPeriods = emaPeriods;
        Multiplier = multiplier;
        AtrPeriods = atrPeriods;
        _lookbackPeriods = Math.Max(emaPeriods, atrPeriods);
        EmaK = 2d / (emaPeriods + 1);
        hubName = $"KELTNER({emaPeriods},{multiplier},{atrPeriods})";

        Reinitialize();
    }

    #endregion

    #region properties

    /// <inheritdoc/>
    public int EmaPeriods { get; init; }

    /// <inheritdoc/>
    public double Multiplier { get; init; }

    /// <inheritdoc/>
    public int AtrPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the EMA.
    /// </summary>
    public double EmaK { get; private init; }

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Calculates the simple moving average of Close prices.
    /// </summary>
    private double CalculateSmaOfClose(int endIndex, int periods)
    {
        if (endIndex < periods - 1 || endIndex + 1 > ProviderCache.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = endIndex - periods + 1; i <= endIndex; i++)
        {
            sum += (double)ProviderCache[i].Close;
        }

        return sum / periods;
    }

    /// <inheritdoc/>
    protected override (KeltnerResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate EMA of Close
        double ema;
        if (i >= EmaPeriods - 1 && Cache[i - 1].Centerline is not null)
        {
            // Calculate EMA normally
            ema = Ema.Increment(EmaK, Cache[i - 1].Centerline!.Value, (double)item.Close);
        }
        else if (i >= EmaPeriods - 1)
        {
            // Initialize as SMA of Close prices
            ema = CalculateSmaOfClose(i, EmaPeriods);
        }
        else
        {
            // warmup periods are never calculable
            ema = double.NaN;
        }

        // Calculate ATR
        double atr;

        if (i == 0)
        {
            atr = double.NaN;
        }
        else if (Cache[i - 1].Atr is not null)
        {
            // Calculate ATR normally using previous ATR
            AtrResult atrResult = Atr.Increment(AtrPeriods, item, (double)ProviderCache[i - 1].Close, Cache[i - 1].Atr);
            atr = atrResult.Atr ?? double.NaN;
        }
        else if (i >= AtrPeriods)
        {
            // Initialize ATR using same method as Series:
            // Sum TR from index 1 to AtrPeriods, then incrementally update to current index
            double sumTr = 0;

            // Initial sum from index 1 to AtrPeriods (matching Series behavior)
            for (int p = 1; p <= AtrPeriods; p++)
            {
                sumTr += Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);
            }

            double prevAtr = sumTr / AtrPeriods;

            // Incrementally update ATR from AtrPeriods+1 to i
            for (int p = AtrPeriods + 1; p <= i; p++)
            {
                double tr = Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);

                prevAtr = ((prevAtr * (AtrPeriods - 1)) + tr) / AtrPeriods;
            }

            atr = prevAtr;
        }
        else
        {
            atr = double.NaN;
        }

        // Only calculate bands when both EMA and ATR are ready
        if (i >= _lookbackPeriods - 1)
        {
            double? atrSpan = atr.NaN2Null() * Multiplier;

            KeltnerResult r = new(
                Timestamp: item.Timestamp,
                UpperBand: ema.NaN2Null() + atrSpan,
                Centerline: ema.NaN2Null(),
                LowerBand: ema.NaN2Null() - atrSpan,
                Width: ema == 0 ? null : 2 * atrSpan / ema) {
                Atr = atr.NaN2Null()
            };

            return (r, i);
        }
        else
        {
            // During warmup, return empty result with just timestamp
            KeltnerResult r = new(Timestamp: item.Timestamp);

            return (r, i);
        }
    }

    #endregion
}


public static partial class Keltner
{
    /// <summary>
    /// Creates a Keltner Channels streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="emaPeriods">The number of periods for the EMA. Default is 20.</param>
    /// <param name="multiplier">The multiplier for the ATR. Default is 2.</param>
    /// <param name="atrPeriods">The number of periods for the ATR. Default is 10.</param>
    /// <returns>An instance of <see cref="KeltnerHub"/>.</returns>
    public static KeltnerHub ToKeltnerHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int emaPeriods = 20,
        double multiplier = 2,
        int atrPeriods = 10)
        => new(quoteProvider, emaPeriods, multiplier, atrPeriods);
}
