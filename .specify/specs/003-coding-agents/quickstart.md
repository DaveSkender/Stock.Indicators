# Quickstart: Custom Agents Validation Scenarios

**Feature**: `003-coding-agents`  
**Date**: November 3, 2025  
**Status**: Phase 1 - Validation Scenarios

## Purpose

This document defines concrete validation scenarios to manually test each custom GitHub Copilot agent. These scenarios verify that agents provide accurate, actionable guidance aligned with scoped instruction files and constitutional principles.

## Validation Approach

- **Manual Testing**: Invoke agents in GitHub Copilot Chat with specific queries
- **Success Criteria**: Agent provides decision guidance + reference links + example paths + next steps
- **Failure Criteria**: Agent duplicates instruction file verbatim, provides incorrect guidance, or fails to reference authoritative sources

## Series Agent (`@series`) Validation

### Scenario 1: New Indicator Implementation

**Query**: `@series I need to implement a new momentum indicator (RSI-style)`

**Expected Response Components**:

1. **File Naming Guidance**
   - ✅ Mentions `{IndicatorName}.StaticSeries.cs` pattern
   - ✅ References file naming conventions section in instruction file

2. **Member Ordering**
   - ✅ Lists standard order (fields, constructors, public methods, private helpers)
   - ✅ Links to implementation requirements section

3. **Validation Patterns**
   - ✅ Explains ArgumentOutOfRangeException for numeric ranges
   - ✅ Explains ArgumentException for semantic issues
   - ✅ Links to input validation patterns section

4. **Reference Implementation**
   - ✅ Points to similar indicator (e.g., `src/e-k/Ema/Ema.StaticSeries.cs`)
   - ✅ Explains why this example is relevant

5. **Next Steps**
   - ✅ Concrete actions (implement extension method, validate parameters, write tests)
   - ✅ Links to code completion checklist

**Failure Modes to Check**:

- ❌ Copies entire input validation code block from instruction file
- ❌ Provides implementation code without referencing instruction file
- ❌ Suggests wrong file naming pattern
- ❌ Missing constitutional principle references (mathematical precision)

### Scenario 2: Test Structure Guidance

**Query**: `@series What test cases are required for Series indicators?`

**Expected Response Components**:

1. **Test Coverage Requirements**
   - ✅ Lists mandatory test methods (Standard, InsufficientQuotes, BadData)
   - ✅ Explains purpose of each test type

2. **Test Base Class**
   - ✅ Specifies `TestBase` (not BufferListTestBase or StreamHubTestBase)
   - ✅ Links to test structure section

3. **Validation Patterns**
   - ✅ Mentions `.Should().BeApproximately()` for floating-point comparisons
   - ✅ Explains warmup period validation expectations

4. **Reference Test**
   - ✅ Points to canonical test file (e.g., `tests/indicators/e-k/Ema/Ema.StaticSeries.Tests.cs`)
   - ✅ Explains what to study in the reference

5. **Next Steps**
   - ✅ Create test file, inherit from TestBase, implement test methods
   - ✅ Links to test coverage expectations section

**Failure Modes to Check**:

- ❌ Copies entire test class code from instruction file
- ❌ Suggests wrong test base class
- ❌ Missing reference to manual calculation requirements

### Scenario 3: Mathematical Accuracy Issue

**Query**: `@series My Series indicator calculations don't match the reference data. How do I debug?`

**Expected Response Components**:

1. **Formula Sourcing Hierarchy**
   - ✅ References constitution principle (mathematical precision)
   - ✅ Links to formula sourcing hierarchy in constitution or copilot-instructions
   - ✅ Mentions manual calculation spreadsheets as source of truth

2. **Common Causes**
   - ✅ Off-by-one warmup periods
   - ✅ Floating-point precision issues
   - ✅ Incorrect order of operations

3. **Diagnostic Steps**
   - ✅ Compare against manual calculations
   - ✅ Check warmup period handling
   - ✅ Verify formula matches authoritative source

4. **Reference to NaN Handling**
   - ✅ Links to NaN handling policy if applicable
   - ✅ Explains when to use double.NaN vs null

5. **Next Steps**
   - ✅ Review formula source, validate warmup logic, check edge cases
   - ✅ Links to mathematical accuracy section

**Failure Modes to Check**:

- ❌ Provides generic debugging advice without referencing constitution
- ❌ Missing formula sourcing hierarchy reference
- ❌ Doesn't mention manual calculation spreadsheets

## Buffer Agent (`@buffer`) Validation

### Scenario 1: Interface Selection

**Query**: `@buffer Which interface should I use for my BufferList indicator that needs OHLCV data?`

**Expected Response Components**:

1. **Decision Tree Navigation**
   - ✅ Explains three interface options (IIncrementFromChain, IIncrementFromQuote, IIncrementFromPairs)
   - ✅ Identifies IIncrementFromQuote as correct choice for OHLCV requirement

2. **When to Use Criteria**
   - ✅ IIncrementFromChain: Chainable from IReusable (simple values)
   - ✅ IIncrementFromQuote: Requires OHLCV properties ✅ (matches query)
   - ✅ IIncrementFromPairs: Dual-stream (not applicable here)

3. **Reference Implementation**
   - ✅ Points to example using IIncrementFromQuote (e.g., `src/a-d/Chandelier/Chandelier.BufferList.cs`)
   - ✅ Explains key characteristics of this pattern

4. **Next Steps**
   - ✅ Implement IIncrementFromQuote interface
   - ✅ Use quote properties (High, Low, Close, etc.)
   - ✅ Links to interface selection section in instruction file

**Failure Modes to Check**:

- ❌ Recommends wrong interface for the requirement
- ❌ Doesn't explain decision criteria
- ❌ Missing reference implementation example

### Scenario 2: Buffer Management

**Query**: `@buffer How do I manage buffer state efficiently in BufferList?`

**Expected Response Components**:

1. **Universal Buffer Utilities**
   - ✅ Recommends `BufferListUtilities.Update()` (standard, no dequeue)
   - ✅ Mentions `BufferListUtilities.UpdateWithDequeue()` (size-limited)
   - ✅ Discourages custom buffer logic

2. **When to Use Each**
   - ✅ Update(): Default choice, no size limit
   - ✅ UpdateWithDequeue(): Memory-conscious, size-limited buffer
   - ✅ Custom logic: Rarely needed, only for special cases

3. **Anti-pattern Warning**
   - ✅ Warns against custom struct for buffer state
   - ✅ Links to buffer management section

4. **Reference Implementation**
   - ✅ Points to UpdateWithDequeue example (e.g., `src/a-d/Adx/Adx.BufferList.cs`)
   - ✅ Points to standard Update example (e.g., `src/s-z/Sma/Sma.BufferList.cs`)

5. **Next Steps**
   - ✅ Choose appropriate utility method
   - ✅ Avoid reinventing buffer logic
   - ✅ Links to comprehensive buffer management guidance

**Failure Modes to Check**:

- ❌ Provides custom buffer implementation code (instead of recommending utilities)
- ❌ Doesn't warn against custom structs
- ❌ Missing reference to BufferListUtilities

### Scenario 3: Test Structure

**Query**: `@buffer How do I test BufferList equivalence with Series results?`

**Expected Response Components**:

1. **Test Base Class**
   - ✅ Emphasizes `BufferListTestBase` (required, not TestBase)
   - ✅ Explains why BufferListTestBase is mandatory

2. **Test Interface Selection**
   - ✅ Lists interface options based on IIncrement implementation
   - ✅ IIncrementFromChain → ITestBufferListChainIncrement
   - ✅ IIncrementFromQuote → ITestBufferListQuoteIncrement

3. **Series Parity Validation**
   - ✅ Must use `.Should().BeEquivalentTo(series, o => o.WithStrictOrdering())`
   - ✅ Explains strict ordering requirement
   - ✅ Links to test structure section

4. **Reference Test**
   - ✅ Points to canonical BufferList test (e.g., `tests/indicators/s-z/Sma/Sma.BufferList.Tests.cs`)
   - ✅ Mentions SeriesParity() test method pattern

5. **Next Steps**
   - ✅ Inherit from BufferListTestBase
   - ✅ Implement appropriate test interface
   - ✅ Add SeriesParity() test method

**Failure Modes to Check**:

- ❌ Suggests TestBase instead of BufferListTestBase
- ❌ Missing strict ordering requirement
- ❌ Doesn't explain test interface selection

## StreamHub Agent (`@streamhub`) Validation

### Scenario 1: Provider Selection

**Query**: `@streamhub I need to implement a new streaming VWAP indicator. What provider base should I use?`

**Expected Response Components**:

1. **Decision Tree Navigation**
   - ✅ Explains three provider options (ChainProvider, QuoteProvider, PairsProvider)
   - ✅ Identifies QuoteProvider as appropriate for VWAP (needs volume + price)

2. **When to Use Criteria**
   - ✅ ChainProvider: Chainable indicators (EMA, RSI)
   - ✅ QuoteProvider: Quote-only indicators (VWAP, Renko, volume-weighted) ✅ (matches query)
   - ✅ PairsProvider: Dual-stream (Correlation, Beta)

3. **Reference Implementation**
   - ✅ Points to QuoteProvider example (e.g., `src/m-r/Renko/Renko.StreamHub.cs`)
   - ✅ Explains key characteristics of QuoteProvider pattern

4. **Implementation Pattern Guidance**
   - ✅ Suggests incremental state pattern for VWAP
   - ✅ Mentions RollbackState override requirement
   - ✅ Links to provider selection section

5. **Next Steps**
   - ✅ Extend QuoteProvider<IQuote, VwapResult>
   - ✅ Implement ToIndicator() with incremental calculation
   - ✅ Override RollbackState() for state management
   - ✅ Links to implementation patterns section

**Failure Modes to Check**:

- ❌ Recommends wrong provider for the requirement
- ❌ Doesn't mention RollbackState requirement
- ❌ Missing implementation pattern guidance

### Scenario 2: Performance Optimization

**Query**: `@streamhub My StreamHub is 50x slower than Series. How do I optimize?`

**Expected Response Components**:

1. **Performance Target**
   - ✅ States ≤1.5x Series target
   - ✅ Identifies 50x as severe O(n²) or O(n) anti-pattern

2. **Common Anti-patterns**
   - ✅ O(n²): Full series recalculation every tick
   - ✅ O(n): Linear window scans every tick
   - ✅ Links to @streamhub-performance for deep dive

3. **Optimization Techniques**
   - ✅ Incremental state variables (O(1))
   - ✅ RollingWindowMax/Min for window operations
   - ✅ Wilder's smoothing pattern for smoothed indicators

4. **Diagnostic Steps**
   - ✅ Check for Series method calls in ToIndicator()
   - ✅ Check for full cache iteration every update
   - ✅ Profile with BenchmarkDotNet

5. **Reference Implementations**
   - ✅ Points to optimized examples (EMA, Chandelier, ADX)
   - ✅ Points to before/after pattern in @streamhub-performance

6. **Next Steps**
   - ✅ Identify bottleneck (O(n²) vs O(n))
   - ✅ Replace with incremental pattern
   - ✅ Benchmark against Series
   - ✅ Consult @streamhub-performance for detailed guidance

**Failure Modes to Check**:

- ❌ Provides generic performance advice without identifying anti-pattern
- ❌ Doesn't mention ≤1.5x target
- ❌ Missing reference to @streamhub-performance sub-agent

### Scenario 3: State Management

**Query**: `@streamhub When do I need to override RollbackState?`

**Expected Response Components**:

1. **Decision Criteria**
   - ✅ Incremental pattern with state variables: REQUIRED
   - ✅ Repaint from anchor with optimization: SHOULD (for performance)
   - ✅ Full rebuild with no optimization: NOT NEEDED (but suboptimal)

2. **Common Scenarios Requiring Override**
   - ✅ Rolling windows (RollingWindowMax/Min)
   - ✅ Running totals/averages (EMA, Wilder's smoothing)
   - ✅ Previous value tracking (_prevValue,_prevHigh)
   - ✅ Buffered historical values (K buffer in Stoch)

3. **Scenarios NOT Requiring Override**
   - ✅ No state variables maintained
   - ✅ Using full Series recalculation (temporary)
   - ✅ Acceptable performance without optimization

4. **Reference to Sub-agent**
   - ✅ Links to @streamhub-state for comprehensive patterns
   - ✅ Mentions canonical RollbackState implementations (Chandelier, Stoch, ADX)

5. **Next Steps**
   - ✅ Identify state variables in your indicator
   - ✅ Decide if optimization needed
   - ✅ Consult @streamhub-state for implementation pattern
   - ✅ Links to RollbackState section in instruction file

**Failure Modes to Check**:

- ❌ Provides absolute rule without explaining decision criteria
- ❌ Doesn't distinguish between incremental and repaint patterns
- ❌ Missing reference to @streamhub-state sub-agent

## Cross-Agent Validation

### Scenario 4: Wrong Agent Invoked

**Query**: `@series How do I implement RollbackState?` (StreamHub-specific question to Series agent)

**Expected Response Components**:

1. **Style Mismatch Recognition**
   - ✅ Recognizes RollbackState is StreamHub-specific
   - ✅ Redirects to @streamhub agent

2. **Polite Redirect**
   - ✅ "For StreamHub indicators, please consult @streamhub instead."
   - ✅ "Series indicators focus on batch processing and don't have RollbackState."

3. **Optional Context**
   - ✅ Briefly explains Series vs StreamHub difference
   - ✅ Mentions Series as canonical reference for mathematical correctness

**Failure Modes to Check**:

- ❌ Attempts to answer StreamHub question as Series agent
- ❌ Provides incorrect guidance outside agent's expertise
- ❌ Doesn't redirect to appropriate agent

## Validation Checklist

### Per-Agent Validation

**Series Agent**:

- [ ] Scenario 1 (new indicator) - Provides file naming + validation + reference + next steps
- [ ] Scenario 2 (test structure) - Specifies TestBase + test coverage + reference test
- [ ] Scenario 3 (math accuracy) - References constitution + formula sourcing + diagnostic steps
- [ ] Wrong agent redirect (if applicable)

**Buffer Agent**:

- [ ] Scenario 1 (interface selection) - Provides decision tree + correct choice + reference
- [ ] Scenario 2 (buffer management) - Recommends utilities + discourages custom logic
- [ ] Scenario 3 (test structure) - Emphasizes BufferListTestBase + Series parity
- [ ] Wrong agent redirect (if applicable)

**StreamHub Agent**:

- [ ] Scenario 1 (provider selection) - Provides decision tree + correct choice + pattern guidance
- [ ] Scenario 2 (performance) - Identifies anti-pattern + optimization techniques + sub-agent reference
- [ ] Scenario 3 (state management) - Provides decision criteria + scenarios + sub-agent reference
- [ ] Wrong agent redirect (if applicable)

### Cross-Cutting Validation

- [ ] No verbatim duplication of instruction file content
- [ ] All reference links resolve correctly
- [ ] All reference implementation paths exist
- [ ] Constitutional principles referenced when applicable
- [ ] Sub-agents referenced appropriately (StreamHub only)
- [ ] Next steps are concrete and actionable
- [ ] Response length ≤300 words (concise)

## Success Criteria

- **80%+ scenarios pass**: Agents provide accurate, actionable guidance
- **Reference links resolve**: All instruction file and implementation links work
- **No duplication**: Agents reference instruction files, don't copy them
- **Appropriate redirects**: Agents recognize out-of-scope queries and redirect

## Failure Response

If any scenario fails:

1. Document the failure (query, expected, actual response)
2. Identify root cause (missing decision tree, incorrect reference, duplication)
3. Update agent definition file to address issue
4. Re-run failed scenario to verify fix
5. Document lessons learned in research.md or plan.md

## Next Steps

After quickstart validation complete:

1. Run `.specify/scripts/bash/update-agent-context.sh copilot` to update agent context
2. Proceed to Phase 2: Generate tasks.md via `/speckit.tasks`
3. After tasks.md complete, proceed to Phase 3+: Implementation via `/speckit.implement`
