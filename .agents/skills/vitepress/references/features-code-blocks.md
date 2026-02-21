---
name: vitepress-code-blocks
description: Syntax highlighting, line highlighting, colored diffs, focus, and line numbers
---

# Code blocks

VitePress uses Shiki for syntax highlighting with powerful code block features.

## Syntax highlighting

Specify language after opening backticks:

````md
```js
export default {
  name: 'MyComponent'
}
```
````

Supports [all languages](https://shiki.style/languages) available in Shiki.

## Line highlighting

Highlight specific lines:

````md
```js{4}
export default {
  data() {
    return {
      msg: 'Highlighted!'  // Line 4 highlighted
    }
  }
}
```
````

Multiple lines and ranges:

````md
```js{1,4,6-8}
// Line 1 highlighted
export default {
  data() {
    return {      // Line 4
      msg: 'Hi',
      foo: 'bar', // Lines 6-8
      baz: 'qux'
    }
  }
}
```
````

Inline highlighting with comment:

````md
```js
export default {
  data() {
    return {
      msg: 'Highlighted!' // [!code highlight]
    }
  }
}
```
````

## Focus

Blur other code and focus specific lines:

````md
```js
export default {
  data() {
    return {
      msg: 'Focused!' // [!code focus]
    }
  }
}
```
````

Focus multiple lines:

```js
// [!code focus:3]
```

## Colored diffs

Show additions and removals:

````md
```js
export default {
  data() {
    return {
      msg: 'Removed' // [!code --]
      msg: 'Added'   // [!code ++]
    }
  }
}
```
````

## Errors and warnings

Color lines as errors or warnings:

````md
```js
export default {
  data() {
    return {
      msg: 'Error',   // [!code error]
      msg: 'Warning'  // [!code warning]
    }
  }
}
```
````

## Line numbers

Enable globally:

```ts
// .vitepress/config.ts
export default {
  markdown: {
    lineNumbers: true
  }
}
```

Per-block override:

````md
```ts:line-numbers
// Line numbers enabled
const a = 1
```

```ts:no-line-numbers
// Line numbers disabled
const b = 2
```
````

Start from specific number:

````md
```ts:line-numbers=5
// Starts at line 5
const a = 1  // This is line 5
const b = 2  // This is line 6
```
````

## Code groups

Tabbed code blocks:

````md
::: code-group

```js [JavaScript]
export default { /* ... */ }
```

```ts [TypeScript]
export default defineConfig({ /* ... */ })
```

:::
````

## Import code snippets

From external files:

```md
<<< @/snippets/snippet.js
```

With highlighting:

```md
<<< @/snippets/snippet.js{2,4-6}
```

Specific region:

```md
<<< @/snippets/snippet.js#regionName{1,2}
```

With language and line numbers:

```md
<<< @/snippets/snippet.cs{1,2,4-6 c#:line-numbers}
```

In code groups:

```md
::: code-group

<<< @/snippets/config.js [JavaScript]
<<< @/snippets/config.ts [TypeScript]

:::
```

## File labels

Add filename labels to code blocks:

````md
```js [vite.config.js]
export default defineConfig({})
```
````

## Key points

- Use `// [!code highlight]` for inline highlighting
- Use `// [!code focus]` to focus with blur effect
- Use `// [!code ++]` and `// [!code --]` for diffs
- Use `// [!code error]` and `// [!code warning]` for status
- `:line-numbers` and `:no-line-numbers` control line numbers per block
- `@` in imports refers to source root
- Code groups create tabbed interfaces


---
Last updated: February 21, 2026

<!--
Source references:
- https://vitepress.dev/guide/markdown#syntax-highlighting-in-code-blocks
- https://vitepress.dev/guide/markdown#line-highlighting-in-code-blocks
-->
