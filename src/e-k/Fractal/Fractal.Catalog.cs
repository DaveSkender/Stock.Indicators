namespace Skender.Stock.Indicators;

public static partial class Fractal
{
    // Fractal Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Williams Fractal (high/low)")
            .WithId("FRACTAL")
            .WithStyle(Style.Series)
            .WithCategory(Category.PricePattern)
            .WithMethodName("ToFractal")
            .AddParameter<int>("windowSpan", "Window Span", description: "Number of periods to look back and forward for the calculation", isRequired: false, defaultValue: 2, minimum: 1, maximum: 100)
            .AddEnumParameter<EndType>("endType", "End Type", description: "Type of price to use for the calculation", isRequired: false, defaultValue: EndType.HighLow)
            .AddResult("FractalBear", "Bear Fractal", ResultType.Default)
            .AddResult("FractalBull", "Bull Fractal", ResultType.Default, isReusable: true)
            .Build();
}
