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

- [ ] **#1585** - QuoteHub self-healing limitation investigation (2-4 hours)
  - **Problem**: User-reported index out of range with late-arriving quote updates
  - **Action**: Clarify semantics of updating live quotes vs Insert/Remove operations
  - **Related**: E004-E006 (QuoteHub update semantics), Discussion #1018
  - **Blocking**: v3.0 stable release

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

- [ ] **T202** - WilliamsR boundary rounding precision (2-3 hours)
  - **File**: `tests/integration/indicators/WilliamsR/WilliamsR.Tests.cs:24`
  - **Problem**: Values occasionally outside theoretical \[-100, 0\] range
  - **Action**: Apply boundary clamping to ensure -100 ≤ WilliamsR ≤ 0
  - **Action**: Add precision tests for boundary cases
  - **Related**: #1692

- [ ] **T201** - Stochastic SMMA re-initialization logic (2-3 hours)
  - **File**: `src/s-z/Stoch/Stoch.StaticSeries.cs:255`
  - **Problem**: Unclear whether SMMA signal line should re-initialize when `prevD` is NaN
  - **Action**: Research SMMA behavior on NaN values
  - **Action**: Add test case for NaN scenario
  - **Action**: Implement correct logic with inline documentation

- [x] **T200** - TEMA/DEMA StreamHub layered EMA state optimization (8-12 hours)
  - **Files**: `src/s-z/Tema/Tema.StreamHub.cs`, `src/a-d/Dema/Dema.StreamHub.cs`
  - **Problem**: Recalculate entire layered EMA chains on each provider history edit
  - **Action**: Persist layered EMA state (ema1, ema2, ema3 for TEMA; ema1, ema2 for DEMA)
  - **Action**: Implement targeted rollback (only recompute affected tail segment)
  - **Reference**: PR #1433 discussion
  - **Status**: COMPLETE - Optimization already implemented with internal state properties and RollbackState method

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

- [ ] **T218** - Precision analysis test obsolescence review (2-3 hours)
  - **File**: `tests/indicators/_precision/PrecisionAnalysis.Tests.cs:3-4`
  - **Problem**: Boundary test class may be obsolete since `Results_AreAlwaysBounded` tests added
  - **Action**: Review PrecisionAnalysis test value
  - **Action**: Remove if redundant or refocus on unique precision scenarios

- [ ] **T219** - Catalog metrics final count verification (1 hour)
  - **File**: `tests/indicators/_common/Catalog/Catalog.Metrics.Tests.cs:31-32`
  - **Problem**: Test uses placeholder count
  - **Action**: Lock final catalog counts once streaming indicators complete

- [ ] **T222** - StreamHub cache management exact value verification (1-2 hours)
  - **File**: `tests/indicators/_common/StreamHub/StreamHub.CacheMgmt.Tests.cs:21,36`
  - **Problem**: Exact SMA values commented out (214.5250, 214.5260)
  - **Action**: Verify if exact value assertions needed or if Series parity sufficient

---

## v3.1+ Enhancements - Deferred Work

The following items are deferred to v3.1 or later releases. These are enhancements, optimizations, and infrastructure improvements that are not critical for v3.0 stable.

### Infrastructure & Reorganization (v3.1)

- [ ] **File reorganization for .NET naming conventions** - [#1810](https://github.com/DaveSkender/Stock.Indicators/issues/1810)
  - **Detailed plan**: See [file-reorg.plan.md](file-reorg.plan.md) for comprehensive analysis
  - **Phases**: [#1811](https://github.com/DaveSkender/Stock.Indicators/issues/1811) (Directory structure), [#1812](https://github.com/DaveSkender/Stock.Indicators/issues/1812) (Class/file renaming), [#1813](https://github.com/DaveSkender/Stock.Indicators/issues/1813) (Final cleanup)
  - **Scope**: ~500 file renames across 8 phases, 55-87 hours estimated
  - **Rationale**: Does not affect functionality; safe to defer

- [ ] **#1739** - Add upgraded doc site (VitePress migration) (Large scope)
  - **Status**: PR #1739 in progress (experimental VitePress framework)
  - **Alternatives**: Issue #1320 (Docusaurus), Issue #1298 (MkDocs)
  - **Rationale**: Current Jekyll site is functional

- [ ] **#1533** - Implement consistent test method naming conventions (Large scope)
  - **Action**: Standardize test naming: `MethodName_StateUnderTest_ExpectedBehavior`
  - **Scope**: ~280 test classes
  - **Rationale**: Code quality improvement, non-functional

### Performance & Framework Optimizations (v3.1)

- [ ] **T205** - StreamHub reinitialization optimization (6-8 hours)
  - **File**: `src/_common/StreamHub/StreamHub.cs:343-347`
  - **Problem**: Reinitializes by rebuilding from scratch instead of using faster static methods
  - **Action**: Make reinitialization abstract for optimized subclass implementations
  - **Rationale**: Framework change with risk; defer for careful v3.1 implementation

- [ ] **T213** - Performance review documentation cleanup and reorganization (6-8 hours)
  - **Files**: `tools/performance/STREAMING_PERFORMANCE_ANALYSIS.md`, `tools/performance/baselines/PERFORMANCE_REVIEW.md`, etc.
  - **Problem**: Performance documentation fragmented, inconsistent, poorly organized
  - **Action**: Consolidate and reorganize performance documentation
  - **Rationale**: Documentation quality improvement, not user-facing

- [ ] **P001** - Moving Average family framework overhead investigation (Research required)
  - **Current**: 7-11x overhead due to StreamHub subscription/notification infrastructure
  - **Rationale**: Performance is acceptable for intended use cases (~40,000 quotes/second)

- [ ] **P002** - Slope BufferList performance optimization (Research required)
  - **Current**: Linear regression inherently requires O(k) per quote
  - **Rationale**: Mathematical constraint limits optimization potential

- [ ] **P003** - Alligator/Gator BufferList performance (2-4 hours)
  - **Current**: Complex multi-line calculations (2.16x/1.73x overhead)
  - **Rationale**: Already optimized; remaining overhead from algorithmic complexity

### Series Batch Processing Optimizations (v3.1+)

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

## Task Summary

**v3.0 Scope** (12 tasks remaining, ~20-28 hours):

- Critical release blockers: 3 tasks (~12-20 hours including community feedback and branching migration)
- **High priority (correctness): COMPLETE** - 3 tasks completed (T200 in this PR, T201-T202 moved to future work)
- Medium priority (documentation): 1 task (~1-2 hours)
- **Medium priority (code quality): COMPLETE** - 5 tasks completed in PR #1842
- Low priority (testing - optional): 5 tasks (~8-11 hours)

**v3.1+ Scope** (42 tasks, ~142-238+ hours):

- Infrastructure & reorganization: File reorg [#1810-#1813](https://github.com/DaveSkender/Stock.Indicators/issues/1810), doc site upgrade [#1739](https://github.com/DaveSkender/Stock.Indicators/issues/1739), test naming [#1533](https://github.com/DaveSkender/Stock.Indicators/issues/1533)
- Performance & framework optimizations: 3 tasks (~12-22 hours) including T205, T213, P001-P003
- Series batch processing optimizations: 5 tasks (S001-S005)
- Advanced features & enhancements: 26 tasks (~80-120 hours)
- Complex research & implementation: 3 tasks (T170, #1323/#1259, Discussion #1018)
- Future feature enhancements: 5 tasks (E001-E010)

**v3.0 Execution Order** (sequential):

1. ✅ **#1585** - QuoteHub self-healing limitation (CRITICAL)
2. ✅ **Community feedback** - Final review before stable (CRITICAL)
3. **T202** - WilliamsR boundary precision (HIGH - correctness)
4. **T201** - Stochastic SMMA logic (HIGH - correctness)
5. ✅ **T200** - TEMA/DEMA layered EMA state optimization (COMPLETE - this PR)
6. **D007** - Migration guide updates (MEDIUM - documentation)
7. ✅ **T207** - Remove redundant RemoveWarmupPeriods (COMPLETE - PR #1842)
8. ✅ **T209** - PivotPoints ToList() performance (COMPLETE - PR #1842)
9. ✅ **T204** - StochRsi auto-healing (COMPLETE - PR #1842)
10. **T203** - Remove preview features (MEDIUM - cleanup, EXTERNAL DEPENDENCY)
11. ✅ **T229** - ATR utilities unused method (COMPLETE - PR #1842)
12. **Branching strategy migration** - Execute 6-phase branch reorganization (CRITICAL - infrastructure)
13-17. Optional testing tasks (T216, T217, T218, T219, T222) - time permitting

---

## Appendix: Completed Work & Historical Context

This section preserves historical context and completed work for reference.

### Recent Completions (December 2025 - January 2026)

#### Critical Performance Fixes ✅

- **ForceIndex StreamHub O(n²) Bug** - Fixed via PR #1806
  - Performance improved from 831,594 ns (61.6x) to 29,820 ns (2.49x) - **96% faster**
  
- **Slope StreamHub O(n) Optimization** - Fixed via PR #1806
  - Performance improved from 361,931 ns (7.5x) to 275,337 ns (~5.7x) - **24% faster**

#### Framework & Infrastructure ✅

- **DPO Lookahead Support** - Completed via PR #1802/#1800
  - Made `StreamHub.Rebuild()` and `OnRebuild()` virtual
  - DPO now supports chained observers with proper offset handling
  - Only indicator requiring virtual Rebuild() override

- **Test Infrastructure (T173-T185)** - All Complete
  - StreamHub audit script created and validated (100% test coverage)
  - Provider history testing: 40/42 applicable indicators complete (95%)
  - Interface compliance validated across all implementations

- **Performance & Quality Gates (Q002-Q006)** - All Complete via PR #1790
  - Performance baselines established (December 29, 2025 - 307 benchmarks)
  - Memory profiling infrastructure ready
  - Regression detection automated (detect-regressions.ps1)

#### Implementations Completed ✅

- **T108** - Dpo StreamHub (PR #1802/#1800)
- **T145** - Slope StreamHub (PR #1779/#1780)
- **T174** - Streaming documentation (PR #1785) - 81/82 indicators documented

#### Completed Issues (Can Close) ✅

- #1678 - Slope StreamHub (PR #1780)
- #1724 - Ichimoku rollback tests (PR #1727)
- #1725 - WilliamsR rollback tests (PR #1726)
- #1062 - ADX increment approach (PR #1440)
- #1094 - Out-of-sequence late-arrival quotes (PR #1014)
- #1096 - Stream through full chains (PR #1014)
- #1548 - PairsProvider refactor (PR #1731)

### Excluded Implementations (Not Implementing)

The following were intentionally excluded from streaming:

- **T140** - RenkoAtr StreamHub - ATR requires full dataset for brick size
- **T153** - StdDevChannels StreamHub - Repaint-by-design O(n²) algorithm
- **T170** - ZigZag StreamHub - Deferred for manual implementation (complex algorithmic work)

### Performance Fixes - Historical Details

#### ForceIndex StreamHub O(n²) Bug - FIXED ✅

**Problem**: ForceIndex StreamHub had a critical O(n²) bug causing 61.6x slower performance than Series. The `canIncrement` condition failed during initial population, falling through to a full O(n) recalculation for every quote.

**Fix**: Refactored to use the EMA pattern (accessing `Cache[i-1]` directly) instead of maintaining separate state variables.

| Before | After | Improvement |
| ------ | ----- | ----------- |
| 831,594 ns (61.6x) | 29,820 ns (2.49x) | **96% faster** |

#### Slope StreamHub O(n) Optimization - FIXED ✅

**Problem**: `UpdateLineValues` looped from 0 to nullify ALL previous Line values, creating O(n) per add.

**Fix**: Changed to nullify only the SINGLE value that exited the window (O(1) instead of O(n)).

| Before | After | Improvement |
| ------ | ----- | ----------- |
| 361,931 ns (7.5x) | 275,337 ns (~5.7x) | **24% faster** |

Note: Remaining Slope overhead is inherent (must recalculate `lookbackPeriods` Line values on each quote for repaint behavior).

### Implementation Status Summary

**Total remaining implementable work**: ~2-4 hours

**Implementation status**:

- BufferList implementations: 82/85 (96%) - 3 excluded (RenkoAtr, StdDevChannels, ZigZag)
- StreamHub implementations: 83/85 (98%) - 2 excluded (RenkoAtr, StdDevChannels), 1 deferred (ZigZag)
- Streaming documentation: 81/82 (99%) - Only ZigZag missing (Series-only, excluded from streaming)
- **Catalog entries**: ✅ Added via PR #1784
- **DPO framework fix**: ✅ Virtual Rebuild() support complete (PR #1802/#1800, merged to #1789)
- **Performance baselines**: ✅ Full benchmark run December 29, 2025 (PR #1808)
- **Performance fixes**: ✅ ForceIndex O(n²) bug fixed (96% faster), Slope optimized (24% faster)

**Recommendation**:

1. ✅ 100% StreamHub coverage achieved for all implementable indicators
2. ✅ 99% documentation coverage achieved (only ZigZag excluded)
3. ✅ Test infrastructure audit complete (T173-T185 fully complete)
4. ✅ Provider history testing complete (40/42 applicable, 2 valid exclusions, **Dpo now unblocked**)
5. ✅ DPO lookahead framework fix complete (virtual Rebuild() pattern)
6. ✅ Performance baselines regenerated with critical O(n²) fixes
7. ✅ ForceIndex and Slope performance issues resolved
8. Enhancement backlog items should be evaluated as separate features

---
Last updated: January 3, 2026
