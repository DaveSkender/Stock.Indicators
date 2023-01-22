namespace Skender.Stock.Indicators;

// RENKO CHART - STANDARD (SERIES)
public static partial class Indicator
{
    internal static List<RenkoResult> CalcRenko<TQuote>(
        this List<TQuote> quotesList,
        decimal brickSize,
        EndType endType)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateRenko(brickSize);

        // initialize
        int length = quotesList.Count;
        List<RenkoResult> results = new(length);
        TQuote q0;

        if (length == 0)
        {
            return results;
        }
        else
        {
            q0 = quotesList[0];
        }

        bool resetHLV = true;
        int decimals = brickSize.GetDecimalPlaces();
        decimal baseline = Math.Round(q0.Close, Math.Max(decimals - 1, 0));

        decimal h = decimal.MinValue;
        decimal l = decimal.MaxValue;
        decimal v = 0;

        RenkoResult lastBrick = new(q0.Date)
        {
            Open = baseline,
            Close = baseline
        };

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            TQuote q = quotesList[i];

            // accumulate brick info
            if (resetHLV)
            {
                // reset
                h = q.High;
                l = q.Low;
                v = q.Volume;
            }
            else
            {
                h = q.High > h ? q.High : h;
                l = q.Low < l ? q.Low : l;
                v += q.Volume;
            }

            // determine if new brick threshold is met
            int newBrickQty = GetNewBricks(endType, q, lastBrick, brickSize);
            int absQty = Math.Abs(newBrickQty);

            // add new brick(s)
            // can add more than one brick!
            for (int b = 0; b < absQty; b++)
            {
                decimal c;
                bool isUp = newBrickQty >= 0;

                if (newBrickQty > 0)
                {
                    baseline = Math.Max(lastBrick.Open, lastBrick.Close);
                    c = baseline + brickSize;
                }
                else
                {
                    baseline = Math.Min(lastBrick.Open, lastBrick.Close);
                    c = baseline - brickSize;
                }

                RenkoResult r = new(q.Date)
                {
                    Open = baseline,
                    High = h,
                    Low = l,
                    Close = c,
                    Volume = v / absQty,
                    IsUp = isUp
                };
                results.Add(r);
                lastBrick = r;
            }

            // init next brick(s)
            resetHLV = absQty != 0;
        }

        return results;
    }

    // calculate brick size
    private static int GetNewBricks<TQuote>(
        EndType endType,
        TQuote q,
        RenkoResult lastBrick,
        decimal brickSize)
        where TQuote : IQuote
    {
        int bricks;
        decimal upper = Math.Max(lastBrick.Open, lastBrick.Close);
        decimal lower = Math.Min(lastBrick.Open, lastBrick.Close);

        switch (endType)
        {
            case EndType.Close:

                bricks = q.Close > upper
                    ? (int)((q.Close - upper) / brickSize)
                    : q.Close < lower
                        ? (int)((q.Close - lower) / brickSize)
                        : 0;

                break;

            case EndType.HighLow:

                // high/low assumption: absolute greater diff wins
                // --> does not consider close direction

                decimal hQty = (q.High - upper) / brickSize;
                decimal lQty = (lower - q.Low) / brickSize;

                bricks = (int)((hQty >= lQty) ? hQty : -lQty);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(endType));
        }

        return bricks;
    }

    // parameter validation
    private static void ValidateRenko(
        decimal brickSize)
    {
        // check parameter arguments
        if (brickSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(brickSize), brickSize,
                "Brick size must be greater than 0 for Renko Charts.");
        }
    }
}
