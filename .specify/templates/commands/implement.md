---
description: Execute the implementation plan by working through the generated tasks.
scripts:
  sh: scripts/execute-implementation.sh
  ps: scripts/execute-implementation.ps1
---

The user input to you can be provided directly by the agent or as a command argument - you **MUST** consider it before proceeding with the prompt (if not empty).

User input:

$ARGUMENTS

1. Identify the target specification directory from user input or find the most recent spec
2. Read `spec.md`, `plan.md`, and `tasks.md` to understand the full implementation scope
3. Execute the implementation following the task dependency order:

   **Implementation Process**

   *Pre-Implementation*
   - Validate all prerequisite tools and dependencies are available
   - Confirm the target indicator doesn't already exist
   - Create a feature branch for the implementation
   - Set up development environment per setup tasks

   *Core Implementation*
   - Execute setup tasks (T001-T010) in order
   - Implement core functionality following the mathematical specification
   - Create proper file structure in the appropriate src/ subdirectory
   - Follow established patterns for method signatures and parameter validation
   - Implement result models following existing conventions

   *Quality Assurance*
   - Execute testing tasks in parallel where possible
   - Validate mathematical accuracy against reference implementations
   - Ensure comprehensive test coverage (>95% target)
   - Run performance benchmarks and validate against requirements
   - Check compliance with coding standards and analyzers

   *Documentation and Integration*
   - Complete XML documentation for all public APIs
   - Create usage examples and validation samples
   - Update any catalog entries if applicable
   - Ensure integration with existing indicator patterns

   **Implementation Guidelines**
   - Follow the project constitution for all decisions
   - Use `double` precision by default, `decimal` only when business accuracy demands it
   - Minimize memory allocations and prefer span-friendly loops
   - Implement comprehensive input validation with descriptive error messages
   - Maintain backward compatibility with existing APIs
   - Follow established naming conventions and code organization

   **Validation Steps**
   - Build the solution without warnings
   - Run all unit tests including new tests
   - Execute performance benchmarks
   - Validate XML documentation completeness
   - Check analyzer compliance

   **Completion Criteria**
   - All tasks marked as complete
   - Mathematical accuracy validated against reference data
   - Performance meets or exceeds requirements
   - Test coverage above 95%
   - Documentation complete and accurate
   - No analyzer warnings or build errors

4. Track progress through each task systematically
5. Report any issues or blockers encountered during implementation
6. Document any deviations from the original plan with rationale
7. Provide final validation report with test results and performance metrics

The implementation should result in a production-ready indicator that meets all specification requirements and project standards.