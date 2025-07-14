namespace Skender.Stock.Indicators;

public static partial class Ichimoku
{
    // ICHIMOKU Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Ichimoku Cloud")
            .WithId("ICHIMOKU")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToIchimoku")
            .AddParameter<int>("tenkanPeriods", "Tenkan Periods", defaultValue: 9, minimum: 1, maximum: 250)
            .AddParameter<int>("kijunPeriods", "Kijun Periods", defaultValue: 26, minimum: 2, maximum: 250)
            .AddParameter<int>("senkouBPeriods", "Senkou B Periods", defaultValue: 52, minimum: 3, maximum: 250)
            .AddResult("TenkanSen", "Tenkan-sen", ResultType.Default, isDefault: true)
            .AddResult("KijunSen", "Kijun-sen", ResultType.Default, isDefault: false)
            .AddResult("SenkouSpanA", "Senkou Span A", ResultType.Default, isDefault: false)
            .AddResult("SenkouSpanB", "Senkou Span B", ResultType.Default, isDefault: false)
            .AddResult("ChikouSpan", "Chikou Span", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for ICHIMOKU.
    // No BufferListing for ICHIMOKU.
}
