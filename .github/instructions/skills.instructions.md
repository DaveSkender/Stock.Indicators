---
applyTo: "**/.github/skills/**/SKILL.md"
description: Skill file authoring standards following agentskills.io specification
---

# GitHub Copilot skills authoring guidelines

This file defines standards for authoring SKILL.md files following the Agent Skills specification from agentskills.io (maintained by Anthropic).

## What are skills?

Skills are folders containing SKILL.md files with instructions, scripts, and resources that agents can load dynamically for specialized tasks.

Per the Agent Skills specification:

- Skills are structured workflow definitions in .github/skills/{skill-name}/ directories
- Each skill directory contains a SKILL.md file (uppercase) as the primary skill definition
- Skills can include supporting resources (templates, scripts, examples) in the same directory
- Define multi-step processes for complex tasks
- Enable autonomous agent behavior with validation gates

## Skill directory structure

Per the agentskills.io specification:

```text
.github/skills/
├── example-skill/
│   ├── SKILL.md              # Skill definition (required)
│   ├── scripts/              # Optional: executable code or utilities
│   ├── references/           # Optional: technical documentation
│   └── assets/               # Optional: templates, example files, resources
```

**Requirements:**

- Skill directory name matches the skill name in front matter (including casing)
- Primary skill file is named SKILL.md (uppercase)
- Use scripts/ for executable code or utilities
- Use references/ for technical documentation
- Use assets/ for templates, example files, and resources

## When to use skills

Use skills for:

- Code pattern implementation (unique requirements for specific code patterns)
- Multi-step workflows (complex processes requiring sequential steps)
- Repeatable processes (standardized workflows executed regularly)
- Quality validation (workflows with built-in validation and error correction)
- Self-healing systems (processes that check and update themselves)

Don't use skills for:

- Simple one-step operations (use agents directly)
- Reference documentation (use .instructions.md files)
- Quick reference data (use .context.md files)
- Documenting what is already evident in code

## Skill content principles

- Depict unique requirements (syntax constraints, required patterns, prohibited patterns)
- Point to reference examples (link to specific working examples)
- Avoid restating the obvious (don't document what is clear from code)
- Prevent unwanted patterns (explicitly call out incorrect or problematic patterns)
- Steer toward proven patterns (document what works well)
- AI-optimized writing (succinct, direct, imperative)
- Present tense only (no historical context or temporary situations)

## Skill file structure

### Front matter

All skill files must start with YAML front matter:

```yaml
---
name: example-skill-name
description: Brief description of skill purpose and capabilities
---
```

- name: Skill identifier (kebab-case, max 64 characters)
- description: What skill does and when to use it (one line, max 1024 characters)

### Workflow sections

Structure skills with these sections in order:

1. Title and introduction (H1 heading, brief description)
2. When to use this skill (specific scenarios)
3. Required tools (tools needed)
4. Workflow (numbered sequential steps with substeps)
5. Completion criteria or quality standards (requirements for success)
6. About maintenance (source of truth, maintenance triggers)

## Naming conventions

**Skill directory names:**

- Match the skill name in front matter exactly (including casing)
- Always use kebab-case (e.g., adr-author, playwright-planning, maintain-skills)
- Be descriptive and specific

**Skill file naming:**

- Primary skill file: SKILL.md (uppercase, per agentskills.io standard)
- Supporting files: descriptive names (e.g., template.md, example-config.json)
- Scripts: use appropriate extensions (.ts, .sh, .py)

## Agent Skills specification alignment

This organization follows the Agent Skills specification from agentskills.io (maintained by Anthropic).

**Key requirements:**

1. Skills are folders, not just files
2. Primary skill file is named SKILL.md (uppercase)
3. Supporting resources live in the same directory
4. Skill directory name matches the name in front matter
5. Skills can be shared across multiple agents
6. Skills focus on how to perform tasks, not agent identity

**Benefits:**

- Reusable workflows across multiple agents
- Clear separation between skills (workflows) and agents (personas)
- Standard format for skill discovery and loading
- Composable: agents can use multiple skills
- Portable: skills work across different agent systems

## File and tool references

Skills use #file: and `#tool:` syntax for references. See markdown.instructions.md for complete syntax rules and guidelines.

## Skill invocation

Skills can be invoked:

- Directly in Copilot Chat: "Use the ADR skill to document this decision"
- Via agents: Through handoff or direct reference
- Via GitHub Actions: Automation workflows
- Via prompts: Interactive guided sessions can delegate to skills

## References

- Agent Skills documentation: <https://agentskills.io>
- Agent Skills specification: <https://agentskills.io/specification>
- Example skills: <https://github.com/anthropics/skills>

---
Last updated: January 25, 2026
