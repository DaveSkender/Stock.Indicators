---
applyTo: "**/*.md"
description: "Markdown rules + repo bootstrap guidance for linting and formatting"
---

# Markdown formatting rules

- These rules are refined (not replaced) via `markdownlint-cli2`.

## Editorial style rules

- Use present tense and imperative mood (e.g., "Use hyphens for lists").
- Exclude historical context, migrations, upgrade paths, and changelog phrasing.
- Do not reference previous tooling or earlier versions of guidance.
- Avoid past-tense verbs unless unavoidable in neutral code examples (prefer present tense).
- Write each rule as a current directive answering: what do we do now?

## Required formatting

**Headers:**

- ATX-style (`#` not underlines), space after marker (`# Title`)
- Sentence case only (first word + proper nouns capitalized)
- Sequential hierarchy (no h1 → h3 jumps), blank lines before/after

```markdown
<!-- good example of Sentence case -->
## Sentence case header

<!-- bad example of header casing -->
## Title Case Header
```

**Code blocks:**

- Fenced with backticks; always specify a language (for markdown content use ````markdown)
- Inline code spans for: commands, file paths, variable names
- Surround code blocks with a blank line; omit trailing whitespace

**Lists:**

- **Always use hyphens (`-`), never asterisks (`*`)**
- 2-space indentation per nesting level
- Consistent markers throughout a document

```markdown
<!-- good example of nesting level indentation -->
- First-level list item
  - Second level list item

<!-- bad example of nesting level indentation -->
- First-level list item
   - Second level list item

<!-- bad example of wrong bullet style -->
* List item 1
* List item 2
```

**End of file elements:**

Include a `Last updated: <Month Day, Year>` line at the end, preceded by a blank line, then a `---` line, and followed by a single trailing newline.

Rules for Last updated element:

- Use only for specification, instructional, and developer documentation.
  - Do not include on Markdown files used as website source pages (e.g. Jekyll).
  - Do not include on Spec Kit files (`.specify/**`, `.github/prompts/speckit.*`).
- Place it last.
- Use the current date.

Example:

```markdown

---
Last updated: July 4, 2025

```

**Links:**

- Fix broken internal links.
- Use reference-style links for complex documents.
- Prefer relative paths for intra-repo links.

## Allowed HTML and front matter

- Allowed inline HTML elements: `details`, `summary`, `br`, `sub`, `sup`, `kbd`, `abbr`, `a`, `img`.
  - Keep HTML minimal and accessible (alt text required for `img`).
- YAML front matter: include only when conventionally required.
  - Follow documented schema sources.

## User GitHub features (when appropriate)

**Collapsible sections:** `<details><summary>Title</summary>Content</details>`

**Task lists:** `- [x] Done` / `- [ ] Todo`

**Alerts:** `> [!NOTE]`, `> [!TIP]`, `> [!IMPORTANT]`, `> [!WARNING]`, `> [!CAUTION]`

**Tables:**

- Pipe-delimited with header separators
- Right-align numeric columns using `---:` in the header separator (e.g., `| ---: |`)
- Use ISO short date `YYYY-MM-DD` and center align dates where appropriate using `:---:` in header separator (e.g., `| :---: |`)

**Diagrams:** Mermaid code blocks for flowcharts/diagrams

Rules for Mermaid diagrams:

- Use GitHub flavored syntax. For example, you must use quotes like `B["POST /user/facts/{factKey}"]` instead of `B[POST /user/facts/{factKey}]`
- Do not use background fill colors like `style AC fill:#e1f5fe`; if colors are needed for differentiation, use colored element borders like `style AC stroke:#e1f5fe`

## Linting and automation (optional)

- Focus on content quality first; add automation only when it adds consistency value.
- Install with: `npm i -D markdownlint-cli2` and add a lean `.markdownlint-cli2.jsonc`.
- Provide scripts: `lint:md` (list) and `lint:md:fix` (apply safe fixes).
- Recommend VS Code extension: `DavidAnson.vscode-markdownlint` (add to `.vscode/extensions.json`).
- Keep configuration minimal; avoid separate ignore files unless required.

## Additional resources

- Refer to repository documentation standards; if absent, add `.github/instructions/documentation.instructions.md`.
- **VS Code snippets** in `snippets/markdown.code-snippets` provide templates for:

  - Document structure with proper timestamps
  - GitHub Flavored Markdown tables (including right-aligned numeric columns)
  - Task lists, collapsible sections, and alert blocks
  - Mermaid diagrams with GitHub-compatible syntax
  - Proper list formatting with hyphens and correct nesting

  Usage: type a snippet prefix (e.g., `doctemplate`, `table`, `tasks`) and press Tab.

---
Last updated: September 26, 2025
