namespace Skender.Stock.Indicators;

public static partial class Cmo
{
    // CMO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chande Momentum Oscillator")
            .WithId("CMO")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToCmo")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Cmo", "CMO", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for CMO.
    // No BufferListing for CMO.
}
