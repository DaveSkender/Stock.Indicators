namespace Skender.Stock.Indicators;

public static partial class Adx
{
    // ADX Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Average Directional Index (ADX)")
            .WithId("ADX")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAdx")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Pdi", "+DI", ResultType.Default)
            .AddResult("Mdi", "-DI", ResultType.Default)
            .AddResult("Dx", "DX", ResultType.Default)
            .AddResult("Adx", "ADX", ResultType.Default, isReusable: true)
            .AddResult("Adxr", "ADXR", ResultType.Default)
            .Build();

    // No StreamListing for ADX.

    // ADX Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new IndicatorListingBuilder()
            .WithName("Average Directional Index (ADX) (Buffer)")
            .WithId("ADX")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAdx")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Pdi", "+DI", ResultType.Default)
            .AddResult("Mdi", "-DI", ResultType.Default)
            .AddResult("Dx", "DX", ResultType.Default)
            .AddResult("Adx", "ADX", ResultType.Default, isReusable: true)
            .AddResult("Adxr", "ADXR", ResultType.Default)
            .Build();
}
