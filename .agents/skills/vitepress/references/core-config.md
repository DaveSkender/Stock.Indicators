---
name: vitepress-configuration
description: Config file setup, defineConfig helper, site metadata, and build options
---

# Configuration

VitePress configuration is defined in `.vitepress/config.[js|ts|mjs|mts]`. Use `defineConfig` for TypeScript intellisense.

## Basic Config

```ts
// .vitepress/config.ts
import { defineConfig } from 'vitepress'

export default defineConfig({
  // Site metadata
  title: 'My Docs',
  description: 'Documentation site',
  lang: 'en-US',
  
  // URL base path (for GitHub Pages: '/repo-name/')
  base: '/',
  
  // Theme configuration
  themeConfig: {
    // See theme-config.md
  }
})
```

## Site Metadata

```ts
export default defineConfig({
  title: 'VitePress',           // Displayed in nav, used in page titles
  titleTemplate: ':title - Docs', // Page title format (:title = h1)
  description: 'Site description', // Meta description
  lang: 'en-US',                 // HTML lang attribute
  
  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }],
    ['meta', { name: 'theme-color', content: '#5f67ee' }],
    ['script', { async: '', src: 'https://analytics.example.com/script.js' }]
  ]
})
```

## Build Options

```ts
export default defineConfig({
  // Source files directory (relative to project root)
  srcDir: './src',
  
  // Exclude patterns from source
  srcExclude: ['**/README.md', '**/TODO.md'],
  
  // Output directory
  outDir: './.vitepress/dist',
  
  // Cache directory
  cacheDir: './.vitepress/cache',
  
  // Clean URLs without .html extension (requires server support)
  cleanUrls: true,
  
  // Ignore dead links during build
  ignoreDeadLinks: true,
  // Or specific patterns:
  ignoreDeadLinks: ['/playground', /^https?:\/\/localhost/],
  
  // Get last updated timestamp from git
  lastUpdated: true
})
```

## Route Rewrites

Map source paths to different output paths:

```ts
export default defineConfig({
  rewrites: {
    // Static mapping
    'packages/pkg-a/src/index.md': 'pkg-a/index.md',
    
    // Dynamic parameters
    'packages/:pkg/src/:slug*': ':pkg/:slug*'
  }
})
```

## Appearance (Dark Mode)

```ts
export default defineConfig({
  appearance: true,           // Enable toggle (default)
  appearance: 'dark',         // Dark by default
  appearance: 'force-dark',   // Always dark, no toggle
  appearance: 'force-auto',   // Always follow system preference
  appearance: false           // Disable dark mode
})
```

## Vite & Vue Configuration

```ts
export default defineConfig({
  // Pass options to Vite
  vite: {
    plugins: [],
    resolve: { alias: {} },
    css: { preprocessorOptions: {} }
  },
  
  // Pass options to @vitejs/plugin-vue
  vue: {
    template: { compilerOptions: {} }
  },
  
  // Configure markdown-it
  markdown: {
    lineNumbers: true,
    toc: { level: [1, 2, 3] },
    math: true, // Requires markdown-it-mathjax3
    container: {
      tipLabel: 'TIP',
      warningLabel: 'WARNING',
      dangerLabel: 'DANGER'
    }
  }
})
```

## Build Hooks

```ts
export default defineConfig({
  // Transform page data
  transformPageData(pageData, { siteConfig }) {
    pageData.frontmatter.head ??= []
    pageData.frontmatter.head.push([
      'meta', { name: 'og:title', content: pageData.title }
    ])
  },
  
  // Transform head before generating each page
  async transformHead(context) {
    return [['meta', { name: 'custom', content: context.page }]]
  },
  
  // After build completes
  async buildEnd(siteConfig) {
    // Generate sitemap, RSS, etc.
  }
})
```

## Dynamic Config

For async configuration:

```ts
export default async () => {
  const data = await fetch('https://api.example.com/data').then(r => r.json())
  
  return defineConfig({
    title: data.title,
    themeConfig: {
      sidebar: data.sidebar
    }
  })
}
```

## Key Points

- Config file supports `.js`, `.ts`, `.mjs`, `.mts` extensions
- Use `defineConfig` for TypeScript support
- `base` must start and end with `/` for sub-path deployments
- `srcDir` separates source files from project root
- Build hooks enable custom transformations and post-processing

<!--
Source references:
- https://vitepress.dev/reference/site-config
- https://vitepress.dev/guide/getting-started
-->
