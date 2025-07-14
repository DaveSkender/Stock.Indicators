namespace Skender.Stock.Indicators;

public static partial class Donchian
{
    // DONCHIAN Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Donchian Channels")
            .WithId("DONCHIAN")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToDonchian")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false)
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: true)
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false)
            .AddResult("Width", "Width", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for DONCHIAN.
    // No BufferListing for DONCHIAN.
}
