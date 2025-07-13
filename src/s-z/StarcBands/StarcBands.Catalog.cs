namespace Skender.Stock.Indicators;

public static partial class StarcBands
{
    // STARC Bands Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("STARC Bands")
            .WithId("STARC")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .AddParameter<int>("smaPeriods", "SMA Periods", description: "Number of periods for the SMA calculation", isRequired: false, defaultValue: 5, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", description: "Multiplier for the ATR calculation", isRequired: false, defaultValue: 2.0, minimum: 1.0, maximum: 10.0)
            .AddParameter<int>("atrPeriods", "ATR Periods", description: "Number of periods for the ATR calculation", isRequired: false, defaultValue: 10, minimum: 1, maximum: 50)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false)
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: true)
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false)
            .Build();
}
