namespace Skender.Stock.Indicators;

public static partial class Chop
{
    // CHOP Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Choppiness Index")
            .WithId("CHOP")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Chop", "CHOP", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for CHOP.
    // No BufferListing for CHOP.
}
