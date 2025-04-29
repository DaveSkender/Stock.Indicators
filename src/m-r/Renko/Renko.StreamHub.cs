namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating Renko chart series in a streaming manner.
/// </summary>
public static partial class Renko
{
    /// <summary>
    /// Converts a quote provider to a Renko hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the quote values.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>A Renko hub.</returns>
    [Stream("RENKO", "Renko", Category.PriceTransform, ChartType.Overlay)]
    public static RenkoHub<TIn> ToRenko<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        [ParamNum<decimal>("Brick Size", 1, 0.000001, 2500)]
        decimal brickSize,
        [ParamEnum<EndType>("End Type", EndType.Close)]
        EndType endType = EndType.Close)
        where TIn : IQuote
        => new(quoteProvider, brickSize, endType);
}

/// <summary>
/// Represents a hub for generating Renko chart results from a stream of quotes.
/// </summary>
/// <typeparam name="TIn">The type of the quote values.</typeparam>
public class RenkoHub<TIn>
    : QuoteProvider<TIn, RenkoResult>, IRenko
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    private RenkoResult lastBrick
        = new(default, default, default,
            default, default, default, default);

    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The type of price to use for the end of the brick.</param>
    internal RenkoHub(
        IQuoteProvider<TIn> provider,
        decimal brickSize,
        EndType endType) : base(provider)
    {
        Renko.Validate(brickSize);
        BrickSize = brickSize;
        EndType = endType;
        hubName = $"RENKO({brickSize},{endType.ToString().ToUpperInvariant()})";

        Reinitialize();
    }
    #endregion

    /// <summary>
    /// Renko hub settings. Since it can produce 0 or many bricks per quote,
    /// the default 1:1 in/out is not used and must be skipped to prevent
    /// same-date triggered rebuilds when caching.
    /// </summary>
    public override BinarySettings Properties { get; init; } = new(0b00000010);  // custom

    /// <inheritdoc/>
    public decimal BrickSize { get; }

    /// <inheritdoc/>
    public EndType EndType { get; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    public override void OnAdd(TIn item, bool notify, int? indexHint)
        => ToIndicator(item, notify, indexHint);

    /// <inheritdoc/>
    protected override (RenkoResult result, int index)
        ToIndicator(TIn item, int? indexHint)
        => throw new InvalidOperationException(); // not used

    /// <summary>
    /// Restores the last brick marker to the state at the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // restore last brick marker
        if (Cache.Count != 0)
        {
            lastBrick = Cache
                .Last(c => c.Timestamp <= timestamp);

            return;
        }

        // skip first quote
        if (ProviderCache.Count <= 1)
        {
            return;
        }

        SetBaselineBrick();
    }

    // re/initialize last brick marker
    private void SetBaselineBrick()
    {
        int decimals = BrickSize.GetDecimalPlaces();

        TIn q0 = ProviderCache[0];

        decimal baseline
            = Math.Round(q0.Close,
                Math.Max(decimals - 1, 0));

        lastBrick = new(
            q0.Timestamp,
            Open: baseline,
            High: 0,
            Low: 0,
            Close: baseline,
            Volume: 0,
            IsUp: false);
    }

    // custom: build 0 to many bricks per quote
    private void ToIndicator(TIn item, bool notify, int? indexHint)
    {
        int providerIndex = indexHint
            ?? throw new InvalidOperationException($"{nameof(indexHint)} cannot be empty");

        // nothing to do
        if (providerIndex <= 0)
        {
            return;
        }

        // establish baseline brick
        if (providerIndex == 1)
        {
            SetBaselineBrick();
        }

        // determine new brick quantity
        int newBrickQty
            = Renko.GetNewBrickQuantity(
                item, lastBrick, BrickSize, EndType);

        int absBrickQty = Math.Abs(newBrickQty);
        bool isUp = newBrickQty >= 0;

        // add new brick(s) ... can add more than one!
        if (absBrickQty > 0)
        {
            // get high/low/volume between bricks
            decimal h = decimal.MinValue;
            decimal l = decimal.MaxValue;
            decimal sumV = 0;  // cumulative

            // by aggregating provider cache range
            int lastBrickIndex = ProviderCache.IndexOf(lastBrick.Timestamp, true);

            for (int w = lastBrickIndex + 1; w <= providerIndex; w++)
            {
                TIn pq = ProviderCache[w];

                h = Math.Max(h, pq.High);
                l = Math.Min(l, pq.Low);
                sumV += pq.Volume;
            }

            decimal v = sumV / absBrickQty;

            for (int b = 0; b < absBrickQty; b++)
            {
                decimal o = isUp
                    ? Math.Max(lastBrick.Open, lastBrick.Close)
                    : Math.Min(lastBrick.Open, lastBrick.Close);

                decimal c = isUp
                    ? o + BrickSize
                    : o - BrickSize;

                // candidate result
                RenkoResult r
                    = new(item.Timestamp, o, h, l, c, v, isUp);

                lastBrick = r;

                // save and send
                AppendCache(r, notify);

                // note: bypass rebuild bit set in Properties to allow
                // sequential bricks with duplicate dates that would
                // normally trigger rebuild, causing stack overflow.
            }
        }
    }
}
