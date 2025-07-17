namespace Skender.Stock.Indicators;

public static partial class Hurst
{
    // HURST Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Hurst Exponent")
            .WithId("HURST")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToHurst")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 100, minimum: 2, maximum: 250)
            .AddResult("HurstExponent", "Hurst Exponent", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for HURST.
    // No BufferListing for HURST.
}
