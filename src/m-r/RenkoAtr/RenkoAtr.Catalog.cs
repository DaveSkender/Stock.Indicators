namespace Skender.Stock.Indicators;

public static partial class RenkoAtr
{
    // Renko (ATR) Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Renko (ATR)")
            .WithId("RENKO-ATR")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("atrPeriods", "ATR Periods", description: "Number of periods for the ATR calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 100)
            .AddEnumParameter<EndType>("endType", "End Type", description: "Type of price to use for the calculation", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default, isDefault: false)
            .AddResult("High", "High", ResultType.Default, isDefault: false)
            .AddResult("Low", "Low", ResultType.Default, isDefault: false)
            .AddResult("Close", "Close", ResultType.Default, isDefault: true)
            .AddResult("Volume", "Volume", ResultType.Default, isDefault: false)
            .AddResult("IsUp", "Is Up", ResultType.Default, isDefault: false)
            .Build();
}
