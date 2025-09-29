namespace Skender.Stock.Indicators;

public static partial class ElderRay
{
    // ELDER-RAY Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Elder-ray Index")
            .WithId("ELDER-RAY")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToElderRay")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 13, minimum: 1, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default)
            .AddResult("BullPower", "Bull Power", ResultType.Default)
            .AddResult("BearPower", "Bear Power", ResultType.Default)
            .AddResult("Value", "Elder Ray", ResultType.Default, isReusable: true) // Calculated value (BullPower + BearPower) for IReusable.Value
            .Build();

    // ELDER-RAY Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for ELDER-RAY.
    // No BufferListing for ELDER-RAY.
}
