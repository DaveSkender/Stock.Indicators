# Tasks: streaming indicators framework

**Input**: Design documents from `/specs/001-develop-streaming-indicators/`
**Prerequisites**: plan.md (required) — data entities documented in plan.md §Data Model

## Requirements Quality Validation

Before implementing each indicator, review the corresponding simplified checklist for requirements validation:

- **BufferList style**: [checklists/buffer-list.md](checklists/buffer-list.md) — 15 essential validation items
- **StreamHub style**: [checklists/stream-hub.md](checklists/stream-hub.md) — 18 essential validation items

These simplified checklists ensure:

- Constitution compliance (mathematical precision, performance, validation, testing, documentation)
- Instruction file adherence (base classes, interfaces, test patterns, utilities)
- Essential quality gates (clarity, completeness, consistency, verifiability)

## Format: `[ID] Description`

- Include exact file paths in descriptions
- All tasks are independently parallelizable

## Path Conventions

Paths assume the single-project layout at the repository root:

- Source: `src/`
- Tests: `tests/`

## Instruction File Compliance Audit (NEW - CRITICAL)

The following tasks address gaps between existing implementations and current instruction file requirements:

### Core Compliance Audit

- [ ] **A001**: Audit existing BufferList implementations for instruction file compliance (`src/**/*.BufferList.cs` against `.github/instructions/indicator-buffer.instructions.md`)
  - [ ] Verify correct base class inheritance (`BufferList<TResult>`)
  - [ ] Check interface implementation (`IIncrementFromChain`/`IIncrementFromQuote`/`IIncrementFromPairs`)
  - [ ] Validate constructor patterns (params-only and params+quotes variants)
  - [ ] Confirm `BufferUtilities` usage instead of manual buffer management
  - [ ] Check member ordering per instruction file conventions

- [ ] **A002**: Audit existing StreamHub implementations for instruction file compliance (`src/**/*.StreamHub.cs` against `.github/instructions/indicator-stream.instructions.md`)
  - [ ] Verify correct provider base (`ChainProvider`/`QuoteProvider`/`PairsProvider`)
  - [ ] Check test interface implementation requirements
  - [ ] Validate provider history testing coverage (Insert/Remove scenarios)
  - [ ] Confirm performance benchmarking inclusion
  - [ ] Check member ordering per instruction file conventions

### Test Compliance Audit

- [ ] **A003**: Audit BufferList test classes for compliance (`tests/**/*.BufferList.Tests.cs`)
  - [ ] Verify inheritance from `BufferListTestBase` (not `TestBase`)
  - [ ] Check implementation of correct test interfaces
  - [ ] Validate coverage of 5 required test methods
  - [ ] Confirm Series parity validation patterns

- [ ] **A004**: Audit StreamHub test classes for compliance (`tests/**/*.StreamHub.Tests.cs`)
  - [ ] Verify inheritance from `StreamHubTestBase`
  - [ ] Check implementation of correct test interfaces per provider pattern
  - [ ] Validate provider history testing (Insert/Remove scenarios)
  - [ ] Confirm performance benchmarking inclusion

### Documentation Compliance

- [ ] **A005**: Update indicator documentation pages for instruction file compliance
  - [ ] Ensure streaming usage sections reference instruction files
  - [ ] Update examples to match current API patterns
  - [ ] Verify consistency with catalog entries

### Implementation Gap Analysis

- [ ] **A006**: Identify and prioritize instruction file compliance gaps
  - [ ] Create priority matrix based on constitution principle violations
  - [ ] Document specific remediation steps for high-priority gaps
  - [ ] Estimate effort for bringing existing implementations into compliance

**Note**: These audit tasks are essential for ensuring the existing 59 BufferList and 40 StreamHub implementations comply with the refined instruction file requirements developed since the original spec kit plans.

The following indicators have series-style implementations but lack BufferList implementations:

### BufferList A-D Group (16 indicators)

- [x] T001 Implement Alligator BufferList in `src/a-d/Alligator/Alligator.BufferList.cs` ✅ Complete
- [x] T002 Implement Aroon BufferList in `src/a-d/Aroon/Aroon.BufferList.cs` ✅ Complete
- [x] T003 Implement AtrStop BufferList in `src/a-d/AtrStop/AtrStop.BufferList.cs` ✅ Complete
- [x] T004 Implement Awesome BufferList in `src/a-d/Awesome/Awesome.BufferList.cs` ✅ Complete
- [x] T005 Implement Beta BufferList in `src/a-d/Beta/Beta.BufferList.cs` ✅ Complete
- [x] T006 Implement Bop BufferList in `src/a-d/Bop/Bop.BufferList.cs` ✅ Complete
- [x] T007 Implement ChaikinOsc BufferList in `src/a-d/ChaikinOsc/ChaikinOsc.BufferList.cs` ✅ Complete
- [x] T008 Implement Chandelier BufferList in `src/a-d/Chandelier/Chandelier.BufferList.cs` ✅ Complete
- [x] T009 Implement Chop BufferList in `src/a-d/Chop/Chop.BufferList.cs` ✅ Complete
- [x] T010 Implement Cmf BufferList in `src/a-d/Cmf/Cmf.BufferList.cs` ✅ Complete
- [ ] T011 Implement ConnorsRsi BufferList in `src/a-d/ConnorsRsi/ConnorsRsi.BufferList.cs`
- [x] T012 Implement Correlation BufferList in `src/a-d/Correlation/Correlation.BufferList.cs` ✅ Complete
- [x] T013 Implement Doji BufferList in `src/a-d/Doji/Doji.BufferList.cs` ✅ Complete
- [x] T014 Implement Donchian BufferList in `src/a-d/Donchian/Donchian.BufferList.cs` ✅ Complete
- [x] T015 Implement Dpo BufferList in `src/a-d/Dpo/Dpo.BufferList.cs` ✅ Complete
- [x] T016 Implement Dynamic BufferList in `src/a-d/Dynamic/Dynamic.BufferList.cs` ✅ Complete

### BufferList E-K Group (12 indicators)

- [x] T017 Implement ElderRay BufferList in `src/e-k/ElderRay/ElderRay.BufferList.cs` ✅ Complete
- [x] T018 Implement Fcb BufferList in `src/e-k/Fcb/Fcb.BufferList.cs` ✅ Complete
- [x] T019 Implement FisherTransform BufferList in `src/e-k/FisherTransform/FisherTransform.BufferList.cs` ✅ Complete
- [x] T020 Implement ForceIndex BufferList in `src/e-k/ForceIndex/ForceIndex.BufferList.cs` ✅ Complete
- [ ] T021 Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs` (Deferred - requires future data)
- [x] T022 Implement Gator BufferList in `src/e-k/Gator/Gator.BufferList.cs` ✅ Complete
- [x] T023 Implement HeikinAshi BufferList in `src/e-k/HeikinAshi/HeikinAshi.BufferList.cs` ✅ Complete
- [ ] T024 Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs` (Deferred - complex Hilbert Transform)
- [ ] T025 Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs` (Deferred - complex Hurst Exponent)
- [ ] T026 Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs` (Deferred - requires future offsets)
- [x] T027 Implement Keltner BufferList in `src/e-k/Keltner/Keltner.BufferList.cs` ✅ Complete
- [x] T028 Implement Kvo BufferList in `src/e-k/Kvo/Kvo.BufferList.cs` ✅ Complete

### BufferList M-R Group (13 indicators)

- [ ] T029 Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs`
- [x] T030 Implement Marubozu BufferList in `src/m-r/Marubozu/Marubozu.BufferList.cs` ✅ Complete
- [x] T031 Implement Mfi BufferList in `src/m-r/Mfi/Mfi.BufferList.cs` ✅ Complete
- [ ] T032 Implement ParabolicSar BufferList in `src/m-r/ParabolicSar/ParabolicSar.BufferList.cs`
- [ ] T033 Implement PivotPoints BufferList in `src/m-r/PivotPoints/PivotPoints.BufferList.cs`
- [ ] T034 Implement Pivots BufferList in `src/m-r/Pivots/Pivots.BufferList.cs`
- [ ] T035 Implement Pmo BufferList in `src/m-r/Pmo/Pmo.BufferList.cs`
- [ ] T036 Implement Prs BufferList in `src/m-r/Prs/Prs.BufferList.cs`
- [ ] T037 Implement Pvo BufferList in `src/m-r/Pvo/Pvo.BufferList.cs`
- [ ] T038 Implement Renko BufferList in `src/m-r/Renko/Renko.BufferList.cs`
- [ ] T039 Implement RenkoAtr BufferList in `src/m-r/RenkoAtr/RenkoAtr.BufferList.cs`
- [ ] T040 Implement RocWb BufferList in `src/m-r/RocWb/RocWb.BufferList.cs`
- [ ] T041 Implement RollingPivots BufferList in `src/m-r/RollingPivots/RollingPivots.BufferList.cs`

### BufferList S-Z Group (14 indicators)

- [ ] T042 Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs` (Deferred - Line property requires retroactive updates)
- [x] T043 Implement SmaAnalysis BufferList in `src/s-z/SmaAnalysis/SmaAnalysis.BufferList.cs` ✅ Complete
- [ ] T044 Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs`
- [ ] T045 Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs`
- [ ] T046 Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs`
- [x] T047 Implement StdDev BufferList in `src/s-z/StdDev/StdDev.BufferList.cs` ✅ Complete
- [ ] T048 Implement StdDevChannels BufferList in `src/s-z/StdDevChannels/StdDevChannels.BufferList.cs`
- [x] T049 Implement SuperTrend BufferList in `src/s-z/SuperTrend/SuperTrend.BufferList.cs` ✅ Complete
- [ ] T050 Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs`
- [x] T051 Implement UlcerIndex BufferList in `src/s-z/UlcerIndex/UlcerIndex.BufferList.cs` ✅ Complete
- [ ] T052 Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` (Deferred - requires retroactive updates)
- [x] T053 Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs` ✅ Complete
- [x] T054 Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs` ✅ Complete
- [ ] T055 Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs` (Deferred - requires retroactive updates and look-ahead)

## Missing StreamHub Implementations

The following indicators have series-style implementations but lack StreamHub implementations:

### StreamHub A-D Group (14 indicators)

- [x] T056 Implement Aroon StreamHub in `src/a-d/Aroon/Aroon.StreamHub.cs` ✅ Complete
- [x] T057 Implement Awesome StreamHub in `src/a-d/Awesome/Awesome.StreamHub.cs` ✅ Complete
- [ ] T058 Implement Beta StreamHub in `src/a-d/Beta/Beta.StreamHub.cs`
- [ ] T059 Implement Bop StreamHub in `src/a-d/Bop/Bop.StreamHub.cs`
- [ ] T060 Implement ChaikinOsc StreamHub in `src/a-d/ChaikinOsc/ChaikinOsc.StreamHub.cs`
- [ ] T061 Implement Chandelier StreamHub in `src/a-d/Chandelier/Chandelier.StreamHub.cs`
- [ ] T062 Implement Chop StreamHub in `src/a-d/Chop/Chop.StreamHub.cs`
- [ ] T063 Implement Cmf StreamHub in `src/a-d/Cmf/Cmf.StreamHub.cs`
- [ ] T064 Implement ConnorsRsi StreamHub in `src/a-d/ConnorsRsi/ConnorsRsi.StreamHub.cs`
- [ ] T065 Implement Correlation StreamHub in `src/a-d/Correlation/Correlation.StreamHub.cs` (WIP, needs revisit - dual-stream sync needs work, see Issue #1511)
- [x] T066 Implement Doji StreamHub in `src/a-d/Doji/Doji.StreamHub.cs` ✅ Complete
- [ ] T067 Implement Donchian StreamHub in `src/a-d/Donchian/Donchian.StreamHub.cs`
- [ ] T068 Implement Dpo StreamHub in `src/a-d/Dpo/Dpo.StreamHub.cs`
- [ ] T069 Implement Dynamic StreamHub in `src/a-d/Dynamic/Dynamic.StreamHub.cs`

### StreamHub E-K Group (12 indicators)

- [ ] T070 Implement ElderRay StreamHub in `src/e-k/ElderRay/ElderRay.StreamHub.cs`
- [ ] T071 Implement Fcb StreamHub in `src/e-k/Fcb/Fcb.StreamHub.cs`
- [ ] T072 Implement FisherTransform StreamHub in `src/e-k/FisherTransform/FisherTransform.StreamHub.cs`
- [ ] T073 Implement ForceIndex StreamHub in `src/e-k/ForceIndex/ForceIndex.StreamHub.cs`
- [ ] T074 Implement Fractal StreamHub in `src/e-k/Fractal/Fractal.StreamHub.cs`
- [ ] T075 Implement Gator StreamHub in `src/e-k/Gator/Gator.StreamHub.cs`
- [ ] T076 Implement HeikinAshi StreamHub in `src/e-k/HeikinAshi/HeikinAshi.StreamHub.cs`
- [ ] T077 Implement HtTrendline StreamHub in `src/e-k/HtTrendline/HtTrendline.StreamHub.cs`
- [ ] T078 Implement Hurst StreamHub in `src/e-k/Hurst/Hurst.StreamHub.cs`
- [ ] T079 Implement Ichimoku StreamHub in `src/e-k/Ichimoku/Ichimoku.StreamHub.cs`
- [ ] T080 Implement Keltner StreamHub in `src/e-k/Keltner/Keltner.StreamHub.cs`
- [ ] T081 Implement Kvo StreamHub in `src/e-k/Kvo/Kvo.StreamHub.cs`

### StreamHub M-R Group (12 indicators)

- [ ] T082 Implement MaEnvelopes StreamHub in `src/m-r/MaEnvelopes/MaEnvelopes.StreamHub.cs`
- [ ] T083 Implement Marubozu StreamHub in `src/m-r/Marubozu/Marubozu.StreamHub.cs`
- [ ] T084 Implement Mfi StreamHub in `src/m-r/Mfi/Mfi.StreamHub.cs`
- [ ] T085 Implement ParabolicSar StreamHub in `src/m-r/ParabolicSar/ParabolicSar.StreamHub.cs`
- [ ] T086 Implement PivotPoints StreamHub in `src/m-r/PivotPoints/PivotPoints.StreamHub.cs`
- [ ] T087 Implement Pivots StreamHub in `src/m-r/Pivots/Pivots.StreamHub.cs`
- [ ] T088 Implement Pmo StreamHub in `src/m-r/Pmo/Pmo.StreamHub.cs`
- [ ] T089 Implement Prs StreamHub in `src/m-r/Prs/Prs.StreamHub.cs`
- [ ] T090 Implement Pvo StreamHub in `src/m-r/Pvo/Pvo.StreamHub.cs`
- [ ] T091 Implement RenkoAtr StreamHub in `src/m-r/RenkoAtr/RenkoAtr.StreamHub.cs`
- [ ] T092 Implement RocWb StreamHub in `src/m-r/RocWb/RocWb.StreamHub.cs`
- [ ] T093 Implement RollingPivots StreamHub in `src/m-r/RollingPivots/RollingPivots.StreamHub.cs`

### StreamHub S-Z Group (14 indicators)

- [ ] T094 Implement Slope StreamHub in `src/s-z/Slope/Slope.StreamHub.cs`
- [ ] T095 Implement SmaAnalysis StreamHub in `src/s-z/SmaAnalysis/SmaAnalysis.StreamHub.cs`
- [ ] T096 Implement Smi StreamHub in `src/s-z/Smi/Smi.StreamHub.cs`
- [ ] T097 Implement StarcBands StreamHub in `src/s-z/StarcBands/StarcBands.StreamHub.cs`
- [ ] T098 Implement Stc StreamHub in `src/s-z/Stc/Stc.StreamHub.cs`
- [ ] T099 Implement StdDev StreamHub in `src/s-z/StdDev/StdDev.StreamHub.cs`
- [ ] T100 Implement StdDevChannels StreamHub in `src/s-z/StdDevChannels/StdDevChannels.StreamHub.cs`
- [ ] T101 Implement SuperTrend StreamHub in `src/s-z/SuperTrend/SuperTrend.StreamHub.cs`
- [ ] T102 Implement Tsi StreamHub in `src/s-z/Tsi/Tsi.StreamHub.cs`
- [ ] T103 Implement UlcerIndex StreamHub in `src/s-z/UlcerIndex/UlcerIndex.StreamHub.cs`
- [ ] T104 Implement VolatilityStop StreamHub in `src/s-z/VolatilityStop/VolatilityStop.StreamHub.cs`
- [ ] T105 Implement Vortex StreamHub in `src/s-z/Vortex/Vortex.StreamHub.cs`
- [ ] T106 Implement Vwap StreamHub in `src/s-z/Vwap/Vwap.StreamHub.cs`
- [ ] T107 Implement ZigZag StreamHub in `src/s-z/ZigZag/ZigZag.StreamHub.cs`

## Implementation Guidelines

Each task should follow these guidelines:

### BufferList Implementation Requirements

- Inherit from `BufferList<TResult>` base class
- Implement appropriate interface (`IIncrementFromChain`, `IIncrementFromQuote`, or `IIncrementFromPairs`)
- Follow patterns from `.github/instructions/indicator-buffer.instructions.md`
- Provide both standard constructor and constructor with values/quotes parameter (matching interface type)
- Use universal `BufferUtilities` extension methods for buffer management
- Include comprehensive unit tests matching patterns in existing tests
- Ensure mathematical correctness matches series implementation

### StreamHub Implementation Requirements

- Extend `ChainProvider<TIn, TResult>` or `QuoteProvider<TIn, TResult>`
- Follow patterns from `.github/instructions/indicator-stream.instructions.md`
- Implement efficient state management for real-time processing
- Include comprehensive unit tests matching patterns in existing tests
- Ensure mathematical correctness matches series implementation
- Optimize for low-latency, high-frequency scenarios

### Testing Requirements

- Create corresponding test files in `tests/indicators/{folder}/{indicator}/`
- Follow naming convention: `{Indicator}.BufferList.Tests.cs` or `{Indicator}.StreamHub.Tests.cs`
- Include all required test methods from test base classes
- Verify parity with series-style calculations
- Test edge cases, reset behavior, and state management

## Dependencies

- Each BufferList and StreamHub implementation is independent
- Tests should be created alongside implementations
- Follow existing patterns in the codebase for consistency
- Reference instruction files for authoritative guidance

## Notes

- All tasks can be executed in parallel as they touch different files
- Maintain strict adherence to existing patterns and conventions
- Run `dotnet test --no-restore` after each implementation
- Follow pre-commit checklist from `.github/instructions/source-code-completion.instructions.md`
- Update catalog entries and documentation as implementations are completed

## Supporting Tasks (Infrastructure & Documentation)

### Testing & Quality Assurance

**Note**: Test implementation is embedded within each T001-T107 task. Each implementation task includes:

- Unit tests in corresponding test file (e.g., `T001` includes `tests/indicators/a-d/Alligator/Alligator.BufferList.Tests.cs`)
- Parity tests validating equivalence with Series implementation
- Edge case tests (reset behavior, state management, error conditions)

### Documentation Updates

The following documentation tasks support the main implementation work:

- [x] **D001**: Update `docs/_indicators/Sma.md` with streaming usage section and examples (already completed)
- [x] **D002**: Update `docs/_indicators/Ema.md` with streaming usage section and examples (already completed)
- [ ] **D003**: Update `docs/_indicators/Rsi.md` with streaming usage section and examples
- [ ] **D004**: Update `docs/_indicators/Macd.md` with streaming usage section and examples
- [ ] **D005**: Update `docs/_indicators/BollingerBands.md` with streaming usage section and examples
- [ ] **D006**: Update `README.md` with streaming overview paragraph and quick-start example
- [ ] **D007**: Update `src/MigrationGuide.V3.md` with streaming capability summary and migration guidance
- [ ] **T108**: Update indicator documentation pages (`docs/_indicators/*.md`) for all streaming-enabled indicators with usage examples (NFR-005)
- [ ] **T109**: Expand `src/MigrationGuide.V3.md` with comprehensive streaming migration guidance including performance benefits and API patterns (NFR-006)

### Quality Gates

- [ ] **Q001**: Update public API approval test baselines for streaming additions (`tests/public-api/`)
- [ ] **Q002**: Run performance benchmarks comparing BufferList vs Series for representative indicators
- [ ] **Q003**: Run performance benchmarks comparing StreamHub vs Series for representative indicators
- [ ] **Q004**: Validate memory overhead stays within <10KB per instance target (NFR-002)

### Test Interface Compliance & Code Quality

- [ ] **T110**: Audit all existing StreamHub test classes for proper test interface implementation according to updated guidelines in `.github/instructions/indicator-stream.instructions.md`
- [ ] **T111**: Update StreamHub test classes that implement wrong interfaces (e.g., missing `ITestChainObserver` for chainable indicators)
// T112: Add ITestRollbackState interface and robust RollbackValidation test to all StreamHub test classes for indicators with RollbackState() implementations (ADX, MAMA, AtrStop, Beta, etc.).
// The RollbackValidation test must:
// - Use a mutable `List<Quote>` (not static array)
// - Add all quotes, verify results match series exactly (strict ordering)
// - Remove a single historical quote (not just the last)
// - Rebuild the expected series with the revised quote list (one missing)
// - Assert exact count and strict ordering for both before and after removal
// - Never re-add the removed quote; the revised series is the new ground truth
// - Reference Adx.StreamHub.Tests.cs for canonical structure
- [ ] **T112**: Add `ITestRollbackState` interface and robust RollbackValidation test to test classes for indicators with `RollbackState()` implementations (ADX, MAMA, AtrStop, Beta, etc.)
- [ ] **T113**: Verify all dual-stream indicators (Correlation, Beta, PRS) implement `ITestPairsObserver` interface correctly
- [ ] **T114**: Create validation script to check test interface compliance across all StreamHub tests

**Notes**:

- Documentation tasks (D001-D007) can proceed in parallel with implementation
- Quality gate tasks (Q001-Q004) should run after substantial implementation progress (e.g., post-Phase 1a completion)
- Test infrastructure already exists; no additional scaffolding tasks required

## Summary

- **Total BufferList tasks**: 55
- **Total StreamHub tasks**: 52
- **Total implementation tasks**: 107
- **Current implementations**: BufferList (50/85 = 59%), StreamHub (36/85 = 42%)
- **Missing BufferList implementations**: 35 indicators
- **Missing StreamHub implementations**: 49 indicators
- **Target coverage**: All 85 series indicators with both BufferList and StreamHub styles
- **Documentation tasks**: 7 (D001-D007)
- **Quality gate tasks**: 4 (Q001-Q004)
- **Test interface compliance tasks**: 5 (T110-T114)
- **Provider history testing tasks**: 6 (T115-T120)

## Provider History Testing (New Tasks)

StreamHub implementations should test removal and revision of provider history to ensure proper state management and recalculation. Analysis shows several tests lack Insert/Remove operations.

### Test Coverage Analysis

Current status of provider history testing in StreamHub tests:

**✅ Tests with proper Insert/Remove coverage:**

- AlligatorHub, AwesomeHub, DojiHub, EmaHub, DemaHub, HmaHub, RocHub, RenkoHub, SmaHub, SmmaHub, T3Hub, TemaHub, TrixHub, TrHub, UltimateHub, WmaHub (16 tests)

**❌ Tests missing Insert/Remove coverage:**

- AdxHub, AtrHub, CciHub, MacdHub, MamaHub, ObvHub, PrsHub, RsiHub, StochHub, StochRsiHub, VwmaHub, WilliamsRHub (12+ tests)

### Implementation Tasks

- [ ] T115 Add provider history (Insert/Remove) testing to QuoteObserver tests in AdxHub at `tests/indicators/a-d/Adx/Adx.StreamHub.Tests.cs` using the robust pattern: use `List<Quote>`, remove by index, verify strict ordering and count, never re-add removed quote, and reference Adx.StreamHub.Tests.cs as canonical example.
- [ ] T116 Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in AtrHub, CciHub, MacdHub, MamaHub, ObvHub, PrsHub at their respective test files, following the improved pattern.
- [ ] T117 Add provider history (Insert/Remove) testing to ChainObserver tests missing Insert/Remove operations in RsiHub, StochRsiHub at their respective test files, following the improved pattern.
- [ ] T118 Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in StochHub, VwmaHub, WilliamsRHub at their respective test files, following the improved pattern.
- [ ] T119 Add virtual ProviderHistoryTesting() method to StreamHubTestBase class in `tests/indicators/_base/StreamHubTestBase.cs` with standard Insert/Remove pattern, and ensure all derived tests override as needed for indicator-specific logic.
- [ ] T120 Update indicator-stream.instructions.md to require provider history testing and reference the new base class virtual method and the robust RollbackValidation pattern.

---
Last updated: October 12, 2025
