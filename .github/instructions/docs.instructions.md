---
applyTo: "docs/**"
description: "Documentation website development - VitePress builds, accessibility testing, content guidelines"
---

# Documentation website instructions

These instructions apply to all files in the docs/ folder.

## Local development

```bash
# from /docs folder
pnpm install
pnpm run docs:dev
# Site will open at http://localhost:5173/
```

## VitePress-specific guidelines

### Alert blocks

VitePress supports native custom container syntax. Use these instead of GitHub alert syntax:

- `::: tip` — Helpful suggestions
- `::: warning` — Important warnings
- `::: danger` — Critical warnings about dangerous operations
- `::: details` — Collapsible details sections
- `::: info` — Informational highlights (default)

```markdown
::: warning
This is a warning message
:::

::: tip ✨ Pro tip
You can add custom titles
:::
```

**Note:** Avoid using GitHub alert blocks (`> [!NOTE]`, `> [!WARNING]`, etc.) in VitePress pages. However, they are still preferred in non-website pages.

### Asset management

- Place static assets in `.vitepress/public/` directory
- Assets in public directory are served from root path
- Use absolute paths starting with `/` for assets
- For images in markdown content, use HTML `<img>` tags with absolute paths

## Build and testing

```bash
# Production build
pnpm run docs:build

# Preview production build (starts server on port 4173)
pnpm run docs:preview
```

### Accessibility testing

**Option 1**: Use VS Code task: **Tasks: Run Task** → `Test: Website a11y (pa11y)`

**Option 2**: Run script manually (from docs/ folder):

```bash
bash .vitepress/test-a11y.sh
```

The script builds the site, starts a local preview server, and runs pa11y-ci against localhost URLs (not production).

## Content guidelines

- Add or update files in `/docs/indicators/` directory
- Place image assets in `/docs/.vitepress/public/assets/` folder
- Follow consistent naming conventions for asset files
- Include comprehensive examples with sample data
- Use HTML `<img>` tags instead of markdown syntax for images (avoids SSR import issues)
- Optimize images to webp format: `cwebp -resize 832 0 -q 100 input.png -o output.webp`

## Accessibility requirements

- Use semantic HTML elements when HTML is required
- Provide alt text for all images
- Ensure proper heading hierarchy (no skipping levels)
- Include descriptive link text (avoid "click here")
- Run Lighthouse for accessibility audits

## Front matter requirements

- Include required YAML front matter for all pages
- Use consistent layout references
- Set appropriate page titles and descriptions
- Include navigation metadata when applicable

---
Last updated: January 25, 2026
