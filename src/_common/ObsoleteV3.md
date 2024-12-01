# v3 Migration Guide

This guide provides a detailed and clear migration path from v2 to v3 of the Stock Indicators library. It includes a summary of all technical changes to the public API, enumerates the exact syntax changes, and provides specific examples of deprecated and breaking changes. Follow the instructions below to update your code to be compatible with the new version.

## Summary of Technical Changes

- All static time-series API method prefixes were renamed from `GetX()` to `ToX()`.
- `Use()` method parameter `candlePart` is now required and no longer defaults to `CandlePart.Close`.
- `Use()` now returns a chainable `QuotePart` instead of a tuple.
- `UlcerIndexResult` property `UI` was renamed to `UlcerIndex`.
- Deprecated 'GetX' tuple interfaces.
- Deprecated internal signals for several indicators.
- `Quote` type was changed to an immutable `record` type.
- `IQuote` interface `Date` property was renamed to `Timestamp`.
- `IQuote` is now a reusable (chainable) type.
- `TQuote` custom quote types now have to implement the `IReusable` interface.
- `IReusableResult` was renamed to `IReusable`.
- `IReusableResult.Value` property was changed to non-nullable and returns `double.NaN` instead of `null`.
- Indicator return types were changed from `sealed class` to immutable `record` types.
- Return type for the `Use()` utility method was renamed from `UseResult` to `QuotePart`.
- `Numerixs` class was renamed to `Numerical`.
- `GetBaseQuote()` indicator and related `BasicData` return types were removed.
- `AtrStopResult` values were changed from `decimal` to `double`.
- `SyncSeries()` utility function and related `SyncType` enum were removed.
- `ToTupleCollection<TQuote>()` utility method was deprecated.
- `Find()` and `FindIndex()` utility methods were removed.

## Exact Syntax Changes

### General Changes

- All static time-series API method prefixes were renamed from `GetX()` to `ToX()`.
- `Use()` method parameter `candlePart` is now required and no longer defaults to `CandlePart.Close`.
- `Use()` now returns a chainable `QuotePart` instead of a tuple.

### Indicator-Specific Changes

- `UlcerIndexResult` property `UI` was renamed to `UlcerIndex`.
- Deprecated 'GetX' tuple interfaces.
- Deprecated internal signals for several indicators.

### Common Breaking Changes

- `Quote` type was changed to an immutable `record` type.
- `IQuote` interface `Date` property was renamed to `Timestamp`.
- `IQuote` is now a reusable (chainable) type.
- `TQuote` custom quote types now have to implement the `IReusable` interface.
- `IReusableResult` was renamed to `IReusable`.
- `IReusableResult.Value` property was changed to non-nullable and returns `double.NaN` instead of `null`.
- Indicator return types were changed from `sealed class` to immutable `record` types.
- Return type for the `Use()` utility method was renamed from `UseResult` to `QuotePart`.
- `Numerixs` class was renamed to `Numerical`.
- `GetBaseQuote()` indicator and related `BasicData` return types were removed.
- `AtrStopResult` values were changed from `decimal` to `double`.
- `SyncSeries()` utility function and related `SyncType` enum were removed.
- `ToTupleCollection<TQuote>()` utility method was deprecated.
- `Find()` and `FindIndex()` utility methods were removed.

## Migration Path

### General Migration Steps

1. Update all static time-series API method prefixes from `GetX()` to `ToX()`.
2. Update `Use()` method calls to include the `candlePart` parameter.
3. Update `Use()` method calls to handle the new `QuotePart` return type.
4. Update `UlcerIndexResult` property `UI` to `UlcerIndex`.
5. Refactor code to remove deprecated 'GetX' tuple interfaces.
6. Refactor code to remove deprecated internal signals for indicators.

### Specific Migration Steps

#### Updating `Quote` Type

1. Change `Quote` type to an immutable `record` type.
2. Rename `IQuote` interface `Date` property to `Timestamp`.
3. Implement the `IReusable` interface for custom `TQuote` types.

#### Updating `IReusableResult`

1. Rename `IReusableResult` to `IReusable`.
2. Update `IReusableResult.Value` property to handle `double.NaN` instead of `null`.

#### Updating Indicator Return Types

1. Change indicator return types from `sealed class` to immutable `record` types.

#### Updating Utility Methods

1. Rename return type for the `Use()` utility method from `UseResult` to `QuotePart`.
2. Rename `Numerixs` class to `Numerical`.
3. Remove `GetBaseQuote()` indicator and related `BasicData` return types.
4. Change `AtrStopResult` values from `decimal` to `double`.
5. Remove `SyncSeries()` utility function and related `SyncType` enum.
6. Deprecate `ToTupleCollection<TQuote>()` utility method.
7. Remove `Find()` and `FindIndex()` utility methods.

## Examples of Deprecated and Breaking Changes

### Deprecated Changes

```csharp
// Old usage
IEnumerable<AdlResult> results = quotes.GetAdl();
IEnumerable<UlcerIndexResult> results = quotes.GetUlcerIndex();
var adlSma = results.ToSma(5);

// New usage
IEnumerable<AdlResult> results = quotes.ToAdl();
IEnumerable<UlcerIndexResult> results = quotes.ToUlcerIndex();
var adlSma = results.ToSma(5);
```

### Breaking Changes

```csharp
// Old usage
public class MyQuote : Quote
{
    public DateTime Date { get; set; }
}

// New usage
public record MyQuote : IQuote
{
    public DateTime Timestamp { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public decimal Volume { get; init; }
}
```

## Clear Instructions for Updating Code

1. Follow the general migration steps to update your code to be compatible with the new version.
2. Refer to the specific migration steps for detailed instructions on updating each aspect of your code.
3. Use the examples of deprecated and breaking changes to guide your code updates.
4. Test your updated code to ensure it is compatible with the new version and functions as expected.

By following this migration guide, you can successfully update your code to be compatible with v3 of the Stock Indicators library. If you encounter any issues or have questions, refer to the project documentation or seek assistance from the community.
