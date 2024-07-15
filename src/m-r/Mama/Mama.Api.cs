namespace Skender.Stock.Indicators;

// MOTHER of ADAPTIVE MOVING AVERAGES - MAMA (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<MamaResult> GetMama<T>(
        this IEnumerable<T> results,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
        where T : IReusable
        => results
            .ToSortedList(CandlePart.HL2)
            .CalcMama(fastLimit, slowLimit);
}
