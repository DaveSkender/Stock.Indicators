namespace Skender.Stock.Indicators;

public static partial class Ultimate
{
    // Ultimate Oscillator Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Ultimate Oscillator")
            .WithId("UO")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToUltimate")
            .AddParameter<int>("shortPeriods", "Short Periods", description: "Number of short periods for the Ultimate Oscillator calculation", isRequired: false, defaultValue: 7, minimum: 1, maximum: 50)
            .AddParameter<int>("middlePeriods", "Middle Periods", description: "Number of middle periods for the Ultimate Oscillator calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 50)
            .AddParameter<int>("longPeriods", "Long Periods", description: "Number of long periods for the Ultimate Oscillator calculation", isRequired: false, defaultValue: 28, minimum: 1, maximum: 250)
            .AddResult("Ultimate", "Ultimate Oscillator", ResultType.Default, isReusable: true)
            .Build();

    // Ultimate Oscillator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for Ultimate Oscillator.
    // No BufferListing for Ultimate Oscillator.
}
