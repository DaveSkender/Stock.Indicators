namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Sma/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>

public static class Sma
{

    // INCREMENT CALCULATIONS

    /// <include file='./info.xml' path='info/type[@name="increment-array"]/*' />
    ///
    public static double Increment(
      double[] prices)
    {
        if (prices is null || prices.Length == 0)
        {
            return double.NaN;
        }

        int length = prices.Length;

        double sum = 0;
        for (int i = 0; i < length; i++)
        {
            sum += prices[i];
        }

        return sum / length;
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
