namespace Skender.Stock.Indicators;

public static partial class PivotPoints
{
    // Pivot Points Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Pivot Points")
            .WithId("PIVOT-POINTS")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToPivotPoints")
            .AddEnumParameter<PeriodSize>("windowSize", "Window Size", description: "Size of the window for pivot calculation", isRequired: false, defaultValue: PeriodSize.Month)
            .AddEnumParameter<PivotPointType>("pointType", "Point Type", description: "Type of pivot points to calculate", isRequired: false, defaultValue: PivotPointType.Standard)
            .AddResult("R3", "Resistance 3", ResultType.Default)
            .AddResult("R2", "Resistance 2", ResultType.Default)
            .AddResult("R1", "Resistance 1", ResultType.Default)
            .AddResult("PP", "Pivot Point", ResultType.Default, isReusable: true)
            .AddResult("S1", "Support 1", ResultType.Default)
            .AddResult("S2", "Support 2", ResultType.Default)
            .AddResult("S3", "Support 3", ResultType.Default)
            .Build();

    // Pivot Points Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for Pivot Points.
    // No BufferListing for Pivot Points.
}
