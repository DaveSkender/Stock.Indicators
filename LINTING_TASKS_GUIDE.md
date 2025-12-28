# VS Code Linting Tasks Guide

This document explains the VS Code linting task structure and when to use each task.

## Quick Reference

### For Regular Development (Fast)

Use these tasks for quick feedback during normal development:

- **`Lint: .NET code`** - Roslynator analysis only (~seconds)
- **`Lint: .NET code (fix)`** - Auto-fix with Roslynator only (~seconds)
- **`Lint: .NET code & markdown files`** - Roslynator + markdown (~seconds)
- **`Lint: .NET code & markdown files (fix)`** - Auto-fix Roslynator + markdown (~seconds)

### For Comprehensive Checks (Slower)

Use these when you want all linters including the slower `dotnet format`:

- **`Lint: All`** - Roslynator + dotnet format + markdown (complete, slower)
- **`Lint: All (fix)`** - Auto-fix all issues including dotnet format (complete, slower)

### Individual Tools

You can also run individual linting tools directly:

- **`Lint: .NET format`** - Only dotnet format check
- **`Lint: .NET format (fix)`** - Only dotnet format auto-fix
- **`Lint: .NET Roslynator (analyze)`** - Only Roslynator analysis
- **`Lint: .NET Roslynator (fix)`** - Only Roslynator auto-fix
- **`Lint: .NET Roslynator (custom)`** - Custom Roslynator with prompts
- **`Lint: Markdown`** - Only markdown linting
- **`Lint: Markdown (fix)`** - Only markdown auto-fix

## Task Hierarchy

```
Fast Development Tasks (Roslynator only):
├── Lint: .NET code
│   ├── Build: .NET Solution (incremental)
│   └── Lint: .NET Roslynator (analyze)
│
├── Lint: .NET code (fix)
│   └── Lint: .NET Roslynator (fix)
│
├── Lint: .NET code & markdown files
│   ├── Build: .NET Solution (incremental)
│   ├── Lint: .NET code
│   └── Lint: Markdown
│
└── Lint: .NET code & markdown files (fix)
    ├── Lint: .NET code (fix)
    └── Lint: Markdown (fix)

Comprehensive Tasks (includes dotnet format):
├── Lint: All
│   ├── Build: .NET Solution (incremental)
│   ├── Lint: .NET Roslynator (analyze)
│   ├── Lint: .NET format
│   └── Lint: Markdown
│
└── Lint: All (fix)
    ├── Lint: .NET Roslynator (fix)
    ├── Lint: .NET format (fix)
    └── Lint: Markdown (fix)
```

## Why Two Levels?

### Speed vs Completeness Trade-off

**Roslynator** is faster because:
- Focuses on whitespace formatting and quick code fixes
- Works with already-built projects
- Optimized for development loop speed

**dotnet format** is slower because:
- May restore, compile, and run analyzers as part of loading workspace
- Applies more comprehensive .editorconfig rules
- Runs all analyzer-based fixes (CAxxxx, IDExxxx)

### Recommended Workflow

1. **During development**: Use `Lint: .NET code & markdown files (fix)` frequently
2. **Before committing**: Run `Lint: All (fix)` to ensure complete compliance
3. **CI/CD**: Uses both Roslynator and dotnet format (as in test-indicators.yml)

## Deprecated Scripts

The following shell scripts are deprecated and should not be used:

- `tools/scripts/lint-code-check.sh` - Use "Lint: All" task instead
- `tools/scripts/lint-code-fix.sh` - Use "Lint: All (fix)" task instead

These scripts are kept for backwards compatibility but will show a deprecation warning when executed.

## CI/CD Behavior

GitHub Actions workflows run both Roslynator and dotnet format separately:

```yaml
- name: Run Roslynator analysis
  run: roslynator analyze ...

- name: Check .NET code formatting
  run: dotnet format --verify-no-changes ...
```

This ensures comprehensive checking in CI while allowing developers to use faster options locally.
