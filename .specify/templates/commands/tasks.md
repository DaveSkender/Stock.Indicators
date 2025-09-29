---
description: Generate actionable, dependency-ordered tasks from an implementation plan.
scripts:
  sh: scripts/create-tasks.sh
  ps: scripts/create-tasks.ps1
---

The user input to you can be provided directly by the agent or as a command argument - you **MUST** consider it before proceeding with the prompt (if not empty).

User input:

$ARGUMENTS

1. Identify the target specification directory from user input or find the most recent spec
2. Read both `spec.md` and `plan.md` to understand requirements and approach
3. Generate a detailed `tasks.md` file with dependency-ordered implementation tasks:

   **Task Categories**

   *Setup Tasks (T001-T010)*
   - Create indicator directory structure
   - Set up project references and dependencies
   - Configure development environment

   *Core Implementation Tasks (T011-T050)*
   - Create method signature and parameter validation
   - Implement mathematical calculation logic
   - Create result model and data structures
   - Add input data validation and error handling
   - Implement streaming support (if applicable)

   *Testing Tasks (T051-T080) [P]*
   - Create unit test framework
   - Add calculation accuracy tests against reference data
   - Create edge case and boundary condition tests
   - Add performance regression tests
   - Validate mathematical precision

   *Performance Tasks (T081-T100) [P]*
   - Add performance benchmarks
   - Optimize memory allocation patterns
   - Implement buffer management for streaming
   - Profile and optimize hot paths

   *Documentation Tasks (T101-T120) [P]*
   - Complete XML documentation
   - Create usage examples and code samples
   - Update API documentation
   - Add mathematical references

   *Integration Tasks (T121-T140)*
   - Update catalog entries
   - Add chainability support (if applicable)
   - Integration testing with existing indicators
   - Final validation and cleanup

   **Task Format**
   Each task should include:
   - Task ID (T001, T002, etc.)
   - Clear, actionable description
   - Specific file paths and locations
   - Dependencies on other tasks
   - Parallel execution indicator [P] where applicable
   - Acceptance criteria
   - Estimated effort

   **Dependency Rules**
   - Setup tasks must complete before implementation
   - Core implementation before performance optimization
   - Testing can run in parallel with implementation [P]
   - Documentation can run in parallel with testing [P]
   - Integration tasks depend on core completion

4. Mark tasks that can be executed in parallel with [P] indicator
5. Include specific file paths and code locations for each task
6. Save the tasks file to the specification directory
7. Report readiness for implementation phase

Tasks should be specific enough that they can be executed by an AI coding assistant without additional context.