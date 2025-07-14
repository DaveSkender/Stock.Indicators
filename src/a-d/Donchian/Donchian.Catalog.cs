namespace Skender.Stock.Indicators;

public static partial class Donchian
{
    // DONCHIAN Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Donchian Channels") // From catalog.bak.json
            .WithId("DONCHIAN") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel) // From catalog.bak.json Category: "PriceChannel"
            .WithMethodName("ToDonchian")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false) // From DonchianResult model
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: false) // From DonchianResult model
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false) // From DonchianResult model
            .AddResult("Width", "Width", ResultType.Default, isDefault: false) // From DonchianResult model
            .Build();

    // No StreamListing for DONCHIAN.
    // No BufferListing for DONCHIAN.
}
