---
applyTo: "**/.github/skills/**/SKILL.md"
description: Skill file authoring standards following agentskills.io specification
---

# GitHub Copilot skills authoring guidelines

This file defines standards for authoring `SKILL.md` files in this organization following the Agent Skills specification from agentskills.io (maintained by Anthropic).

## What are skills?

Skills are folders containing `SKILL.md` files with instructions, scripts, and resources that agents can load dynamically to perform better at specific tasks.

Per the Agent Skills specification:

- Skills are structured workflow definitions in `.github/skills/<skill-name>/` directories
- Each skill directory contains a `SKILL.md` file (uppercase) as the primary skill definition
- Skills can include supporting resources (templates, scripts, examples) in the same directory
- Define multi-step processes for complex tasks
- Provide context, tools, and boundaries for execution
- Enable autonomous agent behavior with validation gates
- Support self-healing maintenance workflows
- Integrate with GitHub Copilot Chat and Coding Agent

## Skill directory structure

Per the [agentskills.io specification](https://agentskills.io/specification), skill directories use these optional subdirectories (example):

```text
.github/skills/
├── example-skill/
│   ├── SKILL.md              # Skill definition (required)
│   ├── scripts/              # Optional: executable code or utilities
│   ├── references/           # Optional: technical documentation
│   └── assets/               # Optional: templates, example files, resources
└── other-skill/
    ├── SKILL.md
    └── ...
```

**Requirements:**

- Skill directory name matches the skill `name` in front matter (including casing)
- Primary skill file is named `SKILL.md` (uppercase)
- Use `scripts/` for executable code or utilities
- Use `references/` for technical documentation
- Use `assets/` for templates, example files, and resources
- Keep related resources together in the skill directory

## When to use skills

Use skills for:

- **Code pattern implementation**: Depict unique requirements for specific code patterns (syntax constraints, required/prohibited patterns, interface requirements, mandatory tests)
- **Multi-step workflows**: Complex processes requiring sequential steps (ADR creation, maintenance)
- **Repeatable processes**: Standardized workflows executed regularly (biweekly maintenance)
- **Quality validation**: Workflows with built-in validation and error correction
- **Self-healing systems**: Processes that check and update themselves (maintenance skills)
- **Guided execution**: Tasks requiring context setup, execution, and validation phases

Don't use skills for:

- Simple one-step operations (use agents directly)
- Reference documentation (use `.instructions.md` files)
- Quick reference data (use `.context.md` files)
- Interactive user guidance (use `.prompt.md` files)
- Documenting what is already evident in code

## Skill content principles

Skills prevent agents from reinventing incorrect patterns and steer toward proven approaches:

- **Depict unique requirements**: Document syntax constraints, required patterns, prohibited patterns, mandatory tests specific to the code pattern
- **Point to reference examples**: Link to specific working examples with contextual notes about syntax and sequences
- **Avoid restating the obvious**: Do not document what is already clear from reading the code
- **Prevent unwanted patterns**: Explicitly call out patterns we have learned are incorrect or problematic
- **Steer toward proven patterns**: Document patterns that work exceptionally well or align to forward-looking goals
- **Use preferred syntax**: Use organization's preferred code syntax in examples unless there is a specific reason to change course
- **AI-optimized writing**: Write for AI consumption, not human readability—be succinct, direct, imperative
- **Present tense only**: No historical context or temporary situations—only current imperatives

## Skill file structure

### Front matter

All skill files must start with YAML front matter:

```yaml
---
name: example-skill-name
description: Brief description of skill purpose and capabilities
---
```

- `name`: Skill identifier name (must be kebab-case).
  - Kebab case only (lowercase letters and hyphens only, must not start or end with a hyphen).
  - Max 64 characters.
- `description`: Describes what the skill does and when to use it.
  - One line, non-empty.
  - Max 1024 characters.

### Workflow sections

Structure all skills with these sections in order:

1. **Title and introduction** (H1 heading, brief description of purpose)
2. **When to use this skill** (specific scenarios where skill applies)
3. **Required tools** (tools needed: MCP, GitHub CLI, etc.)
4. **Workflow** (numbered sequential steps with substeps)
5. **Completion criteria or quality standards** (requirements for success)
6. **Self-healing (for maintenance skills)** (Step 0 before other steps)
7. **About maintenance** (source of truth, maintenance triggers)

### Example structure

`````markdown
---
name: example-skill
description: Example skill demonstrating Agent Skills specification
---

# Skill Name

Brief description of purpose and capabilities.

## When to use this skill

Use this skill when:

- Specific scenario 1
- Specific scenario 2
- Specific scenario 3

## Required tools

- #tool:example-tool - Description of what this tool does
- #tool:another-tool - Description of what this tool does

## Workflow

### Step 1: Initialize

- Gather context
- Validate prerequisites
- Set up environment

### Step 2: Execute main task

- Perform primary operations
- Track progress
- Handle errors

### Step 3: Validate results

- Check outputs
- Fix issues
- Verify success

## Quality standards

- Standard 1
- Standard 2
- Standard 3

## About maintenance of this file

- Official documentation: <https://example.com/docs>

---
Last updated: YYYY-MM-DD
`````

## Naming conventions

### Skill directory names

- Match the skill `name` in front matter exactly (including casing)
- Always use kebab-case (e.g., `adr-author`, `playwright-planning`, `maintain-skills`)
- Be descriptive and specific

**Examples:**

- `adr-author` - kebab-case
- `playwright-planning` - kebab-case
- `maintain-skills` - kebab-case

### Skill file naming

- Primary skill file: `SKILL.md` (uppercase, per agentskills.io standard)
- Supporting files: descriptive names (e.g., `template.md`, `example-config.json`)
- Scripts: use appropriate extensions (`.ts`, `.sh`, `.py`)

## Agent Skills specification alignment

This organization follows the Agent Skills specification from agentskills.io (maintained by Anthropic).

**Key requirements:**

1. Skills are folders, not just files
2. Primary skill file is named `SKILL.md` (uppercase)
3. Supporting resources live in the same directory
4. Skill directory name matches the `name` in front matter
5. Skills can be shared across multiple agents
6. Skills focus on **how to perform tasks**, not agent identity

**Benefits:**

- Reusable workflows across multiple agents
- Clear separation between skills (workflows) and agents (personas)
- Standard format for skill discovery and loading
- Composable: agents can use multiple skills
- Portable: skills work across different agent systems

## File and tool references

Skills use `#file:` and `#tool:` syntax for references. See [markdown.instructions.md](markdown.instructions.md) for complete syntax rules and guidelines.

## Skill invocation

Skills can be invoked:

- **Directly in Copilot Chat**: "Use the ADR skill to document this decision"
- **Via agents**: Through handoff or direct reference
- **Via GitHub Actions**: Automation workflows
- **Via prompts**: Interactive guided sessions can delegate to skills

## Testing skills

Before finalizing a new skill:

1. **Dry run**: Execute with `--dry-run` flag if supported
2. **Validation**: Run validation steps manually
3. **Edge cases**: Test error conditions and variations
4. **References**: Verify all `#file:` and `#tool:` references resolve
5. **Integration**: Test with related agents and skills

## References

- Agent Skills documentation: <https://agentskills.io>
- Agent Skills specification: <https://agentskills.io/specification>
- Example skills: <https://github.com/anthropics/skills>

---
Last updated: January 1, 2026
