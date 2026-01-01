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

Per the [agentskills.io specification](https://agentskills.io/specification), skill directories use these optional subdirectories:

```text
.github/skills/
├── adr-author/
│   ├── SKILL.md              # Skill definition (required)
│   ├── scripts/              # Optional: executable code or utilities
│   ├── references/           # Optional: technical documentation
│   └── assets/               # Optional: templates, example files, resources
├── planning/
│   ├── SKILL.md
│   ├── references/           # Prioritization formulas, style guides
│   └── assets/               # Issue templates, plan templates
└── maintain-skills/
    └── SKILL.md
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

## AI-optimization principles

Skills are written for AI agent consumption, not human reading. Follow these principles:

**Content focus:**

- Depict unique requirements for specific code patterns (syntax constraints, required/prohibited patterns, interface implementations)
- Point to reference examples with contextual notes about syntax and sequences
- NEVER teach what's already in code (e.g., "EMA needs X lookback periods")
- Steer agents away from incorrect patterns toward proven/forward-looking approaches

**Language:**

- Imperative current tense ("MUST do X", "NEVER do Y")
- No historical/temporary references (e.g., "PR #1234 will remove...")
- No descriptive framing (e.g., "Use this reference for...", "This section explains...")
- Start sections with actionable content, not introductions

**Structure:**

- Use "Required" and "Prohibited" section headings
- MUST/NEVER constraint language throughout
- Remove checkbox format (agents don't need interactive checklists)
- Consolidate related constraints into single sections
- Include code examples showing preferred syntax only

**Examples and references:**

- Label examples with context (e.g., "Simple single-value", "Complex multi-stage")
- Show WRONG and CORRECT patterns for anti-patterns
- Use backticks for file paths: `src/path/file.cs` (not markdown links from skills)
- Point to specific implementations, not categories

## Skill file structure

### Front matter

All skill files must start with YAML front matter:

```yaml
---
name: example-skill-name
description: Brief description of skill purpose and capabilities
---
```

- `name`: Skill identifier name.
  - Kebab case only (lowercase letters and hyphens only, must not start or end with a hyphen).
  - Max 64 characters.
- `description`: Describes what the skill does and when to use it.
  - One line, non-empty.
  - Max 1024 characters.

### Workflow sections

Structure varies by skill type. Common sections for implementation pattern skills:

1. **Title** (H1 heading, no descriptive introduction)
2. **Required implementation** or **Required patterns** (MUST constraints)
3. **Prohibited patterns** (NEVER/❌ anti-patterns with consequences)
4. **Testing constraints** (required test patterns, base classes)
5. **Examples** (reference implementations with context labels)
6. **Performance targets** or **Quality standards** (optimization goals)

For workflow skills (e.g., quality gates, ADR authoring):

1. **Title** (H1 heading)
2. **Preparation** or **Prerequisites** (setup requirements)
3. **Workflow** or **Validation sequence** (ordered steps)
4. **Required gates** or **Completion criteria** (MUST pass checklist)
5. **Recovery** (failure handling)

For maintenance/automation skills:

1. **Self-healing** (Step 0 verification)
2. **Workflow** (execution steps)
3. **About maintenance** (source of truth, triggers)

## About maintenance of this file

- Official documentation: <https://example.com/docs>
- Gold copy: #githubRepo `org/repo` file `.github/skills/example-skill/SKILL.md`
- Maintenance: Use `org` MCP server to update from org-level repo
- Customization: Guidance on whether downstream repos can modify

---
Last updated: YYYY-MM-DD

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

Skills use `#file:` and `#tool:` syntax for references. See <markdown.instructions.md> for complete syntax rules and guidelines.

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

- Agent Skills specification: <https://agentskills.io/specification>
- Example skills: <https://github.com/anthropics/skills>
- Agent Skills documentation: <https://agentskills.io>

---
Last updated: December 31, 2025
