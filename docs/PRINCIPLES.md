# Stock Indicators Project Principles

This document outlines the core principles that govern engineering decisions for the Stock Indicators project.

## Core Principles

### 1. Mathematical Precision (NON‑NEGOTIABLE)

All indicator calculations MUST be mathematically correct, deterministic, and reproducible across
supported target frameworks (.NET 8, .NET 9, and .NET 10).

**Rules:**

- No silent rounding or implicit unit conversions; use explicit casting when required.
- Default numeric type is double; decimal ONLY when price-sensitive rounding would materially change output.
- Results MUST match validated reference data (published examples, academic definitions, or vetted calculators).
- All implementation styles (Series/batch, BufferList/streaming, StreamHub/streaming) MUST produce bit-for-bit identical final values for deterministic calculations; Series serves as the validated baseline and streaming implementations must match it precisely.
- Any new indicator requires a written spec (math definition + parameter constraints) before implementation.
- Breaking mathematical behavior changes require a MAJOR library version bump and release note callout.

**Formula Sourcing Hierarchy:**

When implementing or validating indicator formulas, developers MUST follow this sourcing hierarchy:

1. **Primary Source of Truth**: Manual calculation spreadsheets in `tests/indicators/` directory serve as the project's validated ground truth. All implementations MUST match these reference calculations exactly.

2. **Authoritative Sources** (for initial implementation and documentation):
   - Original publications by indicator creators (books, white papers, academic papers)
   - Established technical analysis textbooks by recognized experts
   - Published specifications from reputable financial institutions or exchanges
   - Wikipedia entries with proper citations to primary sources

3. **Acceptable Validation Sources**:
   - Third-party implementations that cite authoritative sources
   - Community-validated calculators with verifiable methodology (e.g., Quantified Strategies)
   - Published analysis from recognized quantitative trading firms

4. **Prohibited Sources**:
   - TradingView and similar charting platforms ([Discussion #801](https://github.com/DaveSkender/Stock.Indicators/discussions/801): "TradingView is usually not a good source of truth")
   - Uncited online calculators or implementations without verifiable methodology
   - Sources that do not reference original formulas or established publications

**Documentation Requirements:**

Indicator documentation pages (`docs/_indicators/*.md`) SHOULD include links to authoritative sources:

- Link to the creator's original publication or specification when available
- Link to reputable secondary sources (e.g., Wikipedia with citations, established textbooks)
- Creator attribution with time period when known

**NaN/Infinity Handling Policy:**

The library uses non-nullable `double` types internally for performance, with intentional IEEE 754 NaN/Infinity propagation:

- **Internal calculations**: Use `double.NaN` to represent undefined/incalculable values; allow natural propagation through operations
- **Division by zero**: MUST guard variable denominators with ternary checks (e.g., `denom != 0 ? num / denom : double.NaN`) to prevent Infinity; choose appropriate fallback (NaN, 0, or null) based on mathematical meaning
- **Result boundaries**: Convert NaN to `null` via `.NaN2Null()` ONLY when returning final results to users
- **Input validation**: Never reject NaN inputs; allow them to flow through calculations naturally
- **State initialization**: Use `double.NaN` for uninitialized state rather than sentinel values (0, -1)

Rationale: This approach achieves significant performance gains from non-nullable types while maintaining mathematical correctness per IEEE 754 standard. NaN is the mathematically correct representation for undefined values.

Detailed implementation guidance: [`src/_common/README.md#nan-handling-policy`](../src/_common/README.md#nan-handling-policy)

**Reputation Criteria:**

Indicators added to the library MUST meet reputation criteria ([Discussion #1024](https://github.com/DaveSkender/Stock.Indicators/discussions/1024)):

- Created by recognized experts in technical analysis or quantitative finance
- Published in established books, periodicals, or academic journals
- Time-tested with documented usage in the financial industry

### 2. Performance First

The library prioritizes low allocation, cache-friendly, single-pass (O(n)) computations.

**Rules:**

- No per-iteration heap allocations in hot loops (avoid LINQ in inner loops; prefer for loops and Span/ReadOnlySpan when possible).
- Performance benchmarks MUST accompany substantial algorithmic changes; regressions >2% mean or >5% p95 MUST be justified or reverted.
- Avoid premature micro-optimizations; prove via BenchmarkDotNet before merging.
- Memory growth MUST be bounded by input length; no unbounded collections or hidden caches.
- Streaming indicators MUST not exceed batch baseline complexity or add >5% overhead steady-state.

### 3. Comprehensive Validation

Input and state validation prevents undefined behavior.

**Rules:**

- Public APIs MUST guard against null inputs, empty sequences, and insufficient history length with clear ArgumentException messages.
- Parameter constraints (min periods, non-negative lengths, etc.) MUST be enforced uniformly and documented.
- Streaming state MUST NOT leak or mutate shared buffers across independent consumers.
- Invariants (e.g., WarmupPeriod >= LookbackPeriod) documented in XML comments and tests.
- Fail fast: detect invalid conditions before allocating large result buffers.

### 4. Test‑Driven Quality

Quality is enforced through disciplined test-first development.

**Rules:**

- New indicators: write unit + edge + streaming parity tests before implementation (initially failing).
- Bug fixes MUST add a regression test proving the defect then verifying the fix.
- Public surface behavior (parameters, defaults, warmup length) MUST be covered; changes require test updates.
- Performance baselines updated only when intentional; accidental deltas investigated.
- CI MUST run unit, integration (where applicable), and performance smoke benchmarks before release tagging.

### 5. Documentation Excellence & Transparency

Documentation is a first-class deliverable.

**Rules:**

- Every public type/member has XML docs (use `inheritdoc` when inheriting unchanged semantics).
- Indicator doc pages (docs/_indicators/*.md) MUST be updated when parameters, formulas, warmup, or examples change.
- Release notes MUST enumerate breaking changes and notable deprecations with migration guidance.
- Examples MUST compile and reflect current API; stale examples are treated as defects.
- Governance, versioning policy, and amendment history remain visible in project documentation.

### 6. Scope & Stewardship

Defines the boundaries and community ethos ensuring longevity and focused value delivery.

**Rules:**

- Ease of Use: Public APIs favor clarity over convenience abstractions; zero hidden magic.
- Unopinionated Implementation: Indicators implement reputable, published formulas without reinterpretation or secret tweaks.
- Single Responsibility: Library ONLY transforms historical quotes + parameters → indicator results; no signal engines, data fetchers, trading logic, or storage layers.
- Encapsulation & Purity: No runtime calls to external services or third‑party packages; caller owns data acquisition and persistence.
- Broad Instrument Support: Maintain correctness for equities, commodities, forex, crypto (including extreme price scales & fractional volumes).
- Simplicity over Feature Creep: Decline features that dilute core purpose ("Could we?" is not "Should we?")—reassess via governance if disputed.
- Community & Openness: Keep contributions reviewable, traceable, and standards-aligned; fast, transparent handling of bug and security reports.

Rationale: Consolidates long‑standing design tenets (Discussion #648) that were implicit but not yet enforceable as a formal principle.

## Additional Constraints

### Performance & Compatibility Standards

- Supported Targets: net10.0, net9.0, net8.0 (all must build & pass tests).
- Baseline Complexity: Single pass O(n) unless mathematically impossible (justify exceptions in PR).
- Warmup Guidance: Provide a deterministic WarmupPeriod helper or documented rule for each indicator.
- Precision Policy: Use double for speed; escalate to decimal only when rounding materially affects financial correctness (> 0.5 tick at 4-decimal pricing).
- Allocation Budget: Result list + minimal working buffers only; no temporary per-step Lists or LINQ projections inside loops.
- Thread Safety: Stateless calculations are thread-safe; streaming hubs must isolate instance state—do not use static mutable fields.
- Backward Compatibility: Renaming public members or altering default parameter values requires MAJOR version bump.

### Error & Exception Conventions

- Use ArgumentOutOfRangeException for invalid numeric parameter ranges.
- Use ArgumentException for semantic misuse (e.g., insufficient history).
- Never swallow exceptions; wrap only to add domain context.
- Messages MUST include parameter name and offending value when relevant.

## Pull Request Requirements

- PR title uses Conventional Commits; link to spec or issue.
- All analyzer warnings resolved or explicitly suppressed with justification.
- Added/changed indicator: include warmup guidance, parameter constraints, example snippet.
- Performance-sensitive changes: attach benchmark delta summary (mean, alloc).
- Streaming indicators: prove batch vs streaming value parity for representative sample in tests.

## Quality Gates (Must Pass)

- Build succeeds with zero warnings treated as errors.
- Unit test suite green; new tests cover modified logic branches.
- No net increase in benchmark mean >2% for unchanged indicators.
- Docs pages updated for any public behavior change.

## Governance

### Authority & Scope

These principles govern engineering decisions for the Stock Indicators project. Where conflicts arise,
these directives supersede ad-hoc conventions.

### Compliance & Review Cadence

- Quarterly review (January/April/July/October) to assess relevance of performance targets & principles.
- Any deviation longer than one release cycle MUST be escalated or removed.

### Enforcement

- PR reviewers MUST cite specific principle(s) when requesting changes.
- Merging code that violates a principle without documented exception is grounds for immediate follow-up issue.

---

**Based on**: Constitution version 1.3.0 | Originally ratified: 2025-10-02 | Last amended: 2025-11-02  
**Document created**: 2025-12-24 (migrated from .specify/memory/constitution.md)

---
Last updated: December 24, 2025
