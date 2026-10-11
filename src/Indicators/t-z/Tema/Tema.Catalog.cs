namespace FacioQuo.Stock.Indicators;

public static partial class Tema
{
    /// <summary>
    /// Triple Exponential Moving Average Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Triple Exponential Moving Average")
            .WithId("TEMA")
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the TEMA calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult(nameof(TemaResult.Tema), "TEMA", IndicatorResult.PricePane, ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// Triple Exponential Moving Average Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName(typeof(Tema), "ToTema")
            .Build();

    /// <summary>
    /// Triple Exponential Moving Average Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName(typeof(Tema), "ToTemaHub")
            .Build();

    /// <summary>
    /// Triple Exponential Moving Average Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName(typeof(Tema), "ToTemaList")
            .Build();
}
