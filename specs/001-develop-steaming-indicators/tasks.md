# Tasks: streaming indicators framework

**Input**: Design documents from `/specs/001-develop-steaming-indicators/`
**Prerequisites**: plan.md (required), data-model.md, quickstart.md

## Format: `[ID] [P?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions

Paths assume the single-project layout at the repository root:

- Source: `src/`
- Tests: `tests/`

## Phase 3.1: Setup and infrastructure

- [ ] T001 Prepare streaming infrastructure directory `src/_common/Streaming/` and ensure namespace alignment in `GlobalUsings.cs` if needed
- [ ] T002 [P] Define streaming interface contract in `src/_common/Streaming/IStreamingIndicator.cs`
  - Include Add, Reset, WarmupPeriod, and IsWarmedUp members with XML docs
  - Enforce null and timestamp validation expectations in documentation
- [ ] T003 [P] Add warmup state enum in `src/_common/Streaming/StreamingState.cs`
  - Provide NotWarmedUp and Ready members with summaries
- [ ] T004 [P] Add streaming result wrapper in `src/_common/Streaming/StreamingResult.cs`
  - Implement generic record with Timestamp, Value, State properties
  - Include XML docs and guard clauses for required init properties
- [ ] T005 [P] Create streaming test helpers in `tests/indicators/_common/StreamingTestHelpers.cs`
  - Provide shared quote feeders, tolerance assertions, and reset utilities

## Phase 3.2: Tests first (TDD) – SMA indicator ⚠️ MUST COMPLETE BEFORE 3.3

- [ ] T006 [P] Create SMA BufferList streaming tests in `tests/indicators/a-d/Sma/Sma.BufferList.Tests.cs`
  - Cover warmup gating, reset behavior, duplicate/out-of-order timestamps, and buffered window rollover
  - Add parity test comparing against batch SMA with 1e-12 tolerance
- [ ] T007 [P] Create SMA StreamHub streaming tests in `tests/indicators/a-d/Sma/Sma.StreamHub.Tests.cs`
  - Cover warmup gating, reset behavior, duplicate/out-of-order timestamps, and circular buffer wraparound
  - Add parity test comparing against batch SMA with 1e-12 tolerance

## Phase 3.3: Core implementation – SMA (ONLY after tests are failing)

- [ ] T008 [P] Implement SMA BufferList indicator in `src/a-d/Sma/Sma.BufferList.cs`
  - Maintain bounded List state and enforce timestamp ordering
  - Return null until warmed and emit results matching batch SMA
- [ ] T009 [P] Implement SMA StreamHub indicator in `src/a-d/Sma/Sma.StreamHub.cs`
  - Use circular buffer with span-friendly calculations and minimal allocations

## Phase 3.4: Tests first (TDD) – EMA indicator

- [ ] T010 [P] Create EMA BufferList streaming tests in `tests/indicators/e-k/Ema/Ema.BufferList.Tests.cs`
  - Validate warmup seed, recursive EMA accuracy, reset behavior, and timestamp validation
  - Include parity checks versus batch EMA with 1e-12 tolerance
- [ ] T011 [P] Create EMA StreamHub streaming tests in `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs`
  - Mirror BufferList coverage plus circular buffer assertions

## Phase 3.5: Core implementation – EMA

- [ ] T012 [P] Implement EMA BufferList indicator in `src/e-k/Ema/Ema.BufferList.cs`
  - Seed with SMA, maintain recursive state, and enforce warmup semantics
- [ ] T013 [P] Implement EMA StreamHub indicator in `src/e-k/Ema/Ema.StreamHub.cs`
  - Optimize seed handling and recursive updates with span buffers

## Phase 3.6: Tests first (TDD) – RSI indicator

- [ ] T014 [P] Create RSI BufferList streaming tests in `tests/indicators/m-r/Rsi/Rsi.BufferList.Tests.cs`
  - Verify warmup period (period + 1), gain/loss accumulation, bounds, and reset
  - Add parity assertions versus batch RSI
- [ ] T015 [P] Create RSI StreamHub streaming tests in `tests/indicators/m-r/Rsi/Rsi.StreamHub.Tests.cs`
  - Mirror BufferList coverage with circular buffer assertions

## Phase 3.7: Core implementation – RSI

- [ ] T016 [P] Implement RSI BufferList indicator in `src/m-r/Rsi/Rsi.BufferList.cs`
  - Track average gains/losses and emit RSI values within expected bounds
- [ ] T017 [P] Implement RSI StreamHub indicator in `src/m-r/Rsi/Rsi.StreamHub.cs`
  - Optimize rolling averages with span math and low allocations

## Phase 3.8: Tests first (TDD) – MACD indicator

- [ ] T018 [P] Create MACD BufferList streaming tests in `tests/indicators/m-r/Macd/Macd.BufferList.Tests.cs`
  - Validate warmup (slow period), fast/slow EMA alignment, signal line, histogram, and reset paths
- [ ] T019 [P] Create MACD StreamHub streaming tests in `tests/indicators/m-r/Macd/Macd.StreamHub.Tests.cs`
  - Mirror BufferList coverage with circular buffer assertions and parity checks

## Phase 3.9: Core implementation – MACD

- [ ] T020 [P] Implement MACD BufferList indicator in `src/m-r/Macd/Macd.BufferList.cs`
  - Maintain dual EMA and signal state plus histogram calculation
- [ ] T021 [P] Implement MACD StreamHub indicator in `src/m-r/Macd/Macd.StreamHub.cs`
  - Optimize dual EMA updates using span-friendly buffers

## Phase 3.10: Tests first (TDD) – Bollinger Bands indicator

- [ ] T022 [P] Create Bollinger Bands BufferList streaming tests in `tests/indicators/a-d/BollingerBands/BollingerBands.BufferList.Tests.cs`
  - Validate warmup, SMA core, standard deviation math, band widths, reset, and timestamp ordering
- [ ] T023 [P] Create Bollinger Bands StreamHub streaming tests in `tests/indicators/a-d/BollingerBands/BollingerBands.StreamHub.Tests.cs`
  - Mirror BufferList coverage with circular buffer assertions and parity checks

## Phase 3.11: Core implementation – Bollinger Bands

- [ ] T024 [P] Implement Bollinger Bands BufferList indicator in `src/a-d/BollingerBands/BollingerBands.BufferList.cs`
  - Compute SMA, standard deviation, and upper/lower bands with bounded List state
- [ ] T025 [P] Implement Bollinger Bands StreamHub indicator in `src/a-d/BollingerBands/BollingerBands.StreamHub.cs`
  - Optimize rolling statistics with span buffers and minimal allocations

## Phase 3.12: Performance testing

- [ ] T026 [P] Author BufferList performance benchmarks in `tests/performance/Benchmarks/StreamingBufferList.cs`
  - Measure SMA, EMA, RSI, MACD, Bollinger scenarios and confirm <5ms avg / <10ms p95 latency and <10KB memory
- [ ] T027 [P] Author StreamHub performance benchmarks in `tests/performance/Benchmarks/StreamingStreamHub.cs`
  - Measure same scenarios verifying <2ms avg / <5ms p95 latency and <5KB memory

## Phase 3.13: Integration testing

- [ ] T028 Implement BufferList quickstart integration test in `tests/indicators/_common/StreamingIntegrationTests.cs`
  - Execute quickstart BufferList loop, assert 483 results, IsWarmedUp true, and parity with batch SMA
- [ ] T029 Implement StreamHub quickstart integration test in `tests/indicators/_common/StreamingIntegrationTests.cs`
  - Execute quickstart StreamHub loop, assert 483 results, performance <50ms, parity with batch SMA
- [ ] T030 Implement streaming error-handling integration tests in `tests/indicators/_common/StreamingIntegrationTests.cs`
  - Assert exceptions for duplicate timestamps, out-of-order timestamps, invalid period, and null quotes

## Phase 3.14: Documentation and release artifacts

- [ ] T031 [P] Update SMA documentation in `docs/_indicators/Sma.md` with streaming usage, warmup guidance, and examples
- [ ] T032 [P] Update EMA documentation in `docs/_indicators/Ema.md` with streaming usage, warmup guidance, and examples
- [ ] T033 [P] Update RSI documentation in `docs/_indicators/Rsi.md` with streaming usage, warmup guidance, and examples
- [ ] T034 [P] Update MACD documentation in `docs/_indicators/Macd.md` with streaming usage, warmup guidance, and examples
- [ ] T035 [P] Update Bollinger Bands documentation in `docs/_indicators/BollingerBands.md` with streaming usage, warmup guidance, and examples
- [ ] T036 Refresh top-level streaming overview in `README.md`
  - Summarize BufferList vs StreamHub, provide quickstart snippet, and link indicator docs without changelog-style language
- [ ] T037 Update streaming migration guidance in `src/_common/ObsoleteV3.md`
  - Document BufferList vs StreamHub usage, supported indicators, and migration notes for the v3 release

## Phase 3.15: API and quality verification

- [ ] T038 Execute public API conformance check via `tests/public-api` suite
  - Ensure streaming types follow existing naming, namespace, and accessibility conventions
  - Confirm no unintended public surface changes or breaking modifications

## Dependencies

- T001 precedes T002–T005 (directory scaffold before files)
- T006 and T007 require T002–T005 complete; both must fail before T008–T009 begin
- For each indicator, complete tests (e.g., T010–T011) before implementations (e.g., T012–T013)
- Indicator rollout order: SMA → EMA → RSI → MACD → Bollinger (later indicators depend on foundations from earlier ones)
- Performance benchmarks (T026–T027) depend on all indicator implementations (T008–T025)
- Integration tests (T028–T030) depend on all implementations and helpers
- Documentation tasks (T031–T037) depend on successful integration tests and benchmark validation
- API conformance (T038) runs after implementations to validate surface stability before release prep

## Parallel execution examples

### Infrastructure burst

```markdown
Task: "Define streaming interface contract in src/_common/Streaming/IStreamingIndicator.cs"
Task: "Add warmup state enum in src/_common/Streaming/StreamingState.cs"
Task: "Add streaming result wrapper in src/_common/Streaming/StreamingResult.cs"
Task: "Create streaming test helpers in tests/indicators/_common/StreamingTestHelpers.cs"
Command: /tasks run T002 T003 T004 T005
```

### SMA test sprint (after setup)

```markdown
Task: "Create SMA BufferList streaming tests in tests/indicators/a-d/Sma/Sma.BufferList.Tests.cs"
Task: "Create SMA StreamHub streaming tests in tests/indicators/a-d/Sma/Sma.StreamHub.Tests.cs"
Command: /tasks run T006 T007
```

### Performance validation pairing

```markdown
Task: "Author BufferList performance benchmarks in tests/performance/Benchmarks/StreamingBufferList.cs"
Task: "Author StreamHub performance benchmarks in tests/performance/Benchmarks/StreamingStreamHub.cs"
Command: /tasks run T026 T027
```

### Documentation blitz

```markdown
Task: "Update EMA documentation in docs/_indicators/Ema.md with streaming usage, warmup guidance, and examples"
Task: "Update MACD documentation in docs/_indicators/Macd.md with streaming usage, warmup guidance, and examples"
Task: "Update Bollinger Bands documentation in docs/_indicators/BollingerBands.md with streaming usage, warmup guidance, and examples"
Command: /tasks run T032 T034 T035

```

### API validation wrap-up

```markdown
Task: "Execute public API conformance check via tests/public-api suite"
Command: /tasks run T038
```

## Notes

- [P] markers indicate tasks touching distinct files with no blocking dependencies
- Maintain strict TDD: author tests, observe failures, then implement
- Run `dotnet test --no-restore` after each implementation phase and benchmark tasks before documentation updates
- Reuse streaming helpers to minimize duplication across test suites
- Align documentation tone with repository standards and include streaming parity guidance drawn from quickstart.md

## Validation checklist

- [x] All entities from data-model.md have corresponding tasks (interface, state, result, indicator implementations)
- [x] Every indicator style has tests scheduled before implementation
- [x] Performance and integration coverage align with quickstart expectations
- [x] Parallel tasks avoid file collisions
- [x] Documentation updates cover all affected public pages and README
- [x] Streaming migration guidance updated in `src/_common/ObsoleteV3.md`
- [x] Public API conventions verified post-implementation

---

- **Total tasks**: 38
- **Parallelizable tasks**: 24 (marked with [P])
- **Sequential tasks**: 14
- **Estimated completion**: 10 focused TDD sessions with follow-up validation

---
Last updated: October 6, 2025
