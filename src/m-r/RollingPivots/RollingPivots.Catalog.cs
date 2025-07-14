namespace Skender.Stock.Indicators;

public static partial class RollingPivots
{
    // Rolling Pivots Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Rolling Pivots")
            .WithId("ROLLING-PIVOTS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToRollingPivots")
            .AddParameter<int>("windowPeriods", "Window Periods", description: "Number of periods for the rolling window", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddParameter<int>("offsetPeriods", "Offset Periods", description: "Number of periods to offset the pivots", isRequired: false, defaultValue: 0, minimum: 0, maximum: 100)
            .AddEnumParameter<PivotPointType>("pointType", "Point Type", description: "Type of pivot points to calculate", isRequired: false, defaultValue: PivotPointType.Standard)
            .AddResult("R3", "Resistance 3", ResultType.Default, isDefault: false)
            .AddResult("R2", "Resistance 2", ResultType.Default, isDefault: false)
            .AddResult("R1", "Resistance 1", ResultType.Default, isDefault: false)
            .AddResult("PP", "Pivot Point", ResultType.Default, isDefault: true)
            .AddResult("S1", "Support 1", ResultType.Default, isDefault: false)
            .AddResult("S2", "Support 2", ResultType.Default, isDefault: false)
            .AddResult("S3", "Support 3", ResultType.Default, isDefault: false)
            .Build();
}
