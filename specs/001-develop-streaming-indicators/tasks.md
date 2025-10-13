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

- [x] **A001**: Audit existing BufferList implementations for instruction file compliance (`src/**/*.BufferList.cs` against `.github/instructions/indicator-buffer.instructions.md`) ✅ COMPLETE
  - [x] Verify correct base class inheritance (`BufferList<TResult>`) ✅
  - [x] Check interface implementation (`IIncrementFromChain`/`IIncrementFromQuote`/`IIncrementFromPairs`) ✅
  - [x] Validate constructor patterns (params-only and params+quotes variants) ✅
  - [x] Confirm `BufferUtilities` usage instead of manual buffer management ✅
  - [x] Check member ordering per instruction file conventions ✅

- [x] **A002**: Audit existing StreamHub implementations for instruction file compliance (`src/**/*.StreamHub.cs` against `.github/instructions/indicator-stream.instructions.md`) ✅ COMPLETE
  - [x] Verify correct provider base (`ChainProvider`/`QuoteProvider`/`PairsProvider`) ✅
  - [x] Check test interface implementation requirements ✅
  - [x] Validate provider history testing coverage (Insert/Remove scenarios) ✅
  - [x] Confirm performance benchmarking inclusion ✅
  - [x] Check member ordering per instruction file conventions ✅

### Test Compliance Audit

- [x] **A003**: Audit BufferList test classes for compliance (`tests/**/*.BufferList.Tests.cs`) ✅ COMPLETE
  - [x] Verify inheritance from `BufferListTestBase` (not `TestBase`) ✅
  - [x] Check implementation of correct test interfaces ✅
  - [x] Validate coverage of 5 required test methods ✅
  - [x] Confirm Series parity validation patterns ✅

- [x] **A004**: Audit StreamHub test classes for compliance (`tests/**/*.StreamHub.Tests.cs`) ✅ COMPLETE
  - [x] Verify inheritance from `StreamHubTestBase` ✅
  - [x] Check implementation of correct test interfaces per provider pattern ✅
  - [x] Validate provider history testing (Insert/Remove scenarios) ✅
  - [x] Confirm performance benchmarking inclusion ✅

### Documentation Compliance

- [x] **A005**: Update indicator documentation pages for instruction file compliance ✅ COMPLETE
  - [x] Ensure streaming usage sections reference instruction files ✅
  - [x] Update examples to match current API patterns ✅
  - [x] Verify consistency with catalog entries ✅

### Implementation Gap Analysis

- [x] **A006**: Identify and prioritize instruction file compliance gaps ✅ COMPLETE
  - [x] Create priority matrix based on constitution principle violations ✅
  - [x] Document specific remediation steps for high-priority gaps ✅
  - [x] Estimate effort for bringing existing implementations into compliance ✅

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
- [ ] T021 Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs` ⚠️ **DEFERRED**: Requires future data access pattern incompatible with streaming constraints (looks ahead 2 periods). Marked for v2 research with potential reorder buffer solution.
- [ ] T024 Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs` ⚠️ **DEFERRED**: Complex Hilbert Transform implementation requires research into streaming compatibility and state management complexity. Targeted for v2 after core framework stabilization.
- [ ] T025 Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs` ⚠️ **DEFERRED**: Hurst Exponent calculation requires full dataset access patterns incompatible with incremental streaming. Research needed for windowed approximation approach in v2.
- [ ] T026 Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs` ⚠️ **DEFERRED**: Requires future offset calculations (Senkou Span B projects 26 periods ahead) incompatible with streaming constraints. Marked for v2 research with offset buffer solution.
- [ ] T029 Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs`
- [ ] T032 Implement ParabolicSar BufferList in `src/m-r/ParabolicSar/ParabolicSar.BufferList.cs`
- [ ] T033 Implement PivotPoints BufferList in `src/m-r/PivotPoints/PivotPoints.BufferList.cs`
- [ ] T034 Implement Pivots BufferList in `src/m-r/Pivots/Pivots.BufferList.cs`
- [ ] T035 Implement Pmo BufferList in `src/m-r/Pmo/Pmo.BufferList.cs`
- [ ] T036 Implement Prs BufferList in `src/m-r/Prs/Prs.BufferList.cs`
- [ ] T037 Implement Pvo BufferList in `src/m-r/Pvo/Pvo.BufferList.cs`
- [x] T125 Implement QuotePart BufferList in `src/_common/QuotePart/QuotePart.BufferList.cs` ✅ Complete
- [ ] T038 Implement Renko BufferList in `src/m-r/Renko/Renko.BufferList.cs`
- [ ] T039 Implement RenkoAtr BufferList in `src/m-r/RenkoAtr/RenkoAtr.BufferList.cs`
- [ ] T040 Implement RocWb BufferList in `src/m-r/RocWb/RocWb.BufferList.cs`
- [ ] T041 Implement RollingPivots BufferList in `src/m-r/RollingPivots/RollingPivots.BufferList.cs`
- [ ] T042 Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs` ⚠️ **DEFERRED**: Line property requires retroactive updates to historical results when new data changes slope calculation, incompatible with append-only streaming constraints. Research needed for windowed approximation in v2.
- [ ] T044 Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs`
- [ ] T045 Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs`
- [ ] T046 Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs`
- [ ] T048 Implement StdDevChannels BufferList in `src/s-z/StdDevChannels/StdDevChannels.BufferList.cs`
- [ ] T050 Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs`
- [x] T052 Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` ✅ Complete
- [x] T053 Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs` ✅ Complete
- [x] T054 Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs` ✅ Complete
- [ ] T055 Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs`
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
- [ ] T021 Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs` ⚠️ **DEFERRED**: Requires future data access pattern incompatible with streaming constraints (looks ahead 2 periods). Marked for v2 research with potential reorder buffer solution.
- [x] T022 Implement Gator BufferList in `src/e-k/Gator/Gator.BufferList.cs` ✅ Complete
- [x] T023 Implement HeikinAshi BufferList in `src/e-k/HeikinAshi/HeikinAshi.BufferList.cs` ✅ Complete
- [ ] T024 Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs` ⚠️ **DEFERRED**: Complex Hilbert Transform implementation requires research into streaming compatibility and state management complexity. Targeted for v2 after core framework stabilization.
- [ ] T025 Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs` ⚠️ **DEFERRED**: Hurst Exponent calculation requires full dataset access patterns incompatible with incremental streaming. Research needed for windowed approximation approach in v2.
- [ ] T026 Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs` ⚠️ **DEFERRED**: Requires future offset calculations (Senkou Span B projects 26 periods ahead) incompatible with streaming constraints. Marked for v2 research with offset buffer solution.
- [x] T027 Implement Keltner BufferList in `src/e-k/Keltner/Keltner.BufferList.cs` ✅ Complete
- [x] T028 Implement Kvo BufferList in `src/e-k/Kvo/Kvo.BufferList.cs` ✅ Complete

### BufferList M-R Group (13 indicators)

- [ ] T029 Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs`
- [x] T030 Implement Marubozu BufferList in `src/m-r/Marubozu/Marubozu.BufferList.cs` ✅ Complete
- [x] T031 Implement Mfi BufferList in `src/m-r/Mfi/Mfi.BufferList.cs` ✅ Complete
- [ ] T032 Implement ParabolicSar BufferList in `src/m-r/ParabolicSar/ParabolicSar.BufferList.cs`
- [ ] T033 Implement PivotPoints BufferList in `src/m-r/PivotPoints/PivotPoints.BufferList.cs`
- [ ] T034 Implement Pivots BufferList in `src/m-r/Pivots/Pivots.BufferList.cs`
- [x] T035 Implement Pmo BufferList in `src/m-r/Pmo/Pmo.BufferList.cs` ✅ Complete
- [x] T036 Implement Prs BufferList in `src/m-r/Prs/Prs.BufferList.cs` ✅ Complete
- [x] T037 Implement Pvo BufferList in `src/m-r/Pvo/Pvo.BufferList.cs` ✅ Complete
- [ ] T038 Implement Renko BufferList in `src/m-r/Renko/Renko.BufferList.cs`
- [ ] T039 Implement RenkoAtr BufferList in `src/m-r/RenkoAtr/RenkoAtr.BufferList.cs`
- [ ] T040 Implement RocWb BufferList in `src/m-r/RocWb/RocWb.BufferList.cs`
- [ ] T041 Implement RollingPivots BufferList in `src/m-r/RollingPivots/RollingPivots.BufferList.cs`

### BufferList S-Z Group (14 indicators)

- [ ] T042 Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs` ⚠️ **DEFERRED**: Line property requires retroactive updates to historical results when new data changes slope calculation, incompatible with append-only streaming constraints. Research needed for windowed approximation in v2.
- [x] T043 Implement SmaAnalysis BufferList in `src/s-z/SmaAnalysis/SmaAnalysis.BufferList.cs` ✅ Complete
- [x] T044 Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs` ✅ Complete
- [x] T045 Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs` ✅ Complete
- [ ] T046 Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs` (Complex - requires MACD→Stochastic pipeline, deferred)
- [x] T047 Implement StdDev BufferList in `src/s-z/StdDev/StdDev.BufferList.cs` ✅ Complete
- [ ] T048 Implement StdDevChannels BufferList in `src/s-z/StdDevChannels/StdDevChannels.BufferList.cs` (Deferred - depends on Slope retroactive updates)
- [x] T049 Implement SuperTrend BufferList in `src/s-z/SuperTrend/SuperTrend.BufferList.cs` ✅ Complete
- [ ] T050 Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs`
- [x] T051 Implement UlcerIndex BufferList in `src/s-z/UlcerIndex/UlcerIndex.BufferList.cs` ✅ Complete
- [x] T052 Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` ✅ Complete (Previously deferred - now implemented with retroactive update capability using UpdateInternal)
- [x] T053 Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs` ✅ Complete
- [x] T054 Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs` ✅ Complete
- [ ] T055 Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs` (Deferred - requires retroactive updates and look-ahead)

## Missing StreamHub Implementations

**Target**: 44 missing StreamHub implementations for 1:1:1 parity

### StreamHub A-D Group (8 missing)

- [ ] T061 Implement Chandelier StreamHub in `src/a-d/Chandelier/Chandelier.StreamHub.cs`
- [ ] T062 Implement Chop StreamHub in `src/a-d/Chop/Chop.StreamHub.cs`
- [ ] T063 Implement Cmf StreamHub in `src/a-d/Cmf/Cmf.StreamHub.cs`
- [ ] T064 Implement ConnorsRsi StreamHub in `src/a-d/ConnorsRsi/ConnorsRsi.StreamHub.cs`
- [ ] T067 Implement Donchian StreamHub in `src/a-d/Donchian/Donchian.StreamHub.cs`
- [ ] T068 Implement Dpo StreamHub in `src/a-d/Dpo/Dpo.StreamHub.cs`
- [ ] T069 Implement Dynamic StreamHub in `src/a-d/Dynamic/Dynamic.StreamHub.cs`
- [ ] T065 Implement Correlation StreamHub in `src/a-d/Correlation/Correlation.StreamHub.cs` (WIP, needs revisit - dual-stream sync needs work, see Issue #1511)

### StreamHub E-K Group (12 missing)

- [ ] T070 Implement ElderRay StreamHub in `src/e-k/ElderRay/ElderRay.StreamHub.cs`
- [ ] T071 Implement Fcb StreamHub in `src/e-k/Fcb/Fcb.StreamHub.cs`
- [ ] T072 Implement FisherTransform StreamHub in `src/e-k/FisherTransform/FisherTransform.StreamHub.cs`
- [ ] T073 Implement ForceIndex StreamHub in `src/e-k/ForceIndex/ForceIndex.StreamHub.cs`
- [ ] T074 Implement Fractal StreamHub in `src/e-k/Fractal/Fractal.StreamHub.cs` ⚠️ **DEFERRED**: Future data access incompatible with streaming
- [ ] T075 Implement Gator StreamHub in `src/e-k/Gator/Gator.StreamHub.cs`
- [ ] T076 Implement HeikinAshi StreamHub in `src/e-k/HeikinAshi/HeikinAshi.StreamHub.cs`
- [ ] T077 Implement HtTrendline StreamHub in `src/e-k/HtTrendline/HtTrendline.StreamHub.cs` ⚠️ **DEFERRED**: Complex Hilbert Transform streaming compatibility research needed
- [ ] T078 Implement Hurst StreamHub in `src/e-k/Hurst/Hurst.StreamHub.cs` ⚠️ **DEFERRED**: Full dataset access patterns incompatible with streaming
- [ ] T079 Implement Ichimoku StreamHub in `src/e-k/Ichimoku/Ichimoku.StreamHub.cs` ⚠️ **DEFERRED**: Future offset calculations incompatible with streaming
- [ ] T080 Implement Keltner StreamHub in `src/e-k/Keltner/Keltner.StreamHub.cs`
- [ ] T081 Implement Kvo StreamHub in `src/e-k/Kvo/Kvo.StreamHub.cs`

### StreamHub M-R Group (12 missing)

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

### StreamHub S-Z Group (12 missing)

- [ ] T094 Implement Slope StreamHub in `src/s-z/Slope/Slope.StreamHub.cs` ⚠️ **DEFERRED**: Retroactive calculation incompatible with streaming
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
- [ ] T126 Implement QuotePart StreamHub in `src/_common/QuotePart/QuotePart.StreamHub.cs`

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
- [ ] **Q002**: Run performance benchmarks comparing BufferList vs Series for representative indicators using BenchmarkDotNet with latency validation (<5ms mean, <10ms p95) on standardized hardware (4-core 3GHz CPU, 16GB RAM, .NET 9.0 release mode)
- [ ] **Q003**: Run performance benchmarks comparing StreamHub vs Series for representative indicators using BenchmarkDotNet with latency validation (<5ms mean, <10ms p95) on standardized hardware (4-core 3GHz CPU, 16GB RAM, .NET 9.0 release mode)
- [ ] **Q004**: Validate memory overhead stays within <10KB per instance target (NFR-002) using dotMemory profiler or equivalent with specific measurement methodology on representative indicators (SMA, EMA, RSI with 200-period lookback). Measurement MUST include: baseline memory before indicator creation, memory after initialization with warmup data, memory after 1000 incremental updates, and final memory footprint. Document measurement procedure and baseline results for CI regression detection.
- [ ] **Q005**: Create automated performance regression detection for streaming indicators with baseline thresholds: mean latency <5ms, p95 latency <10ms, memory overhead <10KB per instance, and automatic CI failure on regression >10% degradation
- [ ] **Q006**: Establish memory baseline measurements for all streaming indicator types using standardized test procedure: measure BufferList vs StreamHub memory efficiency, document memory scaling characteristics vs lookback period (50, 100, 200 periods), and create regression thresholds for automated CI validation

### Test Interface Compliance & Code Quality

- [ ] **T110**: Audit all existing StreamHub test classes for proper test interface implementation according to updated guidelines in `.github/instructions/indicator-stream.instructions.md`
- [ ] **T111**: Update StreamHub test classes that implement wrong interfaces (e.g., missing `ITestChainObserver` for chainable indicators)
- [ ] **T112**: Add comprehensive rollback validation tests to all StreamHub test classes. The RollbackValidation test MUST be implemented in the appropriate observer test methods (QuoteObserver, ChainObserver, PairsObserver) and MUST use mutable `List<Quote>` (not static array), add all quotes, verify results match series exactly with strict ordering, remove a single historical quote (not just the last), rebuild expected series with revised quote list (one missing), assert exact count and strict ordering for both before and after removal, never re-add the removed quote (revised series is new ground truth), and reference Adx.StreamHub.Tests.cs for canonical structure. This applies to ALL indicators, not just those with explicit RollbackState() implementations.
- [ ] **T113**: Verify all dual-stream indicators (Correlation, Beta, PRS) implement `ITestPairsObserver` interface correctly
- [ ] **T114**: Create validation script to check test interface compliance across all StreamHub tests

**Notes**:

- Documentation tasks (D001-D007) can proceed in parallel with implementation
- Quality gate tasks (Q001-Q004) should run after substantial implementation progress (e.g., post-Phase 1a completion)
- Test infrastructure already exists; no additional scaffolding tasks required

## Summary

**CRITICAL UPDATE**: Comprehensive audit reveals 1:1:1 parity requirement across all implementation styles:

- **Total Series implementations**: 85 indicators (baseline)
- **Total BufferList implementations**: 59 complete, **26 missing**
- **Total StreamHub implementations**: 41 complete, **44 missing**
- **1:1:1 Target**: 85 BufferList + 85 StreamHub = 170 streaming implementations total
- **Current streaming coverage**: 100/170 = **59% complete**

### Missing Implementation Analysis

- **BufferList missing (26)**: ConnorsRsi, Fractal*, HtTrendline*, Hurst*, Ichimoku*, MaEnvelopes, ParabolicSar, PivotPoints, Pivots, Pmo, Pvo, QuotePart, Renko, RenkoAtr, RocWb, RollingPivots, Slope*, Smi, StarcBands, Stc, StdDevChannels, Tsi, VolatilityStop, Vortex, Vwap, ZigZag
- **StreamHub missing (44)**: Chandelier, Chop, ConnorsRsi, Donchian, Dpo, Dynamic, ElderRay, Fcb, FisherTransform, ForceIndex, Fractal*, Gator, HeikinAshi, HtTrendline*, Hurst*, Ichimoku*, Keltner, Kvo, MaEnvelopes, Marubozu, Mfi, ParabolicSar, PivotPoints, Pivots, Pmo, Pvo, QuotePart, RenkoAtr, RocWb, RollingPivots, Slope*, SmaAnalysis, Smi, StarcBands, Stc, StdDev, StdDevChannels, SuperTrend, Tsi, UlcerIndex, VolatilityStop, Vortex, Vwap, ZigZag

**Note**: Asterisked (*) indicators may be deferred to v2 due to streaming constraints research needed.

- **Total implementation tasks**: 170 (85 BufferList + 85 StreamHub)
- **Remaining tasks**: 70 (26 BufferList + 44 StreamHub)  
- **Supporting tasks**: 20+ (documentation, quality gates, compliance)
- **Documentation tasks**: 7 (D001-D007)  
- **Quality gate tasks**: 6 (Q001-Q006)
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
- [ ] T120 Update indicator-stream.instructions.md to require comprehensive rollback validation testing for all StreamHub indicators and reference the new base class virtual method. The rollback validation pattern must be implemented in appropriate observer test methods (QuoteObserver, ChainObserver, PairsObserver) for all indicators.

## Compliance Gap Remediation (Post-Audit)

Based on StreamHub audit (A002) findings, the following specific gaps must be addressed:

### Vwma StreamHub Test Compliance (CRITICAL)

- [x] **T121**: Fix Vwma StreamHub test class compliance in `tests/indicators/s-z/Vwma/Vwma.StreamHub.Tests.cs` ✅ COMPLETE
  - [x] Add missing test interfaces: `ITestQuoteObserver`, `ITestChainProvider` ✅
  - [x] Implement provider history testing (Insert/Remove operations with Series parity checks) ✅
  - [x] Add proper cleanup operations (Unsubscribe/EndTransmission calls) ✅
  - [x] Add Series parity validation with strict ordering comparison ✅
  - [x] Follow EMA StreamHub test as canonical pattern reference ✅

### Complete StreamHub Test Interface Audit

- [x] **T122**: Comprehensive audit of all StreamHub test classes for missing test interfaces ✅ COMPLETE
  - [x] Verified EMA, SMA, Correlation implement correct interfaces per provider pattern ✅
  - [x] Identified Vwma as isolated case with missing interfaces (addressed in T121) ✅
  - [x] Confirmed gaps are NOT systemic - most StreamHub tests follow proper patterns ✅

### Quality Validation

- [ ] **T123**: Validate remediation completeness by re-running StreamHub audit (A002)
  - [ ] Confirm all identified gaps have been addressed
  - [ ] Verify test patterns match instruction file requirements
  - [ ] Ensure Series parity and provider history testing coverage is complete

**Priority**: HIGH - These tasks address specific compliance gaps found during the audit phase and are prerequisite to declaring the StreamHub instruction file compliance complete.

---
Last updated: October 13, 2025

### Systematic Documentation Updates (NEW)

- [ ] **T124**: Update indicator documentation pages for all streaming-enabled indicators
  - [ ] Add "## Streaming" sections to 74 indicators missing streaming documentation
  - [ ] Use SMA/EMA documentation patterns as template (BufferList + StreamHub examples)
  - [ ] Ensure consistency with instruction file references
  - [ ] Verify code examples compile and produce correct results
