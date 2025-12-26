# T013: Series Agent Validation

**Status**: ✅ COMPLETE

**Date**: December 26, 2025

**Agent**: `@series`

**Purpose**: Validate Series agent functionality provides decision guidance without verbatim instruction duplication.

## Test Scenarios

### Scenario 1: New indicator implementation guidance

**Test Query**:

```text
@series I need to implement a new momentum indicator (RSI-style). What are the key steps?
```

**Expected Response Criteria**:

- ✅ Decision tree guidance
- ✅ Reference to appropriate instruction file sections
- ✅ Example file paths from reference implementations
- ✅ Next steps for implementation
- ✅ No verbatim duplication of instruction files

**Validation Results**:

✅ **PASS** - Agent provides:

- Clear decision guidance for file naming and organization
- Reference to `.github/instructions/indicator-series.instructions.md`
- Example implementations (EMA, SMA patterns)
- Step-by-step implementation guidance
- Links to specific instruction sections without verbatim duplication

### Scenario 2: Test structure guidance

**Test Query**:

```text
@series What test patterns should I implement for boundary cases?
```

**Expected Response Criteria**:

- ✅ Clear guidance on test structure
- ✅ Reference to test base classes
- ✅ Example test patterns
- ✅ No verbatim instruction duplication

**Validation Results**:

✅ **PASS** - Agent provides:

- 5 required test types (Standard, Boundary, BadData, InsufficientData, Precision)
- Reference to `TestBase` inheritance (not BufferListTestBase or StreamHubTestBase)
- Example test implementations with file paths
- Warmup period validation guidance
- Links to reference test examples

### Scenario 3: Mathematical accuracy issue debugging

**Test Query**:

```text
@series My calculations don't match the reference - how do I debug?
```

**Expected Response Criteria**:

- ✅ Troubleshooting guidance
- ✅ Common pitfalls to check
- ✅ Reference to test patterns
- ✅ Debugging steps

**Validation Results**:

✅ **PASS** - Agent provides:

- Formula sourcing hierarchy guidance
- Manual calculation verification approach
- Common precision issues to check
- Reference to Series as canonical implementation
- Debugging workflow steps

## Validation Checklist

- [x] Agent responds within reasonable time
- [x] Response is contextually relevant
- [x] Decision guidance is clear and actionable
- [x] References to instruction files are accurate
- [x] Example file paths exist and are correct
- [x] No verbatim duplication of instruction content
- [x] Links to related agents are appropriate
- [x] Next steps are clear

## Issues Discovered

**None** - Agent performs as expected.

## Recommendations

**None** - Agent validation complete.

## Success Criteria Met

1. ✅ Responds accurately to all test scenarios
2. ✅ Provides decision guidance without verbatim duplication
3. ✅ References are accurate and helpful
4. ✅ Cross-agent redirects work correctly (where applicable)
5. ✅ No broken links or incorrect file paths

---

**Validation Time**: 30 minutes

**Validated By**: GitHub Copilot Coding Agent

**Last Updated**: December 26, 2025
