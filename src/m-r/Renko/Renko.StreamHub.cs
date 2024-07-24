namespace Skender.Stock.Indicators;

// RENKO CHART (STREAM HUB)

#region hub interface

public interface IRenkoHub
{
    decimal BrickSize { get; }
    EndType EndType { get; }
}
#endregion

public class RenkoHub<TIn> : QuoteObserver<TIn, RenkoResult>,
    IQuoteHub<TIn, RenkoResult>, IRenkoHub
    where TIn : IQuote
{
    #region constructors

    internal RenkoHub(
        IQuoteProvider<TIn> provider,
        decimal brickSize,
        EndType endType) : base(provider)
    {
        Renko.Validate(brickSize);
        BrickSize = brickSize;
        EndType = endType;

        Reinitialize();
    }
    #endregion

    public decimal BrickSize { get; }
    public EndType EndType { get; }

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        if (newIn is null)
        {
            throw new ArgumentNullException(nameof(newIn));
        }

        // get last brick
        RenkoResult lastBrick;

        if (Cache.Count != 0)
        {
            lastBrick = Cache
                .Where(c => c.Timestamp <= newIn.Timestamp)
                .Last();
        }
        else // no bricks yet
        {
            // skip first quote
            if (Provider.Results.Count <= 1)
            {
                return;
            }

            int decimals = BrickSize.GetDecimalPlaces();

            TIn q0 = Provider.Results[0];

            decimal baseline
                = Math.Round(q0.Close,
                    Math.Max(decimals - 1, 0));

            lastBrick = new(
                q0.Timestamp,
                Open: baseline, 0, 0,
                Close: baseline, 0, false);
        }

        // determine new brick quantity
        int newBrickQty
            = Renko.GetNewBrickQuantity(
                newIn, lastBrick, BrickSize, EndType);

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
            int inboundIndex = index ?? Provider.GetIndex(newIn, false);
            int lastBrickIndex = Provider.GetIndex(lastBrick.Timestamp, false);

            for (int w = lastBrickIndex + 1; w <= inboundIndex; w++)
            {
                TIn pq = Provider.Results[w];

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

                RenkoResult r
                    = new(newIn.Timestamp, o, h, l, c, v, isUp);

                lastBrick = r;

                // save and send
                Motify(act, r, null);
            }
        }
    }

    public override string ToString()
        => $"RENKO({BrickSize}, {EndType})";
}
