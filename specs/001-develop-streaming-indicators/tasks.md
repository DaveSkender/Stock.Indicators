# Tasks: streaming indicators framework

**Input**: Design documents from `/specs/001-develop-streaming-indicators/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), data-model.md

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

### Compliance Remediation (T171-T174)

- [x] **T171** Fix Vwma StreamHub test class compliance in `tests/indicators/s-z/Vwma/Vwma.StreamHub.Tests.cs` ✅
  - Add missing test interfaces: `ITestQuoteObserver`, `ITestChainProvider`
  - Implement provider history testing (Insert/Remove operations with Series parity checks)
  - Add proper cleanup operations (Unsubscribe/EndTransmission calls)
  - Add Series parity validation with strict ordering comparison
  - Follow EMA StreamHub test as canonical pattern reference

- [x] **T172** [P] Comprehensive audit of all StreamHub test classes for missing test interfaces ✅
  - Verified EMA, SMA, Correlation implement correct interfaces per provider pattern
  - Identified Vwma as isolated case with missing interfaces (addressed in T171)
  - Confirmed gaps are NOT systemic - most StreamHub tests follow proper patterns

- [ ] **T173** Validate remediation completeness by re-running StreamHub audit (A002)
  - Confirm all identified gaps have been addressed
  - Verify test patterns match instruction file requirements
  - Ensure Series parity and provider history testing coverage is complete

- [ ] **T174** Update indicator documentation pages for all streaming-enabled indicators
  - Add "## Streaming" sections to indicators missing streaming documentation
  - Use SMA/EMA documentation patterns as template (BufferList + StreamHub examples)
  - Ensure consistency with instruction file references
  - Verify code examples compile and produce correct results

**Checkpoint**: Phase 1 establishes quality foundation and compliance baseline for Phase 2-3 expansion

---


## Phase 2: BufferList Implementations

**Purpose**: Complete BufferList streaming implementations for all 85 indicators (1:1:1 parity with Series)

**Dependencies**: Phase 1 compliance audits complete

- [x] **T001** [P] Implement Adl BufferList in `src/a-d/Adl/Adl.BufferList.cs` ✅
- [x] **T002** [P] Implement Adx BufferList in `src/a-d/Adx/Adx.BufferList.cs` ✅
- [x] **T003** [P] Implement Alligator BufferList in `src/a-d/Alligator/Alligator.BufferList.cs` ✅
- [x] **T004** [P] Implement Alma BufferList in `src/a-d/Alma/Alma.BufferList.cs` ✅
- [x] **T005** [P] Implement Aroon BufferList in `src/a-d/Aroon/Aroon.BufferList.cs` ✅
- [x] **T006** [P] Implement Atr BufferList in `src/a-d/Atr/Atr.BufferList.cs` ✅
- [x] **T007** [P] Implement AtrStop BufferList in `src/a-d/AtrStop/AtrStop.BufferList.cs` ✅
- [x] **T008** [P] Implement Awesome BufferList in `src/a-d/Awesome/Awesome.BufferList.cs` ✅
- [x] **T009** [P] Implement Beta BufferList in `src/a-d/Beta/Beta.BufferList.cs` ✅
- [x] **T010** [P] Implement BollingerBands BufferList in `src/a-d/BollingerBands/BollingerBands.BufferList.cs` ✅
- [x] **T011** [P] Implement Bop BufferList in `src/a-d/Bop/Bop.BufferList.cs` ✅
- [x] **T012** [P] Implement Cci BufferList in `src/a-d/Cci/Cci.BufferList.cs` ✅
- [x] **T013** [P] Implement ChaikinOsc BufferList in `src/a-d/ChaikinOsc/ChaikinOsc.BufferList.cs` ✅
- [x] **T014** [P] Implement Chandelier BufferList in `src/a-d/Chandelier/Chandelier.BufferList.cs` ✅
- [x] **T015** [P] Implement Chop BufferList in `src/a-d/Chop/Chop.BufferList.cs` ✅
- [x] **T016** [P] Implement Cmf BufferList in `src/a-d/Cmf/Cmf.BufferList.cs` ✅
- [x] **T017** [P] Implement Cmo BufferList in `src/a-d/Cmo/Cmo.BufferList.cs` ✅
- [ ] **T018** [P] Implement ConnorsRsi BufferList in `src/a-d/ConnorsRsi/ConnorsRsi.BufferList.cs`
- [x] **T019** [P] Implement Correlation BufferList in `src/a-d/Correlation/Correlation.BufferList.cs` ✅
- [x] **T020** [P] Implement Dema BufferList in `src/a-d/Dema/Dema.BufferList.cs` ✅
- [x] **T021** [P] Implement Doji BufferList in `src/a-d/Doji/Doji.BufferList.cs` ✅
- [x] **T022** [P] Implement Donchian BufferList in `src/a-d/Donchian/Donchian.BufferList.cs` ✅
- [x] **T023** [P] Implement Dpo BufferList in `src/a-d/Dpo/Dpo.BufferList.cs` ✅
- [x] **T024** [P] Implement Dynamic BufferList in `src/a-d/Dynamic/Dynamic.BufferList.cs` ✅
- [x] **T025** [P] Implement ElderRay BufferList in `src/e-k/ElderRay/ElderRay.BufferList.cs` ✅
- [x] **T026** [P] Implement Ema BufferList in `src/e-k/Ema/Ema.BufferList.cs` ✅
- [x] **T027** [P] Implement Epma BufferList in `src/e-k/Epma/Epma.BufferList.cs` ✅
- [x] **T028** [P] Implement Fcb BufferList in `src/e-k/Fcb/Fcb.BufferList.cs` ✅
- [x] **T029** [P] Implement FisherTransform BufferList in `src/e-k/FisherTransform/FisherTransform.BufferList.cs` ✅
- [x] **T030** [P] Implement ForceIndex BufferList in `src/e-k/ForceIndex/ForceIndex.BufferList.cs` ✅
- [ ] **T031** [P] Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs` ⚠️ **DEFERRED** (v2)
- [x] **T032** [P] Implement Gator BufferList in `src/e-k/Gator/Gator.BufferList.cs` ✅
- [x] **T033** [P] Implement HeikinAshi BufferList in `src/e-k/HeikinAshi/HeikinAshi.BufferList.cs` ✅
- [x] **T034** [P] Implement Hma BufferList in `src/e-k/Hma/Hma.BufferList.cs` ✅
- [ ] **T035** [P] Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs` ⚠️ **DEFERRED** (v2)
- [ ] **T036** [P] Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs` ⚠️ **DEFERRED** (v2)
- [ ] **T037** [P] Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs` ⚠️ **DEFERRED** (v2)
- [x] **T038** [P] Implement Kama BufferList in `src/e-k/Kama/Kama.BufferList.cs` ✅
- [x] **T039** [P] Implement Keltner BufferList in `src/e-k/Keltner/Keltner.BufferList.cs` ✅
- [x] **T040** [P] Implement Kvo BufferList in `src/e-k/Kvo/Kvo.BufferList.cs` ✅
- [ ] **T041** [P] Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs`
- [x] **T042** [P] Implement Macd BufferList in `src/m-r/Macd/Macd.BufferList.cs` ✅
- [x] **T043** [P] Implement Mama BufferList in `src/m-r/Mama/Mama.BufferList.cs` ✅
- [x] **T044** [P] Implement Marubozu BufferList in `src/m-r/Marubozu/Marubozu.BufferList.cs` ✅
- [x] **T045** [P] Implement Mfi BufferList in `src/m-r/Mfi/Mfi.BufferList.cs` ✅
- [x] **T046** [P] Implement Obv BufferList in `src/m-r/Obv/Obv.BufferList.cs` ✅
- [ ] **T047** [P] Implement ParabolicSar BufferList in `src/m-r/ParabolicSar/ParabolicSar.BufferList.cs`
- [ ] **T048** [P] Implement PivotPoints BufferList in `src/m-r/PivotPoints/PivotPoints.BufferList.cs`
- [ ] **T049** [P] Implement Pivots BufferList in `src/m-r/Pivots/Pivots.BufferList.cs`
- [x] **T050** [P] Implement Pmo BufferList in `src/m-r/Pmo/Pmo.BufferList.cs` ✅
- [x] **T051** [P] Implement Prs BufferList in `src/m-r/Prs/Prs.BufferList.cs` ✅
- [x] **T052** [P] Implement Pvo BufferList in `src/m-r/Pvo/Pvo.BufferList.cs` ✅
- [ ] **T053** [P] Implement QuotePart BufferList in `src/_common/QuotePart/QuotePart.BufferList.cs`
- [ ] **T054** [P] Implement Renko BufferList in `src/m-r/Renko/Renko.BufferList.cs`
- [ ] **T055** [P] Implement RenkoAtr BufferList in `src/m-r/RenkoAtr/RenkoAtr.BufferList.cs`
- [x] **T056** [P] Implement Roc BufferList in `src/m-r/Roc/Roc.BufferList.cs` ✅
- [ ] **T057** [P] Implement RocWb BufferList in `src/m-r/RocWb/RocWb.BufferList.cs`
- [ ] **T058** [P] Implement RollingPivots BufferList in `src/m-r/RollingPivots/RollingPivots.BufferList.cs`
- [x] **T059** [P] Implement Rsi BufferList in `src/m-r/Rsi/Rsi.BufferList.cs` ✅
- [ ] **T060** [P] Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs` ⚠️ **DEFERRED** (v2)
- [x] **T061** [P] Implement Sma BufferList in `src/s-z/Sma/Sma.BufferList.cs` ✅
- [x] **T062** [P] Implement SmaAnalysis BufferList in `src/s-z/SmaAnalysis/SmaAnalysis.BufferList.cs` ✅
- [x] **T063** [P] Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs` ✅
- [x] **T064** [P] Implement Smma BufferList in `src/s-z/Smma/Smma.BufferList.cs` ✅
- [x] **T065** [P] Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs` ✅
- [ ] **T066** [P] Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs`
- [x] **T067** [P] Implement StdDev BufferList in `src/s-z/StdDev/StdDev.BufferList.cs` ✅
- [ ] **T068** [P] Implement StdDevChannels BufferList in `src/s-z/StdDevChannels/StdDevChannels.BufferList.cs`
- [x] **T069** [P] Implement Stoch BufferList in `src/s-z/Stoch/Stoch.BufferList.cs` ✅
- [x] **T070** [P] Implement StochRsi BufferList in `src/s-z/StochRsi/StochRsi.BufferList.cs` ✅
- [x] **T071** [P] Implement SuperTrend BufferList in `src/s-z/SuperTrend/SuperTrend.BufferList.cs` ✅
- [x] **T072** [P] Implement T3 BufferList in `src/s-z/T3/T3.BufferList.cs` ✅
- [x] **T073** [P] Implement Tema BufferList in `src/s-z/Tema/Tema.BufferList.cs` ✅
- [x] **T074** [P] Implement Tr BufferList in `src/s-z/Tr/Tr.BufferList.cs` ✅
- [x] **T075** [P] Implement Trix BufferList in `src/s-z/Trix/Trix.BufferList.cs` ✅
- [ ] **T076** [P] Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs`
- [x] **T077** [P] Implement UlcerIndex BufferList in `src/s-z/UlcerIndex/UlcerIndex.BufferList.cs` ✅
- [x] **T078** [P] Implement Ultimate BufferList in `src/s-z/Ultimate/Ultimate.BufferList.cs` ✅
- [x] **T079** [P] Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` ✅
- [x] **T080** [P] Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs` ✅
- [x] **T081** [P] Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs` ✅
- [x] **T082** [P] Implement Vwma BufferList in `src/s-z/Vwma/Vwma.BufferList.cs` ✅
- [x] **T083** [P] Implement WilliamsR BufferList in `src/s-z/WilliamsR/WilliamsR.BufferList.cs` ✅
- [x] **T084** [P] Implement Wma BufferList in `src/s-z/Wma/Wma.BufferList.cs` ✅
- [ ] **T085** [P] Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs`

**BufferList**: 66/85 complete, 19 remaining

**Checkpoint**: Phase 2 completion achieves complete BufferList coverage for all Series indicators

---

## Phase 3: StreamHub Implementations

**Purpose**: Complete StreamHub streaming implementations for all 85 indicators (1:1:1 parity with Series)

**Dependencies**: Phase 1 compliance audits complete (Phase 2 can proceed in parallel)

- [x] **T086** [P] Implement Adl StreamHub in `src/a-d/Adl/Adl.StreamHub.cs` ✅
- [x] **T087** [P] Implement Adx StreamHub in `src/a-d/Adx/Adx.StreamHub.cs` ✅
- [x] **T088** [P] Implement Alligator StreamHub in `src/a-d/Alligator/Alligator.StreamHub.cs` ✅
- [x] **T089** [P] Implement Alma StreamHub in `src/a-d/Alma/Alma.StreamHub.cs` ✅
- [x] **T090** [P] Implement Aroon StreamHub in `src/a-d/Aroon/Aroon.StreamHub.cs` ✅
- [x] **T091** [P] Implement Atr StreamHub in `src/a-d/Atr/Atr.StreamHub.cs` ✅
- [x] **T092** [P] Implement AtrStop StreamHub in `src/a-d/AtrStop/AtrStop.StreamHub.cs` ✅
- [x] **T093** [P] Implement Awesome StreamHub in `src/a-d/Awesome/Awesome.StreamHub.cs` ✅
- [x] **T094** [P] Implement Beta StreamHub in `src/a-d/Beta/Beta.StreamHub.cs` ✅
- [x] **T095** [P] Implement BollingerBands StreamHub in `src/a-d/BollingerBands/BollingerBands.StreamHub.cs` ✅
- [x] **T096** [P] Implement Bop StreamHub in `src/a-d/Bop/Bop.StreamHub.cs` ✅
- [x] **T097** [P] Implement Cci StreamHub in `src/a-d/Cci/Cci.StreamHub.cs` ✅
- [x] **T098** [P] Implement ChaikinOsc StreamHub in `src/a-d/ChaikinOsc/ChaikinOsc.StreamHub.cs` ✅
- [x] **T099** [P] Implement Chandelier StreamHub in `src/a-d/Chandelier/Chandelier.StreamHub.cs` ✅
- [x] **T100** [P] Implement Chop StreamHub in `src/a-d/Chop/Chop.StreamHub.cs` ✅
- [x] **T101** [P] Implement Cmf StreamHub in `src/a-d/Cmf/Cmf.StreamHub.cs` ✅
- [x] **T102** [P] Implement Cmo StreamHub in `src/a-d/Cmo/Cmo.StreamHub.cs` ✅
- [ ] **T103** [P] Implement ConnorsRsi StreamHub in `src/a-d/ConnorsRsi/ConnorsRsi.StreamHub.cs`
- [x] **T104** [P] Implement Correlation StreamHub in `src/a-d/Correlation/Correlation.StreamHub.cs` ✅
- [x] **T105** [P] Implement Dema StreamHub in `src/a-d/Dema/Dema.StreamHub.cs` ✅
- [x] **T106** [P] Implement Doji StreamHub in `src/a-d/Doji/Doji.StreamHub.cs` ✅
- [ ] **T107** [P] Implement Donchian StreamHub in `src/a-d/Donchian/Donchian.StreamHub.cs`
- [ ] **T108** [P] Implement Dpo StreamHub in `src/a-d/Dpo/Dpo.StreamHub.cs`
- [ ] **T109** [P] Implement Dynamic StreamHub in `src/a-d/Dynamic/Dynamic.StreamHub.cs`
- [ ] **T110** [P] Implement ElderRay StreamHub in `src/e-k/ElderRay/ElderRay.StreamHub.cs`
- [x] **T111** [P] Implement Ema StreamHub in `src/e-k/Ema/Ema.StreamHub.cs` ✅
- [x] **T112** [P] Implement Epma StreamHub in `src/e-k/Epma/Epma.StreamHub.cs` ✅
- [ ] **T113** [P] Implement Fcb StreamHub in `src/e-k/Fcb/Fcb.StreamHub.cs`
- [ ] **T114** [P] Implement FisherTransform StreamHub in `src/e-k/FisherTransform/FisherTransform.StreamHub.cs`
- [ ] **T115** [P] Implement ForceIndex StreamHub in `src/e-k/ForceIndex/ForceIndex.StreamHub.cs`
- [ ] **T116** [P] Implement Fractal StreamHub in `src/e-k/Fractal/Fractal.StreamHub.cs` ⚠️ **DEFERRED** (v2)
- [ ] **T117** [P] Implement Gator StreamHub in `src/e-k/Gator/Gator.StreamHub.cs`
- [ ] **T118** [P] Implement HeikinAshi StreamHub in `src/e-k/HeikinAshi/HeikinAshi.StreamHub.cs`
- [x] **T119** [P] Implement Hma StreamHub in `src/e-k/Hma/Hma.StreamHub.cs` ✅
- [ ] **T120** [P] Implement HtTrendline StreamHub in `src/e-k/HtTrendline/HtTrendline.StreamHub.cs` ⚠️ **DEFERRED** (v2)
- [ ] **T121** [P] Implement Hurst StreamHub in `src/e-k/Hurst/Hurst.StreamHub.cs` ⚠️ **DEFERRED** (v2)
- [ ] **T122** [P] Implement Ichimoku StreamHub in `src/e-k/Ichimoku/Ichimoku.StreamHub.cs` ⚠️ **DEFERRED** (v2)
- [x] **T123** [P] Implement Kama StreamHub in `src/e-k/Kama/Kama.StreamHub.cs` ✅
- [ ] **T124** [P] Implement Keltner StreamHub in `src/e-k/Keltner/Keltner.StreamHub.cs`
- [ ] **T125** [P] Implement Kvo StreamHub in `src/e-k/Kvo/Kvo.StreamHub.cs`
- [ ] **T126** [P] Implement MaEnvelopes StreamHub in `src/m-r/MaEnvelopes/MaEnvelopes.StreamHub.cs`
- [x] **T127** [P] Implement Macd StreamHub in `src/m-r/Macd/Macd.StreamHub.cs` ✅
- [x] **T128** [P] Implement Mama StreamHub in `src/m-r/Mama/Mama.StreamHub.cs` ✅
- [ ] **T129** [P] Implement Marubozu StreamHub in `src/m-r/Marubozu/Marubozu.StreamHub.cs`
- [ ] **T130** [P] Implement Mfi StreamHub in `src/m-r/Mfi/Mfi.StreamHub.cs`
- [x] **T131** [P] Implement Obv StreamHub in `src/m-r/Obv/Obv.StreamHub.cs` ✅
- [ ] **T132** [P] Implement ParabolicSar StreamHub in `src/m-r/ParabolicSar/ParabolicSar.StreamHub.cs`
- [ ] **T133** [P] Implement PivotPoints StreamHub in `src/m-r/PivotPoints/PivotPoints.StreamHub.cs`
- [ ] **T134** [P] Implement Pivots StreamHub in `src/m-r/Pivots/Pivots.StreamHub.cs`
- [ ] **T135** [P] Implement Pmo StreamHub in `src/m-r/Pmo/Pmo.StreamHub.cs`
- [x] **T136** [P] Implement Prs StreamHub in `src/m-r/Prs/Prs.StreamHub.cs` ✅
- [ ] **T137** [P] Implement Pvo StreamHub in `src/m-r/Pvo/Pvo.StreamHub.cs`
- [x] **T138** [P] Implement QuotePart StreamHub in `src/_common/QuotePart/QuotePart.StreamHub.cs` ✅
- [x] **T139** [P] Implement Renko StreamHub in `src/m-r/Renko/Renko.StreamHub.cs` ✅
- [ ] **T140** [P] Implement RenkoAtr StreamHub in `src/m-r/RenkoAtr/RenkoAtr.StreamHub.cs`
- [x] **T141** [P] Implement Roc StreamHub in `src/m-r/Roc/Roc.StreamHub.cs` ✅
- [ ] **T142** [P] Implement RocWb StreamHub in `src/m-r/RocWb/RocWb.StreamHub.cs`
- [ ] **T143** [P] Implement RollingPivots StreamHub in `src/m-r/RollingPivots/RollingPivots.StreamHub.cs`
- [x] **T144** [P] Implement Rsi StreamHub in `src/m-r/Rsi/Rsi.StreamHub.cs` ✅
- [ ] **T145** [P] Implement Slope StreamHub in `src/s-z/Slope/Slope.StreamHub.cs` ⚠️ **DEFERRED** (v2)
- [x] **T146** [P] Implement Sma StreamHub in `src/s-z/Sma/Sma.StreamHub.cs` ✅
- [ ] **T147** [P] Implement SmaAnalysis StreamHub in `src/s-z/SmaAnalysis/SmaAnalysis.StreamHub.cs`
- [ ] **T148** [P] Implement Smi StreamHub in `src/s-z/Smi/Smi.StreamHub.cs`
- [x] **T149** [P] Implement Smma StreamHub in `src/s-z/Smma/Smma.StreamHub.cs` ✅
- [ ] **T150** [P] Implement StarcBands StreamHub in `src/s-z/StarcBands/StarcBands.StreamHub.cs`
- [ ] **T151** [P] Implement Stc StreamHub in `src/s-z/Stc/Stc.StreamHub.cs`
- [ ] **T152** [P] Implement StdDev StreamHub in `src/s-z/StdDev/StdDev.StreamHub.cs`
- [ ] **T153** [P] Implement StdDevChannels StreamHub in `src/s-z/StdDevChannels/StdDevChannels.StreamHub.cs`
- [x] **T154** [P] Implement Stoch StreamHub in `src/s-z/Stoch/Stoch.StreamHub.cs` ✅
- [x] **T155** [P] Implement StochRsi StreamHub in `src/s-z/StochRsi/StochRsi.StreamHub.cs` ✅
- [ ] **T156** [P] Implement SuperTrend StreamHub in `src/s-z/SuperTrend/SuperTrend.StreamHub.cs`
- [x] **T157** [P] Implement T3 StreamHub in `src/s-z/T3/T3.StreamHub.cs` ✅
- [x] **T158** [P] Implement Tema StreamHub in `src/s-z/Tema/Tema.StreamHub.cs` ✅
- [x] **T159** [P] Implement Tr StreamHub in `src/s-z/Tr/Tr.StreamHub.cs` ✅
- [x] **T160** [P] Implement Trix StreamHub in `src/s-z/Trix/Trix.StreamHub.cs` ✅
- [ ] **T161** [P] Implement Tsi StreamHub in `src/s-z/Tsi/Tsi.StreamHub.cs`
- [ ] **T162** [P] Implement UlcerIndex StreamHub in `src/s-z/UlcerIndex/UlcerIndex.StreamHub.cs`
- [x] **T163** [P] Implement Ultimate StreamHub in `src/s-z/Ultimate/Ultimate.StreamHub.cs` ✅
- [ ] **T164** [P] Implement VolatilityStop StreamHub in `src/s-z/VolatilityStop/VolatilityStop.StreamHub.cs`
- [ ] **T165** [P] Implement Vortex StreamHub in `src/s-z/Vortex/Vortex.StreamHub.cs`
- [ ] **T166** [P] Implement Vwap StreamHub in `src/s-z/Vwap/Vwap.StreamHub.cs`
- [x] **T167** [P] Implement Vwma StreamHub in `src/s-z/Vwma/Vwma.StreamHub.cs` ✅
- [x] **T168** [P] Implement WilliamsR StreamHub in `src/s-z/WilliamsR/WilliamsR.StreamHub.cs` ✅
- [x] **T169** [P] Implement Wma StreamHub in `src/s-z/Wma/Wma.StreamHub.cs` ✅
- [ ] **T170** [P] Implement ZigZag StreamHub in `src/s-z/ZigZag/ZigZag.StreamHub.cs`

**StreamHub**: 45/85 complete, 40 remaining

**Checkpoint**: Phase 3 completion achieves 1:1:1 parity across all three implementation styles (Series, BufferList, StreamHub)

---

## Phase 4: Test Infrastructure & Quality Assurance

**Purpose**: Ensure comprehensive test coverage and performance validation

**Dependencies**: Phases 2 and 3 implementations

### StreamHub Test Interface Compliance

- [ ] **T175** [P] Audit all existing StreamHub test classes for proper test interface implementation according to updated guidelines in `.github/instructions/indicator-stream.instructions.md`
- [ ] **T176** Update StreamHub test classes that implement wrong interfaces (e.g., missing `ITestChainObserver` for chainable indicators)
- [ ] **T177** Add comprehensive rollback validation tests to all StreamHub test classes following canonical pattern from `Adx.StreamHub.Tests.cs`
  - Implement in appropriate observer test methods (QuoteObserver, ChainObserver, PairsObserver)
  - Use mutable `List<Quote>` (not static array)
  - Add all quotes, verify results match series exactly with strict ordering
  - Remove a single historical quote (not just the last)
  - Rebuild expected series with revised quote list
  - Assert exact count and strict ordering for both before and after removal
  - Never re-add the removed quote (revised series is new ground truth)
- [ ] **T178** [P] Verify all dual-stream indicators (Correlation, Beta, PRS) implement `ITestPairsObserver` interface correctly
- [ ] **T179** [P] Create validation script to check test interface compliance across all StreamHub tests

### Provider History Testing

- [ ] **T180** Add provider history (Insert/Remove) testing to QuoteObserver tests in AdxHub at `tests/indicators/a-d/Adx/Adx.StreamHub.Tests.cs`
  - Use `List<Quote>` for mutability
  - Remove by index and verify strict ordering and count
  - Never re-add removed quote
  - Reference `Adx.StreamHub.Tests.cs` as canonical example
- [ ] **T181** Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in AtrHub, CciHub, MacdHub, MamaHub, ObvHub, PrsHub
- [ ] **T182** Add provider history (Insert/Remove) testing to ChainObserver tests missing Insert/Remove operations in RsiHub, StochRsiHub
- [ ] **T183** Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in StochHub, VwmaHub, WilliamsRHub
- [ ] **T184** Add virtual ProviderHistoryTesting() method to StreamHubTestBase class in `tests/indicators/_base/StreamHubTestBase.cs`
- [ ] **T185** Update `indicator-stream.instructions.md` to require comprehensive rollback validation testing for all StreamHub indicators

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
- **Phase 2**: BufferList implementations (T001-T085) — Depends on Phase 1 completion
- **Phase 3**: StreamHub implementations (T086-T170) — Depends on Phase 1 completion (can proceed in parallel with Phase 2)
- **Phase 4**: Test infrastructure & quality assurance (T175-T185, Q001-Q006) — Depends on Phases 2 and 3 progress
- **Phase 5**: Documentation & polish (D001-D007) — Depends on Phases 2 and 3 implementations

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

**Implementation Coverage (1:1:1 Parity)**:
- **Total Series implementations**: 85 indicators (baseline)
- **Total BufferList implementations**: 66 complete, 19 remaining (T001-T085)
- **Total StreamHub implementations**: 45 complete, 40 remaining (T086-T170)
- **1:1:1 Target**: 85 BufferList + 85 StreamHub = 170 streaming implementations total
- **Current streaming coverage**: 111/170 = **65% complete**

**Task Breakdown**:
- **Phase 1**: 10 tasks (A001-A006, T171-T174) — 8 complete, 2 remaining
- **Phase 2**: 85 BufferList implementation tasks (T001-T085) — 66 complete, 19 remaining
- **Phase 3**: 85 StreamHub implementation tasks (T086-T170) — 45 complete, 40 remaining
- **Phase 4**: 17 test infrastructure tasks (T175-T185, Q001-Q006) — 0 complete, 17 remaining
- **Phase 5**: 7 documentation tasks (D001-D007) — 2 complete, 5 remaining
- **Total**: 204 tasks — 121 complete, 83 remaining

**Deferred to v2** (streaming incompatible): Fractal, HtTrendline, Hurst, Ichimoku, Slope

---
Last updated: October 13, 2025
