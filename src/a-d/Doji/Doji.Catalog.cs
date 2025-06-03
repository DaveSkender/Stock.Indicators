// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators; // This namespace should contain Match enum

namespace Skender.Stock.Indicators;

public static partial class Doji
{
    // DOJI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Doji") // From catalog.bak.json
            .WithId("DOJI") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.CandlestickPattern) // From catalog.bak.json Category: "CandlestickPattern"
            .AddParameter<double>("maxPriceChangePercent", "Max Price Change %", defaultValue: 0.1, minimum: 0.0, maximum: 0.5) // From catalog.bak.json
            // Result based on CandleResult.Match, mapped to "doji" DataName from catalog.bak.json
            .AddResult("Match", "Match", ResultType.Default, isDefault: true)
            // Optionally, can also add Price if it's considered a direct output of the pattern
            // .AddResult("Price", "Price", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for DOJI.
    // No BufferListing for DOJI.
}
