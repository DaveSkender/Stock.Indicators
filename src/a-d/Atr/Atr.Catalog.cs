namespace Skender.Stock.Indicators;

public static partial class Atr
{
    // ATR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Average True Range (ATR)")
            .WithId("ATR")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToAtr")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Tr", "True Range", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: true)
            .AddResult("Atrp", "ATR %", ResultType.Default, isDefault: false)
            .Build();

    // ATR Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Average True Range (ATR) (Stream)")
            .WithId("ATR")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToAtr")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Tr", "True Range", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: true)
            .AddResult("Atrp", "ATR %", ResultType.Default, isDefault: false)
            .Build();

    // No BufferListing for ATR.
}
