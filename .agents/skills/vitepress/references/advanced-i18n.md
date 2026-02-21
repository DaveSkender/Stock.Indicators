---
name: vitepress-internationalization
description: Setting up multi-language sites with locale configuration and RTL support
---

# Internationalization

VitePress supports multi-language sites through locale configuration.

## Directory structure

Organize content by locale:

```plaintext
docs/
├─ en/
│  ├─ guide.md
│  └─ index.md
├─ zh/
│  ├─ guide.md
│  └─ index.md
└─ fr/
   ├─ guide.md
   └─ index.md
```

Or with root as default language:

```plaintext
docs/
├─ guide.md        # English (root)
├─ index.md
├─ zh/
│  ├─ guide.md
│  └─ index.md
└─ fr/
   ├─ guide.md
   └─ index.md
```

## Configuration

```ts
// .vitepress/config.ts
import { defineConfig } from 'vitepress'

export default defineConfig({
  locales: {
    root: {
      label: 'English',
      lang: 'en'
    },
    zh: {
      label: '简体中文',
      lang: 'zh-CN',
      link: '/zh/'
    },
    fr: {
      label: 'Français',
      lang: 'fr',
      link: '/fr/'
    }
  }
})
```

## Locale-specific config

Override site config per locale:

```ts
locales: {
  root: {
    label: 'English',
    lang: 'en',
    title: 'My Docs',
    description: 'Documentation site',
    themeConfig: {
      nav: [
        { text: 'Guide', link: '/guide/' }
      ],
      sidebar: {
        '/guide/': [
          { text: 'Introduction', link: '/guide/' }
        ]
      }
    }
  },
  zh: {
    label: '简体中文',
    lang: 'zh-CN',
    link: '/zh/',
    title: '我的文档',
    description: '文档站点',
    themeConfig: {
      nav: [
        { text: '指南', link: '/zh/guide/' }
      ],
      sidebar: {
        '/zh/guide/': [
          { text: '介绍', link: '/zh/guide/' }
        ]
      }
    }
  }
}
```

## Locale-specific properties

Each locale can override:

```ts
interface LocaleSpecificConfig {
  lang?: string
  dir?: string              // 'ltr' or 'rtl'
  title?: string
  titleTemplate?: string | boolean
  description?: string
  head?: HeadConfig[]       // Merged with existing
  themeConfig?: ThemeConfig // Shallow merged
}
```

## Search i18n

### Local search

```ts
themeConfig: {
  search: {
    provider: 'local',
    options: {
      locales: {
        zh: {
          translations: {
            button: {
              buttonText: '搜索',
              buttonAriaLabel: '搜索'
            },
            modal: {
              noResultsText: '没有结果',
              resetButtonTitle: '重置搜索',
              footer: {
                selectText: '选择',
                navigateText: '导航',
                closeText: '关闭'
              }
            }
          }
        }
      }
    }
  }
}
```

### Algolia Search

```ts
themeConfig: {
  search: {
    provider: 'algolia',
    options: {
      appId: '...',
      apiKey: '...',
      indexName: '...',
      locales: {
        zh: {
          placeholder: '搜索文档',
          translations: {
            button: { buttonText: '搜索文档' }
          }
        }
      }
    }
  }
}
```

## Separate locale directories

For fully separated locales without root fallback:

```plaintext
docs/
├─ en/
│  └─ index.md
├─ zh/
│  └─ index.md
└─ fr/
   └─ index.md
```

Requires server redirect for `/` → `/en/`. Netlify example:

```plaintext
/* /en/:splat 302 Language=en
/* /zh/:splat 302 Language=zh
/* /en/:splat 302
```

## Persisting language choice

Set cookie on language change:

```vue
<!-- .vitepress/theme/Layout.vue -->
<script setup>
import DefaultTheme from 'vitepress/theme'
import { useData, inBrowser } from 'vitepress'
import { watchEffect } from 'vue'

const { lang } = useData()

watchEffect(() => {
  if (inBrowser) {
    document.cookie = `nf_lang=${lang.value}; expires=Mon, 1 Jan 2030 00:00:00 UTC; path=/`
  }
})
</script>

<template>
  <DefaultTheme.Layout />
</template>
```

## RTL support (experimental)

For right-to-left languages:

```ts
locales: {
  ar: {
    label: 'العربية',
    lang: 'ar',
    dir: 'rtl'
  }
}
```

Requires PostCSS plugin like `postcss-rtlcss`:

```ts
// postcss.config.js
import rtlcss from 'postcss-rtlcss'

export default {
  plugins: [
    rtlcss({
      ltrPrefix: ':where([dir="ltr"])',
      rtlPrefix: ':where([dir="rtl"])'
    })
  ]
}
```

## Organizing config

Split config into separate files:

```
.vitepress/
├─ config/
│  ├─ index.ts      # Main config, merges locales
│  ├─ en.ts         # English config
│  ├─ zh.ts         # Chinese config
│  └─ shared.ts     # Shared config
```

```ts
// .vitepress/config/index.ts
import { defineConfig } from 'vitepress'
import { shared } from './shared'
import { en } from './en'
import { zh } from './zh'

export default defineConfig({
  ...shared,
  locales: {
    root: { label: 'English', ...en },
    zh: { label: '简体中文', ...zh }
  }
})
```

## Key points

- Use `locales` object in config with `root` for default language
- Each locale can override title, description, and themeConfig
- `themeConfig` is shallow merged (define complete nav/sidebar per locale)
- Don't override `themeConfig.algolia` at locale level
- `dir: 'rtl'` enables RTL with PostCSS plugin
- Language switcher appears automatically in nav

---
Last updated: February 21, 2026

<!--
Source references:
- https://vitepress.dev/guide/i18n
-->
