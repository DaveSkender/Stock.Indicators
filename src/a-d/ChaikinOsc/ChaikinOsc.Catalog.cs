namespace Skender.Stock.Indicators;

public static partial class ChaikinOsc
{
    // CHAIKIN-OSC Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Chaikin Money Flow Oscillator")
            .WithId("CHAIKIN-OSC")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToChaikinOsc")
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 3, minimum: 1, maximum: 100)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 10, minimum: 1, maximum: 100)
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default)
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default)
            .AddResult("Adl", "ADL", ResultType.Default)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isReusable: true)
            .Build();

    // CHAIKIN-OSC Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for CHAIKIN-OSC.
    // No BufferListing for CHAIKIN-OSC.
}
