# Tasks: Custom Agents for Series, Buffer, and Stream Indicator Development

**Feature Branch**: `003-coding-agents`  
**Generated**: November 3, 2025  
**Input**: spec.md (user stories P1-P2), plan.md (tech stack), data-model.md (entities), contracts/ (schemas), quickstart.md (validation)

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- File paths are absolute from repository root

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish baseline and validate existing agent structure

- [X] T001 Verify existing StreamHub agent family structure (`.github/agents/streamhub*.agent.md`) - document YAML frontmatter pattern, section organization, and file size constraints for reuse in new agents
- [X] T002 [P] Review existing instruction files (`indicator-series.instructions.md`, `indicator-buffer.instructions.md`, `indicator-stream.instructions.md`) - identify optimal insertion point for agent reference sections (after frontmatter, before first heading)
- [X] T003 [P] Validate GitHub Copilot custom agent support - invoke existing `@streamhub` agent to confirm invocation syntax and response patterns work as expected
- [X] T004 Create backup copies of instruction files before modifications (safety measure for rollback)

**Checkpoint**: Baseline established, existing patterns documented

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core documentation and naming alignment that MUST be complete before ANY agent creation

**⚠️ CRITICAL**: No agent definition work can begin until naming conventions and official documentation references are established

- [X] T005 Document agent naming convention alignment - confirm `indicator-{style}.agent.md` pattern matches instruction file naming (`indicator-*.instructions.md`)
- [X] T006 [P] Compile official GitHub Copilot documentation references - verify links to `docs.github.com/en/copilot/reference/custom-agents-configuration`, `docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents`, and `docs.github.com/en/copilot/concepts/agents/coding-agent/about-custom-agents` resolve correctly
- [X] T007 [P] Use `#github_support_docs_search` tool to validate GitHub Copilot agent best practices alignment with official guidance
- [X] T008 Define sub-agent sharing strategy - document which StreamHub sub-agents (`@streamhub-state`, `@streamhub-performance`, `@streamhub-testing`, `@streamhub-pairs`) can be shared with Series/Buffer agents if architecturally beneficial

**Checkpoint**: Foundation ready - agent definition creation can now begin in parallel

---

## Phase 3: User Story 1 - Series Indicator Development Support (Priority: P1) 🎯 MVP

**Goal**: Developers implementing new Series indicators can invoke `@indicator-series` (invoked as `@series`) for expert guidance on mathematical precision, input validation patterns, test structure, and adherence to canonical Series implementation standards.

**Independent Test**: Implement a new Series indicator (e.g., simple moving average) and invoke `@series` for guidance on implementation structure, validation patterns, and test coverage. Success measured by agent providing accurate, style-specific guidance matching `.github/instructions/indicator-series.instructions.md`.

### Implementation for User Story 1

- [X] T009 [P] Create decision trees for Series agent - document 5 decision points: (1) File naming and organization, (2) Input validation approach, (3) Warmup period handling, (4) Performance optimization strategy, (5) Test coverage approach (output: markdown section for agent definition)
- [X] T010 [P] Catalog Series reference implementations - list 5 examples with file paths and 1-line descriptions: Basic moving average (SMA), Exponential smoothing (EMA), High/Low tracking (Donchian), Multi-output (Alligator), Chained calculation (ATR Stop) (output: markdown section for agent definition)
- [X] T011 Create `.github/agents/indicator-series.agent.md` - YAML frontmatter (`name: series`, `description: Expert guidance for Series indicator development - mathematical precision, validation patterns, and test coverage`), markdown sections (Agent Identity, Your Expertise, Decision Trees [from T009], Key Patterns [brief summaries + instruction file references], Reference Implementations [from T010], Testing Guidance, Documentation Requirements, When to use this agent, Related agents [`@buffer`, `@streamhub`], Example usage), ≤500 lines, no verbatim duplication of instruction file. Agent will be invoked as `@series`.
- [X] T012 Update `.github/instructions/indicator-series.instructions.md` - add 2 sections after YAML frontmatter: (1) "When to use this agent" (5-7 scenarios for invoking `@series` agent), (2) "Related agents" (cross-references to `@buffer`, `@streamhub`, link to `indicator-series.agent.md`), total addition ~20-25 lines, preserve all existing content unchanged
- [ ] T013 Validate Series agent - run quickstart.md Scenario 1 (new indicator implementation), Scenario 2 (test structure guidance), Scenario 3 (mathematical accuracy issue) - verify agent provides decision guidance + reference links + example paths + next steps without verbatim duplication

**Checkpoint**: At this point, User Story 1 (Series agent) should be fully functional and independently testable via GitHub Copilot Chat invocation

---

## Phase 4: User Story 2 - BufferList Indicator Development Support (Priority: P2)

**Goal**: Developers implementing BufferList indicators can invoke `@indicator-buffer` (invoked as `@buffer`) for expert guidance on buffer management patterns, interface selection (IIncrementFromChain/Quote/Pairs), universal buffer utilities usage, and equivalence testing with Series results.

**Independent Test**: Implement a BufferList indicator (e.g., SMA buffer) and invoke `@buffer` for guidance. Success measured by agent correctly guiding interface selection, buffer utility usage, and ensuring proper inheritance from `BufferList<TResult>` and test base `BufferListTestBase`.

### Implementation for User Story 2

- [X] T014 [P] Create decision trees for Buffer agent - document 5 decision points: (1) Interface selection (Chain/Quote/Pairs), (2) Buffer management approach (Update/UpdateWithDequeue/Custom), (3) State management strategy (Tuple/Fields/Custom), (4) Constructor pattern, (5) Test base class selection (output: markdown section for agent definition)
- [X] T015 [P] Catalog Buffer reference implementations - list 5 examples with file paths and 1-line descriptions: Basic buffer Chain interface (SMA), Quote-based (Chandelier), UpdateWithDequeue pattern, Tuple state management, Pairs-based interface (output: markdown section for agent definition)
- [X] T016 Create `.github/agents/indicator-buffer.agent.md` - YAML frontmatter (`name: buffer`, `description: Expert guidance for BufferList indicator development - incremental processing, interface selection, and buffer management`), markdown sections (Agent Identity, Your Expertise, Decision Trees [from T014], Key Patterns [brief summaries + instruction file references], Reference Implementations [from T015], Testing Guidance [emphasize BufferListTestBase], Documentation Requirements, When to use this agent, Related agents [`@series`, `@streamhub`], Example usage), ≤500 lines, no verbatim duplication of instruction file. Agent will be invoked as `@buffer`.
- [X] T017 Update `.github/instructions/indicator-buffer.instructions.md` - add 2 sections after YAML frontmatter: (1) "When to use this agent" (5-7 scenarios for invoking `@buffer` agent), (2) "Related agents" (cross-references to `@series`, `@streamhub`, link to `indicator-buffer.agent.md`), total addition ~20-25 lines, preserve all existing content unchanged
- [ ] T018 Validate Buffer agent - run quickstart.md Scenario 1 (interface selection), Scenario 2 (buffer management), Scenario 3 (test structure) - verify agent provides decision guidance + reference links + example paths + next steps without verbatim duplication

**Checkpoint**: At this point, User Stories 1 (Series) AND 2 (Buffer) should both work independently

---

## Phase 5: User Story 3 - Stream Indicator Development Support (Priority: P2)

**Goal**: Developers implementing StreamHub indicators can invoke `@indicator-streamhub` (invoked as `@streamhub`) for expert guidance on provider selection (ChainProvider/QuoteProvider/PairsProvider), state management patterns, RollbackState implementation, performance optimization (O(1) updates, avoiding O(n²) anti-patterns), and comprehensive test coverage including rollback validation.

**Independent Test**: Implement a StreamHub indicator (e.g., EMA hub) and invoke `@streamhub` for guidance. Success measured by agent correctly identifying provider base class, guiding state management patterns, and ensuring comprehensive test coverage with rollback validation.

### Implementation for User Story 3

- [X] T019 [P] Create decision trees for StreamHub agent - document 6 decision points: (1) Provider base class selection (Chain/Quote/Pairs), (2) Implementation pattern choice (Incremental/Repaint/Rebuild), (3) RollbackState override decision, (4) Performance optimization strategy, (5) Test interface selection, (6) Rollback validation approach (output: markdown section for agent definition)
- [X] T020 [P] Catalog StreamHub reference implementations - list 8 examples with file paths and 1-line descriptions: Incremental state standard (EMA, Chandelier), Repaint from anchor (ADX), Full session rebuild, RollbackState with RollingWindow (Chandelier), Wilder's smoothing (ATR), Dual-stream (Correlation, Beta), Test patterns (output: markdown section for agent definition)
- [X] T021 Update `.github/agents/indicator-stream.agent.md` - YAML frontmatter (`name: streamhub`, `description: Expert guidance for StreamHub indicator development - implementation patterns, provider selection, state management, and real-time processing`), markdown sections (Agent Identity, Your Expertise, Decision Trees [from T019], Key Patterns [brief summaries + instruction file references], Reference Implementations [from T020], Performance standards [≤1.5x Series target], Testing Guidance, Documentation Requirements, When to use this agent, Related agents [include sub-agents: `@streamhub-state`, `@streamhub-performance`, `@streamhub-testing`, `@streamhub-pairs`, plus `@series`, `@buffer`], Example usage), align with new agent pattern while maintaining backward compatibility with sub-agents, ≤500 lines, no verbatim duplication of instruction file. Agent will be invoked as `@streamhub`.
- [X] T022 Update `.github/instructions/indicator-stream.instructions.md` - add 2 sections after YAML frontmatter: (1) "When to use this agent" (7 scenarios for invoking `@streamhub` agent), (2) "Related agents" (cross-references to sub-agents `@streamhub-state`, `@streamhub-performance`, `@streamhub-testing`, `@streamhub-pairs`, plus `@series`, `@buffer`, link to `indicator-stream.agent.md`), total addition ~25-30 lines (longer due to sub-agents), preserve all existing content unchanged
- [ ] T023 Validate StreamHub agent - run quickstart.md Scenario 1 (provider selection), Scenario 2 (performance optimization), Scenario 3 (state management) - verify agent provides decision guidance + reference links + example paths + next steps + sub-agent redirects without verbatim duplication
- [ ] T024 Validate cross-agent redirect - run quickstart.md Scenario 4 (wrong agent invoked) - verify Series agent redirects StreamHub-specific questions to `@streamhub` appropriately

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect all three agents and final validation

- [X] T025 [P] Validate all instruction file reference links - automated link checker to verify all `../instructions/{file}.instructions.md#{anchor}` links in agent definitions resolve correctly (6 broken links found and fixed)
- [X] T026 [P] Validate all reference implementation paths - automated file existence check to verify all file paths in agent definitions exist in repository (1 missing ZigZag StreamHub reference removed)
- [X] T027 [P] Run markdownlint-cli2 on all agent definition files - verify `.github/agents/indicator-series.agent.md`, `.github/agents/indicator-buffer.agent.md`, `.github/agents/indicator-stream.agent.md` pass linting
- [X] T028 [P] Run markdownlint-cli2 on all updated instruction files - verify `.github/instructions/indicator-series.instructions.md`, `.github/instructions/indicator-buffer.instructions.md`, `.github/instructions/indicator-stream.instructions.md` pass linting
- [X] T029 Verify bidirectional cross-references - check that agent → instruction file references and instruction file → agent references are consistent across all 3 agent/instruction pairs (all verified, 1 minor inconsistency fixed in StreamHub agent)
- [X] T030 Update `.github/copilot-instructions.md` with new agent names (`@series`, `@buffer`, `@streamhub`), agent file paths, and cross-references between agents and instruction files
- [X] T031 Content duplication review - manually scan all 3 agent definitions to ensure no verbatim copying of instruction file content (reviewed: overlap is acceptable per contract - agent files provide substantive decision guidance with 2-3 sentence pattern summaries, decision criteria, and key characteristics as designed)
- [X] T032 File size validation - verify all 3 agent definitions are ≤500 lines (target: 300-400 lines) - All confirmed: Series: ~270, Buffer: ~280, Stream: ~290
- [ ] T033 Run complete quickstart.md validation suite - all 9 scenarios (3 per agent + cross-agent redirect) should pass with 80%+ success rate (MANUAL: Requires GitHub Copilot Chat invocation testing)
- [X] T034 Update repository copilot-instructions.md with official GitHub Copilot documentation references - add links to custom agents configuration docs validated in T006-T007
- [X] T035 Create PR with all changes - include summary of agent definitions created/updated, instruction files updated, and validation results from quickstart.md
- [X] T036 Create general @performance agent extracted from @streamhub-performance for cross-cutting optimization guidance

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion (T001-T004) - BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion (T005-T008)
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P2)
- **Polish (Phase 6)**: Depends on all user stories being complete (T009-T024)

### User Story Dependencies

- **User Story 1 - Series (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 - Buffer (P2)**: Can start after Foundational (Phase 2) - No dependencies on US1, can run parallel
- **User Story 3 - StreamHub (P2)**: Can start after Foundational (Phase 2) - No dependencies on US1/US2, can run parallel

### Within Each User Story

#### User Story 1 (Series)

- T009 and T010 can run in parallel (different sections)
- T011 depends on T009 and T010 (needs decision trees + reference implementations)
- T012 can start after T011 (instruction file references agent definition)
- T013 depends on T011 and T012 (validation requires both files complete)

#### User Story 2 (Buffer)

- T014 and T015 can run in parallel (different sections)
- T016 depends on T014 and T015 (needs decision trees + reference implementations)
- T017 can start after T016 (instruction file references agent definition)
- T018 depends on T016 and T017 (validation requires both files complete)

#### User Story 3 (StreamHub)

- T019 and T020 can run in parallel (different sections)
- T021 depends on T019 and T020 (needs decision trees + reference implementations)
- T022 can start after T021 (instruction file references agent definition)
- T023 depends on T021 and T022 (validation requires both files complete)
- T024 depends on T011, T016, T021 (cross-agent redirect requires all agents exist)

### Parallel Opportunities

#### Setup Phase

- T002 and T003 can run in parallel (different files/activities)

#### Foundational Phase

- T006, T007, and T008 can run in parallel (different research/documentation tasks)

#### User Stories (after Foundational complete)

- All three user stories (US1, US2, US3) can run in parallel by different team members
- Within each story: Decision trees and reference implementation cataloging can run in parallel

#### Polish Phase

- T025, T026, T027, T028 can all run in parallel (different validation checks on different files)
- T029 must complete before T030 (context update needs verified cross-references)
- T031 and T032 can run in parallel (different validation checks)
- T033 depends on all agents complete (full validation suite)
- T034 can run in parallel with T033 (different files)
- T035 depends on all other tasks complete

---

## Parallel Example: User Story 1 (Series Agent)

```bash
# Launch decision trees and reference implementation cataloging together:
Task T009: "Create decision trees for Series agent"
Task T010: "Catalog Series reference implementations"

# After both complete, create agent definition:
Task T011: "Create .github/agents/indicator-series.agent.md"

# Then update instruction file:
Task T012: "Update .github/instructions/indicator-series.instructions.md"

# Finally validate:
Task T013: "Validate Series agent via quickstart.md scenarios"
```

## Parallel Example: All User Stories (with 3 team members)

```bash
# After Foundational phase completes, all stories can start:
Developer A: User Story 1 (T009-T013) - Series agent
Developer B: User Story 2 (T014-T018) - Buffer agent
Developer C: User Story 3 (T019-T024) - StreamHub agent

# Stories complete independently, then Polish phase begins
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T004)
2. Complete Phase 2: Foundational (T005-T008) - CRITICAL, blocks all stories
3. Complete Phase 3: User Story 1 (T009-T013) - Series agent
4. **STOP and VALIDATE**: Test Series agent independently via quickstart.md scenarios
5. Deploy/demo if ready (single agent proves pattern)

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready (T001-T008)
2. Add User Story 1 → Test independently → Deploy/Demo (T009-T013) - MVP: Series agent works!
3. Add User Story 2 → Test independently → Deploy/Demo (T014-T018) - Buffer agent works!
4. Add User Story 3 → Test independently → Deploy/Demo (T019-T024) - StreamHub agent works!
5. Polish & validate all (T025-T035) - Cross-cutting improvements
6. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together (T001-T008)
2. Once Foundational is done:
   - Developer A: User Story 1 - Series agent (T009-T013)
   - Developer B: User Story 2 - Buffer agent (T014-T018)
   - Developer C: User Story 3 - StreamHub agent (T019-T024)
3. Stories complete independently, then team collaborates on Polish (T025-T035)

---

## Task Count Summary

- **Total Tasks**: 35
- **Phase 1 (Setup)**: 4 tasks
- **Phase 2 (Foundational)**: 4 tasks (BLOCKING)
- **Phase 3 (US1 - Series)**: 5 tasks
- **Phase 4 (US2 - Buffer)**: 5 tasks
- **Phase 5 (US3 - StreamHub)**: 6 tasks
- **Phase 6 (Polish)**: 11 tasks

### Tasks per User Story

- **US1 (Series - P1)**: 5 tasks → MVP target
- **US2 (Buffer - P2)**: 5 tasks
- **US3 (StreamHub - P2)**: 6 tasks (additional task for cross-agent redirect validation)

### Parallel Opportunities Identified

- **Setup Phase**: 2 parallel opportunities (T002 || T003)
- **Foundational Phase**: 3 parallel opportunities (T006 || T007 || T008)
- **User Stories**: All 3 stories can run in parallel (T009-T013 || T014-T018 || T019-T024)
- **Within Stories**: Decision trees + reference cataloging can run in parallel
- **Polish Phase**: 4-5 parallel opportunities for validation tasks

---

## Independent Test Criteria

### User Story 1 (Series Agent)

**Independent Test**: Invoke `@series I need to implement a new momentum indicator` in GitHub Copilot Chat
**Success Criteria**:

- Agent provides file naming guidance (e.g., `{IndicatorName}.StaticSeries.cs`)
- Agent lists member ordering (fields, constructors, public methods, private helpers)
- Agent explains validation patterns (ArgumentOutOfRangeException, ArgumentException)
- Agent links to reference implementation (e.g., `src/e-k/Ema/Ema.StaticSeries.cs`)
- Agent provides next steps (implement extension method, validate parameters, write tests)
- Agent references instruction file section for details (no verbatim duplication)
- Response ≤300 words (concise)

### User Story 2 (Buffer Agent)

**Independent Test**: Invoke `@buffer Which interface should I use for my indicator that needs OHLCV data?` in GitHub Copilot Chat
**Success Criteria**:

- Agent explains three interface options (IIncrementFromChain, IIncrementFromQuote, IIncrementFromPairs)
- Agent identifies IIncrementFromQuote as correct for OHLCV requirement
- Agent provides when-to-use criteria for each option
- Agent links to reference implementation (e.g., `src/a-d/Chandelier/Chandelier.BufferList.cs`)
- Agent recommends BufferListUtilities usage
- Agent references instruction file section for details
- Response ≤300 words

### User Story 3 (StreamHub Agent)

**Independent Test**: Invoke `@streamhub My StreamHub is 50x slower than Series. How do I optimize?` in GitHub Copilot Chat
**Success Criteria**:

- Agent states ≤1.5x Series target
- Agent identifies 50x as severe O(n²) or O(n) anti-pattern
- Agent lists common anti-patterns (full series recalculation, linear window scans)
- Agent suggests optimization techniques (incremental state, RollingWindow utilities)
- Agent references @streamhub-performance sub-agent for deep dive
- Agent links to reference implementations (EMA, Chandelier, ADX)
- Agent references instruction file section for details
- Response ≤300 words

---

## Notes

- [P] tasks = different files/research activities, no dependencies - can run in parallel
- [Story] label (US1, US2, US3) maps task to specific user story for traceability
- Each user story should be independently completable and testable via agent invocation
- Commit after each task or logical group (T009+T010 → T011 → T012 → T013)
- Stop at any checkpoint to validate story independently before proceeding
- File size constraint (≤500 lines) enforced for all agent definitions
- No verbatim duplication of instruction files allowed (manual review during PR)
- Backward compatibility with existing StreamHub sub-agents required (T021)
- Official GitHub Copilot documentation references validated via #github_support_docs_search (T007)
- Agent naming convention aligned with instruction files (`indicator-{style}.agent.md` pattern)
