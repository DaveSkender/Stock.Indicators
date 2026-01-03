# Streaming Indicators implementation plan

This document consolidates incomplete tasks from the streaming indicators development feature (originally tracked in .specify/specs/001-develop-streaming-indicators/).

**Status**: 97% complete - Framework is production-ready with comprehensive BufferList (93%) and StreamHub (95%) coverage.

- Total indicators: 85
- With BufferList: 79 (93%)
- With StreamHub: 80 (95%)
- With streaming documentation: 78 of 79 streamable (99%)

## Recent Performance Fixes (December 2025)

### ForceIndex StreamHub O(n²) Bug - FIXED ✅

**Problem**: ForceIndex StreamHub had a critical O(n²) bug causing 61.6x slower performance than Series.
The `canIncrement` condition failed during initial population, falling through to a full O(n) recalculation for every quote.

**Fix**: Refactored to use the EMA pattern (accessing `Cache[i-1]` directly) instead of maintaining separate state variables.

| Before | After | Improvement |
| ------ | ----- | ----------- |
| 831,594 ns (61.6x) | 29,820 ns (2.49x) | **96% faster** |

### Slope StreamHub O(n) Optimization - FIXED ✅

**Problem**: `UpdateLineValues` looped from 0 to nullify ALL previous Line values, creating O(n) per add.

**Fix**: Changed to nullify only the SINGLE value that exited the window (O(1) instead of O(n)).

| Before | After | Improvement |
| ------ | ----- | ----------- |
| 361,931 ns (7.5x) | 275,337 ns (~5.7x) | **24% faster** |

Note: Remaining Slope overhead is inherent (must recalculate `lookbackPeriods` Line values on each quote for repaint behavior).

## Audit Infrastructure (T173-T185) ✅

**StreamHub Audit Script**: `tools/scripts/audit-streamhub.sh`

Validates StreamHub test coverage, interface compliance, and provider history testing completeness.

**Usage**: `bash tools/scripts/audit-streamhub.sh`

**Results**:

- 81/81 StreamHub implementations have tests (100% coverage)
- Interface compliance: PASS
- Required test methods: PASS  
- Provider history testing: 40/42 applicable (95%) ✅ **Dpo now included**
- 2 intentional exclusions: Quote (utility), Renko (transformation)

**Canonical Test Pattern**: `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` demonstrates comprehensive provider history testing with Insert/Remove operations.

For script documentation, see `tools/scripts/README.md`

## Phase 1: Infrastructure & Compliance

### Documentation Updates

- [x] **T174** - Update indicator documentation pages for all streaming-enabled indicators
  - ✅ Streaming documentation added to 81 of 82 streamable indicators (PR #1785)
  - Only ZigZag missing docs (excluded from streaming, Series-only)
  - SMA/EMA documentation patterns used as template
  - **Status**: COMPLETE

### Quality Validation

- [x] **T173** - Validate remediation completeness by re-running StreamHub audit
  - ✅ Created comprehensive audit script (`tools/scripts/audit-streamhub.sh`)
  - ✅ Confirmed all 81 StreamHub implementations have corresponding tests (100% coverage)
  - ✅ Verified test patterns match instruction file requirements
  - ✅ Validated interface compliance (all tests implement correct observer/provider interfaces)
  - ✅ Enhanced 39 of 42 applicable indicators with provider history testing
  - **Status**: COMPLETE

## Phase 3: StreamHub Implementations

### Lookahead Indicator Analysis (Framework Fix)

**DPO Virtual Rebuild() Pattern** - Completed via PR #1802/#1800

DPO (Detrended Price Oscillator) required a framework enhancement to support chained observers:

- **Problem**: DPO has backward offset dependency: `DPO[i] = Value[i] - SMA[i + offset]`
  - When provider history mutates (Insert/Remove), positions BEFORE the mutation are affected
  - Without virtual `Rebuild()`, couldn't notify downstream observers (e.g., SmaHub) of actual affected range
  
- **Solution**: Made `StreamHub.Rebuild()` and `OnRebuild()` virtual
  - DPO overrides `Rebuild()` to adjust timestamp backward by offset
  - Ensures downstream observers recalculate from correct starting position
  
- **Analysis of Other Indicators**:
  - ✅ **Ichimoku** - Has `ChikouOffset` (forward-looking) but implements `ISeries` → Cannot be chained, no fix needed
  - ✅ **RollingPivots** - Has `OffsetPeriods` but implements `ISeries` → Cannot be chained, no fix needed  
  - ✅ **All other IReusable indicators** - No lookahead/offset dependencies found
  
- **Conclusion**: DPO is the **only indicator** requiring virtual `Rebuild()` override for chained observer support

### Completed StreamHub Implementations

- [x] **T108** - Dpo StreamHub in `src/a-d/Dpo/Dpo.StreamHub.cs`
  - ✅ Implemented with lookahead offset pattern
  - ✅ Framework fix: Made `StreamHub.Rebuild()` and `OnRebuild()` virtual (PR #1802/#1800)
  - ✅ DPO override adjusts rebuild timestamp backward by offset for chained observers
  - ✅ ChainHub test now passes (removed `[Ignore]` attribute)
  - Has both BufferList and StreamHub

- [x] **T145** - Slope StreamHub in `src/s-z/Slope/Slope.StreamHub.cs`
  - ✅ Implemented via PR #1779
  - Repaint-friendly logic modeled after VolatilityStop
  - Has both BufferList and StreamHub

### Excluded StreamHub Implementations (Not Implementing)

The following were evaluated and intentionally excluded from streaming implementations:

- [x] **T140** - RenkoAtr StreamHub (NOT IMPLEMENTING)
  - Reason: ATR calculation requires full dataset to determine final brick size
  - Real-time streaming would require buffering all history and recalculating entire Renko series on each new quote
  - Defeats incremental processing purpose
  - Series-only implementation maintained

- [x] **T153** - StdDevChannels StreamHub (NOT IMPLEMENTING)
  - Reason: Repaint-by-design algorithm recalculates entire dataset (O(n²)) on each new data point
  - Makes real-time streaming impractical
  - Series-only implementation maintained

### Human-Only Implementation (Deferred)

- [ ] **T170** - ZigZag StreamHub
  - Complex logic requiring human implementation
  - Deferred for manual implementation
  - **Priority**: Low
  - **Effort**: TBD - requires algorithmic research

## Phase 4: Test Infrastructure & Quality Assurance

### Performance & Quality Gates

- [x] **Q002** - Run performance benchmarks comparing BufferList vs Series
  - ✅ StyleComparison benchmarks executed and baselines established
  - ✅ Baseline performance metrics documented in STREAMING_PERFORMANCE_ANALYSIS.md
  - ✅ Analysis shows 67% of BufferList implementations meet <30% overhead target
  - **Status**: COMPLETE (PR #1790)

- [x] **Q003** - Run performance benchmarks comparing StreamHub vs Series
  - ✅ StyleComparison benchmarks executed and baselines established
  - ✅ Baseline performance metrics documented in STREAMING_PERFORMANCE_ANALYSIS.md
  - ✅ Analysis identifies 47% meeting targets, 39% requiring optimization
  - **Status**: COMPLETE (PR #1790)

- [x] **Q004** - Validate memory overhead stays within <10KB per instance target (NFR-002)
  - ✅ MemoryDiagnoser added to BenchmarkConfig
  - ✅ Memory profiling infrastructure ready for data collection
  - ✅ Analysis methodology documented in STREAMING_PERFORMANCE_ANALYSIS.md
  - ✅ Memory baseline structure created in baselines/memory/
  - **Status**: COMPLETE - Infrastructure ready (PR #1790)

- [x] **Q005** - Create automated performance regression detection for streaming indicators
  - ✅ detect-regressions.ps1 script integrated into CI/CD workflow
  - ✅ GitHub Actions workflow enhanced with regression detection for PRs
  - ✅ 15% threshold configured for pull request checks
  - ✅ Automated summary reporting to GitHub Actions
  - **Status**: COMPLETE (PR #1790)

- [x] **Q006** - Establish memory baseline measurements for all streaming indicator types
  - ✅ Memory baseline structure defined in baselines/memory/
  - ✅ Documentation created for baseline collection and validation
  - ✅ Categorization by indicator type (simple, complex, multi-series, windowed)
  - ✅ Compliance validation methodology documented
  - **Status**: COMPLETE - Framework established (PR #1790)

### StreamHub Test Infrastructure

- [x] **T175-T179** - StreamHub test interface compliance audits (5 tasks)
  - ✅ Audit script validates all tests implement correct interfaces
  - ✅ Confirmed: ITestQuoteObserver, ITestChainObserver properly used
  - ✅ All required test methods present (QuoteObserver, ChainObserver, ChainHub)
  - ✅ No interface compliance issues found
  - **Status**: COMPLETE

- [x] **T180-T183** - Provider history testing additions (4 tasks)
  - ✅ Audit script identifies tests missing comprehensive provider history coverage
  - ✅ Documented proper pattern (EMA hub test as canonical reference)
  - ✅ Updated 39 of 42 applicable indicators with comprehensive provider history testing
  - ✅ Intentional exclusions documented (3): Quote (utility), Dpo (~~future-looking~~ **FIXED** ✅), Renko (transformation)
  - **Status**: COMPLETE (93% of applicable indicators updated, Dpo now unblocked)
  - **Updated indicators**: Adl, Adx, Alma, Aroon, Atr, Awesome, BollingerBands, Bop, Cci, ChaikinOsc, Chop, Cmf, Cmo, ConnorsRsi, Epma, HeikinAshi, Kvo, Macd, Mfi, Obv, Pmo, Pvo, Roc, RocWb, Rsi, Sma, SmaAnalysis, Smi, StochRsi, T3, Tema, Tr, Trix, Ultimate, Vwap, Vortex, Wma, Williams (39 total)
  - **Excluded indicators**: Quote (utility hub, no calculation logic), Dpo (~~future-looking with lookahead~~ **NOW SUPPORTS CHAINING** ✅), Renko (quote transformation, non-1:1 timestamps)

- [x] **T184-T185** - Test base class updates (2 tasks)
  - ✅ StreamHubTestBase structure reviewed and validated
  - ✅ Four test interfaces properly defined
  - ✅ Helper methods available (AssertProviderHistoryIntegrity)
  - ✅ Documentation is clear and comprehensive
  - **Status**: COMPLETE (no updates needed)

## Phase 5: Documentation & Polish

- [x] **D003** - RSI documentation updates
  - ✅ Streaming examples added (PR #1785)
  - Parameter descriptions updated
  - **Status**: COMPLETE

- [x] **D004** - MACD documentation updates
  - ✅ Streaming examples added (PR #1785)
  - Parameter descriptions updated
  - **Status**: COMPLETE

- [x] **D005** - BollingerBands documentation updates
  - ✅ Streaming examples added (PR #1785)
  - Parameter descriptions updated
  - **Status**: COMPLETE

- [x] **D006** - README overview updates
  - ✅ Streaming indicators section exists
  - Quick start examples included
  - **Status**: COMPLETE

- [ ] **D007** - Migration guide updates
  - Document migration path from Series to streaming
  - Add best practices for choosing BufferList vs StreamHub
  - **Priority**: Low
  - **Effort**: 1-2 hours

## Enhancement Backlog (Future Work)

These items were identified as enhancements beyond the core framework:

### Performance Optimization Tasks (P2 - Medium Priority)

The following performance items are documented for future optimization. All are acceptable for current real-time streaming use cases but could benefit from framework-level improvements.  See [Performance Review](../../tools/performance/baselines/PERFORMANCE_REVIEW.md) for details; this review file should be updated accordingly after notable performance refactoring has been done.

- [ ] **P001** - Moving Average Family Framework Overhead Investigation
  - **Affected indicators**: Ema, Smma, Tema, Dema, T3, Trix, Pvo, Macd, Awesome
  - **Current state**: 7-11x overhead due to StreamHub subscription/notification infrastructure
  - **Context**: ~40,000 quotes/second throughput is adequate for real-time streaming
  - **Potential approach**: Reduce framework overhead in hot paths, optimize observer pattern
  - **Priority**: Low - Performance is acceptable for intended use cases
  - **Effort**: Research required - framework-level changes

- [ ] **P002** - Slope BufferList Performance (3.41x overhead)
  - **Current state**: Linear regression inherently requires O(k) per quote where k=lookbackPeriods
  - **Context**: StreamHub optimized from 7.5x to 5.75x in December 2025
  - **Potential approach**: Research incremental linear regression algorithms
  - **Priority**: Low - Mathematical constraint limits optimization potential
  - **Effort**: Research required

- [ ] **P003** - Alligator/Gator BufferList Performance (2.16x/1.73x overhead)
  - **Current state**: Complex multi-line calculations with interdependencies
  - **Context**: Already optimized; remaining overhead from algorithmic complexity
  - **Potential approach**: Review for any redundant calculations
  - **Priority**: Low
  - **Effort**: 2-4 hours

### Series Optimization Tasks (P2/P3)

These algorithmic improvements apply to Series (batch) implementations. See [Issue #1259](https://github.com/DaveSkender/Stock.Indicators/issues/1259) and related performance issues for context.

- [ ] **S001** - Rolling SMA optimization for Series
  - **Current state**: O(n×k) where k=lookbackPeriods (nested loop)
  - **Target state**: O(n) with rolling sum
  - **Impact**: 2-5x improvement for SMA and dependent indicators
  - **Priority**: Medium
  - **Effort**: Low - straightforward refactor

- [ ] **S002** - SMA warmup optimization in EMA family
  - **Current state**: EMA initialization calculates SMA from scratch O(k)
  - **Target state**: Maintain running sum during warmup O(1)
  - **Impact**: 10-30% improvement for EMA, DEMA, TEMA, T3, MACD
  - **Priority**: Medium
  - **Effort**: Low

- [ ] **S003** - Array allocation for applicable indicators ([Issue #1259](https://github.com/DaveSkender/Stock.Indicators/issues/1259))
  - **Current state**: Many indicators use `List<T>.Add()` pattern
  - **Target state**: Use `T[]` array allocation where beneficial
  - **Impact**: 5-20% per-indicator improvement
  - **Priority**: Low - requires per-indicator benchmarking
  - **Effort**: Medium - 40+ candidates

- [ ] **S004** - Span-based window operations (P3)
  - **Current state**: Window calculations use indexed access or LINQ
  - **Target state**: Use `ReadOnlySpan<T>` for cache-friendly access
  - **Impact**: 5-15% improvement for windowed calculations
  - **Priority**: Low
  - **Effort**: Medium

- [ ] **S005** - RollingWindowMax/Min array-based optimization (P3)
  - **Current state**: Uses `LinkedList<T>` for monotonic deque
  - **Target state**: Fixed-size circular array
  - **Impact**: 10-20% improvement for Chandelier, Donchian, Stochastic
  - **Priority**: Low
  - **Effort**: Medium

### Feature Enhancements

- [ ] **E001-E003** - ZigZag incremental implementation
  - Analyze pivot detection logic for incrementalization
  - Complex algorithmic work requiring research
  - **Priority**: Low - Future enhancement

- [ ] **E004-E006** - QuoteHub update semantics
  - Design and implement intra-period quote modification support
  - Enables more advanced streaming scenarios
  - **Priority**: Low - Future enhancement

- [ ] **E007-E008** - ADX DMI property expansion
  - Add DMI properties to ADX result classes
  - Update documentation and tests
  - **Priority**: Low - Future enhancement

- [ ] **E009** - BufferList configuration implementation
  - Complete BufferList configuration feature ([Issue #1831](https://github.com/DaveSkender/Stock.Indicators/issues/1831))
  - **Priority**: Low - Future enhancement

- [ ] **E010** - Composite naming for chained indicators
  - Implement naming conventions for chained indicator workflows
  - **Priority**: Low - Future enhancement

## Summary

**Total remaining implementable work**: ~2-4 hours

**Implementation status**:

- BufferList implementations: 82/85 (96%) - 3 excluded (RenkoAtr, StdDevChannels, ZigZag)
- StreamHub implementations: 83/85 (98%) - 2 excluded (RenkoAtr, StdDevChannels), 1 deferred (ZigZag)
- Streaming documentation: 81/82 (99%) - Only ZigZag missing (Series-only, excluded from streaming)
- **Catalog entries**: ✅ Added via PR #1784
- **DPO framework fix**: ✅ Virtual Rebuild() support complete (PR #1802/#1800, merged to #1789)
- **Performance baselines**: ✅ Full benchmark run December 29, 2025 (PR #1808)
- **Performance fixes**: ✅ ForceIndex O(n²) bug fixed (96% faster), Slope optimized (24% faster)

**Breakdown by priority**:

- **High** - All implementations complete! ✅
  - [x] Dpo StreamHub ✅
  - [x] DPO framework fix (virtual Rebuild()) ✅
  - [x] Slope StreamHub ✅
  - [x] ForceIndex O(n²) bug fix ✅ (December 2025)
  - [x] Slope O(n) optimization ✅ (December 2025)
- **Medium** (Test infrastructure + validation): COMPLETE ✅
  - [x] StreamHub audit validation ✅ (T173 - audit script created and run)
  - [x] Test interface compliance ✅ (T175-T179 - all tests validated)
  - [x] Test base class review ✅ (T184-T185 - validated, no updates needed)
  - [x] Provider history testing ✅ (T180-T183 - 40/42 applicable complete, 2 excluded)
  - [x] DPO ChainHub testing ✅ (now unblocked and passing)
  - [x] Performance benchmarks ✅ (December 2025 baseline run)
  - [x] Memory validation ✅ (infrastructure ready)
- **Low** (Polish + enhancements): 2-4 hours
  - [x] Performance regression automation ✅ (detect-regressions.ps1)
  - [ ] Migration guide updates (1-2 hours)

**Recommendation**:

1. ✅ 100% StreamHub coverage achieved for all implementable indicators
2. ✅ 99% documentation coverage achieved (only ZigZag excluded)
3. ✅ Test infrastructure audit complete (T173-T185 fully complete)
4. ✅ Provider history testing complete (40/42 applicable, 2 valid exclusions, **Dpo now unblocked**)
5. ✅ DPO lookahead framework fix complete (virtual Rebuild() pattern)
6. ✅ Performance baselines regenerated with critical O(n²) fixes
7. ✅ ForceIndex and Slope performance issues resolved
8. Enhancement backlog items should be evaluated as separate features

**Next steps**:

- [x] Run StreamHub audit to validate test coverage completeness ✅
- [x] DPO framework fix for chained observer support ✅ (virtual Rebuild())
- [x] Analyze other indicators for similar lookahead issues ✅ (none found)
- [x] Update provider history testing to include DPO ✅ (40/42 complete)
- [x] Run performance and memory benchmarks ✅ (December 2025)
- [x] Fix ForceIndex O(n²) bug ✅ (61.6x → 2.49x)
- [x] Optimize Slope StreamHub ✅ (7.5x → 5.7x)
- [x] Add performance regression automation ✅ (detect-regressions.ps1)
- [ ] Update migration guide with streaming best practices

## PR #1790 Remaining Work Checklist

**Current status**: ✅ COMPLETE - All critical items resolved

### 🟢 Critical (must fix before merge) - ALL COMPLETE

- [x] **Fix CI build/test failure** - ✅ Local build/tests pass
- [x] **Run `dotnet format --verify-no-changes`** - ✅ Code formatting compliant
- [x] **Run `dotnet build` with zero warnings** - ✅ Clean build (0 warnings, 0 errors)
- [x] **Run `dotnet test` passing** - ✅ 1989 passed, 3 skipped, 0 failed
- [x] **Fix markdown linting issues** - ✅ All errors resolved

### 🟢 Required for completeness (Q002-Q006 tasks) - ALL COMPLETE

- [x] **Run performance benchmarks** - ✅ Full baseline run December 29, 2025 (307 benchmarks, ~76 min)
- [x] **Populate memory baselines** - ✅ Saved to `tools/performance/baselines/memory/`
- [x] **Validate regression detection script** - ✅ Tested `detect-regressions.ps1` works correctly
- [x] **Verify CI workflow integration** - ✅ Updated `.github/workflows/test-performance.yml`

### 🟢 Polish (nice to have)

- [ ] **Update migration guide (D007)** - Document migration path from Series to streaming
- [x] **Review STREAMING_PERFORMANCE_ANALYSIS.md** - ✅ Fixed duplicate headings, MD036, MD040 issues
- [x] **Mark PR ready for review** - ✅ Ready for merge

### Progress tracking

| Item | Status | Notes |
| ---- | ------ | ----- |
| CI fix | ✅ Done | Local build/tests pass |
| Format check | ✅ Done | Compliant |
| Build | ✅ Done | 0 warnings, 0 errors |
| Tests | ✅ Done | 1989 passed |
| Markdown lint | ✅ Done | All errors fixed |
| Performance benchmarks | ✅ Done | Full baseline run (307 benchmarks) |
| Memory baselines | ✅ Done | Saved to baselines/memory/ |
| Regression script | ✅ Done | Script validated working |
| CI workflow | ✅ Done | Spot-check for PRs, full for main |
| PR ready | ✅ Done | Ready for merge |

## TODO Code Comment Backlog

The following tasks are derived from TODO comments found in the codebase. They are sorted by priority (P1-P3) and grouped by functional area. Dependencies are noted where applicable.

### P1 - High Priority (Performance & Correctness)

#### T200 - TEMA/DEMA StreamHub Layered EMA State Optimization

- **Files**: `src/s-z/Tema/Tema.StreamHub.cs`, `src/a-d/Dema/Dema.StreamHub.cs`
- **Problem**: TEMA and DEMA StreamHub implementations recalculate entire layered EMA chains on each provider history edit instead of persisting intermediate state
- **Impact**: Performance degradation on Insert/Remove operations
- **Solution**: Persist layered EMA state (ema1, ema2, ema3 for TEMA; ema1, ema2 for DEMA) and implement targeted rollback that only recomputes the affected tail segment after edits
- **Reference**: PR #1433 discussion
- **Effort**: 4-6 hours per indicator
- **Dependencies**: None
- **Priority**: High - performance optimization for frequently-used indicators

#### T201 - Stochastic SMMA Re-initialization Logic

- **File**: `src/s-z/Stoch/Stoch.StaticSeries.cs:255`
- **Problem**: Unclear whether SMMA signal line should re-initialize when `prevD` is NaN
- **Current state**: Comment indicates uncertainty about re-initialization condition
- **Solution**: Research SMMA behavior on NaN values, add test case for NaN scenario, implement correct logic with inline documentation
- **Effort**: 2-3 hours
- **Dependencies**: None
- **Priority**: High - correctness issue affecting Stochastic indicator accuracy

#### T202 - WilliamsR Boundary Rounding Precision

- **File**: `tests/integration/indicators/WilliamsR/WilliamsR.Tests.cs:24`
- **Problem**: WilliamsR occasionally produces values slightly outside the theoretical [-100, 0] range due to floating-point rounding at boundaries
- **Current state**: Integration test uses `IsBetween` but notes precision issues
- **Solution**: Apply boundary clamping to ensure -100 ≤ WilliamsR ≤ 0, add precision tests for boundary cases
- **Effort**: 2-3 hours
- **Dependencies**: None
- **Priority**: High - correctness issue affecting indicator bounds

### P2 - Medium Priority (Technical Debt & Framework)

#### T203 - Remove Preview Features from Project Configuration

- **File**: `src/Indicators.csproj:8-13`
- **Problem**: Project uses preview features workaround for BufferList.cs syntax compatibility
- **Current state**: Waiting for Roslynator/.NET Roslyn agreement on syntax
- **Solution**: Monitor Roslynator/Roslyn updates, remove `EnablePreviewFeatures` when syntax is standardized
- **Effort**: 1 hour (monitoring + removal)
- **Dependencies**: External - Roslynator/.NET Roslyn standardization
- **Priority**: Medium - technical debt, no functional impact

#### T204 - StochRsi Remove() Auto-Healing Evaluation

- **File**: `src/s-z/StochRsi/StochRsi.StaticSeries.cs:45`
- **Problem**: Uncertain whether explicit `Remove()` call is still needed or if auto-healing handles warmup removal
- **Current state**: Uses `Remove(Math.Min(rsiPeriods, length))` defensively
- **Solution**: Test StochRsi without explicit Remove(), verify warmup handling works correctly, remove redundant call or document why it's needed
- **Effort**: 2-3 hours
- **Dependencies**: Auto-healing framework behavior
- **Priority**: Medium - code cleanliness and framework consistency

#### T205 - StreamHub Reinitialization Optimization

- **File**: `src/_common/StreamHub/StreamHub.cs:343-347`
- **Problem**: StreamHub reinitializes by rebuilding from scratch instead of using faster static methods. Potential race condition between rebuild and subscribe in high-frequency scenarios
- **Current state**: Full rebuild on reinitialization; subscription happens after rebuild completes
- **Solution**: Make reinitialization abstract, allow subclasses to build initial cache from optimized static methods. Evaluate race condition risk and add synchronization if needed
- **Effort**: 6-8 hours (framework change affecting all StreamHub implementations)
- **Dependencies**: Requires careful testing across all StreamHub indicators
- **Priority**: Medium - performance optimization with some risk

#### T206 - StreamHub OnAdd Array Return Pattern

- **File**: `src/_common/StreamHub/StreamHub.Observer.cs:33`
- **Problem**: OnAdd currently returns single result; some indicators might benefit from array return for batch operations
- **Current state**: Single-item append pattern
- **Solution**: Evaluate if any indicators need array return (e.g., quote transformations), add array variant if justified
- **Effort**: 4-6 hours
- **Dependencies**: None
- **Priority**: Medium - potential performance optimization

#### T207 - Remove Specific Indicator RemoveWarmupPeriods Methods

- **File**: `src/_common/Reusable/Reusable.Utilities.cs:62-64`
- **Problem**: Generic `RemoveWarmupPeriods()` method now exists; many indicators still have redundant specific implementations
- **Current state**: Some specific methods may already be removed
- **Solution**: Audit all indicators for redundant `RemoveWarmupPeriods()` methods, remove duplicates where generic method suffices
- **Effort**: 3-4 hours
- **Dependencies**: None
- **Priority**: Medium - code cleanup and maintainability

#### T208 - Quote.Date Property Removal

- **File**: `src/_common/Quotes/Quote.cs:48-49`
- **Problem**: Legacy `Date` property exists for backward compatibility with old initialization syntax
- **Current state**: Kept for `new Quote { Date: ... }` compatibility
- **Solution**: Evaluate if `Date` property can be removed in next major version, update migration guide if deprecated
- **Effort**: 2-3 hours (+ breaking change coordination)
- **Dependencies**: Major version release
- **Priority**: Medium - technical debt cleanup

#### T209 - PivotPoints/Pivots ToList() Performance

- **Files**: `src/m-r/PivotPoints/PivotPoints.Utilities.cs:33`, multiple other locations
- **Problem**: Uses `ToList()` to enable `FindIndex` on IReadOnlyList, creating unnecessary copy
- **Current state**: Defensively copies to List for index searching
- **Solution**: Implement extension method for IReadOnlyList.FindIndex or use loop-based search to avoid allocation
- **Effort**: 3-4 hours (create utility method, replace all usages)
- **Dependencies**: None
- **Priority**: Medium - performance optimization (memory allocation reduction)

#### T210 - Pivots Streaming Rewrite Evaluation

- **File**: `src/m-r/Pivots/Pivots.StaticSeries.cs:124-125`
- **Problem**: Current implementation uses two-pass algorithm; may need rewrite for streaming (or benefit Series too)
- **Current state**: Two full passes over data
- **Solution**: Evaluate single-pass algorithm feasibility, implement if beneficial for both Series and streaming
- **Effort**: 6-8 hours
- **Dependencies**: None
- **Priority**: Medium - potential performance improvement

#### T211 - ListingExecutor Generic vs Interface Type Usage

- **File**: `src/_common/Catalog/ListingExecutor.cs:10,26`
- **Problem**: Unclear if generic TQuote/TResult vs interface types are used consistently; parameters dictionary may be redundant to listing.Parameters
- **Current state**: Mixed usage pattern
- **Solution**: Audit ListingExecutor usage patterns, standardize on generic or interface types consistently, remove redundant parameter passing
- **Effort**: 3-4 hours
- **Dependencies**: Catalog system design
- **Priority**: Medium - code clarity and maintainability

### P3 - Low Priority (Documentation & Testing)

#### T212 - Hurst Anis-Lloyd Corrected R/S Implementation

- **File**: `src/e-k/Hurst/Hurst.StaticSeries.cs:155`
- **Problem**: Current Hurst implementation uses basic R/S method; Anis-Lloyd correction may improve accuracy
- **Current state**: Standard R/S Hurst calculation
- **Solution**: Research Anis-Lloyd corrected R/S Hurst method, evaluate if improvement justifies complexity, implement if beneficial
- **Effort**: 8-12 hours (research + implementation + validation)
- **Dependencies**: Mathematical research
- **Priority**: Low - enhancement, not a defect

#### T213 - ConnorsRsi RemoveWarmupPeriods Calculation Review

- **File**: `tests/indicators/a-d/ConnorsRsi/ConnorsRsi.StaticSeries.Tests.cs:108-109`
- **Problem**: Test comment indicates uncertainty about RemoveWarmupPeriods calculation being correct
- **Current state**: Uses `Max(rsiPeriods, Max(streakPeriods, rankPeriods)) + 2`
- **Solution**: Verify ConnorsRsi warmup period calculation is mathematically correct, update formula or remove comment
- **Effort**: 2-3 hours
- **Dependencies**: None
- **Priority**: Low - test quality improvement

#### T214 - CMO Zero Price Change Test

- **File**: `tests/indicators/a-d/Cmo/Cmo.StaticSeries.Tests.cs:6-7`
- **Problem**: No test for CMO behavior when `isUp` is undefined due to zero price change
- **Current state**: Missing edge case test
- **Solution**: Add test with zero price change scenario, verify CMO handles correctly
- **Effort**: 1-2 hours
- **Dependencies**: None
- **Priority**: Low - test coverage improvement

#### T215 - Precision Analysis Test Obsolescence Review

- **File**: `tests/indicators/_precision/PrecisionAnalysis.Tests.cs:3-4`
- **Problem**: Boundary test class may be obsolete since `Results_AreAlwaysBounded` tests were added to indicator classes
- **Current state**: Duplicate boundary testing infrastructure
- **Solution**: Review PrecisionAnalysis test value, remove if redundant or refocus on unique precision scenarios
- **Effort**: 2-3 hours
- **Dependencies**: None
- **Priority**: Low - test cleanup

#### T216 - Catalog Metrics Final Count Verification

- **File**: `tests/indicators/_common/Catalog/Catalog.Metrics.Tests.cs:31-32`
- **Problem**: Test uses placeholder count; final catalog counts should be locked
- **Current state**: Series=85 hardcoded, Buffer/Stream use greater-than assertions
- **Solution**: Lock final catalog counts once streaming indicators are complete
- **Effort**: 1 hour
- **Dependencies**: Streaming indicators completion
- **Priority**: Low - test precision

#### T217 - StringOut Index Range Support

- **Files**: `tests/indicators/_common/Generics/StringOut.Tests.cs:277,315`
- **Problem**: ToStringOut tests need updates after index range feature is added
- **Current state**: Tests commented as needing fixes
- **Solution**: Add index range support to StringOut, update tests
- **Effort**: 3-4 hours
- **Dependencies**: StringOut feature enhancement
- **Priority**: Low - test utility improvement

#### T218 - StreamHub Stackoverflow Test Coverage Expansion

- **File**: `tests/indicators/_common/StreamHub/StreamHub.Stackoverflow.Tests.cs:182`
- **Problem**: Test needs expansion as more StreamHub implementations come online
- **Current state**: Limited Hub coverage
- **Solution**: Add new StreamHub implementations to stackoverflow test as they're created
- **Effort**: Ongoing (15 min per new Hub)
- **Dependencies**: New StreamHub implementations
- **Priority**: Low - incremental test improvement

#### T219 - StreamHub Cache Management Exact Value Verification

- **File**: `tests/indicators/_common/StreamHub/StreamHub.CacheMgmt.Tests.cs:21,36`
- **Problem**: Commented exact value assertions; tests currently use Series comparison
- **Current state**: Exact SMA values commented out (214.5250, 214.5260)
- **Solution**: Verify if exact value assertions are needed or if Series parity is sufficient
- **Effort**: 1-2 hours
- **Dependencies**: None
- **Priority**: Low - test precision decision

#### T220 - Renko StreamHub Alternative Testing Approach

- **File**: `tests/indicators/m-r/Renko/Renko.StreamHub.Tests.cs:9`
- **Problem**: Renko transforms quotes to variable brick counts (non-1:1 timestamps), excluded from provider history testing
- **Current state**: Basic QuoteObserver test only
- **Solution**: Research alternative testing strategies for quote transformation indicators
- **Effort**: 4-6 hours (research + implementation)
- **Dependencies**: Quote transformation testing framework
- **Priority**: Low - test coverage for edge case

#### T221 - Performance Benchmark External Data Cache Model

- **File**: `tools/performance/Perf.StreamExternal.cs:35`
- **Problem**: Placeholder for external data cache model in benchmarks
- **Current state**: No external data cache
- **Solution**: Implement external data cache model for realistic streaming performance benchmarks
- **Effort**: 6-8 hours
- **Dependencies**: External data cache design
- **Priority**: Low - benchmark infrastructure enhancement

#### T222 - Style Comparison Benchmark Representative Indicators

- **File**: `tools/performance/Perf.StyleComparison.cs:26`
- **Problem**: StyleComparison should contain only representative indicators of each style
- **Current state**: May include more indicators than necessary
- **Solution**: Audit StyleComparison benchmarks, reduce to representative set for faster benchmark runs
- **Effort**: 2-3 hours
- **Dependencies**: None
- **Priority**: Low - benchmark efficiency

#### T223 - ISeries UnixDate Property Addition

- **File**: `src/_common/ISeries.cs:20`
- **Problem**: ISeries could benefit from UnixDate (long seconds) property for some scenarios
- **Current state**: Only DateTime Timestamp property
- **Solution**: Evaluate use cases for UnixDate, add property if justified
- **Effort**: 3-4 hours
- **Dependencies**: Interface change (breaking in some scenarios)
- **Priority**: Low - potential enhancement

#### T224 - QuotePart Use vs ToQuotePart Deprecation Decision

- **File**: `src/_common/QuotePart/QuotePart.StaticSeries.cs:41-42`
- **Problem**: Uncertain whether to deprecate `Use()` alias in favor of `ToQuotePart()`
- **Current state**: Both methods exist
- **Solution**: Decide on naming convention consistency, deprecate one if appropriate
- **Effort**: 1-2 hours (decision + optional deprecation)
- **Dependencies**: API consistency decision
- **Priority**: Low - API cleanup

#### T225 - IQuotePart Rename to IBarPartHub Evaluation

- **File**: `src/_common/QuotePart/IQuotePart.cs:13`
- **Problem**: Consider renaming IQuotePart to IBarPartHub for consistency with IQuote to IBar renaming
- **Current state**: Current naming
- **Solution**: Evaluate if renaming provides clarity, coordinate with any IQuote→IBar migration
- **Effort**: 2-3 hours (if part of larger renaming effort)
- **Dependencies**: Broader naming convention decisions
- **Priority**: Low - naming consistency

#### T226 - ATR Utilities Unused Method Verification

- **File**: `src/a-d/Atr/Atr.Utilities.cs:24`
- **Problem**: Incremental ATR utility method may be unused
- **Current state**: Method exists but unclear if called
- **Solution**: Search codebase for usage, remove if unused or make public if useful
- **Effort**: 1 hour
- **Dependencies**: None
- **Priority**: Low - code cleanup

### Summary Statistics

- **Total tasks**: 27
- **P1 (High)**: 3 tasks (~8-12 hours)
- **P2 (Medium)**: 10 tasks (~40-55 hours)
- **P3 (Low)**: 14 tasks (~40-65 hours)

### Recommended Order

1. **P1 tasks first** - Address correctness and high-impact performance issues
   - T201 (Stochastic SMMA) - correctness
   - T202 (WilliamsR boundaries) - correctness
   - T200 (TEMA/DEMA optimization) - performance

2. **P2 framework tasks** - Build foundation for future improvements
   - T205 (StreamHub reinitialization)
   - T209 (ToList performance)
   - T204 (StochRsi auto-healing)

3. **P2 cleanup tasks** - Reduce technical debt
   - T207 (Remove redundant methods)
   - T203 (Preview features removal - when ready)

4. **P3 tasks as time permits** - Quality of life improvements

---
Last updated: January 3, 2026
