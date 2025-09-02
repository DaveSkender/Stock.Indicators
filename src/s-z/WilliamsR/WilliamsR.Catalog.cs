namespace Skender.Stock.Indicators;

public static partial class WilliamsR
{
    // Williams %R Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Williams %R")
            .WithId("WILLR")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToWilliamsR")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the Williams %R calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("WilliamsR", "Williams %R", ResultType.Default, isReusable: true)
            .Build();
}
