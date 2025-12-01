# CodeRabbit Configuration Guide

This document explains the CodeRabbit AI code review configuration optimized for the Stock Indicators for .NET repository.

## Configuration Overview

The `.coderabbit.yml` file provides tailored AI code review instructions for this financial analysis library, focusing on:

- **Mathematical accuracy** in financial calculations
- **Performance optimization** for large datasets
- **.NET best practices** and modern C# patterns
- **Comprehensive test coverage** validation
- **API consistency** across 200+ indicators
- **Documentation completeness** for public APIs

## Key Configuration Sections

### Path-Based Instructions

Different file types receive specialized review focus:

#### Source Code (`src/**/*.cs`)

- Mathematical accuracy verification against reference implementations
- Performance analysis (allocations, LINQ usage, memory efficiency)
- Precision guidance (`double` vs `decimal` for financial calculations)
- Input validation and error handling patterns
- Streaming indicator state management

#### Test Files (`tests/**/*.cs`)

- Test coverage completeness (edge cases, boundary conditions)
- Mathematical validation against manually calculated results
- Performance test regression detection
- Integration test API compatibility
- Error scenario validation

#### Catalog Files (`**/*.Catalog.cs`)

- API parameter definition accuracy
- Result property name matching with Models
- IReusable flag correctness (only one per indicator)
- Method naming pattern consistency (`To{IndicatorName}`)

#### Documentation (`docs/**/*.md`)

- Content accuracy matching current implementation
- Example completeness and correctness
- Accessibility compliance (alt text, heading hierarchy)
- Mathematical formula verification

### Custom Checks

The configuration includes domain-specific pattern detection:

```yaml
# Financial accuracy patterns
- pattern: "decimal.*price"
  message: "Consider if decimal precision is necessary..."

# Performance patterns  
- pattern: "\\.ToList\\(\\)"
  message: "Avoid unnecessary .ToList() calls in performance-critical paths..."

# Validation patterns
- pattern: "ArgumentOutOfRangeException"
  message: "Ensure lookback period validation includes edge cases..."
CodeRabbit is informed about domain-specific concepts:

- Financial indicator calculation principles
- Performance priorities (`double` vs `decimal`)
- Data handling patterns (null values, insufficient data)

The configuration respects existing development tools:

- **EditorConfig**: Follows established formatting rules
- **.NET Analyzers**: Coordinates with existing code quality rules
Predefined templates ensure consistent review quality:
- **Performance Analysis**: Checks for allocations, span usage, LINQ optimization
See [Contributing](/contributing/) for project contribution guidelines.
- **API Consistency**: Ensures naming patterns and documentation standards

## Ignored Files

The configuration excludes files that don't require review:

- Generated code (`*.Designer.cs`, `*.g.cs`)
- Build artifacts (`bin/`, `obj/`)
- Third-party dependencies (`node_modules/`, `packages/`)
- Cache directories (`.vs/`, `.vitepress/cache/`)
- Manual calculation spreadsheets (`*.Calc.xlsx`)

## Metrics and Reporting

Key metrics tracked for this repository type:

- **Test Coverage**: Ensures comprehensive validation
- **Code Duplication**: Identifies refactoring opportunities
- **Cyclomatic Complexity**: Maintains readability
- **Performance Indicators**: Tracks optimization opportunities

## Usage Guidelines

### For Contributors

When submitting PRs, expect CodeRabbit to focus on:

1. **Mathematical correctness** of any financial calculations
2. **Performance impact** of code changes
3. **Test coverage** for new or modified functionality
4. **API consistency** with existing patterns
5. **Documentation completeness** for public APIs

### For Maintainers

The configuration supports different review depths:

- **Auto-approve** simple documentation and test-only changes
- **Detailed review** for core calculation logic changes
- **Performance analysis** for optimization opportunities
- **API design review** for new indicators

## Maintenance

The configuration should be updated when:

- New indicator patterns emerge requiring specific review focus
- Performance bottlenecks are identified that need automated detection
- API design patterns change requiring updated consistency checks
- Documentation standards evolve requiring updated validation

## Benefits

This tailored configuration provides:

- **Domain-aware reviews** understanding financial calculation requirements
- **Performance-focused feedback** for high-throughput scenarios
- **Consistency enforcement** across 200+ similar indicators
- **Quality assurance** for mathematical accuracy
- **Developer efficiency** through relevant, actionable feedback

---

For questions about the CodeRabbit configuration, see the [contributing guidelines](contributing.md) or open an issue.
