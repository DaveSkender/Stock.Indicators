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

- `UlcerIndexResult` property `UI` was renamed to `UlcerIndex`

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

- all backwards compatible v1 accommodations removed.

- no longer supporting .NET Standard 2.0 for older .NET Framework compatibility.

- &#128681; `IReusableResult.Value` property was changed to non-nullable and returns `double.NaN` instead of `null`
  for incalculable periods.  The standard results (e.g. `EmaResult.Ema`) continue to return `null` for incalculable periods.

- Result classes were changes to `record` class types.  This will only impact rare cases where result classes are used for base inheritance.

- Quote class (built-in) was changed to `record` class type.

- `Date` property was widely renamed to `Timestamp` to avoid conflict with C# reserved name.

- `IQuote` customization now has to be `IEquatable<T>` type to support streaming operations.  See [the Guide](/guide) for more information.

- `BasicData` class was renamed to `BasicResult` for consistency with other return types.

- `SyncSeries()` utility function and related `SyncType` enum were removed.  These were primarily for internal
  utility, but were part of the public API since they were useful for custom indicator development.  Internally,
  we've refactored indicators to auto-initialize and heal, so they no longer require re-sizing to support explicit
  warmup periods.
