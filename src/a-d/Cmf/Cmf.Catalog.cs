namespace Skender.Stock.Indicators;

public static partial class Cmf
{
    // CMF Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chaikin Money Flow (CMF)")
            .WithId("CMF")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default, isDefault: false)
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default, isDefault: false)
            .AddResult("Cmf", "CMF", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for CMF.
    // No BufferListing for CMF.
}
