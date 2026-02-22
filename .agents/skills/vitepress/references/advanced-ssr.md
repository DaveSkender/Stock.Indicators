---
name: vitepress-ssr-compatibility
description: Server-side rendering compatibility, ClientOnly component, and handling browser-only code
---

# SSR Compatibility

VitePress pre-renders pages on the server during build. All Vue code must be SSR-compatible.

## The Rule

Only access browser/DOM APIs in Vue lifecycle hooks:
- `onMounted()`
- `onBeforeMount()`

```vue
<script setup>
import { onMounted, ref } from 'vue'

const windowWidth = ref(0)

onMounted(() => {
  // Safe - runs only in browser
  windowWidth.value = window.innerWidth
})
</script>
```

**Do NOT** access at top level:

```vue
<script setup>
// WRONG - runs during SSR where window doesn't exist
const width = window.innerWidth
</script>
```

## ClientOnly Component

Wrap non-SSR-friendly components:

```vue
<template>
  <ClientOnly>
    <BrowserOnlyComponent />
  </ClientOnly>
</template>
```

## Libraries That Access Browser on Import

Some libraries access `window` or `document` when imported:

### Dynamic Import in onMounted

```vue
<script setup>
import { onMounted } from 'vue'

onMounted(async () => {
  const lib = await import('browser-only-library')
  lib.doSomething()
})
</script>
```

### Conditional Import

```ts
if (!import.meta.env.SSR) {
  const lib = await import('browser-only-library')
  lib.doSomething()
}
```

### In enhanceApp

```ts
// .vitepress/theme/index.ts
export default {
  async enhanceApp({ app }) {
    if (!import.meta.env.SSR) {
      const plugin = await import('browser-plugin')
      app.use(plugin.default)
    }
  }
}
```

## defineClientComponent

Helper for components that access browser on import:

```vue
<script setup>
import { defineClientComponent } from 'vitepress'

const BrowserComponent = defineClientComponent(() => {
  return import('browser-only-component')
})
</script>

<template>
  <BrowserComponent />
</template>
```

With props and slots:

```vue
<script setup>
import { ref, h } from 'vue'
import { defineClientComponent } from 'vitepress'

const componentRef = ref(null)

const BrowserComponent = defineClientComponent(
  () => import('browser-only-component'),
  // Props passed to h()
  [
    { ref: componentRef, someProp: 'value' },
    {
      default: () => 'Default slot content',
      header: () => h('div', 'Header slot')
    }
  ],
  // Callback after component loads
  () => {
    console.log('Component loaded', componentRef.value)
  }
)
</script>
```

## Teleports

Teleport to body only with SSG:

```vue
<ClientOnly>
  <Teleport to="body">
    <div class="modal">Modal content</div>
  </Teleport>
</ClientOnly>
```

For other targets, use `postRender` hook:

```ts
// .vitepress/config.ts
export default {
  async postRender(context) {
    // Inject teleport content into final HTML
  }
}
```

## Common SSR Errors

### "window is not defined"

Code accesses `window` at module level:

```ts
// BAD
const width = window.innerWidth

// GOOD
let width: number
onMounted(() => {
  width = window.innerWidth
})
```

### "document is not defined"

Same issue with `document`:

```ts
// BAD
const el = document.querySelector('#app')

// GOOD
onMounted(() => {
  const el = document.querySelector('#app')
})
```

### Hydration Mismatch

Server and client render different content:

```vue
<!-- BAD - different on server vs client -->
<div>{{ typeof window !== 'undefined' ? 'client' : 'server' }}</div>

<!-- GOOD - consistent -->
<ClientOnly>
  <div>Client only content</div>
</ClientOnly>
```

## Checking Environment

```ts
// In Vue component
import.meta.env.SSR  // true on server, false on client

// In VitePress
import { inBrowser } from 'vitepress'
if (inBrowser) {
  // Client-only code
}
```

## Key Points

- Access browser APIs only in `onMounted` or `onBeforeMount`
- Use `<ClientOnly>` for non-SSR components
- Use `defineClientComponent` for libraries that access browser on import
- Check `import.meta.env.SSR` for environment-specific code
- Teleport to body only, or use `postRender` hook
- Consistent rendering prevents hydration mismatches

<!--
Source references:
- https://vitepress.dev/guide/ssr-compat
-->
