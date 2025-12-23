# Implementation Plan: Custom Agents for Series, Buffer, and Stream Indicator Development

**Branch**: `003-coding-agents` | **Date**: November 3, 2025 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/.specify/specs/003-coding-agents/spec.md`

## Summary

Create three custom GitHub Copilot agents (`@series`, `@buffer`, `@streamhub`) to provide expert guidance on indicator implementation styles (Series/batch, BufferList/incremental, StreamHub/streaming). Each agent focuses exclusively on its style, provides decision trees for pattern selection, references authoritative code examples, and defers comprehensive details to scoped instruction files. Agents use minimal YAML frontmatter (name + description) and emphasize mathematical precision, test-driven development, and code completion checklists.

## Technical Context

**Language/Version**: Markdown (GitHub Flavored), YAML frontmatter (GitHub Copilot custom agent format)  
**Primary Dependencies**: GitHub Copilot Chat, VS Code API (#vscodeAPI reference), existing scoped instruction files  
**Storage**: Git repository files (`.github/agents/*.agent.md`, `.github/instructions/*.instructions.md`)  
**Testing**: Manual validation via agent invocation in GitHub Copilot Chat, developer feedback collection  
**Target Platform**: GitHub Copilot Chat in VS Code  
**Project Type**: Documentation artifacts (agent definitions + instruction file updates)  
**Performance Goals**: Agent responses within standard Copilot latency (<5s), accurate pattern guidance 100% of the time  
**Constraints**:

- Agents must not duplicate instruction file content verbatim (decision trees + key patterns only)
- Agent definitions ≤500 lines to maintain readability
- Must maintain backward compatibility with existing StreamHub sub-agents
**Scale/Scope**:
- 3 primary agent definition files (series, buffer, streamhub update)
- 3 instruction file minimal updates (add 2 sections each)
- Target audience: ~10-20 active indicator developers

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Mathematical Precision**: ✅ PASS - Agents enforce formula sourcing hierarchy, NaN handling policy, and Series-as-canonical-reference principle. Agents guide developers to maintain mathematical correctness per constitution requirements.

- **Performance First**: ✅ PASS - Agents emphasize performance patterns (O(1) StreamHub updates, avoiding O(n²) anti-patterns, span-based operations). StreamHub agent explicitly includes performance guidance with ≤1.5x Series target.

- **Comprehensive Validation**: ✅ PASS - All agents guide developers on input validation patterns, edge case handling, and parameter constraint enforcement. Test guidance includes validation of edge cases and invariants.

- **Test-Driven Quality**: ✅ PASS - All agents provide comprehensive test guidance including test interface selection, test base class usage, and Series parity validation for streaming implementations. Code completion checklists referenced in each agent.

- **Documentation Excellence**: ✅ PASS - Agent definitions themselves serve as documentation for developers. Agents guide developers to maintain XML documentation, update public docs, and follow repository standards. Clear references to authoritative instruction files.

- **Scope & Stewardship**: ✅ PASS - Agents focus exclusively on indicator implementation guidance (within library scope). No trading logic, data fetching, or out-of-scope features. Agents maintain separation of concerns between implementation styles.

**Status**: ✅ PASS - All constitutional principles satisfied. This feature enhances developer guidance without introducing computational logic, maintaining focus on documentation quality and adherence to existing standards.

## Project Structure

### Documentation (this feature)

```
.specify/specs/003-coding-agents/
├── spec.md              # Feature specification (completed)
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (pattern analysis, agent structure decisions)
├── data-model.md        # Phase 1 output (agent definition schema, content organization)
├── quickstart.md        # Phase 1 output (validation scenarios for each agent)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Agent Definition Files (repository root)

```
.github/
├── agents/
│   ├── indicator-series.agent.md           # NEW: Series agent definition (invoked as @series)
│   ├── indicator-buffer.agent.md           # NEW: BufferList agent definition (invoked as @buffer)
│   ├── indicator-stream.agent.md           # UPDATE: StreamHub agent definition (invoked as @streamhub)
│   ├── streamhub-state.agent.md            # EXISTING: Unchanged (sub-agent)
│   ├── streamhub-performance.agent.md      # EXISTING: Unchanged (sub-agent)
│   ├── streamhub-testing.agent.md          # EXISTING: Unchanged (sub-agent)
│   └── streamhub-pairs.agent.md            # EXISTING: Unchanged (sub-agent)
└── instructions/
    ├── indicator-series.instructions.md    # UPDATE: Add agent reference sections
    ├── indicator-buffer.instructions.md    # UPDATE: Add agent reference sections
    └── indicator-stream.instructions.md    # UPDATE: Add agent reference sections
```

**Structure Decision**: Agent definitions live in `.github/agents/` (established pattern from existing StreamHub agents). Each agent definition is a standalone markdown file with YAML frontmatter. Instruction files remain in `.github/instructions/` with minimal additions to reference agents.

**Naming Convention**:

- **Agent files**: Use `indicator-{style}.agent.md` pattern to match instruction files (e.g., `indicator-series.agent.md`)
- **YAML name property**: Use simple style name (e.g., `name: series`, `name: buffer`, `name: streamhub`)
- **Invocation syntax**: Use `@{name}` from YAML property (e.g., `@series`, `@buffer`, `@streamhub`)
- **Special case**: StreamHub file is `indicator-stream.agent.md` (matches `indicator-stream.instructions.md`) but YAML name is `streamhub` for backward compatibility with existing usage

## Phase 0: Outline & Research

### Research Tasks

Based on "NEEDS CLARIFICATION" items above and technical unknowns:

1. **Agent Structure Analysis**
   - **Task**: Analyze existing StreamHub agent family (`stream.agent.md`, `streamhub-state.agent.md`, etc.) to extract consistent patterns for agent definition structure
   - **Deliverable**: Document sections (YAML frontmatter, Expertise, Decision Trees, Usage Examples), content organization, and length guidelines

2. **Content Deduplication Strategy**
   - **Task**: Define clear boundaries between agent content (decision trees + key patterns) and instruction file content (comprehensive implementation details)
   - **Deliverable**: Content matrix mapping topics to agent vs. instruction file ownership

3. **Decision Tree Requirements**
   - **Task**: Identify key decision points for each indicator style (interface selection, provider base class, pattern choice, performance trade-offs)
   - **Deliverable**: Decision tree outlines for Series, Buffer, and StreamHub agents

4. **Reference Implementation Mapping**
   - **Task**: Catalog authoritative code examples in repository for each indicator style (Series: EMA/SMA, Buffer: SMA/Chandelier, Stream: EMA/ADX/Stoch)
   - **Deliverable**: Reference implementation table with file paths and pattern categories

5. **Test Guidance Requirements**
   - **Task**: Document test interface requirements, test base class patterns, and validation expectations for each indicator style
   - **Deliverable**: Test guidance outline for each agent

6. **Instruction File Impact Analysis**
   - **Task**: Review existing instruction files to identify optimal insertion points for "When to use this agent" and "Related agents" sections
   - **Deliverable**: Instruction file modification plan with specific section locations

**Output**: `research.md` with all NEEDS CLARIFICATION resolved and technical decisions documented.

## Phase 1: Design & Contracts

**Prerequisites:** `research.md` complete

### 1. Data Model (`data-model.md`)

Define the structure of agent definition files and instruction file updates:

**Entities:**

- **Agent Definition File Schema**
  - YAML frontmatter: `name` (string, unique identifier), `description` (string, agent purpose)
  - Markdown sections: Expertise, Decision Trees, Reference Implementations, Usage Examples
  - Validation rules: ≤500 lines, no verbatim duplication of instruction files, references to instruction files via relative links

- **Decision Tree Node**
  - Question/condition (string, developer's scenario)
  - Branches (array of options with guidance)
  - Reference (link to instruction file section for details)

- **Reference Implementation Entry**
  - Pattern name (string, e.g., "Basic Series calculation")
  - File path (relative from repo root)
  - Key characteristics (array of strings, e.g., "O(n) single pass", "Stateless")

- **Instruction File Update**
  - "When to use this agent" section: Scenarios where developers should invoke the agent
  - "Related agents" section: Cross-references to other agents (main + sub-agents)
  - Placement: After front matter, before first major heading

**Relationships:**

- Agent Definition → Instruction File (1:1, references via hyperlinks)
- Agent → Sub-agents (1:N for StreamHub, 0:N for Series/Buffer initially)
- Agent → Reference Implementations (1:N, catalog of examples)

**State Transitions:**

- Agent Definition: Draft → Under Review → Published (via PR merge)
- Instruction File: Current → Updated (minimal changes, backward compatible)

### 2. API Contracts (`contracts/`)

Since this feature creates documentation artifacts (not runtime APIs), contracts define the agent invocation patterns and expected responses:

**Agent Invocation Contract:**

```yaml
# contracts/agent-invocation.yml
invocation:
  pattern: "@{agent-name} {developer-query}"
  examples:
    - "@series I need to implement a new moving average indicator"
    - "@buffer Which interface should I use for my BufferList indicator?"
    - "@streamhub How do I optimize my StreamHub for O(1) updates?"

response:
  structure:
    - decision_guidance: "Pattern selection based on query context"
    - reference_links: "Links to instruction file sections"
    - example_code_paths: "Paths to reference implementations"
    - next_steps: "Actionable developer tasks"
  
  constraints:
    - no_verbatim_duplication: "Must not copy instruction file content"
    - concise: "Responses should be actionable, not exhaustive"
    - references_required: "Always link to instruction files for details"
```

**Agent Definition File Contract:**

```yaml
# contracts/agent-definition.yml
schema:
  frontmatter:
    required:
      - name: "Unique agent identifier"
      - description: "Agent purpose and capabilities"
    optional: []  # No tools property (defaults to all)
  
  sections:
    required:
      - "Expertise" # Agent's domain knowledge
      - "Decision Trees" # Pattern selection guidance
      - "Reference Implementations" # Canonical examples
      - "Usage Examples" # Invocation patterns
    optional:
      - "Performance Guidance" # For StreamHub agent specifically
      - "Related Agents" # Cross-references
  
  validation:
    max_lines: 500
    format: "GitHub Flavored Markdown"
    no_duplication: "Must not verbatim copy instruction files"
```

**Instruction File Update Contract:**

```yaml
# contracts/instruction-update.yml
updates:
  required_sections:
    - name: "When to use this agent"
      placement: "After YAML frontmatter, before first major heading"
      content: "Scenarios where developers should invoke the agent"
    
    - name: "Related agents"
      placement: "After YAML frontmatter, adjacent to 'When to use' section"
      content: "Cross-references to main agent and related sub-agents"
  
  constraints:
    minimal_changes: "Add only the 2 required sections"
    backward_compatible: "Do not modify existing content"
    preserve_apply_to: "Keep existing applyTo pattern unchanged"
```

### 3. Quickstart Validation Scenarios (`quickstart.md`)

Define concrete validation scenarios to test each agent:

**Series Agent Validation:**

1. Invoke `@series` with "I need to implement a new momentum indicator" → Should guide on file naming, member ordering, validation patterns
2. Invoke `@series` for test guidance → Should reference test structure, reference calculation expectations, warmup validation
3. Invoke `@series` with incorrect math → Should reference formula sourcing hierarchy and constitution principles

**Buffer Agent Validation:**

1. Invoke `@buffer` with "Which interface for my buffer indicator?" → Should explain IIncrementFromChain/Quote/Pairs selection criteria
2. Invoke `@buffer` for buffer management → Should recommend BufferListUtilities, discourage custom logic
3. Invoke `@buffer` for test guidance → Should emphasize BufferListTestBase inheritance and Series parity validation

**StreamHub Agent Validation:**

1. Invoke `@streamhub` with "Need a new streaming indicator" → Should recommend provider base class (ChainProvider/QuoteProvider/PairsProvider)
2. Invoke `@streamhub` for performance → Should identify O(n²) anti-patterns, suggest O(1) incremental patterns
3. Invoke `@streamhub` for state management → Should explain RollbackState override requirements, reference canonical patterns

### 4. Agent Context Update

Run `.specify/scripts/bash/update-agent-context.sh copilot` after defining data model to update agent-specific context with:

- New agent names (`@series`, `@buffer`, `@streamhub`)
- Agent file paths
- Cross-references between agents and instruction files

**Output**: `data-model.md`, `/contracts/*.yml`, `quickstart.md`, updated agent context file

## Phase 2: Task Planning Approach

*This section describes the approach for `/speckit.tasks` command - actual task generation happens in tasks.md*

### Task Organization Strategy

Tasks will be organized into sequential phases with clear dependencies:

**Phase A: Research & Analysis**

- Analyze existing StreamHub agent structure
- Document decision tree requirements
- Map reference implementations
- Define content deduplication boundaries

**Phase B: Agent Definition Creation**

- Create `indicator-series.agent.md` with YAML `name: series` (P1 - canonical reference agent, invoked as `@series`)
- Create `indicator-buffer.agent.md` with YAML `name: buffer` (P2 - incremental processing agent, invoked as `@buffer`)
- Update `indicator-stream.agent.md` with YAML `name: streamhub` (P2 - align with new agent pattern, invoked as `@streamhub` for backward compatibility)

**Phase C: Instruction File Updates**

- Update `indicator-series.instructions.md` (add "When to use this agent" section directing to `@series` and "Related agents" section)
- Update `indicator-buffer.instructions.md` (add "When to use this agent" section directing to `@buffer` and "Related agents" section)
- Update `indicator-stream.instructions.md` (add "When to use this agent" section directing to `@streamhub` and "Related agents" section)

**Phase D: Validation & Documentation**

- Manual validation via agent invocation (quickstart scenarios)
- Update repository copilot-instructions.md (register new agents)
- Create PR with all changes

### Dependency Mapping

- Phase A completes before Phase B begins (research informs agent design)
- Phase B tasks parallelizable (agents independent of each other)
- Phase C depends on Phase B completion (instruction files reference agents)
- Phase D depends on Phase C completion (validation requires all artifacts)

### Parallelization Opportunities

Within Phase B, all three agent creation tasks can proceed in parallel since:

- Each agent focuses on a distinct indicator style
- No cross-dependencies between agent definition files
- All reference the same research findings from Phase A

Within Phase C, all three instruction file updates can proceed in parallel since:

- Updates are isolated to their respective files
- All follow the same pattern (add 2 sections)
- No cross-file dependencies

### Acceptance Criteria Pattern

Each task will include:

- **Definition of Done**: Clear deliverable (file created/updated with specific content)
- **Validation**: Manual test invocation or file structure check
- **Quality Gate**: Markdown linting passes, follows established patterns
- **Documentation**: Cross-references updated (if applicable)

## Phase 3+: Implementation Execution

*Execution phases handled by `/speckit.implement` command referencing tasks.md*

Implementation will follow Test-Driven Documentation (TDD) pattern:

1. **Red**: Define agent invocation scenario in quickstart.md (expected guidance)
2. **Green**: Create/update agent definition file to provide that guidance
3. **Refactor**: Review for duplication, ensure references to instruction files
4. **Validate**: Manually invoke agent in GitHub Copilot Chat, verify response matches expected guidance

**Implementation Sequence:**

1. Phase A: Research (sequential, foundational)
2. Phase B: Agent creation (parallel where possible)
3. Phase C: Instruction updates (parallel)
4. Phase D: Validation and documentation (sequential, final verification)

**Quality Gates:**

- Each agent definition passes markdown linting
- Agent responses validated against quickstart scenarios
- No verbatim duplication of instruction file content detected
- All cross-references resolve correctly
- Backward compatibility with existing StreamHub sub-agents maintained

## Complexity Tracking

*No constitutional violations identified - section included for completeness*

This feature introduces no constitutional violations:

- Adds documentation artifacts only (no computational logic)
- Enhances existing documentation infrastructure (scoped instructions + custom agents)
- Maintains mathematical precision, performance, validation, testing, and documentation principles
- Aligns with Scope & Stewardship by focusing on developer guidance within library scope

No complexity justifications required.

## Progress Tracking

### Completed

- ✅ Feature specification (`spec.md`) - All sections complete, 5 clarifications integrated
- ✅ Quality checklist created and validated
- ✅ Path migration completed (specs moved to `.specify/specs/`)
- ✅ Implementation plan initiated (`plan.md` - this file)
- ✅ Constitution check passed (all 6 principles satisfied)
- ✅ Phase 0: Research & design decisions (`research.md` complete)
  - Agent structure analysis from existing StreamHub agents
  - Content deduplication strategy defined
  - Decision tree requirements mapped (5-6 per agent)
  - Reference implementations cataloged (15+ examples)
  - Test guidance documented
  - Instruction file impact analysis complete
- ✅ Phase 1: Data model and contracts
  - `data-model.md` complete (entities, relationships, validation rules)
  - `contracts/agent-invocation.yml` complete (invocation patterns, response structure)
  - `contracts/agent-definition.yml` complete (file schema, validation rules)
  - `contracts/instruction-update.yml` complete (update requirements, constraints)
  - `quickstart.md` complete (9 validation scenarios across 3 agents)
  - Agent context updated (`.github/copilot-instructions.md` updated with tech stack)

### Pending

- ⏳ Phase 2: Task generation (`tasks.md` via `/speckit.tasks`)
- ⏳ Phase 3+: Implementation execution (`/speckit.implement`)

**Next Action**: Generate actionable task breakdown via `/speckit.tasks` command.
