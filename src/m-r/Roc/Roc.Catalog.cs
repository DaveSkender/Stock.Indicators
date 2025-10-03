namespace Skender.Stock.Indicators;

public static partial class Roc
{
    // Rate of Change Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Rate of Change")
            .WithId("ROC")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToRoc")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the ROC calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Roc", "ROC", ResultType.Default, isReusable: true)
            .Build();

    // Rate of Change Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // Rate of Change Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // Rate of Change Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
