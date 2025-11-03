namespace Skender.Stock.Indicators;

public static partial class RenkoAtr
{
    /// <summary>
    /// Renko (ATR) Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Renko (ATR)")
            .WithId("RENKO-ATR")
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("atrPeriods", "ATR Periods", description: "Number of periods for the ATR calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 100)
            .AddEnumParameter<EndType>("endType", "End Type", description: "Type of price to use for the calculation", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default)
            .AddResult("High", "High", ResultType.Default)
            .AddResult("Low", "Low", ResultType.Default)
            .AddResult("Close", "Close", ResultType.Default, isReusable: true)
            .AddResult("Volume", "Volume", ResultType.Default)
            .AddResult("IsUp", "Is Up", ResultType.Default)
            .Build();

    /// <summary>
    /// Renko (ATR) Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToRenkoAtr")
            .Build();

    /// <summary>
    /// Renko (ATR) Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // No StreamListing for Renko (ATR).
}
