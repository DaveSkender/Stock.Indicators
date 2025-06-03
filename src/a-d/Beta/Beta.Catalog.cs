// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic; // For IEnumerable<Quote>
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Beta
{
    // BETA Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Beta") // From catalog.bak.json
            .WithId("BETA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic) // From catalog.bak.json Category: "PriceCharacteristic"
            .AddParameter<IEnumerable<Quote>>("sourceEval", "Evaluated Prices") // From catalog.bak.json
            .AddParameter<IEnumerable<Quote>>("sourceMrkt", "Market Prices") // From catalog.bak.json
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 50, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("type", "Beta Type", defaultValue: 0, minimum: 0, maximum: 3) // Maps to BetaType enum, int in builder. From catalog.bak.json
            .AddResult("Beta", "Beta", ResultType.Default, isDefault: true) // From BetaResult model
            .AddResult("BetaUp", "Beta Up", ResultType.Default, isDefault: false)
            .AddResult("BetaDown", "Beta Down", ResultType.Default, isDefault: false)
            .AddResult("Ratio", "Ratio", ResultType.Default, isDefault: false)
            .AddResult("Convexity", "Convexity", ResultType.Default, isDefault: false)
            .AddResult("ReturnsEval", "Returns Eval", ResultType.Default, isDefault: false)
            .AddResult("ReturnsMrkt", "Returns Mrkt", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for BETA.
    // No BufferListing for BETA.
}
