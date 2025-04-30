namespace GeneratedCatalog;

// This class contains a method that should trigger our IND001 analyzer warning
public static class TestIndicator
{
    // This method will trigger IND001 because it's missing the required IndicatorAttribute
    public static IReadOnlyList<EmaResult> ToTestIndicator(this IReadOnlyList<Quote> quotes, int period) =>
        // This is just a simple test method to trigger our analyzer
        quotes.ToEma(period);

    [Series("GEN_TEST", "Generator Test", Category.PriceTrend, ChartType.Overlay, "Custom TEST Legend: [P1]")]
    public static IReadOnlyList<EmaResult> ToTestWithLegendOverride(
        this IReadOnlyList<Quote> quotes,
        [ParamNum<int>("Period", 1, 999, 14)] int period) =>
        quotes.ToEma(period);

    [Series("SERIES_TEST", "Series Test", Category.PriceTrend, ChartType.Overlay)]
    public static IReadOnlyList<EmaResult> ToSeriesWithoutOverride(
        this IReadOnlyList<Quote> quotes,
        [ParamNum<int>("Period", 1, 999, 14)] int period) =>
        quotes.ToEma(period);

    // The following method should trigger IND101 warnings for type mismatches
    [Series("TYPE_MISMATCH", "Type Mismatch Test", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<EmaResult> ToTypeMatchTest(
        this IReadOnlyList<Quote> quotes,
        [ParamNum<int>("Lookback Periods", 2, 250, 9)] int lookbackPeriods,
        [ParamNum<int>("Offset", 0, 1, 0.85)] double offset,
        [ParamNum<int>("Sigma", 0, 10, 6)] double sigma) =>
        quotes.ToEma(lookbackPeriods);

    // This test will trigger IND101 for enum parameter with ParamNum attribute
    [Series("ENUM_MISMATCH", "Enum Mismatch Test", Category.PriceTrend, ChartType.Overlay)]
    public static IReadOnlyList<EmaResult> ToEnumMismatchTest(
        this IReadOnlyList<Quote> quotes,
        [ParamNum<int>("Period", 1, 999, 14)] int period,
        [ParamNum<int>("Chart Type", 0, 5, 0)] ChartType chartType) =>
        quotes.ToEma(period);

    // This test will trigger IND101 for boolean parameter with ParamNum attribute
    [Series("BOOL_MISMATCH", "Boolean Mismatch Test", Category.PriceTrend, ChartType.Overlay)]
    public static IReadOnlyList<EmaResult> ToBoolMismatchTest(
        this IReadOnlyList<Quote> quotes,
        [ParamNum<int>("Period", 1, 999, 14)] int period,
        [ParamNum<int>("Use High/Low", 0, 1, 0)] bool useHighLow) =>
        quotes.ToEma(period);
}
