namespace Skender.Stock.Indicators;

// RENKO CHART (STREAM HUB)

#region hub interface

public interface IRenkoHub
{
    decimal BrickSize { get; }
    EndType EndType { get; }
}
#endregion

public class RenkoHub<TIn>
    : QuoteProvider<TIn, RenkoResult>, IRenkoHub
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    private RenkoResult lastBrick
        = new(default, default, default,
            default, default, default, default);

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
    /// Renko hub settings.  Since it can produce 0 or many bricks per quote,
    /// the default 1:1 in/out is not used and must be skipped to prevent
    /// same-date triggerred rebuilds when caching.
    /// </summary>
    public override BinarySettings Properties { get; init; } = new(0b00000010);  // custom

    /// <summary>
    /// Standard brick size for Renko chart.
    /// </summary>
    public decimal BrickSize { get; }

    /// <summary>
    /// Close or High/Low price used to determine when threshold
    /// is met to generate new bricks.
    /// </summary>
    public EndType EndType { get; }

    // METHODS

    public override string ToString() => hubName;

    public override void OnAdd(TIn item, bool notify, int? indexHint)
        => ToIndicator(item, notify, indexHint);

    protected override (RenkoResult result, int index)
        ToIndicator(TIn item, int? indexHint)
        => throw new InvalidOperationException(); // not used

    // TODO: see if returning array of results is possible ^^
    // for all indicators, so we don't have to do this goofy override

    /// <summary>
    /// Restore last brick marker.
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
            int lastBrickIndex = ProviderCache.GetIndex(lastBrick.Timestamp, true);

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
