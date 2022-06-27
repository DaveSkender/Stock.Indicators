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

        int brickIndex = 1;
        int decimals = brickSize.GetDecimalPlaces();

        decimal o = Math.Round(q0.Close, Math.Max(decimals - 1, 0));
        decimal h = q0.Close;
        decimal l = q0.Close;
        decimal v = q0.Close;

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            TQuote q = quotesList[i];

            // accumulate brick info
            if (i == brickIndex)
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
            // TODO: add High/Low handling
            int newBrickQty = GetNewBricks(endType, q, o, brickSize);
            int absQty = Math.Abs(newBrickQty);

            // add new brick(s)
            if (newBrickQty != 0)
            {
                // can add more than one brick!
                for (int b = 0; b < absQty; b++)
                {
                    decimal brickClose = newBrickQty > 0 ?
                        o + brickSize : o - brickSize;

                    RenkoResult result = new(q.Date)
                    {
                        Open = o,
                        High = h,
                        Low = l,
                        Close = brickClose,
                        Volume = v / absQty,
                        IsUp = newBrickQty > 0
                    };
                    results.Add(result);
                    o = brickClose;
                }

                // init next brick(s)
                brickIndex = i + 1;
            }
        }

        return results;
    }

    // calculate brick size
    private static int GetNewBricks<TQuote>(
        EndType endType,
        TQuote q,
        decimal lastOpen,
        decimal brickSize)
        where TQuote : IQuote
    {
        switch (endType)
        {
            case EndType.Close:

                return (int)((q.Close - lastOpen) / brickSize);

            case EndType.HighLow:

                // high/low assumption: absolute greater diff wins
                // --> does not consider close direction

                decimal hQty = (q.High - lastOpen) / brickSize;
                decimal lQty = (q.Low - lastOpen) / brickSize;

                return (int)(Math.Abs(hQty) >= Math.Abs(lQty) ? hQty : lQty);

            default: return 0;
        }
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
