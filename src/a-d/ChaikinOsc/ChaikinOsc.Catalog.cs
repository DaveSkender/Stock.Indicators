namespace Skender.Stock.Indicators;

public static partial class ChaikinOsc
{
    /// <summary>
    /// CHAIKIN-OSC Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Chaikin Money Flow Oscillator")
            .WithId("CHAIKIN-OSC")
            .WithCategory(Category.VolumeBased)
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 3, minimum: 1, maximum: 100)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 10, minimum: 1, maximum: 100)
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default)
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default)
            .AddResult("Adl", "ADL", ResultType.Default)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// CHAIKIN-OSC Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToChaikinOsc")
            .Build();

    // No StreamListing for CHAIKIN-OSC.
    // No BufferListing for CHAIKIN-OSC.
}
