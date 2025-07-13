namespace Skender.Stock.Indicators;

public static partial class Vwap
{
    // VWAP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Volume Weighted Average Price")
            .WithId("VWAP")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .AddParameter<DateTime>("startDate", "Start Date", description: "Starting date for VWAP calculation", isRequired: true)
            .AddResult("Vwap", "VWAP", ResultType.Default, isDefault: true)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false)
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false)
            .Build();
}
