# Stock Indicators for .NET Constitution

## Core Principles

### I. Mathematical Precision

Financial calculations SHOULD prioritize precision and accuracy. While the current codebase primarily uses `double` types for performance reasons, `decimal` types are PREFERRED for price-sensitive calculations where precision loss could impact results. All indicators MUST be mathematically accurate against established reference implementations.

**Rationale**: Financial calculations require accuracy; the choice between `double` and `decimal` types should balance precision needs with performance requirements based on the specific use case.

### II. Performance First

Every indicator MUST be optimized for performance with minimal memory allocation. Excessive LINQ chaining is prohibited. All computationally intensive indicators REQUIRE performance benchmarks. Memory usage patterns MUST be validated for large datasets.

**Rationale**: Technical analysis often involves processing thousands of data points; performance directly impacts user experience and scalability.

### III. Comprehensive Validation

All public methods MUST include complete input validation with descriptive error messages. Edge cases (insufficient data, zero/negative values, null inputs) MUST be handled explicitly. Exception types MUST be consistent across the library.

**Rationale**: Financial data can be unpredictable; robust validation prevents runtime failures and provides clear feedback to developers.

### IV. Test-Driven Quality

Every indicator REQUIRES comprehensive unit tests covering all code paths. Mathematical accuracy MUST be verified against reference implementations. Performance tests are MANDATORY for computationally intensive indicators.

**Rationale**: Financial calculations require absolute correctness; comprehensive testing ensures reliability in production trading systems.

### V. Documentation Excellence

All public methods MUST have complete XML documentation. Code examples MUST be provided for complex indicators. Documentation MUST include parameter constraints, return value descriptions, and usage patterns.

**Rationale**: Technical analysis is complex; clear documentation enables developers to implement indicators correctly and efficiently.

## API Design Standards

### Consistency Requirements

- All indicators follow consistent naming conventions and parameter patterns
- Return types use immutable `record` structures
- Chainable indicators implement `IReusable` interface
- Utility methods follow established patterns for extension and transformation

### Backward Compatibility

- Breaking changes require major version increments
- Obsolete methods provide clear migration paths
- New features maintain compatibility with existing workflows

## Development Workflow

### Code Review Requirements

- Input validation completeness verification
- Mathematical accuracy validation against reference sources
- Performance characteristic review
- XML documentation completeness check
- Unit test coverage validation
- Consistent error message and exception type review

### Quality Gates

- All tests MUST pass before merge
- Performance benchmarks MUST not regress
- Code coverage MUST maintain established thresholds
- Mathematical accuracy MUST be verified for new indicators

## Governance

This constitution supersedes all other development practices. All code reviews MUST verify compliance with these principles. Any deviation requires explicit justification and approval.

Amendments to this constitution require:

1. Documentation of the change rationale
2. Impact assessment on existing codebase
3. Migration plan for affected components
4. Approval from project maintainers

**Version**: 1.0.0 | **Ratified**: 2025-09-26 | **Last Amended**: 2025-09-26
