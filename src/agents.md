# Critical rules for AI agents: indicator formula implementation

This document defines **NON-NEGOTIABLE** rules for AI coding agents working with indicator formulas in the Stock Indicators library. These rules prevent unintended formula modifications that could break mathematical correctness and violate the project's constitution.

## 🚨 CRITICAL: Formula change authority

<!-- ai:rule start -->
### Rule 1: NEVER modify existing indicator formulas without explicit authorization

AI agents **MUST NOT**:

- Change mathematical formulas in existing indicators
- Alter coefficients, constants, or smoothing factors
- Modify calculation sequences or dependencies
- "Optimize" or "simplify" formulas without verification
- Apply formulas from unverified sources (TradingView, random websites, uncited calculators)

**Rationale**: Every formula in this library has been validated against authoritative sources and reference calculations. Arbitrary changes break mathematical correctness and violate [Constitution §1: Mathematical Precision](../.specify/memory/constitution.md).

**Exceptions**: Formula changes require:

1. Explicit user authorization with source citations
2. Updated reference calculations in `tests/indicators/` matching the new formula
3. Constitutional amendment if changing sourcing hierarchy
4. MAJOR version bump per semantic versioning

<!-- ai:rule end -->

## 📚 Formula sourcing hierarchy (from constitution)

When implementing or validating formulas, **ALWAYS** follow this hierarchy:

<!-- ai:rule start -->
### Primary source of truth

**Manual calculation spreadsheets** in `tests/indicators/` directory are the validated ground truth.
All implementations **MUST** match these reference calculations exactly.

### Authoritative sources (for initial implementation)

1. Original publications by indicator creators (books, white papers, academic papers)
2. Established technical analysis textbooks by recognized experts
3. Published specifications from reputable financial institutions or exchanges
4. Wikipedia entries with **proper citations** to primary sources

### Acceptable validation sources

1. Third-party implementations that **cite authoritative sources**
2. Community-validated calculators with **verifiable methodology**
3. Published analysis from recognized quantitative trading firms

### Prohibited sources

**NEVER** use these as formula sources:

- ❌ TradingView and similar charting platforms ([Discussion #801](https://github.com/DaveSkender/Stock.Indicators/discussions/801))
- ❌ Uncited online calculators or implementations
- ❌ Sources without references to original formulas or established publications
- ❌ AI-generated formulas without authoritative verification
- ❌ "Common knowledge" or "industry standard" without citations

**Reference**: [Constitution §1: Mathematical Precision - Formula Sourcing Hierarchy](../.specify/memory/constitution.md)
<!-- ai:rule end -->

## 🔒 Implementation style parity requirements

<!-- ai:rule start -->
### Rule 2: Maintain bit-for-bit parity across implementation styles

All implementation styles (Series, BufferList, StreamHub) **MUST** produce **identical final values** for deterministic calculations:

- **Series** serves as the validated baseline (source of mathematical truth)
- **BufferList** and **StreamHub** must match Series results **exactly**
- No approximations, no tolerance, no "close enough"

**Test requirement**: Every streaming implementation must pass regression tests with deterministic equality against Series baseline results.

**Reference**: [Constitution §1: Mathematical Precision](../.specify/memory/constitution.md)
<!-- ai:rule end -->

## 🛠️ Safe modifications vs. prohibited changes

<!-- ai:rule start -->
### ✅ Allowed modifications (safe)

AI agents **MAY** modify:

- Code structure and organization (refactoring)
- Performance optimizations that preserve mathematical correctness
- Error handling and input validation
- Documentation and comments
- Test coverage and test data
- Variable naming and code style

**Verification required**: All changes must pass existing tests without modifying expected values.

### ❌ Prohibited changes (require authorization)

AI agents **MUST NOT** modify:

- Mathematical formulas or calculation sequences
- Coefficients, smoothing factors, or magic numbers
- Warmup period calculations
- Default parameter values affecting calculations
- Reference test data or expected results

**Verification required**: Any change to formula logic requires:

1. User authorization with authoritative source citation
2. Updated manual calculations in spreadsheets
3. Constitution compliance check
4. Regression test baseline updates

<!-- ai:rule end -->

## 📋 Pre-implementation checklist

<!-- ai:rule start -->
Before implementing **any** changes to indicator code, AI agents must verify:

- [ ] Change does not modify mathematical formulas (unless explicitly authorized with sources)
- [ ] Change preserves bit-for-bit parity with Series baseline (for streaming implementations)
- [ ] Change does not alter default parameter values
- [ ] All existing tests pass without modification to expected values
- [ ] Manual calculation spreadsheets remain valid (if formula unchanged)
- [ ] Changes comply with [Constitution §1: Mathematical Precision](../.specify/memory/constitution.md)
- [ ] Changes follow style-specific guidelines:
  - Series: [indicator-series.instructions.md](../.github/instructions/indicator-series.instructions.md)
  - Stream: [indicator-stream.instructions.md](../.github/instructions/indicator-stream.instructions.md)
  - Buffer: [indicator-buffer.instructions.md](../.github/instructions/indicator-buffer.instructions.md)

<!-- ai:rule end -->

## 🎯 Examples: correct vs. incorrect behavior

### ❌ Example: Incorrect - Unauthorized formula change

```csharp
// WRONG: Changing smoothing factor without authorization
// Original (validated):
mama = (alpha * pr[i]) + ((1d - alpha) * prevMama);

// AI agent changed to (FORBIDDEN):
mama = (0.8 * pr[i]) + (0.2 * prevMama);  // ❌ Arbitrary change!
```

**Problem**: This breaks mathematical correctness and violates constitution principles.

### ✅ Example: Correct - Performance optimization preserving correctness

```csharp
// CORRECT: Refactoring for performance while preserving formula
// Original:
double sum = 0;
for (int i = 0; i < period; i++)
{
    sum += values[i];
}
double average = sum / period;

// Optimized (ALLOWED):
double average = values[..period].Sum() / period;
// Or even better for performance:
double sum = 0;
for (int i = 0; i < period; i++)
{
    sum += values[i];
}
double average = sum / period;  // Keep original if tests verify correctness
```

**Allowed**: Structure changes that preserve exact mathematical results.

## 🔗 Related documentation

For comprehensive guidance, AI agents should consult:

- [Constitution: Mathematical Precision](../.specify/memory/constitution.md) - NON-NEGOTIABLE principles
- [Source Code Completion Checklist](../.github/instructions/source-code-completion.instructions.md) - Pre-commit requirements
- [Indicator Series Guidelines](../.github/instructions/indicator-series.instructions.md) - Series implementation patterns
- [Indicator Stream Guidelines](../.github/instructions/indicator-stream.instructions.md) - StreamHub patterns
- [Indicator Buffer Guidelines](../.github/instructions/indicator-buffer.instructions.md) - BufferList patterns
- [Spec Kit Instructions](../.github/instructions/spec-kit.instructions.md) - Specification-driven development

## 📞 When in doubt

If unsure whether a change modifies formula logic:

1. **STOP** - Do not proceed with the change
2. **ASK** - Request explicit user authorization with source citations
3. **VERIFY** - Ensure authoritative sources support the change
4. **TEST** - Validate against manual calculations before implementation

**Remember**: It is **always safer** to ask than to introduce mathematical errors that could affect
thousands of users relying on this library for financial analysis.

---
Last updated: October 15, 2025
