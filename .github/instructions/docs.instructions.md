---
applyTo: "docs/**"
description: "Documentation website development, VitePress builds, accessibility testing, and content guidelines"
---

# Documentation website instructions

These instructions apply to all files in the `docs/` folder and cover VitePress site development, content creation, accessibility testing, and documentation maintenance.

## Build and development workflow

### Local development setup

```bash
# from /docs folder
pnpm install
pnpm run docs:dev

# the site will open at http://localhost:5173/
```

### Code cleanup and formatting

- **Markdown linting**: Use repository-wide markdown linting rules
- **VitePress configuration**: Follow VitePress best practices in `.vitepress/config.mts`
- **Front matter validation**: Ensure YAML front matter follows documented schema
- **Asset optimization**: Optimize images to `webp` format using ImageMagick or cwebp

  ```bash
  # optimize images to webp
  cwebp -resize 832 0 -q 100 examples.png -o examples-832.webp
  ```

## Content guidelines

### Indicator documentation

When adding or updating indicators:

- Add or update files in `/docs/indicators/` directory
- Place image assets in `/docs/.vitepress/public/assets/` folder
- Follow consistent naming conventions for asset files
- Include comprehensive examples with sample data
- Use HTML `<img>` tags instead of markdown syntax for images to avoid SSR import issues

### Accessibility requirements

- **Lighthouse testing**: Use Chrome Lighthouse for accessibility audits
- **Automated accessibility testing**: Run pa11y-ci against local build

```bash
# accessibility testing after production build
pnpm run docs:build
npx pa11y-ci --sitemap http://localhost:5173/sitemap.xml
```

### Content structure

- Use semantic HTML elements when HTML is required
- Provide alt text for all images
- Ensure proper heading hierarchy (no skipping levels)
- Include descriptive link text (avoid "click here")

## VitePress-specific guidelines

### Front matter requirements

- Include required YAML front matter for all pages
- Use consistent layout references
- Set appropriate page titles and descriptions
- Include navigation metadata when applicable

### Components and layouts

- Use Vue components for interactive elements
- Follow VitePress default theme conventions
- Leverage VitePress built-in components when possible
- Use custom components sparingly

### Alert blocks

VitePress supports native custom container syntax for alert blocks. Use these instead of GitHub alert syntax in VitePress documentation:

- `::: tip` — Helpful suggestions and tips
- `::: warning` — Important warnings and caution messages
- `::: danger` — Critical warnings about dangerous operations
- `::: details` — Collapsible details sections
- `::: info` — Informational highlights (default, can omit type)

**Syntax:**

```markdown
::: warning
This is a warning message
:::

::: tip 💡 Pro tip
You can add custom titles
:::
```

**Note:** Avoid using GitHub alert blocks (`> [!NOTE]`, `> [!WARNING]`, etc.) in VitePress pages. However, they are still preferred in non-website pages.

### Asset management

- Place static assets in `.vitepress/public/` directory
- Assets in public directory are served from root path
- Use absolute paths starting with `/` for assets
- Follow naming conventions for asset files
- For images in markdown content, use HTML `<img>` tags with absolute paths

## Testing and validation

### Pre-commit testing

Before committing documentation changes:

1. **Build verification**: Ensure VitePress builds without errors
2. **Link checking**: Verify all internal and external links work
3. **Accessibility audit**: Run accessibility tests
4. **Content review**: Check for typos and formatting consistency

### Build commands

```bash
# Development server
pnpm run docs:dev

# Production build
pnpm run docs:build

# Preview production build
pnpm run docs:preview
```

### Continuous integration

The documentation site should:

- Build successfully in the CI/CD pipeline
- Pass all accessibility tests
- Have no broken links
- Meet performance benchmarks

## Content maintenance

### Regular updates

- Keep indicator documentation current with library changes
- Update examples when API changes occur
- Refresh screenshots and visual examples periodically
- Maintain accuracy of mathematical formulas and calculations

### Version compatibility

- Document version-specific features appropriately
- Maintain backward compatibility in examples where possible
- Clearly mark deprecated features
- Provide migration guidance for breaking changes

---
Last updated: December 14, 2025
