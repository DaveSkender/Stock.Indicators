namespace Skender.Stock.Indicators;

public static partial class Renko
{
    // Renko Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Renko Chart")
            .WithId("RENKO")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToRenko")
            .AddParameter<double>("brickSize", "Brick Size", description: "The size of each Renko brick", isRequired: true, defaultValue: 1.0, minimum: 0.001, maximum: 1000000.0)
            .AddEnumParameter<EndType>("endType", "End Type", description: "The price candle end type to use as the brick threshold", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default)
            .AddResult("High", "High", ResultType.Default)
            .AddResult("Low", "Low", ResultType.Default)
            .AddResult("Close", "Close", ResultType.Default, isReusable: true)
            .AddResult("Volume", "Volume", ResultType.Default)
            .AddResult("IsUp", "Is Up", ResultType.Default)
            .Build();

    // Renko Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder()
            .WithName("Renko Chart")
            .WithId("RENKO")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToRenko")
            .AddParameter<double>("brickSize", "Brick Size", description: "The size of each Renko brick", isRequired: true, defaultValue: 1.0, minimum: 0.001, maximum: 1000000.0)
            .AddEnumParameter<EndType>("endType", "End Type", description: "The price candle end type to use as the brick threshold", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default)
            .AddResult("High", "High", ResultType.Default)
            .AddResult("Low", "Low", ResultType.Default)
            .AddResult("Close", "Close", ResultType.Default, isReusable: true)
            .AddResult("Volume", "Volume", ResultType.Default)
            .AddResult("IsUp", "Is Up", ResultType.Default)
            .Build();
}
