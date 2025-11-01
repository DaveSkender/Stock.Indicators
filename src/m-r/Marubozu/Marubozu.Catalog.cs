namespace Skender.Stock.Indicators;

public static partial class Marubozu
{
    /// <summary>
    /// MARUBOZU Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Marubozu")
            .WithId("MARUBOZU")
            .WithCategory(Category.CandlestickPattern)
            .WithMethodName("ToMarubozu")
            .AddParameter<double>("minBodyPercent", "Min Body Percent %", defaultValue: 95.0, minimum: 80.0, maximum: 100.0) // Default from C# method signature
            .AddResult("Match", "Match", ResultType.Default, isReusable: true) // Based on CandleResult.Match
            .Build();

    /// <summary>
    /// MARUBOZU Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// MARUBOZU Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // No BufferListing for MARUBOZU.
}
