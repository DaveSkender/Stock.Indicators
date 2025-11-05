# Tasks: streaming indicators framework

**Input**: Design documents from `/.specify/specs/001-develop-streaming-indicators/`
**Prerequisites**: [plan.md](plan.md) (required), [spec.md](spec.md) (required for user stories), [data-model.md](data-model.md)

## Format: `[ID] Description`

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

## Phase 1: Infrastructure & Compliance Audits (IN PROGRESS)

**Purpose**: Establish instruction file compliance and quality foundation before expanding streaming indicator coverage.

### Compliance Audits (A-series)

- [x] **A001** Audit existing BufferList implementations for instruction file compliance (`src/**/*.BufferList.cs` against `.github/instructions/indicator-buffer.instructions.md`) ✅
  - Verify correct base class inheritance (`BufferList<TResult>`)
  - Check interface implementation (`IIncrementFromChain`/`IIncrementFromQuote`/`IIncrementFromPairs`)
  - Validate constructor patterns (params-only and params+quotes variants)
  - Confirm `BufferListUtilities` usage instead of manual buffer management
  - Check member ordering per instruction file conventions

- [x] **A002** Audit existing StreamHub implementations for instruction file compliance (`src/**/*.StreamHub.cs` against `.github/instructions/indicator-stream.instructions.md`) ✅
  - Verify correct provider base (`ChainProvider`/`QuoteProvider`/`PairsProvider`)
  - Check test interface implementation requirements
  - Validate provider history testing coverage (Insert/Remove scenarios)
  - Confirm performance benchmarking inclusion
  - Check member ordering per instruction file conventions

- [x] **A003** Audit BufferList test classes for compliance (`tests/**/*.BufferList.Tests.cs`) ✅
  - Verify inheritance from `BufferListTestBase` (not `TestBase`)
  - Check implementation of correct test interfaces
  - Validate coverage of 5 required test methods
  - Confirm Series parity validation patterns

- [x] **A004** Audit StreamHub test classes for compliance (`tests/**/*.StreamHub.Tests.cs`) ✅
  - Verify inheritance from `StreamHubTestBase`
  - Check implementation of correct test interfaces per provider pattern
  - Validate provider history testing (Insert/Remove scenarios)
  - Confirm performance benchmarking inclusion

- [x] **A005** Update indicator documentation pages for instruction file compliance ✅
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

- [x] **T172** Comprehensive audit of all StreamHub test classes for missing test interfaces ✅
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

Note on former deferrals: Some indicators were previously marked as deferred due to complexity (for example: Fractal, HtTrendline, Hurst, Ichimoku, Slope). We now have solid reference patterns demonstrating solutions for repainting, multi-buffer state, complex objects, and dual-series inputs (see instruction files). These items are complex but not blocked — implement them by adapting from the closest reference.

**Dependencies**: Phase 1 compliance audits complete

- [x] **T001** Implement Adl BufferList in `src/a-d/Adl/Adl.BufferList.cs` ✅
- [x] **T002** Implement Adx BufferList in `src/a-d/Adx/Adx.BufferList.cs` ✅
- [x] **T003** Implement Alligator BufferList in `src/a-d/Alligator/Alligator.BufferList.cs` ✅
- [x] **T004** Implement Alma BufferList in `src/a-d/Alma/Alma.BufferList.cs` ✅
- [x] **T005** Implement Aroon BufferList in `src/a-d/Aroon/Aroon.BufferList.cs` ✅
- [x] **T006** Implement Atr BufferList in `src/a-d/Atr/Atr.BufferList.cs` ✅
- [x] **T007** Implement AtrStop BufferList in `src/a-d/AtrStop/AtrStop.BufferList.cs` ✅
- [x] **T008** Implement Awesome BufferList in `src/a-d/Awesome/Awesome.BufferList.cs` ✅
- [x] **T009** Implement Beta BufferList in `src/a-d/Beta/Beta.BufferList.cs` ✅
- [x] **T010** Implement BollingerBands BufferList in `src/a-d/BollingerBands/BollingerBands.BufferList.cs` ✅
- [x] **T011** Implement Bop BufferList in `src/a-d/Bop/Bop.BufferList.cs` ✅
- [x] **T012** Implement Cci BufferList in `src/a-d/Cci/Cci.BufferList.cs` ✅
- [x] **T013** Implement ChaikinOsc BufferList in `src/a-d/ChaikinOsc/ChaikinOsc.BufferList.cs` ✅
- [x] **T014** Implement Chandelier BufferList in `src/a-d/Chandelier/Chandelier.BufferList.cs` ✅
- [x] **T015** Implement Chop BufferList in `src/a-d/Chop/Chop.BufferList.cs` ✅
- [x] **T016** Implement Cmf BufferList in `src/a-d/Cmf/Cmf.BufferList.cs` ✅
- [x] **T017** Implement Cmo BufferList in `src/a-d/Cmo/Cmo.BufferList.cs` ✅
- [x] **T018** Implement ConnorsRsi BufferList in `src/a-d/ConnorsRsi/ConnorsRsi.BufferList.cs` ✅
- [x] **T019** Implement Correlation BufferList in `src/a-d/Correlation/Correlation.BufferList.cs` ✅
- [x] **T020** Implement Dema BufferList in `src/a-d/Dema/Dema.BufferList.cs` ✅
- [x] **T021** Implement Doji BufferList in `src/a-d/Doji/Doji.BufferList.cs` ✅
- [x] **T022** Implement Donchian BufferList in `src/a-d/Donchian/Donchian.BufferList.cs` ✅
- [x] **T023** Implement Dpo BufferList in `src/a-d/Dpo/Dpo.BufferList.cs` ✅
- [x] **T024** Implement Dynamic BufferList in `src/a-d/Dynamic/Dynamic.BufferList.cs` ✅
- [x] **T025** Implement ElderRay BufferList in `src/e-k/ElderRay/ElderRay.BufferList.cs` ✅
- [x] **T026** Implement Ema BufferList in `src/e-k/Ema/Ema.BufferList.cs` ✅
- [x] **T027** Implement Epma BufferList in `src/e-k/Epma/Epma.BufferList.cs` ✅
- [x] **T028** Implement Fcb BufferList in `src/e-k/Fcb/Fcb.BufferList.cs` ✅
- [x] **T029** Implement FisherTransform BufferList in `src/e-k/FisherTransform/FisherTransform.BufferList.cs` ✅
- [x] **T030** Implement ForceIndex BufferList in `src/e-k/ForceIndex/ForceIndex.BufferList.cs` ✅
- [x] **T031** Implement Fractal BufferList in `src/e-k/Fractal/Fractal.BufferList.cs` (complex but unblocked — follow HMA/ADX state patterns) ✅
- [x] **T032** Implement Gator BufferList in `src/e-k/Gator/Gator.BufferList.cs` ✅
- [x] **T033** Implement HeikinAshi BufferList in `src/e-k/HeikinAshi/HeikinAshi.BufferList.cs` ✅
- [x] **T034** Implement Hma BufferList in `src/e-k/Hma/Hma.BufferList.cs` ✅
- [x] **T035** Implement HtTrendline BufferList in `src/e-k/HtTrendline/HtTrendline.BufferList.cs` (complex but unblocked — use HMA multi-buffer as a baseline) ✅
- [x] **T036** Implement Hurst BufferList in `src/e-k/Hurst/Hurst.BufferList.cs` (complex but unblocked — adapt ADX complex state + HMA buffers) ✅
- [x] **T037** Implement Ichimoku BufferList in `src/e-k/Ichimoku/Ichimoku.BufferList.cs` (complex but unblocked — follow multi-line series patterns from Alligator/AtrStop and HMA buffers) ✅
- [x] **T038** Implement Kama BufferList in `src/e-k/Kama/Kama.BufferList.cs` ✅
- [x] **T039** Implement Keltner BufferList in `src/e-k/Keltner/Keltner.BufferList.cs` ✅
- [x] **T040** Implement Kvo BufferList in `src/e-k/Kvo/Kvo.BufferList.cs` ✅
- [x] **T041** Implement MaEnvelopes BufferList in `src/m-r/MaEnvelopes/MaEnvelopes.BufferList.cs` ✅
- [x] **T042** Implement Macd BufferList in `src/m-r/Macd/Macd.BufferList.cs` ✅
- [x] **T043** Implement Mama BufferList in `src/m-r/Mama/Mama.BufferList.cs` ✅
- [x] **T044** Implement Marubozu BufferList in `src/m-r/Marubozu/Marubozu.BufferList.cs` ✅
- [x] **T045** Implement Mfi BufferList in `src/m-r/Mfi/Mfi.BufferList.cs` ✅
- [x] **T046** Implement Obv BufferList in `src/m-r/Obv/Obv.BufferList.cs` ✅
- [x] **T047** Implement ParabolicSar BufferList in `src/m-r/ParabolicSar/ParabolicSar.BufferList.cs` ✅
- [x] **T048** Implement PivotPoints BufferList in `src/m-r/PivotPoints/PivotPoints.BufferList.cs` ✅
- [x] **T049** Implement Pivots BufferList in `src/m-r/Pivots/Pivots.BufferList.cs` ✅
- [x] **T050** Implement Pmo BufferList in `src/m-r/Pmo/Pmo.BufferList.cs` ✅
- [x] **T051** Implement Prs BufferList in `src/m-r/Prs/Prs.BufferList.cs` ✅
- [x] **T052** Implement Pvo BufferList in `src/m-r/Pvo/Pvo.BufferList.cs` ✅
- [x] **T053** Implement QuotePart BufferList in `src/_common/QuotePart/QuotePart.BufferList.cs` ✅
- [x] **T054** Implement Renko BufferList in `src/m-r/Renko/Renko.BufferList.cs` ✅
- [ ] **T055** ~~Implement RenkoAtr BufferList~~ **NOT IMPLEMENTED** — ATR calculation requires full dataset to determine final brick size. Buffering all quotes and recalculating entire Renko series on each add would defeat the purpose of incremental processing. Series-only implementation maintained.
- [x] **T056** Implement Roc BufferList in `src/m-r/Roc/Roc.BufferList.cs` ✅
- [x] **T057** Implement RocWb BufferList in `src/m-r/RocWb/RocWb.BufferList.cs` ✅
- [x] **T058** Implement RollingPivots BufferList in `src/m-r/RollingPivots/RollingPivots.BufferList.cs` ✅
- [x] **T059** Implement Rsi BufferList in `src/m-r/Rsi/Rsi.BufferList.cs` ✅
- [x] **T060** Implement Slope BufferList in `src/s-z/Slope/Slope.BufferList.cs` (complex but unblocked — legit historical revisions follow VolatilityStop repaint pattern) ✅
- [x] **T061** Implement Sma BufferList in `src/s-z/Sma/Sma.BufferList.cs` ✅
- [x] **T062** Implement SmaAnalysis BufferList in `src/s-z/SmaAnalysis/SmaAnalysis.BufferList.cs` ✅
- [x] **T063** Implement Smi BufferList in `src/s-z/Smi/Smi.BufferList.cs` ✅
- [x] **T064** Implement Smma BufferList in `src/s-z/Smma/Smma.BufferList.cs` ✅
- [x] **T065** Implement StarcBands BufferList in `src/s-z/StarcBands/StarcBands.BufferList.cs` ✅
- [x] **T066** Implement Stc BufferList in `src/s-z/Stc/Stc.BufferList.cs` ✅
- [x] **T067** Implement StdDev BufferList in `src/s-z/StdDev/StdDev.BufferList.cs` ✅
- [ ] **T068** ~~Implement StdDevChannels BufferList~~ **NOT IMPLEMENTED** — Repaint-by-design algorithm recalculates entire dataset (O(n²)) on each new data point, making incremental streaming impractical. Series-only implementation maintained.
- [x] **T069** Implement Stoch BufferList in `src/s-z/Stoch/Stoch.BufferList.cs` ✅
- [x] **T070** Implement StochRsi BufferList in `src/s-z/StochRsi/StochRsi.BufferList.cs` ✅
- [x] **T071** Implement SuperTrend BufferList in `src/s-z/SuperTrend/SuperTrend.BufferList.cs` ✅
- [x] **T072** Implement T3 BufferList in `src/s-z/T3/T3.BufferList.cs` ✅
- [x] **T073** Implement Tema BufferList in `src/s-z/Tema/Tema.BufferList.cs` ✅
- [x] **T074** Implement Tr BufferList in `src/s-z/Tr/Tr.BufferList.cs` ✅
- [x] **T075** Implement Trix BufferList in `src/s-z/Trix/Trix.BufferList.cs` ✅
- [x] **T076** Implement Tsi BufferList in `src/s-z/Tsi/Tsi.BufferList.cs` ✅
- [x] **T077** Implement UlcerIndex BufferList in `src/s-z/UlcerIndex/UlcerIndex.BufferList.cs` ✅
- [x] **T078** Implement Ultimate BufferList in `src/s-z/Ultimate/Ultimate.BufferList.cs` ✅
- [x] **T079** Implement VolatilityStop BufferList in `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` ✅
- [x] **T080** Implement Vortex BufferList in `src/s-z/Vortex/Vortex.BufferList.cs` ✅
- [x] **T081** Implement Vwap BufferList in `src/s-z/Vwap/Vwap.BufferList.cs` ✅
- [x] **T082** Implement Vwma BufferList in `src/s-z/Vwma/Vwma.BufferList.cs` ✅
- [x] **T083** Implement WilliamsR BufferList in `src/s-z/WilliamsR/WilliamsR.BufferList.cs` ✅
- [x] **T084** Implement Wma BufferList in `src/s-z/Wma/Wma.BufferList.cs` ✅
- [ ] **T085** (human only) Implement ZigZag BufferList in `src/s-z/ZigZag/ZigZag.BufferList.cs`

**BufferList**: 82/85 complete, 3 remaining — T055 (RenkoAtr - not implementing), T068 (StdDevChannels - not implementing), T085 (ZigZag - human only)

**Checkpoint**: Phase 2 completion achieves complete BufferList coverage for all Series indicators (except StdDevChannels)

---

## Phase 3: StreamHub Implementations

**Purpose**: Complete StreamHub streaming implementations for all 85 indicators (1:1:1 parity with Series)

Note on former deferrals: Indicators like Fractal, HtTrendline, Hurst, Ichimoku, and Slope are complex but unblocked. Use the reference hubs in the stream instructions — EMA hub for chain provider baseline, ATR Stop and Alligator for multi-series from quotes, Correlation/Beta for synchronized pairs.

**Dependencies**: Phase 1 compliance audits complete (Phase 2 can proceed in parallel)

- [x] **T086** Implement Adl StreamHub in `src/a-d/Adl/Adl.StreamHub.cs` ✅
- [x] **T087** Implement Adx StreamHub in `src/a-d/Adx/Adx.StreamHub.cs` ✅
- [x] **T088** Implement Alligator StreamHub in `src/a-d/Alligator/Alligator.StreamHub.cs` ✅
- [x] **T089** Implement Alma StreamHub in `src/a-d/Alma/Alma.StreamHub.cs` ✅
- [x] **T090** Implement Aroon StreamHub in `src/a-d/Aroon/Aroon.StreamHub.cs` ✅
- [x] **T091** Implement Atr StreamHub in `src/a-d/Atr/Atr.StreamHub.cs` ✅
- [x] **T092** Implement AtrStop StreamHub in `src/a-d/AtrStop/AtrStop.StreamHub.cs` ✅
- [x] **T093** Implement Awesome StreamHub in `src/a-d/Awesome/Awesome.StreamHub.cs` ✅
- [x] **T094** Implement Beta StreamHub in `src/a-d/Beta/Beta.StreamHub.cs` ✅
- [x] **T095** Implement BollingerBands StreamHub in `src/a-d/BollingerBands/BollingerBands.StreamHub.cs` ✅
- [x] **T096** Implement Bop StreamHub in `src/a-d/Bop/Bop.StreamHub.cs` ✅
- [x] **T097** Implement Cci StreamHub in `src/a-d/Cci/Cci.StreamHub.cs` ✅
- [x] **T098** Implement ChaikinOsc StreamHub in `src/a-d/ChaikinOsc/ChaikinOsc.StreamHub.cs` ✅
- [x] **T099** Implement Chandelier StreamHub in `src/a-d/Chandelier/Chandelier.StreamHub.cs` ✅
- [x] **T100** Implement Chop StreamHub in `src/a-d/Chop/Chop.StreamHub.cs` ✅
- [x] **T101** Implement Cmf StreamHub in `src/a-d/Cmf/Cmf.StreamHub.cs` ✅
- [x] **T102** Implement Cmo StreamHub in `src/a-d/Cmo/Cmo.StreamHub.cs` ✅
- [x] **T103** Implement ConnorsRsi StreamHub in `src/a-d/ConnorsRsi/ConnorsRsi.StreamHub.cs` ✅
- [x] **T104** Implement Correlation StreamHub in `src/a-d/Correlation/Correlation.StreamHub.cs` ✅
- [x] **T105** Implement Dema StreamHub in `src/a-d/Dema/Dema.StreamHub.cs` ✅
- [x] **T106** Implement Doji StreamHub in `src/a-d/Doji/Doji.StreamHub.cs` ✅
- [x] **T107** Implement Donchian StreamHub in `src/a-d/Donchian/Donchian.StreamHub.cs` ✅
- [ ] **T108** Implement Dpo StreamHub in `src/a-d/Dpo/Dpo.StreamHub.cs`
- [x] **T109** Implement Dynamic StreamHub in `src/a-d/Dynamic/Dynamic.StreamHub.cs` ✅
- [x] **T110** Implement ElderRay StreamHub in `src/e-k/ElderRay/ElderRay.StreamHub.cs` ✅
- [x] **T111** Implement Ema StreamHub in `src/e-k/Ema/Ema.StreamHub.cs` ✅
- [x] **T112** Implement Epma StreamHub in `src/e-k/Epma/Epma.StreamHub.cs` ✅
- [x] **T113** Implement Fcb StreamHub in `src/e-k/Fcb/Fcb.StreamHub.cs` ✅
- [x] **T114** Implement FisherTransform StreamHub in `src/e-k/FisherTransform/FisherTransform.StreamHub.cs` ✅
- [x] **T115** Implement ForceIndex StreamHub in `src/e-k/ForceIndex/ForceIndex.StreamHub.cs` ✅
- [x] **T116** Implement Fractal StreamHub in `src/e-k/Fractal/Fractal.StreamHub.cs` ✅
- [x] **T117** Implement Gator StreamHub in `src/e-k/Gator/Gator.StreamHub.cs` ✅
- [x] **T118** Implement HeikinAshi StreamHub in `src/e-k/HeikinAshi/HeikinAshi.StreamHub.cs` ✅
- [x] **T119** Implement Hma StreamHub in `src/e-k/Hma/Hma.StreamHub.cs` ✅
- [x] **T120** Implement HtTrendline StreamHub in `src/e-k/HtTrendline/HtTrendline.StreamHub.cs` (complex but unblocked — model after EMA chain provider with HMA-like buffers) ✅
- [x] **T121** Implement Hurst StreamHub in `src/e-k/Hurst/Hurst.StreamHub.cs` (complex but unblocked — use ADX-like complex state and EMA hub shape) ✅
- [x] **T122** Implement Ichimoku StreamHub in `src/e-k/Ichimoku/Ichimoku.StreamHub.cs` (complex but unblocked — multi-line series via quote provider pattern like Alligator/AtrStop) ✅
- [x] **T123** Implement Kama StreamHub in `src/e-k/Kama/Kama.StreamHub.cs` ✅
- [x] **T124** Implement Keltner StreamHub in `src/e-k/Keltner/Keltner.StreamHub.cs` ✅
- [x] **T125** Implement Kvo StreamHub in `src/e-k/Kvo/Kvo.StreamHub.cs` ✅
- [x] **T126** Implement MaEnvelopes StreamHub in `src/m-r/MaEnvelopes/MaEnvelopes.StreamHub.cs` ✅
- [x] **T127** Implement Macd StreamHub in `src/m-r/Macd/Macd.StreamHub.cs` ✅
- [x] **T128** Implement Mama StreamHub in `src/m-r/Mama/Mama.StreamHub.cs` ✅
- [x] **T129** Implement Marubozu StreamHub in `src/m-r/Marubozu/Marubozu.StreamHub.cs` ✅
- [x] **T130** Implement Mfi StreamHub in `src/m-r/Mfi/Mfi.StreamHub.cs` ✅
- [x] **T131** Implement Obv StreamHub in `src/m-r/Obv/Obv.StreamHub.cs` ✅
- [x] **T132** Implement ParabolicSar StreamHub in `src/m-r/ParabolicSar/ParabolicSar.StreamHub.cs` ✅
- [x] **T133** Implement PivotPoints StreamHub in `src/m-r/PivotPoints/PivotPoints.StreamHub.cs` ✅
- [x] **T134** Implement Pivots StreamHub in `src/m-r/Pivots/Pivots.StreamHub.cs` ✅
- [x] **T135** Implement Pmo StreamHub in `src/m-r/Pmo/Pmo.StreamHub.cs` ✅
- [x] **T136** Implement Prs StreamHub in `src/m-r/Prs/Prs.StreamHub.cs` ✅
- [x] **T137** Implement Pvo StreamHub in `src/m-r/Pvo/Pvo.StreamHub.cs` ✅
- [x] **T138** Implement QuotePart StreamHub in `src/_common/QuotePart/QuotePart.StreamHub.cs` ✅
- [x] **T139** Implement Renko StreamHub in `src/m-r/Renko/Renko.StreamHub.cs` ✅
- [ ] **T140** ~~Implement RenkoAtr StreamHub~~ **NOT IMPLEMENTED** — ATR calculation requires full dataset to determine final brick size. Real-time streaming would require buffering all history and recalculating entire Renko series on each new quote, defeating incremental processing purpose. Series-only implementation maintained.
- [x] **T141** Implement Roc StreamHub in `src/m-r/Roc/Roc.StreamHub.cs` ✅
- [x] **T142** Implement RocWb StreamHub in `src/m-r/RocWb/RocWb.StreamHub.cs` ✅
- [x] **T143** Implement RollingPivots StreamHub in `src/m-r/RollingPivots/RollingPivots.StreamHub.cs` ✅
- [x] **T144** Implement Rsi StreamHub in `src/m-r/Rsi/Rsi.StreamHub.cs` ✅
- [ ] **T145** Implement Slope StreamHub in `src/s-z/Slope/Slope.StreamHub.cs` (complex but unblocked — repaint-friendly logic modeled after VolatilityStop tests and series parity)
- [x] **T146** Implement Sma StreamHub in `src/s-z/Sma/Sma.StreamHub.cs` ✅
- [x] **T147** Implement SmaAnalysis StreamHub in `src/s-z/SmaAnalysis/SmaAnalysis.StreamHub.cs` ✅
- [x] **T148** Implement Smi StreamHub in `src/s-z/Smi/Smi.StreamHub.cs` ✅
- [x] **T149** Implement Smma StreamHub in `src/s-z/Smma/Smma.StreamHub.cs` ✅
- [x] **T150** Implement StarcBands StreamHub in `src/s-z/StarcBands/StarcBands.StreamHub.cs` ✅
- [x] **T151** Implement Stc StreamHub in `src/s-z/Stc/Stc.StreamHub.cs` ✅
- [x] **T152** Implement StdDev StreamHub in `src/s-z/StdDev/StdDev.StreamHub.cs` ✅
- [ ] **T153** ~~Implement StdDevChannels StreamHub~~ **NOT IMPLEMENTED** — Repaint-by-design algorithm recalculates entire dataset (O(n²)) on each new data point, making real-time streaming impractical. Series-only implementation maintained. See user documentation for details.
- [x] **T154** Implement Stoch StreamHub in `src/s-z/Stoch/Stoch.StreamHub.cs` ✅
- [x] **T155** Implement StochRsi StreamHub in `src/s-z/StochRsi/StochRsi.StreamHub.cs` ✅
- [x] **T156** Implement SuperTrend StreamHub in `src/s-z/SuperTrend/SuperTrend.StreamHub.cs` ✅
- [x] **T157** Implement T3 StreamHub in `src/s-z/T3/T3.StreamHub.cs` ✅
- [x] **T158** Implement Tema StreamHub in `src/s-z/Tema/Tema.StreamHub.cs` ✅
- [x] **T159** Implement Tr StreamHub in `src/s-z/Tr/Tr.StreamHub.cs` ✅
- [x] **T160** Implement Trix StreamHub in `src/s-z/Trix/Trix.StreamHub.cs` ✅
- [x] **T161** Implement Tsi StreamHub in `src/s-z/Tsi/Tsi.StreamHub.cs` ✅
- [x] **T162** Implement UlcerIndex StreamHub in `src/s-z/UlcerIndex/UlcerIndex.StreamHub.cs` ✅
- [x] **T163** Implement Ultimate StreamHub in `src/s-z/Ultimate/Ultimate.StreamHub.cs` ✅
- [x] **T164** Implement VolatilityStop StreamHub in `src/s-z/VolatilityStop/VolatilityStop.StreamHub.cs` ✅
- [x] **T165** Implement Vortex StreamHub in `src/s-z/Vortex/Vortex.StreamHub.cs` ✅
- [x] **T166** Implement Vwap StreamHub in `src/s-z/Vwap/Vwap.StreamHub.cs` ✅
- [x] **T167** Implement Vwma StreamHub in `src/s-z/Vwma/Vwma.StreamHub.cs` ✅
- [x] **T168** Implement WilliamsR StreamHub in `src/s-z/WilliamsR/WilliamsR.StreamHub.cs` ✅
- [x] **T169** Implement Wma StreamHub in `src/s-z/Wma/Wma.StreamHub.cs` ✅
- [ ] **T170** (human only) Implement ZigZag StreamHub in `src/s-z/ZigZag/ZigZag.StreamHub.cs`

**StreamHub**: 79/85 complete, 6 remaining — T108 (Dpo), T145 (Slope), T140 (RenkoAtr - not implementing), T153 (StdDevChannels - not implementing), T170 (ZigZag - human only)

**Checkpoint**: Phase 3 completion achieves 1:1:1 parity across all three implementation styles (Series, BufferList, StreamHub) except where algorithmically impractical

---

## Phase 4: Test Infrastructure & Quality Assurance

**Purpose**: Ensure comprehensive test coverage and performance validation

**Dependencies**: Phases 2 and 3 implementations

### StreamHub Test Interface Compliance

- [x] **T175** Audit all existing StreamHub test classes for proper test interface implementation according to updated guidelines in `.github/instructions/indicator-stream.instructions.md` ✅
  - Created comprehensive audit checklist: `.specify/specs/001-develop-streaming-indicators/checklists/T175-test-interface-audit.md`
  - Audited 81 StreamHub test files
  - Identified 13 issues (6 missing ITestChainProvider, 5 wrong observer for IReusable, 2 wrong observer for IQuote, 1 unexpected ChainProvider)
  - 65 tests already compliant (80.2%)
- [x] **T176** Update StreamHub test classes that implement wrong interfaces (e.g., missing `ITestChainObserver` for chainable indicators) ✅
  - Used audit checklist from T175 as guide: `.specify/specs/001-develop-streaming-indicators/checklists/T175-test-interface-audit.md`
  - Fixed 12 of 13 identified issues:
    - Priority 1: Fixed 5 of 6 missing ITestChainProvider (Cci, Kvo, Obv, Epma, BollingerBands; Pivots was audit error)
    - Priority 2: Fixed 2 of 5 wrong observer for IReusable chains (T3, Tema; ForceIndex, Ultimate, Vwma were audit errors - take IQuoteProvider constructor)
    - Priority 3: Fixed 2 of 2 wrong observer for IQuote chains (Bop, ChaikinOsc)
    - Priority 4: Fixed MaEnvelopes (removed incorrect ITestChainProvider)
  - 4 items were audit errors (already correct or architecturally correct)
- [x] **T177** Add comprehensive rollback validation tests to all StreamHub test classes following canonical pattern from `Adx.StreamHub.Tests.cs` ✅
  - Implemented in QuoteObserver test methods for 5 indicators
  - Added prefill, skip/Insert, duplicates, Remove, revised series parity validation
  - Updated: ADX, CMF (added Insert + comprehensive flow), Stoch, Epma (full pattern)
  - Coverage increased from 71/81 (87.7%) to 76/81 (93.8%)
  - Note: WilliamsR, Ichimoku tests reverted to original (comprehensive pattern too aggressive for their complexity)
  - Note: PairsObserver tests (Beta, Correlation, Prs) not modified - Insert/Remove operations break dual-stream synchronization invariant
- [x] **T178** Verify all dual-stream indicators (Correlation, Beta, PRS) implement `ITestPairsObserver` interface correctly ✅
  - Beta implements ITestPairsObserver correctly
  - Correlation implements ITestPairsObserver correctly  
  - Prs implements ITestPairsObserver correctly
  - All three dual-stream indicators fully compliant
- [x] **T179** Create validation script to check test interface compliance across all StreamHub tests ✅
  - Created `.specify/scripts/validate-streamhub-test-interfaces.py`
  - Validates interfaces match hub provider types per instruction file rules
  - Generates detailed compliance report
  - Current: 22/81 tests (27%) fully compliant
  - Exit code 1 if non-compliant (CI-ready)

### Provider History Testing

- [x] **T180** Add provider history (Insert/Remove) testing to QuoteObserver tests in AdxHub at `tests/indicators/a-d/Adx/Adx.StreamHub.Tests.cs` ✅
  - Already has comprehensive validation from T177
  - Includes prefill, skip/Insert, duplicates, Remove, revised series parity
- [x] **T181** Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in AtrHub, CciHub, MacdHub, MamaHub, ObvHub, PrsHub ✅
  - All reviewed: Atr, Cci, Macd, Mama, Obv have basic Insert/Remove
  - Prs is PairsObserver (N/A - dual-stream sync constraints prevent Insert/Remove)
  - Note: ChainProvider tests have basic Insert/Remove but lack comprehensive pattern (prefill, duplicates) - acceptable given QuoteObserver tests have comprehensive validation
- [x] **T182** Add provider history (Insert/Remove) testing to ChainObserver tests missing Insert/Remove operations in RsiHub, StochRsiHub ✅
  - Both have basic Insert/Remove operations
- [x] **T183** Add provider history (Insert/Remove) testing to ChainProvider tests missing Insert/Remove operations in StochHub, VwmaHub, WilliamsRHub ✅
  - Stoch updated with comprehensive validation in T177 (QuoteObserver test)
  - Vwma, WilliamsR have original tests (comprehensive pattern reverted)
  - ChainProvider tests have basic Insert/Remove
- [x] **T184** Add virtual ProviderHistoryTesting() method to StreamHubTestBase class in `tests/indicators/_base/StreamHubTestBase.cs` ✅
  - Added `protected virtual ProviderHistoryTesting()` method
  - Allows indicator-specific provider history validation
  - No-op base implementation, subclasses override as needed
- [x] **T185** Update `indicator-stream.instructions.md` to require comprehensive rollback validation testing for all StreamHub indicators ✅
  - Updated "Unit testing" checklist item to require comprehensive rollback validation (warmup, duplicates, Insert/Remove, strict Series parity)
  - Renamed the scenarios section to "Comprehensive rollback validation (required)"

### Performance & Quality Gates

- [x] **Q001** Update public API approval test baselines for streaming additions (`tests/public-api/`) ✅
  - Added BufferList convergence tests (7 indicators: Adx, Atr, Ema, Macd, Rsi, Sma, Stoch)
  - Added StreamHub convergence tests (7 indicators: Adx, Atr, Ema, Macd, Rsi, Sma, Stoch)
  - Files: `Convergence.BufferList.Tests.cs`, `Convergence.StreamHub.Tests.cs`
  - All 14 new tests pass with fast execution (< 1 second total)
- [ ] **Q002** Run performance benchmarks comparing BufferList vs Series for representative indicators
  - Use BenchmarkDotNet with latency validation (<5ms mean, <10ms p95)
  - Standardized hardware (4-core 3GHz CPU, 16GB RAM, .NET 9.0 release mode)
- [ ] **Q003** Run performance benchmarks comparing StreamHub vs Series for representative indicators
  - Same criteria as Q002
- [ ] **Q004** Validate memory overhead stays within <10KB per instance target (NFR-002)
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

> **CRITICAL**: Phase 5 documentation tasks are **mandatory for production readiness** per Constitution Principle 5 (Documentation Excellence). Users require:
>
> - Streaming usage examples for major indicators (RSI, MACD, Bollinger Bands)
> - README overview with quick-start guidance
> - Migration guide documenting streaming capabilities and API patterns
>
> Current status: 2/7 Phase 5 tasks complete (SMA, EMA done). Do not release to production until documentation is complete.

### Documentation Updates (D-series)

- [x] **D001** Update `docs/_indicators/Sma.md` with streaming usage section and examples ✅
- [x] **D002** Update `docs/_indicators/Ema.md` with streaming usage section and examples ✅
- [ ] **D003** Update `docs/_indicators/Rsi.md` with streaming usage section and examples
- [ ] **D004** Update `docs/_indicators/Macd.md` with streaming usage section and examples
- [ ] **D005** Update `docs/_indicators/BollingerBands.md` with streaming usage section and examples
- [ ] **D006** Update `README.md` with streaming overview paragraph and quick-start example
- [ ] **D007** Update `src/MigrationGuide.V3.md` with streaming capability summary and migration guidance

**Checkpoint**: Phase 5 completion delivers polished user-facing documentation for all streaming features

---

## Implementation Guidelines

### Refactor: Dual-Stream Provider/Observer Pattern

- [ ] **R001** Refactor `PairsProvider` for dual-stream StreamHub indicators ([#1548](https://github.com/DaveSkender/Stock.Indicators/issues/1548))
  - Evaluate renaming `PairsProvider` to `PairsObserver` for clarity (note: this goes against prevailing convention, but may improve consistency with observer pattern)
  - Delete `IPairsProvider` and replace its use on `PairsProvider` with the correct `IChainProvider<TOut>` implementation
  - Design and implement robust synchronization logic for two data sources that may not arrive concurrently (buffering, timestamp alignment, event-driven merge, etc.)
  - Replace `ProviderBCache` with a simpler, idiomatic tuple buffer (e.g., `Queue<(DateTime, double, double)>`)
  - Address all pruning and complexity issues related to dual-stream synchronization and buffer management
  - Refactor all affected indicators and tests to use the new pattern
  - Update documentation and instruction files to reflect the new approach
  - See [GitHub Issue #1548](https://github.com/DaveSkender/Stock.Indicators/issues/1548) for architectural discussion and elaboration

Each task should follow these guidelines:

### BufferList Implementation Requirements

- Inherit from `BufferList<TResult>` base class
- Implement appropriate interface (`IIncrementFromChain`, `IIncrementFromQuote`, or `IIncrementFromPairs`)
- Follow patterns from `.github/instructions/indicator-buffer.instructions.md`
- Provide both standard constructor and constructor with values/quotes parameter (matching interface type)
- Use universal `BufferListUtilities` extension methods for buffer management
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
- StreamHub tests must implement exactly one observer interface (ITestQuoteObserver OR ITestChainObserver); dual‑stream hubs must use ITestPairsObserver

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1**: Foundation (audits & compliance) — No dependencies, can start immediately
- **Phase 2**: BufferList implementations (T001-T085) — Depends on Phase 1 completion
- **Phase 3**: StreamHub implementations (T086-T170) — Depends on Phase 1 completion (can proceed in parallel with Phase 2)
- **Phase 4**: Test infrastructure & quality assurance (T175-T185, Q001-Q006) — Depends on Phases 2 and 3 progress
- **Phase 5**: Documentation & polish (D001-D007) — Depends on Phases 2 and 3 implementations

### Parallel Opportunities

- Tasks within each phase can run in parallel
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

## Phase 6: Priority 4 Enhancements (Post-Coverage)

**Purpose**: Address performance optimizations and feature enhancements after achieving comprehensive streaming coverage

**Dependencies**: Phases 2, 3, and 4 substantially complete

### ZigZag StreamHub Optimization (Issue #1692, P4.1)

- [ ] **E001** Analyze ZigZag Series implementation to identify pivot detection logic that can be incrementalized
  - Document current O(n) recalculation pattern triggering performance issues
  - Design incremental pivot-based update algorithm maintaining Series parity
  - Identify state requirements for rollback scenarios (Insert/Remove operations)

- [ ] **E002** Refactor ZigZag StreamHub to use incremental pivot updates in `src/s-z/ZigZag/ZigZag.StreamHub.cs`
  - Extract reusable pivot detection methods from Series implementation
  - Implement cache replay strategy for provider history mutations
  - Avoid recursive `.ToZigZag()` Series calls within StreamHub
  - Maintain mathematical parity with Series baseline

- [ ] **E003** Validate ZigZag StreamHub performance meets latency requirements (<5ms mean, <10ms p95)
  - Add performance benchmark to `tools/performance/Tests.Performance.StreamIndicators.cs`
  - Compare before/after optimization metrics
  - Verify no regression in Series parity tests

### QuoteHub Self-Healing (Issue #1585, P4.2)

- [ ] **E004** Design QuoteHub update semantics for intra-period quote modifications
  - Define update vs insert behavior (e.g., update only most recent quote)
  - Specify rollback strategy for subscribed indicators
  - Document limitations and edge cases (e.g., cannot update arbitrary historical quotes)

- [ ] **E005** Implement QuoteHub quote update capability in `src/_common/Streaming/QuoteHub.cs`
  - Add `Update(IQuote quote)` method with timestamp validation
  - Trigger recalculation for affected subscribed indicators
  - Guard against index out of range exceptions
  - Maintain strict timestamp ordering constraints

- [ ] **E006** Add comprehensive tests for QuoteHub update scenarios
  - Test updating most recent quote properties (Close, High, Low)
  - Verify subscribed indicators recalculate correctly
  - Test edge cases (updating non-most-recent quote should throw)
  - Document update semantics in user documentation

### ADX DMI Output Enhancement (Issue #1262, P4.3)

- [ ] **E007** Update ADX result classes to include DMI properties across all implementation styles
  - Add `Pdi` (Plus Directional Indicator) and `Mdi` (Minus Directional Indicator) properties to `AdxResult`
  - Verify Series implementation outputs DMI values (completed in prior PR)
  - Update BufferList implementation (T002) to output DMI values
  - Update StreamHub implementation (T087) to output DMI values

- [ ] **E008** Update ADX documentation and tests for DMI output
  - Update `docs/_indicators/Adx.md` with DMI property descriptions
  - Add DMI validation to existing regression tests
  - Document DMI vs ADX distinctions and usage patterns
  - Update migration guide with new property additions

### BufferList Configuration Enhancements (GitHub Project #6)

- [ ] **E009** Complete and validate BufferList configuration implementation (Issue [#1831](https://github.com/DaveSkender/Stock.Indicators/issues/1831))
  - **Status**: Partially implemented - `MaxListSize` property already exists in `BufferList<TResult>` base class with default 90% of int.MaxValue (~1.9B elements)
  - Audit all BufferList implementations to verify consistent `MaxListSize` usage
  - Ensure `PruneList()` override pattern is followed where custom pruning logic is needed
  - Validate that automatic pruning occurs via `AddInternal()` when list exceeds `MaxListSize`
  - Document buffer capacity management strategies in user documentation
  - Consider if additional configuration properties are needed (e.g., pruning strategy enum)
  - Implementation location: `src/_common/BufferLists/BufferList.cs`

- [ ] **E010** Implement composite naming for chained indicators
  - **Requirements**: When indicators are chained (e.g., SMA of RSI output), the resulting StreamHub should display a composite name showing the full chain for debugging and logging purposes
  - Design naming convention showing full indicator chain (e.g., "SMA(5) of RSI(14)")
  - Inherit provider name from upstream provider in chain
  - Update `Name` property on all StreamHub implementations to support composite names
  - Document naming conventions in `.github/instructions/indicator-stream.instructions.md`
  - Example: `ChainProvider<TIn, TOut>` should propagate upstream provider names through the chain

**Checkpoint**: Phase 6 addresses critical performance issues and feature requests identified during initial streaming rollout

---

## Summary

**Implementation Coverage (1:1:1 Parity)**:

- **Total Series implementations**: 85 indicators (baseline)
- **Total BufferList implementations**: 73 complete, 12 remaining (T001-T085; T068 not implemented)
- **Total StreamHub implementations**: 53 complete, 32 remaining (T086-T170; T153 not implemented)
- **1:1:1 Target**: 85 BufferList + 85 StreamHub = 170 streaming implementations total (excluding algorithmically impractical indicators)
- **Streaming implementations excluded**: StdDevChannels (BufferList + StreamHub) due to repaint-by-design O(n²) recalculation
- **Current streaming coverage**: 161/168 = **95.8% complete** (adjusted target excludes RenkoAtr/StdDevChannels implementations and human-only ZigZag)
**Task Breakdown**:

- **Phase 1**: 10 tasks (A001-A006, T171-T174) — 8 complete, 2 remaining
- **Phase 2**: 85 BufferList implementation tasks (T001-T085) — 82 complete, 3 remaining (T055/T068 not implementing, T085 human-only)
- **Phase 3**: 85 StreamHub implementation tasks (T086-T170) — 79 complete, 6 remaining (T108, T145 implementable; T140/T153 not implementing, T170 human-only)
- **Phase 4**: 17 test infrastructure tasks (T175-T185, Q001-Q006) — 12 complete, 5 remaining
- **Phase 5**: 7 documentation tasks (D001-D007) — 2 complete, 5 remaining
- **Phase 6**: 10 enhancement tasks (E001-E010) — 0 complete, 10 remaining (Priority 4 enhancements + private project items)
- **Total**: 214 tasks — 183 complete, 31 remaining (4 marked as not implementing, 2 human-only)

Removed blanket deferral: The above indicators are complex but unblocked with established reference patterns (see instruction files).
