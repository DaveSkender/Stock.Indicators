namespace Skender.Stock.Indicators;

public static partial class Pivots
{
    // Pivot Points Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Pivot Points")
            .WithId("PIVOTS")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToPivots")
            .AddParameter<int>("leftSpan", "Left Span", description: "Number of periods to the left for pivot identification", isRequired: false, defaultValue: 2, minimum: 2, maximum: 100)
            .AddParameter<int>("rightSpan", "Right Span", description: "Number of periods to the right for pivot identification", isRequired: false, defaultValue: 2, minimum: 2, maximum: 100)
            .AddParameter<int>("maxTrendPeriods", "Max Trend Periods", description: "Maximum number of periods to track trend", isRequired: false, defaultValue: 20, minimum: 2, maximum: 1000)
            .AddEnumParameter<EndType>("endType", "End Type", description: "Type of price to use for pivot calculation", isRequired: false, defaultValue: EndType.HighLow)
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
