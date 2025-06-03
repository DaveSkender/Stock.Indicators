// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Dpo
{
    // DPO Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Detrended Price Oscillator") // From catalog.bak.json
            .WithId("DPO") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("Dpo", "DPO", ResultType.Default, isDefault: true) // From DpoResult model
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: false) // From DpoResult model
            .Build();

    // No StreamListing for DPO.
    // No BufferListing for DPO.
}
