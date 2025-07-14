namespace Skender.Stock.Indicators;

public static partial class Fcb
{
    // FCB Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Fractal Chaos Bands") // From catalog.bak.json
            .WithId("FCB") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel) // From catalog.bak.json Category: "PriceChannel"
            .WithMethodName("ToFcb")
            .AddParameter<int>("windowSpan", "Window Span", defaultValue: 2, minimum: 2, maximum: 30) // From catalog.bak.json
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: true) // From FcbResult model - primary result
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false) // From FcbResult model
            .Build();

    // No StreamListing for FCB.
    // No BufferListing for FCB.
}
