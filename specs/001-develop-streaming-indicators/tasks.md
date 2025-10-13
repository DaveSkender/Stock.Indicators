# Tasks: streaming indicators framework

**Input**: Design documents from `/specs/001-develop-streaming-indicators/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), data-model.md

**Organization**: Tasks are grouped by implementation phase and indicator category to enable systematic completion and validation.

## Format: `[ID] [P?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions

Paths assume the single-project layout at the repository root:

- Source: `src/`
- Tests: `tests/`

## Requirements Quality Validation

Before implementing each indicator, review the corresponding simplified checklist for requirements validation:

- **BufferList style**: [checklists/buffer-list.md](checklists/buffer-list.md) — 15 essential validation items
- **StreamHub style**: [checklists/stream-hub.md](checklists/stream-hub.md) — 18 essential validation items

These simplified checklists ensure:

- Constitution compliance (mathematical precision, performance, validation, testing, documentation)
- Instruction file adherence (base classes, interfaces, test patterns, utilities)
- Essential quality gates (clarity, completeness, consistency, verifiability)

## Requirements Quality Validation

Before implementing each indicator, review the corresponding simplified checklist for requirements validation:

- **BufferList style**: [checklists/buffer-list.md](checklists/buffer-list.md) — 15 essential validation items
- **StreamHub style**: [checklists/stream-hub.md](checklists/stream-hub.md) — 18 essential validation items

These simplified checklists ensure:

- Constitution compliance (mathematical precision, performance, validation, testing, documentation)
- Instruction file adherence (base classes, interfaces, test patterns, utilities)
- Essential quality gates (clarity, completeness, consistency, verifiability)

---

## Phase 1: Infrastructure & Compliance Audits (COMPLETED) ✅

**Purpose**: Establish instruction file compliance and quality foundation before expanding streaming indicator coverage.

### Compliance Audits (A-series)

- [x] **A001** [P] Audit existing BufferList implementations for instruction file compliance (`src/**/*.BufferList.cs` against `.github/instructions/indicator-buffer.instructions.md`) ✅
  - Verify correct base class inheritance (`BufferList<TResult>`)
  - Check interface implementation (`IIncrementFromChain`/`IIncrementFromQuote`/`IIncrementFromPairs`)
  - Validate constructor patterns (params-only and params+quotes variants)
  - Confirm `BufferUtilities` usage instead of manual buffer management
  - Check member ordering per instruction file conventions

- [x] **A002** [P] Audit existing StreamHub implementations for instruction file compliance (`src/**/*.StreamHub.cs` against `.github/instructions/indicator-stream.instructions.md`) ✅
  - Verify correct provider base (`ChainProvider`/`QuoteProvider`/`PairsProvider`)
  - Check test interface implementation requirements
  - Validate provider history testing coverage (Insert/Remove scenarios)
  - Confirm performance benchmarking inclusion
  - Check member ordering per instruction file conventions

- [x] **A003** [P] Audit BufferList test classes for compliance (`tests/**/*.BufferList.Tests.cs`) ✅
  - Verify inheritance from `BufferListTestBase` (not `TestBase`)
  - Check implementation of correct test interfaces
  - Validate coverage of 5 required test methods
  - Confirm Series parity validation patterns

- [x] **A004** [P] Audit StreamHub test classes for compliance (`tests/**/*.StreamHub.Tests.cs`) ✅
  - Verify inheritance from `StreamHubTestBase`
  - Check implementation of correct test interfaces per provider pattern
  - Validate provider history testing (Insert/Remove scenarios)
  - Confirm performance benchmarking inclusion

- [x] **A005** [P] Update indicator documentation pages for instruction file compliance ✅
  - Ensure streaming usage sections reference instruction files
  - Update examples to match current API patterns
  - Verify consistency with catalog entries

- [x] **A006** Identify and prioritize instruction file compliance gaps ✅
  - Create priority matrix based on constitution principle violations
  - Document specific remediation steps for high-priority gaps
  - Estimate effort for bringing existing implementations into compliance

### Compliance Remediation (T121-T124)

- [x] **T121** Fix Vwma StreamHub test class compliance in `tests/indicators/s-z/Vwma/Vwma.StreamHub.Tests.cs` ✅
  - Add missing test interfaces: `ITestQuoteObserver`, `ITestChainProvider`
  - Implement provider history testing (Insert/Remove operations with Series parity checks)
  - Add proper cleanup operations (Unsubscribe/EndTransmission calls)
  - Add Series parity validation with strict ordering comparison
  - Follow EMA StreamHub test as canonical pattern reference

- [x] **T122** [P] Comprehensive audit of all StreamHub test classes for missing test interfaces ✅
  - Verified EMA, SMA, Correlation implement correct interfaces per provider pattern
  - Identified Vwma as isolated case with missing interfaces (addressed in T121)
  - Confirmed gaps are NOT systemic - most StreamHub tests follow proper patterns

- [ ] **T123** Validate remediation completeness by re-running StreamHub audit (A002)
  - Confirm all identified gaps have been addressed
  - Verify test patterns match instruction file requirements
  - Ensure Series parity and provider history testing coverage is complete

- [ ] **T124** Update indicator documentation pages for all streaming-enabled indicators
  - Add "## Streaming" sections to 74 indicators missing streaming documentation
  - Use SMA/EMA documentation patterns as template (BufferList + StreamHub examples)
  - Ensure consistency with instruction file references
  - Verify code examples compile and produce correct results

**Checkpoint**: Phase 1 establishes quality foundation and compliance baseline for Phase 2 expansion

---

## Phase 2: BufferList Implementations

**Purpose**: Complete BufferList streaming implementations for all feasible indicators

**Dependencies**: Phase 1 compliance audits complete

## Phase 2: BufferList Implementations

**Purpose**: Complete BufferList streaming implementations for all feasible indicators

**Dependencies**: Phase 1 compliance audits complete

### BufferList A-D Group (19 indicators)

- [x] **T001** [P] Implement Alligator BufferList in `src/a-d/Alligator/Alligator.BufferList.cs` ✅
- [x] **T002** [P] Implement Aroon BufferList in `src/a-d/Aroon/Aroon.BufferList.cs` ✅
- [x] **T003** [P] Implement AtrStop BufferList in `src/a-d/AtrStop/AtrStop.BufferList.cs` ✅
- [x] **T004** [P] Implement Awesome BufferList in `src/a-d/Awesome/Awesome.BufferList.cs` ✅
- [x] **T005** [P] Implement Beta BufferList in `src/a-d/Beta/Beta.BufferList.cs` ✅
- [x] **T006** [P] Implement Bop BufferList in `src/a-d/Bop/Bop.BufferList.cs` ✅
- [x] **T007** [P] Implement ChaikinOsc BufferList in `src/a-d/ChaikinOsc/ChaikinOsc.BufferList.cs` ✅
- [x] **T008** [P] Implement Chandelier BufferList in `src/a-d/Chandelier/Chandelier.BufferList.cs` ✅
- [x] **T009** [P] Implement Chop BufferList in `src/a-d/Chop/Chop.BufferList.cs` ✅
- [x] **T010** [P] Implement Cmf BufferList in `src/a-d/Cmf/Cmf.BufferList.cs` ✅
- [ ] **T011** [P] Implement ConnorsRsi BufferList in `src/a-d/ConnorsRsi/ConnorsRsi.BufferList.cs`
- [x] **T012** [P] Implement Correlation BufferList in `src/a-d/Correlation/Correlation.BufferList.cs` ✅
- [x] **T013** [P] Implement Doji BufferList in `src/a-d/Doji/Doji.BufferList.cs` ✅
- [x] **T014** [P] Implement Donchian BufferList in `src/a-d/Donchian/Donchian.BufferList.cs` ✅
- [x] **T015** [P] Implement Dpo BufferList in `src/a-d/Dpo/Dpo.BufferList.cs` ✅
- [x] **T016** [P] Implement Dynamic BufferList in `src/a-d/Dynamic/Dynamic.BufferList.cs` ✅

### BufferList E-K Group (9 indicators)

- [x] **T017** [P] Implement ElderRay BufferList in `src/e-k/ElderRay/ElderRay.BufferList.cs` ✅
- [x] **T018** [P] Implement Fcb BufferList in `src/e-k/Fcb/Fcb.BufferList.cs` ✅
- [x] **T019** [P] Implement FisherTransform BufferList in `src/e-k/FisherTransform/FisherTransform.BufferList.cs` ✅
- [x] **T020** [P] Implement ForceIndex BufferList in `src/e-k/ForceIndex/ForceIndex.BufferList.cs` ✅
- [ ] **T021** [P] Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs` ⚠️ **DEFERRED** (v2): Requires future data access pattern (looks ahead 2 periods)
- [x] **T022** [P] Implement Gator BufferList in `src/e-k/Gator/Gator.BufferList.cs` ✅
- [x] **T023** [P] Implement HeikinAshi BufferList in `src/e-k/HeikinAshi/HeikinAshi.BufferList.cs` ✅
- [ ] **T024** [P] Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs` ⚠️ **DEFERRED** (v2): Complex Hilbert Transform streaming compatibility research needed
- [ ] **T025** [P] Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs` ⚠️ **DEFERRED** (v2): Full dataset access patterns incompatible with incremental streaming
- [ ] **T026** [P] Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs` ⚠️ **DEFERRED** (v2): Future offset calculations incompatible with streaming
- [x] **T027** [P] Implement Keltner BufferList in `src/e-k/Keltner/Keltner.BufferList.cs` ✅
- [x] **T028** [P] Implement Kvo BufferList in `src/e-k/Kvo/Kvo.BufferList.cs` ✅

### BufferList M-R Group (13 indicators)

- [ ] **T029** [P] Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs`
- [x] **T030** [P] Implement Marubozu BufferList in `src/m-r/Marubozu/Marubozu.BufferList.cs` ✅
- [x] **T031** [P] Implement Mfi BufferList in `src/m-r/Mfi/Mfi.BufferList.cs` ✅
- [ ] **T032** [P] Implement ParabolicSar BufferList in `src/m-r/ParabolicSar/ParabolicSar.BufferList.cs`
- [ ] **T033** [P] Implement PivotPoints BufferList in `src/m-r/PivotPoints/PivotPoints.BufferList.cs`
- [ ] **T034** [P] Implement Pivots BufferList in `src/m-r/Pivots/Pivots.BufferList.cs`
- [x] **T035** [P] Implement Pmo BufferList in `src/m-r/Pmo/Pmo.BufferList.cs` ✅
- [x] **T036** [P] Implement Prs BufferList in `src/m-r/Prs/Prs.BufferList.cs` ✅
- [x] **T037** [P] Implement Pvo BufferList in `src/m-r/Pvo/Pvo.BufferList.cs` ✅
- [ ] **T038** [P] Implement Renko BufferList in `src/m-r/Renko/Renko.BufferList.cs`
- [ ] **T039** [P] Implement RenkoAtr BufferList in `src/m-r/RenkoAtr/RenkoAtr.BufferList.cs`
- [ ] **T040** [P] Implement RocWb BufferList in `src/m-r/RocWb/RocWb.BufferList.cs`
- [ ] **T041** [P] Implement RollingPivots BufferList in `src/m-r/RollingPivots/RollingPivots.BufferList.cs`

### BufferList S-Z Group (14 indicators)

- [ ] **T042** [P] Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs` ⚠️ **DEFERRED** (v2): Retroactive updates incompatible with append-only streaming
- [x] **T043** [P] Implement SmaAnalysis BufferList in `src/s-z/SmaAnalysis/SmaAnalysis.BufferList.cs` ✅
- [x] **T044** [P] Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs` ✅
- [x] **T045** [P] Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs` ✅
- [ ] **T046** [P] Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs`
- [x] **T047** [P] Implement StdDev BufferList in `src/s-z/StdDev/StdDev.BufferList.cs` ✅
- [ ] **T048** [P] Implement StdDevChannels BufferList in `src/s-z/StdDevChannels/StdDevChannels.BufferList.cs`
- [x] **T049** [P] Implement SuperTrend BufferList in `src/s-z/SuperTrend/SuperTrend.BufferList.cs` ✅
- [ ] **T050** [P] Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs`
- [x] **T051** [P] Implement UlcerIndex BufferList in `src/s-z/UlcerIndex/UlcerIndex.BufferList.cs` ✅
- [x] **T052** [P] Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` ✅
- [x] **T053** [P] Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs` ✅
- [x] **T054** [P] Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs` ✅
- [ ] **T055** [P] Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs`

### BufferList Common Utilities

- [ ] **T125** [P] Implement QuotePart BufferList in `src/_common/QuotePart/QuotePart.BufferList.cs`

**Checkpoint**: Phase 2 completion establishes comprehensive BufferList coverage (59/85 complete, 26 remaining)

---

## Phase 3: StreamHub Implementations

**Purpose**: Complete StreamHub streaming implementations for all feasible indicators

**Dependencies**: Phase 1 compliance audits complete (Phase 2 can proceed in parallel)

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

## Phase 3: StreamHub Implementations

**Purpose**: Complete StreamHub streaming implementations for all feasible indicators

**Dependencies**: Phase 1 compliance audits complete (Phase 2 can proceed in parallel)

### StreamHub A-D Group (8 indicators)

- [ ] **T061** [P] Implement Chandelier StreamHub in `src/a-d/Chandelier/Chandelier.StreamHub.cs`
- [ ] **T062** [P] Implement Chop StreamHub in `src/a-d/Chop/Chop.StreamHub.cs`
- [ ] **T063** [P] Implement Cmf StreamHub in `src/a-d/Cmf/Cmf.StreamHub.cs`
- [ ] **T064** [P] Implement ConnorsRsi StreamHub in `src/a-d/ConnorsRsi/ConnorsRsi.StreamHub.cs`
- [ ] **T065** [P] Implement Correlation StreamHub in `src/a-d/Correlation/Correlation.StreamHub.cs` (WIP - dual-stream sync needs work, see Issue #1511)
- [ ] **T067** [P] Implement Donchian StreamHub in `src/a-d/Donchian/Donchian.StreamHub.cs`
- [ ] **T068** [P] Implement Dpo StreamHub in `src/a-d/Dpo/Dpo.StreamHub.cs`
- [ ] **T069** [P] Implement Dynamic StreamHub in `src/a-d/Dynamic/Dynamic.StreamHub.cs`

### StreamHub E-K Group (12 indicators)

- [ ] **T070** [P] Implement ElderRay StreamHub in `src/e-k/ElderRay/ElderRay.StreamHub.cs`
- [ ] **T071** [P] Implement Fcb StreamHub in `src/e-k/Fcb/Fcb.StreamHub.cs`
- [ ] **T072** [P] Implement FisherTransform StreamHub in `src/e-k/FisherTransform/FisherTransform.StreamHub.cs`
- [ ] **T073** [P] Implement ForceIndex StreamHub in `src/e-k/ForceIndex/ForceIndex.StreamHub.cs`
- [ ] **T074** [P] Implement Fractal StreamHub in `src/e-k/Fractal/Fractal.StreamHub.cs` ⚠️ **DEFERRED** (v2): Future data access incompatible with streaming
- [ ] **T075** [P] Implement Gator StreamHub in `src/e-k/Gator/Gator.StreamHub.cs`
- [ ] **T076** [P] Implement HeikinAshi StreamHub in `src/e-k/HeikinAshi/HeikinAshi.StreamHub.cs`
- [ ] **T077** [P] Implement HtTrendline StreamHub in `src/e-k/HtTrendline/HtTrendline.StreamHub.cs` ⚠️ **DEFERRED** (v2): Hilbert Transform streaming compatibility research needed
- [ ] **T078** [P] Implement Hurst StreamHub in `src/e-k/Hurst/Hurst.StreamHub.cs` ⚠️ **DEFERRED** (v2): Full dataset access patterns incompatible with streaming
- [ ] **T079** [P] Implement Ichimoku StreamHub in `src/e-k/Ichimoku/Ichimoku.StreamHub.cs` ⚠️ **DEFERRED** (v2): Future offset calculations incompatible with streaming
- [ ] **T080** [P] Implement Keltner StreamHub in `src/e-k/Keltner/Keltner.StreamHub.cs`
- [ ] **T081** [P] Implement Kvo StreamHub in `src/e-k/Kvo/Kvo.StreamHub.cs`

### StreamHub M-R Group (13 indicators)

- [ ] **T082** [P] Implement MaEnvelopes StreamHub in `src/m-r/MaEnvelopes/MaEnvelopes.StreamHub.cs`
- [ ] **T083** [P] Implement Marubozu StreamHub in `src/m-r/Marubozu/Marubozu.StreamHub.cs`
- [ ] **T084** [P] Implement Mfi StreamHub in `src/m-r/Mfi/Mfi.StreamHub.cs`
- [ ] **T085** [P] Implement ParabolicSar StreamHub in `src/m-r/ParabolicSar/ParabolicSar.StreamHub.cs`
- [ ] **T086** [P] Implement PivotPoints StreamHub in `src/m-r/PivotPoints/PivotPoints.StreamHub.cs`
- [ ] **T087** [P] Implement Pivots StreamHub in `src/m-r/Pivots/Pivots.StreamHub.cs`
- [ ] **T088** [P] Implement Pmo StreamHub in `src/m-r/Pmo/Pmo.StreamHub.cs`
- [ ] **T089** [P] Implement Prs StreamHub in `src/m-r/Prs/Prs.StreamHub.cs`
- [ ] **T090** [P] Implement Pvo StreamHub in `src/m-r/Pvo/Pvo.StreamHub.cs`
- [ ] **T091** [P] Implement RenkoAtr StreamHub in `src/m-r/RenkoAtr/RenkoAtr.StreamHub.cs`
- [ ] **T092** [P] Implement RocWb StreamHub in `src/m-r/RocWb/RocWb.StreamHub.cs`
- [ ] **T093** [P] Implement RollingPivots StreamHub in `src/m-r/RollingPivots/RollingPivots.StreamHub.cs`

### StreamHub S-Z Group (14 indicators)

- [ ] **T094** [P] Implement Slope StreamHub in `src/s-z/Slope/Slope.StreamHub.cs` ⚠️ **DEFERRED** (v2): Retroactive calculation incompatible with streaming
- [ ] **T095** [P] Implement SmaAnalysis StreamHub in `src/s-z/SmaAnalysis/SmaAnalysis.StreamHub.cs`
- [ ] **T096** [P] Implement Smi StreamHub in `src/s-z/Smi/Smi.StreamHub.cs`
- [ ] **T097** [P] Implement StarcBands StreamHub in `src/s-z/StarcBands/StarcBands.StreamHub.cs`
- [ ] **T098** [P] Implement Stc StreamHub in `src/s-z/Stc/Stc.StreamHub.cs`
- [ ] **T099** [P] Implement StdDev StreamHub in `src/s-z/StdDev/StdDev.StreamHub.cs`
- [ ] **T100** [P] Implement StdDevChannels StreamHub in `src/s-z/StdDevChannels/StdDevChannels.StreamHub.cs`
- [ ] **T101** [P] Implement SuperTrend StreamHub in `src/s-z/SuperTrend/SuperTrend.StreamHub.cs`
- [ ] **T102** [P] Implement Tsi StreamHub in `src/s-z/Tsi/Tsi.StreamHub.cs`
- [ ] **T103** [P] Implement UlcerIndex StreamHub in `src/s-z/UlcerIndex/UlcerIndex.StreamHub.cs`
- [ ] **T104** [P] Implement VolatilityStop StreamHub in `src/s-z/VolatilityStop/VolatilityStop.cs`
- [ ] **T105** [P] Implement Vortex StreamHub in `src/s-z/Vortex/Vortex.StreamHub.cs`
- [ ] **T106** [P] Implement Vwap StreamHub in `src/s-z/Vwap/Vwap.StreamHub.cs`
- [ ] **T107** [P] Implement ZigZag StreamHub in `src/s-z/ZigZag/ZigZag.StreamHub.cs`

### StreamHub Common Utilities

- [ ] **T126** [P] Implement QuotePart StreamHub in `src/_common/QuotePart/QuotePart.StreamHub.cs`

**Checkpoint**: Phase 3 completion achieves 1:1:1 parity across all three implementation styles (Series, BufferList, StreamHub)

---

## Phase 4: Test Infrastructure & Quality Assurance

**Purpose**: Ensure comprehensive test coverage and performance validation

**Dependencies**: Phases 2 and 3 implementations

## Phase 4: Test Infrastructure & Quality Assurance

**Purpose**: Ensure comprehensive test coverage and performance validation

**Dependencies**: Phases 2 and 3 implementations

### StreamHub Test Interface Compliance

- [ ] **T110** [P] Audit all existing StreamHub test classes for proper test interface implementation according to updated guidelines in `.github/instructions/indicator-stream.instructions.md`
- [ ] **T111** Update StreamHub test classes that implement wrong interfaces (e.g., missing `ITestChainObserver` for chainable indicators)
- [ ] **T112** Add comprehensive rollback validation tests to all StreamHub test classes following canonical pattern from `Adx.StreamHub.Tests.cs`
  - Implement in appropriate observer test methods (QuoteObserver, ChainObserver, PairsObserver)
  - Use mutable `List<Quote>` (not static array)
  - Add all quotes, verify results match series exactly with strict ordering
  - Remove a single historical quote (not just the last)
  - Rebuild expected series with revised quote list
  - Assert exact count and strict ordering for both before and after removal
  - Never re-add the removed quote (revised series is new ground truth)
- [ ] **T113** [P] Verify all dual-stream indicators (Correlation, Beta, PRS) implement `ITestPairsObserver` interface correctly
- [ ] **T114** [P] Create validation script to check test interface compliance across all StreamHub tests

### Provider History Testing

- [ ] **T115** Add provider history (Insert/Remove) testing to QuoteObserver tests in AdxHub at `tests/indicators/a-d/Adx/Adx.StreamHub.Tests.cs`
  - Use `List<Quote>` for mutability
  - Remove by index and verify strict ordering and count
  - Never re-add removed quote
  - Reference `Adx.StreamHub.Tests.cs` as canonical example
- [ ] **T116** Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in AtrHub, CciHub, MacdHub, MamaHub, ObvHub, PrsHub
- [ ] **T117** Add provider history (Insert/Remove) testing to ChainObserver tests missing Insert/Remove operations in RsiHub, StochRsiHub
- [ ] **T118** Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in StochHub, VwmaHub, WilliamsRHub
- [ ] **T119** Add virtual ProviderHistoryTesting() method to StreamHubTestBase class in `tests/indicators/_base/StreamHubTestBase.cs`
- [ ] **T120** Update `indicator-stream.instructions.md` to require comprehensive rollback validation testing for all StreamHub indicators

### Performance & Quality Gates

- [ ] **Q001** [P] Update public API approval test baselines for streaming additions (`tests/public-api/`)
- [ ] **Q002** [P] Run performance benchmarks comparing BufferList vs Series for representative indicators
  - Use BenchmarkDotNet with latency validation (<5ms mean, <10ms p95)
  - Standardized hardware (4-core 3GHz CPU, 16GB RAM, .NET 9.0 release mode)
- [ ] **Q003** [P] Run performance benchmarks comparing StreamHub vs Series for representative indicators
  - Same criteria as Q002
- [ ] **Q004** [P] Validate memory overhead stays within <10KB per instance target (NFR-002)
  - Use dotMemory profiler or equivalent
  - Test on representative indicators (SMA, EMA, RSI with 200-period lookback)
  - Document measurement procedure and baseline results for CI regression detection
- [ ] **Q005** Create automated performance regression detection for streaming indicators
  - Baseline thresholds: mean latency <5ms, p95 latency <10ms, memory overhead <10KB per instance
  - Automatic CI failure on regression >10% degradation
- [ ] **Q006** Establish memory baseline measurements for all streaming indicator types
  - Measure BufferList vs StreamHub memory efficiency
  - Document memory scaling characteristics vs lookback period (50, 100, 200 periods)
  - Create regression thresholds for automated CI validation

**Checkpoint**: Phase 4 ensures quality gates and comprehensive test coverage for all streaming implementations

---

## Phase 5: Documentation & Polish

**Purpose**: Complete user-facing documentation and migration guidance

**Dependencies**: Phases 2 and 3 implementations

### Documentation Updates (D-series)

- [x] **D001** [P] Update `docs/_indicators/Sma.md` with streaming usage section and examples ✅
- [x] **D002** [P] Update `docs/_indicators/Ema.md` with streaming usage section and examples ✅
- [ ] **D003** [P] Update `docs/_indicators/Rsi.md` with streaming usage section and examples
- [ ] **D004** [P] Update `docs/_indicators/Macd.md` with streaming usage section and examples
- [ ] **D005** [P] Update `docs/_indicators/BollingerBands.md` with streaming usage section and examples
- [ ] **D006** [P] Update `README.md` with streaming overview paragraph and quick-start example
- [ ] **D007** Update `src/MigrationGuide.V3.md` with streaming capability summary and migration guidance

### Comprehensive Documentation

- [ ] **T108** Update indicator documentation pages (`docs/_indicators/*.md`) for all streaming-enabled indicators with usage examples (NFR-005)
  - Add "## Streaming" sections to 74 indicators missing streaming documentation
  - Use SMA/EMA documentation patterns as template (BufferList + StreamHub examples)
  - Ensure consistency with instruction file references
  - Verify code examples compile and produce correct results
- [ ] **T109** Expand `src/MigrationGuide.V3.md` with comprehensive streaming migration guidance including performance benefits and API patterns (NFR-006)

**Checkpoint**: Phase 5 completion delivers polished user-facing documentation for all streaming features

---

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

- Extend `ChainProvider<TIn, TResult>`, `QuoteProvider<TIn, TResult>`, or `PairsProvider<TIn, TResult>`
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

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1**: Foundation (audits & compliance) — No dependencies, can start immediately
- **Phase 2**: BufferList implementations — Depends on Phase 1 completion
- **Phase 3**: StreamHub implementations — Depends on Phase 1 completion (can proceed in parallel with Phase 2)
- **Phase 4**: Test infrastructure & quality assurance — Depends on Phases 2 and 3 progress
- **Phase 5**: Documentation & polish — Depends on Phases 2 and 3 implementations

### Parallel Opportunities

- All [P] marked tasks within each phase can run in parallel
- Phases 2 and 3 can proceed in parallel after Phase 1 completion
- Different indicator implementations are completely independent
- Documentation tasks can begin as soon as corresponding implementations complete

### Notes

- All implementation tasks can be executed in parallel as they touch different files
- Maintain strict adherence to existing patterns and conventions
- Run `dotnet test --no-restore` after each implementation
- Follow pre-commit checklist from `.github/instructions/source-code-completion.instructions.md`
- Update catalog entries and documentation as implementations are completed

---

## Summary

**CRITICAL UPDATE**: Comprehensive audit reveals 1:1:1 parity requirement across all implementation styles:

**Implementation Coverage**:
- **Total Series implementations**: 85 indicators (baseline)
- **Total BufferList implementations**: 59 complete, 26 remaining
- **Total StreamHub implementations**: 41 complete, 44 remaining
- **1:1:1 Target**: 85 BufferList + 85 StreamHub = 170 streaming implementations total
- **Current streaming coverage**: 100/170 = **59% complete**

**Task Breakdown**:
- **Phase 1**: 10 tasks (A001-A006, T121-T124) — 6 complete, 4 remaining
- **Phase 2**: 55 BufferList implementation tasks (T001-T055) — 39 complete, 16 remaining
- **Phase 3**: 47 StreamHub implementation tasks (T061-T107, T126) — 0 complete, 47 remaining
- **Phase 4**: 11 test infrastructure tasks (T110-T120, Q001-Q006) — 0 complete, 11 remaining
- **Phase 5**: 9 documentation tasks (D001-D007, T108-T109) — 2 complete, 7 remaining
- **Total**: 132 tasks — 47 complete, 85 remaining

**Deferred to v2** (streaming incompatible): Fractal, HtTrendline, Hurst, Ichimoku, Slope

---
Last updated: October 13, 2025

