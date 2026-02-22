---
name: vue-in-vitepress-markdown
description: Using Vue components, script setup, directives, and templating in markdown files
---

# Vue in Markdown

VitePress markdown files are compiled as Vue Single-File Components, enabling full Vue functionality.

## Interpolation

Vue expressions work in markdown:

```md
{{ 1 + 1 }}

{{ new Date().toLocaleDateString() }}
```

## Directives

HTML with Vue directives:

```md
<span v-for="i in 3">{{ i }}</span>

<div v-if="$frontmatter.showBanner">
  Banner content
</div>
```

## Script and Style

Add `<script setup>` and `<style>` after frontmatter:

```md
---
title: My Page
---

<script setup>
import { ref } from 'vue'
import MyComponent from './MyComponent.vue'

const count = ref(0)
</script>

# {{ $frontmatter.title }}

Count: {{ count }}

<button @click="count++">Increment</button>

<MyComponent />

<style module>
.button {
  color: red;
}
</style>
```

**Note:** Use `<style module>` instead of `<style scoped>` to avoid bloating page size.

## Importing Components

Local import (code-split per page):

```md
<script setup>
import CustomComponent from '../components/CustomComponent.vue'
</script>

<CustomComponent />
```

## Global Components

Register in theme for use everywhere:

```ts
// .vitepress/theme/index.ts
import DefaultTheme from 'vitepress/theme'
import MyGlobalComponent from './MyGlobalComponent.vue'

export default {
  extends: DefaultTheme,
  enhanceApp({ app }) {
    app.component('MyGlobalComponent', MyGlobalComponent)
  }
}
```

Then use in any markdown:

```md
<MyGlobalComponent />
```

**Important:** Component names must contain a hyphen or be PascalCase to avoid being treated as inline HTML elements.

## Runtime API

Access VitePress data:

```md
<script setup>
import { useData, useRoute, useRouter } from 'vitepress'

const { page, frontmatter, theme, site } = useData()
const route = useRoute()
const router = useRouter()
</script>

Current page: {{ page.relativePath }}
```

## Global Variables

Available without import:

```md
# {{ $frontmatter.title }}

Params: {{ $params.id }}
```

## Components in Headers

```md
# My Title <Badge type="tip" text="v2.0" />
```

## Escaping Vue Syntax

Prevent Vue interpolation:

```md
<span v-pre>{{ will be displayed as-is }}</span>
```

Or use container:

```md
::: v-pre
{{ this won't be processed }}
:::
```

## Vue in Code Blocks

Enable Vue processing in fenced code with `-vue` suffix:

````md
```js-vue
Hello {{ 1 + 1 }}
```
````

## CSS Pre-processors

Supported out of the box (install the preprocessor):

```bash
npm install -D sass  # for .scss/.sass
npm install -D less  # for .less
npm install -D stylus # for .styl/.stylus
```

```vue
<style lang="scss">
.title {
  font-size: 20px;
}
</style>
```

## Using Teleports

Teleport to body only with SSG:

```md
<ClientOnly>
  <Teleport to="#modal">
    <div>Modal content</div>
  </Teleport>
</ClientOnly>
```

## VS Code IntelliSense

Enable Vue language features for `.md` files:

```json
// tsconfig.json
{
  "include": ["docs/**/*.ts", "docs/**/*.vue", "docs/**/*.md"],
  "vueCompilerOptions": {
    "vitePressExtensions": [".md"]
  }
}
```

```json
// .vscode/settings.json
{
  "vue.server.includeLanguages": ["vue", "markdown"]
}
```

## Key Points

- Markdown files are Vue SFCs - use `<script setup>` and `<style>`
- Access page data via `useData()` or `$frontmatter` global
- Import components locally or register globally in theme
- Use `<style module>` instead of `<style scoped>`
- Wrap non-SSR components in `<ClientOnly>`
- Component names must be PascalCase or contain hyphens

<!--
Source references:
- https://vitepress.dev/guide/using-vue
- https://vitepress.dev/reference/runtime-api
-->
