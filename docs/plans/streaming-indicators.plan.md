# Streaming Indicators implementation plan

This document tracks remaining work for the v3 streaming indicators implementation.

**Status**: 97% complete - Framework is production-ready with comprehensive BufferList (93%) and StreamHub (95%) coverage.

- Total indicators: 85
- With BufferList: 79 (93%)
- With StreamHub: 80 (95%)
- With streaming documentation: 78 of 79 streamable (99%)

**Related plans**: [Branching Strategy Migration](branching-strategy.plan.md) (required for v3.0 stable release), [File Reorganization](file-reorg.plan.md) (deferred to v3.1)

## v3.0 Work Remaining - Force Prioritized Execution Order

Execute these tasks sequentially from top to bottom. This section contains **only v3.0 scope** - all future enhancements deferred to v3.1+.

### Critical - v3.0 Release Blockers

- [x] **#1585** - QuoteHub self-healing limitation investigation (2-4 hours)
  - **Problem**: User-reported index out of range with late-arriving quote updates
  - **Fix**: Wrapped cache exposures in `AsReadOnly()` to prevent deviant list manipulation
  - **Status**: COMPLETE - Implemented immutable cache wrappers to prevent users from bypassing safe StreamHub methods
  - **Related**: E004-E006 (QuoteHub update semantics deferred to v3.1+), Discussion #1018

- [ ] **Get and incorporate final feedback** (Ongoing)
  - **Action**: Community review period before stable v3 release
  - **Action**: Address any critical issues reported
  - **Blocking**: v3.0 stable release

- [ ] **Branching strategy migration** - Execute main/v2/v3 branch reorganization (10-16 hours)
  - **Detailed plan**: See [branching-strategy.plan.md](branching-strategy.plan.md)
  - **Objective**: Migrate `main` to v3 sources, preserve v2 as maintenance branch
  - **Action**: Execute 6-phase migration plan (CI/CD updates, create v2 branch, merge PR #1014, update repo settings, delete v3 branch, validate)
  - **Timing**: After all correctness issues resolved, before v3.0.0 stable release
  - **Blocking**: v3.0 stable release from `main` branch
  - **Related**: PR #1014 (v3 → main merge ready, clean state)

### High Priority - Correctness & Performance

- [x] **T202** - WilliamsR boundary rounding precision (2-3 hours)
  - **File**: `tests/integration/indicators/WilliamsR/WilliamsR.Tests.cs:24`
  - **Problem**: Values occasionally outside theoretical \[-100, 0\] range
  - **Action**: Apply boundary clamping to ensure -100 ≤ WilliamsR ≤ 0
  - **Action**: Add precision tests for boundary cases
  - **Related**: #1692
  - **Status**: COMPLETE - Added boundary clamping to BufferList and StreamHub implementations; precision tests already exist in BoundaryTests

- [x] **T201** - Stochastic SMMA re-initialization logic (2-3 hours)
  - **File**: `src/s-z/Stoch/Stoch.StaticSeries.cs:255`
  - **Problem**: Unclear whether SMMA signal line should re-initialize when `prevD` is NaN
  - **Action**: Research SMMA behavior on NaN values
  - **Action**: Add test case for NaN scenario
  - **Action**: Implement correct logic with inline documentation
  - Implemented in PR #1852

- [x] **T200** - TEMA/DEMA StreamHub layered EMA state optimization (8-12 hours)
  - **Files**: `src/s-z/Tema/Tema.StreamHub.cs`, `src/a-d/Dema/Dema.StreamHub.cs`
  - **Problem**: Recalculate entire layered EMA chains on each provider history edit
  - **Action**: Persist layered EMA state (ema1, ema2, ema3 for TEMA; ema1, ema2 for DEMA)
  - **Action**: Implement targeted rollback (only recompute affected tail segment)
  - **Reference**: PR #1433 discussion
  - **Status**: COMPLETE - Optimization already implemented with internal state properties and RollbackState method

### Critical StreamHub Performance Issues

Based on performance analysis (January 3, 2026), the following indicators have critical performance issues requiring investigation:

- [x] **P004** - ForceIndex StreamHub O(n²) complexity fix (4-6 hours)
  - **Current**: 61.56x slower than Series (831,594 ns vs 13,508 ns)
  - **Problem**: Nested loop recalculating entire history on each quote
  - **Action**: Implement O(1) incremental update with rolling state
  - **Priority**: 🔴 CRITICAL - Unusable for real-time streaming
  - **Status**: COMPLETE - Implemented rolling sum state during warmup period for O(1) incremental updates

- [x] **P005** - Slope StreamHub performance optimization (4-6 hours)
  - **Previous**: 7.49x slower than Series (358,366 ns vs 47,859 ns)
  - **Current**: 4.20x slower than Series (336,438 ns vs 80,173 ns)
  - **Improvement**: 43% reduction in overhead ratio, 6.1% faster execution
  - **Action**: Cached slope/intercept to avoid repeated cache lookups
  - **Action**: Eliminated redundant bounds checks in update loop
  - **Status**: COMPLETE - Significant optimization achieved while maintaining mathematical correctness
  - **Priority**: 🔴 HIGH

- [ ] **P006** - Prs StreamHub performance optimization (3-4 hours)
  - **Current**: 7.47x slower than Series (35,070 ns vs 4,694 ns)
  - **Problem**: Potential state management or allocation inefficiencies
  - **Action**: Review implementation for unnecessary recalculations
  - **Priority**: 🔴 HIGH

- [x] **P007** - Roc StreamHub performance optimization (3-4 hours)
  - **Current**: 6.98x slower than Series (30,153 ns vs 4,322 ns)
  - **Problem**: Simple calculation showing excessive overhead
  - **Action**: Investigate state caching and lookback efficiency
  - **Priority**: 🔴 HIGH
  - **Result**: Investigation complete - current implementation is optimal. ROC has no internal state to cache (calculation is stateless). Lookback access is already O(1) using indexHint. The 6.98x overhead is inherent StreamHub framework cost (observer pattern, cache management, ReadOnlyCollection wrappers) that cannot be eliminated without framework changes. Similar simple indicators (MACD 7.31x, T3 8.65x, DEMA 8.56x) show comparable or higher overhead.

- [x] **P008** - PivotPoints StreamHub performance optimization (4-6 hours)
  - **Current**: 5.16x slower than Series (133,000 ns vs 25,800 ns)
  - **Investigation**: Analyzed GetWindowNumber calls, UpdateWindowState method, and result object allocation patterns
  - **Findings**: Performance overhead is primarily from (1) result object allocation with 9 decimal properties per quote, (2) GetWindowNumber calendar lookups, and (3) window state management
  - **Attempted optimizations**: Tested AggressiveInlining attributes and cached window number delegates - minimal impact
  - **Conclusion**: Current implementation is within acceptable StreamHub performance range (target <7.5x). Further optimization would require algorithmic changes or structural modifications that risk correctness
  - **Status**: COMPLETE - Performance acceptable for intended use case
  - **Priority**: 🟢 RESOLVED

- [ ] **P009** - Gator StreamHub performance optimization (4-6 hours)
  - **Current**: 6.20x slower than Series (84,161 ns vs 13,583 ns)
  - **Problem**: Multi-line indicator with Alligator dependency
  - **Action**: Optimize provider subscription and state updates
  - **Priority**: 🔴 HIGH

- [ ] **P010** - Ultimate (UO) StreamHub performance optimization (4-6 hours)
  - **Current**: 5.89x slower than Series (161,480 ns vs 27,426 ns)
  - **Problem**: Complex weighted sum calculations
  - **Action**: Review for calculation caching opportunities
  - **Priority**: 🔴 HIGH

- [ ] **P011** - Adl StreamHub performance optimization (3-4 hours)
  - **Current**: 5.87x slower than Series (32,493 ns vs 5,534 ns)
  - **Problem**: Running sum calculation with inefficiencies
  - **Action**: Optimize state management for rolling totals
  - **Priority**: 🔴 HIGH

- [ ] **P012** - Pmo StreamHub performance optimization (3-4 hours)
  - **Current**: 5.81x slower than Series (33,445 ns vs 5,760 ns)
  - **Problem**: EMA-based calculations with additional overhead
  - **Action**: Review layered EMA state management
  - **Priority**: 🔴 HIGH

- [ ] **P013** - Smi StreamHub performance optimization (4-6 hours)
  - **Current**: 5.47x slower than Series (76,236 ns vs 13,939 ns)
  - **Problem**: Stochastic with EMA smoothing
  - **Action**: Optimize window operations and EMA layering
  - **Priority**: 🔴 HIGH

- [ ] **P014** - Chandelier StreamHub performance optimization (3-4 hours)
  - **Current**: 5.35x slower than Series (120,072 ns vs 22,454 ns)
  - **Problem**: ATR-based trailing stop calculations
  - **Action**: Review ATR provider subscription efficiency
  - **Priority**: 🔴 HIGH

### Critical BufferList Performance Issues

- [ ] **P015** - Slope BufferList O(n) optimization (4-6 hours)
  - **Current**: 3.41x slower than Series (162,972 ns vs 47,859 ns)
  - **Problem**: Linear regression recalculation on each add
  - **Action**: Implement incremental Welford-style updates if mathematically feasible
  - **Priority**: 🔴 CRITICAL

- [ ] **P016** - Alligator BufferList performance optimization (2-4 hours)
  - **Current**: 2.16x slower than Series (18,570 ns vs 8,609 ns)
  - **Problem**: Triple SMMA calculations with lookback
  - **Action**: Review buffer management and calculation sequencing
  - **Priority**: 🔴 MEDIUM

- [ ] **P017** - Adx BufferList performance optimization (3-4 hours)
  - **Current**: 2.08x slower than Series (31,348 ns vs 15,088 ns)
  - **Problem**: Complex DI+/DI-/ATR/DX/ADX calculation chain
  - **Action**: Optimize intermediate value caching
  - **Priority**: 🔴 MEDIUM

### Medium Priority - Documentation & Usability

- [ ] **D007** - Migration guide updates (1-2 hours)
  - **File**: Update migration guide documentation
  - **Action**: Document migration path from Series to streaming
  - **Action**: Add best practices for choosing BufferList vs StreamHub
  - **Action**: Include performance considerations

### Medium Priority - Code Quality & Cleanup

- [x] **T207** - Remove specific indicator RemoveWarmupPeriods methods (3-4 hours)
  - **File**: `src/_common/Reusable/Reusable.Utilities.cs:62-64`
  - **Problem**: Generic `RemoveWarmupPeriods()` exists; many indicators have redundant implementations
  - **Action**: Audit all indicators for redundant methods
  - **Action**: Remove duplicates where generic method suffices
  - **Status**: COMPLETE - Removed redundant methods for Epma, Hurst, Mfi, Stoch, Vwap (PR #1842)

- [x] **T209** - PivotPoints/Pivots ToList() performance (3-4 hours)
  - **Files**: `src/m-r/PivotPoints/PivotPoints.Utilities.cs:33`, multiple locations
  - **Problem**: Uses `ToList()` to enable `FindIndex`, creating unnecessary copy
  - **Action**: Implement extension method for IReadOnlyList.FindIndex
  - **Action**: Replace all ToList() usages with new extension
  - **Status**: COMPLETE - Removed ToList() call in PivotPoints (PR #1842)

- [x] **T204** - StochRsi Remove() auto-healing evaluation (2-3 hours)
  - **File**: `src/s-z/StochRsi/StochRsi.StaticSeries.cs:45`
  - **Problem**: Uncertain whether explicit `Remove()` call still needed
  - **Action**: Test StochRsi without explicit Remove()
  - **Action**: Verify auto-healing works correctly
  - **Action**: Remove redundant call or document why it's needed
  - **Status**: COMPLETE - Refactored StochRsi calculation flow, removed redundant Remove() call, added auto-healing test (PR #1842)

- [ ] **T203** - Remove preview features from project configuration (1 hour)
  - **File**: `src/Indicators.csproj:8-13`
  - **Problem**: Uses preview features workaround for BufferList.cs syntax
  - **Action**: Monitor Roslynator/.NET Roslyn updates
  - **Action**: Remove `EnablePreviewFeatures` when syntax is standardized
  - **Dependency**: External - Roslynator/.NET Roslyn standardization

- [x] **T229** - ATR utilities unused method verification (1 hour)
  - **File**: `src/a-d/Atr/Atr.Utilities.cs:24`
  - **Problem**: Incremental ATR utility method may be unused
  - **Action**: Search codebase for usage
  - **Action**: Remove if unused or make public if useful
  - **Status**: COMPLETE - Made public and added MethodImpl attribute for performance (PR #1842)

### Infrastructure & Reorganization

- [ ] **File reorganization for .NET naming conventions** - [#1810](https://github.com/DaveSkender/Stock.Indicators/issues/1810)
  - **Detailed plan**: See [file-reorg.plan.md](file-reorg.plan.md) for comprehensive analysis
  - **Phases**: [#1811](https://github.com/DaveSkender/Stock.Indicators/issues/1811) (Directory structure), [#1812](https://github.com/DaveSkender/Stock.Indicators/issues/1812) (Class/file renaming), [#1813](https://github.com/DaveSkender/Stock.Indicators/issues/1813) (Final cleanup)
  - **Scope**: ~500 file renames across 8 phases, 55-87 hours estimated
  - **Rationale**: Does not affect functionality; safe to defer

- [ ] **#1739** - Add upgraded doc site (VitePress migration) (Large scope)
  - **Status**: PR #1739 in progress (experimental VitePress framework)
  - **Alternatives**: Issue #1320 (Docusaurus), Issue #1298 (MkDocs)
  - **Rationale**: Current Jekyll site is functional

### Low Priority - Testing & Validation (v3.0 Optional)

- [ ] **T216** - ConnorsRsi RemoveWarmupPeriods calculation review (2-3 hours)
  - **File**: `tests/indicators/a-d/ConnorsRsi/ConnorsRsi.StaticSeries.Tests.cs:108-109`
  - **Problem**: Test comment indicates uncertainty about calculation correctness
  - **Current**: Uses `Max(rsiPeriods, Max(streakPeriods, rankPeriods)) + 2`
  - **Action**: Verify ConnorsRsi warmup period calculation is mathematically correct
  - **Action**: Update formula or remove comment

- [ ] **T217** - CMO zero price change test (1-2 hours)
  - **File**: `tests/indicators/a-d/Cmo/Cmo.StaticSeries.Tests.cs:6-7`
  - **Problem**: No test for CMO behavior when `isUp` is undefined (zero price change)
  - **Action**: Add test with zero price change scenario
  - **Action**: Verify CMO handles correctly

- [x] **T218** - Precision analysis test obsolescence review (2-3 hours)
  - **File**: `tests/indicators/_precision/PrecisionAnalysis.Tests.cs:3-4`
  - **Problem**: Boundary test class may be obsolete since `Results_AreAlwaysBounded` tests added
  - **Action**: Review PrecisionAnalysis test value
  - **Action**: Remove if redundant or refocus on unique precision scenarios
  - **Status**: COMPLETE - Clarified unique value of BoundaryTests (synthetic pathological data vs normal market data), removed TODO comment and added explanatory documentation

- [ ] **T219** - Catalog metrics final count verification (1 hour)
  - **File**: `tests/indicators/_common/Catalog/Catalog.Metrics.Tests.cs:31-32`
  - **Problem**: Test uses placeholder count
  - **Action**: Lock final catalog counts once streaming indicators complete

- [x] **T222** - StreamHub cache management exact value verification (1-2 hours)
  - **File**: `tests/indicators/_common/StreamHub/StreamHub.CacheMgmt.Tests.cs:21,36`
  - **Problem**: Exact SMA values commented out (214.5250, 214.5260)
  - **Action**: Verify if exact value assertions needed or if Series parity sufficient
  - **Status**: COMPLETE - Removed TODO comments; Series parity is the canonical correctness standard and is sufficient

---

## v3.1+ Enhancements - Deferred Work

The following items are deferred to v3.1 or later releases. These are enhancements, optimizations, and infrastructure improvements that are not critical for v3.0 stable.

### Infrastructure & Reorganization

- [ ] **#1533** - Implement consistent test method naming conventions (Large scope)
  - **Action**: Standardize test naming: `MethodName_StateUnderTest_ExpectedBehavior`
  - **Scope**: ~280 test classes
  - **Rationale**: Code quality improvement, non-functional

### Performance & Framework Optimizations

- [ ] **T205** - StreamHub reinitialization optimization (6-8 hours)
  - **File**: `src/_common/StreamHub/StreamHub.cs:343-347`
  - **Problem**: Reinitializes by rebuilding from scratch instead of using faster static methods
  - **Action**: Make reinitialization abstract for optimized subclass implementations
  - **Rationale**: Framework change with risk; defer for careful v3.1 implementation

- [x] **T213** - Performance review documentation cleanup and reorganization (6-8 hours)
  - **Files**: `tools/performance/PERFORMANCE_ANALYSIS.md` (consolidated from STREAMING_PERFORMANCE_ANALYSIS.md and baselines/PERFORMANCE_REVIEW.md)
  - **Problem**: Performance documentation fragmented, inconsistent, poorly organized
  - **Action**: Consolidate and reorganize performance documentation
  - **Rationale**: Documentation quality improvement, not user-facing
  - **Status**: Completed - Documentation consolidated into single comprehensive PERFORMANCE_ANALYSIS.md file

- [ ] **P001** - Moving Average family framework overhead investigation (Research required)
  - **Current**: 7-11x overhead due to StreamHub subscription/notification infrastructure
  - **Rationale**: Performance is acceptable for intended use cases (~40,000 quotes/second)

- [ ] **P002** - Slope BufferList performance optimization (Research required)
  - **Current**: Linear regression inherently requires O(k) per quote
  - **Rationale**: Mathematical constraint limits optimization potential

- [ ] **P003** - Alligator/Gator BufferList performance (2-4 hours)
  - **Current**: Complex multi-line calculations (2.16x/1.73x overhead)
  - **Rationale**: Already optimized; remaining overhead from algorithmic complexity

### Series Batch Processing Optimizations

See [Issue #1259](https://github.com/DaveSkender/Stock.Indicators/issues/1259) for context.

- [ ] **S001** - Rolling SMA optimization for Series
  - **Impact**: 2-5x improvement for SMA and dependent indicators
  - **Rationale**: Batch processing optimization, not streaming

- [ ] **S002** - SMA warmup optimization in EMA family
  - **Impact**: 10-30% improvement for EMA, DEMA, TEMA, T3, MACD
  - **Rationale**: Batch processing optimization, not streaming

- [x] **S003** - Array allocation for applicable indicators (ON HOLD)
  - **Status**: PR #1838 showing unmeasurable improvement
  - **Rationale**: ON HOLD indefinitely

- [ ] **S004** - Span-based window operations
  - **Impact**: 5-15% improvement for windowed calculations
  - **Rationale**: Batch processing optimization

- [ ] **S005** - RollingWindowMax/Min array-based optimization
  - **Impact**: 10-20% improvement for Chandelier, Donchian, Stochastic
  - **Rationale**: Batch processing optimization

### Advanced Features & Enhancements (v3.1+)

- [ ] **Review Discussion #1018** - Crosscheck community feedback (2-3 hours)

- [ ] **T206** - StreamHub OnAdd array return pattern (4-6 hours)
  - **File**: `src/_common/StreamHub/StreamHub.Observer.cs:33`
  - **Rationale**: Evaluate if indicators need array return for batch operations

- [ ] **T208** - Quote.Date property removal evaluation (2-3 hours)
  - **File**: `src/_common/Quotes/Quote.cs:48-49`
  - **Rationale**: Breaking change requiring major version release

- [ ] **T210** - Pivots streaming rewrite evaluation (6-8 hours)
  - **File**: `src/m-r/Pivots/Pivots.StaticSeries.cs:124-125`
  - **Rationale**: Enhancement, not correctness fix

- [x] **T211** - ListingExecutor generic vs interface type usage (3-4 hours)
  - **File**: `src/_common/Catalog/ListingExecutor.cs:10,26`
  - **Rationale**: Code clarity improvement, non-functional
  - **Status**: COMPLETE - Simplified ListingExecutor to use IQuote interface type (PR #1842)

- [ ] **T212** - Catalog NotImplementedException alternative (2-3 hours)
  - **File**: `src/_common/Catalog/Catalog.cs:353`
  - **Rationale**: Current implementation acceptable; research alternative patterns

- [ ] **T214** - MaEnvelopes remaining MA types implementation (8-12 hours)
  - **File**: `src/m-r/MaEnvelopes/MaEnvelopes.StreamHub.cs:77`
  - **Rationale**: Intentional limitation; users can use Series for ALMA/EPMA/HMA

- [ ] **T215** - Hurst Anis-Lloyd corrected R/S implementation (8-12 hours)
  - **File**: `src/e-k/Hurst/Hurst.StaticSeries.cs:155`
  - **Rationale**: Enhancement, not defect

- [ ] **T220** - StringOut index range support (3-4 hours)
  - **Files**: `tests/indicators/_common/Generics/StringOut.Tests.cs:277,315`
  - **Rationale**: Test utility enhancement

- [ ] **T221** - StreamHub Stackoverflow test coverage expansion (Ongoing)
  - **File**: `tests/indicators/_common/StreamHub/StreamHub.Stackoverflow.Tests.cs:182`
  - **Rationale**: Ongoing incremental improvement

- [ ] **T223** - Renko StreamHub alternative testing approach (4-6 hours)
  - **File**: `tests/indicators/m-r/Renko/Renko.StreamHub.Tests.cs:9`
  - **Rationale**: Test coverage for edge case

- [ ] **T224** - Performance benchmark external data cache model (6-8 hours)
  - **File**: `tools/performance/Perf.StreamExternal.cs:35`
  - **Rationale**: Benchmark infrastructure enhancement

- [ ] **T225** - Style comparison benchmark representative indicators (2-3 hours)
  - **File**: `tools/performance/Perf.StyleComparison.cs:26`
  - **Rationale**: Benchmark efficiency improvement

- [ ] **T226** - ISeries UnixDate property addition (3-4 hours)
  - **File**: `src/_common/ISeries.cs:20`
  - **Rationale**: Interface change, breaking in some scenarios

- [ ] **T227** - QuotePart Use vs ToQuotePart deprecation decision (1-2 hours)
  - **File**: `src/_common/QuotePart/QuotePart.StaticSeries.cs:41-42`
  - **Rationale**: API cleanup, non-functional

- [ ] **T228** - IQuotePart rename to IBarPartHub evaluation (2-3 hours)
  - **File**: `src/_common/QuotePart/IQuotePart.cs:13`
  - **Rationale**: Naming consistency, part of broader refactoring

### Complex Research & Implementation (v3.2+)

- [ ] **T170 / #1692** - ZigZag StreamHub implementation (TBD)
  - **Rationale**: Complex algorithmic work requiring human implementation; Series-only acceptable for v3.0

- [ ] **#1323 / #1259** - Heap allocation optimization (Large scope)
  - **Rationale**: Struct-based Quote type and ArrayPool usage requires architectural discussion

- [ ] **Review Discussion #1018** - Community feedback on state rollback (2-3 hours)
  - **Rationale**: Evaluate feature requests for future planning

- [ ] **E001-E003** - ZigZag incremental implementation (TBD - complex research)
  - **Action**: Analyze pivot detection logic for incrementalization
  - **Related**: T170, #1692

- [ ] **E004-E006** - QuoteHub update semantics (TBD)
  - **Action**: Design and implement intra-period quote modification support
  - **Impact**: Enables more advanced streaming scenarios
  - **Related**: #1585

- [ ] **E007-E008** - ADX DMI property expansion (TBD)
  - **Action**: Add DMI properties to ADX result classes
  - **Action**: Update documentation and tests

- [ ] **E009** - BufferList configuration implementation (TBD)
  - **Action**: Complete BufferList configuration feature
  - **Related**: [Issue #1831](https://github.com/DaveSkender/Stock.Indicators/issues/1831)

- [ ] **E010** - Composite naming for chained indicators (TBD)
  - **Action**: Implement naming conventions for chained indicator workflows

---
Last updated: January 3, 2026
