namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (COMMON)

public static class StochRsi
{
    // parameter validation
    internal static void Validate(
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
    {
        // check parameter arguments
        if (rsiPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                "RSI periods must be greater than 0 for Stochastic RSI.");
        }

        if (stochPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stochPeriods), stochPeriods,
                "STOCH periods must be greater than 0 for Stochastic RSI.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic RSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic RSI.");
        }
    }

}
