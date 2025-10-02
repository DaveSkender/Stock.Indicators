# Implementation Plan: StaticSeries Regression Baselines

## Overview

Establish a deterministic regression baseline for all StaticSeries indicator `Standard` tests. Baselines capture full output sequences (including warmup null periods) so future refactors cannot silently alter numerical results or warmup alignment.

## Objectives

- Persist one canonical dataset per indicator (Standard scenario) as JSON.
- Provide generator tool for explicit (manual) baseline regeneration.
- Provide regression test suite that re-computes results and compares against stored baselines with controlled numeric tolerances.
- Integrate regression tests into CI after unit tests.

## Scope

**In**: All `Standard()` methods in `tests/indicators/**/**/*StaticSeries.Tests.cs`.
**Deferred**: Non-Standard scenario variants (e.g., `StandardHighLow`, CandlePart variations), BufferList, StreamHub, and chainor/observer style tests.
**Out**: Performance benchmarking, streaming deltas, documentation snapshots.

## Architecture

### Components

1. Baseline Generator (console project `tests/tools/BaselineGenerator/`).
2. Baseline Data Store (`tests/regression/data/`).
3. Regression Test Project (`Tests.Regression`) or suite (dynamic test enumerator) reading baselines and re-validating outputs.
4. Shared Reflection Utility to map result record properties into serializable row objects.

### Flow

Quote Set (static test data) → Indicator Standard call → Result records → Normalized row list → JSON file (with metadata) → Checked into git → Regression test loads JSON → Recompute → Compare.

## File Format

Each file: `<Indicator>.Standard.json`

```json
{
  "meta": {
    "indicator": "Sma",
    "scenario": "Standard",
    "sourceCommit": "<git-sha>",
    "libraryVersion": "<assembly-version>",
    "generatedUtc": "2025-10-02T12:34:56Z",
    "schemaVersion": 1,
    "parameters": { "lookbackPeriods": 20 },
    "hash": "<sha256-of-data-array>",
    "notes": "Warmup nulls included"
  },
  "data": [
    { "t": "2017-01-03T00:00:00Z", "Sma": null },
    ...,
    { "t": "2018-12-31T00:00:00Z", "Sma": 251.8600 }
  ]
}
```

Rules:

- `t` = ISO-8601 UTC timestamp (trim trailing `.000`).
- Property order: `t`, then alphabetical of indicator fields.
- Omit fields that are null AND remain null entire dataset.
- Keep warmup null periods (critical for drift detection).
- Doubles serialized with round-trip (`"R"`/`G17`) format and invariant culture.
- No indentation (minified) to reduce size; diffs driven by stable ordering.
- `hash` = SHA256 of canonical JSON serialization of `data` array (no whitespace). Used to quickly detect data changes.

## Numeric Comparison Tolerances

- Absolute tolerance: 1e-10
- Relative tolerance: 1e-8 (applied when |expected| > 1e-6)
- A value passes if either tolerance satisfied.
- Strict mode: env var `REGRESSION_STRICT=1` disables tolerance (exact match required).

## Baseline Generation Mechanism

Rationale: Keep generation out of normal test runs to prevent accidental drift.
Implementation:

- Console app executed manually:
  `dotnet run --project tests/tools/BaselineGenerator --output tests/regression/data`
- Enumerates indicator specs (static registry list) with parameter snapshots.
- Executes each Standard scenario using shared quote dataset.
- Serializes results, computes hash, writes file (overwrites existing).
- Injects `sourceCommit` via executing `git rev-parse HEAD` (fallback `UNKNOWN`).
- Validates: ensured sorted timestamps, no NaN values, presence of at least one non-null computed value.
- Fails generation if invariants violated.

## Regression Comparison Strategy

Single dynamic test enumerates all baseline files:

1. Load JSON meta + data.
2. Recompute using registry mapping.
3. Materialize runtime rows (same shaping).
4. Validate:
   - File presence (all expected indicators); no unexpected extras unless flagged.
   - Row count equality.
   - Timestamp equality per index.
   - Property set equality.
   - Null parity.
   - Numeric tolerance check.
5. Collect mismatches; show first 5 diffs per file, fail once completed (aggregate assertion pattern).
6. Filtering: `REGRESSION_FILTER=IndicatorName` narrows to subset for debugging.

Failure Categories:

- STRUCTURE: missing/extra file or property
- LENGTH: row count differs
- ORDER: timestamp mismatch
- VALUE: numeric delta > tolerance

## Directory Structure

```text
tests/
  regression/
    data/
      Sma.Standard.json
      Rsi.Standard.json
      ...
    README.md (explains format & workflow)
  tools/
    BaselineGenerator/
      BaselineGenerator.csproj
      Program.cs
```

## Edge Cases & Exclusions

- Deterministic indicators only (all StaticSeries are deterministic with fixed input dataset).
- Variants like `StandardHighLow` initially excluded (can add later as separate scenario file names `<Indicator>.StandardHighLow.json`).
- ZigZag & Renko: still deterministic; tolerance handles small rounding; included.
- Ensure no `double.NaN`: generator throws if encountered.
- Warmup handling: preserved (null vs non-null shift is a regression signal).

## Performance & Size

Estimate: ~40k rows overall, ~4MB raw JSON, <1MB gzip (not needed initially). No performance concerns in test runtime; comparison loops are O(n) per indicator.
Optimizations: property omission for always-null fields, minified JSON.

## Versioning & Update Workflow

- Schema version = 1 at inception.
- Bump only for structural changes (e.g., rename keys, nested layout changes). Adding new meta keys is non-breaking.
- Update workflow:
  1. Change code intentionally.
  2. Run unit tests (regression expected to fail initially).
  3. Run generator to refresh baselines.
  4. Commit with Conventional Commit `test: regenerate regression baselines after <reason>`.
  5. PR description includes summary of changed indicators and diff sample.
- `libraryVersion` retrieved from assembly info at runtime.

## Automation Integration (CI)

- Add `Tests.Regression` to solution; run after primary unit tests.
- If regression fails, pipeline fails (hard gate) unless branch label / env override (e.g., `ALLOW_BASELINE_DRIFT=1`) explicitly set for exploratory work.
- Optional future optimization: only run regression on PR (not on every push) by workflow condition.

## Risks & Mitigation

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Baseline bloat | Larger repo | Minified JSON, omit always-null fields, future compression if >10MB |
| Accidental regeneration | False stability | Manual generator; not tied to default test run |
| Floating precision false alarms | Noise | Dual tolerance + strict mode toggle |
| Schema drift without version bump | Ambiguous diffs | Enforce schema version constant & review PR checklist |
| Indicator addition without baseline | Partial coverage | Failing regression test that enumerates missing expected file |
| Over-tolerance masking real issues | Hidden regressions | Modest tolerances chosen; strict mode for confirmation |

## Future Enhancements

- Add BufferList and StreamHub baselines once stable.
- Add multi-scenario coverage (HighLow, alternative candle parts).
- Add a diff reporter that prints top relative % changes.
- Optional binary-packed baseline for faster IO if scale increases.

## Success Criteria

- All current StaticSeries indicators have baseline file.
- Regression test passes on clean build.
- Intentional output changes require explicit baseline regen & PR justification.

## Implementation Tasks (Summary)

1. Create generator project & registry.
2. Implement serializer & hash util.
3. Generate initial baselines & commit.
4. Add regression test project.
5. Wire CI workflow stage (update pipeline config outside scope of this file).
6. Document workflow in `tests/regression/README.md`.

## Timeline

- Day 1: Implement generator + initial baselines.
- Day 2: Add regression test suite + CI integration.
- Day 3: Extend to any complex indicators needing extra parameter context.

---
Plan Version: 1.0
Created: 2025-10-02
Status: Approved Draft
