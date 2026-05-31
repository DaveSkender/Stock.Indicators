---
name: vitepress-deployment
description: Deploying VitePress sites to various platforms including GitHub Pages, Netlify, Vercel, and more
---

# Deployment

Deploy VitePress static sites to various hosting platforms.

## Build and Preview

```bash
# Build production files
npm run docs:build

# Preview locally
npm run docs:preview
```

Output is in `.vitepress/dist` by default.

## Setting Base Path

For sub-path deployment (e.g., `https://user.github.io/repo/`):

```ts
// .vitepress/config.ts
export default {
  base: '/repo/'
}
```

## GitHub Pages

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy VitePress

on:
  push:
    branches: [main]
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

concurrency:
  group: pages
  cancel-in-progress: false

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v5
        with:
          fetch-depth: 0
      - uses: actions/setup-node@v6
        with:
          node-version: 24
          cache: npm
      - run: npm ci
      - run: npm run docs:build
      - uses: actions/upload-pages-artifact@v3
        with:
          path: docs/.vitepress/dist

  deploy:
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    needs: build
    runs-on: ubuntu-latest
    steps:
      - uses: actions/deploy-pages@v4
        id: deployment
```

Enable GitHub Pages in repository settings → Pages → Source: "GitHub Actions".

For pnpm, add before setup-node:

```yaml
- uses: pnpm/action-setup@v4
  with:
    version: 9
```

## Netlify / Vercel / Cloudflare Pages

Configure in dashboard:

| Setting | Value |
|---------|-------|
| Build Command | `npm run docs:build` |
| Output Directory | `docs/.vitepress/dist` |
| Node Version | `20` (or above) |

**Warning:** Don't enable "Auto Minify" for HTML - it removes Vue hydration comments.

### Vercel Configuration

For clean URLs, add `vercel.json`:

```json
{
  "cleanUrls": true
}
```

## GitLab Pages

Create `.gitlab-ci.yml`:

```yaml
image: node:18

pages:
  cache:
    paths:
      - node_modules/
  script:
    - npm install
    - npm run docs:build
  artifacts:
    paths:
      - public
  only:
    - main
```

Set `outDir: '../public'` in config if needed.

## Firebase

```json
// firebase.json
{
  "hosting": {
    "public": "docs/.vitepress/dist",
    "ignore": []
  }
}
```

```bash
npm run docs:build
firebase deploy
```

## Nginx

```nginx
server {
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml;

    listen 80;
    server_name _;
    index index.html;

    location / {
        root /app;
        try_files $uri $uri.html $uri/ =404;
        error_page 404 /404.html;
        error_page 403 /404.html;
    }

    # Cache hashed assets
    location ~* ^/assets/ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

**Important:** Don't default to `index.html` like SPAs - use `$uri.html` for clean URLs.

## HTTP Cache Headers

For hashed assets (immutable):

```
Cache-Control: max-age=31536000, immutable
```

### Netlify `_headers`

Place in `docs/public/_headers`:

```
/assets/*
  cache-control: max-age=31536000
  cache-control: immutable
```

### Vercel `vercel.json`

```json
{
  "headers": [
    {
      "source": "/assets/(.*)",
      "headers": [
        {
          "key": "Cache-Control",
          "value": "max-age=31536000, immutable"
        }
      ]
    }
  ]
}
```

## Other Platforms

| Platform | Guide |
|----------|-------|
| Azure Static Web Apps | Set `app_location: /`, `output_location: docs/.vitepress/dist` |
| Surge | `npx surge docs/.vitepress/dist` |
| Heroku | Use `heroku-buildpack-static` |
| Render | Build: `npm run docs:build`, Publish: `docs/.vitepress/dist` |
| Kinsta | Follow [Kinsta docs](https://kinsta.com/docs/vitepress-static-site-example/) |

## Key Points

- Set `base` for sub-path deployments
- GitHub Pages requires workflow file and enabling Pages in settings
- Most platforms: Build `npm run docs:build`, output `docs/.vitepress/dist`
- Don't enable HTML minification (breaks hydration)
- Cache `/assets/*` with immutable headers
- For clean URLs on Nginx, use `try_files $uri $uri.html $uri/ =404`

<!--
Source references:
- https://vitepress.dev/guide/deploy
-->
