// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class ChaikinOsc
{
    // CHAIKIN-OSC Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chaikin Money Flow Oscillator") // From catalog.bak.json
            .WithId("CHAIKIN-OSC") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 3, minimum: 1, maximum: 100) // From catalog.bak.json
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 10, minimum: 1, maximum: 100) // From catalog.bak.json
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default, isDefault: false) // From ChaikinOscResult model
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default, isDefault: false) // From ChaikinOscResult model
            .AddResult("Adl", "ADL", ResultType.Default, isDefault: false) // From ChaikinOscResult model
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true) // From ChaikinOscResult model
            .Build();

    // No StreamListing for CHAIKIN-OSC.
    // No BufferListing for CHAIKIN-OSC.
}
