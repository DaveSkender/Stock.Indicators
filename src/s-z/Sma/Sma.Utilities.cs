namespace Skender.Stock.Indicators;

public static partial class Sma
{
    internal static double Increment<T>(
        this IReadOnlyList<T> values,
        int endIndex,
        int lookbackPeriods)
        where T : IReusable
    {
        int offset = lookbackPeriods - 1;

        if (endIndex < offset || endIndex >= values.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        int startIndex = endIndex - offset;

        for (int i = startIndex; i <= endIndex; i++)
        {
            sum += values[i].Value;
        }

        return sum / lookbackPeriods;
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
