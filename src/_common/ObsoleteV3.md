# v3 migration guide

We've discontinued all bridge features for v1 backwards compatibility.
Correct all Warnings from this library before migrating from v2 to v3.
If you are still using v1, migrate to v2 first, to ease the transition to v3.

Where possible, we've added bridge features to ease transition from v2 to v3.
You will see supporting migration Warning in your compiler with additional instructions for [deprecated changes](#deprecation-changes).

In addition there are [breaking changes](#breaking-changes) that will require your attention.

## Deprecation changes

See your compiler `Warning` to identify these in your code.

- `Use()` method parameter `candlePart` is now required and no longer defaults to `CandlePart.Close`.
- `Use()` now returns a chainable `QuotePart` instead of a tuple.  These also replace the redundant `GetBaseQuote()` and `BaseQuote` items, respectively.

- `UlcerIndexResult` property `UI` was renamed to `UlcerIndex`

- **Deprecated 'GetX' tuple interfaces**.

- **Deprecated internal signals**: several indicators were originally built with integrated but optional
  moving averages, often by specifying an optional `smaPeriods` parameter.  With more moving average chaining options,
  these are obsolete, so we've removed them for simplification.  These were persisted to avoid breaking your code;
  however, you will see a compiler `Warnings` to help you identify areas to refactor.  Check for use in ADL, OBV, ROC, STDDEV, TRIX, and others.
  Future versions will not support the old API and will produce compiler `Errors`.

  ```csharp
  // To refactor, here's an example replacement for ADL:

  var results = quotes.GetAdl(10);
  var adlSma  = results.GetSma(5);

  // ref: old usage example

  var results = quotes
    .GetAdl(lookbackPeriods: 10, smaPeriods: 5);
  ```

## Breaking changes

Not all, but some of these will be shown as compiler `Errors` in your code.
Items marked with &#128681; require special attention since they will not produce compiler Errors or Warnings.

- all v1 backwards compatibility accommodations were removed.

- no longer supporting .NET Standard 2.0 for older .NET Framework compatibility.

### Common breaking changes

- `Quote` type (built-in) was changed to an _**immutable**_ `record` type; and its `IQuote` interface `Date` property was widely renamed to `Timestamp`, to avoid a conflict with a C# reserved name.  This will break your implementation if you were using `Quote` as an inherited base type.  To fix, define your custom `TQuote` type on the `IQuote` interface instead (example below).

- `IQuote` is now a reusable (chainable) type.  It auto-selects `Close` price as the _default_ consumed value.

- `TQuote` custom quote types now have to implement the `IReusable` interface to support chaining operations.  The best way to fix is to change your `TQuote` to implement a `IReusable.Value` pointer to your `IQuote.Close` price. See [the Guide](/guide) for more information.  Example:

  ```csharp
  public record MyCustomQuote (

      // `IQuote` properties
      DateTime Timestamp,
      decimal Open,
      decimal High,
      decimal Low,
      decimal MyClose,  // custom
      decimal Volume,

      // custom properties
      string? MyCustomProperty = default

  ) : IQuote // base: IReusable, IEquality<IQuote>
  {
      // custom mapped properties
      decimal IQuote.Close
        => MyClose;

      // `IReusable` compliance
      double IReusable.Value
        => (double)Close;
  }
  ```

- `IReusableResult` was renamed to `IReusable` since it is no longer limited to _result_ types.

- &#128681; `IReusableResult.Value` property was changed to non-nullable and returns `double.NaN` instead of `null` for incalculable periods.  The standard results (e.g. `EmaResult.Ema`) continue to return `null` for incalculable periods.  This was done to improve internal chaining and streaming performance.

- Indicator return types were changed from `sealed class` to _**immutable**_ `record` types to improve internal chaining and streaming performance.  This should not impact negatively; however, these can now be inherited as base classes.

### Less common breaking changes

- Return type for the `Use()` utility method was renamed from `UseResult` to `QuotePart` for clarity of its wider purpose.

- `GetBaseQuote()` indicator and related `BasicData` return types were removed since they are redundant to the `Use()` method and `QuotePart` return types, respectively.

- `SyncSeries()` utility function and related `SyncType` enum were removed.  This was primarily an internal utility, but was part of the public API to support user who wanted to build custom indicator development.  Internally, we've refactored indicators to auto-initialize and heal, so they no longer require re-sizing to support explicit warmup periods.

- `ToTupleCollection<TQuote>()` utility method was deprecated.  This was available to support custom indicator development, but is no longer needed.  We've discontinued using _tuples_ as an interface to chainable indicators.

- `Find()` and `FindIndex()` utility methods were removed.  These were redundant to the native C# `List.Find()` method and `List.FindIndex()` methods, respectively.
