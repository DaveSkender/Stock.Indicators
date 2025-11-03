# Feature specification: streaming indicators framework

**Feature branch**: `001-develop-streaming-indicators`
**Created**: 2025-10-02
**Status**: Draft
**Input**: User description: "develop steaming indicators in two styles (buffering lists and more advanced stream hubs) to enable use of realtime financial market data streams that automatically generate indicator data incrementally."

## User scenarios & testing

### Primary user story

As a developer building real-time trading applications, I need indicators that update incrementally when new price data arrives, so that I can display live technical analysis without recalculating entire historical datasets every tick.

### Acceptance scenarios

1. **Given** an initialized streaming indicator with historical warmup data, **When** a single new price quote arrives, **Then** the indicator produces one new result value in <5ms average latency
2. **Given** a buffering-style streaming indicator receiving quotes, **When** 1000 quotes are streamed sequentially, **Then** final indicator values match batch-calculated results exactly (deterministic equality)
3. **Given** a stream hub managing state across multiple ticks, **When** a quote arrives with a timestamp less than or equal to the most recent processed timestamp, **Then** the system rejects the quote with a descriptive exception (no reordering tolerance in v1)

### Edge cases

- What happens when the first streamed quote has insufficient warmup history? System must validate minimum warmup period and reject or buffer until satisfied.
- How does the system handle duplicate timestamps? Must reject duplicates with `ArgumentException` and surface the offending timestamp.
- What happens when memory limits are approached in buffering mode? BufferList enforces `MaxListSize` property (default 90% of `int.MaxValue` ≈ 1.9B elements). When exceeded, oldest elements are automatically pruned via `AddInternal()` mechanism. StreamHub uses fixed-size circular buffers calculated from indicator parameters (typically `lookbackPeriod × 1.2` for margin), preventing unbounded growth.

## Requirements

### Functional requirements

- **FR-001**: Library MUST provide two streaming indicator styles: BufferList (simpler, list-backed state) and StreamHub (advanced, optimized state management)
- **FR-002**: Streaming indicators MUST accept single quote additions incrementally and produce corresponding result values
- **FR-003**: Streaming indicators MUST maintain internal state (buffers, accumulators, derived values) across calls without recalculating from scratch
- **FR-004**: Final indicator values MUST be mathematically identical to batch-calculated results when same quote sequence is provided (deterministic equality; no tolerance for drift)
- **FR-005**: Streaming indicators MUST validate warmup period requirements before producing results
- **FR-006**: Streaming indicators MUST enforce strictly ascending timestamps by rejecting any duplicate or out-of-order quotes
- **FR-007**: BufferList style MUST use List-backed storage with O(1) append and bounded size enforcement
- **FR-008**: StreamHub style MUST optimize for high-frequency scenarios with span-based buffers and minimal allocations
- **FR-009**: Streaming indicators MUST provide a method to reset state and reinitialize warmup
- **FR-010**: Library MUST include streaming variants for all feasible Series-based indicators. Initial set (SMA, EMA, RSI, MACD, Bollinger Bands) serves as reference implementations for pattern validation; full deployment targets 80 implementable indicators across 97 implementation tasks (T001-T055 BufferList + T061-T107 StreamHub excluding 5 deferred indicators), excluding 5 indicators deferred to v2 due to streaming incompatibility (Fractal, HtTrendline, Hurst, Ichimoku, Slope).

### Non-functional requirements

- **NFR-001**: Average per-tick update latency <5ms (p95 <10ms) for single indicator instance, measured on commodity hardware (4-core 3GHz CPU, 16GB RAM) using BenchmarkDotNet with standard .NET 9.0 release configuration. Latency measured as wall-clock time for single `Add(quote)` call after warmup period completion.
- **NFR-002**: Memory overhead per streaming indicator instance <10KB for indicators with lookback periods ≤200 (covers approximately 90% of library indicators based on catalog analysis; larger periods documented per-indicator)
- **NFR-003**: Streaming parity tests MUST validate equivalence with batch calculations for all supported indicators
- **NFR-004**: API design MUST follow existing library conventions (no breaking changes to batch APIs)
- **NFR-005**: Documentation MUST include streaming usage examples, warmup guidance, and performance characteristics
- **NFR-006**: Release deliverable MUST leverage existing GitHub Releases automation and update `src/MigrationGuide.V3.md` with streaming migration guidance per Constitution Principle 5
- **NFR-007**: Regression tests MUST validate all three implementation styles (Series, BufferList, StreamHub) against baseline results with deterministic equality (NOT approximate equality). Baselines stored in `tests/indicators/_testdata/results/{indicator}.standard.json` and generated using catalog default parameters.

### Interface architecture requirements

BufferList implementations MUST implement ONE of three increment interfaces based on data requirements:

- **FR-011**: `IIncrementFromChain` MUST be used for chainable indicators that work with single reusable values (SMA, EMA, RSI, MACD). These indicators MUST accept `IReusable` inputs and MUST NOT have `Add(IQuote)` methods. Constructor MUST accept `IReadOnlyList<IReusable>` values. Extension method MUST use `<T>` generic with `where T : IReusable` constraint.

- **FR-012**: `IIncrementFromQuote` MUST be used for indicators requiring multiple OHLC values per quote (VWMA needs price+volume, Stoch needs HLC, Vwap needs HLCV). Constructor MUST accept `IReadOnlyList<IQuote>` quotes. Extension method MUST use `<TQuote>` generic with `where TQuote : IQuote` constraint.

- **FR-013**: `IIncrementFromPairs` MUST be used for indicators requiring two synchronized input series (Correlation, Beta). These indicators MUST accept paired `IReusable` values.

- **FR-014**: Chainable indicators (`IIncrementFromChain`) that specifically require OHLC price data (such as Alligator, Mama, FisherTransform) SHOULD use utility methods: `value.Hl2OrValue()` returns HL2 if IQuote, otherwise Value; `value.QuotePartOrValue(CandlePart.*)` returns specified part if IQuote, otherwise Value. Most chainable indicators (SMA, EMA, RSI, MACD) do not need these utilities and should use `value.Value` directly.

See `.github/instructions/indicator-buffer.instructions.md` for complete implementation patterns and examples.

### Key entities

- **`IIncrementFromChain`**: Interface for chainable indicators accepting single reusable values (implements `Add(DateTime, double)`, `Add(IReusable)`, `Add(IReadOnlyList<IReusable>)`)
- **`IIncrementFromQuote`**: Interface for indicators requiring OHLC quote data (implements `Add(IQuote)`, `Add(IReadOnlyList<IQuote>)`)
- **`IIncrementFromPairs`**: Interface for dual-series indicators (implements paired `IReusable` addition methods)
- **BufferList Indicator**: Concrete streaming style using List-backed state, simpler to understand and extend
- **StreamHub Indicator**: Concrete streaming style using optimized buffers, suitable for high-frequency scenarios
- **Quote**: Input entity representing OHLCV data with timestamp (implements `IQuote`)
- **Reusable Value**: Input entity with timestamp and single numeric value (implements `IReusable`)
- **Indicator Result**: Output entity with timestamp, indicator value(s), and metadata
- **Warmup State**: Tracks whether indicator has sufficient history to produce valid results
- **Regression Baseline**: JSON-serialized expected results from Series implementation using standard test data (502 quotes), serving as reference for BufferList and StreamHub parity validation

## Terminology & Definitions

- **BufferList**: List-backed streaming implementation using `List<T>` for state storage. Simpler to implement and debug. Suitable for moderate frequency scenarios (<1k ticks/sec). Naming convention: `{IndicatorName}List` (e.g., `SmaList`, `EmaList`, `DojiList`).
- **StreamHub**: Span-optimized streaming implementation using circular buffers for state storage. Optimized for high-frequency scenarios (>10k ticks/sec). Naming convention: `{IndicatorName}Hub` (e.g., `SmaHub`, `EmaHub`, `RsiHub`).
- **Warmup Period**: Minimum number of quotes required before indicator produces valid results. Inherited from Series implementation's `WarmupPeriod` property.
- **Streaming Parity**: Requirement that streaming and batch (Series) calculations produce mathematically identical results when given the same quote sequence. Tested using deterministic equality assertions (`BeEquivalentTo` with `WithStrictOrdering()`), never approximate equality.
- **Buffer Capacity**: Maximum number of elements stored in internal buffers. For BufferList: dynamically managed via `MaxListSize` (default ~1.9B elements). For StreamHub: typically fixed at `lookbackPeriod + margin` for efficient circular buffer operations.
- **Regression Test**: Automated validation that compares indicator output against pre-generated baseline results to detect unintended behavioral changes. Uses `RegressionTestBase<TResult>` with three test methods: `Series()`, `Buffer()`, `Stream()`. Executed via `[TestCategory("Regression")]` in isolated test runs.

## Implementation Scope & Phasing

### Initial Scope (Phase 1)

The streaming framework targets comprehensive coverage across the library's indicator catalog. Initial implementation focuses on establishing patterns and infrastructure through systematic rollout.

### Priority 4 Enhancements (Post-Initial Coverage)

After achieving comprehensive BufferList and StreamHub coverage, the following enhancements address performance optimizations and feature requests:

- **ZigZag StreamHub optimization** (P4.1): Refactor from O(n) Series recalculation to incremental pivot-based updates
- **QuoteHub self-healing** (P4.2): Enable intra-period quote updates for live tick aggregation scenarios
- **ADX DMI output** (P4.3): Add +DI/-DI properties to ADX results across all implementation styles

### Target Coverage

- **Total indicators with Series implementations**: ~85 (80 implementable + 5 deferred to v2)
- **BufferList target**: 50 indicators requiring streaming support (excludes 5 deferred)
- **StreamHub target**: 47 indicators requiring streaming support (excludes 5 deferred)  
- **Implementation tasks**: 97 (T001-T107: 50 BufferList + 47 StreamHub, excluding deferred)
- **Supporting tasks**: 20 (D001-D007 + T108-T109 documentation, Q001-Q006 quality gates, T110-T124 compliance)
- **Total tasks**: 117 (97 implementation + 20 supporting)

### Phasing Strategy

Implementation proceeds in alphabetical indicator groups to enable parallel development:

- **Phase 1a (A-D indicators)**: Tasks T001-T016 (BufferList) + T056-T069 (StreamHub)
- **Phase 1b (E-K indicators)**: Tasks T017-T028 (BufferList) + T070-T081 (StreamHub)
- **Phase 1c (M-R indicators)**: Tasks T029-T041 (BufferList) + T082-T093 (StreamHub)
- **Phase 1d (S-Z indicators)**: Tasks T042-T055 (BufferList) + T094-T107 (StreamHub)

Each phase can proceed independently since indicators don't have inter-dependencies (with noted exceptions like ChaikinOsc depending on ADL, which are handled via composition patterns).

### Key Indicators for Validation

The following indicators serve as reference implementations for pattern validation:

- **SMA, EMA, RSI**: Basic calculation patterns (already complete)
- **MACD, Bollinger Bands**: Multi-series output patterns (already complete)
- **Alligator**: Offset-based calculations (T001, in PR #1497)
- **AtrStop**: Nested indicator composition (T003, in PR #1497)
- **Correlation**: Dual-series input pattern (T012, in PR #1499)

### Priority 4 User Stories (Performance & Enhancements)

**P4.1: ZigZag StreamHub Performance Optimization** (Issue #1692)

As a developer using ZigZag in real-time scenarios, I want the StreamHub implementation to use incremental pivot-based updates instead of recursive full Series recalculation, so that per-tick latency remains acceptable (<5ms mean) for live trading applications.

*Acceptance criteria*:
- ZigZag StreamHub avoids O(n) Series recalculation on each quote
- Uses incremental pivot detection and cache replay for rollback scenarios
- Maintains mathematical parity with Series implementation
- Achieves <5ms mean latency, <10ms p95 latency per NFR-001

**P4.2: QuoteHub Self-Healing for Live Tick Aggregation** (Issue #1585)

As a developer aggregating live ticks into period-based quotes (e.g., 1-minute bars), I want QuoteHub to support updating existing quotes without causing index out of range exceptions, so that I can handle intra-period quote updates during bar formation.

*Acceptance criteria*:
- QuoteHub supports updating the most recent quote (e.g., updating Close/High/Low as ticks arrive)
- No index out of range exceptions when quote properties change
- Subscribed indicators recalculate affected results correctly
- Documentation clarifies update vs insert semantics and limitations

**P4.3: ADX DMI Output Enhancement** (Issue #1262)

As a developer using ADX for trend analysis, I want DMI (Directional Movement Indicator) values included in ADX results, so that I can analyze directional movement alongside ADX without calculating separately.

*Acceptance criteria*:
- ADX result includes +DI and -DI output properties
- Values match established DMI calculation formulas
- Documentation clarifies DMI vs ADX distinctions and usage
- Applies to Series, BufferList, and StreamHub implementations
- Migration guide documents new properties for existing users

---

## Review & acceptance checklist

### Content quality

- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

### Requirement completeness

- [x] No [NEEDS CLARIFICATION] markers remain
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified
- [x] Regression testing requirements incorporated (NFR-007, terminology updated)

### Quality validation checklists

**Purpose**: Validate requirement quality and completeness before implementation, ensuring constitution compliance and instruction file adherence.

Review requirements against these simplified checklists:

- **BufferList implementations**: [checklists/buffer-list.md](checklists/buffer-list.md) — 15 essential validation items
- **StreamHub implementations**: [checklists/stream-hub.md](checklists/stream-hub.md) — 18 essential validation items

These validate requirements for:

- Constitution principle compliance (mathematical precision, performance, validation, testing, documentation)
- Instruction file adherence (base classes, interfaces, test patterns, utilities)  
- Essential quality gates (clarity, completeness, consistency, verifiability)

**Note**: Checklists validate requirement quality, not implementation completeness. They ensure specifications are well-written and comprehensive before coding begins.

---

## Execution status

- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities marked
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [x] Review checklist passed
- [x] Regression testing requirements integrated (October 9, 2025)

---
Last updated: October 13, 2025
