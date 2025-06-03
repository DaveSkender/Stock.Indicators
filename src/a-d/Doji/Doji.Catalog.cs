using System;
using System.Collections.Generic;
using Skender.Stock.Indicators; // This namespace should contain Match enum

namespace Skender.Stock.Indicators;

public static partial class Doji
{
    // DOJI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Doji")
            .WithId("DOJI")
            .WithStyle(Style.Series)
            .WithCategory(Category.CandlestickPattern)
            .AddParameter<double>("maxPriceChangePercent", "Max Price Change %", defaultValue: 0.1, minimum: 0.0, maximum: 0.5)
            .AddResult("Match", "Match", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for DOJI.
    // No BufferListing for DOJI.
}
