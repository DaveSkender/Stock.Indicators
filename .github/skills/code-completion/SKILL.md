---
name: code-completion
description: Quality gates checklist for completing code work before finishing implementation cycles
---

# Code completion checklist

Execute these quality gates before finishing any code work cycle to ensure the codebase is production-ready.

## When to use this skill

Use this skill when:

- Implementing new features
- Fixing bugs
- Refactoring code
- Modifying existing functionality
- Making any changes to C# source code
- Adding or updating indicators
- A human reminds you to run the "code completion checklist"

## Required tools

- #tool:runCommands - Execute shell commands for linting, building, testing
- #tool:read - Read error output and configuration files

## Workflow

### Step 0: Remove dead code

Before running quality gates:

- Delete obsolete or commented-out code
- Remove unused imports, variables, helper methods
- Strip debugging aids (`Console.WriteLine`, breakpoints)
- Remove scratch files (`.bak`, `.new`, `.debug.*`)

### Step 1: Run linters

Execute linting for all code (markdown + .NET):

```bash
dotnet format --severity info --no-restore
npx markdownlint-cli2 --fix
```

Success criteria: **Zero linting errors and zero warnings**. All warnings must be fixed before completing work.

CRITICAL: Suppressing warnings is not an appropriate way to resolve lint or build warnings.

Individual checks (if needed):

```bash
# Roslynator only (fast)
roslynator fix --properties TargetFramework=net10.0 --severity-level info --verbosity normal

# .NET format only
dotnet format --severity info --no-restore

# Markdown only
npx markdownlint-cli2 --fix
```

VS Code tasks: `Lint: All (fix)` or `Lint: .NET code & markdown files (fix)` (faster)

Handle failures:

- Review reported issues
- Fix manually or re-run fix commands
- For unfixable issues, document justification and seek approval
- Re-run lint to verify

### Step 2: Build solution

Verify compilation and build artifacts:

```bash
dotnet build "Stock.Indicators.sln" -v minimal --nologo
```

Success criteria: All projects compile without errors. No build warnings.

VS Code task: `Build: .NET Solution (incremental)`

Handle failures:

- Review compilation errors and warnings
- Fix code issues, type errors, or configuration problems
- Ensure all packages are restored (`dotnet restore`)
- Re-run build to verify

### Step 3: Run test suites

Execute all unit tests (integration tests run in CI only):

```bash
dotnet test "Stock.Indicators.sln" --no-restore --nologo
```

Success criteria: All tests pass. No test failures. Coverage ≥ 98% (validated in CI/CD via Codacy).

VS Code task: `Test: Unit tests`

Handle failures:

- Review test output and failure messages
- Fix code or update tests as appropriate
- Re-run test suite to verify

### Step 4: Update documentation

When changing indicators or public APIs:

- Update XML documentation for changed public APIs
- Update `docs/_indicators/{IndicatorName}.md` for indicator changes
- Update `src/MigrationGuide.V3.md` for breaking changes
- Update obsolete bridge files (`src/Obsolete.V3.*.cs`) for deprecated APIs

### Step 5: Verify and commit

After all gates pass:

```bash
dotnet format --verify-no-changes --severity info --no-restore
dotnet build "Stock.Indicators.sln" -v minimal --nologo
dotnet test "Stock.Indicators.sln" --no-build --nologo
npx markdownlint-cli2
```

- Review changes for completeness
- Address any remaining warnings
- Document intentional deviations
- Commit changes if working in unattended mode

## Quality standards

Before marking work complete:

- **Zero linting errors and zero warnings** - all warnings must be fixed
- **Zero build warnings and zero errors** - all compilation warnings must be addressed
- All projects build successfully
- All tests pass with no warnings
- Coverage threshold maintained (≥ 98% validated in CI/CD)
- Documentation updated for public API changes
- Migration bridges updated for breaking changes

**Critical**: Treat all warnings as errors. Do not ignore, defer, suppress, or accept warnings regardless of scope, type, or reason. Warnings indicate issues that must be resolved.

Abbreviated verification command:

```bash
dotnet format --no-restore && dotnet build && dotnet test --no-restore && npx markdownlint-cli2
```

## Required indicator components

For new or updated indicators, MUST include:

- Series implementation (`*.StaticSeries.cs`)
- Catalog entry and registration (`*.Catalog.cs`)
- Unit tests with full coverage (`*.Tests.cs`)
- Documentation (`docs/_indicators/{Name}.md`)
- Regression test baseline (if algorithm changed)
- Performance benchmark (for complex indicators)

## Required migration bridge

When changing public APIs, MUST:

- Add `[Obsolete]` attribute with migration message
- Update `src/MigrationGuide.V3.md`
- Update bridge files:
  - `src/Obsolete.V3.Indicators.cs`
  - `src/Obsolete.V3.Other.cs`

## Best practices

- Run linters frequently during development, not just at the end
- Address linting errors as they appear
- Use VS Code tasks for quick access to quality gates
- Run specific tests during active development
- Document exceptions when disabling analyzer rules

## Quick reference

| Gate                | Command                                                                                                    | VS Code task                         |
| ------------------- | ---------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| Lint .NET (fix)     | `roslynator fix --properties TargetFramework=net10.0 --severity-level info`                               | `Lint: .NET Roslynator (fix)`        |
| Lint all (fix)      | `dotnet format --severity info --no-restore && npx markdownlint-cli2 --fix`                               | `Lint: All (fix)`                    |
| Lint markdown (fix) | `npx markdownlint-cli2 --fix`                                                                              | `Lint: Markdown (fix)`               |
| Build               | `dotnet build -v minimal --nologo`                                                                         | `Build: .NET Solution (incremental)` |
| Test                | `dotnet test --no-restore --nologo`                                                                        | `Test: Unit tests`                   |
| Full check          | `dotnet format --no-restore && dotnet build && dotnet test --no-restore && npx markdownlint-cli2`         | Run sequentially                     |

## Configuration files

Linting:

- Analyzers: #file:../../../src/Directory.Build.props
- .NET format: #file:../../../.editorconfig
- Roslynator: Global configuration
- Markdown: #file:../../../.markdownlint-cli2.jsonc

Build:

- Solution: #file:../../../Stock.Indicators.sln
- Project: #file:../../../src/Indicators.csproj
- Dependencies: #file:../../../src/Directory.Packages.props

Test:

- Unit tests: #file:../../../tests/tests.unit.runsettings
- Regression: #file:../../../tests/tests.regression.runsettings
- Integration: #file:../../../tests/tests.integration.runsettings

## About maintenance of this file

When lint/build/test commands change or new quality gates are added, update this skill file to reflect current workflow

---
Last updated: January 2, 2026
