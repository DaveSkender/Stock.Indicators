---
applyTo: ".specify/**,.github/prompts/speckit.*"
description: "Spec Kit development workflow and artifact editing guidelines"
---

# Copilot Instructions for Spec Kit Development

## Overview

This repository uses [Spec Kit](https://github.com/github/spec-kit) for Specification-Driven Development. All features follow a structured workflow: **specify → clarify → plan → tasks → analyze → implement**.

Spec Kit enables systematic feature development through:

- **Constitution-driven governance** - Project principles guide all decisions
- **Specification-first design** - Define WHAT before HOW
- **Test-driven implementation** - Contracts → tests → code
- **Cross-artifact validation** - Consistency checks across specs, plans, and tasks

## File Structure

### Core configuration

- `.specify/memory/constitution.md` - Project governing principles (source of truth for all architectural decisions)
- `.specify/templates/` - Templates for specs, plans, tasks, and checklists
- `.specify/scripts/bash/` - Helper scripts for workflow automation
- `.github/prompts/speckit.*.prompt.md` - Slash commands for development workflow (`/speckit.constitution`, `/speckit.specify`, etc.)

### Feature-specific artifacts

Each feature lives in `.specify/specs/{###-feature-name}/` with:

- **`spec.md`** - Requirements and user stories (WHAT and WHY)
- **`plan.md`** - Technical implementation plan (HOW, tech stack, architecture)
- **`tasks.md`** - Actionable task breakdown with dependencies
- **`research.md`** - Technical research and decisions (optional)
- **`data-model.md`** - Entity definitions and relationships (optional)
- **`contracts/`** - API specifications (optional)
- **`quickstart.md`** - Key validation scenarios (optional)
- **`checklists/`** - Quality validation checklists (optional)

## Editing Guidelines

### General principles

**Honor existing repository instructions**: Always read and follow existing Copilot instructions, coding standards, and conventions defined elsewhere in the repository. Spec Kit artifacts should comply with repository-wide standards.

**Examples of existing instructions to honor:**

- Language-specific style guides (see [code-completion.instructions.md](code-completion.instructions.md))
- Testing framework conventions (MSTest, test organization patterns)
- Documentation formatting requirements (see [markdown.instructions.md](markdown.instructions.md))
- Package management policies (NuGet via Directory.Packages.props)
- Git workflow and branch naming conventions (see PR guidelines in main copilot-instructions.md)

**When generating Spec Kit artifacts** (especially `quickstart.md`, `research.md`, and code samples in `plan.md`), ensure they align with Stock Indicators repository standards:

- Use `double` for performance, `decimal` only when precision demands it
- Follow span-friendly, allocation-conscious patterns
- Include comprehensive input validation
- Provide XML documentation for all public APIs
- Reference constitution principles in design decisions

### When editing Spec Kit artifacts

#### 1. Constitution (`.specify/memory/constitution.md`)

- This is the **source of truth** for architectural decisions
- Changes require explicit user approval
- Use `/speckit.constitution` command for updates
- Never contradict principles without amending the constitution first
- Current constitution version: See file header for version number

**Stock Indicators constitution principles:**

1. Mathematical Precision (NON-NEGOTIABLE)
2. Performance First
3. Comprehensive Validation
4. Test-Driven Quality
5. Documentation Excellence
6. Scope & Stewardship

#### 2. Specifications (`.specify/specs/{###-feature}/spec.md`)

- Focus on **WHAT and WHY**, not HOW
- Avoid tech stack details (those belong in `plan.md`)
- Use `/speckit.specify` to create, `/speckit.clarify` to refine
- Maintain user story priorities (P1, P2, P3)
- Include functional requirements (FRx), non-functional requirements, and success criteria

**Template sections:**

- Overview / Background
- User scenarios & testing
- Functional requirements
- Key entities
- Implementation phases
- Success criteria
- Constraints & assumptions
- Dependencies & risks

#### 3. Plans (`.specify/specs/{###-feature}/plan.md`)

- Reference constitution compliance in "Constitution Check" section
- Keep high-level; move detailed algorithms to separate files
- Use `/speckit.plan` to generate or update
- Always validate against constitution principles
- **Apply repository coding standards** to any code examples or pseudocode

**Template sections:**

- Summary
- Technical context (language, dependencies, platform)
- Constitution check (MUST pass before Phase 0)
- Project structure
- Phase 0: Research and design decisions
- Phase 1: Design and contracts (data model, API contracts, test scenarios)
- Phase 2: Task planning approach (describe, don't execute)
- Phase 3+: Future implementation (execution phases)
- Complexity tracking (if violations exist)
- Progress tracking

#### Critical: Constitution Check requirement

The plan.md MUST include a constitution check before Phase 0 research. Address each constitutional principle:

```markdown
## Constitution check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Mathematical Precision**: [How this feature ensures mathematical correctness]
- **Performance First**: [Performance considerations and targets]
- **Comprehensive Validation**: [Input validation and edge case handling]
- **Test-Driven Quality**: [Testing strategy]
- **Documentation Excellence**: [Documentation approach]
- **Scope & Stewardship**: [How this aligns with library scope]

**Status**: PASS | NEEDS WORK (violations listed below)
```

If violations exist, document them in the "Complexity tracking" section with justifications.

#### 4. Tasks (`.specify/specs/{###-feature}/tasks.md`)

- Organize by phase and component
- Mark parallelizable tasks with `[P]`
- Use `/speckit.tasks` to generate
- Mark completed tasks with `[X]` or update status
- Include clear acceptance criteria for each task

**Task structure:**

```markdown
### T###: Task title

**Description**: Clear description of task
**Location**: File paths where work happens
**Dependencies**: T### (prerequisite tasks)
**Acceptance criteria**:

- Criterion 1
- Criterion 2
- Criterion 3
```

**Dependency mapping:**

Include a "Dependencies" section mapping task relationships:

```markdown
## Dependencies

- **T001 → T002**: Task 1 must complete before Task 2
- **T003, T004 → T005**: Tasks 3 and 4 must both complete before Task 5
```

#### 5. Quickstart & Validation (`.specify/specs/{###-feature}/quickstart.md`)

- **Critical: Honor repository testing conventions** (see [code-completion.instructions.md](code-completion.instructions.md))
- Follow existing test file organization: `tests/indicators/`, `tests/integration/`, etc.
- Use MSTest framework with standard patterns from existing tests
- Include warmup period validation for indicators
- Test both batch and streaming paths (if applicable)
- Align with repository-wide quality gates (see constitution)
- Respect documentation formatting standards (see [markdown.instructions.md](markdown.instructions.md))

**Stock Indicators quickstart essentials:**

- Load test data: `TestData.GetDefault()` or `TestData.GetCompare()`
- Validate warmup periods: Assert correct number of initial nulls
- Test mathematical accuracy: Compare against reference data
- Test edge cases: Empty quotes, insufficient history, invalid parameters
- Performance validation: BenchmarkDotNet for computationally intensive indicators

### Command workflow reminders

**Standard development flow:**

1. **`/speckit.constitution`** - Establish or update governing principles (one-time or as needed)
2. **`/speckit.specify`** - Define what you want to build (user-focused goals)
3. **`/speckit.clarify`** - Clarify underspecified areas (recommended before planning)
4. **`/speckit.plan`** - Create technical implementation plan with tech stack
5. **`/speckit.tasks`** - Generate actionable task lists
6. **`/speckit.analyze`** - Validate consistency and coverage (run before implement)
7. **`/speckit.implement`** - Execute tasks to build the feature

**Optional commands:**

- **`/speckit.checklist`** - Generate custom quality checklists for specific validation concerns (security, UX, performance)

**Critical workflow gates:**

- Always run `/speckit.clarify` before `/speckit.plan` to reduce rework
- Run `/speckit.analyze` after `/speckit.tasks` and before `/speckit.implement`
- Validate plan completeness and constitution compliance before generating tasks
- Never skip the constitution check in plan.md

### Using quality checklists

The `/speckit.checklist` command generates custom quality validation checklists (think of them as "unit tests for English"). Use it after planning or before implementation to validate specific quality concerns.

**When to use:**

- After `/speckit.plan` to validate architectural decisions
- Before `/speckit.implement` to ensure completeness
- For specific domain validation (security, UX, API design, performance)

**Example usage for Stock Indicators:**

```prompt
/speckit.checklist Generate a performance checklist focusing on allocation budgets, LINQ usage in hot paths, and span-based optimization opportunities
```

```prompt
/speckit.checklist Create a mathematical precision checklist for indicator accuracy, warmup period correctness, and streaming/batch value parity
```

The checklist will be saved to `.specify/specs/{###-feature}/checklists/` and can be manually reviewed and checked off before proceeding with implementation.

### Constitution authority

The project constitution is **non-negotiable** during analysis and implementation. If a specification conflicts with constitutional principles:

1. **Flag the conflict clearly** - Identify which principle is violated and why
2. **Recommend adjusting the spec/plan/tasks** - Align with existing principles
3. **If the principle itself needs revision** - That's a separate explicit action via `/speckit.constitution`

**Never:**

- Silently ignore constitutional violations
- Reinterpret principles to justify violations
- Dilute principle requirements without explicit approval
- Proceed with implementation that conflicts with constitution

**The constitution check in plan.md is a hard gate** - plans with unresolved violations should not proceed to task generation.

### Idiomatic patterns for Stock Indicators

- **Test-first development** - Contracts → tests → implementation (follows Constitution Principle IV)
- **Incremental delivery** - MVP first, then priority-based enhancements
- **Clear separation** - spec.md (what/why) → plan.md (how) → tasks.md (steps)
- **Constitution checks** - Validate during planning phase, not implementation phase
- **Performance validation** - Benchmark computationally intensive indicators
- **Documentation sync** - Update `docs/_indicators/*.md` when behavior changes
- **Streaming parity** - Batch and streaming paths must converge to identical final values

**Stock Indicators specific patterns:**

- Use `double` as default numeric type (constitution principle: Mathematical Precision)
- Minimize allocations in hot loops (constitution principle: Performance First)
- Validate all public API inputs (constitution principle: Comprehensive Validation)
- Provide comprehensive unit tests (constitution principle: Test-Driven Quality)
- Include XML documentation for all public members (constitution principle: Documentation Excellence)
- Stay within library scope boundaries (constitution principle: Scope & Stewardship)

## Integration with existing repository instructions

Spec Kit instructions work alongside existing scoped instructions:

| Spec Kit Artifact        | Also Apply Instructions                                                          |
|--------------------------|----------------------------------------------------------------------------------|
| `plan.md` code examples  | [code-completion.instructions.md](code-completion.instructions.md)               |
| `quickstart.md` testing  | [code-completion.instructions.md](code-completion.instructions.md)               |
| Series indicator specs   | [indicator-series.instructions.md](indicator-series.instructions.md)             |
| Stream indicator specs   | [indicator-stream.instructions.md](indicator-stream.instructions.md)             |
| Buffer indicator specs   | [indicator-buffer.instructions.md](indicator-buffer.instructions.md)             |
| Performance specs        | [performance-testing.instructions.md](performance-testing.instructions.md)       |
| Any markdown files       | [markdown.instructions.md](markdown.instructions.md)                             |
| Documentation specs      | [documentation.instructions.md](documentation.instructions.md)                   |

**When conflicts arise:**

1. Constitution takes precedence over all other instructions
2. Scoped instructions provide detailed implementation guidance
3. Spec Kit instructions provide workflow structure
4. Escalate genuine conflicts to user for resolution

## Common pitfalls to avoid

1. **Skipping clarification** - Running `/speckit.plan` before `/speckit.clarify` leads to rework
2. **Tech stack in spec.md** - Keep implementation details in plan.md, not spec.md
3. **Missing constitution check** - Every plan.md must validate against all 6 constitutional principles
4. **Ignoring dependencies** - Task ordering matters; respect dependency chains in tasks.md
5. **Skipping analysis** - Always run `/speckit.analyze` before `/speckit.implement` to catch issues early
6. **Violating constitution** - Never implement features that conflict with constitutional principles without explicit approval

**Stock Indicators specific pitfalls:**

1. **Off-by-one warmup periods** - Validate lookback/warmup calculations carefully
2. **Forgetting null checks** - Empty quotes cause stateful streaming regressions
3. **LINQ in hot loops** - Performance regressions from unnecessary allocations
4. **Documentation drift** - Sync `docs/_indicators/*.md` with code changes
5. **Missing streaming tests** - Verify batch/streaming value parity for new indicators

## Troubleshooting

### "Not on a feature branch" error

Spec Kit scripts expect branch names like `001-feature-name`. If using a different naming convention:

- Manually create `.specify/specs/###-feature-name/` directory
- Copy templates from `.specify/templates/` to feature directory
- Proceed with normal workflow (spec → clarify → plan → tasks → analyze → implement)

### Constitution check fails

If plan.md constitution check shows violations:

1. Review the specific principle being violated
2. Determine if it's a real violation or misunderstanding
3. If real: Adjust the plan to comply with the principle
4. If principle needs change: Separate `/speckit.constitution` update required
5. Document any temporary exceptions in "Complexity tracking" section with justification and resolution timeline

### Missing prerequisite tools

Spec Kit requires:

- Python 3.11+
- uv (package manager)
- Git

Install Spec Kit globally:

```bash
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git
```

Verify installation:

```bash
uv tool list
specify check
```

## References

- [Spec Kit GitHub Repository](https://github.com/github/spec-kit) - Canonical source
- Local constitution: [constitution.md](../../.specify/memory/constitution.md) - Project principles
- Command templates: [speckit.*.prompt.md](../prompts/) - Slash command implementations
- Main Copilot instructions: [copilot-instructions.md](../copilot-instructions.md) - Repository-wide guidance
- Stock Indicators guiding principles: [Discussion #648](https://github.com/DaveSkender/Stock.Indicators/discussions/648)

---
Last updated: October 7, 2025
