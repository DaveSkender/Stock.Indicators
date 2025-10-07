# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Execution flow (main)

```text
1. Load plan.md from feature directory
   → If not found: ERROR "No implementation plan found"
   → Extract: tech stack, libraries, structure
2. Load optional design documents:
   → data-model.md: Extract entities → model tasks
   → contracts/: Each file → contract test task
   → research.md: Extract decisions → setup tasks
3. Generate tasks by category:
   → Setup: project init, dependencies, linting
   → Tests: contract tests, integration tests
   → Core: models, services, CLI commands
   → Integration: DB, middleware, logging
   → Polish: unit tests, performance, docs
4. Apply task rules:
   → Different files = mark [P] for parallel
   → Same file = sequential (no [P])
   → Tests before implementation (TDD)
5. Number tasks sequentially (T001, T002...)
6. Generate dependency graph
7. Create parallel execution examples
8. Validate task completeness:
   → All contracts have tests?
   → All entities have models?
   → All endpoints implemented?
9. Return: SUCCESS (tasks ready for execution)
```

## Format: `[ID] [P?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions

- **Single project**: `src/`, `tests/` at repository root
- **Web app**: `backend/src/`, `frontend/src/`
- **Mobile**: `api/src/`, `ios/src/` or `android/src/`
- Paths shown below assume single project - adjust based on plan.md structure

## Phase 3.1: Setup

- [ ] T001 Create project structure per implementation plan
- [ ] T002 Initialize [language] project with [framework] dependencies
- [ ] T003 [P] Configure linting and formatting tools

## Phase 3.2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE 3.3

**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**

**Template Generation**: Iterate over contracts from `contracts/` directory and integration test scenarios from spec/plan:

- [ ] T00X [P] Contract test for {contract} → tests/contract/{contract}_test.{ext}
  - Example: `Contract test POST /api/users → tests/contract/test_users_post.py`
- [ ] T00X [P] Integration test for {scenario} → tests/integration/test_{scenario}.{ext}
  - Example: `Integration test user registration → tests/integration/test_registration.py`

## Phase 3.3: Core Implementation (ONLY after tests are failing)

**Template Generation**: Iterate over entities from `data-model.md` and endpoints from `contracts/`:

- [ ] T00X [P] {Entity} model → src/models/{entity}.{ext}
  - Example: `User model → src/models/user.py`
- [ ] T00X [P] {Entity}Service CRUD → src/services/{entity}_service.{ext}
  - Example: `UserService CRUD → src/services/user_service.py`
- [ ] T00X [P] CLI --{command} → src/cli/{feature}_commands.{ext}
  - Example: `CLI --create-user → src/cli/user_commands.py`
- [ ] T00X {Method} {Endpoint} endpoint
  - Example: `POST /api/users endpoint`
  - Note: Sequential (no [P]) if endpoints share same file
- [ ] T00X Input validation
- [ ] T00X Error handling and logging

## Phase 3.4: Integration

**Template Generation**: Based on technical plan integration requirements:

- [ ] T00X Connect {Entity}Service to DB
- [ ] T00X {Middleware} middleware
  - Example: `Auth middleware`
- [ ] T00X Request/response logging
- [ ] T00X Security headers and CORS

## Phase 3.5: Polish

**Template Generation**: Based on plan polish requirements:

- [ ] T00X [P] Unit tests for {component} → tests/unit/test_{component}.{ext}
  - Example: `Unit tests for validation → tests/unit/test_validation.py`
- [ ] T00X Performance tests ({threshold})
  - Example: `Performance tests (<200ms)`
- [ ] T00X [P] Update docs/{feature_name}.md
- [ ] T00X Remove duplication
- [ ] T00X Run manual-testing.md if present

## Dependencies

- Document dependencies using the generated task IDs (e.g., `T00A blocks T00B`)
- Ensure tests precede their corresponding implementation tasks
- Integration/polish tasks must wait on the implementation work they depend on

## Parallel example

```markdown
# Example: launch contract/integration tests in parallel when they touch distinct files
Task: "Contract test {method} {path} → {contract_path}"
Task: "Contract test {method} {path} → {contract_path}"
Task: "Integration test {scenario} → {integration_path}"
Task: "Integration test {scenario} → {integration_path}"
```

## Notes

- [P] tasks = different files, no dependencies
- Verify tests fail before implementing
- Commit after each task
- Avoid: vague tasks, same file conflicts

## Task Generation Rules

*Applied during main() execution*

1. **From Contracts**:
   - Each contract file → contract test task [P]
   - Each endpoint → implementation task

2. **From Data Model**:
   - Each entity → model creation task [P]
   - Relationships → service layer tasks

3. **From User Stories**:
   - Each story → integration test [P]
   - Quickstart scenarios → validation tasks

4. **Ordering**:
   - Setup → Tests → Models → Services → Endpoints → Polish
   - Dependencies block parallel execution

## Validation Checklist

*GATE: Checked by main() before returning*

- [ ] All contracts have corresponding tests
- [ ] All entities have model tasks
- [ ] All tests come before implementation
- [ ] Parallel tasks truly independent
- [ ] Each task specifies exact file path
- [ ] No task modifies same file as another [P] task
