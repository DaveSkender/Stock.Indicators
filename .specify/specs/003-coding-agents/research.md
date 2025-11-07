# Research: Custom Agents for Series, Buffer, and Stream Indicator Development

**Feature**: `003-coding-agents`  
**Date**: November 3, 2025  
**Status**: Phase 0 Complete

## Executive Summary

This research resolves all technical unknowns for implementing three custom GitHub Copilot agents (`@series`, `@buffer`, `@streamhub`). Key findings:

1. **Agent Structure**: YAML frontmatter (name + description) followed by markdown sections (Expertise, Decision Trees, Reference Implementations, Usage Examples)
2. **Content Strategy**: Agents provide decision trees + key patterns; instruction files remain comprehensive source of truth
3. **Decision Points**: 8-12 key decisions per agent (interface selection, pattern choice, test structure, performance optimization)
4. **Reference Implementations**: 15+ canonical examples mapped across all three styles
5. **Test Guidance**: Style-specific test base classes, interfaces, and validation requirements documented
6. **Instruction Updates**: Two sections added to each file: "When to use this agent" + "Related agents"

## 1. Agent Structure Analysis

### Findings from Existing StreamHub Agents

Analyzed four existing agent files:

- `stream.agent.md` (primary agent)
- `streamhub-state.agent.md` (specialized sub-agent)
- `streamhub-performance.agent.md` (specialized sub-agent)
- streamhub-testing.agent.md (inferred from pattern)

**Consistent Structure Pattern:**

```markdown
---
name: {agent-identifier}
description: {one-line agent purpose and capabilities}
---

# {Agent Name} Agent

You are a {domain} expert for the Stock Indicators library.

## Your Expertise

You specialize in:
- {capability 1}
- {capability 2}
- ...

## {Domain-Specific Sections}

{Decision trees, patterns, reference implementations}

## When to use this agent

Invoke `@{agent-name}` when you need help with:
- {scenario 1}
- {scenario 2}

## Related agents

For specialized topics, consult:
- `@{related-agent}` - {description}

## Example usage

```text
@{agent-name} {example query 1}
@{agent-name} {example query 2}
```

```

**Section Organization Pattern:**

1. **Frontmatter** (2 properties: name, description)
2. **Identity Statement** ("You are a {domain} expert...")
3. **Expertise List** (bulleted capabilities)
4. **Core Content** (domain-specific sections, varies by agent)
   - Decision trees ("When to use X vs Y")
   - Implementation patterns
   - Reference implementations (file paths + descriptions)
   - Anti-patterns to avoid
   - Testing guidance
5. **Discovery Sections** (added in later updates)
   - "When to use this agent"
   - "Related agents"
6. **Usage Examples** (concrete invocation patterns)

**Length Guidelines:**

- Primary agents (streamhub): ~300-400 lines
- Specialized sub-agents (streamhub-state, streamhub-performance): ~250-350 lines
- Target for new agents: ≤500 lines (spec constraint)
- Average section: 20-80 lines depending on complexity

### Decision: Agent Definition Structure

**Adopted Structure for Series, Buffer, StreamHub:**

```markdown
---
name: {style}  # "series", "buffer", "streamhub"
description: {Expert guidance for {Style} indicator development - {key capabilities}}
---

# {Style} Development Agent

You are a {Style} development expert for the Stock Indicators library.

## Your Expertise

You specialize in:
- {Style-specific capabilities}

## Decision Trees

### {Decision Point 1}

{Question/scenario}

**Options:**
- {Option A}: {When to use} → Reference: {instruction file section}
- {Option B}: {When to use} → Reference: {instruction file section}

### {Decision Point 2}

...

## Key Patterns

{Brief summaries of critical patterns, NOT full duplication}

## Reference Implementations

Point developers to these canonical patterns:

- **{Pattern Category}**: `{file path}` - {Key characteristics}
- ...

## Testing Guidance

{Style-specific test requirements, interfaces, validation expectations}

## Documentation Requirements

{XML docs, inline comments, public docs expectations}

## When to use this agent

Invoke `@{agent-name}` when you need help with:
- {Scenario 1}
- {Scenario 2}

## Related agents

{Cross-references to other agents}

## Example usage

```text
@{agent-name} {Example query 1}
@{agent-name} {Example query 2}
```

```

**Rationale**: This structure balances discoverability, decision guidance, and references without duplicating comprehensive instruction file content.

## 2. Content Deduplication Strategy

### Problem Statement

Clarification requirement: "avoid having duplicative content between agents and instruction files."

### Analysis

**Instruction Files Currently Contain:**
- Comprehensive implementation guidelines (10-15 pages)
- Complete code patterns with full context
- Detailed checklists (10-20 items)
- Step-by-step implementation procedures
- Edge case handling details
- Performance optimization techniques
- Full test structure requirements

**Agents Should Provide:**
- Decision trees for pattern selection (which interface? which provider?)
- Key pattern summaries (2-3 sentences, not full code)
- References to instruction file sections for details
- Quick orientation for developers ("start here")
- Cross-references to related agents

### Decision: Content Ownership Matrix

| Content Type | Agent | Instruction File |
|--------------|-------|------------------|
| Decision trees (interface selection, pattern choice) | ✅ Full content | ❌ Not included |
| Key pattern summaries (2-3 sentences) | ✅ Brief summary | ✅ Full details |
| Reference implementation catalog | ✅ Paths + 1-line descriptions | ✅ Detailed usage |
| Code completion checklists | ✅ Brief overview + link | ✅ Complete checklist |
| Test requirements | ✅ Interface names + when to use | ✅ Full test structure |
| Mathematical formulas | ❌ Link to instruction file | ✅ Full formulas |
| Performance optimization | ✅ Key anti-patterns + link | ✅ Detailed techniques |
| Edge cases | ❌ Link to instruction file | ✅ Comprehensive coverage |

**Duplication Acceptable:**
- Agent invocation examples (unique to agent context)
- "When to use this agent" scenarios (agent-specific)
- Reference implementation file paths (useful in both contexts)
- Key constitutional principles (brief reminders, not full text)

**Duplication Prohibited:**
- Verbatim copying of code patterns
- Complete checklists (link instead)
- Full implementation procedures
- Comprehensive edge case lists

**Enforcement:**
- Manual review during PR for verbatim duplication
- Agent definitions target ≤500 lines to prevent bloat
- Regular cross-references to instruction files required

**Rationale**: Agents act as "table of contents + decision guides" while instruction files remain the authoritative comprehensive source. This prevents maintenance burden of keeping two copies in sync.

## 3. Decision Tree Requirements

### Series Indicator Decision Points

1. **File naming and organization**
   - Q: What file structure should I use?
   - Options: `{IndicatorName}.StaticSeries.cs` (standard) vs separate interface file
   - Reference: indicator-series.instructions.md § File naming conventions

2. **Input validation approach**
   - Q: How do I validate parameters?
   - Options: ArgumentOutOfRangeException (for numeric ranges) vs ArgumentException (for semantic issues)
   - Reference: indicator-series.instructions.md § Input validation patterns

3. **Warmup period handling**
   - Q: How many null results should I return during warmup?
   - Options: Return empty results vs return partial results with nulls
   - Reference: indicator-series.instructions.md § Result initialization

4. **Performance optimization strategy**
   - Q: When should I optimize for performance?
   - Options: Optimize immediately vs optimize after correctness proven
   - Reference: indicator-series.instructions.md § Performance expectations

5. **Test coverage approach**
   - Q: What test cases are mandatory?
   - Options: Happy path + insufficient quotes + bad data (minimum) vs comprehensive edge cases
   - Reference: indicator-series.instructions.md § Test coverage expectations

### Buffer Indicator Decision Points

1. **Interface selection**
   - Q: Which IIncrement interface should I implement?
   - Options:
     - `IIncrementFromChain` - For indicators chaining from IReusable (EMA, SMA)
     - `IIncrementFromQuote` - For indicators requiring OHLCV data
     - `IIncrementFromPairs` - For dual-stream indicators (rare)
   - Reference: indicator-buffer.instructions.md § Interface selection

2. **Buffer management approach**
   - Q: Should I implement custom buffer logic?
   - Options:
     - `BufferListUtilities.Update()` - Standard, no dequeue
     - `BufferListUtilities.UpdateWithDequeue()` - With size limit
     - Custom logic - Discouraged, only if necessary
   - Reference: indicator-buffer.instructions.md § Buffer management

3. **State management strategy**
   - Q: How do I manage internal state?
   - Options:
     - Tuple fields - Preferred for 2-3 values
     - Custom struct - Avoid (anti-pattern)
     - Multiple fields - For complex state
   - Reference: indicator-buffer.instructions.md § State management

4. **Constructor pattern**
   - Q: What constructor signatures are required?
   - Options:
     - Standard parameters only (always required)
     - Parameters + quotes with chaining (required for convenience)
   - Reference: indicator-buffer.instructions.md § Constructor patterns

5. **Test base class selection**
   - Q: Which test base should I inherit?
   - Options:
     - `BufferListTestBase` - Required for BufferList tests
     - `TestBase` - Wrong, will fail validation
   - Reference: indicator-buffer.instructions.md § Test structure

### StreamHub Indicator Decision Points

1. **Provider base class selection**
   - Q: Which provider should my indicator extend?
   - Options:
     - `ChainProvider<IReusable, TResult>` - For chainable indicators
     - `QuoteProvider<TIn, TResult>` - For quote-only indicators
     - `PairsProvider<TIn, TResult>` - For dual-stream indicators
   - Reference: indicator-stream.instructions.md § Provider selection

2. **Implementation pattern choice**
   - Q: What calculation pattern should I use?
   - Options:
     - Incremental state (O(1)) - Most indicators, maintains state
     - Repaint from anchor (O(k)) - Indicators with stable historical anchors
     - Full session rebuild (O(n)) - Rare, session-based only
   - Reference: indicator-stream.instructions.md § Implementation patterns

3. **RollbackState override decision**
   - Q: Do I need to override RollbackState?
   - Options:
     - Yes - Incremental pattern with state variables (required)
     - Yes - Repaint pattern with anchor tracking (for optimization)
     - No - Full rebuild pattern with no optimization (acceptable but suboptimal)
   - Reference: indicator-stream.instructions.md § RollbackState requirements, @streamhub-state

4. **Performance optimization strategy**
   - Q: How do I meet the ≤1.5x Series target?
   - Options:
     - RollingWindowMax/Min - For window operations
     - Incremental state variables - For running calculations
     - Wilder's smoothing pattern - For specific smoothing algorithms
   - Reference: indicator-stream.instructions.md § Performance optimization, @streamhub-performance

5. **Test interface selection**
   - Q: Which test interface should I implement?
   - Options:
     - `ITestChainObserver` - For ChainProvider hubs
     - `ITestQuoteObserver` - For QuoteProvider hubs
     - `ITestChainProvider` - For ChainProvider hubs needing provider features
     - `ITestPairsObserver` - For PairsProvider hubs
   - Reference: indicator-stream.instructions.md § Test requirements, @streamhub-testing

6. **Rollback validation approach**
   - Q: What rollback test scenarios are required?
   - Options:
     - Warmup prefill (always required)
     - Insert/Remove mutations (always required)
     - Duplicate arrivals (always required)
     - Series parity validation (always required)
   - Reference: indicator-stream.instructions.md § Rollback testing, @streamhub-testing

### Decision Tree Format

```markdown
### {Decision Point Title}

**Scenario**: {When developer faces this choice}

**Options**:

1. **{Option A}**
   - When to use: {Criteria for choosing this option}
   - Key characteristics: {Brief 2-3 bullet points}
   - Reference: [instruction file section](../instructions/{file}.instructions.md#{anchor})

2. **{Option B}**
   - When to use: {Criteria for choosing this option}
   - Key characteristics: {Brief 2-3 bullet points}
   - Reference: [instruction file section](../instructions/{file}.instructions.md#{anchor})

**Example**: `{Concrete example indicator using each option}`
```

## 4. Reference Implementation Mapping

### Series Reference Implementations

| Pattern Category | File Path | Key Characteristics | When to Use |
|------------------|-----------|---------------------|-------------|
| Basic moving average | `src/s-z/Sma/Sma.StaticSeries.cs` | Simple loop, straightforward calculation | Starting point for simple indicators |
| Exponential smoothing | `src/e-k/Ema/Ema.StaticSeries.cs` | Running state, warmup handling | Smoothed indicators |
| High/Low tracking | `src/a-d/Donchian/Donchian.StaticSeries.cs` | Window-based max/min | Range indicators |
| Multi-output | `src/a-d/Alligator/Alligator.StaticSeries.cs` | Multiple EMA lines | Complex multi-value indicators |
| Chained calculation | `src/a-d/AtrStop/AtrStop.StaticSeries.cs` | Uses intermediate indicator | Composite indicators |

### Buffer Reference Implementations

| Pattern Category | File Path | Key Characteristics | When to Use |
|------------------|-----------|---------------------|-------------|
| Basic buffer (Chain) | `src/s-z/Sma/Sma.BufferList.cs` | IIncrementFromChain, simple buffer | Chainable indicators |
| Quote-based | `src/a-d/Chandelier/Chandelier.BufferList.cs` | IIncrementFromQuote, OHLCV data | Indicators needing quote properties |
| Pairs-based | (rare pattern, consult instruction file) | IIncrementFromPairs | Dual-stream indicators |
| UpdateWithDequeue | `src/a-d/Adx/Adx.BufferList.cs` | Size-limited buffer | Memory-conscious indicators |
| Tuple state | `src/a-d/Alligator/Alligator.BufferList.cs` | Internal state as tuple | Multi-value state management |

### StreamHub Reference Implementations

| Pattern Category | File Path | Key Characteristics | When to Use |
|------------------|-----------|---------------------|-------------|
| **Incremental State (Standard)** - Chain provider | `src/e-k/Ema/Ema.StreamHub.cs` | ChainProvider, O(1) state | Most indicators |
| **Incremental State (Standard)** - Rolling windows | `src/a-d/Chandelier/Chandelier.StreamHub.cs` | RollingWindowMax/Min | Window-based indicators |
| **Incremental State (Standard)** - Complex state | `src/a-d/Adx/Adx.StreamHub.cs` | Multi-buffer Wilder's smoothing | Complex stateful indicators |
| **Incremental State (Standard)** - Dual-stream | `src/a-d/Correlation/Correlation.StreamHub.cs` | PairsProvider, synchronized inputs | Correlation, Beta |
| **Incremental State (Standard)** - Quote provider | `src/m-r/Renko/Renko.StreamHub.cs` | QuoteProvider pattern | Quote-only indicators |
| **Repaint from Anchor (Partial Rebuild)** - Pivot tracking | `src/s-z/ZigZag/ZigZag.StreamHub.cs` | Anchor state, O(k) from pivot | Pivot/anchor-based indicators |
| **Full Session Rebuild** - Session-based | `src/m-r/PivotPoints/PivotPoints.StreamHub.cs` | Session boundaries | Session-dependent calculations |

### Reference Implementation Documentation Format

In agent files, reference implementations will be listed as:

```markdown
## Reference Implementations

Point developers to these canonical patterns:

**{Pattern Category}**:
- `{relative/path/to/file.cs}` - {1-line description of key characteristics}
- `{relative/path/to/file.cs}` - {1-line description}

**{Another Category}**:
- `{relative/path/to/file.cs}` - {1-line description}
```

**Example**:

```markdown
## Reference Implementations

**Incremental state (standard)**:
- `src/e-k/Ema/Ema.StreamHub.cs` - Chain provider with O(1) incremental state updates
- `src/a-d/Chandelier/Chandelier.StreamHub.cs` - Rolling windows for efficient max/min tracking
- `src/a-d/Adx/Adx.StreamHub.cs` - Complex multi-buffer Wilder's smoothing pattern

**Repaint from anchor (partial rebuild)**:
- `src/s-z/ZigZag/ZigZag.StreamHub.cs` - Pivot tracking with O(k) recalculation from last confirmed anchor

For detailed implementation guidance, see `.github/instructions/indicator-stream.instructions.md`.
```

## 5. Test Guidance Requirements

### Series Test Requirements

**Test Base Class**: `TestBase` (standard)

**Required Test Methods**:

1. `Standard()` - Happy path with historical data
2. `InsufficientQuotes()` - Test with insufficient data
3. `BadData()` - Parameter validation
4. `Removed()` - (If applicable) Test with removed quotes
5. `Exceptions()` - (If comprehensive) All exception scenarios

**Validation Patterns**:

- Use `.Should().BeApproximately()` for floating-point comparisons
- Verify specific data points against reference calculations
- Check warmup period handling (correct number of null results)
- Validate timestamps align with input quotes

**Reference Test**: `tests/indicators/e-k/Ema/Ema.StaticSeries.Tests.cs`

### Buffer Test Requirements

**Test Base Class**: `BufferListTestBase` (REQUIRED - not TestBase!)

**Required Test Interfaces**:

- `IIncrementFromChain` → Implement `ITestBufferListChainIncrement`
- `IIncrementFromQuote` → Implement `ITestBufferListQuoteIncrement`
- `IIncrementFromPairs` → Implement `ITestBufferListPairsIncrement`

**Required Test Methods**:

1. `BufferInitialization()` - Verify buffer starts empty
2. `IncrementalUpdates()` - Test incremental processing
3. `SeriesParity()` - Verify equivalence with Series results
4. `ChainSyntax()` - Test constructor with quotes parameter
5. `Exceptions()` - Parameter validation

**Validation Patterns**:

- Must verify strict equivalence with Series: `.Should().BeEquivalentTo(series, o => o.WithStrictOrdering())`
- Test both constructor patterns (with/without quotes)
- Verify buffer state management

**Reference Test**: `tests/indicators/s-z/Sma/Sma.BufferList.Tests.cs`

### StreamHub Test Requirements

**Test Base Class**: `StreamHubTestBase`

**Required Test Interfaces** (select based on provider):

- ChainProvider → `ITestChainObserver` or `ITestChainProvider`
- QuoteProvider → `ITestQuoteObserver`
- PairsProvider → `ITestPairsObserver`

**Interface Selection Guidance**:

- Use `ITestChainProvider` when testing provider features (subscribers, rollback)
- Use `ITestChainObserver` for simple observation patterns
- Use `ITestQuoteObserver` for quote-only indicators
- Use `ITestPairsObserver` for dual-stream indicators

**Required Test Methods**:

1. `Standard()` - Happy path streaming
2. `RollbackWarmup()` - Rollback during warmup period
3. `RollbackDuplicate()` - Rollback with duplicate timestamp
4. `RollbackInsert()` - Insert historical quote and verify recalculation
5. `RollbackRemove()` - Remove historical quote and verify parity
6. `SeriesParity()` - Verify strict equivalence with Series
7. `Exceptions()` - Parameter validation

**Validation Patterns**:

- Must verify strict Series parity: `.Should().BeEquivalentTo(series, o => o.WithStrictOrdering())`
- Comprehensive rollback validation (all 5 rollback scenarios)
- Performance validation against ≤1.5x Series target (optional but recommended)

**Reference Tests**:

- Comprehensive: `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs`
- Rollback patterns: `tests/indicators/a-d/Adx/Adx.StreamHub.Tests.cs`
- Pairs provider: `tests/indicators/a-d/Correlation/Correlation.StreamHub.Tests.cs`

### Test Guidance Summary for Agents

```markdown
## Testing Guidance

**Test Base Class**: {TestBase | BufferListTestBase | StreamHubTestBase}

**Required Interfaces**: {Style-specific interfaces}

**Must Include**:
- {Key test scenarios}
- Series parity validation
- Parameter validation
- {Style-specific requirements}

**Reference**: `{canonical test file path}`

For comprehensive test guidelines, see `.github/instructions/indicator-{style}.instructions.md`.
```

## 6. Instruction File Impact Analysis

### Current Instruction Files

1. **indicator-series.instructions.md** (185 lines)
   - Structure: Frontmatter → Checklist → Conventions → Testing → Standards
   - Target insertion point: After frontmatter (line 8), before "## File naming conventions"

2. **indicator-buffer.instructions.md** (similar structure)
   - Target insertion point: After frontmatter, before first major section

3. **indicator-stream.instructions.md** (most comprehensive)
   - Target insertion point: After frontmatter, before first major section

### Required New Sections

**Section 1: "When to use this agent"** (8-12 lines)

```markdown
## When to use this agent

Invoke `@{agent-name}` when you need help with:

- {Scenario 1: implementation guidance}
- {Scenario 2: pattern selection}
- {Scenario 3: test structure}
- {Scenario 4: debugging issues}
- {Scenario 5: style-specific questions}

For comprehensive implementation details, continue reading this document.
```

**Section 2: "Related agents"** (6-10 lines)

```markdown
## Related agents

For related topics, consult:

- `@{primary-agent}` - {Brief description}
- `@{related-agent-1}` - {Brief description}
- `@{related-agent-2}` - {Brief description}

See also: `.github/cheatsheets/{agent-name}.agent.md` for decision trees and quick reference.
```

### Placement Strategy

**Option A: After frontmatter, before content** (RECOMMENDED)

```markdown
---
applyTo: "..."
description: "..."
---

## When to use this agent

{Content}

## Related agents

{Content}

# {Original first heading}

{Rest of document unchanged}
```

**Rationale**: Provides immediate discoverability for developers landing on instruction file. Acts as "table of contents" pointing to agent for quick guidance vs. comprehensive details.

**Option B: At end of document**

- Less discoverable
- Breaks reading flow
- Not recommended

### Decision: Instruction File Updates

**Adopted Approach**:

1. Add two sections after YAML frontmatter, before first major heading
2. Keep sections brief (8-12 lines for "When to use", 6-10 lines for "Related")
3. Maintain all existing content unchanged
4. Preserve applyTo pattern unchanged
5. Total addition per file: ~15-25 lines

**Files to Update**:

- `.github/instructions/indicator-series.instructions.md`
- `.github/instructions/indicator-buffer.instructions.md`
- `.github/instructions/indicator-stream.instructions.md`

## Technical Decisions Summary

| Decision Point | Resolution | Rationale |
|----------------|------------|-----------|
| Agent frontmatter properties | name + description only (no tools) | Matches existing pattern, tools defaults to all |
| Agent definition length | Target ≤500 lines, typical 300-400 | Maintains readability, forces conciseness |
| Content deduplication strategy | Agents: decision trees + key patterns; Instructions: comprehensive details | Prevents maintenance burden, clear separation of concerns |
| Decision trees per agent | 5-6 major decisions | Covers key developer choice points without overwhelming |
| Reference implementations | 5-8 per agent with 1-line descriptions | Sufficient variety without exhaustive catalog |
| Test guidance depth | Interface names + when to use, link for details | Quick orientation without duplicating test structure |
| Instruction file insertion point | After frontmatter, before first heading | Maximizes discoverability |
| Instruction file additions | 2 sections, ~15-25 lines total | Minimal impact, backward compatible |

## Best Practices from Existing Agents

### From stream.agent.md

**Strengths**:

- Clear provider selection decision tree
- Pattern categorization (incremental vs full rebuild)
- Explicit anti-patterns section
- Comprehensive reference implementation catalog
- Related agents section with clear cross-references

**Adoption for New Agents**:

- Include "Anti-patterns to avoid" section
- Categorize patterns by use case
- Provide explicit when-to-use criteria for each option
- Cross-reference specialized agents

### From streamhub-state.agent.md

**Strengths**:

- Focused scope (state management only)
- Code examples showing right vs wrong patterns
- "Common mistakes to avoid" section
- "Lessons learned from real implementations"

**Adoption for New Agents**:

- Include comparison examples (❌ wrong vs ✅ correct)
- Document common pitfalls
- Reference real-world implementations

### From streamhub-performance.agent.md

**Strengths**:

- Performance target clearly stated (≤1.5x)
- Explicit O(n²) and O(n) anti-pattern identification
- Before/after code comparisons
- Performance testing guidance

**Adoption for New Agents**:

- State performance expectations upfront
- Use before/after patterns for clarity
- Include testing/validation guidance

## Implementation Priorities

### Phase A: Foundation (Series Agent)

**Priority**: P1 (highest - Series is canonical reference)

**Rationale**: Series indicators are the source of mathematical truth. All other styles (Buffer, Stream) must match Series results. Implementing Series agent first establishes the pattern for Buffer and StreamHub agents.

**Key Decisions**:

1. File naming conventions → Simple, straightforward
2. Validation patterns → Standard .NET patterns
3. Warmup handling → Null result handling
4. Test coverage → Happy path + edge cases
5. Performance expectations → Baseline for comparison

### Phase B: Incremental Processing (Buffer Agent)

**Priority**: P2 (after Series foundation established)

**Rationale**: BufferList enables incremental processing and must maintain parity with Series. Intermediate complexity between Series and StreamHub.

**Key Decisions**:

1. Interface selection → Critical choice point (Chain/Quote/Pairs)
2. Buffer management → Discourage custom logic, promote utilities
3. State management → Tuple vs fields
4. Constructor patterns → Two required signatures
5. Test structure → BufferListTestBase + specific interfaces

### Phase C: Real-Time Streaming (StreamHub Agent)

**Priority**: P2 (parallel with Buffer agent)

**Rationale**: StreamHub supports real-time processing with stateful operations. Most complex implementation style. Update existing agent to align with new pattern rather than create from scratch.

**Key Decisions**:

1. Provider selection → Most critical decision
2. Implementation pattern → Incremental vs repaint vs full rebuild
3. RollbackState override → When required vs optional
4. Performance optimization → ≤1.5x target
5. Test coverage → Comprehensive rollback validation

## Next Steps

Phase 0 research complete. Proceed to Phase 1:

1. **Create data-model.md**
   - Define Agent Definition File Schema
   - Define Decision Tree Node structure
   - Define Reference Implementation Entry format
   - Define Instruction File Update structure

2. **Create contracts/**
   - `agent-invocation.yml` - Invocation patterns and response structure
   - `agent-definition.yml` - File schema and validation rules
   - `instruction-update.yml` - Update requirements and constraints

3. **Create quickstart.md**
   - Series agent validation scenarios (3 scenarios)
   - Buffer agent validation scenarios (3 scenarios)
   - StreamHub agent validation scenarios (3 scenarios)

4. **Run agent context update**
   - Execute `.specify/scripts/bash/update-agent-context.sh copilot`
   - Add new agent names and file paths to context

All technical unknowns from plan.md resolved. Ready to proceed with Phase 1: Design & Contracts.
