namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Chandelier Exit.
/// </summary>
public class ChandelierHub
    : ChainHub<AtrResult, ChandelierResult>, IChandelier
{
    private readonly IQuoteProvider<IQuote> _quoteProvider;
    private CircularDoubleBuffer _highBuffer;
    private CircularDoubleBuffer _lowBuffer;

    internal ChandelierHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods,
        double multiplier,
        Direction type)
        : this(
            provider.ToAtrHub(lookbackPeriods),
            provider,
            lookbackPeriods,
            multiplier,
            type)
    { }

    internal ChandelierHub(
        AtrHub atrHub,
        IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods,
        double multiplier,
        Direction type)
        : base(atrHub)
    {
        ArgumentNullException.ThrowIfNull(atrHub);
        ArgumentNullException.ThrowIfNull(quoteProvider);
        Chandelier.Validate(lookbackPeriods, multiplier);

        _quoteProvider = quoteProvider;
        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        Type = type;

        string typeName = type.ToString().ToUpperInvariant();
        Name = FormattableString.Invariant(
            $"CHEXIT({lookbackPeriods},{multiplier},{typeName})");

        _highBuffer = new CircularDoubleBuffer(lookbackPeriods);
        _lowBuffer = new CircularDoubleBuffer(lookbackPeriods);

        // Validate cache size for warmup requirements
        ValidateCacheSize(lookbackPeriods + 1, Name);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double Multiplier { get; init; }

    /// <inheritdoc/>
    public Direction Type { get; init; }

    /// <inheritdoc/>
    protected override (ChandelierResult result, int index)
        ToIndicator(AtrResult item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        IQuote quote = _quoteProvider.Results[i];

        AddToBuffers(quote);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        double? atr = item.Atr;

        if (atr is null)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        double? exit = Type switch {
            Direction.Long => _highBuffer.GetMax() - (atr.Value * Multiplier),
            Direction.Short => _lowBuffer.GetMin() + (atr.Value * Multiplier),
            _ => throw new InvalidOperationException($"Unknown direction type: {Type}")
        };

        ChandelierResult r = new(
            Timestamp: item.Timestamp,
            ChandelierExit: exit);

        return (r, i);
    }

    private void AddToBuffers(IQuote item)
    {
        _highBuffer.Add((double)item.High);
        _lowBuffer.Add((double)item.Low);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(int restoreIndex)
    {
        _highBuffer.Clear();
        _lowBuffer.Clear();

        if (restoreIndex < 0)
        {
            return;
        }

        DateTime ts = ProviderCache[restoreIndex].Timestamp;
        int quoteIndex = _quoteProvider.Results.IndexGte(ts);

        if (quoteIndex < 0)
        {
            return;
        }

        int startIdx = Math.Max(0, quoteIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= quoteIndex; p++)
        {
            IQuote quote = _quoteProvider.Results[p];
            _highBuffer.Add((double)quote.High);
            _lowBuffer.Add((double)quote.Low);
        }
    }
}

/// <summary>
/// Streaming hub for Chandelier Exit using a stream hub.
/// </summary>
public static partial class Chandelier
{
    /// <summary>
    /// Creates a Chandelier Exit streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">Quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier to apply to the ATR.</param>
    /// <param name="type">Type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
    public static ChandelierHub ToChandelierHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
             => new(quoteProvider, lookbackPeriods, multiplier, type);

    /// <summary>
    /// Creates a new Chandelier Exit hub, using ATR values from an existing ATR hub.
    /// </summary>
    /// <param name="atrHub">Existing ATR hub.</param>
    /// <param name="quoteProvider">Quote provider (must be the same provider used by the ATR hub).</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier to apply to the ATR.</param>
    /// <param name="type">Type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
    /// <remarks>
    /// <para>IMPORTANT: This is not a normal chaining approach.</para>
    /// This extension overrides the standard chaining pattern to specifically
    /// reuse an existing <see cref="AtrHub"/> internally, avoiding duplicate ATR calculations
    /// when multiple indicators need the same ATR values.
    /// The quote provider must be the same provider used to create the ATR hub;
    /// providing a different provider may result in index misalignment and incorrect calculations.
    /// </remarks>
    public static ChandelierHub ToChandelierHub(
        this AtrHub atrHub,
        IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
             => new(atrHub, quoteProvider, lookbackPeriods, multiplier, type);
}
