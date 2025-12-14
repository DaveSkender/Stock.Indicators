---
applyTo: "**/*.md,.markdownlint-cli2.jsonc"
description: Markdown formatting guide
---

# Markdown authoring rules

Keep Markdown contributions consistent with GitHub Flavored Markdown (GFM) and the VS Code Markdown language features documented at <https://github.github.com/gfm/> and <https://code.visualstudio.com/docs/languages/markdown> while aligning with repository automation and preview tooling.

> [!CRITICAL]
> **Context loading warning:** In some AI agent environments (GitHub Copilot in VS Code), `#file:` references in auto-loaded files automatically expand their targets into the context window. This can cause exponential context bloat and degrade agent performance. **Entry point files** like `AGENTS.md`, `copilot-instructions.md`, and root instruction files should **NEVER contain `#file:` references** to other instruction or context files. Use plain-text file path mentions instead and let agents fetch files on-demand.

## Baseline workflow

- Run `npx markdownlint-cli2 --no-globs {glob} --fix` in the terminal, fix any items not auto-fixed, followed by `npx markdownlint-cli2 --no-globs {glob}` to verify there are no remaining linting issues, before opening a pull request.
  - **Important:** Always use `--no-globs` when specifying explicit file paths to prevent the tool from expanding globs defined in `.markdownlint-cli2.jsonc`.
- For continuous linting during large edits, run with `--watch` to surface issues immediately.
- Preview with VS Code's built-in Markdown preview (`Ctrl+Shift+V`).
- Never bypass lint warnings; resolve or bracket narrow suppressions with `<!-- markdownlint-disable -->`.

## Formatting requirements

### Editorial style

- Use present tense and imperative mood.
- Exclude historical context and migration details.
- Keep each rule as a current directive.
- Headings and bold labels follow sentence case: first word + proper nouns only.

### Content reuse and separation of concerns

**Do not repeat yourself (DRY):**

- Maintain a single source of truth per concept.
- Reference existing documents instead of duplicating content.
- Consolidate overlapping content when possible.

**Separation of concerns:**

- Give each document a single purpose.
- Keep conceptual, procedural, and configuration guidance distinct.
- Separate different target audiences clearly.

**Acceptable limited duplication:**

- Short orienting summaries (2–3 sentences).
- Critical inline warnings.
- Code examples for distinct use cases.
- Cross-references for related but distinct documents.

### Headers and structure

- Use ATX (`#`) headers.
- Sentence case only.
- Sequential hierarchy with blank lines around headers.

### Lists

- Always use hyphens (`-`) for bullets.
- Indent nested lists with two spaces.
- Avoid ordered lists unless sequence matters.

```markdown
<!-- good -->
- First item
  - Nested item

<!-- bad -->
* Wrong bullet
```

### Code blocks

- Use fenced code blocks with a language identifier (use `plaintext` if needed).
- Include blank lines around fences.
- Avoid inline comments in fences.
- Increase fence length when nesting (outer fences need more backticks than inner).

Example of hierarchical fencing:

`````markdown
## Header in markdown example

Other text **formatted** with markdown syntax. Inner code block:

```csharp
int foo = 25;
```

> [!IMPORTANT]
> Outer fencing must have more backticks than inner ones for proper termination.

`````

---

## File and context references

Choose the appropriate referencing style based on whether the file content is needed for the current task.

### When to use `#file:` context tokens

Use `#file:` when the agent **must read the file content** to complete the task:

- Instruction files that define coding standards for the current work
- Templates or schemas the agent must follow
- Configuration files the agent needs to modify or validate against
- Context files containing data required for the task

```markdown
Follow conventions from #file:../copilot-instructions.md
Apply the template in #file:adr-template.md
```

**Syntax rules for `#file:` and `#folder:` tokens:**

- Tokens are context variables, not clickable links.
- Do not wrap in backticks.
- Do not place punctuation immediately after the token.
- Paths are relative to the current file.

Correct: `See #file:markdown.instructions.md for details`

Incorrect: `See #file:markdown.instructions.md.` (trailing punctuation breaks resolution)

### When to use plain-text mentions

Use plain-text mentions when referencing files **for awareness only** (agent decides if content is needed):

- Pointing users or agents to related documentation
- Mentioning files that exist but aren't required for the current task
- Referencing files in informational lists or navigation sections

```markdown
Refer to the AGENTS.md file for project context.
See the contributing guide in docs/contributing.md for details.
```

### Avoiding context window bloat

Root entry points (AGENTS.md, copilot-instructions.md) are auto-loaded in many contexts. To prevent cascading file loads:

- **CRITICAL: Entry point files must NOT use `#file:` references.** Files like `AGENTS.md`, `copilot-instructions.md`, and root-level instruction files are auto-loaded and will cascade their `#file:` references into context, causing bloat. Use plain-text file path mentions instead.
- **Scoped instruction files may use `#file:` selectively.** Files in `.github/instructions/` with `applyTo` patterns are auto-attached only in their specific domains and can safely use `#file:` for on-demand fetching.
- **Agent files should use targeted `#file:` references.** Agent files reference instruction files they need; this is intentional and domain-appropriate.
- **Minimize cascading hierarchies.** Avoid chains like: AGENTS.md → instruction file → context file → another instruction file.
- **Prefer plain-text mentions in navigational sections.** Let agents decide what to fetch: `See the markdown authoring guidelines in .github/instructions/markdown.instructions.md`
- **Never use `file:` URI scheme** (e.g., `file:///path/to/doc.md`). These always force auto-loading.
- **Use standard Markdown links for URLs only**, not for local workspace files.

### Tool references (`#tool:`)

Use `#tool:name` format for tool references. Do not wrap in backticks.

```markdown
Use #tool:search for locating information
```

### Agent references (`@AgentName`)

Use `@AgentName` syntax when referencing custom agents or subagents:

- **In documentation**: Wrap in backticks for readability (e.g., `@Planner`, `@DotNetDeveloper`).
- **In agent files**: Use plain `@AgentName` without backticks for invocation or handoff contexts.
- **Handoffs**: Reference target agents in YAML front matter `handoffs` section.

```markdown
<!-- In documentation or prose -->
Use `@Planner` to create GitHub Issues hierarchies.
Delegate backend work to `@DotNetDeveloper` or `@NestJsDeveloper`.

<!-- In agent file handoff instructions -->
@Researcher investigate the authentication options
```

---

## File size and organization

- Keep Markdown files under ~500 lines when possible.
- Split files >800 lines unless they are cohesive.
- Prefer refactoring for succinctness before splitting.
- Split by concepts, workflows, or functional areas.
- Use index files and consistent naming in directories.

**When to split files:**

- Each major section (h2) could stand alone as a separate topic.
- Document serves multiple distinct audiences or use cases.
- Navigation becomes difficult due to excessive scrolling.

Example file tree:

```text
my-repo/
├── .github/
│   ├── copilot-instructions.md       # Meta-instructions (optional for downstream repos)
│   ├── instructions/
│   ├── prompts/
│   └── workflows/
├── docs/
├── src/                              # Source code
└── README.md                         # Human-oriented overview
```

### End of file elements

- End with:

  ```markdown

  ---
  Last updated: <Month Day, Year>

  ```

- Do not include change logs here.

### HTML elements

Avoid inline HTML unless no Markdown equivalent exists. Allowed elements are defined in `.markdownlint-cli2.jsonc`. Use sparingly and ensure accessibility (e.g., alt text for images).

### GitHub features

- Use `<details>` for collapsible sections (one of few valid HTML use cases).
- Use GitHub alert blocks (`> [!NOTE]`, `> [!TIP]`, `> [!IMPORTANT]`, `> [!WARNING]`, `> [!CAUTION]`) sparingly.
- Use pipe-delimited tables with header separators.
  - Right-align numeric columns: `| ---: |`
  - Center-align dates: `| :---: |` with ISO format `YYYY-MM-DD`

## Mermaid diagrams

- Use ` ```mermaid ` fences with a brief preceding description.
- Quote node labels (e.g., `A["Start"]` not `A[Start]`).
- Prefer stroke styling over filled colors for better theme compatibility.
- Validate diagrams render legibly in both dark and light themes.
- Validate diagrams before committing.

## Math and alerts

- Present LaTeX in fenced blocks (`plaintext` or `math`).
- Use alert blocks sparingly for execution-critical details.

## Tooling checklist

- Keep `.markdownlint-cli2.jsonc`, `.editorconfig`, and `.vscode/settings.json` aligned.
- Use the VS Code markdownlint extension.
- Document exceptions in `.markdownlint-cli2.jsonc`.

## About maintenance of this file

- Align with official GitHub markdown documentation: <https://github.github.com/gfm/>

---
Last updated: December 7, 2025
