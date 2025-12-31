namespace Skender.Stock.Indicators;

public static partial class Beta
{
    /// <summary>
    /// BETA Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Beta")
            .WithId("BETA")
            .WithCategory(Category.PriceCharacteristic)
            .AddSeriesParameter("sourceEval", "Evaluated Prices")
            .AddSeriesParameter("sourceMrkt", "Market Prices")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 50, minimum: 1, maximum: 250)
            .AddEnumParameter<BetaType>("type", "Beta Type", defaultValue: BetaType.Standard)
            .AddResult("Beta", "Beta", ResultType.Default, isReusable: true)
            .AddResult("BetaUp", "Beta Up", ResultType.Default)
            .AddResult("BetaDown", "Beta Down", ResultType.Default)
            .AddResult("Ratio", "Ratio", ResultType.Default)
            .AddResult("Convexity", "Convexity", ResultType.Default)
            .AddResult("ReturnsEval", "Returns Eval", ResultType.Default)
            .AddResult("ReturnsMrkt", "Returns Mrkt", ResultType.Default)
            .Build();

    /// <summary>
    /// BETA Series Listing
    /// </summary>
    /// <remarks>
    /// Note: BufferList and StreamHub listings were removed due to PairsProvider synchronization challenges.
    /// See docs/plans/pairhubs.plan.md for details.
    /// </remarks>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToBeta")
            .Build();
}
