# Documentation website

Visit [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev) to read our documentation.

This site is built with [VitePress](https://vitepress.dev) and deployed to Cloudflare Pages.

## Local Development

```bash
cd docs
NODE_AUTH_TOKEN=$(gh auth token) pnpm install
pnpm run docs:dev
```

The site will open at `http://localhost:5173/`

### GitHub Packages authentication (required for install)

This site renders interactive charts via [`@facioquo/indy-charts`](https://github.com/facioquo/stock-charts), a restricted-access package on [GitHub Packages](https://github.com/features/packages). `pnpm install` needs an authenticated token with `read:packages` scope to fetch it.

The simplest way to provide one is via the [GitHub CLI](https://cli.github.com):

```bash
# one-time per machine — be sure to grant `read:packages` when prompted
gh auth login

# every install — export the token for the @facioquo registry in .npmrc
NODE_AUTH_TOKEN=$(gh auth token) pnpm install
```

Alternatively, [create a personal access token (classic)](https://github.com/settings/tokens/new?scopes=read:packages) with `read:packages` and export it as `NODE_AUTH_TOKEN` in your shell profile. The repo-root `docs/.npmrc` wires the token into the `@facioquo` scope automatically.

CI workflows authenticate via the `FACIOQUO_PACKAGES_TOKEN` repository secret — already configured in `.github/workflows/*.yml`.

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
