# Implementation plan: streaming indicators framework

**Branch**: `001-develop-steaming-indicators` | **Date**: 2025-10-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-develop-steaming-indicators/spec.md`

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
- Scale/scope: Five initial indicators (SMA, EMA, RSI, MACD, Bollinger Bands), extensible pattern for 200+ total

## Constitution check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Precision**: Math defined in existing batch indicators; streaming must replicate exactly. Warmup periods inherited from batch. No rounding ambiguity—deterministic state updates.
- **Performance**: O(1) per-tick updates (no recalculation). BufferList: List-based O(1) append. StreamHub: span-optimized minimal allocations. Benchmarks required.
- **Validation**: Enforce warmup period before results. Reject non-ascending timestamps. Bounded buffer sizes prevent runaway memory.
- **Test-Driven**: Unit tests for state transitions, streaming parity tests (batch equivalence), edge tests (warmup, duplicates, buffer limits).
- **Documentation**: Add streaming usage section to each indicator doc page. Include warmup, performance notes, examples. Update library README with streaming overview.
- **Scope & Stewardship**: Pure transformation (quotes → results). No external data, no trading logic, no persistence. Simplicity: two styles only (avoid proliferation).

**Status**: PASS (no violations)

## Project structure

### Documentation (this feature)

```text
specs/001-develop-steaming-indicators/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
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

**IStreamingIndicator Interface**:

```csharp
interface IStreamingIndicator<TQuote, TResult>
{
    TResult? Add(TQuote quote);
    void Reset();
    int WarmupPeriod { get; }
    bool IsWarmedUp { get; }
}
```

**BufferList Pattern** (example for SMA):

```csharp
class SmaBufferList : IStreamingIndicator<Quote, SmaResult>
{
    private readonly int period;
    private readonly List<decimal> buffer;
    
    SmaResult? Add(Quote quote);
    void Reset();
}
```

**StreamHub Pattern** (example for SMA):

```csharp
class SmaStreamHub : IStreamingIndicator<Quote, SmaResult>
{
    private readonly int period;
    private readonly decimal[] buffer;
    private int head, count;
    
    SmaResult? Add(Quote quote);
    void Reset();
}
```

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

### Quickstart example

```csharp
// BufferList usage
var sma = new SmaBufferList(period: 20);

foreach (var quote in liveStream)
{
    var result = sma.Add(quote);
    if (result != null)
        Console.WriteLine($"{result.Date}: {result.Sma:F4}");
}

// StreamHub usage (same API)
var smaHub = new SmaStreamHub(period: 20);
foreach (var quote in liveStream)
{
    var result = smaHub.Add(quote);
    if (result != null)
        Console.WriteLine($"{result.Date}: {result.Sma:F4}");
}
```

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
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

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
