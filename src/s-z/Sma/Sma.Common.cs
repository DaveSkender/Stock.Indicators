using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Sma/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Sma : ChainProvider
{
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

    // increment calculation
    /// <include file='./info.xml' path='info/type[@name="increment-tuple"]/*' />
    ///
    public static double Increment(
      Collection<(DateTime Date, double Value)> priceList,
      int lookbackPeriods)
    {
        List<(DateTime, double)> tpList = priceList.ToSortedList();
        int length = tpList.Count;

        return length < lookbackPeriods
           ? double.NaN
           : Increment(tpList, length - 1, lookbackPeriods);
    }

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
        for (int i = 0; i <= length; i++)
        {
            sum += prices[i];
        }

        return sum / length;
    }

    internal static double Increment(
        List<(DateTime Date, double Value)> tpList,
        int index,
        int lookbackPeriods)
    {
        if (index < lookbackPeriods - 1)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = index - lookbackPeriods + 1; i <= index; i++)
        {
            sum += tpList[i].Value;
        }

        return sum / lookbackPeriods;
    }

    // TODO: this is too slow whe attempting to use on Adl, unused
    internal static double? Increment<TResult>(
        List<TResult> results,
        int index,
        int lookbackPeriods)
        where TResult : IReusableResult
    {
        if (index < lookbackPeriods - 1)
        {
            return null;
        }

        double? sum = 0;
        for (int i = index - lookbackPeriods + 1; i <= index; i++)
        {
            sum += results[i].Value;
        }

        return sum / lookbackPeriods;
    }
}
