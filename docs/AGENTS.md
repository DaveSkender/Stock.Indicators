# Documentation website development

This folder contains the Jekyll-based documentation website for Stock Indicators.

## Quick reference

- Follow documentation instructions in .github/instructions/docs.instructions.md
- Follow markdown instructions in .github/instructions/markdown.instructions.md
- Use `bundle exec jekyll serve --livereload` for local development
- Site will be available at `http://127.0.0.1:4000`

## Adding indicator documentation

- Add or update files in `_indicators/` directory
- Place image assets in `assets/` folder
- Include comprehensive examples with sample data
- Ensure proper YAML front matter

## Testing documentation changes

```bash
# Install dependencies (first time)
bundle install

# Start local server with live reload
bundle exec jekyll serve --livereload

# Lint markdown
npx markdownlint-cli2 --fix
```

---
Last updated: December 30, 2025
