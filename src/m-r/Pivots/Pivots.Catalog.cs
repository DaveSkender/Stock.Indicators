namespace Skender.Stock.Indicators;

public static partial class Pivots
{
    // Pivot Points Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Pivot Points")
            .WithId("PIVOTS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
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
