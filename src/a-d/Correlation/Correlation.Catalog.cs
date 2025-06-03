// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic; // For IEnumerable<Quote>
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Correlation
{
    // CORR Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Correlation") // From catalog.bak.json
            .WithId("CORR") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<IEnumerable<Quote>>("sourceA", "Source A") // From catalog.bak.json
            .AddParameter<IEnumerable<Quote>>("sourceB", "Source B") // From catalog.bak.json
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("VarianceA", "Variance A", ResultType.Default, isDefault: false) // From CorrResult model
            .AddResult("VarianceB", "Variance B", ResultType.Default, isDefault: false) // From CorrResult model
            .AddResult("Covariance", "Covariance", ResultType.Default, isDefault: false) // From CorrResult model
            .AddResult("Correlation", "Correlation", ResultType.Default, isDefault: true) // From CorrResult model
            .AddResult("RSquared", "R-Squared", ResultType.Default, isDefault: false) // From CorrResult model
            .Build();

    // No StreamListing for CORR.
    // No BufferListing for CORR.
}
