namespace Skender.Stock.Indicators;

public static partial class Ultimate
{
    // Ultimate Oscillator Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Ultimate Oscillator")
            .WithId("UO")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("shortPeriods", "Short Periods", description: "Number of short periods for the Ultimate Oscillator calculation", isRequired: false, defaultValue: 7, minimum: 1, maximum: 50)
            .AddParameter<int>("middlePeriods", "Middle Periods", description: "Number of middle periods for the Ultimate Oscillator calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 50)
            .AddParameter<int>("longPeriods", "Long Periods", description: "Number of long periods for the Ultimate Oscillator calculation", isRequired: false, defaultValue: 28, minimum: 1, maximum: 250)
            .AddResult("UltimateOscillator", "Ultimate Oscillator", ResultType.Default, isDefault: true)
            .Build();
}