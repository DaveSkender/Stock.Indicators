# Documentation website

Visit [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev) to read the published documentation.

The site uses Angular with standalone components and is not intended to be browsed directly from this repository.

## Development

### Run the site locally

```bash
# Install dependencies
npm install

# Start the development server
npm run start
# Site will be available at http://localhost:4200

# Build for production
npm run build:complete

# Run tests
npm run test        # Jest unit tests
npm run e2e         # Playwright e2e tests
```

### Linting

The documentation markdown uses `markdownlint-cli2` with shared rule settings.

```bash
npm run lint:md      # Reports issues
npm run lint:md:fix  # Applies safe auto-fixes
```

Rule relaxations (for front matter titles and inline HTML used by the site) live in `.markdownlint-cli2.jsonc`, keeping the CLI, CI, and IDE integrations aligned.

## Content structure

- Store user-facing markdown pages in `docs/pages`. Use `permalink` front matter to define their routes.
- Use `docs/pages/home.md` for the landing page instead of `pages/index.md`.
- Keep the repository root markdown limited to `README.md` and `contributing.md` to avoid duplicate sources.

---
Last updated: September 28, 2025
