import fs from 'fs'
import path from 'path'
import { defineConfig } from 'vitepress'

const publicDirPath = path.resolve(__dirname, 'public')
const distDirPath = path.resolve(__dirname, 'dist')

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "Stock Indicators for .NET",
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
    logo: '/favicon.svg',

    nav: [
      { text: 'Home', link: '/' },
      { text: 'Guide', link: '/guide' },
      { text: 'Indicators', link: '/indicators' },
      { text: 'Utilities', link: '/utilities' },
      // TODO: Examples page needs VitePress conversion - currently excluded in srcExclude
      // { text: 'Examples', link: '/examples/' },
    ],

    sidebar: {
      '/guide': [
        {
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        }
      ],
      '/utilities': [
        {
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        }
      ],
      '/performance': [
        {
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        }
      ],
      '/contributing': [
        {
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        }
      ],
      '/indicators': [
        {
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        },
        {
          text: 'Indicators',
          items: [
            { text: 'Overview', link: '/indicators' },
          ]
        },
        {
          text: 'Moving averages',
          collapsed: true,
          items: [
            { text: 'Arnaud Legoux Moving Average', link: '/indicators/Alma' },
            { text: 'Double Exponential Moving Average', link: '/indicators/Dema' },
            { text: 'McGinley Dynamic', link: '/indicators/Dynamic' },
            { text: 'Exponential Moving Average', link: '/indicators/Ema' },
            { text: 'Endpoint Moving Average', link: '/indicators/Epma' },
            { text: 'Hull Moving Average', link: '/indicators/Hma' },
            { text: 'Hilbert Transform Instantaneous Trendline', link: '/indicators/HtTrendline' },
            { text: 'Kaufman\'s Adaptive Moving Average', link: '/indicators/Kama' },
            { text: 'MESA Adaptive Moving Average', link: '/indicators/Mama' },
            { text: 'Simple Moving Average', link: '/indicators/Sma' },
            { text: 'Smoothed Moving Average', link: '/indicators/Smma' },
            { text: 'T3 Moving Average', link: '/indicators/T3' },
            { text: 'Triple Exponential Moving Average', link: '/indicators/Tema' },
            { text: 'Volume Weighted Moving Average', link: '/indicators/Vwma' },
            { text: 'Weighted Moving Average', link: '/indicators/Wma' },
          ]
        },
        {
          text: 'Oscillators',
          collapsed: true,
          items: [
            { text: 'Awesome Oscillator', link: '/indicators/Awesome' },
            { text: 'Balance of Power', link: '/indicators/Bop' },
            { text: 'Commodity Channel Index', link: '/indicators/Cci' },
            { text: 'Choppiness Index', link: '/indicators/Chop' },
            { text: 'Chande Momentum Oscillator', link: '/indicators/Cmo' },
            { text: 'ConnorsRSI', link: '/indicators/ConnorsRsi' },
            { text: 'Correlation Coefficient', link: '/indicators/Correlation' },
            { text: 'Detrended Price Oscillator', link: '/indicators/Dpo' },
            { text: 'Gator Oscillator', link: '/indicators/Gator' },
            { text: 'Price Momentum Oscillator', link: '/indicators/Pmo' },
            { text: 'Rate of Change', link: '/indicators/Roc' },
            { text: 'Relative Strength Index', link: '/indicators/Rsi' },
            { text: 'Stochastic Momentum Index', link: '/indicators/Smi' },
            { text: 'Schaff Trend Cycle', link: '/indicators/Stc' },
            { text: 'Stochastic Oscillator', link: '/indicators/Stoch' },
            { text: 'Stochastic RSI', link: '/indicators/StochRsi' },
            { text: 'TRIX', link: '/indicators/Trix' },
            { text: 'True Strength Index', link: '/indicators/Tsi' },
            { text: 'Ultimate Oscillator', link: '/indicators/Ultimate' },
            { text: 'Williams Percent Range', link: '/indicators/WilliamsR' },
          ]
        },
        {
          text: 'Price channels',
          collapsed: true,
          items: [
            { text: 'Bollinger Bands', link: '/indicators/BollingerBands' },
            { text: 'Donchian Channels', link: '/indicators/Donchian' },
            { text: 'Fractal Chaos Bands', link: '/indicators/Fcb' },
            { text: 'Keltner Channels', link: '/indicators/Keltner' },
            { text: 'Moving Average Envelopes', link: '/indicators/MaEnvelopes' },
            { text: 'STARC Bands', link: '/indicators/StarcBands' },
            { text: 'Standard Deviation Channels', link: '/indicators/StdDevChannels' },
            { text: 'Volume Weighted Average Price', link: '/indicators/Vwap' },
          ]
        },
        {
          text: 'Price trends',
          collapsed: true,
          items: [
            { text: 'Average Directional Index (ADX/DMI)', link: '/indicators/Adx' },
            { text: 'Williams Alligator', link: '/indicators/Alligator' },
            { text: 'Aroon Indicator', link: '/indicators/Aroon' },
            { text: 'ATR Trailing Stop', link: '/indicators/AtrStop' },
            { text: 'Chandelier Exit', link: '/indicators/Chandelier' },
            { text: 'Elder-ray Index', link: '/indicators/ElderRay' },
            { text: 'Ichimoku Cloud', link: '/indicators/Ichimoku' },
            { text: 'Moving Average Convergence Divergence', link: '/indicators/Macd' },
            { text: 'Pivot Points', link: '/indicators/PivotPoints' },
            { text: 'Rate of Change with Bands', link: '/indicators/RocWb' },
            { text: 'Rolling Pivots', link: '/indicators/RollingPivots' },
            { text: 'SuperTrend', link: '/indicators/SuperTrend' },
            { text: 'Vortex Indicator', link: '/indicators/Vortex' },
          ]
        },
        {
          text: 'Stop and reverse',
          collapsed: true,
          items: [
            { text: 'Parabolic SAR', link: '/indicators/ParabolicSar' },
            { text: 'Volatility Stop', link: '/indicators/VolatilityStop' },
          ]
        },
        {
          text: 'Volume based',
          collapsed: true,
          items: [
            { text: 'Accumulation Distribution Line', link: '/indicators/Adl' },
            { text: 'Chaikin Oscillator', link: '/indicators/ChaikinOsc' },
            { text: 'Chaikin Money Flow', link: '/indicators/Cmf' },
            { text: 'Force Index', link: '/indicators/ForceIndex' },
            { text: 'Klinger Volume Oscillator', link: '/indicators/Kvo' },
            { text: 'Money Flow Index', link: '/indicators/Mfi' },
            { text: 'On-Balance Volume', link: '/indicators/Obv' },
            { text: 'Price Volume Oscillator', link: '/indicators/Pvo' },
          ]
        },
        {
          text: 'Price characteristics',
          collapsed: true,
          items: [
            { text: 'Average True Range', link: '/indicators/Atr' },
            { text: 'Beta', link: '/indicators/Beta' },
            { text: 'Hurst Exponent', link: '/indicators/Hurst' },
            { text: 'Price Relative Strength', link: '/indicators/Prs' },
            { text: 'Linear Regression Slope', link: '/indicators/Slope' },
            { text: 'Standard Deviation', link: '/indicators/StdDev' },
            { text: 'True Range', link: '/indicators/Atr' },
            { text: 'Ulcer Index', link: '/indicators/UlcerIndex' },
          ]
        },
        {
          text: 'Candlestick Patterns',
          collapsed: true,
          items: [
            { text: 'Doji', link: '/indicators/Doji' },
            { text: 'Marubozu', link: '/indicators/Marubozu' },
          ]
        },
        {
          text: 'Other price patterns',
          collapsed: true,
          items: [
            { text: 'Pivots', link: '/indicators/Pivots' },
            { text: 'Williams Fractal', link: '/indicators/Fractal' },
          ]
        },
        {
          text: 'Price transforms',
          collapsed: true,
          items: [
            { text: 'Ehlers Fisher Transform', link: '/indicators/FisherTransform' },
            { text: 'Heikin Ashi', link: '/indicators/HeikinAshi' },
            { text: 'Quote Part', link: '/indicators/QuotePart' },
            { text: 'Renko Charts', link: '/indicators/Renko' },
            { text: 'ZigZag', link: '/indicators/ZigZag' },
          ]
        },
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
  publicDir: path.resolve(__dirname, 'public'),

  cleanUrls: true,

  // Allow specific dead links that are expected (legacy Jekyll templates, etc.)
  ignoreDeadLinks: [
    /\.\.\/src\/_common\/README/,
    /%7B%7Bsite\.github\.repository_url%7D%7D/,
    /\.\.\/\.\.\/tools\/performance\/baselines\/PERFORMANCE_REVIEW/
  ],

  // Redirect old URLs to new locations (including aliases from legacy Jekyll site)
  rewrites: {
    // Legacy BasicQuote redirect
    'indicators/BasicQuote': 'indicators/QuotePart',

    // Alternative indicator names (aliases)
    'indicators/AtrTrailingStop': 'indicators/AtrStop',
    'indicators/BullAndBearPower': 'indicators/ElderRay',
    'indicators/DominantCyclePeriods': 'indicators/HtTrendline',
    'indicators/DirectionalMovementIndex': 'indicators/Adx',
    'indicators/DMI': 'indicators/Adx',
    'indicators/HistoricalVolatility': 'indicators/StdDev',
    'indicators/HV': 'indicators/StdDev',
    'indicators/HurstExponent': 'indicators/Hurst',
    'indicators/KDJ': 'indicators/Stoch',
    'indicators/KDJIndex': 'indicators/Stoch',
    'indicators/LeastSquaresMovingAverage': 'indicators/Epma',
    'indicators/LSMA': 'indicators/Epma',
    'indicators/LinearRegression': 'indicators/Slope',
    'indicators/MeanAbsoluteDeviation': 'indicators/Sma',
    'indicators/MeanSquareError': 'indicators/Sma',
    'indicators/MeanAbsolutePercentageError': 'indicators/Sma',
    'indicators/ModifiedMovingAverage': 'indicators/Smma',
    'indicators/MMA': 'indicators/Smma',
    'indicators/MomentumOscillator': 'indicators/Roc',
    'indicators/NormalizedAverageTrueRange': 'indicators/Atr',
    'indicators/HL2': 'indicators/QuotePart',
    'indicators/HLC3': 'indicators/QuotePart',
    'indicators/OC2': 'indicators/QuotePart',
    'indicators/OHL3': 'indicators/QuotePart',
    'indicators/OHLC4': 'indicators/QuotePart',
    'indicators/PriceChannels': 'indicators/Donchian',
    'indicators/RSquared': 'indicators/Correlation',
    'indicators/CoefficientOfDetermination': 'indicators/Correlation',
    'indicators/RescaledRangeAnalysis': 'indicators/Hurst',
    'indicators/RunningMovingAverage': 'indicators/Smma',
    'indicators/RMA': 'indicators/Smma',
    'indicators/TrueRange': 'indicators/Atr',
    'indicators/TR': 'indicators/Atr',
    'indicators/ZScore': 'indicators/StdDev'
  },

  vite: {
    plugins: [
      {
        // Ensure public assets (favicons, manifest, redirects) are copied to dist
        name: 'copy-public-assets',
        closeBundle() {
          if (fs.existsSync(publicDirPath)) {
            fs.cpSync(publicDirPath, distDirPath, { recursive: true, dereference: true })
          }
        }
      },
      {
        // Serve public assets during development (JSON data files, etc.)
        name: 'serve-public-assets',
        configureServer(server) {
          server.middlewares.use((req, res, next) => {
            if (req.url && req.url.startsWith('/data/') && req.url.endsWith('.json')) {
              const filePath = path.join(publicDirPath, req.url)
              if (fs.existsSync(filePath)) {
                res.setHeader('Content-Type', 'application/json')
                res.end(fs.readFileSync(filePath, 'utf-8'))
                return
              }
            }
            next()
          })
        }
      }
    ],
    server: {
      fs: {
        allow: ['..']
      }
    },
    ssr: {
      noExternal: ['**']
    },
    build: {
      copyPublicDir: true,
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
    'examples/**',
    'Gemfile*',
    '.pa11yci',
    '.offline/**',
    '_headers',
    'README.md',
    'AGENTS.md',
    'PRINCIPLES.md'
   ]
})
