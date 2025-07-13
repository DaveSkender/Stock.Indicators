namespace Skender.Stock.Indicators;

public static partial class Beta
{
    // BETA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Beta")
            .WithId("BETA")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .AddParameter<IEnumerable<Quote>>("sourceEval", "Evaluated Prices")
            .AddParameter<IEnumerable<Quote>>("sourceMrkt", "Market Prices")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 50, minimum: 1, maximum: 250)
            .AddEnumParameter<BetaType>("type", "Beta Type", defaultValue: BetaType.Standard)
            .AddResult("Beta", "Beta", ResultType.Default, isDefault: true)
            .AddResult("BetaUp", "Beta Up", ResultType.Default, isDefault: false)
            .AddResult("BetaDown", "Beta Down", ResultType.Default, isDefault: false)
            .AddResult("Ratio", "Ratio", ResultType.Default, isDefault: false)
            .AddResult("Convexity", "Convexity", ResultType.Default, isDefault: false)
            .AddResult("ReturnsEval", "Returns Eval", ResultType.Default, isDefault: false)
            .AddResult("ReturnsMrkt", "Returns Mrkt", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for BETA.
    // No BufferListing for BETA.
}
