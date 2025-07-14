namespace Skender.Stock.Indicators;

public static partial class Vortex
{
    // Vortex Indicator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Vortex Indicator")
            .WithId("VORTEX")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToVortex")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the Vortex calculation", isRequired: false, defaultValue: 14, minimum: 2, maximum: 100)
            .AddResult("Pvi", "VI+", ResultType.Default, isDefault: true)
            .AddResult("Nvi", "VI-", ResultType.Default, isDefault: false)
            .Build();
}
