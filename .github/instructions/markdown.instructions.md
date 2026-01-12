---
applyTo: "**/*.md,.markdownlint*.{yaml,yml,json,jsonc}"
description: Markdown formatting guide
---

# Markdown authoring rules

Agents: follow these conventions when creating or modifying Markdown files. All rules align with GitHub Flavored Markdown (GFM) and VS Code Markdown language features while ensuring consistent automation and linting.

> [!IMPORTANT]
> **CRITICAL: context loading warning:** Entry point files (`AGENTS.md`, root instruction files) auto-load in many contexts. These files must NOT use `#file:` references as they cascade and cause exponential context bloat. Use plain-text file path mentions instead.

> [!NOTE]
> **First time setup:** If configuring a new repository, see the Setup section at the end of this file for complete configuration requirements.

## Baseline workflow

**Agent execution sequence for Markdown tasks:**

1. Run `npx markdownlint-cli2 --no-globs {filepath} --fix` to auto-fix issues (e.g., `npx markdownlint-cli2 --no-globs path/to/file.md --fix`)
   - **Critical:** Always use `--no-globs` with explicit file paths to prevent unintended glob expansion
2. Manually fix remaining issues that cannot be auto-corrected
3. Run `npx markdownlint-cli2 --no-globs {filepath}` to verify zero errors (e.g., `npx markdownlint-cli2 --no-globs path/to/file.md`)
4. Never bypass lint warnings; resolve or add narrow suppressions with `<!-- markdownlint-disable MD### -->...<!-- markdownlint-enable MD### -->`

**Optional tools:**

- Add `--watch` flag during multi-file edits for continuous feedback
- Preview with VS Code Markdown preview (`Ctrl+Shift+V`) when uncertain about rendering

## Common errors and fixes

**Agents: Detect and fix these patterns automatically:**

| Error pattern | Fix |
| ------------- | --- |
| **Title case in headers** | Convert to sentence case: "How To Use" → "How to use" |
| **Title case in bold labels** | Convert to sentence case: "**Next Steps:**" → "**Next steps:**" |
| Asterisk bullets (`*`, `+`) | Replace with hyphens (`-`) |
| Setext headers (`===`, `---`) | Convert to ATX (`#`, `##`) |
| Missing blank lines around headers | Add blank line before and after |
| Missing blank lines around code blocks | Add blank line before and after fence |
| Backticks in `#file:` references | Remove backticks: `#file:path` not `` `#file:path` `` |
| `#file:` in entry point files | Replace with plain-text path mention |
| Ordered lists for non-sequential items | Convert to unordered hyphen lists |
| Trailing punctuation after `#file:` | Remove punctuation or add space |
| Missing language identifier in fenced blocks | Add language (or `plaintext`) |
| Nested code blocks with equal fence length | Increase outer fence length |

## Formatting requirements

### Editorial style

- **Voice:** Present tense, imperative mood ("Run the command" not "You should run")
- **Headers:** Sentence case only (first word + proper nouns capitalized)
- **Focus:** Current directives only; exclude historical context and migration details
- **Tone:** Direct and actionable for autonomous agent execution

### Content reuse and separation of concerns

**Single source of truth:**

- Reference existing documents instead of duplicating content
- Consolidate overlapping content when possible
- Keep each document focused on one purpose

**Acceptable limited duplication:**

- Short orienting summaries (2–3 sentences)
- Critical inline warnings requiring immediate visibility
- Code examples demonstrating distinct use cases
- Cross-references providing essential context

### Headers

- **Style:** ATX only (`#`, `##`, `###`); never Setext (`===`, `---`)
- **Capitalization:** Sentence case only
  - Capitalize: First word + proper nouns (GitHub, TypeScript, MADR)
  - Lowercase: Articles (a, an, the), prepositions (of, to, for), conjunctions (and, but, or)
- **Spacing:** Blank line before and after every header
- **Hierarchy:** Sequential (no skipping levels: `#` → `##` → `###`)

**Sentence case examples:**

```markdown
<!-- Correct -->
## Agent authoring guidelines
## How to use context files
## Configuration for TypeScript projects

<!-- Incorrect -->
## Agent Authoring Guidelines
## How To Use Context Files
## Configuration For TypeScript Projects
```

### Lists

- **Bullets:** Always use hyphens (`-`); never asterisks (`*`) or plus signs (`+`)
- **Nesting:** Indent nested lists with exactly two spaces
- **Ordering:** Use ordered lists (1., 2., 3.) only when sequence matters
- **Bold labels:** Use sentence case for bold labels that start list items

```markdown
<!-- Correct -->
- **Installation steps:** Run the following commands
- **Next steps:** Configure the settings
  - Nested item

<!-- Incorrect -->
- **Installation Steps:** Run the following commands
- **Next Steps:** Configure the settings
* Wrong bullet character
+ Also wrong
```

### Code blocks

- **Fencing:** Always use fenced blocks (` ``` `); never indented code blocks
- **Language identifier:** Required on all fences; use `plaintext` when language is unknown
- **Spacing:** Blank line before and after fences
- **Nesting:** Outer fences must have more backticks than inner fences

**Nested fence example:**

`````markdown
## Outer document

Text content. Inner code block:

```csharp
int foo = 25;
```

More content.
`````

**Fence length rule:** 5 backticks contain 3-backtick blocks, 7 backticks contain 5-backtick blocks, etc.

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
Follow conventions from #file:../../AGENTS.md
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

Root entry points (AGENTS.md) are auto-loaded in many contexts. To prevent cascading file loads:

- **CRITICAL: Entry point files must NOT use `#file:` references.** Files like `AGENTS.md` and root-level instruction files are auto-loaded and will cascade their `#file:` references into context, causing bloat. Use plain-text file path mentions instead.
- **Scoped instruction files may use `#file:` selectively.** Files in `.github/instructions/` with `applyTo` patterns are auto-attached only in their specific domains and can safely use `#file:` for on-demand fetching.
- **Agent files should use targeted `#file:` references.** Agent files reference instruction files they need; this is intentional and domain-appropriate.
- **Minimize cascading hierarchies.** Avoid chains like: AGENTS.md → instruction file → context file → another instruction file.
- **Prefer plain-text mentions in navigational sections.** Let agents decide what to fetch: `See the markdown authoring guidelines in .github/instructions/markdown.instructions.md`
- **Never use `file:` URI scheme** (e.g., `file:///path/to/doc.md`). These always force auto-loading.
- **Use standard Markdown links for URLs only**, not for local workspace files.

### Tool references (`#tool:`)

Use `#tool:name` format for tool references. Do not wrap in backticks.

**When to use `#tool:` syntax:**

The `#tool:` syntax is ONLY applicable for:

- **VS Code Copilot Chat built-in tools**: `search`, `edit`, `read`, `runCommands`, `runTasks`, etc.
- **MCP server tools**: Tools from MCP servers using `server/tool` format

**Do NOT use `#tool:` syntax for:**

- Generic tool categories (e.g., "Build system tools", "Linting tools")
- Executable commands (e.g., `npm`, `dotnet`, `eslint`)
- General software or frameworks

**Examples:**

```markdown
Use #tool:search for locating information
```

**MCP server tools use `server/tool` format:**

```markdown
#tool:mslearn/microsoft_docs_search
#tool:github/pull_request_read
#tool:github/web_search
```

**Common error - Do NOT use internal MCP function names:**

```markdown
❌ BAD: mcp_mslearn_microsoft_docs_search
❌ BAD: `mcp_github_pull_request_read`
❌ BAD: fetch_webpage

✅ GOOD: #tool:mslearn/microsoft_docs_search
✅ GOOD: #tool:github/pull_request_read
✅ GOOD: #tool:fetch
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
│   ├── instructions/
│   ├── prompts/
│   └── workflows/
├── docs/
│   └── AGENTS.md                     # Subfolder agent context (optional)
├── src/                              # Source code
│   └── AGENTS.md                     # Subfolder agent context (optional)
├── AGENTS.md                         # Root agent instructions
└── README.md                         # Human-oriented overview
```

Prefer a central AGENTS.md file for AI agent context. See [agents.md specification](https://agents.md/) for cross-agent compatibility.

### End of file elements

- End with:

  ```markdown

  ---
  Last updated: <Month Day, Year>

  ```

- Do not include change logs here.

### HTML elements

**Agents: Avoid HTML unless no Markdown equivalent exists.**

Allowed elements (defined in `.markdownlint-cli2.jsonc`):

- `<details>`, `<summary>` (collapsible sections)
- `<br>` (line breaks in tables)
- `<sub>`, `<sup>`, `<kbd>`, `<abbr>` (semantic formatting)
- `<a>`, `<img>` (links and images when Markdown syntax insufficient)

**Accessibility requirements:**

- All `<img>` must have `alt` text
- All `<a>` must have descriptive link text

### GitHub features

**Alert blocks:** Use sparingly for critical execution warnings

- `> [!NOTE]` — Informational highlights
- `> [!TIP]` — Helpful suggestions
- `> [!IMPORTANT]` — Critical information
- `> [!WARNING]` — Caution required
- `> [!CAUTION]` — Danger or risk

> Exception: for docs VitePress website, use native `:::` codeblocks for alerts.

**Tables:** Use pipe-delimited format with alignment:

```markdown
| Column | Number | Date |
|--------|-------:|:----:|
| Text   | 123    | 2025-01-15 |
```

- Left-align: `| --- |` (default)
- Right-align: `| ---: |` (numbers)
- Center-align: `| :---: |` (dates in ISO format `YYYY-MM-DD`)

## Mermaid diagrams

- **Fence:** Use ` ```mermaid ` with language identifier
- **Description:** Include brief plain-text description before diagram
- **Node labels:** Always quote labels within square brackets for Mermaid syntax
- **Styling:** Use stroke styling, not fill colors (better theme compatibility)
- **Validation:** Test rendering in both dark and light themes before committing

## Validation and quality checks

**Agents: Execute these checks before committing Markdown changes:**

1. **Linting:** Zero errors from `npx markdownlint-cli2 --no-globs {filepath}`
2. **Links:** All internal links resolve to existing files
3. **Code fences:** All fences have language identifiers
4. **Headers:** Sentence case only, ATX style, sequential hierarchy
5. **Bold labels:** Sentence case for all bold labels at start of list items
6. **Lists:** Hyphen bullets only, proper indentation
7. **End of file:** Blank line, separator, "Last updated" footer
8. **Diagrams:** Mermaid diagrams render in preview
9. **HTML:** Only allowed elements, all images have alt text

## Setup

**Agents: Complete these one-time configuration steps when setting up a new repository or when markdown tooling is missing.**

### Step 1: Install VS Code extensions

Add to `.vscode/extensions.json`:

```jsonc
{
  "recommendations": [
    "DavidAnson.vscode-markdownlint",  // REQUIRED: Markdown linting
    "bierner.github-markdown-preview",  // Enhanced preview with GitHub styling
    "EditorConfig.EditorConfig"         // Cross-editor consistency
  ]
}
```

**Extension purposes:**

- `vscode-markdownlint`: Real-time linting, auto-fix on save, integrates with markdownlint-cli2
- `github-markdown-preview`: Preview rendering matches GitHub's Markdown processor
- `EditorConfig`: Ensures consistent indentation/line endings across editors

### Step 2: Configure VS Code settings

Add to `.vscode/settings.json`:

```jsonc
{
  "[markdown]": {
    "editor.defaultFormatter": "DavidAnson.vscode-markdownlint",
    "editor.formatOnSave": true,
    "editor.codeActionsOnSave": {
      "source.fixAll.markdownlint": "explicit"
    }
  },
  "files.associations": {
    "*.md": "markdown"
  }
}
```

**Settings effects:**

- `defaultFormatter`: Uses markdownlint for formatting (aligns with CLI tool)
- `formatOnSave`: Auto-formats on save (applies fixable rules automatically)
- `codeActionsOnSave`: Runs markdownlint fixes explicitly on save
- `files.associations`: Ensures `.md` files are recognized as markdown

### Step 3: Create linting configuration

Create `.markdownlint-cli2.jsonc` with baseline settings.
Use this example as a starting point if no configuration exists.

```jsonc
{
  "globs": ["**/*.md"],
  "gitignore": true,  // REQUIRED: Respect root-level .gitignore patterns
  "ignores": [
    // Customize based on project structure
    "node_modules/**",
    "**/node_modules/**",
    "packages/*/dist/**",
    "packages/*/lib/**"
  ],
  "config": {
    "default": true,
    "MD003": { "style": "atx" },
    "MD004": { "style": "dash" },
    "MD007": { "indent": 2 },
    "MD013": false,
    "MD024": { "siblings_only": true },
    "MD028": false,
    "MD033": {
      "allowed_elements": ["details", "summary", "br", "sub", "sup", "kbd", "abbr", "a", "img", "workflow"]
    },
    "MD046": { "style": "fenced" },
    "MD048": { "style": "backtick" }
  }
}
```

**Configuration requirements:**

1. **MUST have:** `"gitignore": true` (prevents linting ignored files)
2. **Customize:** `ignores` array based on project build outputs and structure
3. **Remove:** Competing configuration files (e.g., `markdownlint.json`, `.markdownlintrc`) if present
4. **Document exceptions:** Add rule overrides to `config` section, never use inline disables

**Optional rule overrides:**

- **MD060 (table-column-style)**: Enforces consistent table column formatting (aligned/compact/tight). Disabled by default as most projects don't require strict table formatting. Enable only if your project requires consistent table column styles.
- **MD041 (top-level header)**: May require RegExp customization when front-matter replaces it.
  Example: `"MD041": { "front_matter_title": "^\\s*(?:[Tt]itle|[Dd]escription|[Nn]ame)\\s*" },`

### Step 4: Add VS Code tasks (optional)

Add markdown linting tasks to `.vscode/tasks.json` for integrated workflow:

```jsonc
{
  "$schema": "vscode://schemas/tasks",
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Lint: Markdown",
      "detail": "Verify formatting for all markdown files",
      "type": "shell",
      "command": "npx markdownlint-cli2",
      "group": "test",
      "problemMatcher": "$markdownlint",
      "presentation": {
        "revealProblems": "onProblem",
        "clear": true
      }
    },
    {
      "label": "Lint: Markdown (auto-fix)",
      "detail": "Auto-fix formatting for all markdown files",
      "type": "shell",
      "command": "npx markdownlint-cli2 --fix",
      "problemMatcher": "$markdownlint",
      "presentation": {
        "revealProblems": "onProblem",
        "clear": true
      }
    }
  ]
}
```

**Task benefits:**

- Run linting from VS Code Tasks menu (`Ctrl+Shift+P` → Tasks: Run Task)
- `$markdownlint` problem matcher integrates errors into Problems panel
- `revealProblems: "onProblem"` shows output only when errors exist
- Integrates with CI/CD workflows and package.json scripts

### Step 5: Verify configuration alignment

**Agents: Confirm consistency across all configuration files:**

- `.markdownlint-cli2.jsonc` — Linting rules (see Step 3)
- `.vscode/settings.json` — Editor settings match linting rules (see Step 2)
- `.vscode/extensions.json` — Recommended extensions installed (see Step 1)
- `.vscode/tasks.json` — Task definitions for linting workflow (see Step 4)
- `.editorconfig` — Tab/space settings align (2 spaces for markdown)

### Step 6: Test the setup

```bash
# Install npm/pnpm dependencies (if needed)
npm install -g markdownlint-cli2

# Test linting on all markdown files
npx markdownlint-cli2

# Test auto-fix
npx markdownlint-cli2 --fix

# Test specific file
npx markdownlint-cli2 --no-globs "README.md"
```

**Expected results:**

- Zero linting errors on existing markdown files (or known violations)
- Auto-fix resolves formatting issues (bullets, headers, spacing)
- VS Code shows inline warnings for markdown violations
- Format-on-save applies fixes automatically

---
Last updated: December 31, 2025
