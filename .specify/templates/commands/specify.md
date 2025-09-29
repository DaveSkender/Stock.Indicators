---
description: Create or update a feature specification from a natural language description.
scripts:
  sh: scripts/create-specification.sh
  ps: scripts/create-specification.ps1
---

# Specify Command Template

The user input to you can be provided directly by the agent or as a command argument - you **MUST** consider it before proceeding with the prompt (if not empty).

User input:

$ARGUMENTS

Given the feature description provided in the user input, create a detailed specification:

1. Determine the appropriate spec identifier (e.g., 002-new-indicator-name)
2. Create the specification directory under `specs/` if it doesn't exist
3. Write a comprehensive `spec.md` file that includes:

   **Feature Overview**
   - Clear description of the technical indicator or feature
   - Purpose and use cases in financial analysis
   - Target users (quantitative analysts, traders, developers)

   **Mathematical Foundation**
   - Precise mathematical formulation
   - Input data requirements (OHLCV, periods, parameters)
   - Calculation methodology and algorithms
   - Expected output format and data types

   **Technical Requirements**
   - Performance characteristics and optimization needs
   - Memory usage considerations
   - Streaming vs. batch processing capabilities
   - Precision requirements (double vs. decimal)

   **API Design**
   - Method signatures and parameter validation
   - Return types and result models
   - Integration with existing indicator patterns
   - Chainability and reusability considerations

   **Validation Criteria**
   - Reference implementations for accuracy testing
   - Edge cases and boundary conditions
   - Performance benchmarks and acceptance criteria
   - Unit test coverage requirements

   **Documentation Requirements**
   - XML documentation standards
   - Usage examples and code samples
   - Mathematical references and citations

4. Ensure the specification aligns with the project constitution and existing patterns
5. Create the spec file at the appropriate path under `specs/`
6. Report the location and readiness for the next development phase

The specification should be detailed enough for implementation without additional clarification.
