# Custom Coding Agents - Remaining Work

This document consolidates incomplete tasks from the custom coding agents feature (originally tracked in .specify/specs/003-coding-agents/).

**Status**: ~85% complete - Core agents implemented and functional, validation tasks remaining.

## Overview

Three custom agents were created to assist with indicator development:

- `@series` - Series indicator development expert
- `@buffer` - BufferList indicator development expert
- `@streamhub` - StreamHub indicator development expert (with 4 sub-agents)

**Sub-agents** (for StreamHub specialization):

- `@streamhub-state` - RollbackState patterns and cache replay
- `@streamhub-performance` - O(1) optimization and anti-patterns
- `@streamhub-testing` - Test interface selection and rollback validation
- `@streamhub-pairs` - Dual-stream patterns and synchronization

## Remaining Tasks

### Phase 3: Series Agent Validation (1 task)

**T013** - Validate Series agent functionality

- **Scenarios to test**:
  1. New indicator implementation guidance
  2. Test structure guidance
  3. Mathematical accuracy issue debugging
- **Validation criteria**:
  - Agent provides decision guidance + reference links
  - Agent provides example paths from reference implementations
  - Agent provides next steps without verbatim duplication
  - No instruction file content duplication
- **Priority**: Medium
- **Effort**: 30-45 minutes

### Phase 4: Buffer Agent Validation (1 task)

**T018** - Validate Buffer agent functionality

- **Scenarios to test**:
  1. Interface selection (Chain/Quote/Pairs)
  2. Buffer management (Update/UpdateWithDequeue patterns)
  3. Test structure (BufferListTestBase inheritance)
- **Validation criteria**:
  - Agent correctly guides interface selection
  - Agent provides buffer utility usage examples
  - Agent ensures proper base class inheritance
  - No verbatim instruction file duplication
- **Priority**: Medium
- **Effort**: 30-45 minutes

### Phase 5: StreamHub Agent Validation (2 tasks)

**T023** - Validate StreamHub agent functionality

- **Scenarios to test**:
  1. Provider selection (Chain/Quote/Pairs)
  2. Performance optimization guidance
  3. State management patterns
- **Validation criteria**:
  - Agent provides decision guidance + reference links
  - Agent provides example paths + next steps
  - Agent correctly redirects to sub-agents when appropriate
  - No verbatim duplication
- **Priority**: Medium
- **Effort**: 30-45 minutes

**T024** - Validate cross-agent redirect functionality

- **Scenario to test**:
  - Invoke wrong agent for a question (e.g., ask `@series` about StreamHub-specific topics)
- **Validation criteria**:
  - Series agent recognizes StreamHub-specific question
  - Agent redirects to appropriate agent (`@streamhub`)
  - Redirection is helpful and contextual
- **Priority**: Low
- **Effort**: 15-30 minutes

### Phase 6: Final Polish (Already Complete)

**T025** - Validate all instruction file reference links ✅

- Automated link checker verified all references
- 6 broken links found and fixed
- **Status**: Complete

## Testing Approach

For each validation task, use the following methodology:

### 1. Agent Invocation Test

```text
@{agent} I need to implement a new {indicator-type} indicator. What are the key steps?
```

**Expected Response**:

- Decision tree guidance
- Reference to appropriate instruction file sections
- Example file paths from reference implementations
- Next steps for implementation
- NO verbatim duplication of instruction files

### 2. Specific Scenario Test

```text
@{agent} Which interface should I use for an indicator that needs OHLCV data?
```

**Expected Response**:

- Clear decision guidance
- Interface selection criteria
- Example implementations
- Link to relevant instruction sections

### 3. Error Scenario Test

```text
@{agent} My indicator is showing different results between Series and StreamHub
```

**Expected Response**:

- Troubleshooting guidance
- Common pitfalls to check
- Reference to test patterns
- Debugging steps

### 4. Cross-Agent Redirect Test

```text
@series How do I implement RollbackState for a StreamHub indicator?
```

**Expected Response**:

- Recognition that this is StreamHub-specific
- Redirection to `@streamhub` or `@streamhub-state`
- Brief context about why the redirect

## Validation Checklist

For each agent test, verify:

- [ ] Agent responds within reasonable time
- [ ] Response is contextually relevant
- [ ] Decision guidance is clear and actionable
- [ ] References to instruction files are accurate
- [ ] Example file paths exist and are correct
- [ ] No verbatim duplication of instruction content
- [ ] Links to related agents are appropriate
- [ ] Next steps are clear

## Success Criteria

An agent is considered validated when:

1. ✅ Responds accurately to all test scenarios
2. ✅ Provides decision guidance without verbatim duplication
3. ✅ References are accurate and helpful
4. ✅ Cross-agent redirects work correctly (where applicable)
5. ✅ No broken links or incorrect file paths

## Summary

**Total Remaining Work**: ~2-3 hours

**Breakdown**:

- T013 (Series validation): 30-45 minutes
- T018 (Buffer validation): 30-45 minutes
- T023 (StreamHub validation): 30-45 minutes
- T024 (Cross-agent redirect): 15-30 minutes

**Priority**: Medium - Agents are functional but formal validation incomplete

**Recommendation**:

- Complete validation tasks to ensure agent quality
- Document any issues discovered
- Update agent definitions if validation reveals problems
- Consider creating a validation test suite for future agent updates

---

**Source**: Migrated from .specify/specs/003-coding-agents/tasks.md  
**Last updated**: December 24, 2025
