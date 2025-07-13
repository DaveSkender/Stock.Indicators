namespace Skender.Stock.Indicators;

public static partial class Trix
{
    // TRIX Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Triple Exponential Moving Average Oscillator")
            .WithId("TRIX")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the TRIX calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Trix", "TRIX", ResultType.Default, isDefault: true)
            .Build();
}
