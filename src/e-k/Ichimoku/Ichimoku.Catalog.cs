namespace Skender.Stock.Indicators;

public static partial class Ichimoku
{
    // ICHIMOKU Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Ichimoku Cloud") // From catalog.bak.json
            .WithId("ICHIMOKU") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend) // From catalog.bak.json Category: "PriceTrend"
            .AddParameter<int>("tenkanPeriods", "Tenkan Periods", defaultValue: 9, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("kijunPeriods", "Kijun Periods", defaultValue: 26, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("senkouBPeriods", "Senkou B Periods", defaultValue: 52, minimum: 3, maximum: 250) // From catalog.bak.json
            .AddResult("TenkanSen", "Tenkan-sen", ResultType.Default, isDefault: true) // From IchimokuResult model
            .AddResult("KijunSen", "Kijun-sen", ResultType.Default, isDefault: false) // From IchimokuResult model
            .AddResult("SenkouSpanA", "Senkou Span A", ResultType.Default, isDefault: false) // From IchimokuResult model
            .AddResult("SenkouSpanB", "Senkou Span B", ResultType.Default, isDefault: false) // From IchimokuResult model
            .AddResult("ChikouSpan", "Chikou Span", ResultType.Default, isDefault: false) // From IchimokuResult model
            .Build();

    // No StreamListing for ICHIMOKU.
    // No BufferListing for ICHIMOKU.
}
