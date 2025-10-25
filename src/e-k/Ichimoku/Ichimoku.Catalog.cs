namespace Skender.Stock.Indicators;

public static partial class Ichimoku
{
    // ICHIMOKU Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Ichimoku Cloud")
            .WithId("ICHIMOKU")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToIchimoku")
            .AddParameter<int>("tenkanPeriods", "Tenkan Periods", defaultValue: 9, minimum: 1, maximum: 250)
            .AddParameter<int>("kijunPeriods", "Kijun Periods", defaultValue: 26, minimum: 2, maximum: 250)
            .AddParameter<int>("senkouBPeriods", "Senkou B Periods", defaultValue: 52, minimum: 3, maximum: 250)
            .AddResult("TenkanSen", "Tenkan-sen", ResultType.Default, isReusable: true)
            .AddResult("KijunSen", "Kijun-sen", ResultType.Default)
            .AddResult("SenkouSpanA", "Senkou Span A", ResultType.Default)
            .AddResult("SenkouSpanB", "Senkou Span B", ResultType.Default)
            .AddResult("ChikouSpan", "Chikou Span", ResultType.Default)
            .Build();

    // ICHIMOKU Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for ICHIMOKU.
    // No BufferListing for ICHIMOKU.
}
