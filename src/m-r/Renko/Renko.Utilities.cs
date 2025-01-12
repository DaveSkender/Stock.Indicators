/// <summary>
/// Provides utility methods for Renko chart calculations.
/// </summary>
public static partial class Renko
{
    /// <summary>
    /// Calculates the number of new bricks to be added based on the current quote and the last brick.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote values.</typeparam>
    /// <param name="q">The current quote.</param>
    /// <param name="lastBrick">The last Renko brick.</param>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The type of price to use for the end of the brick.</param>
    /// <returns>The number of new bricks to be added.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the end type is out of range.</exception>
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

    /// <summary>
    /// Validates the parameters for Renko chart calculations.
    /// </summary>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the brick size is less than or equal to 0.</exception>
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
