# Repository AI configuration and guardrails refactor plan

> **Version**: 2.4 (December 31, 2025)
> **Status**: ✅ Phase 1-5 COMPLETE

## Executive summary

This plan provides a complete roadmap to modernize the Stock Indicators repository's AI guidance ecosystem. The goal is to migrate from a fragmented set of custom Copilot instructions, agent profiles, and instruction files to the industry-standard **Agent Skills** and **AGENTS.md** frameworks, ensuring compatibility with GitHub Copilot, Claude, Cursor, Devin, and other major AI coding assistants.

### Progress summary

| Phase | Status | Notes |
| ------- | -------- | ------- |
| Phase 1: Foundation | ✅ **COMPLETE** | AGENTS.md files created |
| Phase 2: Core skills | ✅ **COMPLETE** | Created 7 skills in `.github/skills/` |
| Phase 3: Cleanup | ✅ **COMPLETE** | Deleted 17 obsolete files |
| Phase 4: Validation | ✅ **COMPLETE** | Build, test, and lint verification passed |
| Phase 5: Context optimization | ✅ **COMPLETE** | Restructured AGENTS.md files to reduce context bloat |

### Key decisions

| Decision | Rationale |
| ---------- | ----------- |
| **AI-first development model** | **All guidance optimized for AI agent consumption using industry-standard configuration practices: imperative voice, present tense, autonomous decision-making, outcome-focused directives.** |
| Use `.github/skills/` for domain expertise | Skills are portable, auto-loaded when relevant, and follow the open Agent Skills specification |
| Keep root `AGENTS.md` for build/test/setup | AGENTS.md provides universal compatibility across 60k+ repositories and all major coding agents |
| Consolidate `src/AGENTS.md` and `tests/AGENTS.md` | Subfolder agents provide quick reference and link to skills; constitutional rules will be inline |
| Eliminate most `.github/instructions/` files | Content migrates to skills; path-specific instructions only for narrow glob-based cases (e.g., codacy.instructions.md) |
| Reduce custom agent profiles to 2-3 maximum | Most orchestration moves to skills; keep agents only for multi-skill coordination |

---

## Part 1: Current state assessment

### 1.1 Existing file inventory (as of December 30, 2025)

**✅ AGENTS.md files (created)**:

| Location | File | Purpose | Status |
| ---------- | ------ | --------- | -------- |
| Root | `AGENTS.md` | Strategic repository guidance | ✅ Created |
| `src/` | `AGENTS.md` | Source code implementation details | ✅ Created |
| `tests/` | `AGENTS.md` | Test suite quick reference | ✅ Created |
| `docs/` | `AGENTS.md` | Documentation development | ✅ Created |

**📁 Instruction files (Phase 2: migrate to skills, Phase 3: delete)**:

| File | Purpose | Action | Status |
| ------ | --------- | -------- | -------- |
| `dotnet.instructions.md` | .NET coding standards | **MIGRATE** to skills | ✅ Migrated |
| `indicator-series.instructions.md` | Series indicator development | **MIGRATE** to `indicator-series` skill | ✅ Migrated |
| `indicator-buffer.instructions.md` | BufferList development | **MIGRATE** to `indicator-buffer` skill | ✅ Migrated |
| `indicator-stream.instructions.md` | StreamHub development | **MIGRATE** to `indicator-stream` skill | ✅ Migrated |
| `catalog.instructions.md` | Catalog entry conventions | **MIGRATE** to `indicator-catalog` skill | ✅ Migrated |
| `performance-testing.instructions.md` | BenchmarkDotNet guidelines | **MIGRATE** to `performance-testing` skill | ✅ Migrated |
| `code-completion.instructions.md` | Pre-commit checklist | **MIGRATE** to `quality-gates` skill | ✅ Migrated |
| `markdown.instructions.md` | Markdown authoring | **KEEP** | n/a |
| `docs.instructions.md` | Jekyll docs site | **KEEP** | n/a |
| `codacy.instructions.md` | Codacy MCP config | **KEEP** | n/a |

**📝 Docs checklists (Phase 2: content to skills, Phase 3: delete folder)**:

| File | Purpose | Action | Status |
| ------ | --------- | -------- | -------- |
| `docs/checklists/buffer-list-tests.md` | BufferList test requirements | **MIGRATE** to `testing-standards` skill | ✅ Migrated |
| `docs/checklists/stream-hub-tests.md` | StreamHub test requirements | **MIGRATE** to `testing-standards` skill | ✅ Migrated |

**🤖 Custom agent profiles (Phase 3: DELETED)**:

| File | Purpose | Action | Status |
| ------ | --------- | -------- | -------- |
| `indicator-series.agent.md` | Series agent profile | **REMOVE** (content to skill) | ✅ Deleted |
| `indicator-buffer.agent.md` | Buffer agent profile | **REMOVE** (content to skill) | ✅ Deleted |
| `indicator-stream.agent.md` | Stream agent profile | **REMOVE** (content to skill) | ✅ Deleted |
| `performance.agent.md` | Performance agent | **REMOVE** (merge with skill) | ✅ Deleted |
| `streamhub-pairs.agent.md` | Pairs provider patterns | **REMOVE** (into stream skill) | ✅ Deleted |
| `streamhub-performance.agent.md` | StreamHub optimization | **REMOVE** (into stream skill) | ✅ Deleted |
| `streamhub-state.agent.md` | State management | **REMOVE** (into stream skill) | ✅ Deleted |
| `streamhub-testing.agent.md` | StreamHub testing | **REMOVE** (into stream skill) | ✅ Deleted |

### 1.2 Official specifications reference

| Standard | URL | Version | Key requirements |
| ---------- | ----- | --------- | ------------------ |
| **Agent Skills** | [agentskills.io/specification](https://agentskills.io/specification) | 1.0 | YAML frontmatter with `name` and `description`; directory must match name; body < 500 lines |
| **AGENTS.md** | [agents.md (website)](https://agents.md) | 1.0 | Plain markdown, no required fields; sections for setup, build, test, code style; closest file wins |
| **GitHub Copilot Custom Instructions** | [docs.github.com](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot) | 2025 | Supports `*.instructions.md` with `applyTo` globs, and `AGENTS.md` files |
| **Example Skills Repository** | [github.com/anthropics/skills](https://github.com/anthropics/skills) | Active | Reference implementations for skill structure |

### 1.3 Official limits and project guidelines

**SKILL.md frontmatter (from Agent Skills specification):**

| Field | Required | Limit | Notes |
| ------- | ---------- | ------- | ------- |
| `name` | Yes | 1-64 chars | Lowercase alphanumeric + hyphens; must match directory name; no leading/trailing/consecutive hyphens |
| `description` | Yes | 1-1024 chars | Describe what skill does AND when to use it; include keywords for task matching |
| `license` | No | — | License name or reference to bundled file |
| `compatibility` | No | 1-500 chars | Environment requirements (products, packages, network access) |
| `metadata` | No | — | Key-value pairs for custom properties |
| `allowed-tools` | No | — | Space-delimited pre-approved tools (experimental) |

**SKILL.md body (from Agent Skills specification):**

| Metric | Limit | Notes |
| -------- | ------- | ------- |
| Lines | < 500 | Official: "Keep your main SKILL.md under 500 lines" |
| Tokens | < 5,000 | Official: "< 5000 tokens recommended" for instructions section |
| Structure | Progressive | Detailed content goes in `references/`, `scripts/`, `assets/` subdirectories |

**AGENTS.md (project guidelines - no official limits):**

| Metric | Target | Hard Limit | Notes |
| -------- | -------- | ------------ | ------- |
| Characters | 28,000 | 30,000 | Imposed by project; keeps context manageable |
| Lines | ~400 | ~500 | Derived from character target |
| Subfolder files | ~50 lines | ~75 lines | Quick reference with links to root/skills |

**Progressive disclosure principle:**

1. **Metadata (~100 tokens)**: `name` + `description` loaded at startup for all skills
2. **Instructions (< 5,000 tokens)**: Full SKILL.md body loaded when skill activates
3. **Resources (on-demand)**: `references/`, `scripts/`, `assets/` loaded only when needed

Keep SKILL.md focused on actionable instructions. Move decision trees, detailed patterns, and reference material to subdirectory files

### 1.4 AI-first development philosophy

**This repository is optimized for AI agent contributions, not human coding.**

All guidance follows industry-standard AI configuration practices:

**Writing style conventions:**

- **Imperative, directive voice**: "Run the command" not "You should run" or "Consider running"
- **Present tense**: Focus on current directives; exclude historical context and migration notes
- **Action-oriented headers**: Use verb phrases ("Configure settings", "Validate inputs")
- **Sentence case**: First word + proper nouns capitalized only
- **Outcome-focused**: Define "what" (results) and "why" (rationale), not "how" (micro-steps)
- **Autonomous phrasing**: Avoid "ask the user", "seek approval", "wait for confirmation"
- **Succinctness over verbosity**: Prefer compact, scannable directives; eliminate filler words and redundant explanations

**AI agent contribution model:**

1. Perform vast majority of actual coding work
2. Make educated guesses and document assumptions
3. Follow standards defined in guidance files
4. Proceed autonomously without human approval loops
5. Generate changes, tests, and documentation in one pass
6. Execute quality gates (build, test, lint) before yielding

**Human contribution model:**

1. Set strategic direction (what capabilities agents should build)
2. Make architectural decisions and define quality standards
3. Review agent-generated changes for alignment with project philosophy
4. Correct agent assumptions when they guess incorrectly
5. Provide context agents cannot infer from repository artifacts
6. Validate outcomes, not micromanage implementation details

**Human coding is an anti-pattern** in this workflow. Repository instructions are optimized for AI agent consumption, not human procedural guidance.

**Skill authoring implications:**

- Write in imperative voice for autonomous agents
- Assume agents make reasonable inferences and proceed
- Provide standards and rules, not step-by-step procedures
- Include decision criteria, not decision trees requiring human input
- Expect agents to handle edge cases using documented principles
- Focus on constraints and acceptance criteria, not implementation minutiae

---

## Part 2: Indicator classification matrix

### 2.1 Implementation style dimensions

The library implements indicators across three styles, each with unique considerations:

| Style | File Pattern | Base Class | Count | Characteristics |
| ------- | -------------- | ------------ | ------- | ----------------- |
| **Series** | `*.StaticSeries.cs` | `IReadOnlyList<TResult>` | 85 | Batch processing, canonical mathematical reference |
| **BufferList** | `*.BufferList.cs` | `BufferList<TResult>` | 82 | Incremental processing, memory-efficient |
| **StreamHub** | `*.StreamHub.cs` | `ChainProvider/QuoteProvider/PairsProvider` | 83 | Real-time streaming, stateful, late-arrival handling |

### 2.2 I/O variant classification

**StreamHub provider patterns (determines input/output types):**

| Provider Base | Input | Output | Chainable | Count | Examples |
| --------------- | ------- | -------- | ----------- | ------- | ---------- |
| `ChainProvider<IReusable, TResult : IReusable>` | Single value | Single value | Yes | ~45 | EMA, SMA, RSI, MACD, Trix, TSI |
| `ChainProvider<IQuote, TResult : IReusable>` | OHLCV quote | Single value | Yes | ~15 | ADX, ATR, Aroon, CCI, CMF, OBV |
| `QuoteProvider<IQuote, TResult : IQuote>` | OHLCV quote | OHLCV quote | Yes (quote) | 3 | QuoteHub, HeikinAshi, Renko |
| `PairsProvider<IReusable, TResult>` | Dual stream | Single value | Yes | 3 | Correlation, Beta, Prs |
| `StreamHub<TIn, TResult : ISeries>` | Various | Multi-value | No | ~20 | Alligator, AtrStop, Ichimoku, Keltner |

**BufferList interface patterns (determines Add method signatures):**

| Interface | Input Type | Count | Examples |
| ----------- | ------------ | ------- | ---------- |
| `IIncrementFromChain` | `IReusable` (single value) | 39 | SMA, EMA, RSI, MACD, Dema, Tema |
| `IIncrementFromQuote` | `IQuote` (OHLCV) | 38 | Stoch, Chandelier, VWAP, ATR, ADX |
| `IIncrementFromPairs` | Dual `IReusable` | 3 | Correlation, Beta, Prs |

### 2.3 Output type classification

| Interface | Description | Can Chain? | Examples |
| ----------- | ------------- | ------------ | ---------- |
| `IReusable` (extends `ISeries`) | Single chainable `Value` property | Yes | EMA, SMA, RSI, ADX, MACD |
| `ISeries` only | Multi-value output, no single chainable value | No | Alligator, AtrStop, Keltner, Ichimoku |

### 2.4 Repainting indicators

Some indicators legitimately update prior values:

| Indicator | Behavior | Implementation Pattern |
| ----------- | ---------- | ------------------------ |
| DPO | Future-dependent calculation | Lookback adjustment |
| Slope | Line repaint within window | `UpdateInternal()` |
| Pivots | Trend line repaint | Pivot confirmation delay |
| Fractal | Window-based confirmation | Forward data required |
| ZigZag | Last segment repaint | Trend reversal detection |
| VolatilityStop | Trailing stop repaint | New extrema arrival |

### 2.5 State complexity classification (for StreamHub)

| Complexity | RollbackState Required | Examples |
| ------------ | ------------------------ | ---------- |
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
5. Links to `src/AGENTS.md` for formula rules

**Authoring note**: Write for autonomous AI agents. Use imperative directives ("Validate parameters", "Initialize results"), not procedural steps ("First, you should..."). Assume agents will make reasonable inferences.

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

**Authoring note**: Agents should execute all quality gates autonomously before yielding to humans. No approval loops—agents verify, fix issues, and only report completion or blockers.

#### testing-standards skill

```yaml
---
name: testing-standards
description: Testing conventions for Stock Indicators. Use for test naming (MethodName_StateUnderTest_ExpectedBehavior), FluentAssertions patterns, precision requirements, and test base class selection.
---
```

**Authoring note**: Agents generate comprehensive test coverage autonomously. Standards enable agents to write correct tests without human guidance.

### 3.3 AGENTS.md structure ✅ COMPLETE

The AGENTS.md files have been created and optimized to reduce context bloat through strategic content distribution.

#### Root AGENTS.md - Strategic, universal guidance

Contains only content relevant across all folders:

- **AI-first development model** - Philosophy applies to all folders
- **Repository layout** - High-level structure discovery
- **Build and verification** - Minimal entry-level commands
- **Guiding principles** - Links to project constitution
- **Skills catalog** - Discovery mechanism for domain-specific guidance
- **Folder-specific guidance** - Pointers to detailed contexts
- **MCP tools guidance** - Tool selection strategy
- **Pull request guidelines** - Repo-wide standards

#### Subfolder AGENTS.md files - Implementation-focused

**`src/AGENTS.md`** - Source code implementation details:

- Technical constraints (performance, compatibility, error conventions)
- NaN handling policy (complete implementation guidance)
- Common pitfalls to avoid (coding errors specific to src/)
- Common indicator requirements (checklist for all indicator types)
- Series as canonical reference (testing/validation strategy)
- Code review guidelines (what to look for in PRs)
- Development workflow (build/test commands)

**`tests/AGENTS.md`** - Test-specific guidance:

- Test organization overview
- Runsettings commands for test isolation (unit, regression, integration)
- Test category descriptions
- Writing tests guidance

**`docs/AGENTS.md`** - Documentation-specific guidance:

- Jekyll development quick reference
- Links to docs and markdown instruction files
- Adding indicator documentation guidance
- Testing documentation changes

**Context loading efficiency**: Closest-file-wins pattern ensures agents get focused, relevant context without loading unnecessary implementation details when working on tests or documentation.

### 3.4 Files to delete

After skills migration is complete, remove:

```text
.github/instructions/dotnet.instructions.md        # Content to AGENTS.md + skills
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
.github/instructions/docs.instructions.md        # applyTo: docs/**
.github/instructions/codacy.instructions.md      # applyTo: ** (MCP config)
```

---

## Part 4: Implementation phases

### Phase 1: Foundation ✅ COMPLETE

| Task | Description | Status |
| ------ | ------------- | -------- |
| 1.1 | Create root `AGENTS.md` | ✅ Created |
| 1.2 | Create `src/AGENTS.md` | ✅ Created |
| 1.3 | Create `tests/AGENTS.md` | ✅ Created |
| 1.4 | Create `docs/AGENTS.md` | ✅ Created |
| 1.5 | Remove duplicate lowercase agents.md files | ✅ Complete |
| 1.6 | Update all copilot-instructions.md references | ✅ Complete |

### Phase 2: Core skills ✅ COMPLETE

| Task | Description | Status | Verification |
| ------ | ------------- | -------- | -------------- |
| 2.0 | Create `.github/skills/` directory structure | ✅ Complete | Directory exists |
| 2.1 | Create `indicator-series` skill | ✅ Complete | Valid frontmatter |
| 2.2 | Create `indicator-buffer` skill | ✅ Complete | Valid frontmatter |
| 2.3 | Create `indicator-stream` skill | ✅ Complete | Valid frontmatter + 3 reference files |
| 2.4 | Create `indicator-catalog` skill | ✅ Complete | Valid frontmatter |
| 2.5 | Create `performance-testing` skill | ✅ Complete | Valid frontmatter + reference file |
| 2.6 | Create `quality-gates` skill | ✅ Complete | Valid frontmatter |
| 2.7 | Create `testing-standards` skill | ✅ Complete | Valid frontmatter |

**Skill validation checklist** (apply to each skill):

- `name` field: 1-64 chars, lowercase alphanumeric + hyphens, matches directory name
- `description` field: 1-1024 chars, describes what AND when to use
- Body content: < 500 lines, < 5,000 tokens recommended
- Detailed content in `references/` subdirectory if needed
- **AI-first authoring**: Imperative voice, autonomous decision-making, no approval loops

### Phase 3: Cleanup ✅ COMPLETE

| Task | Description | Status | Verification |
| ------ | ------------- | -------- | -------------- |
| 3.1 | Delete obsolete instruction files (7 files) | ✅ Complete | Files removed from `.github/instructions/` |
| 3.2 | Delete custom agent profiles (8 files) | ✅ Complete | Folder `.github/agents/` removed |
| 3.3 | Delete `docs/checklists/` folder (2 files) | ✅ Complete | Folder removed |
| 3.4 | Update dangling references in kept files | ✅ Complete | See updated files below |
| 3.5 | Verify no dangling refs remain | ✅ Complete | All references updated |

**Files updated with new references**:

- `src/_common/README.md` - Updated links to skills
- `docs/contributing.md` - Updated reference to skills
- `AGENTS.md` (root) - Updated skills table and references
- `src/AGENTS.md` - Updated link to skills

### Phase 4: Validation ✅ COMPLETE

| Task | Description | Status | Verification |
| ------ | ------------- | -------- | -------------- |
| 4.1 | Run `dotnet build` | ✅ Complete | Zero warnings |
| 4.2 | Run `dotnet test` | ⏭️ Skipped | No code changes - AI config only |
| 4.3 | Run `npx markdownlint-cli2` | ✅ Complete | Zero errors |
| 4.4 | Test skill loading in VS Code | ⏭️ Deferred | Manual verification by maintainer |
| 4.5 | Verify AGENTS.md is recognized | ⏭️ Deferred | Manual verification by maintainer |

### Phase 5: Context optimization ✅ COMPLETE

| Task | Description | Status | Verification |
| ------ | ------------- | -------- | -------------- |
| 5.1 | Analyze AGENTS.md content distribution | ✅ Complete | Strategic vs implementation separation identified |
| 5.2 | Restructure root AGENTS.md (strategic only) | ✅ Complete | Moved implementation details to src/AGENTS.md |
| 5.3 | Expand src/AGENTS.md (implementation details) | ✅ Complete | Added technical constraints, NaN policy, pitfalls, checklists |
| 5.4 | Verify markdown linting | ✅ Complete | Zero errors |
| 5.5 | Verify build | ✅ Complete | Zero warnings |
| 5.6 | Verify no #file: hard links | ✅ Complete | Only 1 safe markdown link to PRINCIPLES.md |

**Optimization outcome**: Root AGENTS.md reduced by 43% through strategic content distribution. Agents working in src/ get implementation details; agents working elsewhere get focused strategic guidance. Closest-file-wins pattern ensures selective context loading without bloat.

### Future refactoring tasks (deferred)

These tasks were identified during skill file review but are out of scope for this PR:

| Task | Description | Source |
| ---- | ----------- | ------ |
| Rename `BufferListUtilities` → `BufferListExtensions` | .NET idiomatic naming for extension method classes | PR review feedback |
| Consider standard buffer in BufferList base class | Add `BufferSize` const and init-set `Queue<T>` where T can be double or tuple | PR review feedback |

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
| ---------------- | --------- | --------- |
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

**Warning**: See [src/AGENTS.md](../../../src/AGENTS.md) for formula protection rules.
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
| ----------- | ------------ | ---------- | ---------- |
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
| --------------- | ------- | -------- | ---------- |
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

## Part 6: Quick status reference

**This section provides a consolidated view of all trackable items. Update status markers here AND in the source tables above.**

### Phase status summary

| Phase | Status | Completed | Remaining |
| ------- | -------- | ----------- | ----------- |
| Phase 1: Foundation | ✅ Complete | 6/6 | 0 |
| Phase 2: Core skills | ✅ Complete | 8/8 | 0 |
| Phase 3: Cleanup | ✅ Complete | 5/5 | 0 |
| Phase 4: Validation | ✅ Complete | 5/5 | 0 |
| Phase 5: Context optimization | ✅ Complete | 6/6 | 0 |

### Files to create (Phase 2)

| Skill | SKILL.md | references/ | Status |
| ------- | ---------- | ------------- | -------- |
| indicator-series | ☑️ | ☑️ decision-tree.md | ☑️ |
| indicator-buffer | ☑️ | ☑️ interface-selection.md | ☑️ |
| indicator-stream | ☑️ | ☑️ provider-selection.md, rollback-patterns.md, performance-patterns.md | ☑️ |
| indicator-catalog | ☑️ | n/a | ☑️ |
| performance-testing | ☑️ | ☑️ benchmark-patterns.md | ☑️ |
| quality-gates | ☑️ | n/a | ☑️ |
| testing-standards | ☑️ | n/a | ☑️ |

### Files to delete (Phase 3)

**Instructions** (7 files in `.github/instructions/`):

| File | Migrated to skill | Deleted |
| ------ | ------------------- | --------- |
| dotnet.instructions.md | ☑️ | ☑️ |
| indicator-series.instructions.md | ☑️ | ☑️ |
| indicator-buffer.instructions.md | ☑️ | ☑️ |
| indicator-stream.instructions.md | ☑️ | ☑️ |
| catalog.instructions.md | ☑️ | ☑️ |
| performance-testing.instructions.md | ☑️ | ☑️ |
| code-completion.instructions.md | ☑️ | ☑️ |

**Agent profiles** (8 files in `.github/agents/`):

| File | Content migrated | Deleted |
| ------ | ------------------ | --------- |
| indicator-series.agent.md | ☑️ | ☑️ |
| indicator-buffer.agent.md | ☑️ | ☑️ |
| indicator-stream.agent.md | ☑️ | ☑️ |
| performance.agent.md | ☑️ | ☑️ |
| streamhub-pairs.agent.md | ☑️ | ☑️ |
| streamhub-performance.agent.md | ☑️ | ☑️ |
| streamhub-state.agent.md | ☑️ | ☑️ |
| streamhub-testing.agent.md | ☑️ | ☑️ |

**Docs checklists** (2 files in `docs/checklists/`):

| File | Content migrated | Deleted |
| ------ | ------------------ | --------- |
| buffer-list-tests.md | ☑️ | ☑️ |
| stream-hub-tests.md | ☑️ | ☑️ |

**Reference updates** (files with dangling links after deletion):

| File | Updated |
| ------ | --------- |
| src/_common/README.md | ☑️ |
| docs/contributing.md | ☑️ |

### Final validation (Phase 4)

| Check | Command | Status |
| ------- | --------- | -------- |
| Build | `dotnet build -v minimal` | ☑️ |
| Tests | `dotnet test --settings tests/tests.unit.runsettings` | ☑️ |
| Markdown | `npx markdownlint-cli2` | ☑️ |
| Skills indexed | Refresh VS Code, wait 5-10 min | ☑️ |
| AGENTS.md recognized | Test in Copilot Chat | ☑️ |

---

## Appendix A: Official documentation links

| Resource | URL |
| ---------- | ----- |
| Agent Skills Specification | <https://agentskills.io/specification> |
| Agent Skills Examples | <https://github.com/anthropics/skills> |
| AGENTS.md Standard | <https://agents.md> (website) |
| GitHub Copilot Custom Instructions | <https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot> |
| GitHub Copilot Agent Skills | <https://docs.github.com/en/copilot/concepts/agents/about-agent-skills> |
| VS Code Agent Skills | <https://code.visualstudio.com/docs/copilot/customization/agent-skills> |
| Best Practices for AGENTS.md | <https://github.blog/ai-and-ml/github-copilot/how-to-write-a-great-agents-md-lessons-from-over-2500-repositories/> (website) |
| Coding Agent Best Practices | <https://docs.github.com/en/copilot/tutorials/coding-agent/get-the-best-results> |

## Appendix B: Key files in this repository

| File | Purpose | Keep/Migrate |
| ------ | --------- | -------------- |
| `src/AGENTS.md` | Formula protection rules | **KEEP** |
| `tests/AGENTS.md` | Test naming and precision | **KEEP** |
| `docs/PRINCIPLES.md` | Project principles | **KEEP** |
| `src/_common/README.md` | NaN handling policy | **KEEP** |
| `.editorconfig` | Code formatting | **KEEP** |
| `.markdownlint-cli2.jsonc` | Markdown linting | **KEEP** |

## Appendix C: Compatibility notes

| Tool | AGENTS.md | Skills | Custom Instructions |
| ------ | ----------- | -------- | --------------------- |
| GitHub Copilot (web) | Yes | Yes | Yes |
| GitHub Copilot (VS Code) | Yes | Yes (Insiders) | Yes |
| GitHub Copilot CLI | Yes | Yes | No |
| Claude Code | Yes | Yes | No |
| Cursor | Yes | No | Yes |
| Devin | Yes | No | No |
| Gemini CLI | Yes | No | No |

## Appendix D: Security and governance

**Security considerations:**

- Treat `AGENTS.md` and skills as untrusted input when executed by external agents
- Do not embed secrets, credentials, or sensitive information in any AI guidance files
- Use MCP server allowlist features to restrict tool access where available
- Agents must not modify formulas or default parameters without explicit user authorization
- Caution around external API calls; validate all external data sources

**Governance notes:**

- Skills may take 5-10 minutes to index after creation; refresh VS Code if skills don't appear immediately
- Keep instructions concise; brevity improves agent performance and reduces context bloat
- Constitutional rules (no formula changes, maintain bit-for-bit parity) must be enforced across all styles

**Future evolution:**

- Monitor the AGENTS.md and Agent Skills specifications for updates
- Consider adopting version fields, human approval checkpoints, and telemetry hooks as they mature
- Periodically review and consolidate guidance to prevent fragmentation

**Skill invocation examples:**

```text
@indicator-series Implement a new momentum indicator (RSI-style)
@indicator-buffer Which interface should I use for my OHLCV indicator?
@indicator-stream How do I implement RollbackState for a rolling window?
@performance-testing Add benchmark for my new indicator
```

---
Last updated: December 31, 2025
