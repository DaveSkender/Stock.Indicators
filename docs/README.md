# Documentation website

Visit [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev) to read our documentation.

This site is built with [VitePress](https://vitepress.dev) and deployed to Cloudflare Pages.

## Local Development

```bash
cd docs
pnpm install
pnpm run docs:dev
```

The site will open at `http://localhost:5173/`

### GitHub Token (Optional)

The contributors list is automatically populated using the GitHub API. The build process will auto-detect your GitHub token from:

1. `GITHUB_TOKEN` environment variable (if set)
2. GitHub CLI (`gh auth token`) if authenticated

To authenticate with GitHub CLI:

```bash
gh auth login
```

If no token is found, the site will build successfully but the contributors list will be empty.

## Building

```bash
pnpm run docs:build
```

The built site will be in `.vitepress/dist/`

## Preview Build

```bash
pnpm run docs:preview
```
