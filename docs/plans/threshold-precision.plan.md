# Bounded indicator threshold precision

## Purpose

- Eliminate floating-point boundary violations in indicators with mathematically guaranteed bounds.
- Achieve deterministic, accurate mathematical reproducibility between all indicator styles (Series, StreamHub, BufferList).
- Use algebraically stable algorithms that do not require "close enough" matching or forced rounding in indicator algorithms.
- Strengthen tests to assert strict bounds with no tolerance.

## Problem analysis (December 2024)

### Current test status

Running `Results_AreAlwaysBounded` tests across all indicators reveals:

| Status   | Count | Notes                                                                                  |
| -------- | ----: | -------------------------------------------------------------------------------------- |
| Passing  |    48 | Tests pass with default `Quotes` data but may not expose precision issues              |
| Failing  |     3 | StochRsi (all 3 styles) - value 100.00000000000001 exceeds bound of 100                |

**Key observation**: Most tests currently pass because the default quote data does not produce values that push calculations to the exact boundaries where floating-point precision loss becomes visible. This does NOT mean those indicators are algebraically stable.

### Root cause analysis

The precision failures manifest at exact boundary conditions (0 or 100) where:

1. **Floating-point arithmetic accumulation**: Multiple divisions, multiplications, and EMA smoothing accumulate tiny precision errors (typically ~1e-14)
2. **Formula structure**: Formulas like `100 * (A / B)` where A is approximately B produce values infinitesimally above 100
3. **Composition effects**: Indicators built on other indicators (StochRsi = Stoch of RSI) compound precision issues
4. **State accumulation**: StreamHub and BufferList styles maintain running state that can drift differently than batch Series calculations

### Why current tests pass but are inadequate

The default `Quotes` test data (S&P 500 historical prices) rarely produces values at exact mathematical boundaries. To properly expose precision vulnerabilities, tests need input data that:

1. Creates scenarios where numerator equals denominator (e.g., close = high = low for Stoch)
2. Produces long periods of identical prices (triggers edge cases in gain/loss calculations)
3. Uses values that exercise floating-point representation limits

## Guardrails

- **No clamping**: Boundary violations must fail tests, not be masked by clamping to bounds.
- **No "close enough" comparisons**: We cannot use epsilon tolerance for bound checks.
- **No forced rounding in algorithms**: Cannot arbitrarily round to hide precision issues.
- **Series as canonical reference**: All styles must match Series results exactly.
- **Algorithm-level fixes only**: Precision must be addressed at the mathematical level, not test level.

## Indicator categorization

### Category A: Algebraically stable (low/no risk)

These indicators use formulas where precision errors cannot violate bounds due to integer-like or constrained arithmetic:

| Indicator | Bounds          | Stability rationale                                                                          |
| --------- | --------------- | -------------------------------------------------------------------------------------------- |
| Aroon     | 0-100, -100-100 | Integer numerator from counting; `100d * intValue / lookback` where intValue in [0,lookback] |

**Testing approach**: These should pass with any input data. If they fail, it indicates a different bug.

### Category B: Precision-vulnerable (high risk)

These indicators use ratio-based or exponential smoothing formulas where floating-point accumulation can exceed theoretical bounds:

| Indicator  | Bounds   | Risk source                                                                                                   |
| ---------- | -------- | ------------------------------------------------------------------------------------------------------------- |
| RSI        | 0-100    | `100 - 100/(1+rs)` with Wilder's smoothing; rs = avgGain/avgLoss where avgGain or avgLoss can approach zero   |
| CMO        | -100-100 | `100 * (sH - sL) / (sH + sL)` where sH approximately equals sL creates boundary conditions                    |
| TSI        | -100-100 | Double EMA smoothing of price changes; `100 * (smoothed_change / smoothed_abs_change)`                        |
| MFI        | 0-100    | Same formula structure as RSI: `100 - 100/(1 + ratio)`                                                        |
| Stoch      | 0-100    | `100 * (Close - lowLow) / (highHigh - lowLow)` when Close = highHigh                                          |
| WilliamsR  | -100-0   | Stoch - 100; inherits Stoch precision issues                                                                  |
| StochRsi   | 0-100    | **CURRENTLY FAILING** - Stoch of RSI; compounds both indicators' precision issues                             |
| Stc        | 0-100    | Stoch of MACD; composition magnifies precision drift                                                          |
| ConnorsRsi | 0-100    | `(RSI + StreakRSI + PercentRank) / 3`; RSI contribution can exceed bounds                                     |
| SMI        | -100-100 | `100 * (smEma2 / (0.5 * hlEma2))` with double EMA smoothing                                                   |
| Ultimate   | 0-100    | Weighted average of BP/TR ratios                                                                              |

**Testing approach**: These require carefully designed input data that produces boundary conditions.

### Category C: Statistically bounded (theoretical risk)

These indicators are bounded by statistical properties but use complex calculations:

| Indicator   | Bounds | Risk assessment                                                                           |
| ----------- | ------ | ----------------------------------------------------------------------------------------- |
| Correlation | -1-1   | Pearson correlation coefficient; bounded by definition but uses complex variance calcs    |
| RSquared    | 0-1    | Square of correlation; inherits precision from Correlation                                |
| Chop        | 0-100  | `100 * log10(ATR/range) / log10(period)`; log constrains output, division can err         |
| ADX         | 0-100  | DI calculations use division; DX uses `abs(PDI-MDI)/(PDI+MDI)`                            |
| PDI/MDI     | 0-100  | `100 * pdm/trs`; bounded by construction but uses smoothing                               |
| BOP         | -1-1   | `(Close-Open)/(High-Low)`; naturally bounded but division precision applies               |
| CMF         | -1-1   | Money flow volume ratio; uses ADL internally                                              |
| KAMA Er     | 0-1    | `change / volatility`; volatility always >= change                                        |
| Hurst       | 0-1    | R/S analysis with log regression; complex but naturally bounded                           |

**Testing approach**: Monitor for failures; may need precision fixes if specific data exposes issues.

## Proposed solution: Algebraically stable formulas

Rather than applying `ToPrecision(14)` as a band-aid, we should investigate algebraically equivalent formulas that prevent boundary violations by construction.

### Approach 1: Pre-normalized calculations

For formulas like `100 * (A/B)` where A <= B mathematically:

```csharp
// BEFORE: Can exceed 100 due to floating-point
double result = 100d * numerator / denominator;

// AFTER: Normalize ratio first, then scale
double ratio = Math.Min(1.0, numerator / denominator); // Safe if we KNOW ratio <= 1
double result = 100d * ratio;
```

**Issue**: This is effectively clamping. Not acceptable per guardrails.

### Approach 2: Reformulated arithmetic

For RSI-style formulas `100 - 100/(1+rs)`:

```csharp
// Original: 100 - 100/(1+rs) can produce 100.0000000001 when rs approaches infinity
// Algebraically equivalent: 100 * rs / (1 + rs)
double rsi = 100d * avgGain / (avgGain + avgLoss);
```

This formulation:

- Produces 0 when avgGain = 0 (mathematically correct)
- Produces 100 when avgLoss = 0 (via explicit check or limit)
- Cannot exceed 100 because avgGain <= (avgGain + avgLoss) always
- Uses single division instead of subtraction of ratios

**This is the preferred approach**: Reformulate the math to be inherently bounded.

### Approach 3: Exact representation at boundaries

Some indicators can use exact values at known boundary conditions:

```csharp
// For Stoch when Close equals highHigh exactly
if (q.Close == highHigh)
{
    oscillator = 100d; // Exact value, no calculation needed
}
else if (q.Close == lowLow)
{
    oscillator = 0d;
}
else
{
    oscillator = 100d * (q.Close - lowLow) / (highHigh - lowLow);
}
```

**Consideration**: This only works for discrete boundary detection and may not catch near-boundary floating-point comparisons.

## Revised task list

### Phase 0: Test infrastructure preparation

- [ ] Create unique test quote datasets designed to expose boundary conditions:
  - [ ] `BoundaryQuotes_StochExtreme`: Quotes where Close = High = Low for some periods
  - [ ] `BoundaryQuotes_RsiExtreme`: Long sequences of rising prices (RSI approaches 100) and falling prices (RSI approaches 0)
  - [ ] `BoundaryQuotes_CMOExtreme`: Alternating patterns that create sH approximately equal to sL conditions
- [ ] Update `Results_AreAlwaysBounded` tests to use indicator-specific boundary quotes
- [ ] Document which tests fail after using proper boundary test data

### Phase 1: Analyze and categorize failures

After running tests with proper boundary data:

- [ ] Document exact failing values and their deviation from bounds
- [ ] Identify common patterns (e.g., all RSI-formula indicators fail together)
- [ ] Determine if failures are consistent across Series/StreamHub/BufferList
- [ ] Prioritize fixes based on:
  1. Indicators currently failing (StochRsi)
  2. Foundation indicators used by others (RSI, Stoch)
  3. Popularity/usage frequency

### Phase 2: Implement algebraically stable formulas (per indicator)

**Do NOT implement source fixes in this analysis phase.**

For each precision-vulnerable indicator, research and document:

- [ ] RSI family (RSI, ConnorsRsi, StochRsi):
  - Alternative formula: `100 * avgGain / (avgGain + avgLoss)`
  - Impact analysis on StreamHub and BufferList state management
  - Verification that Series/Stream/Buffer produce identical results

- [ ] Stoch family (Stoch, WilliamsR, Stc):
  - Alternative formula or boundary detection approach
  - Impact on smoothing calculations (signal line)

- [ ] CMO/TSI (similar formula structure):
  - Alternative formulations using single division
  - State management implications

- [ ] Other bounded indicators as needed

### Phase 3: Implementation and verification

For each indicator:

- [ ] Implement algebraically stable formula in Series (canonical)
- [ ] Update BufferList to match Series exactly
- [ ] Update StreamHub to match Series exactly
- [ ] Verify all three styles produce identical results
- [ ] Add XML remarks documenting bounded range guarantee

## Current test file inventory

The following files contain `Results_AreAlwaysBounded` tests that need updating:

### Static Series (17 tests)

- Adx, Aroon, Bop, Chop, Cmf, Cmo, ConnorsRsi, Correlation, Hurst, Kama
- Mfi, Rsi, Stc, Stoch, StochRsi, Tsi, Ultimate, WilliamsR

### BufferList (15 tests)

- Adx, Aroon, Bop, Chop, Cmf, Cmo, ConnorsRsi, Correlation
- Mfi, Rsi, Stc, Stoch, StochRsi, Tsi, Ultimate, WilliamsR

### StreamHub (19 tests)

- Aroon, Bop, Chop, Cmf, Cmo, ConnorsRsi, Correlation, Mfi
- Rsi, Smi, Stc, Stoch, StochRsi, Tsi, UlcerIndex, Ultimate, WilliamsR

## Immediate next steps

1. **Create boundary test data** that exercises edge cases
2. **Run tests with new data** and document which indicators fail
3. **Research algebraically stable formulas** for failing indicators
4. **Implement fixes starting with foundation indicators** (RSI, Stoch) that others depend on
5. **Propagate fixes to composite indicators** (StochRsi, ConnorsRsi, Stc, WilliamsR)

## Done criteria

- [ ] All `Results_AreAlwaysBounded` tests pass with boundary-condition test data
- [ ] No precision tolerance or clamping used in tests or source code
- [ ] Series/StreamHub/BufferList produce mathematically identical results
- [ ] Algebraically stable formulas documented in code comments
- [ ] XML remarks added for bounded results stating the range guarantee
- [ ] Code [completion checklist](../../.github/instructions/code-completion.instructions.md) completed with no failures
