# .NET source code development

This folder contains the Stock Indicators library source code.

## Implementation guidance

See .github/skills/ for detailed indicator development guidance:

- indicator-series - Series indicator implementation patterns
- indicator-buffer - BufferList indicator implementation patterns  
- indicator-stream - StreamHub indicator implementation patterns
- indicator-catalog - Catalog entry creation and registration
- code-completion - Quality gates for completing code work

## Technical constraints

**Performance & compatibility:**

- Targets: net10.0, net9.0, net8.0 (all must build and pass tests)
- Complexity: Single-pass O(n) unless mathematically impossible
- Warmup: Provide deterministic WarmupPeriod helper for each indicator
- Precision: Use double for speed; escalate to decimal only when rounding affects financial correctness
- Allocation: Result list + minimal working buffers only
- Thread safety: Stateless calculations are thread-safe; streaming hubs isolate instance state
- Backward compatibility: Renaming public members or altering defaults requires MAJOR version bump

**Error conventions:**

- Use ArgumentOutOfRangeException for invalid numeric parameter ranges
- Use ArgumentException for semantic misuse (e.g., insufficient history)
- Never swallow exceptions; wrap only to add context
- Messages MUST include parameter name and offending value when relevant

## NaN handling policy

This library uses non-nullable double types internally for performance, with intentional NaN propagation:

**Core principles:**

1. Natural propagation - NaN values propagate through calculations (any operation with NaN produces NaN)
2. Internal representation - Use double.NaN internally when a value cannot be calculated
3. External representation - Convert NaN to null (via .NaN2Null()) only at final result boundary
4. No rejection - Never reject NaN inputs; allow them to flow through the system
5. Performance first - Non-nullable double provides significant performance gains

**Implementation guidelines:**

- Division by zero - Guard variable denominators with ternary checks (e.g., `denom != 0 ? num / denom : double.NaN`)
- No epsilon comparisons - Use exact zero comparison (!= 0 or == 0), never epsilon values
- NaN propagation - Accept NaN inputs and allow natural propagation
- State initialization - Use double.NaN for uninitialized state instead of sentinel values

See _common/README.md for complete policy documentation.

## Series as the canonical reference

- Series indicators are the canonical source of truth for numerical correctness
- Series results are based on authoritative publications and manually verified calculations
- Stream and Buffer implementations must match Series results for same inputs once warmed up
- For discrepancies, fix Stream/Buffer unless there is verified issue with Series and reference data

---
Last updated: January 25, 2026
