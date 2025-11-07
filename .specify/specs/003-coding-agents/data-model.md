# Data Model: Custom Agents for Series, Buffer, and Stream Indicator Development

**Feature**: `003-coding-agents`  
**Date**: November 3, 2025  
**Status**: Phase 1 - Design

## Overview

This document defines the data structures and relationships for custom GitHub Copilot agent definitions and their integration with scoped instruction files.

## Core Entities

### 1. Agent Definition File

**Purpose**: Markdown file defining a custom GitHub Copilot agent with expertise, decision guidance, and usage patterns.

**Location**: `.github/cheatsheets/indicator-{style}.agent.md`

**Schema**:

```yaml
# YAML Frontmatter
name: string          # Unique agent identifier for invocation (e.g., "series", "buffer", "streamhub")
description: string   # One-line agent purpose and capabilities
# Note: 'tools' property omitted (defaults to all available tools)
```

**Markdown Structure**:

```markdown
# {Agent Name} Development Agent

You are a {domain} development expert for the Stock Indicators library.

## Your Expertise

- {Capability 1}
- {Capability 2}
- ...

## Decision Trees

### {Decision Point 1}
...

### {Decision Point 2}
...

## Key Patterns

{Brief pattern summaries with references}

## Reference Implementations

- `{file-path}` - {1-line description}
- ...

## Testing Guidance

{Style-specific test requirements}

## Documentation Requirements

{XML docs, inline comments, public docs expectations}

## When to use this agent

- {Scenario 1}
- {Scenario 2}
- ...

## Related agents

- `@{agent}` - {description}
- ...

## Example usage

```text
@{agent-name} {query}
```

```

**Validation Rules**:

- File size: ≤500 lines (maintain readability)
- Frontmatter: Exactly 2 properties (name, description)
- References: All instruction file links must resolve
- No duplication: Must not verbatim copy instruction file content
- Formatting: Must pass markdownlint-cli2

**Instances**:

- `indicator-series.agent.md` (YAML `name: series`, invoked as `@series`) - Series indicator development agent
- `indicator-buffer.agent.md` (YAML `name: buffer`, invoked as `@buffer`) - BufferList indicator development agent
- `indicator-stream.agent.md` (YAML `name: streamhub`, invoked as `@streamhub`) - StreamHub indicator development agent (update existing)

### 2. Decision Tree

**Purpose**: Structured guidance for developers choosing between implementation options.

**Structure**:

```markdown
### {Decision Point Title}

**Scenario**: {When developer faces this choice}

**Options**:

1. **{Option A Name}**
   - When to use: {Selection criteria}
   - Key characteristics:
     - {Characteristic 1}
     - {Characteristic 2}
   - Reference: [section](../instructions/{file}.instructions.md#{anchor})

2. **{Option B Name}**
   - When to use: {Selection criteria}
   - Key characteristics:
     - {Characteristic 1}
     - {Characteristic 2}
   - Reference: [section](../instructions/{file}.instructions.md#{anchor})

**Example**: `{Concrete indicator using each option}`
```

**Properties**:

- `title` (string): Decision point name
- `scenario` (string): When developer encounters this choice
- `options` (array of DecisionOption): Available choices

**DecisionOption Properties**:

- `name` (string): Option identifier
- `whenToUse` (string): Selection criteria
- `characteristics` (array of string): Key attributes
- `reference` (RelativeLink): Link to instruction file section
- `example` (string, optional): Concrete indicator example

**Validation Rules**:

- Minimum 2 options per decision
- All references must resolve to valid instruction file sections
- Examples must reference actual repository files

**Typical Count**:

- Series agent: 5 decision trees
- Buffer agent: 5 decision trees
- StreamHub agent: 6 decision trees

### 3. Reference Implementation Entry

**Purpose**: Catalog of canonical code examples demonstrating correct patterns.

**Structure**:

```markdown
## Reference Implementations

**{Pattern Category}**:

- `{relative-path-from-repo-root}` - {1-line description}
- `{relative-path-from-repo-root}` - {1-line description}

**{Another Category}**:

- `{relative-path-from-repo-root}` - {1-line description}
```

**Properties**:

- `category` (string): Pattern grouping (e.g., "Basic moving average", "Incremental state")
- `filePath` (string): Relative path from repository root
- `description` (string): 1-line summary of key characteristics
- `whenToUse` (string, optional): Guidance on when this pattern applies

**Validation Rules**:

- All file paths must exist in repository
- Descriptions limited to ≤100 characters
- Categories align with instruction file organization

**Typical Count**:

- Series agent: 5 reference implementations
- Buffer agent: 5 reference implementations
- StreamHub agent: 8 reference implementations (includes repaint pattern)

### 4. Instruction File Update

**Purpose**: Minimal additions to existing scoped instruction files to reference custom agents.

**Location**: `.github/instructions/indicator-{style}.instructions.md`

**Structure**:

```markdown
---
applyTo: "{file-pattern}"
description: "{existing description}"
---

## When to use this agent

Invoke `@{agent-name}` when you need help with:

- {Scenario 1}
- {Scenario 2}
- {Scenario 3}
- ...

For comprehensive implementation details, continue reading this document.

## Related agents

{For Series and Buffer agents}:
- `@series` - Series indicator development guidance (if not self-reference)
- `@buffer` - BufferList indicator development guidance (if not self-reference)
- `@streamhub` - StreamHub indicator development guidance (if not self-reference)

{For StreamHub agent - include sub-agents}:
- `@streamhub-state` - RollbackState patterns and cache replay strategies
- `@streamhub-performance` - Performance optimization and O(1) patterns
- `@streamhub-testing` - Test coverage and rollback validation
- `@streamhub-pairs` - Dual-stream indicators (PairsProvider usage)

See also: `.github/cheatsheets/{agent-name}.agent.md` for decision trees and quick reference.

# {Existing first heading}

{Rest of document unchanged}
```

**Properties**:

- `placement` (enum): "after-frontmatter-before-content" (only supported option)
- `whenToUseScenarios` (array of string): 5-7 scenarios for agent invocation
- `relatedAgents` (array of AgentReference): Cross-references
- `preserveExistingContent` (boolean): Always true

**AgentReference Properties**:

- `agentName` (string): Agent identifier (e.g., "series", "streamhub-state")
- `description` (string): Brief agent purpose

**Validation Rules**:

- Preserve all existing content unchanged
- Add exactly 2 new sections
- Total addition: ~15-25 lines
- Maintain applyTo pattern unchanged
- All agent references must exist

**Instances**:

- `indicator-series.instructions.md` - Add agent reference sections
- `indicator-buffer.instructions.md` - Add agent reference sections
- `indicator-stream.instructions.md` - Add agent reference sections (include sub-agents)

## Relationships

### Agent → Instruction File (1:1)

Each agent definition references exactly one primary scoped instruction file:

- `indicator-series.agent.md` (invoked as `@series`) → `indicator-series.instructions.md`
- `indicator-buffer.agent.md` (invoked as `@buffer`) → `indicator-buffer.instructions.md`
- `indicator-stream.agent.md` (invoked as `@streamhub`) → `indicator-stream.instructions.md`

**Link Pattern**: `../instructions/{file}.instructions.md#{section-anchor}`

### Agent → Reference Implementations (1:N)

Each agent catalogs 5-8 reference implementations:

- Series: Basic patterns (SMA, EMA, Donchian, Alligator, AtrStop)
- Buffer: Interface patterns (Chain, Quote, UpdateWithDequeue, Tuple state)
- StreamHub: Pattern variety (Incremental, Repaint from anchor, Full rebuild)

**Cardinality**: 1 agent → 5-8 implementations

### Agent → Sub-agents (0:N)

StreamHub has specialized sub-agents; Series and Buffer initially do not:

- `indicator-stream.agent.md` → `streamhub-state.agent.md` (state management)
- `indicator-stream.agent.md` → `streamhub-performance.agent.md` (optimization)
- `indicator-stream.agent.md` → `streamhub-testing.agent.md` (test coverage)
- `indicator-stream.agent.md` → `streamhub-pairs.agent.md` (dual-stream)

**Cardinality**: Series/Buffer → 0 sub-agents, StreamHub → 4 sub-agents

### Agent → Decision Trees (1:N)

Each agent contains 5-6 decision trees:

- Series: 5 decision points
- Buffer: 5 decision points
- StreamHub: 6 decision points

**Cardinality**: 1 agent → 5-6 decision trees

### Instruction File → Agents (1:1 primary + N related)

Each instruction file references:

- 1 primary agent (self)
- 0-4 related agents (cross-references)

**Example** (indicator-stream.instructions.md):

- Primary: `@streamhub`
- Related: `@streamhub-state`, `@streamhub-performance`, `@streamhub-testing`, `@streamhub-pairs`

## State Transitions

### Agent Definition Lifecycle

```
Draft → Under Review → Published
  ↓         ↓              ↓
(file created) (PR review) (PR merged to branch)
```

**States**:

- **Draft**: Agent file exists in feature branch, content in progress
- **Under Review**: PR opened, subject to markdown linting and content review
- **Published**: Merged to main branch, available for developer invocation

**Transitions**:

- Draft → Under Review: PR creation triggers markdown linting, duplication check
- Under Review → Published: PR approval and merge
- Under Review → Draft: Requested changes, PR updated

### Instruction File Update Lifecycle

```
Current → Updated
   ↓         ↓
(baseline) (2 sections added)
```

**States**:

- **Current**: Instruction file in current state (before agent references)
- **Updated**: Instruction file with "When to use this agent" and "Related agents" sections added

**Transition**:

- Current → Updated: Minimal changes applied, backward compatible

**Validation**:

- All existing content preserved
- applyTo pattern unchanged
- Total changes ≤30 lines

## Data Integrity Constraints

### Reference Integrity

1. **Agent → Instruction File**
   - All instruction file links in agent definitions must resolve
   - Anchor references must match actual section headings

2. **Agent → Reference Implementation**
   - All file paths must exist in repository
   - Referenced files must match the described pattern

3. **Instruction File → Agent**
   - All agent references must exist (file created or planned)
   - Cross-references must be bidirectional (agent ↔ instruction)

### Content Consistency

1. **No Verbatim Duplication**
   - Agent decision trees are unique (not in instruction files)
   - Key pattern summaries in agents ≠ full patterns in instructions
   - Code examples in agents are references, not full implementations

2. **Terminology Alignment**
   - Pattern names consistent across agents and instructions
   - Interface names match actual .NET interface names
   - Reference implementation categories align across agents

### Version Consistency

1. **Agent ↔ Instruction File**
   - When instruction file updates patterns, agent must reference updated section
   - When agent adds decision tree, instruction file should cover the pattern

2. **Cross-Agent References**
   - Related agents section must stay synchronized
   - Sub-agent references in parent agent must match actual sub-agents

## Entity Relationship Diagram

```
┌─────────────────────────┐
│   Agent Definition      │
│  (series/buffer/        │
│   streamhub)            │
└───────┬─────────────────┘
        │
        │ 1:1
        ↓
┌─────────────────────────┐       ┌─────────────────────────┐
│ Instruction File        │←──────│  Reference              │
│ (indicator-*.           │  N:1  │  Implementation         │
│  instructions.md)       │       │  Entry                  │
└─────────────────────────┘       └─────────────────────────┘
        ↑                                   ↑
        │                                   │
        │ 1:1                              │ 1:N
        │                                   │
┌─────────────────────────┐       ┌─────────────────────────┐
│ Instruction File        │       │   Agent Definition      │
│ Update                  │       │   (same as above)       │
└─────────────────────────┘       └───────┬─────────────────┘
                                          │
                                          │ 1:N
                                          ↓
                                  ┌─────────────────────────┐
                                  │  Decision Tree          │
                                  └─────────────────────────┘
                                          ↓
                                          │ 1:N
                                          ↓
                                  ┌─────────────────────────┐
                                  │  Decision Option        │
                                  └─────────────────────────┘
```

## File Modification Summary

### New Files (2)

1. `.github/cheatsheets/indicator-series.agent.md` - Series agent definition (YAML `name: series`, invoked as `@series`)
2. `.github/cheatsheets/indicator-buffer.agent.md` - Buffer agent definition (YAML `name: buffer`, invoked as `@buffer`)

### Updated Files (4)

1. `.github/cheatsheets/indicator-stream.agent.md` - Align with new agent pattern (YAML `name: streamhub`, invoked as `@streamhub`)
2. `.github/instructions/indicator-series.instructions.md` - Add agent reference sections
3. `.github/instructions/indicator-buffer.instructions.md` - Add agent reference sections
4. `.github/instructions/indicator-stream.instructions.md` - Add agent reference sections

### Unchanged Files (4)

1. `.github/cheatsheets/streamhub-state.agent.md` - No changes required
2. `.github/cheatsheets/streamhub-performance.agent.md` - No changes required
3. `.github/cheatsheets/streamhub-testing.agent.md` - No changes required
4. `.github/cheatsheets/streamhub-pairs.agent.md` - No changes required

## Next Steps

With data model defined, proceed to:

1. Create contracts/ (agent invocation, definition, update contracts in YAML)
2. Create quickstart.md (validation scenarios for each agent)
3. Run agent context update script
