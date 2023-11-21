# v3 migration guide

## Breaking changes in v3

- `IReusableResult.Value` property was changed to non-nullable and returns `double.NaN` instead of `null`
  for incalculable periods.  The standard results (e.g. `EmaResult.Ema` will still return null).

- `UlcerIndexResult` property `UI` was renamed to `UlcerIndex`

- **Deprecated internal signals**: several indicators were originally built with integrated but optional
  moving averages.  With more moving average chaining options, these are obsolete, so we've removed them
  for simplification.  These were persisted to avoid breaking your code; however, you will see a compiler
  `Warnings` to help you identify areas to refactor.  Check for use in ADL, OBV, and others.
  Future versions will not support the old API and will produce compiler `Errors`.

```csharp
// To refactor, here's an example replacement for ADL:

// old usage


```