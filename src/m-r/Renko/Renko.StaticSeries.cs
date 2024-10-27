namespace Skender.Stock.Indicators;

// RENKO CHART (SERIES)

public static partial class Renko
{
    public static IReadOnlyList<RenkoResult> ToRenko<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        decimal brickSize,
        EndType endType = EndType.Close)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(brickSize);

        // initialize
        int length = quotes.Count;
        List<RenkoResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        // first brick baseline
        TQuote q0 = quotes[0];

        int decimals = brickSize.GetDecimalPlaces();
        decimal baseline = Math.Round(q0.Close, Math.Max(decimals - 1, 0));

        RenkoResult lastBrick = new(
            q0.Timestamp,
            Open: baseline, 0, 0,
            Close: baseline, 0, false);

        // initialize high/low/volume tracking
        decimal h = decimal.MinValue;
        decimal l = decimal.MaxValue;
        decimal sumV = 0;  // cumulative

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            TQuote q = quotes[i];

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

                RenkoResult r
                    = new(q.Timestamp, o, h, l, c, v, isUp);

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
