namespace Skender.Stock.Indicators;

public static partial class Tema
{
    // Triple Exponential Moving Average Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Triple Exponential Moving Average")
            .WithId("TEMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToTema")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the TEMA calculation", isRequired: false, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Tema", "TEMA", ResultType.Default, isReusable: true)
            .Build();

    // Triple Exponential Moving Average Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for TEMA.

    // Triple Exponential Moving Average Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // Triple Exponential Moving Average Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
