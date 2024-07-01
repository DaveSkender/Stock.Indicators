namespace Skender.Stock.Indicators;

// RENKO CHART - STANDARD (SERIES)

public static partial class Renko
{
    internal static List<RenkoResult> CalcRenko<TQuote>(
        this List<TQuote> quotesList,
        decimal brickSize,
        EndType endType)
        where TQuote : IQuote
    {
        // check parameter arguments
        Validate(brickSize);

        // initialize
        int length = quotesList.Count;
        List<RenkoResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        // first brick baseline
        TQuote q0 = quotesList[0];

        int decimals = brickSize.GetDecimalPlaces();
        decimal baseline = Math.Round(q0.Close, Math.Max(decimals - 1, 0));

        RenkoResult lastBrick = new() {
            Open = baseline,
            Close = baseline
        };

        // initialize high/low/volume tracking
        decimal h = decimal.MinValue;
        decimal l = decimal.MaxValue;
        decimal sumV = 0;  // cumulative

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            TQuote q = quotesList[i];

            // track high/low/volume between bricks
            h = Math.Max(h, q.High);
            l = Math.Min(l, q.Low);
            sumV += q.Volume;

            // determine new brick quantity
            int newBrickQty = GetNewBrickQuantity(q, lastBrick, brickSize, endType);
            int absBrickQty = Math.Abs(newBrickQty);
            bool isUp = newBrickQty >= 0;

            // add new brick(s)
            // can add more than one brick!
            for (int b = 0; b < absBrickQty; b++)
            {
                decimal o;
                decimal c;
                decimal v = sumV / absBrickQty;

                if (isUp)
                {
                    o = Math.Max(lastBrick.Open, lastBrick.Close);
                    c = o + brickSize;
                }
                else
                {
                    o = Math.Min(lastBrick.Open, lastBrick.Close);
                    c = o - brickSize;
                }

                RenkoResult r = new() {
                    Timestamp = q.Timestamp,
                    Open = o,
                    High = h,
                    Low = l,
                    Close = c,
                    Volume = v,
                    IsUp = isUp
                };

                results.Add(r);
                lastBrick = r;
            }

            // reset high/low/volume tracking
            if (absBrickQty != 0)
            {
                h = decimal.MinValue;
                l = decimal.MaxValue;
                sumV = 0;
            }
        }

        return results;
    }
}
