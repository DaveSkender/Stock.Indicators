---
applyTo: "docs/**"
description: "Documentation website development, Jekyll builds, and content guidelines"
---

# Documentation website instructions

These instructions apply to all files in the `docs/` folder and cover Jekyll site development and content creation.

## Build and development workflow

### Local development setup

```bash
bundle install
bundle exec jekyll serve --livereload

```

The site will be available at `http://127.0.0.1:4000`.

### Code cleanup and formatting

- **Markdown linting**: Follow markdown linting rules in the `.github/instructions/markdown.instructions.md` file
- **Jekyll configuration**: Follow Jekyll best practices in `_config.yml`
- **Front matter validation**: Ensure YAML front matter follows documented schema

## Content guidelines

### Indicator documentation

When adding or updating indicators:

- Add or update files in the `_indicators/` directory
- Place image assets in the `assets/` folder
- Follow consistent naming conventions for asset files
- Include comprehensive examples with sample data

### Content structure

- Use semantic HTML elements when HTML is required
- Provide alt text for all images
- Ensure proper heading hierarchy (no skipping levels)
- Include descriptive link text (avoid "click here")

## Jekyll-specific guidelines

### Front matter requirements

- Include required YAML front matter for all pages
- Use consistent layout references
- Set appropriate page titles and descriptions
- Include navigation metadata when applicable

### Template and includes

- Follow established patterns in `_layouts/` and `_includes/`
- Maintain consistency in template structure
- Use Jekyll's data files in `_data/` for structured content
- Leverage Jekyll plugins appropriately

### Asset management

- Place static assets in appropriate subdirectories under `/assets/`
- Use Jekyll's asset pipeline for CSS and JavaScript
- Optimize images for web delivery
- Follow naming conventions for asset files

## Testing and validation

### Pre-commit testing

Before committing documentation changes:

1. Build verification: Ensure Jekyll builds without errors
2. Link checking: Verify all internal and external links work
3. Content review: Check for typos and formatting consistency

### Continuous integration

The documentation site should build successfully in the CI/CD pipeline.

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
Last updated: December 7, 2025
