# Documentation System

This repository now uses Angular + Scully for documentation generation, replacing the previous Jekyll-based system.

## Quick Start

```bash
# Install dependencies
npm install

# Start development server
ng serve

# Build for production
npm run build:complete
```

## Architecture

- **Angular 20**: Modern framework for the documentation app
- **Scully**: Static site generator for SEO-friendly pre-rendered pages
- **Marked**: Markdown parser for converting .md files to HTML
- **Custom Service**: `MarkdownService` loads and parses markdown files with frontmatter

## File Structure

```
src/
├── app/
│   ├── docs/
│   │   ├── docs/          # Main docs component
│   │   ├── markdown.ts    # Markdown parsing service
│   │   └── docs-module.ts # Docs module
│   └── app.config.ts      # App configuration
├── docs/                  # Markdown source files
└── index.html             # App entry point

public/
└── docs/                  # Markdown files served as assets

dist/static/               # Build output (for Cloudflare Pages)
```

## Features

- ✅ Markdown-based content with frontmatter support
- ✅ SEO-friendly metadata (title, description, keywords)
- ✅ Automatic sitemap.xml generation
- ✅ Clean URLs and routing
- ✅ Responsive design
- ✅ Ready for Cloudflare Pages deployment

## Content Management

### Adding New Pages

1. Create a `.md` file in `src/docs/`
2. Add frontmatter:
   ```yaml
   ---
   title: Page Title
   description: Page description for SEO
   keywords: comma, separated, keywords
   order: 1
   ---
   ```
3. Copy the file to `public/docs/`
4. Build to regenerate sitemap

### Frontmatter Fields

- `title`: Page title (used in `<title>` tag)
- `description`: Meta description for SEO
- `keywords`: Meta keywords for SEO
- `order`: Optional ordering for navigation

## Deployment

The build outputs to `dist/static/` which can be directly deployed to Cloudflare Pages.

```bash
npm run build:complete
# Upload dist/static/ contents to Cloudflare Pages
```

## Migration from Jekyll

This system replaces the previous Jekyll-based docs. Key improvements:

- Modern Angular ecosystem integration
- Better TypeScript support
- Simplified maintenance
- Direct Cloudflare Pages compatibility
- Automatic sitemap generation