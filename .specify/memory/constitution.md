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

### Spec-Driven Development

1. All new features begin with specification using `/specify` command
2. Technical planning follows with `/plan` command
3. Implementation tasks are generated using `/tasks` command  
4. Development proceeds with `/implement` command
5. All phases include peer review and validation against constitution

### Quality Assurance

- Unit tests must achieve >95% code coverage
- Performance benchmarks required for computational indicators
- Mathematical accuracy validated against reference implementations
- Documentation updated with every API change

## Governance

**Ratification Date**: 2025-01-27
**Last Amended**: 2025-01-27
**Constitution Version**: 2.0.0

This constitution governs all development activities for the Stock Indicators for .NET library and ensures consistent, high-quality implementations that serve the financial analysis community.
