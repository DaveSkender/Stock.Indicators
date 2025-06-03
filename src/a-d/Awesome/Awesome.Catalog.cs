// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Awesome
{
    // AWESOME Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Awesome Oscillator") // From catalog.bak.json
            .WithId("AWESOME") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 5, minimum: 1, maximum: 100) // From catalog.bak.json
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 34, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true) // From AwesomeResult model
            .AddResult("Normalized", "Normalized", ResultType.Default, isDefault: false) // From AwesomeResult model
            .Build();

    // No StreamListing for AWESOME.
    // No BufferListing for AWESOME.
}
