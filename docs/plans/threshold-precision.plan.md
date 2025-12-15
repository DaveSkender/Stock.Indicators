# Bounded indicator threshold precision

## Purpose

- Eliminate floating-point boundary violations in indicators with mathematically guaranteed bounds.
- Apply a single precision strategy (ToPrecision(14) on raw bounded formulas) across Series, StreamHub, and BufferList.
- Strengthen tests to assert strict bounds with no tolerance and increase numeric precision checks.

## Guardrails

- No clamping; boundary violations must fail tests, not be masked.
- Apply precision only to indicators that are provably bounded by formula.
- Add brief XML remarks noting the guaranteed bounded range; no API changes.
- Tests must assert exact bounds (no epsilon) and raise overall assertion precision from 4–6 decimals to the highest feasible for each indicator’s reference data.

## Shared tasks (all bounded indicators)

- Add `TestAsserts.AlwaysBounded(results, min, max)` helper in tests to enforce zero-tolerance bounds (skip bounds check for null/NaN warmup values; enforce for computed values).
- Raise assertion precision in existing indicator tests (use the maximum sensible decimals for each reference dataset; prefer 10–14 where stable).
- Add or extend boundary-focused tests using quote feeds that previously exposed rounding drift.
- Apply `ToPrecision(14)` to the raw bounded computation before downstream arithmetic in Series, StreamHub, and BufferList implementations.
- Add a one-line XML `<remarks>` to the bounded result type stating the range guarantee and precision handling.

## Indicator task list (guaranteed-bounded cohort)

- [ ] Stoch (0–100)
  - [ ] Confirm ToPrecision(14) present in Series, StreamHub, BufferList.
  - [ ] Add strict bound tests via shared helper; increase precision assertions.
  - [ ] Add XML remark on result.
- [ ] WilliamsR (-100–0)
  - [ ] Confirm ToPrecision(14) present in all styles.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.
- [ ] StochRsi (0–100)
  - [ ] Verify ToPrecision(14) in Series/BufferList; align with StreamHub.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.
- [ ] Stc (0–100)
  - [ ] Verify ToPrecision(14) present in all styles (StreamHub already uses it); align Series/BufferList if needed.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.
- [ ] Rsi (0–100)
  - [ ] Add ToPrecision(14) to bounded math in all styles.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.
- [ ] Cmo (-100–100)
  - [ ] Add ToPrecision(14) to bounded math in all styles.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.
- [ ] ConnorsRsi (0–100)
  - [ ] Add ToPrecision(14) to composite bounded outputs in all styles.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.
- Chop (0–100)
  - [ ] Confirm formula is strictly bounded; if yes, add ToPrecision(14) in all styles.
  - [ ] Add strict bound tests; raise precision assertions.
  - [ ] Add XML remark.

## Implementation order

- Complete shared helper and test precision uplift first (unblocks all indicators).
- Tackle indicators in order of usage and risk: Rsi → ConnorsRsi → Cmo → Chop → StochRsi → Stc (verify) → already-fixed Stoch/WilliamsR (test+XML harmonization).

## Done criteria

- [ ] Shared bound assertion helper in place and used by all bounded indicators.
- [ ] All bounded indicators apply ToPrecision(14) in every style and have strict bound tests with heightened precision.
- [ ] XML remarks added for bounded results.
- [ ] Series/Stream/Buffer parity confirmed via tests.
