namespace Skender.Stock.Indicators;

// RENKO CHART (STREAMING)

#region hub interface

public interface IRenkoHub
{
    decimal BrickSize { get; }
    EndType EndType { get; }
}
#endregion

public class RenkoHub<TQuote>
    : ChainProvider<RenkoResult>, IStreamHub<TQuote, RenkoResult>, IRenkoHub
    where TQuote : struct, IQuote
{
    private readonly StreamCache<RenkoResult> _cache;
    private readonly StreamObserver<TQuote, RenkoResult> _observer;
    private readonly QuoteProvider<TQuote> _supplier;

    #region constructors

    public RenkoHub(
        QuoteProvider<TQuote> provider,
        decimal brickSize,
        EndType endType)
        : this(provider, cache: new(),
              brickSize, endType)
    { }

    private RenkoHub(
        QuoteProvider<TQuote> provider,
        StreamCache<RenkoResult> cache,
        decimal brickSize,
        EndType endType) : base(cache)
    {
        Renko.Validate(brickSize);
        BrickSize = brickSize;
        EndType = endType;

        _cache = cache;
        _supplier = provider;
        _observer = new(this, this, provider);
    }
    #endregion

    public decimal BrickSize { get; }
    public EndType EndType { get; }


    // METHODS

    public override string ToString()
        => $"RENKO({BrickSize}, {EndType}) - {_cache.Cache.Count} items";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextNew(TQuote newItem)
    {
        // get last brick
        RenkoResult lastBrick;

        if (_cache.Cache.Count != 0)
        {
            lastBrick = Results
                .Where(c => c.Timestamp <= newItem.Timestamp)
                .Last();
        }
        else // no bricks yet
        {
            // skip first quote
            if (_supplier.Cache.Count <= 1)
            {
                return;
            }

            int decimals = BrickSize.GetDecimalPlaces();

            TQuote q0 = _supplier.ReadCache[0];

            decimal baseline
                = Math.Round(q0.Close,
                    Math.Max(decimals - 1, 0));

            lastBrick = new() {
                Timestamp = q0.Timestamp,
                Open = baseline,
                Close = baseline
            };
        }

        // determine new brick quantity
        int newBrickQty
            = Renko.GetNewBrickQuantity(
                newItem, lastBrick, BrickSize, EndType);

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
            int inboundIndex = _supplier.Position(newItem);
            int lastBrickIndex = _supplier.Position(lastBrick.Timestamp);

            for (int w = lastBrickIndex + 1; w <= inboundIndex; w++)
            {
                TQuote pq = _supplier.ReadCache[w];

                h = Math.Max(h, pq.High);
                l = Math.Min(l, pq.Low);
                sumV += pq.Volume;
            }

            decimal v = sumV / absBrickQty;

            for (int b = 0; b < absBrickQty; b++)
            {
                decimal o;
                decimal c;

                if (isUp)
                {
                    o = Math.Max(lastBrick.Open, lastBrick.Close);
                    c = o + BrickSize;
                }
                else
                {
                    o = Math.Min(lastBrick.Open, lastBrick.Close);
                    c = o - BrickSize;
                }

                RenkoResult r = new() {
                    Timestamp = newItem.Timestamp,
                    Open = o,
                    High = h,
                    Low = l,
                    Close = c,
                    Volume = v,
                    IsUp = isUp
                };

                // save to cache
                Act act = _cache.Modify(Act.AddNew, r);

                // send to observers
                NotifyObservers(act, r);

                lastBrick = r;
            }
        }
    }
}
