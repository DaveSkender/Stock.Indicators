# Feature 003: Implement Missing RollbackState Overrides + Rolling Window Optimizations

**Quick Reference** | **Details**
------------------- | -----------
**Status** | Ready for Implementation
**Priority** | High (Constitutional Compliance + Performance)
**Effort** | ~12-16 hours
**Indicators Affected** | 7 total (2 RollbackState + 5 Performance)
**Test Scenarios** | 10+ rollback tests + performance benchmarks

---

## 📋 Quick Summary

This feature addresses two related improvements to StreamHub implementations:

### Part 1: Missing RollbackState Overrides (Constitutional Compliance)

After reviewing recently updated streaming indicator instructions, **2 StreamHub implementations** maintain stateful fields but use inline rebuild logic instead of the proper `RollbackState` override pattern:

1. **CmoHub** (`src/a-d/Cmo/Cmo.StreamHub.cs`) - Queue-based tick buffer
2. **CciHub** (`src/a-d/Cci/Cci.StreamHub.cs`) - CciList internal state

### Part 2: Rolling Window Performance Optimizations  

**5 StreamHub implementations** use O(n) linear scans that can be replaced with O(1) amortized `RollingWindowMax/Min` utilities:

1. **DonchianHub** - High/low tracking (simple)
2. **WilliamsRHub** - High/low tracking (simple)
3. **FisherTransformHub** - Price min/max (moderate)
4. **ChopHub** - True high/low (moderate)
5. **StochRsiHub** - RSI buffer max/min (moderate)

**Expected Performance Impact**: 10-50x speedup for large lookback periods

**Why this matters:**

- ❌ Inline rebuild logic contaminates hot path (Part 1)
- ❌ Violates separation of concerns (Part 1)
- ❌ Bypasses framework integration (Part 1)
- ❌ O(n) linear scans hurt performance unnecessarily (Part 2)
- ✅ RollingWindowMax/Min utilities provide O(1) amortized operations (Part 2)

---

## 📁 Spec Kit Artifacts

- **[spec.md](./spec.md)** - Feature specification with user stories and acceptance criteria
- **[plan.md](./plan.md)** - Technical implementation plan with architecture and patterns
- **[tasks.md](./tasks.md)** - 9 actionable tasks with detailed implementation steps

---

## 🎯 User Stories

### US1: CMO StreamHub (Priority 1)

Implement `RollbackState` override for CMO's tick buffer management.

- **Complexity**: Moderate (single Queue state)
- **Tasks**: T001-T004
- **Pattern**: Follow StochHub buffer management

### US2: CCI StreamHub (Priority 2)

Implement `RollbackState` override for CCI's CciList state management.

- **Complexity**: Moderate-Complex (internal buffers)
- **Tasks**: T005-T008
- **Pattern**: Follow AdxHub state restoration

### US3: Validation & Documentation (Priority 3)

Complete validation, documentation updates, and compliance verification.

- **Tasks**: T009
- **Deliverables**: Updated instructions, validated compliance

### US4: Rolling Window Refactorings (Priority 4)

Refactor 5 indicators to use RollingWindowMax/Min utilities for O(1) performance.

- **Tasks**: T010-T015
- **Complexity**: Simple (2) + Moderate (3)
- **Impact**: 10-50x performance improvement

---

## 🔧 Implementation Pattern

### ✅ CORRECT - RollbackState Override

```csharp
protected override void RollbackState(DateTime timestamp)
{
    // 1. Clear stateful fields
    _stateField.Clear();
    
    // 2. Find rollback position
    int index = ProviderCache.IndexGte(timestamp);
    if (index <= 0) return;
    
    // 3. Rebuild from cache
    int targetIndex = index - 1;
    int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);
    
    for (int p = startIdx; p <= targetIndex; p++)
    {
        // Rebuild state from ProviderCache[p]
    }
}
```

### ❌ WRONG - Inline Rebuild Detection

```csharp
protected override ToIndicator(item, indexHint)
{
    // ❌ DON'T DO THIS
    bool needsRebuild = DetectRebuildCondition();
    if (needsRebuild)
    {
        // Inline rebuild logic
    }
    
    // Normal processing
}
```

---

## 📊 Task Overview

**Total Tasks**: 15  
**Parallel Opportunities**: Analysis (T001, T005, T010-T011) and Test Writing (T002, T006)

### Phase 1: CMO (US1)

- **T001**: Analyze CMO state requirements [P]
- **T002**: Write CMO rollback tests
- **T003**: Implement CmoHub.RollbackState
- **T004**: Validate CMO regression tests

### Phase 2: CCI (US2)

- **T005**: Analyze CCI state requirements [P]
- **T006**: Write CCI rollback tests
- **T007**: Implement CciHub.RollbackState
- **T008**: Validate CCI regression tests

### Phase 3: Final (US3)

- **T009**: Final validation & documentation

### Phase 4: Rolling Window Refactorings (US4)

- **T010**: Refactor DonchianHub [P] (simple)
- **T011**: Refactor WilliamsRHub [P] (simple)
- **T012**: Refactor FisherTransformHub (moderate)
- **T013**: Refactor ChopHub (moderate)
- **T014**: Refactor StochRsiHub (moderate)
- **T015**: Performance benchmarking & validation

---

## ✅ Success Criteria

### Implementation

- [ ] Both StreamHubs override `RollbackState` correctly
- [ ] All inline rebuild logic removed from `ToIndicator`
- [ ] Code follows established patterns (AdxHub, StochHub)
- [ ] XML documentation complete

### Testing

- [ ] All 10 rollback tests pass (5 per indicator)
- [ ] All existing regression tests pass
- [ ] Series parity verified (bit-for-bit equality)
- [ ] Performance benchmarks within 5% variance

### Quality

- [ ] Zero analyzer warnings
- [ ] Code formatting passes
- [ ] Markdown linting passes
- [ ] Full test suite passes

### Documentation

- [ ] Reference implementations added to instructions
- [ ] Spec Kit artifacts complete
- [ ] Code comments accurate

---

## 🔗 Key References

### Instructions

- [indicator-stream.instructions.md](../../.github/instructions/indicator-stream.instructions.md) - RollbackState pattern guidance
- [source-code-completion.instructions.md](../../.github/instructions/source-code-completion.instructions.md) - Pre-commit checklist
- [spec-kit.instructions.md](../../.github/instructions/spec-kit.instructions.md) - Spec Kit workflow

### Reference Implementations

- `src/a-d/Chandelier/Chandelier.StreamHub.cs` - Simple rolling window
- `src/s-z/Stoch/Stoch.StreamHub.cs` - Complex buffer prefill
- `src/a-d/Adx/Adx.StreamHub.cs` - Wilder's smoothing state

### Test Patterns

- `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` - Canonical rollback tests (5 scenarios)

---

## 🚀 Getting Started

1. **Review the spec**: Read [spec.md](./spec.md) for user stories and acceptance criteria
2. **Understand the plan**: Read [plan.md](./plan.md) for technical architecture and patterns
3. **Execute tasks**: Follow [tasks.md](./tasks.md) step-by-step (T001 → T009)
4. **Start with CMO**: Complete US1 (T001-T004) before moving to CCI
5. **Validate thoroughly**: Run all tests and benchmarks before completion

---

## 📈 Impact

**Before**:

- 2 StreamHubs using anti-pattern (inline rebuild)
- 5 StreamHubs using O(n) linear scans for max/min
- Constitutional violation (improper state management)
- Performance bottlenecks for large lookback periods

**After**:

- 100% compliance with RollbackState pattern
- All 21 stateful StreamHubs follow framework integration
- O(1) amortized performance for rolling max/min operations
- 10-50x speedup for indicators with large lookback periods
- Clean hot path with optimal performance
- Proper separation of concerns

---

Last updated: 2025-10-25
