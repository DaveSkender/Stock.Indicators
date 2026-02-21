# Agent skills

This directory contains agent skill definitions (SKILL.md) for all domain-specific workflows available to AI agents across the organization.

> AGENTS: before editing files in this directory tree, read this entire file before loading additional references.

## Primary directive

Ensure developers and AI agents can correctly execute complex, domain-specific tasks on their first attempt by loading the appropriate skill at the right time — reducing human clarification rounds and rework.

> AGENTS: If you are only referencing a skill, do not continue reading content in this file; instead, read the entire referenced SKILL.md file before loading its own references.

## Secondary directives

1. Skills contain no repository-specific content (org names, URLs, agent names, file paths, schedules) — ensuring every skill works correctly when installed in any repository (not as important as primary)
2. Always load the relevant SKILL.md before editing skill content — preventing blind edits that break validated workflows (not as important as 1)
3. Skills maintain progressive disclosure targets (< 30K chars) — ensuring skills load efficiently without bloating agent context windows (not as important as 2)

## Directory structure

Standard skill folder taxonomy

```plaintext
.agents/skills/
├── skill-name/
│   ├── SKILL.md      # Required: primary entrypoint loaded via #skill:skill-name
│   ├── assets/       # Optional: copyable templates (e.g., *.template.md)
│   ├── references/   # Optional: progressive disclosure subordinate docs
│   └── scripts/      # Optional: executable files (e.g., install.sh)
├── AGENTS.md         # Authoring conventions and guardrails (this file)
└── ...
```

## Commands

```bash
npx skills list              # List installed skills
npx skills find <query>      # Search skills.sh public registry
npx skills check             # Check for available skill updates (from source)
npx skills update            # Update all skills (from sources), use <name> for one
npx skills init .agents/skills/<name>  # Scaffold a new skill directory and SKILL.md file
npx skills remove <name>     # Remove (delete) a skill
pnpm run lint:skills         # Validate all skills against the official spec

# Install specific skill for specific agent from a source repository
npx skills add <owner/repo> --skill <name> --agent <agent> --yes
```

When adding skills from a source repository, use

- the `--agent universal` for the `<agent>` target when adding new skills
- the `--copy` modifier when adding skills from a private source repository

Use `npx skills --help` for CLI syntax definitions and its [npm registry page](https://www.npmjs.com/package/skills) for additional parameter options and values.

## Boundaries

✅ Use the [agentskills.io specification](https://agentskills.io/specification.md) as the canonical reference guide when creating or editing content inside any skill subdirectory

✅ Always load the relevant SKILL.md before editing content inside any skill subdirectory

✅ Always use `npx skills init .agents/skills/<name>` to scaffold new skills — do not create SKILL.md files manually

⚠️ Ask before deleting any skill directory — verify no agents, instructions, or AGENTS.md
   entries reference it before removal

🚫 Never manually edit community-sourced skills

🚫 Never let skills reference other workspace files outside of their own directory.  Skills must be independent and isolated.

🚫 Never duplicate skill index tables in other files — AGENTS.md at the root is the single
   source of truth for the skills catalog
