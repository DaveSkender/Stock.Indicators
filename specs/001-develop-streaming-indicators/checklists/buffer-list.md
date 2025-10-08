# BufferList Implementation Requirements Checklist

**Purpose**: Validate requirements quality and completeness for BufferList indicator implementations
**Created**: October 7, 2025
**Feature**: [spec.md](../spec.md) | [tasks.md](../tasks.md)
**Instructions**: [buffer-indicators.instructions.md](../../../.github/instructions/buffer-indicators.instructions.md)

**Note**: This checklist tests the REQUIREMENTS themselves for quality, clarity, and completeness - NOT the implementation. Each item validates whether requirements are well-written, complete, and unambiguous.

## Requirement Completeness

- [ ] CHK001 - Are base class inheritance requirements explicitly specified (must inherit from `BufferList<TResult>`)? [Completeness, Instructions §Core Structure]
- [ ] CHK002 - Are interface selection criteria clearly defined (`IBufferReusable` vs `IBufferList`)? [Clarity, Instructions §Interface Selection Guidelines]
- [ ] CHK003 - Are both constructor patterns mandatory requirements documented (parameter-only and quotes parameter)? [Completeness, Instructions §Constructor Pattern]
- [ ] CHK004 - Is the requirement for `AddInternal()` usage instead of `base.Add()` explicitly stated? [Completeness, Instructions §Base Class Pattern]
- [ ] CHK005 - Are buffer management requirements defined (must use `BufferUtilities` extension methods)? [Completeness, Instructions §Universal Buffer Utilities]
- [ ] CHK006 - Are warmup period handling requirements specified (return null until sufficient data)? [Completeness, Spec §FR-005]
- [ ] CHK007 - Are timestamp validation requirements defined (strictly ascending, reject duplicates/out-of-order)? [Completeness, Spec §FR-006]
- [ ] CHK008 - Are auto-pruning behavior requirements documented (automatic overflow trimming)? [Completeness, Instructions §Base Class Pattern]
- [ ] CHK009 - Are `Clear()` method requirements specified (override and reset both list and buffers)? [Completeness, Instructions §Core Structure]
- [ ] CHK010 - Are file naming convention requirements explicitly stated (`{IndicatorName}.BufferList.cs`)? [Completeness, Instructions §File Naming Conventions]

## Requirement Clarity

- [ ] CHK011 - Is "universal buffer utilities usage" quantified with specific method names (`Update`, `UpdateWithDequeue`)? [Clarity, Instructions §Universal Buffer Utility Usage]
- [ ] CHK012 - Are "O(1) append" performance characteristics measurable/testable? [Measurability, Spec §NFR-001]
- [ ] CHK013 - Is the "90% of int.MaxValue" default `MaxListSize` value explicitly documented? [Clarity, Instructions §Auto-pruning]
- [ ] CHK014 - Are constructor chaining requirements clear (use `: this(...)` syntax)? [Clarity, Instructions §Constructor Pattern]
- [ ] CHK015 - Is expression-bodied syntax requirement for quotes constructor explicit (`=> Add(quotes);`)? [Clarity, Instructions §Constructor Pattern]
- [ ] CHK016 - Are buffer capacity requirements quantified (based on lookback period parameter)? [Clarity, Instructions §Core Structure]
- [ ] CHK017 - Is "mathematical correctness" defined as deterministic equality (NOT approximate equality)? [Clarity, Spec §FR-004]
- [ ] CHK018 - Are "efficient buffer operations" defined with specific patterns (FIFO Queue)? [Clarity, Instructions §Universal Buffer Utilities]
- [ ] CHK019 - Is the requirement to "never mutate _internalList directly" explicitly documented? [Clarity, Instructions §Base Class Pattern]
- [ ] CHK020 - Are parameter validation requirements specific (call static `Indicator.Validate()` method)? [Clarity, Instructions §Core Structure]

## Requirement Consistency

- [ ] CHK021 - Are buffer management patterns consistent across all buffer types in requirements? [Consistency, Instructions §Inconsistent Buffer Patterns]
- [ ] CHK022 - Do constructor requirements align with existing indicator patterns (SMA, EMA examples)? [Consistency, Instructions §Reference Examples]
- [ ] CHK023 - Are naming conventions consistent with established codebase patterns (`{Name}List`)? [Consistency, Instructions §File Naming Conventions]
- [ ] CHK024 - Do interface selection requirements match static series implementation capabilities? [Consistency, Instructions §Interface Selection Guidelines]
- [ ] CHK025 - Are test requirements consistent with `BufferListTestBase` test patterns? [Consistency, Instructions §Test Structure Pattern]
- [ ] CHK026 - Do requirements align with zero-dependency principle stated in constitution? [Consistency, Plan §Technical Context]
- [ ] CHK027 - Are exception type requirements consistent (`ArgumentException` for validation failures)? [Consistency, Spec §FR-006]
- [ ] CHK028 - Do buffer utility requirements match universal `BufferUtilities` extension method signatures? [Consistency, Instructions §Buffer Utility Patterns]

## Acceptance Criteria Quality

- [ ] CHK029 - Are streaming parity test criteria measurable (exact equality, all non-null results)? [Measurability, Instructions §Test Coverage Expectations]
- [ ] CHK030 - Can "comprehensive unit tests" coverage be objectively verified (all test methods enumerated)? [Measurability, Instructions §Test Structure Pattern]
- [ ] CHK031 - Are performance benchmark criteria specific (<5ms mean, <10ms p95 latency)? [Measurability, Spec §NFR-001]
- [ ] CHK032 - Can "memory efficiency" be objectively measured (<10KB per instance)? [Measurability, Spec §NFR-002]
- [ ] CHK033 - Are auto-pruning test criteria measurable (overflow scenarios with `MaxListSize`)? [Measurability, Instructions §Test Coverage Expectations]
- [ ] CHK034 - Can "state reset correctness" be objectively verified (equals pre-population state)? [Measurability, Instructions §Test Structure Pattern]
- [ ] CHK035 - Are test base class requirements specified (inherit from `BufferListTestBase`)? [Completeness, TestBase.cs]
- [ ] CHK035a - Is it clear that ALL BufferList test classes MUST inherit from `BufferListTestBase` (not `TestBase` directly)? [Clarity, CRITICAL]
- [ ] CHK036 - Are test interface requirements specified (implement `ITestReusableBufferList` when appropriate)? [Completeness, TestBase.cs]
- [ ] CHK037 - Are test interface requirements specified (implement `ITestNonStandardBufferListCache` when using non-standard cache)? [Completeness, TestBase.cs]
- [ ] CHK038 - Are requirements for deterministic equality assertions specified (use `BeEquivalentTo` with `WithStrictOrdering`, NOT approximate equality)? [Clarity, TestBase.cs]

## Scenario Coverage

- [ ] CHK039 - Are requirements defined for incremental quote addition scenario? [Coverage, Instructions §Test Structure Pattern]
- [ ] CHK040 - Are requirements defined for batch quote addition scenario (collection initializer)? [Coverage, Instructions §Test Structure Pattern]
- [ ] CHK041 - Are requirements defined for quotes constructor scenario? [Coverage, Instructions §Test Pattern Notes]
- [ ] CHK042 - Are requirements defined for discrete value addition scenario (IReusable)? [Coverage, Instructions §Test Structure Pattern]
- [ ] CHK043 - Are requirements defined for state reset and repopulation scenario? [Coverage, Instructions §Test Structure Pattern]
- [ ] CHK044 - Are requirements defined for auto-pruning scenario (exceeding MaxListSize)? [Coverage, Instructions §Test Pattern Notes]
- [ ] CHK045 - Are requirements defined for non-standard cache pruning scenario (List-based state buffers with `AutoBufferPruning` test)? [Coverage, Instructions §Auto-pruning, TestBase.cs]
- [ ] CHK046 - Are requirements defined for insufficient warmup data scenario? [Coverage, Spec §Edge Cases]
- [ ] CHK047 - Are requirements defined for exact warmup threshold scenario? [Coverage, Instructions §Core Structure]

## Edge Case Coverage

- [ ] CHK048 - Are requirements defined for duplicate timestamp handling (ArgumentException)? [Edge Case, Spec §FR-006]
- [ ] CHK049 - Are requirements defined for out-of-order timestamp handling (ArgumentException)? [Edge Case, Spec §FR-006]
- [ ] CHK050 - Are requirements defined for null quote input handling? [Edge Case, Gap]
- [ ] CHK051 - Are requirements defined for empty quote collection handling? [Edge Case, Gap]
- [ ] CHK052 - Are requirements defined for buffer wraparound consistency? [Edge Case, Instructions §State Management]
- [ ] CHK053 - Are requirements defined for large lookback period scenarios (e.g., 200-period)? [Edge Case, Plan §Test Scenarios]
- [ ] CHK054 - Are requirements defined for single-quote buffer scenarios (lookback=1)? [Edge Case, Gap]
- [ ] CHK055 - Are requirements defined for zero or negative lookback period validation? [Edge Case, Instructions §Core Structure]
- [ ] CHK056 - Are requirements defined for NaN/Infinity value handling in calculations? [Edge Case, Gap]
- [ ] CHK057 - Are requirements defined for overflow scenarios in running sum calculations? [Edge Case, Gap]
- [ ] CHK058 - Are requirements defined for simultaneous list and non-standard cache pruning scenarios? [Edge Case, Instructions §Auto-pruning]

## Non-Functional Requirements

- [ ] CHK059 - Are memory management requirements specified (constant memory, bounded buffers)? [NFR, Instructions §Memory Management]
- [ ] CHK060 - Are thread safety requirements documented (single-writer scenarios)? [NFR, Instructions §Thread Safety Considerations]
- [ ] CHK061 - Are garbage collection pressure requirements defined (minimal allocations)? [NFR, Spec §NFR-002]
- [ ] CHK062 - Are performance profiling requirements specified (BenchmarkDotNet validation)? [NFR, Instructions §Performance Benchmarking]
- [ ] CHK063 - Are numerical stability requirements documented (avoid precision loss)? [NFR, Instructions §State Management]
- [ ] CHK064 - Are memory leak prevention requirements specified? [NFR, Instructions §Test Coverage Expectations]
- [ ] CHK065 - Are disposal pattern requirements documented (when applicable)? [NFR, Instructions §Memory Management]
- [ ] CHK066 - Are requirements for bounded non-standard cache growth specified (prevent unbounded List growth)? [NFR, Instructions §Auto-pruning]

## Dependencies & Assumptions

- [ ] CHK067 - Are Queue data structure requirements validated (FIFO operations)? [Dependency, Instructions §Universal Buffer Utilities]
- [ ] CHK068 - Are non-standard cache data structure requirements validated (List-based state buffers)? [Dependency, Instructions §Auto-pruning]
- [ ] CHK069 - Are `BufferUtilities` extension method dependencies documented? [Dependency, Instructions §Universal Buffer Utilities]
- [ ] CHK070 - Are base class `BufferList<TResult>` API requirements documented? [Dependency, Instructions §Base Class Pattern]
- [ ] CHK071 - Is the assumption of mathematical equivalence to series implementation validated? [Assumption, Spec §FR-004]
- [ ] CHK072 - Are test data assumptions documented (Standard test dataset - 502 quotes)? [Assumption, Plan §Test Scenarios]
- [ ] CHK073 - Are multi-target framework requirements specified (net8.0, net9.0)? [Dependency, Plan §Technical Context]
- [ ] CHK074 - Are static indicator method dependencies documented (validation, series calculation)? [Dependency, Instructions §Core Structure]

## Ambiguities & Conflicts

- [ ] CHK075 - Is the "buffer capacity vs lookback period" relationship unambiguous? [Ambiguity, Instructions §Core Structure]
- [ ] CHK076 - Is the distinction between "buffer size" and "list size" clearly defined? [Ambiguity, Instructions §Auto-pruning]
- [ ] CHK077 - Are "custom state pruning" requirements clear (List vs Queue buffers)? [Ambiguity, Instructions §Auto-pruning]
- [ ] CHK078 - Are criteria for when to use `ITestNonStandardBufferListCache` clearly defined (non-Queue cache = List-based state)? [Ambiguity, TestBase.cs]
- [ ] CHK079 - Is the "moderate frequency" threshold quantified (<1k ticks/sec)? [Ambiguity, Plan §Key Decisions]
- [ ] CHK080 - Are requirements for "indicators with multiple parameters" constructor patterns unambiguous? [Ambiguity, Instructions §Constructor Pattern]
- [ ] CHK081 - Is the distinction between `IBufferReusable` and `IBufferList` usage criteria clear? [Ambiguity, Instructions §Interface Selection Guidelines]
- [ ] CHK082 - Are requirements for "complex buffer scenarios" (multi-buffer) explicitly defined? [Ambiguity, Instructions §Buffer Utility Patterns]
- [ ] CHK083 - Are "running sum efficiency" optimization requirements clearly specified? [Ambiguity, Instructions §Buffer Utility Patterns]

## Traceability

- [ ] CHK084 - Is a requirement ID scheme established for buffer implementation requirements? [Traceability, Gap]
- [ ] CHK085 - Are all functional requirements traceable to test requirements? [Traceability, Gap]
- [ ] CHK086 - Are buffer utility method requirements traceable to `BufferUtilities.cs`? [Traceability, Instructions §Universal Buffer Utilities]
- [ ] CHK087 - Are test method requirements traceable to `BufferListTestBase` and test interfaces? [Traceability, Instructions §Test Structure Pattern]
- [ ] CHK088 - Are non-standard cache test requirements traceable to `ITestNonStandardBufferListCache.AutoBufferPruning()`? [Traceability, TestBase.cs]
- [ ] CHK089 - Are performance requirements traceable to benchmark implementations? [Traceability, Instructions §Performance Benchmarking]
- [ ] CHK090 - Are all specification requirements (FR-001 through FR-010) covered by implementation requirements? [Traceability, Spec §Requirements]

## Documentation Requirements Quality

- [ ] CHK091 - Are XML documentation requirements specified for public members? [Completeness, Instructions §Implementation Requirements]
- [ ] CHK092 - Are code example requirements defined (usage patterns in docs)? [Completeness, Gap]
- [ ] CHK093 - Are warmup guidance documentation requirements specified? [Completeness, Spec §NFR-005]
- [ ] CHK094 - Are performance characteristic documentation requirements defined? [Completeness, Spec §NFR-005]
- [ ] CHK095 - Are streaming usage example requirements specified for indicator docs? [Completeness, Plan §Documentation Updates]
- [ ] CHK096 - Are requirements for documenting non-standard cache pruning behavior specified? [Completeness, Instructions §Auto-pruning]

## Implementation Pattern Requirements

- [ ] CHK097 - Are extension method requirements specified (`To{Name}List` pattern)? [Completeness, Instructions §Extension Method]
- [ ] CHK098 - Are requirements for avoiding manual buffer management explicitly stated? [Completeness, Instructions §Common Anti-Patterns]
- [ ] CHK099 - Are requirements for consistent buffer utility usage across all buffers specified? [Completeness, Instructions §Common Anti-Patterns]
- [ ] CHK100 - Are requirements for efficient sum maintenance (dequeue tracking) specified? [Completeness, Instructions §Common Anti-Patterns]
- [ ] CHK101 - Are reference implementation requirements documented (SMA, EMA, HMA, ADX, MAMA examples)? [Completeness, Instructions §Reference Examples]
- [ ] CHK102 - Are requirements for proper non-standard cache pruning patterns specified (separate from Queue-based buffers)? [Completeness, Instructions §Auto-pruning]

## Quality Standards Requirements

- [ ] CHK103 - Are code quality standard requirements traceable to `.editorconfig` conventions? [Traceability, Instructions §Quality Standards]
- [ ] CHK104 - Are requirements for strict ordering assertions in tests specified (`WithStrictOrdering()`)? [Completeness, Instructions §Test Pattern Notes]
- [ ] CHK105 - Are requirements prohibiting approximate equality in deterministic tests specified (NO `BeApproximately`, use exact `BeEquivalentTo`)? [Completeness, TestBase.cs]
- [ ] CHK106 - Are requirements for public API approval test updates specified? [Completeness, Plan §Quality Gates]
- [ ] CHK107 - Are requirements for integration with series-style indicators specified? [Completeness, Instructions §Integration with Other Styles]
- [ ] CHK108 - Are requirements for consistency with library conventions documented? [Completeness, Spec §NFR-004]
- [ ] CHK109 - Are requirements for aligning with idiomatic reference implementations specified? [Completeness, Gap]
- [ ] CHK110 - Are requirements for proper test base class inheritance documented (`BufferListTestBase`)? [Completeness, TestBase.cs]
- [ ] CHK110a - Is it EXPLICITLY stated that test classes MUST inherit from `BufferListTestBase` and NOT from `TestBase` directly? [Clarity, CRITICAL]
- [ ] CHK110b - Are consequences of incorrect base class inheritance documented (missing abstract method implementations)? [Completeness, CRITICAL]
- [ ] CHK111 - Are requirements for implementing `ITestReusableBufferList` when supporting IReusable inputs specified? [Completeness, TestBase.cs]
- [ ] CHK112 - Are requirements for implementing `ITestNonStandardBufferListCache` when using List-based state caches specified? [Completeness, TestBase.cs]
- [ ] CHK113 - Are requirements for `AutoBufferPruning()` test when using non-standard caches specified? [Completeness, TestBase.cs]

## Notes

- This checklist validates REQUIREMENTS quality for BufferList implementations - NOT implementation correctness
- Each item checks if requirements are complete, clear, consistent, measurable, and cover all scenarios
- Check items as requirements are clarified, documented, or validated
- Use traceability references to link requirements back to specifications and instruction files
- Add findings or gaps discovered during requirements review inline

---
**Checklist Items**: 116
**Coverage**: Completeness, Clarity, Consistency, Acceptance Criteria, Scenarios, Edge Cases, NFRs, Dependencies, Ambiguities, Traceability, Documentation, Patterns, Quality Standards

**Key Requirements**:

1. **CRITICAL**: Test classes MUST inherit from `BufferListTestBase`, NOT `TestBase` directly (CHK110, CHK110a, CHK110b)
2. Tests must use deterministic equality (`BeEquivalentTo` with `WithStrictOrdering()`) - NO approximate equality (CHK105)
3. BufferList tests must implement appropriate interfaces:
   - `ITestReusableBufferList` when indicator supports `IReusable` inputs (CHK111)
   - `ITestNonStandardBufferListCache` when using List-based state caches (not just Queue) (CHK112)
4. Implementation must align with idiomatic reference patterns (SMA, EMA, Doji, MAMA examples)
5. Non-standard caches (List-based state) require proper pruning and `AutoBufferPruning()` test (CHK113)
