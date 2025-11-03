# Feature Specification: Custom Agents for Series, Buffer, and Stream Indicator Development

**Feature Branch**: `003-create-3-custom`  
**Created**: November 3, 2025  
**Status**: Draft  
**Input**: User description: "create 3 custom agents to support each specific style of indicator implementation. We've already implemented an initial one for stream hub that we can improve and use for lessons learned; however, it will need to be updated no matter what."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Series Indicator Development Support (Priority: P1)

Developers implementing new Series indicators need expert guidance on mathematical precision, input validation patterns, test structure, and adherence to the canonical Series implementation standards.

**Why this priority**: Series indicators are the foundation of the library - all other styles (Buffer, Stream) must match Series results. Series is the source of mathematical truth, making this the most critical agent for maintaining library correctness.

**Independent Test**: Can be fully tested by creating a new Series indicator (e.g., implementing a simple moving average) and invoking `@series` for guidance on implementation structure, validation patterns, and test coverage. Success is measured by the agent providing accurate, style-specific guidance that matches `.github/instructions/indicator-series.instructions.md`.

**Acceptance Scenarios**:

1. **Given** a developer wants to implement a new Series indicator, **When** they invoke `@series` with their indicator description, **Then** the agent provides guidance on file naming, member ordering, validation patterns, and test structure specific to Series style
2. **Given** a developer has written Series indicator code, **When** they ask `@series` for review, **Then** the agent identifies deviations from Series conventions (member ordering, validation patterns, mathematical precision requirements)
3. **Given** a developer needs to write tests, **When** they invoke `@series` for test guidance, **Then** the agent explains test structure, reference calculation expectations, and Series-specific test patterns
4. **Given** a developer encounters a mathematical accuracy issue, **When** they consult `@series`, **Then** the agent references the formula sourcing hierarchy, NaN handling policy, and precision requirements from the constitution

---

### User Story 2 - BufferList Indicator Development Support (Priority: P2)

Developers implementing BufferList indicators need expert guidance on buffer management patterns, interface selection (IIncrementFromChain/Quote/Pairs), universal buffer utilities usage, and equivalence testing with Series results.

**Why this priority**: BufferList indicators enable incremental data processing with efficient memory management. While important, they must match Series results and are secondary to the canonical Series implementations.

**Independent Test**: Can be fully tested by implementing a BufferList indicator (e.g., SMA buffer) and invoking `@bufferlist` for guidance. Success is measured by the agent correctly guiding interface selection, buffer utility usage, and ensuring proper inheritance from `BufferList<TResult>` and test base `BufferListTestBase`.

**Acceptance Scenarios**:

1. **Given** a developer needs to create a BufferList indicator, **When** they invoke `@bufferlist` with their indicator type, **Then** the agent recommends the correct interface (IIncrementFromChain, IIncrementFromQuote, or IIncrementFromPairs) based on the indicator's data requirements
2. **Given** a developer is implementing buffer management logic, **When** they ask `@bufferlist` for guidance, **Then** the agent directs them to use universal `BufferListUtilities` extension methods (`.Update()` or `.UpdateWithDequeue()`) instead of custom buffer logic
3. **Given** a developer needs to write BufferList tests, **When** they consult `@bufferlist`, **Then** the agent emphasizes inheriting from `BufferListTestBase` (not `TestBase`), implementing the correct test interface, and validating equivalence with Series results
4. **Given** a developer encounters a buffer state management issue, **When** they invoke `@bufferlist`, **Then** the agent explains proper `Clear()` implementation patterns, tuple usage for internal state, and avoidance of custom struct anti-patterns
5. **Given** a developer is unsure about constructor patterns, **When** they ask `@bufferlist`, **Then** the agent explains the two-constructor requirement (standard parameters, and parameters + quotes with chaining syntax `: this(params) => Add(quotes);`)

---

### User Story 3 - Stream Indicator Development Support (Priority: P2)

Developers implementing StreamHub indicators need expert guidance on provider selection (ChainProvider/QuoteProvider/PairsProvider), state management patterns, RollbackState implementation, performance optimization (O(1) updates, avoiding O(n²) anti-patterns), and comprehensive test coverage including rollback validation.

**Why this priority**: StreamHub indicators enable real-time processing and must maintain stateful operations efficiently. While critical for streaming scenarios, they must match Series results and are secondary to the canonical Series implementations.

**Independent Test**: Can be fully tested by implementing a StreamHub indicator (e.g., EMA hub) and invoking `@streamhub` for guidance. Success is measured by the agent correctly identifying the provider base class, guiding state management patterns, and ensuring comprehensive test coverage with rollback validation.

**Acceptance Scenarios**:

1. **Given** a developer wants to implement a new StreamHub indicator, **When** they invoke `@streamhub` with their indicator description, **Then** the agent recommends the appropriate provider base class (ChainProvider, QuoteProvider, or PairsProvider) based on input/output requirements
2. **Given** a developer needs to optimize StreamHub performance, **When** they consult `@streamhub` or `@streamhub-performance`, **Then** the agent identifies O(n²) anti-patterns (full series recalculation), suggests O(1) incremental state patterns, and references `RollingWindowMax/Min` utilities
3. **Given** a developer is implementing stateful logic, **When** they ask `@streamhub` or `@streamhub-state` about state management, **Then** the agent explains when to override `RollbackState`, how to rebuild state from cache after Insert/Remove operations, and references canonical patterns (EMA, ADX, Stoch)
4. **Given** a developer needs to write StreamHub tests, **When** they invoke `@streamhub` or `@streamhub-testing`, **Then** the agent explains test interface selection (ITestQuoteObserver, ITestChainProvider, ITestPairsObserver), comprehensive rollback validation requirements, and Series parity expectations
5. **Given** a developer encounters implementation pattern confusion (incremental vs repaint-from-anchor vs full session rebuild), **When** they consult `@streamhub`, **Then** the agent explains the three patterns, when to use each, and provides reference implementations
6. **Given** a developer is implementing a dual-stream indicator, **When** they invoke `@streamhub-pairs`, **Then** the agent explains PairsProvider usage, timestamp synchronization requirements, dual-cache coordination, and references Correlation/Beta implementations

---

### Edge Cases

- What happens when a developer invokes the wrong agent for their indicator style (e.g., asks `@series` about StreamHub-specific patterns)?
  - Agent should recognize the style mismatch and redirect to the appropriate agent (e.g., "For StreamHub indicators, please consult `@streamhub` instead")
- How does the system handle indicators that implement multiple styles (Series + Buffer + Stream)?
  - Each agent focuses on its specific style; developers can consult multiple agents sequentially for comprehensive guidance
- What happens when an existing StreamHub agent (`@streamhub`, `@streamhub-state`, etc.) overlaps with the new agent responsibilities?
  - The new agents will complement existing specialized agents; `@series` and `@buffer` are new, while `@streamhub` gets updated to align with the new agent pattern
- How do agents handle evolving coding standards or instruction file updates?
  - Agents reference the scoped instruction files as source of truth; updates to instruction files automatically propagate to agent behavior

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide three custom agents: `@series`, `@bufferlist`, and `@streamhub` (updated)
- **FR-002**: Each agent MUST focus exclusively on its indicator style (Series, BufferList, StreamHub) and reference the corresponding scoped instruction file. Instruction files will be updated with minimal additions: "When to use this agent" and "Related agents" sections to support agent discoverability and routing.
- **FR-003**: Agents MUST provide guidance on file naming conventions, member ordering, implementation patterns, and test structure specific to their style using a dual approach: brief overviews within the agent definition plus explicit references to detailed checklists in scoped instruction files
- **FR-004**: Agents MUST reference authoritative code examples from the repository (e.g., EMA, SMA, ADX, Chandelier, Correlation)
- **FR-005**: Agents MUST explain when to use specific patterns, interfaces, or base classes based on indicator requirements
- **FR-006**: Agents MUST emphasize mathematical precision requirements from the constitution (formula sourcing hierarchy, NaN handling policy)
- **FR-007**: Agents MUST guide developers on test coverage expectations, including specific test interfaces and validation requirements
- **FR-008**: Agents MUST redirect developers to specialized sub-agents when appropriate (e.g., `@streamhub` → `@streamhub-state` for deep state management questions)
- **FR-009**: Series agent MUST emphasize Series-as-canonical-reference principle and that BufferList/StreamHub must match Series results
- **FR-010**: BufferList agent MUST emphasize universal buffer utilities usage (`BufferListUtilities`) and discourage custom buffer management
- **FR-011**: StreamHub agent MUST explain the three implementation patterns (incremental state, repaint-from-anchor, full session rebuild) and when to use each
- **FR-012**: StreamHub agent MUST explain performance standards (≤1.5x Series target) and common anti-patterns (O(n²) recalculation, O(n) window scans)
- **FR-013**: StreamHub agent MUST emphasize RollbackState override requirements for stateful indicators
- **FR-014**: All agents MUST reference the code completion checklists from their respective instruction files
- **FR-015**: Agents MUST maintain consistency with existing repository documentation and custom agent patterns established by StreamHub agents

### Key Entities

- **Custom Agent**: A specialized AI assistant invoked via `@<agent-name>` that provides expert guidance on a specific indicator implementation style
- **Scoped Instruction File**: Markdown file in `.github/instructions/` containing comprehensive guidelines for a specific indicator style (e.g., `indicator-series.instructions.md`)
- **Agent Definition File**: Markdown file in `.github/agents/` containing agent metadata (name, description, expertise, usage examples) and instructions. Uses YAML frontmatter with minimal properties: `name` (unique agent identifier) and `description` (agent purpose and capabilities), following the pattern established by existing StreamHub agents.
- **Indicator Style**: One of three implementation approaches (Series, Buffer, StreamHub) with distinct patterns, base classes, and requirements
- **Reference Implementation**: Canonical code example in the repository that demonstrates correct implementation patterns (e.g., `Ema.StaticSeries.cs`, `Sma.BufferList.cs`, `Adx.StreamHub.cs`)
- **Test Interface**: Interface defining required test methods for a specific indicator style or capability (e.g., `ITestChainObserver`, `BufferListTestBase`)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Developers can invoke each agent (`@series`, `@bufferlist`, `@streamhub`) and receive style-specific guidance within a single chat interaction
- **SC-002**: Agent responses reference the correct scoped instruction file and authoritative code examples 100% of the time
- **SC-003**: Agents correctly identify and explain indicator-specific patterns (e.g., interface selection for BufferList, provider base class for StreamHub) based on developer's indicator description
- **SC-004**: StreamHub agent update maintains backward compatibility with existing StreamHub sub-agents (`@streamhub-state`, `@streamhub-performance`, `@streamhub-testing`, `@streamhub-pairs`)
- **SC-005**: Developers implementing indicators with agent guidance produce code that passes the code completion checklists from instruction files on first review (measured by PR review feedback)
- **SC-006**: Agent responses align with constitution principles (mathematical precision, performance first, comprehensive validation) as verified by spot-checking agent outputs
- **SC-007**: Series and BufferList agents provide value comparable to existing StreamHub agents as measured by developer feedback and usage patterns
- **SC-008**: Agent definitions follow consistent structure and conventions established by existing StreamHub agent files

## Assumptions

- Developers are familiar with basic GitHub Copilot Chat usage and custom agent invocation syntax (`@agent-name`)
- Scoped instruction files (`.github/instructions/indicator-*.instructions.md`) are comprehensive and up-to-date
- Existing StreamHub agent structure (`.github/agents/streamhub*.agent.md`) provides a viable pattern for Series and BufferList agents
- Developers have access to reference implementations in the repository
- GitHub Copilot supports the custom agent pattern used in this repository
- The instruction file pattern (scoped to file paths via `applyTo`) works consistently with custom agent definitions

## Dependencies

- GitHub Copilot Chat with custom agent support
- Existing scoped instruction files:
  - `.github/instructions/indicator-series.instructions.md`
  - `.github/instructions/indicator-buffer.instructions.md`
  - `.github/instructions/indicator-stream.instructions.md`
- Existing custom agent definitions (StreamHub family) as reference patterns
- Repository constitution (`.specify/memory/constitution.md`) defining guiding principles
- Reference implementations for each style in `src/` directories

## Constraints

- Agents must not duplicate content from instruction files verbatim; agents include decision trees and key pattern summaries while instruction files remain the canonical source for comprehensive implementation details and full checklists
- Agent responses must remain concise while providing sufficient context for developers to take action
- Agents must maintain separation of concerns: Series focuses on batch calculations, BufferList on incremental processing, StreamHub on real-time stateful operations
- StreamHub agent update must preserve existing sub-agent ecosystem without breaking changes
- All agent definitions must follow the repository's markdown formatting standards (`.markdownlint-cli2.jsonc`)
- Agent names must be concise and memorable (single word where possible)

## Out of Scope

- Automated code generation or implementation by agents (agents provide guidance only)
- Agents for non-indicator code (catalog, utilities, testing frameworks)
- Migration of existing indicators (agents focus on new implementations or updates)
- Multi-agent orchestration or workflow automation
- Agent training or fine-tuning beyond definition file content
- Support for non-GitHub Copilot AI assistants
- Specialized sub-agents for Series or BufferList (unlike StreamHub's specialized family); initial scope includes only the three primary style agents
- Cross-style implementation generation (e.g., auto-generating Buffer from Series)

## Clarifications

### Session 2025-11-03

- Q: What required YAML frontmatter properties should each agent file include? → A: Minimal frontmatter with only `name` and `description` (matching existing StreamHub agent pattern), no explicit `tools` property (defaults to all available tools)
- Q: How should each agent structure its step-by-step guidance? → A: Dual approach - Brief overview in agent + explicit references to detailed checklists in instruction files
- Q: What content should agents include versus defer to instruction files? → A: Agents include decision trees + key pattern summaries; Instruction files have comprehensive implementation details + full checklists
- Q: What updates should be made to instruction files? → A: Minimal additions - Add "When to use this agent" and "Related agents" sections only
- Q: What naming pattern should be used for agent files? → A: Simple style names matching agent invocation: `series.agent.md`, `buffer.agent.md`, `stream.agent.md`
