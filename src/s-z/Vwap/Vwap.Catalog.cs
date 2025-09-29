namespace Skender.Stock.Indicators;

public static partial class Vwap
{
    // VWAP Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Volume Weighted Average Price")
            .WithId("VWAP")
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToVwap")
            .AddDateParameter("startDate", "Start Date", description: "Starting date for VWAP calculation", isRequired: true)
            .AddResult("Vwap", "VWAP", ResultType.Default, isReusable: true)
            .AddResult("UpperBand", "Upper Band", ResultType.Default)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .Build();

    // VWAP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for VWAP.
    // No BufferListing for VWAP.
}
