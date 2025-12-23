# Bounded indicator threshold precision

## Purpose

- Eliminate floating-point boundary violations in indicators with mathematically guaranteed bounds.
- Achieve deterministic, accurate mathematical reproducibility between all indicator styles (Series, StreamHub, BufferList).
- Use algebraically stable algorithms that do not require "close enough" matching or forced rounding in indicator algorithms.
- Strengthen tests to assert strict bounds with no tolerance.

## Phase 0: Test infrastructure (COMPLETED)

### Boundary test data created

Created `tests/indicators/_testdata/TestData.Boundary.cs` with specialized quote generators:

| Generator                     | Purpose                                      | Indicator Target |
| ----------------------------- | -------------------------------------------- | ---------------- |
| `GetMonotonicallyIncreasing`  | All gains, no losses - pushes RSI toward 100 | RSI, StochRsi    |
| `GetMonotonicallyDecreasing`  | All losses, no gains - pushes RSI toward 0   | RSI, StochRsi    |
| `GetCloseEqualsHigh`          | Close = High every bar - Stoch should be 100 | Stoch, WilliamsR |
| `GetCloseEqualsLow`           | Close = Low every bar - Stoch should be 0    | Stoch, WilliamsR |
| `GetFlatCandles`              | O=H=L=C - tests zero-range handling          | Stoch, BOP       |
| `GetAlternating`              | Alternating up/down - CMO near 0             | CMO, TSI         |
| `GetTinyMovements`            | Very small increments - precision limits     | All              |

### Precision analysis tests created

Created `tests/indicators/_precision/PrecisionAnalysis.Tests.cs` with diagnostic tests that analyze bounds violations across all indicator styles.

## Phase 1: Analysis and categorization (COMPLETED)

### Current test status

Running `Results_AreAlwaysBounded` tests across all indicators:

| Status   | Count | Indicators                                                           |
| -------- | ----: | -------------------------------------------------------------------- |
| Passing  |    48 | All except StochRsi                                                  |
| Failing  |     3 | StochRsi (Series, BufferList, StreamHub) - identical error           |

### Key discovery: Precision issue is algorithm-level, not style-specific

The StochRsi failure shows **identical** precision errors across all three styles:

- **Value**: `100.00000000000001`
- **Deviation**: `1.4210854715202004E-14` above 100
- **Cause**: Floating-point representation of `(close - lowLow) / (highHigh - lowLow)` when `close == highHigh`

This proves the precision issue originates in the **core algorithm**, not from state accumulation or style-specific drift. Fixing the algorithm will fix all styles simultaneously.

### Boundary test results

Our synthetic boundary data (Phase 0) does **NOT** trigger precision violations in most indicators because:

1. The boundary conditions are "clean" - exact decimal values
2. The default S&P 500 `Quotes` data happens to produce specific floating-point values that expose the issue
3. Real-world data with irregular values is more likely to hit precision edge cases

This confirms: We need algebraically stable formulas, not just better test data.

### Root cause analysis

The Stoch oscillator formula at `src/s-z/Stoch/Stoch.StaticSeries.cs:166-170`:

```csharp
o[i] = highHigh - lowLow != 0
     ? 100d * (q.Close - lowLow) / (highHigh - lowLow)
     : 0;
```

When `q.Close == highHigh`:

- Mathematically: `(close - lowLow) / (highHigh - lowLow) = 1.0`
- Floating-point: May produce `1.0000000000000002` due to representation

This propagates through StochRsi because it applies Stoch to RSI values.

## Phase 2: Algebraically stable formula research (COMPLETED)

### Rejected approach: Clamping

```csharp
// This hides mathematical errors - NOT ACCEPTABLE
double result = Math.Clamp(100d * ratio, 0, 100);
```

### Recommended approach: Boundary detection with exact values

For Stoch-style formulas where close can exactly equal highHigh or lowLow:

```csharp
// Algebraically stable: detect boundary conditions first
double oscillator;
if (q.Close == highHigh)
{
    oscillator = 100d;  // Exact value, no calculation
}
else if (q.Close == lowLow)
{
    oscillator = 0d;    // Exact value, no calculation
}
else
{
    oscillator = 100d * (q.Close - lowLow) / (highHigh - lowLow);
}
```

**Why this works:**

- Eliminates floating-point division when result is exactly at boundary
- Produces mathematically correct values at boundaries
- Non-boundary calculations remain unchanged
- No precision loss for values strictly between bounds

### Alternative approach: Reformulated RSI arithmetic

For RSI-style formulas `100 - 100/(1+rs)`:

```csharp
// Original: Can exceed 100 when avgLoss approaches 0
double rsi = avgLoss > 0
    ? 100 - (100 / (1 + (avgGain / avgLoss)))
    : 100;

// Reformulated: Algebraically equivalent, inherently bounded
double rsi = avgLoss > 0
    ? 100d * avgGain / (avgGain + avgLoss)
    : 100;
```

**Why this works:**

- Single division instead of nested operations
- Numerator (avgGain) is always <= denominator (avgGain + avgLoss)
- Result cannot exceed 100 by construction
- Produces 0 when avgGain = 0, 100 when avgLoss = 0

### Formula transformation summary

| Indicator  | Current formula                                  | Proposed fix                                      |
| ---------- | ------------------------------------------------ | ------------------------------------------------- |
| Stoch      | `100 * (C-L) / (H-L)`                            | Boundary detection: if C==H return 100            |
| WilliamsR  | `Stoch - 100`                                    | Inherits Stoch fix                                |
| StochRsi   | Stoch(RSI values)                                | Inherits Stoch fix                                |
| Stc        | Stoch(MACD values)                               | Inherits Stoch fix                                |
| RSI        | `100 - 100/(1+rs)`                               | `100 * avgGain / (avgGain + avgLoss)`             |
| MFI        | `100 - 100/(1+mfRatio)`                          | `100 * posMF / (posMF + negMF)`                   |
| CMO        | `100 * (sH-sL) / (sH+sL)`                        | Boundary detection: if sH==0 return -100, etc.    |
| TSI        | `100 * smoothedPC / smoothedAbsPC`               | Boundary detection for edge cases                 |

## Phase 3: Implementation plan

### Priority order

1. **Stoch** (foundation) - fixes Stoch, StochRsi, Stc, WilliamsR
2. **RSI** (foundation) - may help StochRsi if RSI itself has edge cases
3. **CMO** - similar formula structure
4. **MFI** - RSI-style formula
5. **TSI** - complex smoothing, monitor for issues

### Implementation steps per indicator

For each indicator:

- [ ] Implement boundary detection or formula reformulation in Series (canonical)
- [ ] Apply identical fix to BufferList
- [ ] Apply identical fix to StreamHub
- [ ] Verify all three styles produce identical results
- [ ] Run `Results_AreAlwaysBounded` tests with default Quotes
- [ ] Add XML remarks documenting bounded range guarantee

### Specific tasks

#### Stoch family (fixes 4 indicators)

- [ ] Update `Stoch.StaticSeries.cs` line ~169 with boundary detection
- [ ] Update `Stoch.StreamHub.cs` with same logic
- [ ] Update `Stoch.BufferList.cs` with same logic
- [ ] Verify StochRsi, Stc, WilliamsR tests now pass

#### RSI family

- [ ] Research if RSI itself needs the reformulated formula
- [ ] If needed, update `Rsi.StaticSeries.cs` with `100 * avgGain / (avgGain + avgLoss)`
- [ ] Propagate to BufferList and StreamHub
- [ ] Verify ConnorsRsi if it inherits RSI values

## Guardrails

- **No clamping**: Boundary violations must fail tests, not be masked.
- **No epsilon tolerance**: Bound checks must use exact comparison.
- **No forced rounding**: Cannot use `ToPrecision()` to hide issues.
- **Series as canonical**: All styles must match Series results exactly.
- **Algorithm-level fixes**: Address root cause in formulas, not symptoms.

## Done criteria

- [ ] All `Results_AreAlwaysBounded` tests pass (currently 48/51 passing)
- [ ] No precision tolerance or clamping used in tests or source code
- [ ] Series/StreamHub/BufferList produce mathematically identical results
- [ ] Algebraically stable formulas documented in code comments
- [ ] XML remarks added for bounded results stating the range guarantee
- [ ] Code [completion checklist](../../.github/instructions/code-completion.instructions.md) completed with no failures

## Lessons learned

1. **Test data matters less than formula stability**: Synthetic boundary data doesn't reliably expose precision issues; real-world data with irregular values is more likely to trigger edge cases.

2. **Precision errors are algorithm-level**: When all three styles fail with identical errors, the fix should be in the core formula, not style-specific workarounds.

3. **Boundary detection is preferred over reformulation**: For Stoch-style indicators where we can detect exact boundary conditions, explicit checks are cleaner than algebraic reformulation.

4. **Foundation indicators cascade fixes**: Fixing Stoch once fixes StochRsi, Stc, and WilliamsR automatically.

5. **Epsilon is ~1e-14**: The observed precision error is consistent with IEEE 754 double precision limits (about 15-16 significant decimal digits).
