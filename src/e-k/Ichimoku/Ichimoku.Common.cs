namespace Skender.Stock.Indicators;

// ICHIMOKU CLOUD (COMMON)

public static class Ichimoku
{
    internal static void Validate(
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
    {
        // check parameter arguments
        if (tenkanPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tenkanPeriods), tenkanPeriods,
                "Tenkan periods must be greater than 0 for Ichimoku Cloud.");
        }

        if (kijunPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kijunPeriods), kijunPeriods,
                "Kijun periods must be greater than 0 for Ichimoku Cloud.");
        }

        if (senkouBPeriods <= kijunPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(senkouBPeriods), senkouBPeriods,
                "Senkou B periods must be greater than Kijun periods for Ichimoku Cloud.");
        }

        if (senkouOffset < 0 || chikouOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(senkouOffset), senkouOffset,
                "Senkou and Chikou offset periods must be non-negative for Ichimoku Cloud.");
        }
    }

}
