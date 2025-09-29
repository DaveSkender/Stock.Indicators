namespace Skender.Stock.Indicators;

public static partial class Mama
{
    // MAMA Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("MESA Adaptive Moving Average")
            .WithId("MAMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToMama")
            .AddParameter<double>("fastLimit", "Fast Limit", defaultValue: 0.5, minimum: 0.01, maximum: 0.99)
            .AddParameter<double>("slowLimit", "Slow Limit", defaultValue: 0.05, minimum: 0.01, maximum: 0.99)
            .AddResult("Mama", "MAMA", ResultType.Default, isReusable: true)
            .AddResult("Fama", "FAMA", ResultType.Default)
            .Build();

    // MAMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for MAMA.
    // No BufferListing for MAMA.
}
