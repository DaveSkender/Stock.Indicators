import path from 'path'
import { fileURLToPath } from 'url'
import { defineConfig } from 'vitepress'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "stock indicators",
  description: "Transform price quotes into trading insights.",

  // Default to dark theme (toggle still available)
  appearance: 'dark',

  sitemap: {
    hostname: 'https://dotnet.stockindicators.dev'
  },

  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }],
    ['link', { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' }],
    ['link', { rel: 'apple-touch-icon', sizes: '180x180', href: '/assets/icons/apple-touch-icon.png' }],
    ['link', { rel: 'icon', type: 'image/png', sizes: '32x32', href: '/assets/icons/favicon-32x32.png' }],
    ['link', { rel: 'icon', type: 'image/png', sizes: '16x16', href: '/assets/icons/favicon-16x16.png' }],
    ['link', { rel: 'manifest', href: '/assets/manifest.json' }],
    ['meta', { name: 'theme-color', content: '#22272e' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:title', content: 'Stock Indicators for .NET' }],
    ['meta', { property: 'og:description', content: 'Transform price quotes into trading insights.' }],
    ['meta', { property: 'og:image', content: '/assets/social-banner.png' }],
    ['meta', { name: 'twitter:card', content: 'summary' }],
    ['meta', { name: 'twitter:site', content: '@daveskender' }],
  ],

  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    logo: {
      src: '/favicon.svg',
      alt: 'Stock Indicators for .NET'
    },

    nav: [
      { text: 'Home', link: '/' },
      { text: 'Indicators', link: '/indicators' },
      { text: 'Guide', link: '/guide' },
      {
        text: 'More',
        items: [
          { text: 'Migration (v2→v3)', link: '/migration' },
          { text: 'v2 Docs', link: 'https://dotnet.stockindicators.dev' }
        ]
      }
    ],

    sidebar: {
      '/features': [
        {
          items:[
            { text: 'Getting started', link: '/guide' },
            {
              text: 'Features',
              items: [
                { text: 'Overview', link: '/features/' },
                { text: 'Batch (Series)', link: '/features/batch' },
                { text: 'Buffer lists', link: '/features/buffer' },
                { text: 'Stream hubs', link: '/features/stream' },
                { text: 'Utilities', link: '/utilities/' },
              ]
            },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Examples', link: '/examples/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
           ]
        }
      ],
      '/guide': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Examples', link: '/examples/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        }
      ],
      '/utilities': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Examples', link: '/examples/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        },
        {
          text: 'Utilities',
          items: [
            { text: 'Overview', link: '/utilities/' },
            { text: 'Quote utilities', link: '/utilities/quotes' },
            { text: 'Result utilities', link: '/utilities/results' },
            { text: 'Helper utilities', link: '/utilities/helpers' },
            { text: 'Indicator catalog', link: '/utilities/catalog' },
          ]
        }
      ],
      '/examples': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Examples', link: '/examples/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        },
        {
          text: 'Examples',
          items: [
            { text: 'Getting started', link: '/examples/' },
          ]
        }
      ],
      '/performance': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        }
      ],
      '/migration': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        }
      ],
      '/contributing': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        }
      ],
      '/about': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        }
      ],
      '/indicators': [
        {
          text: 'Documentation',
          items: [
            { text: 'Getting started', link: '/guide' },
            { text: 'Features', link: '/features/' },
            { text: 'Indicators', link: '/indicators' },
            { text: 'Utilities', link: '/utilities/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Migration (v2→v3)', link: '/migration' },
            { text: 'Contributing', link: '/contributing' },
            { text: 'About', link: '/about' },
          ]
        },
        {
          text: 'Price trends',
          collapsed: true,
          items: [
            { text: 'Average Directional Index (ADX)', link: '/indicators/Adx' },
            { text: 'Aroon Indicator', link: '/indicators/Aroon' },
            { text: 'ATR Trailing Stop', link: '/indicators/AtrStop' },
            { text: 'Directional Movement Index (DMI)', link: '/indicators/Adx' },
            { text: 'Elder-ray Index', link: '/indicators/ElderRay' },
            { text: 'Ichimoku Cloud', link: '/indicators/Ichimoku' },
            { text: 'Moving Average Convergence Divergence', link: '/indicators/Macd' },
            { text: 'Pivot Points', link: '/indicators/PivotPoints' },
            { text: 'Rate of Change with Bands', link: '/indicators/RocWb' },
            { text: 'Rolling Pivots', link: '/indicators/RollingPivots' },
            { text: 'SuperTrend', link: '/indicators/SuperTrend' },
            { text: 'Vortex Indicator', link: '/indicators/Vortex' },
            { text: 'Williams Alligator', link: '/indicators/Alligator' },
          ]
        },
        {
          text: 'Price channels',
          collapsed: true,
          items: [
            { text: 'Bollinger Bands®', link: '/indicators/BollingerBands' },
            { text: 'Donchian Channels', link: '/indicators/Donchian' },
            { text: 'Fractal Chaos Bands', link: '/indicators/Fcb' },
            { text: 'Keltner Channels', link: '/indicators/Keltner' },
            { text: 'Moving Average Envelopes', link: '/indicators/MaEnvelopes' },
            { text: 'Pivot Points', link: '/indicators/PivotPoints' },
            { text: 'Price Channels', link: '/indicators/Donchian' },
            { text: 'Rolling Pivot Points', link: '/indicators/RollingPivots' },
            { text: 'STARC Bands', link: '/indicators/StarcBands' },
            { text: 'Standard Deviation Channels', link: '/indicators/StdDevChannels' },
          ]
        },
        {
          text: 'Oscillators',
          collapsed: true,
          items: [
            { text: 'Awesome Oscillator', link: '/indicators/Awesome' },
            { text: 'Chande Momentum Oscillator', link: '/indicators/Cmo' },
            { text: 'Commodity Channel Index', link: '/indicators/Cci' },
            { text: 'ConnorsRSI', link: '/indicators/ConnorsRsi' },
            { text: 'Detrended Price Oscillator', link: '/indicators/Dpo' },
            { text: 'Gator Oscillator', link: '/indicators/Gator' },
            { text: 'KDJ Index', link: '/indicators/Stoch' },
            { text: 'Price Momentum Oscillator', link: '/indicators/Pmo' },
            { text: 'Relative Strength Index', link: '/indicators/Rsi' },
            { text: 'Schaff Trend Cycle', link: '/indicators/Stc' },
            { text: 'Stochastic Momentum Index', link: '/indicators/Smi' },
            { text: 'Stochastic Oscillator', link: '/indicators/Stoch' },
            { text: 'Stochastic RSI', link: '/indicators/StochRsi' },
            { text: 'Triple EMA Oscillator (TRIX)', link: '/indicators/Trix' },
            { text: 'True Strength Index', link: '/indicators/Tsi' },
            { text: 'Ultimate Oscillator', link: '/indicators/Ultimate' },
            { text: 'Williams Percent Range (%R)', link: '/indicators/WilliamsR' },
          ]
        },
        {
          text: 'Stop and reverse',
          collapsed: true,
          items: [
            { text: 'ATR Trailing Stop', link: '/indicators/AtrStop' },
            { text: 'Chandelier Exit', link: '/indicators/Chandelier' },
            { text: 'Parabolic SAR', link: '/indicators/ParabolicSar' },
            { text: 'SuperTrend', link: '/indicators/SuperTrend' },
            { text: 'Volatility Stop', link: '/indicators/VolatilityStop' },
          ]
        },
        {
          text: 'Candlestick Patterns',
          collapsed: true,
          items: [
            { text: 'Doji', link: '/indicators/Doji' },
            { text: 'Marubozu', link: '/indicators/Marubozu' },
            {
              text: 'Other price patterns',
              collapsed: false,
              items: [
                { text: 'Pivots', link: '/indicators/Pivots' },
                { text: 'Williams Fractal', link: '/indicators/Fractal' },
              ]
            }
          ],
      },
      {
        text: 'Volume based',
        collapsed: true,
        items: [
          { text: 'Accumulation Distribution Line', link: '/indicators/Adl' },
          { text: 'Chaikin Money Flow', link: '/indicators/Cmf' },
          { text: 'Chaikin Oscillator', link: '/indicators/ChaikinOsc' },
          { text: 'Force Index', link: '/indicators/ForceIndex' },
          { text: 'Klinger Volume Oscillator', link: '/indicators/Kvo' },
          { text: 'Money Flow Index', link: '/indicators/Mfi' },
          { text: 'On-Balance Volume', link: '/indicators/Obv' },
          { text: 'Price Volume Oscillator', link: '/indicators/Pvo' },
          { text: 'Volume Weighted Average Price', link: '/indicators/Vwap' },
          { text: 'Volume Weighted Moving Average', link: '/indicators/Vwma' },

        ]
      },
        {
          text: 'Moving averages',
          collapsed: true,
          items: [
            { text: 'Arnaud Legoux Moving Average', link: '/indicators/Alma' },
            { text: 'Double Exponential Moving Average', link: '/indicators/Dema' },
            { text: 'Endpoint Moving Average', link: '/indicators/Epma' },
            { text: 'Exponential Moving Average', link: '/indicators/Ema' },
            { text: 'Hilbert Transform Instantaneous Trendline', link: '/indicators/HtTrendline' },
            { text: 'Hull Moving Average', link: '/indicators/Hma' },
            { text: 'Kaufman\'s Adaptive Moving Average', link: '/indicators/Kama' },
            { text: 'Least Squares Moving Average (LSMA)', link: '/indicators/Epma' },
            { text: 'MESA Adaptive Moving Average', link: '/indicators/Mama' },
            { text: 'McGinley Dynamic', link: '/indicators/Dynamic' },
            { text: 'Modified Moving Average (MMA)', link: '/indicators/Smma' },
            { text: 'Running Moving Average (RMA)', link: '/indicators/Smma' },
            { text: 'Simple Moving Average', link: '/indicators/Sma' },
            { text: 'Smoothed Moving Average', link: '/indicators/Smma' },
            { text: 'T3 Moving Average', link: '/indicators/T3' },
            { text: 'Triple Exponential Moving Average', link: '/indicators/Tema' },
            { text: 'Volume Weighted Average Price', link: '/indicators/Vwap' },
            { text: 'Volume Weighted Moving Average', link: '/indicators/Vwma' },
            { text: 'Weighted Moving Average', link: '/indicators/Wma' },
          ]
        },
        {
          text: 'Price transforms',
          collapsed: true,
          items: [
            { text: 'Basic quote transforms', link: '/indicators/QuotePart' },
            { text: 'Ehlers Fisher Transform', link: '/indicators/FisherTransform' },
            { text: 'Heikin Ashi', link: '/indicators/HeikinAshi' },
            { text: 'HL2, HLC3, OC2, OHL3, OHLC4', link: '/indicators/QuotePart' },
            { text: 'Renko Charts', link: '/indicators/Renko' },
            { text: 'ZigZag', link: '/indicators/ZigZag' },
          ]
        },
        {
          text: 'Price characteristics',
          collapsed: true,
          items: [
            { text: 'Average True Range', link: '/indicators/Atr' },
            { text: 'Balance of Power', link: '/indicators/Bop' },
            { text: 'Bull and Bear Power', link: '/indicators/ElderRay' },
            { text: 'Choppiness Index', link: '/indicators/Chop' },
            { text: 'Dominant Cycle Periods', link: '/indicators/HtTrendline' },
            { text: 'Historical Volatility (HV)', link: '/indicators/StdDev' },
            { text: 'Hurst Exponent', link: '/indicators/Hurst' },
            { text: 'Momentum Oscillator (MO)', link: '/indicators/Roc' },
            { text: 'Normalized Average True Range', link: '/indicators/Atr' },
            { text: 'Price Momentum Oscillator (PMO)', link: '/indicators/Pmo' },
            { text: 'Price Relative Strength (PRS)', link: '/indicators/Prs' },
            { text: 'Rate of Change (ROC)', link: '/indicators/Roc' },
            { text: 'ROC with Bands', link: '/indicators/RocWb' },
            { text: 'Rescaled Range Analysis', link: '/indicators/Hurst' },
            { text: 'True Range (TR)', link: '/indicators/Atr' },
            { text: 'True Strength Index (TSI)', link: '/indicators/Tsi' },
            { text: 'Ulcer Index (UI)', link: '/indicators/UlcerIndex' },
          ]
        },
        {
          text: 'Numerical analysis',
          collapsed: true,
          items: [
            { text: 'Beta', link: '/indicators/Beta' },
            { text: 'Correlation Coefficient', link: '/indicators/Correlation' },
            { text: 'Linear Regression (best-fit line)', link: '/indicators/Slope' },
            { text: 'Mean absolute deviation', link: '/indicators/Sma#analysis' },
            { text: 'Mean absolute percentage error', link: '/indicators/Sma#analysis' },
            { text: 'Mean square error', link: '/indicators/Sma#analysis' },
            { text: 'R-Squared (R²) Coefficient of Determination', link: '/indicators/Correlation' },
            { text: 'Slope and linear regression', link: '/indicators/Slope' },
            { text: 'Standard Deviation', link: '/indicators/StdDev' },
            { text: 'Z-Score', link: '/indicators/StdDev' },
          ]
        }
      ],
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/DaveSkender/Stock.Indicators' },
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

  // Allow specific dead links that are expected (legacy Jekyll templates, etc.)
  ignoreDeadLinks: [
    /\.\.\/src\/_common\/README/,
    /%7B%7Bsite\.github\.repository_url%7D%7D/,
    /\.\.\/\.\.\/tools\/performance\/baselines\/PERFORMANCE_REVIEW/
  ],

  // Redirect old URLs to new locations
  rewrites: {
    // Legacy routes
    'indicators/BasicQuote': 'indicators/QuotePart',
  },

  vite: {
    publicDir: path.resolve(__dirname, 'public'),
    server: {
      fs: {
        allow: ['..']
      }
    },
    ssr: {
      noExternal: true
    },
    build: {
      rollupOptions: {
        output: {
          assetFileNames: 'assets/[name].[hash][extname]'
        }
      }
    }
  },

  // Exclude legacy Jekyll directories and build artifacts
  srcExclude: [
    'vendor/**',
    '.bundle/**',
    '_site/**',
    '_layouts/**',
    '_includes/**',
    '_data/**',
    'pages/**',
    '_indicators/**',
    'examples/Backtest/**',
    'examples/ConsoleApp/**',
    'examples/CustomIndicatorsUsage/**',
    'examples/UseQuoteApi/**',
    'examples/**/*.{sln,csproj,cs,json,png,zip,editorconfig}',
    'plans/**',
    'Gemfile*',
    '.pa11yci',
    '.offline/**',
    '_headers',
    'README.md',
    'AGENTS.md',
    'PRINCIPLES.md'
   ]
})
