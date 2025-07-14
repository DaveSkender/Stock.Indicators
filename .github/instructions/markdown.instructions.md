---
applyTo: "**/*.md"
description: "Guidelines for writing and formatting Markdown files"
---

# Markdown formatting rules

## Required formatting

**Headers:**

- ATX-style (`#` not underlines), space after marker (`# Title`)
- **Sentence case only** (first word + proper nouns capitalized)
- Sequential hierarchy (no h1→h3 jumps), blank lines before/after

**Code blocks:**

- Language-specified fenced blocks: ````markdown for markdown content
- Inline code spans for: commands, file paths, variable names
- Blank lines surrounding code blocks

**Lists:**

- **Always use hyphens (`-`), never asterisks (`*`)**
- 2-space indentation per nesting level
- Consistent markers throughout document

**Required elements:**

- Alt text for all images
- "Last updated: " date at end (always update when editing)
- Single blank line at file end
- No trailing line spaces

**Links:**

- Fix all broken internal links
- Use reference-style for complex documents

## Linting and standards

- use `npm run lint:md` to identify non-compliant formatting
- linting configuration is in the `.markdownlint.json` file
- we use npm packages `markdownlint` and `markdownlint-cli`

## README.md structure and purpose

**README.md files serve as directory indexes:**

- **Primary purpose**: Navigation hub for markdown files in the current directory
- **When multiple markdown files exist**: README.md contains a table-based list of all markdown files in the folder
- **When it's the only markdown file**: README.md contains the actual content for that directory
- **Hierarchical structure**: Parent README.md files reference child directory README.md files only, not individual files within subdirectories
- **Consistency**: Every directory containing markdown files should have a README.md as the entry point

**README.md table format:**

- Use pipe-delimited tables with headers: `| File | Description |`
- Link to files valid relative paths
- For subdirectories: Link only to their README.md, not individual files within
- Include brief descriptions explaining each file's purpose

**Example structure:**

```text
docs/
├── README.md (indexes system/ and user/ directories)
├── system/
│   ├── README.md (indexes all .md files in system/)
│   ├── architecture.md
│   └── deployment.md
└── user/
    ├── README.md (indexes all .md files in user/)
    └── guide.md
```

## GitHub features (when appropriate)

**Collapsible sections:** `<details><summary>Title</summary>Content</details>`

**Task lists:** `- [x] Done` / `- [ ] Todo`

**Alerts:** `> [!NOTE]`, `> [!WARNING]`, `> [!TIP]`

**Tables:** Pipe-delimited with header separators

**Diagrams:** Mermaid code blocks for flowcharts/diagrams

## Additional resources

See the [Documentation guide](/docs/guides/documentation.guide.md) for additional information on documentation category-specific rules and broad use guidance.

---
Last updated: June 20, 2025
