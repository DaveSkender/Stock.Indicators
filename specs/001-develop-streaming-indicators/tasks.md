# Tasks: streaming indicators framework

**Input**: Design documents from `/specs/001-develop-streaming-indicators/`
**Prerequisites**: plan.md (required) — data entities documented in plan.md §Data Model

## Requirements Quality Validation

Before implementing each indicator, review the corresponding checklist for requirements validation:

- **BufferList style**: [checklists/buffer-list.md](checklists/buffer-list.md) — 113 validation items
- **StreamHub style**: [checklists/stream-hub.md](checklists/stream-hub.md) — 115 validation items

These checklists ensure:

- Deterministic mathematical equality (no approximate equality assertions)
- Proper test interface implementation (`ITestReusableBufferList`, `ITestNonStandardBufferListCache`)
- Complete coverage of edge cases, error conditions, and performance expectations
- Alignment with constitution principles and instruction file patterns

## Format: `[ID] Description`

- Include exact file paths in descriptions
- All tasks are independently parallelizable

## Path Conventions

Paths assume the single-project layout at the repository root:

- Source: `src/`
- Tests: `tests/`

## Missing BufferList Implementations

The following indicators have series-style implementations but lack BufferList implementations:

### BufferList A-D Group (16 indicators)

- [x] T001 Implement Alligator BufferList in `src/a-d/Alligator/Alligator.BufferList.cs` ✅ PR #1497
- [x] T002 Implement Aroon BufferList in `src/a-d/Aroon/Aroon.BufferList.cs` ✅ PR #1497
- [x] T003 Implement AtrStop BufferList in `src/a-d/AtrStop/AtrStop.BufferList.cs` ✅ PR #1497
- [x] T004 Implement Awesome BufferList in `src/a-d/Awesome/Awesome.BufferList.cs` ✅ PR #1497
- [x] T005 Implement Beta BufferList in `src/a-d/Beta/Beta.BufferList.cs` ✅
- [x] T006 Implement Bop BufferList in `src/a-d/Bop/Bop.BufferList.cs`
- [x] T007 Implement ChaikinOsc BufferList in `src/a-d/ChaikinOsc/ChaikinOsc.BufferList.cs`
- [x] T008 Implement Chandelier BufferList in `src/a-d/Chandelier/Chandelier.BufferList.cs`
- [x] T009 Implement Chop BufferList in `src/a-d/Chop/Chop.BufferList.cs`
- [x] T010 Implement Cmf BufferList in `src/a-d/Cmf/Cmf.BufferList.cs`
- [ ] T011 Implement ConnorsRsi BufferList in `src/a-d/ConnorsRsi/ConnorsRsi.BufferList.cs`
- [X] T012 Implement Correlation BufferList in `src/a-d/Correlation/Correlation.BufferList.cs` ✅ PR #1499 ✓ Verified
- [X] T013 Implement Doji BufferList in `src/a-d/Doji/Doji.BufferList.cs` ✅ PR #1499 ✓ Verified
- [X] T014 Implement Donchian BufferList in `src/a-d/Donchian/Donchian.BufferList.cs` ✅ PR #1499 ✓ Verified
- [X] T015 Implement Dpo BufferList in `src/a-d/Dpo/Dpo.BufferList.cs` ✅ PR #1499 ✓ Verified
- [X] T016 Implement Dynamic BufferList in `src/a-d/Dynamic/Dynamic.BufferList.cs` ✅ This PR ✓ Verified

### BufferList E-K Group (12 indicators)

- [X] T017 Implement ElderRay BufferList in `src/e-k/ElderRay/ElderRay.BufferList.cs` ✅ This PR ✓ Verified
- [X] T018 Implement Fcb BufferList in `src/e-k/Fcb/Fcb.BufferList.cs` ✅ This PR ✓ Verified
- [X] T019 Implement FisherTransform BufferList in `src/e-k/FisherTransform/FisherTransform.BufferList.cs` ✅ This PR ⚠️ FP precision
- [X] T020 Implement ForceIndex BufferList in `src/e-k/ForceIndex/ForceIndex.BufferList.cs` ✅ This PR ✓ Verified
- [ ] T021 Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs`
- [ ] T022 Implement Gator BufferList in `src/e-k/Gator/Gator.BufferList.cs`
- [ ] T023 Implement HeikinAshi BufferList in `src/e-k/HeikinAshi/HeikinAshi.BufferList.cs`
- [ ] T024 Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs`
- [ ] T025 Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs`
- [ ] T026 Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs`
- [ ] T027 Implement Keltner BufferList in `src/e-k/Keltner/Keltner.BufferList.cs`
- [ ] T028 Implement Kvo BufferList in `src/e-k/Kvo/Kvo.BufferList.cs`

### BufferList M-R Group (13 indicators)

- [ ] T029 Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs`
- [ ] T030 Implement Marubozu BufferList in `src/m-r/Marubozu/Marubozu.BufferList.cs`
- [ ] T031 Implement Mfi BufferList in `src/m-r/Mfi/Mfi.BufferList.cs`
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

- [ ] T042 Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs`
- [ ] T043 Implement SmaAnalysis BufferList in `src/s-z/SmaAnalysis/SmaAnalysis.BufferList.cs`
- [ ] T044 Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs`
- [ ] T045 Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs`
- [ ] T046 Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs`
- [ ] T047 Implement StdDev BufferList in `src/s-z/StdDev/StdDev.BufferList.cs`
- [ ] T048 Implement StdDevChannels BufferList in `src/s-z/StdDevChannels/StdDevChannels.BufferList.cs`
- [ ] T049 Implement SuperTrend BufferList in `src/s-z/SuperTrend/SuperTrend.BufferList.cs`
- [ ] T050 Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs`
- [ ] T051 Implement UlcerIndex BufferList in `src/s-z/UlcerIndex/UlcerIndex.BufferList.cs`
- [ ] T052 Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs`
- [ ] T053 Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs`
- [ ] T054 Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs`
- [ ] T055 Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs`

## Missing StreamHub Implementations

The following indicators have series-style implementations but lack StreamHub implementations:

### StreamHub A-D Group (14 indicators)

- [x] T056 Implement Aroon StreamHub in `src/a-d/Aroon/Aroon.StreamHub.cs`
- [x] T057 Implement Awesome StreamHub in `src/a-d/Awesome/Awesome.StreamHub.cs`
- [ ] T058 Implement Beta StreamHub in `src/a-d/Beta/Beta.StreamHub.cs`
- [ ] T059 Implement Bop StreamHub in `src/a-d/Bop/Bop.StreamHub.cs`
- [ ] T060 Implement ChaikinOsc StreamHub in `src/a-d/ChaikinOsc/ChaikinOsc.StreamHub.cs`
- [ ] T061 Implement Chandelier StreamHub in `src/a-d/Chandelier/Chandelier.StreamHub.cs`
- [ ] T062 Implement Chop StreamHub in `src/a-d/Chop/Chop.StreamHub.cs`
- [ ] T063 Implement Cmf StreamHub in `src/a-d/Cmf/Cmf.StreamHub.cs`
- [ ] T064 Implement ConnorsRsi StreamHub in `src/a-d/ConnorsRsi/ConnorsRsi.StreamHub.cs`
- [x] T065 Implement Correlation StreamHub in `src/a-d/Correlation/Correlation.StreamHub.cs` (WIP - dual-stream sync needs work)
- [x] T066 Implement Doji StreamHub in `src/a-d/Doji/Doji.StreamHub.cs`
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
- Implement appropriate interface (`IBufferReusable` or `IBufferList`)
- Follow patterns from `.github/instructions/buffer-indicators.instructions.md`
- Provide both standard constructor and constructor with quotes parameter
- Use universal `BufferUtilities` extension methods for buffer management
- Include comprehensive unit tests matching patterns in existing tests
- Ensure mathematical correctness matches series implementation

### StreamHub Implementation Requirements

- Extend `ChainProvider<TIn, TResult>` or `QuoteProvider<TIn, TResult>`
- Follow patterns from `.github/instructions/stream-indicators.instructions.md`
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
- [ ] **D007**: Update `src/_common/ObsoleteV3.md` with streaming capability summary and migration guidance
- [ ] **T108**: Update indicator documentation pages (`docs/_indicators/*.md`) for all streaming-enabled indicators with usage examples (NFR-005)
- [ ] **T109**: Expand `src/_common/ObsoleteV3.md` with comprehensive streaming migration guidance including performance benefits and API patterns (NFR-006)

### Quality Gates

- [ ] **Q001**: Update public API approval test baselines for streaming additions (`tests/public-api/`)
- [ ] **Q002**: Run performance benchmarks comparing BufferList vs Series for representative indicators
- [ ] **Q003**: Run performance benchmarks comparing StreamHub vs Series for representative indicators
- [ ] **Q004**: Validate memory overhead stays within <10KB per instance target (NFR-002)

**Notes**:

- Documentation tasks (D001-D007) can proceed in parallel with implementation
- Quality gate tasks (Q001-Q004) should run after substantial implementation progress (e.g., post-Phase 1a completion)
- Test infrastructure already exists; no additional scaffolding tasks required

## Summary

- **Total BufferList tasks**: 55
- **Total StreamHub tasks**: 52
- **Total implementation tasks**: 107
- **Current implementations**: BufferList (29), StreamHub (32)
- **Target coverage**: All 84 series indicators with both BufferList and StreamHub styles
- **Documentation tasks**: 7 (D001-D007)
- **Quality gate tasks**: 4 (Q001-Q004)

---
Last updated: October 7, 2025
