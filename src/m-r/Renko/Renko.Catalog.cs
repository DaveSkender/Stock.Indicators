namespace Skender.Stock.Indicators;

public static partial class Renko
{
    // Renko Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Renko Chart")
            .WithId("RENKO")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform)
            .AddParameter<double>("brickSize", "Brick Size", description: "The size of each Renko brick", isRequired: true, defaultValue: 1.0, minimum: 0.001, maximum: 1000000.0)
            .AddEnumParameter<EndType>("endType", "End Type", description: "The price candle end type to use as the brick threshold", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default, isDefault: false)
            .AddResult("High", "High", ResultType.Default, isDefault: false)
            .AddResult("Low", "Low", ResultType.Default, isDefault: false)
            .AddResult("Close", "Close", ResultType.Default, isDefault: true)
            .AddResult("Volume", "Volume", ResultType.Default, isDefault: false)
            .AddResult("IsUp", "Is Up", ResultType.Default, isDefault: false)
            .Build();

    // Renko Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Renko Chart")
            .WithId("RENKO")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceTransform)
            .AddParameter<double>("brickSize", "Brick Size", description: "The size of each Renko brick", isRequired: true, defaultValue: 1.0, minimum: 0.001, maximum: 1000000.0)
            .AddEnumParameter<EndType>("endType", "End Type", description: "The price candle end type to use as the brick threshold", isRequired: false, defaultValue: EndType.Close)
            .AddResult("Open", "Open", ResultType.Default, isDefault: false)
            .AddResult("High", "High", ResultType.Default, isDefault: false)
            .AddResult("Low", "Low", ResultType.Default, isDefault: false)
            .AddResult("Close", "Close", ResultType.Default, isDefault: true)
            .AddResult("Volume", "Volume", ResultType.Default, isDefault: false)
            .AddResult("IsUp", "Is Up", ResultType.Default, isDefault: false)
            .Build();
}