# Documentation website

Visit [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev) to read our documentation.

This site is built with [VitePress](https://vitepress.dev) and deployed to Cloudflare Pages.

## Local Development

The site renders charts via [`@facioquo/indy-charts`](https://github.com/facioquo/stock-charts) from GitHub Packages, which needs an authenticated install. Add the `read:packages` scope to your gh CLI token once, then install and run:

```bash
gh auth refresh --scopes read:packages       # one-time per token
cd docs
NODE_AUTH_TOKEN=$(gh auth token) pnpm install
pnpm run docs:dev
```

The site will open at `http://localhost:5173/`. The `NODE_AUTH_TOKEN` prefix is wired into `docs/.npmrc` to authenticate the `@facioquo` scope. VS Code's `Install: Node packages (pnpm)` task does the same — pick whichever fits your flow.

CI workflows authenticate via the `FACIOQUO_PACKAGES_TOKEN` repository secret; nothing to set up there.

### GitHub token (optional)

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
