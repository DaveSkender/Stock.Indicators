namespace Skender.Stock.Indicators;

public static partial class Mama
{
    // MAMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("MESA Adaptive Moving Average") // From catalog.bak.json
            .WithId("MAMA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            .WithMethodName("ToMama")
            .AddParameter<double>("fastLimit", "Fast Limit", defaultValue: 0.5, minimum: 0.01, maximum: 0.99) // From catalog.bak.json
            .AddParameter<double>("slowLimit", "Slow Limit", defaultValue: 0.05, minimum: 0.01, maximum: 0.99) // From catalog.bak.json
            .AddResult("Mama", "MAMA", ResultType.Default, isDefault: true) // From MamaResult model
            .AddResult("Fama", "FAMA", ResultType.Default, isDefault: false) // From MamaResult model
            .Build();

    // No StreamListing for MAMA.
    // No BufferListing for MAMA.
}
