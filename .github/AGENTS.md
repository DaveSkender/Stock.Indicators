# GitHub configuration and agentics primitives

This directory contains GitHub Copilot configuration files and GitHub Actions workflows.

> AGENTS: read this entire file before proceeding to edit files in this folder.

## Primary directive

Ensure developers and AI agents across all organization repositories receive consistent, spec-compliant guidance primitives that enable autonomous, correct operation without human intervention.

## Secondary directives

1. All primitive files (agents, instructions, skills) contain no repository-specific content — org names, hardcoded paths, agent names, and schedules belong in AGENTS.md only, keeping primitives portable and reusable across repositories (not as important as primary)
2. Select the correct primitive type for each task (agent vs. instruction vs. skill vs. prompt) — preventing misfiled content that degrades guidance quality (not as important as #1)
3. Maintain hierarchy compliance in all files — no upward references from skills to instructions or AGENTS.md, ensuring discovery chains function correctly (not as important as #2)

## Directory structure

```plaintext
.github/
├── instructions/    # File-pattern routing instruction files
├── workflows/       # GitHub Actions workflows
│   └── *.yml        # CI/CD pipeline definitions
├── ISSUE_TEMPLATE/  # GitHub issue templates
└── AGENTS.md        # Authoring conventions and guardrails (this file)
```

Agent skills live in `/.agents/skills/`, not in `.github/`.

## Skills

- Execute #skill:agent-customization before authoring instruction files

## Boundaries

✅ Always load #skill:agent-customization before editing any file in `instructions/`

⚠️ Ask before deleting any instruction file — verify no `applyTo` pattern depends on it

🚫 Never create instruction files for cross-cutting concerns or frameworks — use skills instead

🚫 Never place skill files here — skills belong in `/.agents/skills/`

🚫 Never reference AGENTS.md or instruction files using #file: tokens from inside skill files
