namespace Skender.Stock.Indicators;

public static partial class Cci
{
    // CCI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Commodity Channel Index (CCI)")
            .WithId("CCI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Cci", "CCI", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for CCI.
    // No BufferListing for CCI.
}
