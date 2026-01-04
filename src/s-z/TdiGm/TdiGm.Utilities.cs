namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the TDI-GM indicator.
/// </summary>
public static partial class TdiGm
{
    /// <summary>
    /// Validates the parameters for the TDI-GM calculation.
    /// </summary>
    internal static void Validate(
        int rsiPeriod,
        int bandLength,
        int fastLength,
        int slowLength
        )
    {
        // check parameter arguments
        if (rsiPeriod <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriod), rsiPeriod,
                "RSI period must be greater than 0.");
        }

        if (rsiPeriod > 250)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriod), rsiPeriod,
                "RSI period must not exceed 250.");
        }
        
        if (bandLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bandLength), bandLength,
                "Band length must be greater than 0.");
        }

        if (bandLength > 250)
        {
            throw new ArgumentOutOfRangeException(nameof(bandLength), bandLength,
                "Band length must not exceed 250.");
        }

        if (fastLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastLength), fastLength,
                "Fast length must be greater than 0.");
        }

        if (fastLength > 250)
        {
            throw new ArgumentOutOfRangeException(nameof(fastLength), fastLength,
                "Fast length must not exceed 250.");
        }

        if (slowLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slowLength), slowLength,
                "Slow length must be greater than 0.");
        }

        if (slowLength > 250)
        {
            throw new ArgumentOutOfRangeException(nameof(slowLength), slowLength,
                "Slow length must not exceed 250.");
        }
    }
}
