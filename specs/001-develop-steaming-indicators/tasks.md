# Tasks: Streaming Indicators Framework

**Input**: Design documents from `/specs/001-develop-steaming-indicators/`
**Prerequisites**: plan.md (required), data-model.md, quickstart.md

## Format: `[ID] [P?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions

Paths assume single project structure at repository root:

- Source: `src/`
- Tests: `tests/indicators/` and `tests/performance/`

## Phase 3.1: Setup & Infrastructure

- [ ] T001 Create streaming infrastructure folder at `src/_common/Streaming/`
- [ ] T002 [P] Create `IStreamingIndicator.cs` interface in `src/_common/Streaming/IStreamingIndicator.cs`
- [ ] T003 [P] Create `StreamingState.cs` enum in `src/_common/Streaming/StreamingState.cs`
- [ ] T004 [P] Create streaming test helper utilities in `tests/indicators/_common/StreamingTestHelpers.cs`

## Phase 3.2: Tests First (TDD) - SMA Indicator ⚠️ MUST COMPLETE BEFORE 3.3

**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**

- [ ] T005 [P] BufferList unit tests for SMA in `tests/indicators/a-d/Sma/Sma.BufferList.Tests.cs`
  - Test warmup period enforcement
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate timestamp rejection
  - Test out-of-order timestamp rejection
  - Test buffer capacity enforcement

- [ ] T006 [P] StreamHub unit tests for SMA in `tests/indicators/a-d/Sma/Sma.StreamHub.Tests.cs`
  - Test warmup period enforcement
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate timestamp rejection
  - Test out-of-order timestamp rejection
  - Test circular buffer wraparound

- [ ] T007 [P] Streaming parity test for SMA BufferList in `tests/indicators/a-d/Sma/Sma.BufferList.Tests.cs`
  - Feed Standard test data (502 quotes) to batch and streaming
  - Compare all results element-wise with 1e-12 tolerance

- [ ] T008 [P] Streaming parity test for SMA StreamHub in `tests/indicators/a-d/Sma/Sma.StreamHub.Tests.cs`
  - Feed Standard test data (502 quotes) to batch and streaming
  - Compare all results element-wise with 1e-12 tolerance

## Phase 3.3: Core Implementation - SMA (ONLY after tests are failing)

- [ ] T009 Implement SMA BufferList in `src/a-d/Sma/Sma.BufferList.cs`
  - List-backed state management
  - O(1) append with bounded capacity
  - Warmup validation
  - Timestamp ordering enforcement

- [ ] T010 Implement SMA StreamHub in `src/a-d/Sma/Sma.StreamHub.cs`
  - Circular buffer with span optimization
  - O(1) updates with minimal allocations
  - Warmup validation
  - Timestamp ordering enforcement

## Phase 3.4: Tests First (TDD) - EMA Indicator

- [ ] T011 [P] BufferList unit tests for EMA in `tests/indicators/e-k/Ema/Ema.BufferList.Tests.cs`
  - Test warmup period enforcement (period quotes for initial SMA seed)
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection
  - Test EMA recursive formula accuracy

- [ ] T012 [P] StreamHub unit tests for EMA in `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs`
  - Test warmup period enforcement
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection
  - Test EMA recursive formula accuracy

- [ ] T013 [P] Streaming parity test for EMA BufferList in `tests/indicators/e-k/Ema/Ema.BufferList.Tests.cs`

- [ ] T014 [P] Streaming parity test for EMA StreamHub in `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs`

## Phase 3.5: Core Implementation - EMA

- [ ] T015 Implement EMA BufferList in `src/e-k/Ema/Ema.BufferList.cs`
  - Initial SMA seed calculation
  - Recursive EMA formula
  - State management for previous EMA value

- [ ] T016 Implement EMA StreamHub in `src/e-k/Ema/Ema.StreamHub.cs`
  - Optimized seed calculation
  - Recursive EMA with minimal state

## Phase 3.6: Tests First (TDD) - RSI Indicator

- [ ] T017 [P] BufferList unit tests for RSI in `tests/indicators/m-r/Rsi/Rsi.BufferList.Tests.cs`
  - Test warmup period enforcement (period + 1 quotes)
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection
  - Test RSI bounds (0-100)
  - Test gain/loss calculation accuracy

- [ ] T018 [P] StreamHub unit tests for RSI in `tests/indicators/m-r/Rsi/Rsi.StreamHub.Tests.cs`
  - Test warmup period enforcement
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection
  - Test RSI bounds (0-100)

- [ ] T019 [P] Streaming parity test for RSI BufferList in `tests/indicators/m-r/Rsi/Rsi.BufferList.Tests.cs`

- [ ] T020 [P] Streaming parity test for RSI StreamHub in `tests/indicators/m-r/Rsi/Rsi.StreamHub.Tests.cs`

## Phase 3.7: Core Implementation - RSI

- [ ] T021 Implement RSI BufferList in `src/m-r/Rsi/Rsi.BufferList.cs`
  - Gain/loss tracking
  - Average gain/loss calculation
  - RS and RSI formula

- [ ] T022 Implement RSI StreamHub in `src/m-r/Rsi/Rsi.StreamHub.cs`
  - Optimized gain/loss state
  - Span-based calculations

## Phase 3.8: Tests First (TDD) - MACD Indicator

- [ ] T023 [P] BufferList unit tests for MACD in `tests/indicators/m-r/Macd/Macd.BufferList.Tests.cs`
  - Test warmup period enforcement (slowPeriod quotes)
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection
  - Test MACD line, Signal line, Histogram calculations

- [ ] T024 [P] StreamHub unit tests for MACD in `tests/indicators/m-r/Macd/Macd.StreamHub.Tests.cs`
  - Test warmup period enforcement
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection

- [ ] T025 [P] Streaming parity test for MACD BufferList in `tests/indicators/m-r/Macd/Macd.BufferList.Tests.cs`

- [ ] T026 [P] Streaming parity test for MACD StreamHub in `tests/indicators/m-r/Macd/Macd.StreamHub.Tests.cs`

## Phase 3.9: Core Implementation - MACD

- [ ] T027 Implement MACD BufferList in `src/m-r/Macd/Macd.BufferList.cs`
  - Dual EMA calculation (fast and slow)
  - Signal line EMA
  - Histogram derivation

- [ ] T028 Implement MACD StreamHub in `src/m-r/Macd/Macd.StreamHub.cs`
  - Optimized dual EMA state
  - Signal line with minimal allocations

## Phase 3.10: Tests First (TDD) - Bollinger Bands Indicator

- [ ] T029 [P] BufferList unit tests for Bollinger Bands in `tests/indicators/a-d/BollingerBands/BollingerBands.BufferList.Tests.cs`
  - Test warmup period enforcement (period quotes)
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection
  - Test SMA, upper band, lower band calculations
  - Test standard deviation accuracy

- [ ] T030 [P] StreamHub unit tests for Bollinger Bands in `tests/indicators/a-d/BollingerBands/BollingerBands.StreamHub.Tests.cs`
  - Test warmup period enforcement
  - Test null results before warmup
  - Test Reset() functionality
  - Test duplicate/out-of-order timestamp rejection

- [ ] T031 [P] Streaming parity test for Bollinger Bands BufferList in `tests/indicators/a-d/BollingerBands/BollingerBands.BufferList.Tests.cs`

- [ ] T032 [P] Streaming parity test for Bollinger Bands StreamHub in `tests/indicators/a-d/BollingerBands/BollingerBands.StreamHub.Tests.cs`

## Phase 3.11: Core Implementation - Bollinger Bands

- [ ] T033 Implement Bollinger Bands BufferList in `src/a-d/BollingerBands/BollingerBands.BufferList.cs`
  - SMA calculation
  - Standard deviation calculation
  - Upper/lower band derivation

- [ ] T034 Implement Bollinger Bands StreamHub in `src/a-d/BollingerBands/BollingerBands.StreamHub.cs`
  - Optimized SMA and stdDev
  - Span-based band calculations

## Phase 3.12: Performance Testing

- [ ] T035 [P] BufferList performance benchmarks in `tests/performance/Benchmarks/StreamingBufferList.cs`
  - Benchmark SMA, EMA, RSI, MACD, Bollinger Bands
  - Verify <5ms avg per-tick latency
  - Verify <10ms p95 latency
  - Verify <10KB memory per instance

- [ ] T036 [P] StreamHub performance benchmarks in `tests/performance/Benchmarks/StreamingStreamHub.cs`
  - Benchmark SMA, EMA, RSI, MACD, Bollinger Bands
  - Verify <2ms avg per-tick latency
  - Verify <5ms p95 latency
  - Verify <5KB memory per instance

## Phase 3.13: Integration Testing

- [ ] T037 [P] Integration test for BufferList quickstart scenario in `tests/indicators/_common/StreamingIntegrationTests.cs`
  - Execute quickstart.md BufferList example
  - Validate 483 results from 502 quotes (20-period warmup)
  - Verify parity with batch SMA

- [ ] T038 [P] Integration test for StreamHub quickstart scenario in `tests/indicators/_common/StreamingIntegrationTests.cs`
  - Execute quickstart.md StreamHub example
  - Validate performance <50ms for 502 quotes
  - Verify parity with batch SMA

- [ ] T039 [P] Integration test for error handling in `tests/indicators/_common/StreamingIntegrationTests.cs`
  - Test duplicate timestamp exception
  - Test out-of-order timestamp exception
  - Test invalid period exception
  - Test null quote exception

## Phase 3.14: Documentation

- [ ] T040 [P] Update SMA documentation in `docs/_indicators/Sma.md`
  - Add streaming usage section
  - Include BufferList and StreamHub examples
  - Document warmup period
  - Add performance notes

- [ ] T041 [P] Update EMA documentation in `docs/_indicators/Ema.md`
  - Add streaming usage section
  - Include examples
  - Document warmup period

- [ ] T042 [P] Update RSI documentation in `docs/_indicators/Rsi.md`
  - Add streaming usage section
  - Include examples
  - Document warmup period

- [ ] T043 [P] Update MACD documentation in `docs/_indicators/Macd.md`
  - Add streaming usage section
  - Include examples
  - Document warmup period

- [ ] T044 [P] Update Bollinger Bands documentation in `docs/_indicators/BollingerBands.md`
  - Add streaming usage section
  - Include examples
  - Document warmup period

- [ ] T045 Update README.md with streaming overview in `README.md`
  - Add streaming indicators section
  - Explain BufferList vs StreamHub
  - Provide getting started example
  - Link to indicator pages

- [ ] T046 Add release notes entry for streaming capabilities in appropriate release notes file
  - Document new streaming API
  - List supported indicators
  - Note breaking changes (if any)

## Dependencies

**Setup Phase**:

- T001 blocks T002, T003 (infrastructure folder must exist first)
- T002, T003, T004 can run in parallel [P]

**Per-Indicator Pattern** (SMA example, repeated for each indicator):

- Tests (T005-T008) before implementation (T009-T010)
- T005, T006, T007, T008 can run in parallel [P]
- T009 blocks nothing (can proceed once tests fail)
- T010 blocks nothing (can proceed once tests fail)

**Cross-Indicator**:

- SMA (T009-T010) should complete before EMA (T015-T016) for confidence
- EMA before RSI, RSI before MACD, MACD before Bollinger (sequential rollout)
- Performance tests (T035-T036) after all implementations (T009-T034)
- Integration tests (T037-T039) after all implementations
- Documentation (T040-T046) after all implementations and tests pass

**Polish Phase**:

- T035, T036 can run in parallel [P]
- T037, T038, T039 can run in parallel [P]
- T040-T044 can run in parallel [P]
- T045 after T040-T044 (references indicator pages)
- T046 after T045 (release notes reference README)

## Parallel Execution Examples

### Phase 3.1 Infrastructure (Parallel)

```markdown
Task: "Create IStreamingIndicator.cs interface in src/_common/Streaming/IStreamingIndicator.cs"
Task: "Create StreamingState.cs enum in src/_common/Streaming/StreamingState.cs"
Task: "Create streaming test helper utilities in tests/indicators/_common/StreamingTestHelpers.cs"
```

### Phase 3.2 SMA Tests (Parallel)

```markdown
Task: "BufferList unit tests for SMA in tests/indicators/a-d/Sma/Sma.BufferList.Tests.cs"
Task: "StreamHub unit tests for SMA in tests/indicators/a-d/Sma/Sma.StreamHub.Tests.cs"
Task: "Streaming parity test for SMA BufferList in tests/indicators/a-d/Sma/Sma.BufferList.Tests.cs"
Task: "Streaming parity test for SMA StreamHub in tests/indicators/a-d/Sma/Sma.StreamHub.Tests.cs"
```

### Phase 3.12 Performance Benchmarks (Parallel)

```markdown
Task: "BufferList performance benchmarks in tests/performance/Benchmarks/StreamingBufferList.cs"
Task: "StreamHub performance benchmarks in tests/performance/Benchmarks/StreamingStreamHub.cs"
```

### Phase 3.14 Documentation Updates (Parallel)

```markdown
Task: "Update SMA documentation in docs/_indicators/Sma.md"
Task: "Update EMA documentation in docs/_indicators/Ema.md"
Task: "Update RSI documentation in docs/_indicators/Rsi.md"
Task: "Update MACD documentation in docs/_indicators/Macd.md"
Task: "Update Bollinger Bands documentation in docs/_indicators/BollingerBands.md"
```

## Notes

- [P] tasks target different files and have no shared dependencies
- Verify all tests fail before implementing (TDD principle)
- Run `dotnet test --no-restore` after each implementation task
- Run `dotnet format` before committing
- Each indicator follows the same test → implement → verify pattern
- Performance tests should run on production-like data volumes
- Integration tests validate the quickstart.md scenarios

## Validation Checklist

*Checked before task execution begins*

- [x] All indicators have unit tests (warmup, reset, validation)
- [x] All indicators have parity tests (batch equivalence)
- [x] All implementations have corresponding tests
- [x] Parallel tasks target different files
- [x] Each task specifies exact file path
- [x] Dependencies clearly documented
- [x] TDD ordering enforced (tests before implementation)
- [x] Performance targets specified in benchmark tasks
- [x] Documentation updates cover all 5 indicators

---

**Total Tasks**: 46
**Parallelizable Tasks**: 28 (marked with [P])
**Sequential Tasks**: 18
**Estimated Completion**: 10-15 implementation sessions (assuming TDD workflow)
