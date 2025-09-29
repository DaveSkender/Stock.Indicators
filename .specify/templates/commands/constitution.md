# Constitution Command Template

---
description: Create or update the project constitution with governance principles and standards.
scripts:
  sh: scripts/update-constitution.sh
  ps: scripts/update-constitution.ps1
---

The user input to you can be provided directly by the agent or as a command argument - you **MUST** consider it before proceeding with the prompt (if not empty).

User input:

$ARGUMENTS

1. Review the current constitution at `.specify/memory/constitution.md`
2. Consider any updates or amendments based on user input and current project needs
3. Update the constitution with current governance principles including:
   - Mathematical precision standards for financial calculations
   - Performance requirements and optimization guidelines
   - Testing and validation requirements
   - Documentation standards
   - API design consistency rules
   - Development workflow processes

4. Ensure the constitution reflects the current state of the Stock Indicators project:
   - .NET 8.0 and 9.0 multi-targeting
   - Focus on streaming indicators and real-time processing
   - Emphasis on accuracy over speed where financial precision matters
   - Comprehensive unit testing with >95% coverage expectations
   - Performance benchmarking for computational indicators

5. Update version information and amendment dates as appropriate
6. Save the updated constitution back to `.specify/memory/constitution.md`

The constitution should serve as the definitive governance document for all development decisions in the Stock Indicators library.
