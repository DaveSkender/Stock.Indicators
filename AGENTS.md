# Stock Indicators for .NET

This repository hosts **Stock Indicators for .NET**, the production source for the <a href="https://www.nuget.org/packages/Skender.Stock.Indicators">Skender.Stock.Indicators</a> NuGet package. The library offers financial market technical analysis indicators with a focus on accuracy, performance, and ergonomics for financial analytics.

- Multi-targets `net10.0`, `net9.0`, and `net8.0` with analyzers enabled for strict code quality.
- Active development expands streaming indicator support—consult open specs before modifying stateful pipelines.
- Documentation at <a href="https://dotnet.stockindicators.dev">dotnet.stockindicators.dev</a> is sourced from the `docs/` content in this repository.

## AI-first development model

**This repository is optimized for AI agent contributions, not human coding.**

Guidance follows industry standards for AI configuration:

- **Imperative, directive voice**: "Run the command" not "You should run"
- **Present tense**: Avoid historical context; focus on current directives
- **Autonomous decision-making**: Agents infer, proceed, and document assumptions
- **Outcome-focused**: Define "what" (results) and "why" (rationale), not "how" (micro-steps)
- **No approval loops**: Agents complete work (code + tests + docs) before yielding to humans
- **Succinctness over verbosity**: Prefer compact, scannable directives; eliminate filler words and redundant explanations

**AI agents contribute by:**

1. Performing the vast majority of actual coding work
2. Making educated guesses and documenting assumptions
3. Following standards defined in guidance files
4. Proceeding autonomously without human approval loops
5. Generating changes, tests, and documentation in one pass
6. Executing quality gates (build, test, lint) before yielding

**Humans contribute by:**

1. Setting strategic direction (what capabilities to build)
2. Making architectural decisions and defining quality standards
3. Reviewing agent-generated changes for alignment with project philosophy
4. Correcting agent assumptions when they guess incorrectly
5. Providing context agents cannot infer from repository artifacts
6. Validating outcomes, not micromanaging implementation details

**Human coding is an anti-pattern** in this workflow. If you are a human developer who wants to write code, you can—but recognize that this repository's instructions are optimized for AI agent consumption, not human procedural guidance.

**For AI agents:**

- Make reasonable inferences and proceed autonomously
- Generate complete implementations (code + tests + docs) in one turn
- Execute quality gates (build, test, lint) before yielding to humans
- Document assumptions when making educated guesses
- Follow established patterns and standards without seeking approval
- Handle edge cases using documented principles and constitutional rules

## Guiding principles

This library follows six core principles that balance usability, performance, precision, and security: **Mathematical Precision** (non-negotiable), **Performance First** (critical), **Comprehensive Validation**, **Test-Driven Quality**, **Documentation Excellence**, and **Scope & Stewardship**.

See [PRINCIPLES.md](docs/PRINCIPLES.md) for constitutional philosophy and rationale. This file (AGENTS.md) provides operational implementation guidance.

## Repository layout

```text
(root)
├── src/                   # Library source code
│    ├── _common/          # Shared utilities, base classes, and common types
│    ├── a-d/              # Indicators A-D (alphabetical organization)
│    ├── e-k/              # Indicators E-K
│    ├── m-r/              # Indicators M-R
│    ├── s-z/              # Indicators S-Z
│    └── Indicators.csproj # Main project file
├── tests/                 # Unit, integration, performance, and simulation suites
└── Stock.Indicators.sln   # Primary solution for src + tests
.
├── docs/                  # Public documentation site (VitePress)
└── .specify/              # Spec Kit configuration and active specifications
```

## Build and verification

See the code-completion skill (.github/skills/code-completion/SKILL.md) for complete quality gates, linting commands, build procedures, and testing workflows.

## Skills for development

This repository uses Agent Skills (.github/skills/) for domain-specific guidance. Skills are automatically loaded when relevant:

| Skill | Description | When to use |
| ----- | ----------- | ----------- |
| indicator-series | Series indicator development - mathematical precision, validation patterns, test coverage | Implementing new Series indicators, validating calculations, structuring tests |
| indicator-buffer | BufferList indicator development - incremental processing, interface selection, buffer management | Implementing BufferList indicators, choosing interfaces, managing state efficiently |
| indicator-stream | StreamHub indicator development - implementation patterns, provider selection, state management | Implementing StreamHub indicators, choosing provider base classes, optimizing real-time processing |
| indicator-catalog | Catalog entry creation and registration | Creating indicator catalog entries for automation |
| performance-testing | Benchmarking with BenchmarkDotNet, regression detection | Adding performance tests, optimizing indicator performance |
| code-completion | Quality gates checklist for completing code work | Before finishing any implementation, bug fix, or refactoring |
| testing-standards | Test naming, FluentAssertions, Series parity | Writing comprehensive tests, debugging test failures |

Skills are defined in .github/skills/ following the Agent Skills specification. Refer to the skills instruction file (.github/instructions/skills.instructions.md) when developing new skills.

## Folder-specific guidance

Domain-specific instruction files are auto-loaded by pattern matching:

- Markdown files: See .github/instructions/markdown.instructions.md for authoring standards
- Documentation site: See .github/instructions/docs.instructions.md for VitePress development
- Source code: See src/AGENTS.md for implementation constraints
- Test suite: See tests/AGENTS.md for test organization
- Skills authoring: See .github/instructions/skills.instructions.md for skill development

## MCP tools guidance

MCP servers configured in .vscode/mcp.json provide research and documentation lookup:

- mslearn/* - C# coding conventions, .NET best practices, performance optimization
- context7/* - NuGet package dependencies and external libraries
- github/web_search - Indicator algorithms, financial calculations, technical analysis standards  
- github/* - CI workflow details, repository changes, pull requests, issues

Use MCP tools for research; use direct file operations for local work.

## Pull request guidelines

PR titles follow Conventional Commits format: `type: Subject` (≤65 characters, subject starts uppercase).

Types: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert, plan.

Examples: `feat: Add RSI indicator`, `fix: Resolve MACD calculation error`, `docs: Update API documentation`

---
Last updated: January 25, 2026
