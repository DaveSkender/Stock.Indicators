using Skender.Stock.Indicators;

namespace TestNamespace;

// This class contains a method that should trigger our IND001 analyzer warning
public static class TestIndicator
{
    // This method will trigger IND001 because it's missing the required IndicatorAttribute
    public static IReadOnlyList<EmaResult> ToTestIndicator(this IReadOnlyList<Quote> quotes, int period) =>
        // This is just a simple test method to trigger our analyzer
        quotes.ToEma(period);

    [Series("GEN_TEST", "Generator Test", Skender.Stock.Indicators.Category.PriceTrend, Skender.Stock.Indicators.ChartType.Overlay, "Custom TEST Legend: [P1]")]
    public static IReadOnlyList<EmaResult> ToTestWithLegendOverride(
        this IReadOnlyList<Quote> quotes,
        [Param("Period", 1, 999, 14)] int period) =>
        quotes.ToEma(period);

    [Series("SERIES_TEST", "Series Test", Skender.Stock.Indicators.Category.PriceTrend, Skender.Stock.Indicators.ChartType.Overlay)]
    public static IReadOnlyList<EmaResult> ToSeriesWithoutOverride(
        this IReadOnlyList<Quote> quotes,
        [Param("Period", 1, 999, 14)] int period) =>
        quotes.ToEma(period);
}
