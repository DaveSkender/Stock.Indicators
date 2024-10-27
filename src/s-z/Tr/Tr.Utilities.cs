namespace Skender.Stock.Indicators;

// TRUE RANGE (UTILITIES)

public static partial class Tr
{
    public static double Increment(
        double high,
        double low,
        double prevClose)
    {
        double hmpc = Math.Abs(high - prevClose);
        double lmpc = Math.Abs(low - prevClose);

        return Math.Max(high - low, Math.Max(hmpc, lmpc));
    }
}
