<!--
Sync Impact Report
- Version change: 1.2.1 → 1.2.2 (PATCH: clarified that all implementation styles must match precisely)
- Modified principles: Principle 1: Mathematical Precision (NON‑NEGOTIABLE) - expanded streaming/batch parity rule to include all three implementation styles (Series, BufferList, StreamHub)
- Added sections: None
- Removed sections: None
- Templates requiring updates:
	- .specify/templates/plan-template.md (✅ no changes needed)
	- .specify/templates/spec-template.md (✅ no changes needed)
	- .specify/templates/tasks-template.md (✅ no changes needed)
	- specs/001-develop-streaming-indicators/spec.md (✅ already enforces three-way parity)
	- specs/001-develop-streaming-indicators/plan.md (✅ already enforces deterministic equality across styles)
	- specs/001-develop-streaming-indicators/tasks.md (✅ already references deterministic checklists)
	- specs/001-develop-streaming-indicators/checklists/buffer-list.md (✅ enforces Series baseline parity)
	- specs/001-develop-streaming-indicators/checklists/stream-hub.md (✅ enforces Series baseline parity)
- Follow-up TODOs: None
- Rationale: The previous phrasing "streaming and batch paths" was ambiguous about which implementation served as the baseline. The library has three implementation styles (Series/batch, BufferList/streaming, StreamHub/streaming), and Series is the validated baseline. All streaming implementations (BufferList and StreamHub) must match the Series results precisely—not just match each other. This clarification establishes the Series style as the mathematical source of truth for parity testing.
-->

# Stock Indicators Project Constitution

## Core Principles

### 1. Mathematical Precision (NON‑NEGOTIABLE)

All indicator calculations MUST be mathematically correct, deterministic, and reproducible across
supported target frameworks (.NET 8 & .NET 9).

Rules:

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

Examples:

- ConnorsRSI includes links to [the guidebook PDF](https://alvarezquanttrading.com/wp-content/uploads/2016/05/ConnorsRSIGuidebook.pdf)
- Parabolic SAR links to [Wikipedia](https://en.wikipedia.org/wiki/Parabolic_SAR) with proper citations

**Reputation Criteria:**

Indicators added to the library MUST meet reputation criteria ([Discussion #1024](https://github.com/DaveSkender/Stock.Indicators/discussions/1024)):

- Created by recognized experts in technical analysis or quantitative finance
- Published in established books, periodicals, or academic journals
- Time-tested with documented usage in the financial industry

### 2. Performance First

The library prioritizes low allocation, cache-friendly, single-pass (O(n)) computations.

Rules:

- No per-iteration heap allocations in hot loops (avoid LINQ in inner loops; prefer for loops and Span/ReadOnlySpan when possible).
- Performance benchmarks MUST accompany substantial algorithmic changes; regressions >2% mean or >5% p95 MUST be justified or reverted.
- Avoid premature micro-optimizations; prove via BenchmarkDotNet before merging.
- Memory growth MUST be bounded by input length; no unbounded collections or hidden caches.
- Streaming indicators MUST not exceed batch baseline complexity or add >5% overhead steady-state.

### 3. Comprehensive Validation

Input and state validation prevents undefined behavior.

Rules:

- Public APIs MUST guard against null inputs, empty sequences, and insufficient history length with clear ArgumentException messages.
- Parameter constraints (min periods, non-negative lengths, etc.) MUST be enforced uniformly and documented.
- Streaming state MUST NOT leak or mutate shared buffers across independent consumers.
- Invariants (e.g., WarmupPeriod >= LookbackPeriod) documented in XML comments and tests.
- Fail fast: detect invalid conditions before allocating large result buffers.

### 4. Test‑Driven Quality

Quality is enforced through disciplined test-first development.

Rules:

- New indicators: write unit + edge + streaming parity tests before implementation (initially failing).
- Bug fixes MUST add a regression test proving the defect then verifying the fix.
- Public surface behavior (parameters, defaults, warmup length) MUST be covered; changes require test updates.
- Performance baselines updated only when intentional; accidental deltas investigated.
- CI MUST run unit, integration (where applicable), and performance smoke benchmarks before release tagging.

### 5. Documentation Excellence & Transparency

Documentation is a first-class deliverable.

Rules:

- Every public type/member has XML docs (use `inheritdoc` when inheriting unchanged semantics).
- Indicator doc pages (docs/_indicators/*.md) MUST be updated when parameters, formulas, warmup, or examples change.
- Release notes MUST enumerate breaking changes and notable deprecations with migration guidance.
- Examples MUST compile and reflect current API; stale examples are treated as defects.
- Governance, versioning policy, and amendment history remain visible in this constitution.

## Additional Constraints

### Performance & Compatibility Standards

- Supported Targets: net8.0, net9.0 (both must build & pass tests).
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

## Development Workflow & Quality Gates

### Spec-Driven Flow

1. Draft feature spec (spec-template) capturing WHAT, not HOW.
2. Implementation plan (/plan) enforces Constitution Check before design tasks proceed.
3. Task generation (/tasks) produces ordered TDD-first execution list.
4. Implementation strictly follows Red → Green → Refactor loop.
5. Validation: unit tests, streaming parity, performance smoke, docs sync.

### Pull Request Requirements

- PR title uses Conventional Commits; link to spec or issue.
- All analyzer warnings resolved or explicitly suppressed with justification.
- Added/changed indicator: include warmup guidance, parameter constraints, example snippet.
- Performance-sensitive changes: attach benchmark delta summary (mean, alloc).
- Streaming indicators: prove batch vs streaming value parity for representative sample in tests.

### Quality Gates (Must Pass)

- Build succeeds with zero warnings treated as errors.
- Unit test suite green; new tests cover modified logic branches.
- No net increase in benchmark mean >2% for unchanged indicators.
- Docs pages updated for any public behavior change.
- Constitution Check shows zero unapproved violations (Complexity Tracking used if temporary deviation).

## Governance

### Authority & Scope

This constitution governs engineering decisions for the Stock Indicators project. Where conflicts arise,
its directives supersede ad-hoc conventions. Exceptions require explicit, time-bounded approval recorded
in PR description + Complexity Tracking section of the plan.

### Amendment Procedure

1. Open an Issue or Discussion proposing change (motivation, impact, classification: patch/minor/major).
2. Draft PR updating constitution + any affected templates (plan/spec/tasks) + Sync Impact Report.
3. Require at least one maintainer approval; major changes require two.
4. Upon merge: increment version per rules; announce in release notes if impactful to contributors.

### Versioning Policy (Semantic)

- MAJOR: Remove or redefine a principle; change governance process; invalidate existing gating rules.
- MINOR: Add a new principle/section or materially expand requirements.
- PATCH: Clarify wording, fix typos, tighten language without changing intent.

### Compliance & Review Cadence

- Automated Constitution Check executed during /plan ensures early detection.
- Quarterly review (January/April/July/October) to assess relevance of performance targets & principles.
- Any deviation longer than one release cycle MUST be escalated or removed.

### Enforcement

- PR reviewers MUST cite specific principle(s) when requesting changes.
- Merging code that violates a principle without documented exception is grounds for immediate follow-up issue.

### 6. Scope & Stewardship

Defines the boundaries and community ethos ensuring longevity and focused value delivery.

Rules:

- Ease of Use: Public APIs favor clarity over convenience abstractions; zero hidden magic.
- Unopinionated Implementation: Indicators implement reputable, published formulas without reinterpretation or secret tweaks.
- Single Responsibility: Library ONLY transforms historical quotes + parameters → indicator results; no signal engines, data fetchers, trading logic, or storage layers.
- Encapsulation & Purity: No runtime calls to external services or third‑party packages; caller owns data acquisition and persistence.
- Broad Instrument Support: Maintain correctness for equities, commodities, forex, crypto (including extreme price scales & fractional volumes).
- Simplicity over Feature Creep: Decline features that dilute core purpose (“Could we?” is not “Should we?”)—reassess via governance if disputed.
- Community & Openness: Keep contributions reviewable, traceable, and standards-aligned; fast, transparent handling of bug and security reports.

Rationale: Consolidates long‑standing design tenets (Discussion #648) that were implicit but not yet enforceable as a formal principle.

**Version**: 1.2.2 | **Ratified**: 2025-10-02 | **Last Amended**: 2025-10-07
