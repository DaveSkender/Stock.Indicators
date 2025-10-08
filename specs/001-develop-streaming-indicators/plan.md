# Implementation plan: streaming indicators framework

**Branch**: `001-develop-streaming-indicators` | **Date**: 2025-10-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-develop-streaming-indicators/spec.md`

> **IMPORTANT**: This planning document contains conceptual examples that may not match actual codebase patterns. For authoritative implementation guidance, always reference:
>
> - `.github/instructions/buffer-indicators.instructions.md` (BufferList pattern)
> - `.github/instructions/stream-indicators.instructions.md` (StreamHub pattern)
> - Existing implementations in `src/**/*.BufferList.cs` and `src/**/*.StreamHub.cs`

## Summary

Implement two streaming indicator styles (BufferList and StreamHub) enabling incremental calculation of technical indicators from real-time market data feeds. BufferList provides simpler List-backed state management while StreamHub offers optimized span-based buffers for high-frequency scenarios. Both styles maintain state across ticks, validate warmup requirements, and converge to batch-calculated results within floating-point tolerance.

## Technical context

- Language/version: C# / .NET 8.0 and .NET 9.0
- Primary dependencies: None (library follows zero-dependency principle)
- Storage: In-memory state (bounded buffers, no persistence)
- Testing: MSTest with streaming parity tests, unit tests, performance benchmarks
- Target platform: Multi-target `net8.0;net9.0`
- Project type: Single project (library enhancement)
- Performance goals: <5ms average per-tick latency (p95 <10ms), <10KB memory per instance
- Constraints: O(1) incremental updates, streaming parity with batch within 1e-12, bounded buffers
- Scale/scope: 107 total tasks across phased rollout (55 BufferList + 52 StreamHub implementations), targeting all feasible Series-based indicators

## Constitution check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Mathematical Precision (NON-NEGOTIABLE)**: Streaming implementations MUST produce mathematically identical results to batch calculations. All warmup periods inherited from validated batch indicators. State updates are deterministic with no silent rounding. Streaming and batch paths converge within 1e-12 tolerance (floating-point drift only). No new mathematical formulas introduced—pure refactoring of calculation timing.

- **Performance First**: O(1) per-tick incremental updates with no recalculation of historical values. BufferList uses List-based O(1) append with bounded capacity. StreamHub uses span-optimized circular buffers for minimal allocations. Target: <5ms mean latency, <10ms p95 latency, <10KB memory per instance. BenchmarkDotNet validation required before merge. No LINQ in per-tick hot paths.

- **Comprehensive Validation**: All public streaming APIs guard against null inputs, empty sequences, and insufficient warmup history with clear `ArgumentException` messages. Enforce strictly ascending timestamps; reject duplicates or out-of-order quotes. Bounded buffer sizes prevent unbounded memory growth. Warmup period validated against batch indicator requirements. State isolation ensures no leakage across streaming instances.

- **Test-Driven Quality**: TDD workflow mandatory: write failing tests before implementation. Each indicator requires: unit tests (state transitions, warmup thresholds, reset behavior), streaming parity tests (batch equivalence validation with Standard test data), edge tests (duplicate timestamps, out-of-order quotes, buffer limits). Regression baseline tests added to prevent future drift. Performance benchmarks required for each streaming style.

- **Documentation Excellence**: Update `docs/_indicators/*.md` for each streaming-enabled indicator with new "Streaming Usage" section including examples, warmup guidance, and performance characteristics. Add streaming overview to library `README.md`. Provide XML documentation for all new public streaming APIs. Include code examples demonstrating BufferList and StreamHub patterns. Document timestamp ordering requirements and buffer capacity constraints.

- **Scope & Stewardship**: Streaming remains pure transformation (quotes + parameters → results). Zero external dependencies maintained. No data fetchers, persistence layers, or trading logic. Two streaming styles only (BufferList for simplicity, StreamHub for performance)—avoid style proliferation. Supports all instrument types (equities, forex, crypto) without reinterpretation of published formulas. Community-reviewable implementation following established repository patterns.

**Status**: PASS (no violations)

## Project structure

### Documentation (this feature)

```text
specs/001-develop-streaming-indicators/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/tasks command)
```

### Source code (repository root)

```text
src/
├── _common/
│   └── Streaming/
│       ├── IStreamingIndicator.cs       # Base interface
│       ├── StreamingState.cs            # Warmup/ready state tracking
│       └── StreamingResult.cs           # Result wrapper
├── a-d/
│   └── Sma/
│       ├── Sma.Series.cs                # Existing batch (unchanged)
│       ├── Sma.BufferList.cs            # New: BufferList streaming
│       └── Sma.StreamHub.cs             # New: StreamHub streaming
├── e-k/
│   └── Ema/
│       ├── Ema.Series.cs
│       ├── Ema.BufferList.cs
│       └── Ema.StreamHub.cs
└── ... (similar for RSI, MACD, Bollinger)

tests/indicators/
├── a-d/
│   └── Sma/
│       ├── Sma.Tests.cs                 # Existing batch tests (unchanged)
│       ├── Sma.BufferList.Tests.cs      # New: BufferList unit + parity
│       └── Sma.StreamHub.Tests.cs       # New: StreamHub unit + parity
└── ... (similar pattern for each indicator)

tests/performance/
└── Benchmarks/
    ├── StreamingBufferList.cs           # BufferList benchmarks
    └── StreamingStreamHub.cs            # StreamHub benchmarks
```

**Structure decision**: Single project enhancement. Streaming code added alongside existing Series-based indicators using partial classes or new sibling files.

## Phase 0: Research and design decisions

No external research required—design extends existing indicator patterns.

### Key decisions

1. **Two Streaming Styles Rationale**:
   - BufferList: Simpler to implement and debug, familiar List-based API, suitable for moderate frequency (<1k ticks/sec)
   - StreamHub: Optimized for high frequency (>10k ticks/sec), uses spans/arrays, minimal GC pressure

2. **State Management**:
   - BufferList: Maintain List of historical values up to lookback period
   - StreamHub: Circular buffer (span-backed) with head/tail pointers

3. **Warmup Handling**:
   - Track `IsWarmedUp` boolean based on accumulated quote count vs required warmup period
   - Return null results until warmed up (consistent with batch behavior)

4. **Timestamp Validation**:
    - Enforce strictly ascending timestamps; any duplicate or out-of-order quote triggers `ArgumentException`
    - Future enhancement: optional reorder buffer (deferred to v2 once requirements clarified)

5. **Parity Testing Strategy**:
   - Feed identical quote sequences to batch and streaming
   - Compare final values element-wise with 1e-12 tolerance
   - Test with Standard test data (502 quotes) for each indicator

**Output**: No separate research.md needed (decisions captured above)

## Phase 1: Design and contracts

Prerequisites: research.md complete (decisions documented in Phase 0 above)

### Data model

**Entities**:

- **IStreamingIndicator\<TQuote, TResult>**: Generic interface with `Add(TQuote)` → `TResult?` method
- **StreamingState**: Enum { NotWarmedUp, Ready }
- **StreamingResult\<T>**: Wrapper containing { DateTime, T Value, StreamingState }
- **BufferList**: List-backed implementation with `Add()`, `Reset()`, bounded capacity
- **StreamHub**: Span-optimized implementation with circular buffer, `Add()`, `Reset()`

### API contracts

**CRITICAL: The examples in this section are CONCEPTUAL ONLY and do NOT match actual implementation patterns.**

For authoritative implementation guidance, see:

- **BufferList**: `.github/instructions/buffer-indicators.instructions.md` + existing implementations in `src/**/*.BufferList.cs`
- **StreamHub**: `.github/instructions/stream-indicators.instructions.md` + existing implementations in `src/**/*.StreamHub.cs`

#### Actual Implementation Patterns

**BufferList Pattern** (actual naming: `{IndicatorName}List`):

- **Base class**: Inherits from `BufferList<TResult>`
- **Interfaces**: Implements `IBufferReusable` (for value-based inputs) or `IBufferList` (IQuote-only)
- **Core method**: Uses `AddInternal(TResult result)` for automatic list management
- **Constructors**: Provides both parameter-only and `(parameters, IEnumerable<IQuote> quotes)` overloads
- **Examples**: `SmaList`, `EmaList`, `RsiList`, `AlligatorList`, `AtrStopList`

**StreamHub Pattern** (actual naming: `{IndicatorName}Hub<TIn>`):

- **Base class**: Extends `StreamHub<TIn, TResult>` (abstract partial class)
- **Provider pattern**: Uses `ChainProvider<TIn, TResult>` or `QuoteProvider<TIn, TResult>` for chaining
- **Interfaces**: Implements indicator-specific interface (e.g., `ISma`, `IEma`)
- **Core method**: Overrides `ToIndicator(TIn item, int? indexHint)` for result generation
- **Observer pattern**: Supports subscription via `IStreamObservable`/`IStreamObserver`
- **Examples**: `SmaHub<TIn>`, `EmaHub<TIn>`, `RsiHub<TIn>`, `AlligatorHub<TIn>`

#### Infrastructure Status

The following base classes and utilities already exist in `src/_common/`:

- ✅ `BufferList<TResult>` - Abstract base for list-backed streaming
- ✅ `BufferUtilities` - Extension methods for buffer management (Update, Prune, etc.)
- ✅ `StreamHub<TIn, TOut>` - Abstract base for hub-based streaming
- ✅ `ChainProvider<TIn, TResult>` / `QuoteProvider<TIn, TResult>` - Provider implementations
- ✅ `IStreamObservable<T>` / `IStreamObserver<T>` - Observer pattern interfaces

**No additional infrastructure tasks required** - all foundational types are production-ready.

### Test scenarios

**Unit Tests** (per indicator, per style):

- Add quotes below warmup → returns null
- Add quotes at warmup → returns first result
- Add quotes after warmup → returns subsequent results
- Reset → clears state, requires rewarmup
- Duplicate timestamp → throws ArgumentException
- Out-of-order timestamp → throws ArgumentException

**Streaming parity tests** (per indicator, per style):

- Feed Standard test data (502 quotes) to batch and streaming
- Compare all non-null results element-wise
- Assert max difference <1e-12

**Edge tests**:

- Buffer capacity enforcement (BufferList)
- Circular wraparound (StreamHub)
- Large warmup periods (e.g., 200-period SMA)

### Implementation guidelines

**IMPORTANT**: Follow the repository's established instruction files for implementation:

- **Buffer-style indicators**: See `.github/instructions/buffer-indicators.instructions.md`
  - Defines BufferList pattern, state management, and testing requirements
  - Includes examples of circular buffer patterns and List-based state
  - **Real implementation**: `{IndicatorName}List` class inheriting from `BufferList<TResult>`
  - **Example**: `SmaList`, `EmaList`, `RsiList` (see existing implementations in `src/`)
  
- **Stream-style indicators**: See `.github/instructions/stream-indicators.instructions.md`
  - Defines StreamHub pattern, span-based optimizations, and performance requirements
  - Includes guidance on memory management and high-frequency scenarios
  - **Real implementation**: `{IndicatorName}Hub<TIn>` class extending `ChainProvider<TIn, TResult>`
  - **Example**: `SmaHub<TIn>`, `EmaHub<TIn>`, `RsiHub<TIn>` (see existing implementations in `src/`)

- **Series-style indicators**: See `.github/instructions/series-indicators.instructions.md`
  - Existing batch calculation patterns (unchanged by this feature)
  - Reference for mathematical correctness and parity validation

- **Source code completion**: See `.github/instructions/source-code-completion.instructions.md`
  - Pre-commit checklist for code quality, testing, and documentation
  - Ensures consistency with library standards

**Note**: The actual API patterns in the codebase differ from preliminary planning examples. Always reference the instruction files and existing implementations (`src/**/*.BufferList.cs`, `src/**/*.StreamHub.cs`) as the authoritative source.

### Documentation updates

- Update `docs/_indicators/Sma.md` (and EMA, RSI, MACD, BollingerBands) with streaming section
- Add streaming examples to each indicator page
- Update `README.md` with streaming overview paragraph
- Update `src/_common/ObsoleteV3.md` migration guide with streaming capability summary (public release notes remain automated via GitHub Releases)

### Quality gates and conformance

- Run public API approval tests (`tests/public-api`) to confirm streaming additions respect existing signatures and conventions

## Phase 2: Task planning approach

*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task generation strategy**:

- Load Phase 1 contracts and data model
- Generate tasks per indicator per style (5 indicators × 2 styles = 10 implementation groups)
- Each group: interface → BufferList impl → BufferList tests → StreamHub impl → StreamHub tests
- Mark [P] for parallel execution across different indicators
- Sequential within each indicator (tests depend on impl)

**Ordering strategy**:

- Common infrastructure first (IStreamingIndicator, StreamingState, test helpers)
- Then per-indicator rollout: SMA → EMA → RSI → MACD → Bollinger Bands
- Tests before marking implementation complete (TDD)

**Estimated output**: ~40 tasks (infrastructure + 5 indicators × 2 styles × 3 tasks each)

## Phase 3+: Future implementation

*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, performance validation, documentation review)

## Complexity tracking

*Fill ONLY if Constitution Check has violations that must be justified*

No violations—section intentionally empty.

## Progress tracking

*This checklist is updated during execution flow*

**Phase status**:

- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [ ] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate status**:

- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented

---

*Based on Constitution v1.1.0 - See `/memory/constitution.md`*

---
Last updated: October 6, 2025
