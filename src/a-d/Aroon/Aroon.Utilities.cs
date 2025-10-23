namespace Skender.Stock.Indicators;

public static partial class Aroon
{
    /// <summary>
    /// parameter validation
    /// </summary>
    /// <param name="lookbackPeriods"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Aroon.");
        }
    }
}
