namespace Skender.Stock.Indicators;

// RENKO CHART (UTILITIES)

public static partial class Renko
{
    // calculate brick size
    internal static int GetNewBrickQuantity<TQuote>(
        TQuote q,
        RenkoResult lastBrick,
        decimal brickSize,
        EndType endType)
        where TQuote : IQuote
    {
        int brickQuantity;
        decimal upper = Math.Max(lastBrick.Open, lastBrick.Close);
        decimal lower = Math.Min(lastBrick.Open, lastBrick.Close);

        switch (endType)
        {
            case EndType.Close:

                brickQuantity
                    = q.Close > upper
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

                brickQuantity = (int)(hQty >= lQty ? hQty : -lQty);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(endType));
        }

        return brickQuantity;
    }

    // parameter validation
    internal static void Validate(
        decimal brickSize)
    {
        // check parameter arguments
        if (brickSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(brickSize), brickSize,
                "Brick size must be greater than 0 for Renko Charts.");
        }
    }

}
