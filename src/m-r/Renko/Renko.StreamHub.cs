namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating Renko chart series in a streaming manner.
/// </summary>
public class RenkoHub
    : QuoteProvider<IQuote, RenkoResult>, IRenko
{

    private RenkoResult lastBrick
        = new(default, default, default,
            default, default, default, default);

    internal RenkoHub(
        IQuoteProvider<IQuote> provider,
        decimal brickSize,
        EndType endType) : base(provider)
    {
        Renko.Validate(brickSize);
        BrickSize = brickSize;
        EndType = endType;
        Name = $"RENKO({brickSize},{endType.ToString().ToUpperInvariant()})";

        Reinitialize();
    }

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
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        ToIndicator(item, notify, indexHint);
    }

    /// <inheritdoc />
    protected override (RenkoResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
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

    /// <summary>
    /// re/initialize last brick marker
    /// </summary>
    private void SetBaselineBrick()
    {
        int decimals = BrickSize.GetDecimalPlaces();

        IQuote q0 = ProviderCache[0];

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

    /// <summary>
    /// custom: build 0 to many bricks per quote
    /// </summary>
    /// <param name="item">Item to process</param>
    /// <param name="notify">Whether to notify observers</param>
    /// <param name="indexHint">Optional index hint for performance</param>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
    private void ToIndicator(IQuote item, bool notify, int? indexHint)
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
                IQuote pq = ProviderCache[w];

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

public static partial class Renko
{
    /// <summary>
    /// Converts a quote provider to a Renko hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>A Renko hub.</returns>
    public static RenkoHub ToRenkoHub(
        this IQuoteProvider<IQuote> quoteProvider,
        decimal brickSize,
        EndType endType = EndType.Close)
        => new(quoteProvider, brickSize, endType);
}
