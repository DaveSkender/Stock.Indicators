namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Ema/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Ema : ChainProvider
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }

    // increment calculations
    /// <include file='./info.xml' path='info/type[@name="increment-lookback"]/*' />
    ///
    public static double Increment(
        int lookbackPeriods,
        double lastEma,
        double newPrice)
    {
        double k = 2d / (lookbackPeriods + 1);
        return Increment(k, lastEma, newPrice);
    }

    /// <include file='./info.xml' path='info/type[@name="increment-k"]/*' />
    ///
    public static double Increment(double k, double lastEma, double newPrice)
        => lastEma + (k * (newPrice - lastEma));
}
