---
title: Indicator catalog
description: Programmatic access to indicator metadata for building dynamic UIs or automation.
---

# Indicator catalog (metadata)

Use the indicator catalog to discover indicators, build simple pickers, or export metadata for a UI.

- Discover indicators and parameters at runtime
- Build configuration UIs or export to JSON
- Optionally execute an indicator by ID (no compile-time generics required)

::: info Non-idiomatic
_The Catalog_ provides a programmatic way to interact with indicators and options; however, it is not the idiomatic .NET way to use this library. See the examples in [the Guide](/guide) for normal syntax examples.
:::

## Browse or export the catalog

```csharp
using Skender.Stock.Indicators;
using System.Text.Json;

// all listings (name, id, style, category, parameters, results)
IReadOnlyCollection<IndicatorListing> indicatorListings
    = CatalogRegistry.Get();

// optional: filter helpers
IndicatorListing? emaSeriesListing 
    = CatalogRegistry.Get("EMA", Style.Series);

IReadOnlyCollection<IndicatorListing> seriesListings
    = CatalogRegistry.Get(Style.Series);

// convert to JSON
string catalogJson = myListings.ToJson();
```

## Execute by ID (dynamic)

```csharp
// run an indicator using just ID + Style
IReadOnlyList<EmaResult> byId = quotes.ExecuteById<EmaResult>(
    id: "EMA",
    style: Style.Series,
    parameters: new() {
        { "lookbackPeriods", lookback }
    });
```

## Execute by config (JSON)

```csharp
string json = """
    {
        "id" : "EMA",
        "style" : "Series",
        "parameters" : { "lookbackPeriods" : 20 }
    }
    """;

IReadOnlyList<EmaResult> emaResultsFromJson
    = quotes.ExecuteFromJson<EmaResult>(json);
```

## Execute with strong typing

```csharp
// prefer typed results when you know the indicator
IndicatorListing indicatorListing = IndicatorRegistry
  .GetByIdAndStyle("EMA", Style.Series)
  ?? throw new InvalidOperationException("Indicator 'EMA' (Series) not found.");

// Call the quotes overload directly
IReadOnlyList<EmaResult> emaResultsWithDefaults = indicatorListing
  .Execute<EmaResult>(quotes);

// Or with specified parameters
IReadOnlyList<EmaResult> emaResultsWithParams = indicatorListing
  .FromSource(quotes)
  .WithParamValue("lookbackPeriods", 10)
  .Execute<EmaResult>();
```

---
Last updated: January 7, 2026
