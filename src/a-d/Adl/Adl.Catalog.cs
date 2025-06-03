// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Adl
{
    // ADL Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Accumulation Distribution Line (ADL)") // From catalog.bak.json
            .WithId("ADL") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            // ADL has no parameters in catalog.bak.json
            .AddResult("adl", "Accumulation Distribution Line (ADL)", ResultType.Default, isDefault: true) // From catalog.bak.json Results
            .Build();

    // ADL Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Accumulation Distribution Line (ADL) (Stream)") // Adjusted name
            .WithId("ADL")
            .WithStyle(Style.Stream)
            .WithCategory(Category.VolumeBased)
            // ADL has no parameters in catalog.bak.json
            .AddResult("adl", "Accumulation Distribution Line (ADL)", ResultType.Default, isDefault: true) // From catalog.bak.json Results
            .Build();

    // No BufferListing for ADL as it does not have a [BufferIndicator] attribute.
}
