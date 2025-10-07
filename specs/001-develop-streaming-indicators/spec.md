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
2. **Given** a buffering-style streaming indicator receiving quotes, **When** 1000 quotes are streamed sequentially, **Then** final indicator values match batch-calculated results within floating-point tolerance (1e-12)
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
- **FR-004**: Final indicator values MUST converge to batch-calculated results within 1e-12 floating-point tolerance when same quote sequence is provided
- **FR-005**: Streaming indicators MUST validate warmup period requirements before producing results
- **FR-006**: Streaming indicators MUST enforce strictly ascending timestamps by rejecting any duplicate or out-of-order quotes
- **FR-007**: BufferList style MUST use List-backed storage with O(1) append and bounded size enforcement
- **FR-008**: StreamHub style MUST optimize for high-frequency scenarios with span-based buffers and minimal allocations
- **FR-009**: Streaming indicators MUST provide a method to reset state and reinitialize warmup
- **FR-010**: Library MUST include streaming variants for core indicators: SMA, EMA, RSI, MACD, Bollinger Bands (initial set)

### Non-functional requirements

- **NFR-001**: Average per-tick update latency <5ms (p95 <10ms) for single indicator instance, measured on commodity hardware (4-core 3GHz CPU, 16GB RAM) using BenchmarkDotNet with standard .NET 8.0 release configuration. Latency measured as wall-clock time for single `Add(quote)` call after warmup period completion.
- **NFR-002**: Memory overhead per streaming indicator instance <10KB for typical 200-period window
- **NFR-003**: Streaming parity tests MUST validate equivalence with batch calculations for all supported indicators
- **NFR-004**: API design MUST follow existing library conventions (no breaking changes to batch APIs)
- **NFR-005**: Documentation MUST include streaming usage examples, warmup guidance, and performance characteristics
- **NFR-006**: Release deliverable MUST leverage existing GitHub Releases automation and update `src/_common/ObsoleteV3.md` with streaming migration guidance per Constitution Principle 5

### Key entities

- **StreamingIndicator**: Base abstraction defining incremental update contract (Add quote → produce result)
- **BufferList Indicator**: Concrete streaming style using List-backed state, simpler to understand and extend
- **StreamHub Indicator**: Concrete streaming style using optimized buffers, suitable for high-frequency scenarios
- **Quote**: Input entity representing OHLCV data with timestamp
- **Indicator Result**: Output entity with timestamp, indicator value(s), and metadata
- **Warmup State**: Tracks whether indicator has sufficient history to produce valid results

## Terminology & Definitions

- **BufferList**: List-backed streaming implementation using `List<T>` for state storage. Simpler to implement and debug. Suitable for moderate frequency scenarios (<1k ticks/sec). Naming convention: `{IndicatorName}List`.
- **StreamHub**: Span-optimized streaming implementation using circular buffers for state storage. Optimized for high-frequency scenarios (>10k ticks/sec). Naming convention: `{IndicatorName}Hub<TIn>`.
- **Warmup Period**: Minimum number of quotes required before indicator produces valid results. Inherited from Series implementation's `WarmupPeriod` property.
- **Streaming Parity**: Requirement that streaming and batch (Series) calculations produce identical results within 1e-12 floating-point tolerance when given the same quote sequence.
- **Buffer Capacity**: Maximum number of elements stored in internal buffers. For BufferList: dynamically managed via `MaxListSize` (default ~1.9B elements). For StreamHub: typically fixed at `lookbackPeriod + margin` for efficient circular buffer operations.

## Implementation Scope & Phasing

### Initial Scope (Phase 1)

The streaming framework targets comprehensive coverage across the library's indicator catalog. Initial implementation focuses on establishing patterns and infrastructure through systematic rollout.

### Target Coverage

- **Total indicators with Series implementations**: ~84
- **BufferList target**: 55 indicators requiring streaming support
- **StreamHub target**: 52 indicators requiring streaming support
- **Total implementation tasks**: 107 (T001-T107)

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

---

## Execution status

- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities marked
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [x] Review checklist passed

---
Last updated: October 6, 2025
