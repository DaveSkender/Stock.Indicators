---
title: Utilities and helpers
description: The Stock Indicators for .NET library includes utilities to help you use and transform historical prices quotes and indicator results, and to create custom indicators.
---

# Utilities and helper functions

The library provides utilities and helper functions to work with historical quotes, indicator results, and numerical analysis.

## Quote utilities

[Quote utilities](/utilities/quotes) help you prepare and transform historical price quotes before using them with indicators.

- **Use alternate price** - Analyze different price elements (HL2, HLC3, etc.)
- **Sort quotes** - Ensure chronological order
- **Resize quote history** - Aggregate into larger timeframes
- **Extended candle properties** - Add calculated candle properties
- **Validate quote history** - Advanced data quality checks

## Result utilities

[Result utilities](/utilities/results) help you work with indicator results after calculation.

- **Condense** - Remove non-essential results
- **Find by date** - Lookup results for specific dates
- **Remove warmup periods** - Prune initial convergence periods
- **Sort results** - Ensure chronological order

## Helper utilities

[Helper utilities](/utilities/helpers) provide numerical analysis tools for creating custom indicators.

- **Numerical methods** - Slope, standard deviation
- **NullMath** - Null-safe mathematical operations

## Indicator catalog

[Indicator catalog](/utilities/catalog) provides programmatic access to indicator metadata for building dynamic UIs or automation.

- **Browse or export** - Discover indicators and parameters at runtime
- **Execute by ID** - Run indicators dynamically without generics
- **Execute from JSON** - Configure and run indicators from JSON

---
Last updated: January 7, 2026
