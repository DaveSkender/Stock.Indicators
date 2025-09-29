namespace Skender.Stock.Indicators;

public static partial class Correlation
{
    // CORR Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Correlation")
            .WithId("CORR")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToCorrelation")
            .AddSeriesParameter("sourceA", "Source A")
            .AddSeriesParameter("sourceB", "Source B")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("VarianceA", "Variance A", ResultType.Default)
            .AddResult("VarianceB", "Variance B", ResultType.Default)
            .AddResult("Covariance", "Covariance", ResultType.Default)
            .AddResult("Correlation", "Correlation", ResultType.Default, isReusable: true)
            .AddResult("RSquared", "R-Squared", ResultType.Default)
            .Build();

    // CORR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for CORR.
    // No BufferListing for CORR.
}
