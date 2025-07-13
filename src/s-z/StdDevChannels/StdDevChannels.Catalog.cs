namespace Skender.Stock.Indicators;

public static partial class StdDevChannels
{
    // Standard Deviation Channels Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Standard Deviation Channels")
            .WithId("STDEV-CHANNELS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the standard deviation calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddParameter<double>("stdDeviations", "Standard Deviations", description: "Number of standard deviations for the channels", isRequired: false, defaultValue: 2.0, minimum: 0.01, maximum: 10.0)
            .AddResult("UpperChannel", "Upper Channel", ResultType.Default, isDefault: false)
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: true)
            .AddResult("LowerChannel", "Lower Channel", ResultType.Default, isDefault: false)
            .Build();
}
