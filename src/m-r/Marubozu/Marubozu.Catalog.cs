namespace Skender.Stock.Indicators;

public static partial class Marubozu
{
    // MARUBOZU Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Marubozu")
            .WithId("MARUBOZU")
            .WithCategory(Category.CandlestickPattern)
            .WithMethodName("ToMarubozu")
            .AddParameter<double>("minBodyPercent", "Min Body Percent %", defaultValue: 95.0, minimum: 80.0, maximum: 100.0) // Default from C# method signature
            .AddResult("Match", "Match", ResultType.Default, isReusable: true) // Based on CandleResult.Match
            .Build();

    // MARUBOZU Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for MARUBOZU.
    // No BufferListing for MARUBOZU.
}
