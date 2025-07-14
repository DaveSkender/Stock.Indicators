namespace Skender.Stock.Indicators;

public static partial class Cmf
{
    // CMF Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chaikin Money Flow (CMF)")
            .WithId("CMF")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToCmf")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default)
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default)
            .AddResult("Cmf", "CMF", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for CMF.
    // No BufferListing for CMF.
}
