// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Atr
{
    // ATR Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Average True Range (ATR)") // From catalog.bak.json
            .WithId("ATR") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic) // From catalog.bak.json Category: "PriceCharacteristic"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("Tr", "True Range", ResultType.Default, isDefault: false) // From AtrResult model
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: true) // From AtrResult model, making Atr default
            .AddResult("Atrp", "ATR %", ResultType.Default, isDefault: false) // From AtrResult model
            .Build();

    // ATR Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Average True Range (ATR) (Stream)") // Adjusted name
            .WithId("ATR")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceCharacteristic)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Tr", "True Range", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: true)
            .AddResult("Atrp", "ATR %", ResultType.Default, isDefault: false)
            .Build();

    // No BufferListing for ATR.
}
