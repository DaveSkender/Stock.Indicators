namespace Skender.Stock.Indicators;

public static partial class AtrStop
{
    /// <summary>
    /// Catalog listing for the ATR Trailing Stop indicator.
    /// </summary>
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("ATR Trailing Stop")
        .WithId("ATR-STOP")
        .WithStyle(Style.Series)
        .WithCategory(Category.PriceTrend)
        .AddParameter<int>("lookbackPeriods", "Lookback Periods",
            "Number of periods for the ATR calculation", defaultValue: 21, minimum: 1, maximum: 50)
        .AddParameter<double>("multiplier", "Multiplier",
            "Multiplier for ATR to determine stop distance", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
        .AddEnumParameter<EndType>("endType", "End Type",
            "Type of price data to use for calculation", defaultValue: EndType.Close)
        .AddResult("AtrStop", "ATR Trailing Stop", ResultType.Default, true)
        .AddResult("BuyStop", "Buy Stop", ResultType.Default, false)
        .AddResult("SellStop", "Sell Stop", ResultType.Default, false)
        .AddResult("Atr", "ATR", ResultType.Default, false)
        .Build();

    /// <summary>
    /// Provides catalog information for the ATR Trailing Stop indicator.
    /// </summary>
    /// <returns>IndicatorListing object containing catalog information.</returns>
    internal static IndicatorListing Catalog()
        => Listing;
}
