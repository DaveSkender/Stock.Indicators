namespace Skender.Stock.Indicators;

public static partial class FisherTransform
{
    // FISHER Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Ehlers Fisher Transform")
            .WithId("FISHER")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToFisherTransform")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 10, minimum: 1, maximum: 250)
            .AddResult("Fisher", "Fisher", ResultType.Default, isReusable: true)
            .AddResult("Trigger", "Trigger", ResultType.Default)
            .Build();

    // No StreamListing for FISHER.
    // No BufferListing for FISHER.
}
