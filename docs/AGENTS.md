# Documentation website

This folder contains the VitePress documentation site for Stock Indicators at [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev).

Load #skill:vitepress for VitePress configuration, routing, theme, and component guidance.

Load #skill:markdown for general Markdown authoring standards, linting workflow, and validation.

## Quick start

```bash
# from /docs folder
pnpm install
pnpm run docs:dev
# Opens at http://localhost:5173/
```

## Indy Charts

Indy Charts is a public npm package hosted in the facioquo GitHub org GitHub Packages registry and has source code in the facioquo/stock-charts repository. As such it requires [setup and general GitHub authentication through a personal access token](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-npm-registry#installing-a-package), which is stored in this repos FACIOQUO_PACKAGE_TOKEN secret for use in GitHub Action workflows that build, test, and deploy this documentation site. For local environments, developers must extend their normal GitHub session token with `gh auth refresh --scopes read:packages`.

## Build and preview

```bash
# Production build
pnpm run docs:build

# Preview production build (port 4173)
pnpm run docs:preview
```

## Visual inspection with Playwright

Use the Playwright MCP tool to visually inspect docs work against the dev server.

Start the dev server first (port 5173):

```bash
# from /docs folder
pnpm run docs:dev
```

Then use the #tool:playwright MCP tool to navigate, screenshot, and inspect pages:

- Navigate to `http://localhost:5173/<page-path>` to inspect any page
- Take screenshots to verify layout, typography, and component rendering
- Inspect element state, check link targets, and validate page structure

## Content guidelines

- Add indicator pages to the `indicators/` directory
- Place image assets in `.vitepress/public/assets/`
- Use HTML `<img>` tags instead of Markdown image syntax (avoids SSR import issues)
- Optimize images to webp: `cwebp -resize 832 0 -q 100 input.png -o output.webp`
- All pages require YAML front matter with title, description, and layout metadata

## VitePress alert blocks

Use VitePress native container syntax instead of GitHub alert syntax in docs pages:

- `::: tip` — helpful suggestions
- `::: warning` — important warnings
- `::: danger` — critical warnings
- `::: info` — informational highlights (default)
- `::: details` — collapsible sections

GitHub alert blocks (`> [!NOTE]`, `> [!WARNING]`) are still preferred in non-website Markdown files.
