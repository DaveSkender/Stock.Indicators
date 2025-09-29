namespace Skender.Stock.Indicators;

public static partial class Atr
{
    // ATR Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Average True Range (ATR)")
            .WithId("ATR")
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToAtr")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Tr", "True Range", ResultType.Default)
            .AddResult("Atr", "ATR", ResultType.Default)
            .AddResult("Atrp", "ATR %", ResultType.Default, isReusable: true)
            .Build();

    // ATR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // ATR Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // No BufferListing for ATR.
}
