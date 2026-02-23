# Markdown validation checklist

Complete validation checklist for Markdown files before committing.

## Automated linting

- [ ] Zero errors from `npx markdownlint-cli2 --no-globs {filepath}`
- [ ] All structural issues auto-fixed
- [ ] All lists use hyphen bullets
- [ ] All headers are ATX style
- [ ] Blank lines around headers and code blocks

## Manual semantic review

- [ ] All headers are sentence case
- [ ] All bold labels at list start are sentence case
- [ ] Proper nouns capitalized (GitHub, TypeScript, MADR)
- [ ] Articles, prepositions, conjunctions lowercase

## Reference syntax

- [ ] No backticks around #file: tokens
- [ ] No trailing punctuation after #file: tokens
- [ ] Entry point files use plain-text path mentions, not #file:
- [ ] Skill references use #skill: for loading, links for optional
- [ ] Tool references use #tool:server/name format
- [ ] Agent references use `AgentName` in docs, AgentName in agents

## Content quality

- [ ] All internal markdown links resolve to existing files
- [ ] All code fences have language identifiers
- [ ] Headers follow sequential hierarchy (no skipping levels)
- [ ] Lists use proper indentation
- [ ] End-of-file formatting applied (if required)
  - Except: README.md (root), AGENTS.md (all), .agents/**/*.md, .github/**/*.md, docs/adr/*.md — do NOT include 'Last updated' or other end-of-file footers for these paths
- [ ] Mermaid diagrams use stroke styling (no fill colors)
- [ ] HTML uses only allowed elements
- [ ] All images have alt text

## Repository-specific

- [ ] File follows organizational voice (present tense, imperative)
- [ ] No historical context or migration details
- [ ] Content is actionable for autonomous agents
- [ ] No duplicate content (single source of truth)

## Pre-commit final check

- [ ] Run `npx markdownlint-cli2 --no-globs {filepath}` — zero errors
- [ ] Preview rendered markdown in VS Code (`Ctrl+Shift+V`)
- [ ] Verify all links clickable and resolve
- [ ] Verify all diagrams render correctly
- [ ] File ready for commit
