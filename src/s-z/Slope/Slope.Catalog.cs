namespace Skender.Stock.Indicators;

public static partial class Slope
{
    // Slope Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Slope")
            .WithId("SLOPE")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the slope calculation", isRequired: false, defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Slope", "Slope", ResultType.Default, isDefault: true)
            .AddResult("Intercept", "Intercept", ResultType.Default, isDefault: false)
            .AddResult("StdDev", "Standard Deviation", ResultType.Default, isDefault: false)
            .AddResult("RSquared", "R-Squared", ResultType.Default, isDefault: false)
            .Build();
}