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

## Building

```bash
pnpm run docs:build
```

The built site will be in `.vitepress/dist/`

## Preview Build

```bash
pnpm run docs:preview
```
