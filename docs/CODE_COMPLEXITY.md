# Code Complexity Policy

## Overview

This document explains the approach to code complexity in the Stock Indicators for .NET library, particularly regarding Codacy Lizard complexity warnings.

## Financial Algorithm Complexity

Financial indicator implementations (files ending in `*.StaticSeries.cs`, `*.StreamHub.cs`, `*.BufferList.cs`) often exceed standard complexity thresholds. This is **expected and accepted** due to:

### 1. Mathematical Precision (Constitution Principle I - NON-NEGOTIABLE)
Financial calculations must be mathematically accurate and verified against reference implementations. Breaking up algorithms can introduce rounding errors or logic errors that compromise correctness.

### 2. Performance First (Constitution Principle II)
Indicators must minimize memory allocations and avoid excessive LINQ operations in hot paths. Extracting methods can introduce:
- Additional method call overhead
- Increased allocations
- Performance degradation

### 3. Algorithm Cohesion
Financial algorithms are best understood when the complete calculation logic is in one place. This allows:
- Easy verification against reference implementations
- Clear understanding of the complete algorithm
- Direct comparison with academic papers and trading literature

### 4. State Management Complexity
Streaming indicators (StreamHub) maintain internal state and must handle:
- Real-time quote updates
- State rollback for historical data changes
- Multiple conditional branches for different market states
- Edge cases for numerical stability

### 5. Multiple Market Conditions
Indicators must handle various market scenarios:
- Uptrends, downtrends, sideways movement
- Gap handling, volatility changes
- Initialization periods, warmup requirements
- Data edge cases (nulls, zeros, negatives)

## Examples of Necessary Complexity

### High Complexity Examples (Acceptable)

| Indicator | Method | Complexity | Lines | Reason |
|-----------|--------|-----------|-------|--------|
| RSI | RsiHub::ToIndicator | 43 | 133 | Wilder's smoothing + streaming state |
| ADX | AdxHub::ToIndicator | 40 | 124 | +DI/-DI calculation + smoothing |
| ADX | AdxHub::RollbackState | 40 | 119 | Complex state restoration for streaming |
| Stochastic | Stoch::CalcStoch | 23 | 132 | Multiple smoothing passes + %K/%D |
| SuperTrend | SuperTrend::CalcSuperTrend | 23 | 68 | Trend detection with ATR bands |

### Refactored Utility Examples (Completed)

| File | Method | Was | Now | Approach |
|------|--------|-----|-----|----------|
| StringOut.Type.cs | foreach | 13 | ✅ <8 | Extract formatting helpers |
| StringOut.Type.cs | GetPropertyDescriptionsFromXml | 10 | ✅ <8 | Use LINQ filtering |
| StringOut.List.cs | ColloquialTypeName | 9 | ✅ <8 | Simplify logic |

## Utility Code Standards

Utility and infrastructure code (non-financial calculations) should maintain:

- **Cyclomatic complexity**: ≤ 8
- **Method length**: ≤ 50 lines
- **Clear separation of concerns**
- **Single responsibility**

These files are safe and encouraged for refactoring:
- General utilities (`Numerical.cs`, etc.)
- String formatting (`StringOut.*`)
- Catalog/registration code (if refactoring improves maintainability)
- Test infrastructure

## When to Refactor

### ✅ DO Refactor When:
- Method is utility/infrastructure code (not financial calculation)
- Complexity comes from redundant conditionals
- Logic can be simplified without performance impact
- All tests pass after refactoring
- Build remains clean (zero warnings)

### ❌ DO NOT Refactor When:
- Method implements financial algorithm
- Mathematical correctness could be compromised
- Performance would be degraded
- Algorithm logic would be fragmented
- Verification against references would be harder

## Codacy Configuration

The `.codacy/codacy.yaml` file excludes financial algorithm files from Lizard complexity warnings:

```yaml
lizard@1.17.31:
    exclude_patterns:
        - "src/**/**.StaticSeries.cs"
        - "src/**/**.StreamHub.cs"
        - "src/**/**.BufferList.cs"
        - "src/_common/Catalog/Catalog.Listings.cs"
```

## Testing Requirements

All code, regardless of complexity, must maintain:

- **98%+ code coverage**
- **Mathematical accuracy verification** against reference data
- **Performance benchmarks** for computationally intensive indicators
- **Comprehensive edge case testing**

## References

- [Repository Constitution](https://github.com/DaveSkender/Stock.Indicators/discussions/648)
- [Contributing Guidelines](/docs/contributing.md)
- [Codacy Analysis Report](/CODACY_ANALYSIS.md)
- [Codacy Resolution Summary](/CODACY_RESOLUTION_SUMMARY.md)

---
Last updated: October 29, 2025
