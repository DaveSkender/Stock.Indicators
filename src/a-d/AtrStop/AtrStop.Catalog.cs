// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class AtrStop
{
    // ATR-STOP Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("ATR Trailing Stop") // From catalog.bak.json
            .WithId("ATR-STOP") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend) // From catalog.bak.json Category: "PriceTrend"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddParameter<int>("endType", "End Type", defaultValue: 0, minimum: 0, maximum: 1) // Maps to EndType enum, int in builder
            .AddResult("AtrStop", "ATR Stop", ResultType.Default, isDefault: true) // Default from model
            .AddResult("BuyStop", "Buy Stop", ResultType.Default, isDefault: false)
            .AddResult("SellStop", "Sell Stop", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: false)
            .Build();

    // ATR-STOP Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("ATR Trailing Stop (Stream)") // Adjusted name
            .WithId("ATR-STOP")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddParameter<int>("endType", "End Type", defaultValue: 0, minimum: 0, maximum: 1)
            .AddResult("AtrStop", "ATR Stop", ResultType.Default, isDefault: true)
            .AddResult("BuyStop", "Buy Stop", ResultType.Default, isDefault: false)
            .AddResult("SellStop", "Sell Stop", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: false)
            .Build();

    // No BufferListing for ATR-STOP.
}
