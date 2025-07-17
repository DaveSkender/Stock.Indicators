namespace Skender.Stock.Indicators;

public static partial class AtrStop
{
    // ATR-STOP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("ATR Trailing Stop")
            .WithId("ATR-STOP")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAtrStop")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddEnumParameter<EndType>("endType", "End Type", defaultValue: EndType.Close)
            .AddResult("AtrStop", "ATR Trailing Stop", ResultType.Default, isReusable: true)
            .AddResult("BuyStop", "Buy Stop", ResultType.Default)
            .AddResult("SellStop", "Sell Stop", ResultType.Default)
            .AddResult("Atr", "ATR", ResultType.Default)
            .Build();

    // ATR-STOP Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("ATR Trailing Stop (Stream)")
            .WithId("ATR-STOP")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAtrStop")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddEnumParameter<EndType>("endType", "End Type", defaultValue: EndType.Close)
            .AddResult("AtrStop", "ATR Trailing Stop", ResultType.Default, isReusable: true)
            .AddResult("BuyStop", "Buy Stop", ResultType.Default)
            .AddResult("SellStop", "Sell Stop", ResultType.Default)
            .AddResult("Atr", "ATR", ResultType.Default)
            .Build();

    // No BufferListing for ATR-STOP.
}
