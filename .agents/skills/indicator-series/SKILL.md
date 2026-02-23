---
name: indicator-series
description: Implement Series-style batch indicators with mathematical precision. Use for new StaticSeries implementations or optimization. Series results are the canonical reference—all other styles must match exactly. Focus on cross-cutting requirements and performance optimization decisions.
---

# Series indicator development

## File structure

All files live in `src/{category}/{Indicator}/`:

| File | Purpose |
| ---- | ------- |
| `{Indicator}.StaticSeries.cs` | Static partial class — `To{Indicator}()` + `To{Indicator}List()` entry points |
| `{Indicator}.StreamHub.cs` | Hub class (internal ctor) + `To{Indicator}Hub()` extension |
| `{Indicator}.BufferList.cs` | List class + `To{Indicator}List()` extension |
| `{Indicator}.Catalog.cs` | `CommonListing`, `SeriesListing`, `StreamListing`, `BufferListing` |
| `{Indicator}.Models.cs` | Result record(s) |
| `{Indicator}.Utilities.cs` | `Validate()` (internal), `Increment()` (public), `RemoveWarmupPeriods()` |
| `I{Indicator}.cs` | Parameter interface (parameter properties only; NOT result properties) |

Test files mirror in `tests/indicators/{category}/{Indicator}/`:

- `{Indicator}.StaticSeries.Tests.cs`
- `{Indicator}.BufferList.Tests.cs`
- `{Indicator}.StreamHub.Tests.cs`
- `{Indicator}.Regression.Tests.cs`

Category folders: `a-d`, `e-k`, `m-r`, `s-z` (alphabetical)

## Performance optimization

Array allocation pattern (use for predictable result counts; benchmark first):

```csharp
TResult[] results = new TResult[length];
// ... assign results[i] = new TResult(...);
return new List<TResult>(results);  // NOT results.ToList()
```

Some indicators (e.g., ADL) are faster with `List.Add()` — benchmark both.

## Required implementation

Beyond the `.StaticSeries.cs` file, ensure:

- [ ] **Catalog registration**: Create `src/**/{Indicator}.Catalog.cs` and register in `Catalog.Listings.cs`
- [ ] **Interface file**: Create `src/**/{Indicator}/I{Indicator}.cs` with parameter properties (NOT result properties)
- [ ] **Unit tests**: Create `tests/indicators/**/{Indicator}.StaticSeries.Tests.cs`
  - Inherit from `StaticSeriesTestBase`
  - Include `[TestCategory("Regression")]` for baseline validation
  - Verify against manually calculated reference values
- [ ] **Performance benchmark**: Add to `tools/performance/Perf.Series.cs`
- [ ] **Public documentation**: Update `docs/indicators/{Indicator}.md`
- [ ] **Regression tests**: Add to `tests/indicators/**/{Indicator}.Regression.Tests.cs`
- [ ] **Migration guide**: Update `docs/migration.md` for notable and breaking changes from v2

## Precision testing

- Store reference data in `{Indicator}.Data.cs` at maximum precision
- Regression: compare full dataset using Money10-Money12
- Spot checks: use Money4
- Document when precision must be lowered due to accumulated floating-point error

## Examples

- Simple: `src/s-z/Sma/Sma.StaticSeries.cs`
- Exponential smoothing: `src/e-k/Ema/Ema.StaticSeries.cs`
- Complex multi-stage: `src/a-d/Adx/Adx.StaticSeries.cs`
- Multi-value results: `src/a-d/Alligator/Alligator.StaticSeries.cs`

See [references/decision-tree.md](references/decision-tree.md) for result interface selection.

## Constraints

- Series is canonical truth — BufferList and StreamHub MUST match exactly
- Verify algorithms against authoritative reference publications only
- Never reject NaN inputs; guard against division by zero
- Fix formulas, not symptoms — see src/AGENTS.md
