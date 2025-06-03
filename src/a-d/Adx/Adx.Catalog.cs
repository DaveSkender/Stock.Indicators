// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Adx
{
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Average Directional Index (ADX)")
            .WithId("ADX")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("adx", "Average Directional Index (ADX)", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing as [StreamIndicator] was not found

    public static readonly IndicatorListing BufferListing =
        new IndicatorListingBuilder()
            .WithName("Average Directional Index (ADX) (Buffer)")
            .WithId("ADX")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("adx", "Average Directional Index (ADX)", ResultType.Default, isDefault: true)
            .Build();
}
