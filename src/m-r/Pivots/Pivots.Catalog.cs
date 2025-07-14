namespace Skender.Stock.Indicators;

public static partial class Pivots
{
    // Pivot Points Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Pivot Points")
            .WithId("PIVOTS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToPivots")
            .AddEnumParameter<PivotPointType>("pointType", "Point Type", description: "Type of pivot points to calculate", isRequired: false, defaultValue: PivotPointType.Standard)
            .AddResult("R3", "Resistance 3", ResultType.Default)
            .AddResult("R2", "Resistance 2", ResultType.Default)
            .AddResult("R1", "Resistance 1", ResultType.Default)
            .AddResult("PP", "Pivot Point", ResultType.Default, isReusable: true)
            .AddResult("S1", "Support 1", ResultType.Default)
            .AddResult("S2", "Support 2", ResultType.Default)
            .AddResult("S3", "Support 3", ResultType.Default)
            .Build();
}
