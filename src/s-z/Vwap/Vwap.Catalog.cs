namespace Skender.Stock.Indicators;

public static partial class Vwap
{
    // VWAP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Volume Weighted Average Price")
            .WithId("VWAP")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToVwap")
            .AddDateParameter("startDate", "Start Date", description: "Starting date for VWAP calculation", isRequired: true)
            .AddResult("Vwap", "VWAP", ResultType.Default, isReusable: true)
            .AddResult("UpperBand", "Upper Band", ResultType.Default)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .Build();
}
