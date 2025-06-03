// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators; // This namespace should contain EndType

namespace Skender.Stock.Indicators;

public static partial class AtrStop
{
    // ATR-STOP Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("ATR Trailing Stop")
            .WithId("ATR-STOP")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddEnumParameter<EndType>("endType", "End Type", defaultValue: EndType.Close) // Use AddEnumParameter; EndType.Close corresponds to 0
            .AddResult("AtrStop", "ATR Stop", ResultType.Default, isDefault: true)
            .AddResult("BuyStop", "Buy Stop", ResultType.Default, isDefault: false)
            .AddResult("SellStop", "Sell Stop", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: false)
            .Build();

    // ATR-STOP Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("ATR Trailing Stop (Stream)")
            .WithId("ATR-STOP")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 21, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddEnumParameter<EndType>("endType", "End Type", defaultValue: EndType.Close) // Use AddEnumParameter
            .AddResult("AtrStop", "ATR Stop", ResultType.Default, isDefault: true)
            .AddResult("BuyStop", "Buy Stop", ResultType.Default, isDefault: false)
            .AddResult("SellStop", "Sell Stop", ResultType.Default, isDefault: false)
            .AddResult("Atr", "ATR", ResultType.Default, isDefault: false)
            .Build();

    // No BufferListing for ATR-STOP.
}
