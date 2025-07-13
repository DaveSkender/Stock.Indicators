namespace Skender.Stock.Indicators;

public static partial class ChaikinOsc
{
    // CHAIKIN-OSC Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chaikin Money Flow Oscillator")
            .WithId("CHAIKIN-OSC")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 3, minimum: 1, maximum: 100)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 10, minimum: 1, maximum: 100)
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default, isDefault: false)
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default, isDefault: false)
            .AddResult("Adl", "ADL", ResultType.Default, isDefault: false)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for CHAIKIN-OSC.
    // No BufferListing for CHAIKIN-OSC.
}
