using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Dpo
{
    // DPO Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Detrended Price Oscillator")
            .WithId("DPO")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Dpo", "DPO", ResultType.Default, isDefault: true)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for DPO.
    // No BufferListing for DPO.
}
