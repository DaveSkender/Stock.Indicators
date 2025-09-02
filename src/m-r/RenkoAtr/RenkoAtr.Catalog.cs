namespace Skender.Stock.Indicators;

public static partial class RenkoAtr
{
    // Renko (ATR) Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Renko (ATR)")
            .WithId("RENKO-ATR")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToRenkoAtr")
            .AddParameter<int>("atrPeriods", "ATR Periods", description: "Number of periods for the ATR calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 100)
            .AddEnumParameter<EndType>("endType", "End Type", description: "Type of price to use for the calculation", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default)
            .AddResult("High", "High", ResultType.Default)
            .AddResult("Low", "Low", ResultType.Default)
            .AddResult("Close", "Close", ResultType.Default, isReusable: true)
            .AddResult("Volume", "Volume", ResultType.Default)
            .AddResult("IsUp", "Is Up", ResultType.Default)
            .Build();
}
