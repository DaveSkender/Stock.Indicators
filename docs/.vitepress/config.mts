import { defineConfig } from 'vitepress'
import path from 'path'
import handleAssetPaths from './plugins/handleAssetPaths.mts'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "Stock Indicators for .NET",
  description: "Transform price quotes into trading insights.",
  
  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }],
    ['link', { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' }],
    ['link', { rel: 'apple-touch-icon', sizes: '180x180', href: '/assets/icons/apple-touch-icon.png' }],
    ['link', { rel: 'icon', type: 'image/png', sizes: '32x32', href: '/assets/icons/favicon-32x32.png' }],
    ['link', { rel: 'icon', type: 'image/png', sizes: '16x16', href: '/assets/icons/favicon-16x16.png' }],
    ['link', { rel: 'manifest', href: '/assets/manifest.json' }],
    ['meta', { name: 'theme-color', content: '#159957' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:title', content: 'Stock Indicators for .NET' }],
    ['meta', { property: 'og:description', content: 'Transform price quotes into trading insights.' }],
    ['meta', { property: 'og:image', content: '/assets/social-banner.png' }],
    ['meta', { name: 'twitter:card', content: 'summary' }],
    ['meta', { name: 'twitter:site', content: '@daveskender' }],
  ],

  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    logo: '/assets/icons/android-chrome-192x192.png',
    
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Guide', link: '/guide' },
      { text: 'Indicators', link: '/indicators' },
      { text: 'Utilities', link: '/utilities' },
      { text: 'Examples', link: '/examples/' },
    ],

    sidebar: {
      '/guide': [
        {
          text: 'Guide and Pro Tips',
          items: [
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        }
      ],
      '/indicators': [
        {
          text: 'Indicators',
          items: [
            { text: 'Overview', link: '/indicators' },
          ]
        }
      ],
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/DaveSkender/Stock.Indicators' },
      { icon: 'twitter', link: 'https://twitter.com/daveskender' },
      { icon: 'linkedin', link: 'https://www.linkedin.com/in/skender' },
    ],

    footer: {
      message: 'Licensed under Apache 2.0',
      copyright: 'Copyright © Dave Skender'
    },

    search: {
      provider: 'local'
    },

    editLink: {
      pattern: 'https://github.com/DaveSkender/Stock.Indicators/edit/main/docs/:path',
      text: 'Edit this page on GitHub'
    },

    lastUpdated: {
      text: 'Last updated',
      formatOptions: {
        dateStyle: 'short',
        timeStyle: 'short'
      }
    }
  },

  srcDir: '.',
  outDir: '.vitepress/dist',
  
  cleanUrls: true,
  
  ignoreDeadLinks: true,
  
  vite: {
    plugins: [handleAssetPaths()],
    server: {
      fs: {
        allow: ['..']
      }
    },
    ssr: {
      noExternal: ['**']
    },
    build: {
      rollupOptions: {
        output: {
          assetFileNames: 'assets/[name].[hash][extname]'
        }
      }
    }
  },
  
  // Exclude Jekyll and build directories
  srcExclude: [
    'vendor/**',
    '.bundle/**',
    '_site/**',
    '_layouts/**',
    '_includes/**',
    '_data/**',
    'pages/**',
    '_indicators/**',
    'Gemfile*',
    '.pa11yci',
    '.offline/**',
    '_headers',
    'examples/**'
  ],
  
  markdown: {
    theme: {
      light: 'github-light',
      dark: 'github-dark'
    },
    config: (md) => {
      // Override image rendering to use HTML instead of imports
      const defaultImageRender = md.renderer.rules.image || function(tokens, idx, options, env, self) {
        return self.renderToken(tokens, idx, options)
      }
      
      md.renderer.rules.image = function (tokens, idx, options, env, self) {
        const token = tokens[idx]
        const srcIndex = token.attrIndex('src')
        if (srcIndex >= 0) {
          const src = token.attrs[srcIndex][1]
          // If it's an absolute path starting with /assets/, keep it as-is
          if (src.startsWith('/assets/')) {
            const alt = token.content
            return `<img src="${src}" alt="${alt}" />`
          }
        }
        return defaultImageRender(tokens, idx, options, env, self)
      }
    }
  }
})
