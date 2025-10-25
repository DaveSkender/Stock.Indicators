namespace Skender.Stock.Indicators;

public static partial class AtrStop
{
    // ATR-STOP Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("ATR Trailing Stop")
            .WithId("ATR-STOP")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAtrStop")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddEnumParameter<EndType>("endType", "End Type", defaultValue: EndType.Close)
            .AddResult("AtrStop", "ATR Trailing Stop", ResultType.Default, isReusable: true)
            .AddResult("BuyStop", "Buy Stop", ResultType.Default)
            .AddResult("SellStop", "Sell Stop", ResultType.Default)
            .AddResult("Atr", "ATR", ResultType.Default)
            .Build();

    // ATR-STOP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // ATR-STOP Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // No BufferListing for ATR-STOP.
}
