---
name: vitepress-dynamic-routes
description: Generate multiple pages from a single markdown template using paths loader files
---

# Dynamic routes

Generate many pages from a single markdown file and dynamic data. Useful for blogs, package docs, or any data-driven pages.

## Basic setup

Create a template file with parameter in brackets and a paths loader:

```plaintext
.
└─ packages/
   ├─ [pkg].md           # Route template
   └─ [pkg].paths.js     # Paths loader
```

The paths loader exports a `paths` method returning route parameters:

```js
// packages/[pkg].paths.js
export default {
  paths() {
    return [
      { params: { pkg: 'foo' }},
      { params: { pkg: 'bar' }},
      { params: { pkg: 'baz' }}
    ]
  }
}
```

Generated pages:
- `/packages/foo.html`
- `/packages/bar.html`
- `/packages/baz.html`

## Multiple parameters

```plaintext
.
└─ packages/
   ├─ [pkg]-[version].md
   └─ [pkg]-[version].paths.js
```

```js
// packages/[pkg]-[version].paths.js
export default {
  paths() {
    return [
      { params: { pkg: 'foo', version: '1.0.0' }},
      { params: { pkg: 'foo', version: '2.0.0' }},
      { params: { pkg: 'bar', version: '1.0.0' }}
    ]
  }
}
```

## Dynamic path generation

From local files:

```js
// packages/[pkg].paths.js
import fs from 'node:fs'

export default {
  paths() {
    return fs.readdirSync('packages').map(pkg => ({
      params: { pkg }
    }))
  }
}
```

From remote API:

```js
// packages/[pkg].paths.js
export default {
  async paths() {
    const packages = await fetch('https://api.example.com/packages').then(r => r.json())
    
    return packages.map(pkg => ({
      params: {
        pkg: pkg.name,
        version: pkg.version
      }
    }))
  }
}
```

## Accessing params in page

Template globals:

```md
<!-- packages/[pkg].md -->
# Package: {{ $params.pkg }}

Version: {{ $params.version }}
```

In script:

```vue
<script setup>
import { useData } from 'vitepress'
const { params } = useData()
</script>

<template>
  <h1>{{ params.pkg }}</h1>
</template>
```

## Passing content

For heavy content (raw markdown/HTML from CMS), use `content` instead of params to avoid bloating the client bundle:

```js
// posts/[slug].paths.js
export default {
  async paths() {
    const posts = await fetch('https://cms.example.com/posts').then(r => r.json())
    
    return posts.map(post => ({
      params: { slug: post.slug },
      content: post.content  // Raw markdown or HTML
    }))
  }
}
```

Render content in template:

```md
<!-- posts/[slug].md -->
---
title: {{ $params.title }}
---

<!-- @content -->
```

The `<!-- @content -->` placeholder is replaced with the content from the paths loader.

## Watch option

Auto-rebuild when template or data files change:

```js
// posts/[slug].paths.js
export default {
  watch: [
    './templates/**/*.njk',
    '../data/**/*.json'
  ],
  
  paths(watchedFiles) {
    const dataFiles = watchedFiles.filter(f => f.endsWith('.json'))
    
    return dataFiles.map(file => {
      const data = JSON.parse(fs.readFileSync(file, 'utf-8'))
      return {
        params: { slug: data.slug },
        content: renderTemplate(data)
      }
    })
  }
}
```

## Complete example: Blog

```js
// posts/[slug].paths.js
import fs from 'node:fs'
import matter from 'gray-matter'

export default {
  watch: ['./posts/*.md'],
  
  paths(files) {
    return files
      .filter(f => !f.includes('[slug]'))
      .map(file => {
        const content = fs.readFileSync(file, 'utf-8')
        const { data, content: body } = matter(content)
        const slug = file.match(/([^/]+)\.md$/)[1]
        
        return {
          params: { 
            slug,
            title: data.title,
            date: data.date
          },
          content: body
        }
      })
  }
}
```

```md
<!-- posts/[slug].md -->
---
layout: doc
---

# {{ $params.title }}

<time>{{ $params.date }}</time>

<!-- @content -->
```

## Key points

- Template file uses `[param]` syntax in filename
- Paths loader file must be named `[param].paths.js` or `.ts`
- `paths()` returns array of `{ params: {...}, content?: string }`
- Use `$params` in templates or `useData().params` in scripts
- Use `content` for heavy data to avoid client bundle bloat
- `watch` enables HMR for template/data file changes

---
Last updated: February 21, 2026

<!--
Source references:
- https://vitepress.dev/guide/routing#dynamic-routes
-->
