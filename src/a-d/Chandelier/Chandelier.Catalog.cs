namespace Skender.Stock.Indicators;

public static partial class Chandelier
{
    // CHEXIT Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Chandelier Exit")
            .WithId("CHEXIT")
            .WithStyle(Style.Series)
            .WithCategory(Category.StopAndReverse)
            .WithMethodName("ToChandelier")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 22, minimum: 1, maximum: 250)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 1.0, maximum: 10.0)
            .AddEnumParameter<Direction>("type", "Direction", defaultValue: Direction.Long)
            .AddResult("ChandelierExit", "Chandelier Exit", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for CHEXIT.
    // No BufferListing for CHEXIT.
}
