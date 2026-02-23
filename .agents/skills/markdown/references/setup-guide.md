# Markdown tooling setup guide

Complete setup guide for markdown authoring tools in a new repository.

## Quick installation

For automated tool installation, use the installation script:

```bash
bash .agents/skills/markdown/scripts/install.sh
```

This installs markdownlint-cli2 globally via pnpm/npm. For manual setup or VS Code integration, follow the steps below.

## Step 1: Install VS Code extensions

Add to `.vscode/extensions.json`:

```jsonc
{
  "recommendations": [
    "DavidAnson.vscode-markdownlint",   // REQUIRED: Markdown linting
    "bierner.github-markdown-preview",  // Enhanced preview with GitHub styling
    "EditorConfig.EditorConfig"         // Cross-editor consistency
  ]
}
```

**Extension purposes:**

- `vscode-markdownlint`: Real-time linting, auto-fix on save, integrates with markdownlint-cli2
- `github-markdown-preview`: Preview rendering matches GitHub's Markdown processor
- `EditorConfig`: Ensures consistent indentation/line endings across editors

## Step 2: Configure VS Code settings

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
- `files.associations`: Ensures `.md` files are recognized as markdown

## Step 3: Create linting configuration

Create `.markdownlint-cli2.jsonc` with baseline settings.
Use this example as a starting point if no configuration exists.

```jsonc
{
  "globs": ["**/*.md"],
  "gitignore": true,
  "ignores": [
    // Customize based on project structure
    "node_modules/**",
    "**/node_modules/**",
    "**/dist/**",
    "**/test-results/**"
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
      "allowed_elements": ["details", "summary", "br", "sub", "sup", "kbd", "abbr", "a", "img"]
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
4. **Document exceptions:** Prefer adding rule overrides to the `config` section. For narrow, targeted cases where restructuring isn't possible, use inline suppressions with `<!-- markdownlint-disable MD### -->...<!-- markdownlint-enable MD### -->`

## Step 4: Add VS Code tasks

Add markdown linting tasks to `.vscode/tasks.json` for integrated workflow. If tasks with these labels already exist, replace them with these definitions.

```jsonc
{
  "$schema": "vscode://schemas/tasks",
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Lint: Markdown (verify)",
      "detail": "Verify formatting for all markdown files",
      "type": "shell",
      "command": "npx markdownlint-cli2",
      "group": "test",
      "problemMatcher": "$markdownlint"
    },
    {
      "label": "Lint: Markdown (auto-fix)",
      "detail": "Auto-fix formatting for all markdown files",
      "type": "shell",
      "command": "npx markdownlint-cli2 --fix",
      "problemMatcher": "$markdownlint"
    },
    {
      "label": "Lint: Markdown (filtered)",
      "detail": "Auto-fix formatting for a markdown file. AGENTS: do not use this option.",
      "type": "shell",
      "command": "npx markdownlint-cli2 --no-globs ${input:markdownGlobPattern} --fix",
      "problemMatcher": "$markdownlint"
    }
  ],
  "inputs": [
    {
      "id": "markdownGlobPattern",
      "type": "promptString",
      "description": "Markdown file path (e.g., README.md or docs/guide.md)",
      "default": "**/*.md"
    }
  ]
}
```

## Step 5: Verify configuration alignment

Confirm consistency across all configuration files:

- `.markdownlint-cli2.jsonc` — Linting rules
- `.vscode/settings.json` — Editor settings match linting rules
- `.vscode/extensions.json` — Recommended extensions installed
- `.vscode/tasks.json` — Task definitions for linting workflow
- `.editorconfig` — Tab/space settings align (2 spaces for markdown)

## Step 6: Test the setup

```bash
# Install npm/pnpm dependencies (if needed)
npm install -g markdownlint-cli2

# Test linting on a specific markdown file
npx markdownlint-cli2 --no-globs "README.md"

# Test auto-fix on a specific markdown file
npx markdownlint-cli2 --no-globs "README.md" --fix

# Test linting on multiple specific files
npx markdownlint-cli2 --no-globs "README.md" "CONTRIBUTING.md"
```

**Expected results:**

- Zero linting errors on existing markdown files (or known violations)
- Auto-fix resolves formatting issues (bullets, headers, spacing)
- VS Code shows inline warnings for markdown violations
- Format-on-save applies fixes automatically
