# Feature Specification: Missing RollbackState Overrides

**Feature ID**: 003-implement-missing-rollback-overrides  
**Status**: Ready for Implementation  
**Created**: 2025-10-25  
**Last Updated**: 2025-10-25

---

## Problem Statement

After reviewing the recently updated streaming indicator instructions (`.github/instructions/indicator-stream.instructions.md`), a comprehensive codebase analysis revealed that **2 StreamHub implementations** maintain stateful fields but do NOT override the `RollbackState` method. This violates the constitutional requirement for proper state management in streaming indicators.

### Current State

- **100+ StreamHub implementations** exist in the codebase
- **19 StreamHubs** correctly implement `RollbackState` overrides
- **2 StreamHubs** have stateful fields but use inline rebuild logic instead of `RollbackState`:
  1. `CmoHub` - Queue-based tick buffer for momentum calculations
  2. `CciHub` - CciList internal state for commodity channel index

### Why This Matters

Per [indicator-stream.instructions.md](../../.github/instructions/indicator-stream.instructions.md#rollbackstate-override-pattern):

> **When to override `RollbackState`**: Any StreamHub that maintains stateful fields (beyond simple cache lookups) MUST override `RollbackState(DateTime timestamp)` to handle Insert/Remove operations and explicit Rebuild() calls.

**Problems with inline rebuild logic**:
- ❌ Violates separation of concerns (hot path contamination)
- ❌ Conditional logic in performance-critical `ToIndicator` method
- ❌ Framework integration bypassed (manual rebuild detection)
- ❌ Inconsistent with established patterns (AdxHub, StochHub, etc.)

---

## User Stories

### Priority 1: CMO StreamHub Rollback Implementation

**As a** developer using streaming indicators  
**I want** CmoHub to properly handle Insert/Remove operations via RollbackState  
**So that** late-arriving or corrected historical quotes are processed correctly without inline rebuild logic

**Acceptance Criteria**:
- [ ] `CmoHub` overrides `RollbackState(DateTime timestamp)` method
- [ ] Tick buffer (`_tickBuffer`) is properly cleared and rebuilt from ProviderCache
- [ ] Inline rebuild logic removed from `ToIndicator` method
- [ ] Comprehensive rollback tests pass (warmup, duplicate arrivals, Insert/Remove, Series parity)
- [ ] All existing regression tests continue to pass
- [ ] Series parity maintained (bit-for-bit equality with Series baseline)

**Independent Test Criteria**:
- Insert historical quote → CmoHub produces correct CMO values matching Series
- Remove quote from history → CmoHub maintains state consistency
- Duplicate arrivals → CmoHub handles gracefully without state corruption
- Explicit Rebuild() → Tick buffer properly restored

---

### Priority 2: CCI StreamHub Rollback Implementation

**As a** developer using streaming indicators  
**I want** CciHub to properly handle Insert/Remove operations via RollbackState  
**So that** late-arriving or corrected historical quotes are processed correctly without complex inline synchronization logic

**Acceptance Criteria**:
- [ ] `CciHub` overrides `RollbackState(DateTime timestamp)` method
- [ ] CciList internal state properly cleared and rebuilt from ProviderCache
- [ ] All inline state synchronization logic removed from `ToIndicator` method
- [ ] Comprehensive rollback tests pass (warmup, duplicate arrivals, Insert/Remove, Series parity)
- [ ] All existing regression tests continue to pass
- [ ] Series parity maintained (bit-for-bit equality with Series baseline)

**Independent Test Criteria**:
- Insert historical quote → CciHub produces correct CCI values matching Series
- Remove quote from history → CciList maintains internal state consistency
- Duplicate arrivals → CciHub handles gracefully without state corruption
- Explicit Rebuild() → CciList (SMA, mean deviation buffers) properly restored

---

### Priority 3: Documentation & Validation

**As a** contributor to the Stock Indicators library  
**I want** updated documentation and validated compliance  
**So that** future StreamHub implementations follow the correct RollbackState pattern

**Acceptance Criteria**:
- [ ] All tests pass (unit, regression, rollback validation)
- [ ] No performance regressions introduced
- [ ] Documentation updated with CmoHub and CciHub as reference implementations
- [ ] Code quality checks pass (formatting, linting, analyzers)
- [ ] Compliance with instruction file requirements verified

**Independent Test Criteria**:
- Full test suite passes without failures
- Performance benchmarks show no regressions
- Documentation accurately reflects new reference implementations
- Codacy analysis shows no new issues

---

### Priority 4: Rolling Window Utility Refactorings

**As a** developer optimizing streaming indicator performance  
**I want** StreamHubs to use efficient RollingWindowMax/Min utilities instead of O(n) linear scans  
**So that** indicators achieve O(1) amortized performance for rolling max/min tracking

**Acceptance Criteria**:
- [ ] 5 StreamHubs refactored to use RollingWindowMax/Min utilities:
  - DonchianHub (simple: high/low tracking)
  - WilliamsRHub (simple: high/low tracking)
  - FisherTransformHub (moderate: price min/max)
  - ChopHub (moderate: true high/low)
  - StochRsiHub (moderate: RSI buffer max/min)
- [ ] O(n) linear scans replaced with O(1) amortized window operations
- [ ] RollbackState overrides added/updated for proper state management
- [ ] All existing tests continue to pass
- [ ] Performance benchmarks demonstrate O(n) → O(1) improvement
- [ ] Series parity maintained (bit-for-bit equality with Series baseline)

**Independent Test Criteria**:
- Performance benchmarks show 10-50x speedup for large lookback periods
- Memory allocations remain stable or reduce
- Mathematical correctness verified (Series parity)
- Rollback operations work correctly with new window state

**Implementation Priority**:
1. **High Priority** (Simple + High Impact): Donchian, Williams %R
2. **Medium Priority** (Moderate complexity): Fisher Transform, Chop, Stoch RSI

**Excluded from Scope**:
- AroonHub (requires index tracking, not just values - utility enhancement needed first)

---

## Success Metrics

### Primary Metrics

1. **Correctness**: All rollback tests pass (5 scenarios per indicator)
2. **Series Parity**: Bit-for-bit equality with Series baseline after rollback operations
3. **Code Quality**: Zero analyzer warnings, all formatting/linting checks pass
4. **Test Coverage**: 100% coverage of rollback scenarios per instruction file requirements

### Performance Metrics

- No performance regressions in hot path (`ToIndicator`)
- Memory allocations remain stable or improve (removal of conditional logic)
- Benchmark results comparable to baseline (within 5% variance)
- **Rolling window refactorings**: 10-50x speedup for indicators with large lookback periods (O(n) → O(1) amortized)

---

## Out of Scope

- StreamHubs that are already stateless (100+ implementations that correctly use Cache/ProviderCache)
- StreamHubs that already implement RollbackState (19 existing implementations)
- New indicator development (this is fixing existing implementations)
- Changes to RollbackState framework itself (base class behavior)
- **AroonHub rolling window refactoring** (requires index tracking enhancement to utilities - separate feature)

---

## Dependencies

### Required Documents

- ✅ [indicator-stream.instructions.md](../../.github/instructions/indicator-stream.instructions.md) - RollbackState pattern guidance
- ✅ [source-code-completion.instructions.md](../../.github/instructions/source-code-completion.instructions.md) - Pre-commit checklist
- ✅ [spec-kit.instructions.md](../../.github/instructions/spec-kit.instructions.md) - Spec Kit workflow

### Reference Implementations

- `src/e-k/Ema/Ema.StreamHub.cs` - Simple cache-based (no RollbackState needed)
- `src/a-d/Chandelier/Chandelier.StreamHub.cs` - Rolling window RollbackState
- `src/s-z/Stoch/Stoch.StreamHub.cs` - Complex buffer prefill RollbackState
- `src/a-d/Adx/Adx.StreamHub.cs` - Wilder's smoothing state RollbackState

### Test Patterns

- `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` - Canonical rollback test pattern (5 required scenarios)

---

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Breaking existing behavior | High | Comprehensive regression tests, Series parity validation |
| Performance regression | Medium | Performance benchmarks before/after, hot path optimization focus |
| Edge case handling | Medium | Test warmup periods, empty state, boundary conditions |
| Documentation drift | Low | Update docs as part of tasks, validation checklist |

---

## Implementation Notes

### Key Principles

1. **Separation of Concerns**: `ToIndicator` handles incremental processing, `RollbackState` handles cache rebuilds
2. **Framework Integration**: Let StreamHub base class call `RollbackState` automatically
3. **Pattern Consistency**: Follow established patterns from AdxHub, StochHub, ChandelierHub
4. **Mathematical Precision**: Maintain bit-for-bit parity with Series baseline results

### Common Pitfalls to Avoid

- ❌ Leaving inline rebuild logic in `ToIndicator`
- ❌ Forgetting to clear stateful fields before rebuild
- ❌ Off-by-one errors in buffer prefill logic
- ❌ Not handling edge cases (empty cache, index < warmup)
- ❌ Skipping comprehensive rollback test scenarios

---

## Appendix: Analysis Summary

### Codebase Analysis Results

**Total StreamHub implementations**: 116 files

**Category Breakdown**:
- ✅ Already have RollbackState: 19 files
- ✅ Stateless (no RollbackState needed): 95 files
- ❌ **Need RollbackState**: 2 files (this spec)

**Files Requiring Implementation**:

1. **`src/a-d/Cmo/Cmo.StreamHub.cs`**
   - State: `Queue<(bool? isUp, double value)> _tickBuffer`
   - Current approach: Inline `canIncrement` check with else-rebuild
   - Complexity: Moderate (buffer management)

2. **`src/a-d/Cci/Cci.StreamHub.cs`**
   - State: `CciList _cciList` (custom BufferList-style collection)
   - Current approach: Complex inline state synchronization (4 conditional branches)
   - Complexity: Moderate to Complex (internal buffers for SMA and mean deviation)

---

Last updated: 2025-10-25
