namespace Skender.Stock.Indicators;

public static partial class ZigZag
{
    // Zig Zag Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Zig Zag (close)")
            .WithId("ZIGZAG-CLOSE")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToZigZag")
            .AddEnumParameter<EndType>("endType", "End Type", description: "Type of price to use for the calculation", isRequired: false, defaultValue: EndType.Close)
            .AddParameter<double>("percentChange", "Percent Change", description: "Minimum percent change required for a new direction", isRequired: false, defaultValue: 5.0, minimum: 1.0, maximum: 200.0)
            .AddResult("ZigZag", "Zig Zag", ResultType.Default, isDefault: true)
            .AddResult("PointType", "Point Type", ResultType.Default, isDefault: false)
            .AddResult("RetraceHigh", "Retrace High", ResultType.Default, isDefault: false)
            .AddResult("RetraceLow", "Retrace Low", ResultType.Default, isDefault: false)
            .Build();
}
