# Repository AI configuration and guardrails refactor plan

> **Version**: 2.0 (December 30, 2025)
> **Status**: Comprehensive plan ready for implementation

## Executive summary

This plan provides a complete roadmap to modernize the Stock Indicators repository's AI guidance ecosystem. The goal is to migrate from a fragmented set of custom Copilot instructions, agent profiles, and instruction files to the industry-standard **Agent Skills** and **AGENTS.md** frameworks, ensuring compatibility with GitHub Copilot, Claude, Cursor, Devin, and other major AI coding assistants.

### Key decisions

| Decision | Rationale |
|----------|-----------|
| Use `.github/skills/` for domain expertise | Skills are portable, auto-loaded when relevant, and follow the open Agent Skills specification |
| Keep root `AGENTS.md` for build/test/setup | AGENTS.md provides universal compatibility across 60k+ repositories and all major coding agents |
| Preserve `src/agents.md` and `tests/agents.md` | These contain critical constitutional rules (formula protection, test naming) that must remain proximate to source |
| Eliminate most `.github/instructions/` files | Content migrates to skills; path-specific instructions only for narrow glob-based cases (e.g., codacy.instructions.md) |
| Reduce custom agent profiles to 2-3 maximum | Most orchestration moves to skills; keep agents only for multi-skill coordination |

---

## Part 1: Current state assessment

### 1.1 Existing file inventory

| Location | File | Purpose | Lines | Action |
|----------|------|---------|-------|--------|
| `.github/` | `copilot-instructions.md` | Root Copilot instructions | 228 | **MIGRATE** to `AGENTS.md` + skills |
| `.github/instructions/` | `dotnet.instructions.md` | .NET coding standards | ~400 | **MIGRATE** to `AGENTS.md` and skills |
| `.github/instructions/` | `indicator-series.instructions.md` | Series indicator development | ~350 | **MIGRATE** to `indicator-series` skill |
| `.github/instructions/` | `indicator-buffer.instructions.md` | BufferList development | ~450 | **MIGRATE** to `indicator-buffer` skill |
| `.github/instructions/` | `indicator-stream.instructions.md` | StreamHub development | ~650 | **MIGRATE** to `indicator-stream` skill |
| `.github/instructions/` | `catalog.instructions.md` | Catalog entry conventions | ~200 | **MIGRATE** to `indicator-catalog` skill |
| `.github/instructions/` | `performance-testing.instructions.md` | BenchmarkDotNet guidelines | ~200 | **MIGRATE** to `performance-testing` skill |
| `.github/instructions/` | `code-completion.instructions.md` | Pre-commit checklist | ~400 | **MIGRATE** to `quality-gates` skill |
| `.github/instructions/` | `markdown.instructions.md` | Markdown authoring | ~500 | **KEEP** as path-specific instruction |
| `.github/instructions/` | `docs.instructions.md` | Jekyll docs site | ~100 | **KEEP** as path-specific instruction |
| `.github/instructions/` | `codacy.instructions.md` | Codacy MCP config | ~50 | **KEEP** as path-specific instruction |
| `.github/agents/` | `indicator-series.agent.md` | Series agent profile | 278 | **REMOVE** (content to skill) |
| `.github/agents/` | `indicator-buffer.agent.md` | Buffer agent profile | 315 | **REMOVE** (content to skill) |
| `.github/agents/` | `indicator-stream.agent.md` | Stream agent profile | 293 | **REMOVE** (content to skill) |
| `.github/agents/` | `performance.agent.md` | Performance agent | 327 | **SIMPLIFY** or merge with skill |
| `.github/agents/` | `streamhub-*.agent.md` | StreamHub sub-agents (4 files) | ~100 each | **CONSOLIDATE** into stream skill |
| `src/` | `agents.md` | Formula protection rules | 199 | **KEEP** as guardrail |
| `tests/` | `agents.md` | Test naming/precision rules | 298 | **KEEP** as guardrail |

**Current totals**:

- 10 instruction files (~3,300 lines)
- 8 custom agent profiles (~1,800 lines)
- 2 source-level agents.md (~500 lines)
- **Total: ~5,600 lines of AI guidance**

### 1.2 Official specifications reference

| Standard | URL | Version | Key requirements |
|----------|-----|---------|------------------|
| **Agent Skills** | [agentskills.io/specification](https://agentskills.io/specification) | 1.0 | YAML frontmatter with `name` (lowercase, 1-64 chars) and `description` (1-1024 chars); skill directory must match name; keep SKILL.md under 500 lines |
| **AGENTS.md** | [agents.md](https://agents.md) | 1.0 | Plain markdown, no required fields; sections for setup, build, test, code style, security; closest file in directory tree takes precedence |
| **GitHub Copilot Custom Instructions** | [docs.github.com](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot) | 2025 | Supports `copilot-instructions.md`, `*.instructions.md` with `applyTo` globs, and `AGENTS.md` files |
| **Example Skills Repository** | [github.com/anthropics/skills](https://github.com/anthropics/skills) | Active | Reference implementations for skill structure |

---

## Part 2: Indicator classification matrix

### 2.1 Implementation style dimensions

The library implements indicators across three styles, each with unique considerations:

| Style | File Pattern | Base Class | Count | Characteristics |
|-------|--------------|------------|-------|-----------------|
| **Series** | `*.StaticSeries.cs` | `IReadOnlyList<TResult>` | 85 | Batch processing, canonical mathematical reference |
| **BufferList** | `*.BufferList.cs` | `BufferList<TResult>` | 82 | Incremental processing, memory-efficient |
| **StreamHub** | `*.StreamHub.cs` | `ChainProvider/QuoteProvider/PairsProvider` | 83 | Real-time streaming, stateful, late-arrival handling |

### 2.2 I/O variant classification

**StreamHub provider patterns (determines input/output types):**

| Provider Base | Input | Output | Chainable | Count | Examples |
|---------------|-------|--------|-----------|-------|----------|
| `ChainProvider<IReusable, TResult : IReusable>` | Single value | Single value | Yes | ~45 | EMA, SMA, RSI, MACD, Trix, TSI |
| `ChainProvider<IQuote, TResult : IReusable>` | OHLCV quote | Single value | Yes | ~15 | ADX, ATR, Aroon, CCI, CMF, OBV |
| `QuoteProvider<IQuote, TResult : IQuote>` | OHLCV quote | OHLCV quote | Yes (quote) | 3 | QuoteHub, HeikinAshi, Renko |
| `PairsProvider<IReusable, TResult>` | Dual stream | Single value | Yes | 3 | Correlation, Beta, Prs |
| `StreamHub<TIn, TResult : ISeries>` | Various | Multi-value | No | ~20 | Alligator, AtrStop, Ichimoku, Keltner |

**BufferList interface patterns (determines Add method signatures):**

| Interface | Input Type | Count | Examples |
|-----------|------------|-------|----------|
| `IIncrementFromChain` | `IReusable` (single value) | 39 | SMA, EMA, RSI, MACD, Dema, Tema |
| `IIncrementFromQuote` | `IQuote` (OHLCV) | 38 | Stoch, Chandelier, VWAP, ATR, ADX |
| `IIncrementFromPairs` | Dual `IReusable` | 3 | Correlation, Beta, Prs |

### 2.3 Output type classification

| Interface | Description | Can Chain? | Examples |
|-----------|-------------|------------|----------|
| `IReusable` (extends `ISeries`) | Single chainable `Value` property | Yes | EMA, SMA, RSI, ADX, MACD |
| `ISeries` only | Multi-value output, no single chainable value | No | Alligator, AtrStop, Keltner, Ichimoku |

### 2.4 Repainting indicators

Some indicators legitimately update prior values:

| Indicator | Behavior | Implementation Pattern |
|-----------|----------|------------------------|
| DPO | Future-dependent calculation | Lookback adjustment |
| Slope | Line repaint within window | `UpdateInternal()` |
| Pivots | Trend line repaint | Pivot confirmation delay |
| Fractal | Window-based confirmation | Forward data required |
| ZigZag | Last segment repaint | Trend reversal detection |
| VolatilityStop | Trailing stop repaint | New extrema arrival |

### 2.5 State complexity classification (for StreamHub)

| Complexity | RollbackState Required | Examples |
|------------|------------------------|----------|
| **Stateless** | No | Simple passthrough indicators |
| **Simple state** | Yes | EMA (single prior value), SMA (running sum) |
| **Rolling window** | Yes | Chandelier, Stoch, Donchian, Aroon (RollingWindowMax/Min) |
| **Wilder's smoothing** | Yes | RSI, ADX, ATR (gain/loss averages) |
| **Multi-EMA chains** | Yes | MACD, Trix, TSI, T3, DEMA, TEMA |
| **Anchor tracking** | Yes | ParabolicSar, AtrStop, SuperTrend (trend reversal) |
| **Dual-stream** | Yes | Correlation, Beta (synchronized caches) |

---

## Part 3: Target architecture

### 3.1 Skills taxonomy

Create `.github/skills/` with the following structure:

```text
.github/skills/
├── indicator-series/
│   ├── SKILL.md
│   └── references/
│       └── decision-tree.md
├── indicator-buffer/
│   ├── SKILL.md
│   └── references/
│       └── interface-selection.md
├── indicator-stream/
│   ├── SKILL.md
│   ├── references/
│   │   ├── provider-selection.md
│   │   ├── rollback-patterns.md
│   │   └── performance-patterns.md
│   └── scripts/
│       └── validate-hub.ps1
├── indicator-catalog/
│   └── SKILL.md
├── performance-testing/
│   ├── SKILL.md
│   └── references/
│       └── benchmark-patterns.md
├── quality-gates/
│   └── SKILL.md
└── testing-standards/
    └── SKILL.md
```

### 3.2 Skill specifications

#### indicator-series skill

```yaml
---
name: indicator-series
description: Implement Series-style batch indicators with mathematical precision. Use for new StaticSeries implementations, warmup period calculations, validation patterns, and test coverage. Series results are the canonical reference—all other styles must match exactly.
---
```

**Content structure** (< 500 lines):

1. Core patterns (warmup, validation, result initialization)
2. Decision tree for file structure
3. Reference implementations (EMA, SMA, ATRStop, Alligator)
4. Testing checklist (StaticSeriesTestBase inheritance, precision)
5. Links to `src/agents.md` for formula rules

#### indicator-buffer skill

```yaml
---
name: indicator-buffer
description: Implement BufferList incremental indicators with efficient state management. Use for IIncrementFromChain, IIncrementFromQuote, or IIncrementFromPairs implementations. Covers interface selection, constructor patterns, and BufferListTestBase testing requirements.
---
```

**Content structure**:

1. Interface selection decision tree
2. Constructor patterns (primary + chaining)
3. BufferListUtilities usage (Update, UpdateWithDequeue)
4. State management (tuples vs fields)
5. Series parity validation

#### indicator-stream skill

```yaml
---
name: indicator-stream
description: Implement StreamHub real-time indicators with O(1) performance. Use for ChainProvider, QuoteProvider, or PairsProvider implementations. Covers provider selection, RollbackState patterns, performance anti-patterns, and comprehensive testing with StreamHubTestBase.
---
```

**Content structure**:

1. Provider base class selection guide
2. I/O variant classification
3. RollbackState implementation patterns
4. Performance anti-patterns (O(n squared) vs O(1))
5. Testing interfaces (ITestChainObserver, ITestQuoteObserver, ITestPairsObserver)
6. Reference implementations (EMA, ADX, Correlation)

#### indicator-catalog skill

```yaml
---
name: indicator-catalog
description: Create and register indicator catalog entries for automation. Use for Catalog.cs files, CatalogListingBuilder patterns, parameter/result definitions, and PopulateCatalog registration.
---
```

#### performance-testing skill

```yaml
---
name: performance-testing
description: Benchmark indicator performance with BenchmarkDotNet. Use for Series/Buffer/Stream benchmarks, regression detection, and optimization patterns. Target 1.5x Series for StreamHub, 1.2x for BufferList.
---
```

#### quality-gates skill

```yaml
---
name: quality-gates
description: Pre-commit quality checklist for Stock Indicators development. Use before completing work to ensure clean build, passing tests, documentation updates, and migration bridge requirements.
---
```

#### testing-standards skill

```yaml
---
name: testing-standards
description: Testing conventions for Stock Indicators. Use for test naming (MethodName_StateUnderTest_ExpectedBehavior), FluentAssertions patterns, precision requirements, and test base class selection.
---
```

### 3.3 AGENTS.md structure

#### Root AGENTS.md

Create `AGENTS.md` at repository root (< 150 lines):

```markdown
# AGENTS.md

Instructions for AI coding agents working in the Stock Indicators repository.

## Project overview

Stock Indicators for .NET is a NuGet package providing 200+ technical analysis indicators. Multi-targets net8.0, net9.0, net10.0.

## Setup commands

- Install: `dotnet restore`
- Build: `dotnet build`
- Test: `dotnet test --settings tests/tests.unit.runsettings`
- Lint: `dotnet format && npx markdownlint-cli2`

## Code style

- PascalCase for public members, _camelCase for private fields
- Use `double` (not `decimal`) for calculations
- Guard division by zero: `denom != 0 ? num / denom : double.NaN`
- See `.editorconfig` for formatting rules

## Testing

- Inherit from `StaticSeriesTestBase`, `BufferListTestBase`, or `StreamHubTestBase`
- Test naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Use FluentAssertions: `result.Should().Be(expected)`
- Stream/Buffer must match Series results exactly

## Constitutional rules (NON-NEGOTIABLE)

1. **Never modify indicator formulas** without explicit authorization and source citations
2. **Maintain bit-for-bit parity** across Series, BufferList, and StreamHub implementations
3. **Series is canonical** - fix discrepancies in Buffer/Stream, not Series

See `src/agents.md` for formula protection rules.
See `tests/agents.md` for test naming and precision requirements.

## Skills available

- `indicator-series` - Batch indicator implementation
- `indicator-buffer` - Incremental BufferList development
- `indicator-stream` - Real-time StreamHub development
- `indicator-catalog` - Catalog entry conventions
- `performance-testing` - BenchmarkDotNet guidelines
- `quality-gates` - Pre-commit checklist
- `testing-standards` - Test conventions
```

#### Subfolder AGENTS.md files

**`src/AGENTS.md`** (brief, links to skills):

```markdown
# Source Code Instructions

See `src/agents.md` for formula protection rules (NON-NEGOTIABLE).

## Indicator development

Use skills:
- Series: `.github/skills/indicator-series/SKILL.md`
- Buffer: `.github/skills/indicator-buffer/SKILL.md`
- Stream: `.github/skills/indicator-stream/SKILL.md`
- Catalog: `.github/skills/indicator-catalog/SKILL.md`

## File organization

- `_common/` - Shared utilities, base classes
- `a-d/`, `e-k/`, `m-r/`, `s-z/` - Indicators (alphabetical)
```

**`tests/AGENTS.md`** (brief, links to skill):

```markdown
# Test Instructions

See `tests/agents.md` for test naming and precision requirements.

Use `.github/skills/testing-standards/SKILL.md` for conventions.

## Test base classes

- `StaticSeriesTestBase` - Series indicator tests
- `BufferListTestBase` - BufferList tests (REQUIRED, not TestBase)
- `StreamHubTestBase` - StreamHub tests
- `RegressionTestBase` - Baseline validation
```

### 3.4 Files to delete

After migration, remove:

```text
.github/copilot-instructions.md
.github/instructions/dotnet.instructions.md
.github/instructions/indicator-series.instructions.md
.github/instructions/indicator-buffer.instructions.md
.github/instructions/indicator-stream.instructions.md
.github/instructions/catalog.instructions.md
.github/instructions/performance-testing.instructions.md
.github/instructions/code-completion.instructions.md
.github/agents/indicator-series.agent.md
.github/agents/indicator-buffer.agent.md
.github/agents/indicator-stream.agent.md
.github/agents/performance.agent.md
.github/agents/streamhub-pairs.agent.md
.github/agents/streamhub-performance.agent.md
.github/agents/streamhub-state.agent.md
.github/agents/streamhub-testing.agent.md
```

### 3.5 Files to keep (path-specific instructions)

```text
.github/instructions/markdown.instructions.md    # applyTo: **/*.md
.github/instructions/docs.instructions.md         # applyTo: docs/**
.github/instructions/codacy.instructions.md       # applyTo: ** (MCP config)
src/agents.md                                     # Formula protection
tests/agents.md                                   # Test conventions
```

---

## Part 4: Implementation phases

### Phase 1: Foundation (Estimated: 2 hours)

| Task | Description | Verification |
|------|-------------|--------------|
| 1.1 | Create `.github/skills/` directory structure | Directory exists |
| 1.2 | Create root `AGENTS.md` | File exists, < 150 lines |
| 1.3 | Create `src/AGENTS.md` | File exists, links to src/agents.md |
| 1.4 | Create `tests/AGENTS.md` | File exists, links to tests/agents.md |

### Phase 2: Core skills (Estimated: 4 hours)

| Task | Description | Verification |
|------|-------------|--------------|
| 2.1 | Create `indicator-series` skill | SKILL.md < 500 lines, valid YAML |
| 2.2 | Create `indicator-buffer` skill | SKILL.md < 500 lines, valid YAML |
| 2.3 | Create `indicator-stream` skill | SKILL.md < 500 lines, valid YAML |
| 2.4 | Create `indicator-catalog` skill | SKILL.md valid |
| 2.5 | Create `performance-testing` skill | SKILL.md valid |
| 2.6 | Create `quality-gates` skill | SKILL.md valid |
| 2.7 | Create `testing-standards` skill | SKILL.md valid |

### Phase 3: Cleanup (Estimated: 1 hour)

| Task | Description | Verification |
|------|-------------|--------------|
| 3.1 | Delete obsolete instruction files | 8 files removed |
| 3.2 | Delete custom agent profiles | 8 files removed |
| 3.3 | Delete `copilot-instructions.md` | File removed |
| 3.4 | Update any remaining references | Grep shows no dangling refs |

### Phase 4: Validation (Estimated: 1 hour)

| Task | Description | Verification |
|------|-------------|--------------|
| 4.1 | Run `dotnet build` | Zero warnings |
| 4.2 | Run `dotnet test` | All tests pass |
| 4.3 | Run `npx markdownlint-cli2` | Zero errors |
| 4.4 | Test skill loading in VS Code | Skills appear in Copilot context |
| 4.5 | Verify AGENTS.md is recognized | Check in Copilot coding agent |

---

## Part 5: Detailed skill content templates

### 5.1 indicator-series SKILL.md template

````markdown
---
name: indicator-series
description: Implement Series-style batch indicators with mathematical precision. Use for new StaticSeries implementations, warmup period calculations, validation patterns, and test coverage. Series results are the canonical reference—all other styles must match exactly.
---

# Series Indicator Development

Series indicators process complete datasets and return results all at once. They are the foundation style that establishes mathematical correctness.

## File structure

- Implementation: `src/{category}/{Indicator}/{Indicator}.StaticSeries.cs`
- Test: `tests/indicators/{category}/{Indicator}/{Indicator}.StaticSeries.Tests.cs`
- Categories: a-d, e-k, m-r, s-z (alphabetical)

## Core pattern

```csharp
public static IReadOnlyList<TResult> ToIndicator(
    this IReadOnlyList<IQuote> quotes,
    int lookbackPeriods)
{
    // 1. Validate parameters
    ArgumentOutOfRangeException.ThrowIfLessThan(lookbackPeriods, 1);
    
    // 2. Initialize results
    int length = quotes.Count;
    List<TResult> results = new(length);
    
    // 3. Calculate warmup period results (null values)
    // 4. Calculate main results
    // 5. Return results
    return results;
}
```

## Warmup period calculation

| Indicator Type | Formula | Example |
|----------------|---------|---------|
| Simple MA | `lookback - 1` | SMA(20) -> 19 warmup |
| Exponential | `lookback` | EMA(12) -> 12 warmup |
| Multi-stage | Sum of stages | MACD(12,26,9) -> 34 warmup |

## Validation patterns

- Null quotes: `ArgumentNullException.ThrowIfNull(quotes)`
- Invalid range: `ArgumentOutOfRangeException.ThrowIfLessThan(period, 1)`
- Semantic error: `ArgumentException` for logical constraints

## Testing requirements

1. Inherit from `StaticSeriesTestBase`
2. Implement Standard, Boundary, BadData, InsufficientData tests
3. Verify against manually calculated reference values
4. Use exact equality: `result.Value.Should().Be(expected)`

## Reference implementations

- Simple: `src/s-z/Sma/Sma.StaticSeries.cs`
- Exponential: `src/e-k/Ema/Ema.StaticSeries.cs`
- Complex: `src/a-d/Adx/Adx.StaticSeries.cs`

## Constitutional rules

**Warning**: See `src/agents.md` for formula protection rules.
Series is the canonical source of mathematical truth.
````

### 5.2 indicator-buffer SKILL.md template

````markdown
---
name: indicator-buffer
description: Implement BufferList incremental indicators with efficient state management. Use for IIncrementFromChain, IIncrementFromQuote, or IIncrementFromPairs implementations. Covers interface selection, constructor patterns, and BufferListTestBase testing requirements.
---

# BufferList Indicator Development

BufferList indicators process data incrementally with efficient buffering, matching Series results exactly.

## Interface selection

| Interface | Input Type | Use Case | Examples |
|-----------|------------|----------|----------|
| `IIncrementFromChain` | IReusable | Chainable indicators | SMA, EMA, RSI |
| `IIncrementFromQuote` | IQuote | Needs OHLCV properties | Stoch, ATR, VWAP |
| `IIncrementFromPairs` | Dual IReusable | Two synchronized series | Correlation, Beta |

## Constructor pattern

```csharp
public class MyIndicatorList : BufferList<MyResult>, IIncrementFromChain
{
    // Primary constructor (parameters only)
    public MyIndicatorList(int lookbackPeriods)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(lookbackPeriods, 1);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<double>(lookbackPeriods);
    }

    // Chaining constructor (parameters + quotes)
    public MyIndicatorList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);
}
```

## Buffer management

Always use `BufferListUtilities`:
- `_buffer.Update(capacity, value)` - Standard rolling buffer
- `_buffer.UpdateWithDequeue(capacity, value)` - Returns dequeued value for sum adjustment

## State management

- Prefer tuples: `private (double sum, int count) _state;`
- Avoid custom structs for internal buffer state
- Reset in Clear(): `_state = default; _buffer.Clear(); base.Clear();`

## Testing requirements

1. Inherit from `BufferListTestBase` (NOT TestBase)
2. Implement test interface matching buffer interface
3. Verify exact Series parity: `bufferResults.IsExactly(seriesResults)`

## Reference implementations

- Chain: `src/e-k/Ema/Ema.BufferList.cs`
- Quote: `src/s-z/Stoch/Stoch.BufferList.cs`
- Pairs: `src/a-d/Correlation/Correlation.BufferList.cs`
````

### 5.3 indicator-stream SKILL.md template

````markdown
---
name: indicator-stream
description: Implement StreamHub real-time indicators with O(1) performance. Use for ChainProvider, QuoteProvider, or PairsProvider implementations. Covers provider selection, RollbackState patterns, performance anti-patterns, and comprehensive testing with StreamHubTestBase.
---

# StreamHub Indicator Development

StreamHub indicators support real-time data processing with stateful operations, achieving 1.5x Series performance or better.

## Provider selection

| Provider Base | Input | Output | Use Case |
|---------------|-------|--------|----------|
| `ChainProvider<IReusable, TResult>` | Single value | IReusable | Chainable indicators |
| `ChainProvider<IQuote, TResult>` | OHLCV | IReusable | Quote-driven, chainable output |
| `QuoteProvider<IQuote, TResult>` | OHLCV | IQuote | Quote-to-quote transformation |
| `PairsProvider<IReusable, TResult>` | Dual stream | IReusable | Synchronized dual inputs |

## Performance anti-patterns

### WRONG: O(n squared) recalculation (FORBIDDEN)

```csharp
// WRONG - Rebuilds entire history on each tick
for (int k = 0; k <= i; k++) { subset.Add(cache[k]); }
var result = subset.ToIndicator();
```

### CORRECT: O(1) incremental update

```csharp
// CORRECT - Maintain state, update incrementally
_avgGain = ((_avgGain * (period - 1)) + gain) / period;
```

## RollbackState pattern

Override when maintaining stateful fields:

```csharp
protected override void RollbackState(DateTime timestamp)
{
    _window.Clear();
    int targetIndex = ProviderCache.IndexGte(timestamp) - 1;
    int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);
    
    for (int p = startIdx; p <= targetIndex; p++)
        _window.Add(ProviderCache[p].Value);
}
```

## Testing requirements

1. Inherit from `StreamHubTestBase`
2. Implement exactly ONE observer interface:
   - `ITestChainObserver` (most common)
   - `ITestQuoteObserver` (quote-only providers)
   - `ITestPairsObserver` (dual-stream)
3. Test provider history mutations (Insert/Remove)
4. Verify exact Series parity

## Reference implementations

- Chain: `src/e-k/Ema/Ema.StreamHub.cs`
- Complex state: `src/a-d/Adx/Adx.StreamHub.cs`
- Pairs: `src/a-d/Correlation/Correlation.StreamHub.cs`
- Rolling window: `src/a-d/Chandelier/Chandelier.StreamHub.cs`

See `references/rollback-patterns.md` for detailed RollbackState examples.
See `references/performance-patterns.md` for O(1) optimization techniques.
````

---

## Part 6: Migration checklist

Create `docs/plans/ai-config-refactor.checklist.md`:

```markdown
# AI Configuration Refactor Checklist

## Phase 1: Foundation
- [ ] Create `.github/skills/` directory
- [ ] Create root `AGENTS.md`
- [ ] Create `src/AGENTS.md`
- [ ] Create `tests/AGENTS.md`

## Phase 2: Core Skills
- [ ] Create `indicator-series` skill (SKILL.md + references/)
- [ ] Create `indicator-buffer` skill (SKILL.md + references/)
- [ ] Create `indicator-stream` skill (SKILL.md + references/)
- [ ] Create `indicator-catalog` skill
- [ ] Create `performance-testing` skill
- [ ] Create `quality-gates` skill
- [ ] Create `testing-standards` skill

## Phase 3: Cleanup
- [ ] Delete `.github/copilot-instructions.md`
- [ ] Delete `.github/instructions/dotnet.instructions.md`
- [ ] Delete `.github/instructions/indicator-series.instructions.md`
- [ ] Delete `.github/instructions/indicator-buffer.instructions.md`
- [ ] Delete `.github/instructions/indicator-stream.instructions.md`
- [ ] Delete `.github/instructions/catalog.instructions.md`
- [ ] Delete `.github/instructions/performance-testing.instructions.md`
- [ ] Delete `.github/instructions/code-completion.instructions.md`
- [ ] Delete `.github/agents/indicator-series.agent.md`
- [ ] Delete `.github/agents/indicator-buffer.agent.md`
- [ ] Delete `.github/agents/indicator-stream.agent.md`
- [ ] Delete `.github/agents/performance.agent.md`
- [ ] Delete `.github/agents/streamhub-pairs.agent.md`
- [ ] Delete `.github/agents/streamhub-performance.agent.md`
- [ ] Delete `.github/agents/streamhub-state.agent.md`
- [ ] Delete `.github/agents/streamhub-testing.agent.md`

## Phase 4: Validation
- [ ] `dotnet build` succeeds with zero warnings
- [ ] `dotnet test` passes all tests
- [ ] `npx markdownlint-cli2` has zero errors
- [ ] Skills load in VS Code Copilot
- [ ] AGENTS.md recognized by coding agents
- [ ] No dangling file references (grep verification)

## Phase 5: Commit
- [ ] Commit with message: `refactor: migrate AI instructions to skills and AGENTS.md`
- [ ] Update this checklist to mark complete
```

---

## Appendix A: Official documentation links

| Resource | URL |
|----------|-----|
| Agent Skills Specification | https://agentskills.io/specification |
| Agent Skills Examples | https://github.com/anthropics/skills |
| AGENTS.md Standard | https://agents.md |
| GitHub Copilot Custom Instructions | https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot |
| GitHub Copilot Agent Skills | https://docs.github.com/en/copilot/concepts/agents/about-agent-skills |
| VS Code Agent Skills | https://code.visualstudio.com/docs/copilot/customization/agent-skills |
| Best Practices for AGENTS.md | https://github.blog/ai-and-ml/github-copilot/how-to-write-a-great-agents-md-lessons-from-over-2500-repositories/ |
| Coding Agent Best Practices | https://docs.github.com/en/copilot/tutorials/coding-agent/get-the-best-results |

## Appendix B: Key files in this repository

| File | Purpose | Keep/Migrate |
|------|---------|--------------|
| `src/agents.md` | Formula protection rules | **KEEP** |
| `tests/agents.md` | Test naming and precision | **KEEP** |
| `docs/PRINCIPLES.md` | Project principles | **KEEP** |
| `src/_common/README.md` | NaN handling policy | **KEEP** |
| `.editorconfig` | Code formatting | **KEEP** |
| `.markdownlint-cli2.jsonc` | Markdown linting | **KEEP** |

## Appendix C: Compatibility notes

| Tool | AGENTS.md | Skills | Custom Instructions |
|------|-----------|--------|---------------------|
| GitHub Copilot (web) | Yes | Yes | Yes |
| GitHub Copilot (VS Code) | Yes | Yes (Insiders) | Yes |
| GitHub Copilot CLI | Yes | Yes | No |
| Claude Code | Yes | Yes | No |
| Cursor | Yes | No | Yes |
| Devin | Yes | No | No |
| Gemini CLI | Yes | No | No |

---

**End of plan**

---
Last updated: December 30, 2025
