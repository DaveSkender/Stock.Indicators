# Stock Indicators project principles

Six core principles govern engineering decisions ([Discussion #648](https://github.com/DaveSkender/Stock.Indicators/discussions/648)).

## Core principles

### 1. Mathematical precision (NON‑NEGOTIABLE)

All indicators MUST be mathematically correct, deterministic, and reproducible across net8.0, net9.0, net10.0.

**Rules:**

- No silent rounding or implicit unit conversions; use explicit casting when required.
- Default numeric type is double; decimal ONLY when price-sensitive rounding would materially change output.
- Results MUST match validated reference data (published examples, academic definitions, or vetted calculators).
- All implementation styles (Series/batch, BufferList/streaming, StreamHub/streaming) MUST produce bit-for-bit identical final values for deterministic calculations; Series serves as the validated baseline and streaming implementations must match it precisely.
- Any new indicator requires a written spec (math definition + parameter constraints) before implementation.
- Breaking mathematical behavior changes require a MAJOR library version bump and release note callout.

**Formula sourcing hierarchy:**

1. **Primary**: Manual calculation spreadsheets in `tests/indicators/` (all implementations MUST match exactly)
2. **Authoritative**: Original creator publications, expert textbooks, reputable institutional specs, cited Wikipedia
3. **Acceptable**: Third-party implementations citing authoritative sources, community-validated calculators
4. **Prohibited**: TradingView ([#801](https://github.com/DaveSkender/Stock.Indicators/discussions/801)), uncited calculators, sources without formula references

**Documentation requirements:**

`docs/_indicators/*.md` SHOULD link to creator's original publication and reputable secondary sources.

**NaN handling:**

Use non-nullable `double` with IEEE 754 NaN propagation:

- **Internal calculations**: Use `double.NaN` to represent undefined/incalculable values; allow natural propagation through operations
- **Division by zero**: MUST guard variable denominators with ternary checks (e.g., `denom != 0 ? num / denom : double.NaN`) to prevent Infinity; choose appropriate fallback (NaN, 0, or null) based on mathematical meaning
- **Result boundaries**: Convert NaN to `null` via `.NaN2Null()` ONLY when returning final results to users
- **Input validation**: Never reject NaN inputs; allow them to flow through calculations naturally
- **State initialization**: Use `double.NaN` for uninitialized state (not 0 or -1)

See [`src/_common/README.md#nan-handling-policy`](../src/_common/README.md#nan-handling-policy).

**Reputation criteria** ([#1024](https://github.com/DaveSkender/Stock.Indicators/discussions/1024)):

Indicators MUST be created by recognized experts, published in established venues, and time-tested in the financial industry.

### 2. Performance first (CRITICAL)

Low allocation, cache-friendly, single-pass O(n) computations.

**Rules:**

- No per-iteration heap allocations in hot loops; avoid LINQ, prefer `for` loops and `Span<T>`/`ReadOnlySpan<T>`
- Benchmarks MUST accompany algorithmic changes; regressions >2% mean or >5% p95 require justification or revert
- Prove optimizations via BenchmarkDotNet before merging
- Memory growth bounded by input length; no unbounded collections
- Streaming MUST not exceed batch complexity or add >5% overhead

### 3. Comprehensive validation

**Rules:**

- Guard against null inputs, empty sequences, insufficient history with clear `ArgumentException` messages
- Enforce and document parameter constraints (min periods, non-negative lengths)
- Streaming state MUST NOT leak or mutate shared buffers
- Document invariants in XML comments and tests
- Fail fast before allocating result buffers

### 4. Test‑driven quality

**Rules:**

- New indicators: write unit + edge + streaming parity tests before implementation (TDD)
- Bug fixes MUST add regression test proving defect and fix
- Cover public surface behavior (parameters, defaults, warmup); update tests when changed
- Performance baselines updated only when intentional
- CI runs unit, integration, performance benchmarks before release

### 5. Documentation excellence

**Rules:**

- Every public type/member has XML docs (`inheritdoc` for inherited semantics)
- Update `docs/_indicators/*.md` when parameters, formulas, warmup, or examples change
- Release notes enumerate breaking changes with migration guidance
- Examples MUST compile and reflect current API

### 6. Scope & stewardship

Source: [Discussion #648](https://github.com/DaveSkender/Stock.Indicators/discussions/648)

**Rules:**

- **Ease of use**: Clarity over convenience; zero hidden magic
- **Unopinionated**: Implement reputable formulas without reinterpretation
- **Single responsibility**: quotes + parameters → results; no signals, data fetching, trading logic, or storage
- **Encapsulation**: No external service calls or third-party packages
- **Broad instruments**: Support equities, commodities, forex, crypto (extreme scales, fractional volume)
- **Simplicity**: Decline features diluting core purpose
- **Community**: Reviewable, traceable, standards-aligned; fast bug/security resolution

## Additional constraints

### Performance & compatibility

- Targets: net10.0, net9.0, net8.0 (all build & pass tests)
- Complexity: Single-pass O(n) (justify exceptions in PR)
- Warmup: Provide deterministic `WarmupPeriod` helper or documented rule
- Precision: `double` for speed; `decimal` only when rounding affects financial correctness (>0.5 tick at 4 decimals)
- Allocation: Result list + minimal buffers; no per-step `List<T>` or LINQ in loops
- Thread safety: Stateless calcs thread-safe; streaming hubs isolate instance state (no static mutable fields)
- Backward compatibility: Renaming public members or altering defaults requires MAJOR version bump

### Error conventions

- `ArgumentOutOfRangeException` for invalid numeric ranges
- `ArgumentException` for semantic misuse
- Never swallow exceptions; wrap only to add context
- Messages include parameter name and offending value

## Pull request requirements

- Title: Conventional Commits format; link spec/issue
- Resolve all analyzer warnings or suppress with justification
- New/changed indicators: warmup guidance, parameter constraints, example snippet
- Performance changes: benchmark delta summary (mean, alloc)
- Streaming indicators: prove batch vs streaming parity in tests

## Quality gates

- Build: zero warnings
- Tests: all pass; new tests cover modified logic
- Performance: no increase >2% mean for unchanged indicators
- Docs: updated for behavior changes

## Governance

- **Authority**: Supersedes ad-hoc conventions
- **Review**: Quarterly (Jan/Apr/Jul/Oct); deviations >1 release require escalation
- **Enforcement**: PR reviewers cite specific principles; violations require immediate follow-up

---
Last updated: December 30, 2025
