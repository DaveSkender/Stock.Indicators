---
name: vitepress-data-loading
description: Build-time data loaders for fetching remote data or processing local files
---

# Data Loading

VitePress data loaders run at build time to load arbitrary data that's serialized as JSON in the client bundle.

## Basic Usage

Create a file ending with `.data.js` or `.data.ts`:

```ts
// example.data.ts
export default {
  load() {
    return {
      hello: 'world',
      timestamp: Date.now()
    }
  }
}
```

Import the `data` named export:

```vue
<script setup>
import { data } from './example.data.ts'
</script>

<template>
  <pre>{{ data }}</pre>
</template>
```

## Async Data

Fetch remote data:

```ts
// api.data.ts
export default {
  async load() {
    const response = await fetch('https://api.example.com/data')
    return response.json()
  }
}
```

## Local Files with Watch

Process local files with hot reload:

```ts
// posts.data.ts
import fs from 'node:fs'
import { parse } from 'csv-parse/sync'

export default {
  watch: ['./data/*.csv'],
  load(watchedFiles) {
    // watchedFiles = array of absolute paths
    return watchedFiles.map(file => {
      return parse(fs.readFileSync(file, 'utf-8'), {
        columns: true,
        skip_empty_lines: true
      })
    })
  }
}
```

## createContentLoader

Helper for loading markdown content (common for blogs/archives):

```ts
// posts.data.ts
import { createContentLoader } from 'vitepress'

export default createContentLoader('posts/*.md')
```

Returns array of `ContentData`:

```ts
interface ContentData {
  url: string                    // e.g. /posts/hello.html
  frontmatter: Record<string, any>
  src?: string                   // raw markdown (opt-in)
  html?: string                  // rendered HTML (opt-in)
  excerpt?: string               // excerpt HTML (opt-in)
}
```

With options:

```ts
// posts.data.ts
import { createContentLoader } from 'vitepress'

export default createContentLoader('posts/*.md', {
  includeSrc: true,     // Include raw markdown
  render: true,         // Include rendered HTML
  excerpt: true,        // Include excerpt (content before first ---)
  
  transform(rawData) {
    // Sort by date, newest first
    return rawData
      .sort((a, b) => +new Date(b.frontmatter.date) - +new Date(a.frontmatter.date))
      .map(page => ({
        title: page.frontmatter.title,
        url: page.url,
        date: page.frontmatter.date,
        excerpt: page.excerpt
      }))
  }
})
```

## Usage Example: Blog Index

```ts
// posts.data.ts
import { createContentLoader } from 'vitepress'

export default createContentLoader('posts/*.md', {
  excerpt: true,
  transform(data) {
    return data
      .filter(post => !post.frontmatter.draft)
      .sort((a, b) => +new Date(b.frontmatter.date) - +new Date(a.frontmatter.date))
  }
})
```

```vue
<!-- posts/index.md -->
<script setup>
import { data as posts } from './posts.data.ts'
</script>

<template>
  <ul>
    <li v-for="post in posts" :key="post.url">
      <a :href="post.url">{{ post.frontmatter.title }}</a>
      <span>{{ post.frontmatter.date }}</span>
    </li>
  </ul>
</template>
```

## Typed Data Loaders

```ts
// example.data.ts
import { defineLoader } from 'vitepress'

export interface Data {
  posts: Array<{ title: string; url: string }>
}

declare const data: Data
export { data }

export default defineLoader({
  watch: ['./posts/*.md'],
  async load(): Promise<Data> {
    // ...
    return { posts: [] }
  }
})
```

## In Build Hooks

Use in config for generating additional files:

```ts
// .vitepress/config.ts
import { createContentLoader } from 'vitepress'

export default {
  async buildEnd() {
    const posts = await createContentLoader('posts/*.md').load()
    // Generate RSS feed, sitemap, etc.
  }
}
```

## Accessing Config

```ts
// example.data.ts
import type { SiteConfig } from 'vitepress'

export default {
  load() {
    const config: SiteConfig = (globalThis as any).VITEPRESS_CONFIG
    return { base: config.site.base }
  }
}
```

## Key Points

- Data loaders run only at build time in Node.js
- File must end with `.data.js` or `.data.ts`
- Import the `data` named export (not default)
- Use `watch` for local file hot reload during dev
- `createContentLoader` simplifies loading markdown collections
- Keep data small - it's inlined in the client bundle
- Heavy data should use `transform` to reduce payload

<!--
Source references:
- https://vitepress.dev/guide/data-loading
-->
