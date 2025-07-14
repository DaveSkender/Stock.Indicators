namespace Skender.Stock.Indicators;

public static partial class Marubozu
{
    // MARUBOZU Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Marubozu") // From catalog.bak.json
            .WithId("MARUBOZU") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.CandlestickPattern) // From catalog.bak.json Category: "CandlestickPattern"
            .WithMethodName("ToMarubozu")
            .AddParameter<double>("minBodyPercent", "Min Body Percent %", defaultValue: 95.0, minimum: 80.0, maximum: 100.0) // Default from C# method signature
            .AddResult("Match", "Match", ResultType.Default, isDefault: true) // Based on CandleResult.Match
            .Build();

    // No StreamListing for MARUBOZU.
    // No BufferListing for MARUBOZU.
}
