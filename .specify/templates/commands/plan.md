---
description: Create a detailed implementation plan for a feature specification.
scripts:
  sh: scripts/create-plan.sh
  ps: scripts/create-plan.ps1
---

The user input to you can be provided directly by the agent or as a command argument - you **MUST** consider it before proceeding with the prompt (if not empty).

User input:

$ARGUMENTS

1. Identify the target specification from user input or find the most recent spec in `specs/`
2. Read the specification file to understand requirements
3. Create a comprehensive `plan.md` in the same specification directory that includes:

   **Architecture Overview**
   - Code organization within the existing src/ structure (a-d/, e-k/, m-r/, s-z/)
   - Integration points with existing indicators and utilities
   - Data flow and processing pipeline design
   - Performance optimization strategies

   **Implementation Phases**
   
   *Phase 1: Core Implementation*
   - Create indicator method signature following established patterns
   - Implement mathematical calculation logic
   - Add input validation and error handling
   - Create result model with appropriate data types

   *Phase 2: Performance Optimization*
   - Implement streaming support if applicable
   - Add buffer management for real-time scenarios
   - Optimize memory allocation and span usage
   - Add performance benchmarks

   *Phase 3: Testing & Validation*
   - Create comprehensive unit tests
   - Validate against reference implementations
   - Add edge case testing
   - Performance regression testing

   *Phase 4: Documentation & Integration*
   - Complete XML documentation
   - Add usage examples
   - Update catalog entries if applicable
   - Integration with existing chainable indicators

   **Technical Decisions**
   - Precision choice (double vs decimal) with rationale
   - Streaming vs batch-only implementation
   - Memory management strategy
   - Parameter validation approach

   **File Structure**
   - Specific file paths and organization
   - Dependencies on existing utilities
   - Test file locations and patterns

   **Risk Assessment**
   - Technical challenges and mitigation strategies
   - Performance implications
   - Breaking change considerations

4. Ensure the plan aligns with project constitution and coding standards
5. Save the plan to the specification directory
6. Report readiness for task generation phase

The plan should provide clear guidance for implementation without ambiguity.