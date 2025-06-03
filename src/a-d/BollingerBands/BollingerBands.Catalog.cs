// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class BollingerBands
{
    // BB Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Bollinger Bands®") // From catalog.bak.json
            .WithId("BB") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel) // From catalog.bak.json Category: "PriceChannel"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddParameter<double>("standardDeviations", "Standard Deviations", defaultValue: 2.0, minimum: 0.01, maximum: 10.0) // From catalog.bak.json
            .AddResult("Sma", "Centerline (SMA)", ResultType.Default, isDefault: true) // Corresponds to 'centerline' in bak.json
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false)
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false)
            .AddResult("PercentB", "%B", ResultType.Default, isDefault: false)
            .AddResult("ZScore", "Z-Score", ResultType.Default, isDefault: false)
            .AddResult("Width", "Width", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for BB.
    // No BufferListing for BB.
}
