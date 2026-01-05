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
    logo: {
      src: '/favicon.svg',
      alt: 'Stock Indicators for .NET'
    },

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
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Examples', link: '/examples/' },
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
            { text: 'Examples', link: '/examples/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        }
      ],
      '/examples': [
        {
          text: 'Documentation',
          items: [
            { text: 'Indicators', link: '/indicators' },
            { text: 'Getting Started', link: '/guide' },
            { text: 'Utilities', link: '/utilities' },
            { text: 'Examples', link: '/examples/' },
            { text: 'Performance', link: '/performance' },
            { text: 'Contributing', link: '/contributing' },
          ]
        },
        {
          text: 'Examples',
          items: [
            { text: 'Getting Started', link: '/examples/' },
            { text: 'Custom Indicators', link: '/examples/CustomIndicatorsLibrary/' },
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
          text: 'All Indicators (A-Z)',
          collapsed: false,
          items: [
            { text: "Accumulation / Distribution Line (ADL)", link: "/indicators/Adl" },
            { text: "Arnaud Legoux Moving Average (ALMA)", link: "/indicators/Alma" },
            { text: "Aroon", link: "/indicators/Aroon" },
            { text: "ATR Trailing Stop", link: "/indicators/AtrStop" },
            { text: "Average Directional Index (ADX)", link: "/indicators/Adx" },
            { text: "Average True Range (ATR) / True Range (TR)", link: "/indicators/Atr" },
            { text: "Awesome Oscillator (AO)", link: "/indicators/Awesome" },
            { text: "Balance of Power (BOP)", link: "/indicators/Bop" },
            { text: "Beta Coefficient", link: "/indicators/Beta" },
            { text: "Bollinger Bands&#174;", link: "/indicators/BollingerBands" },
            { text: "Chaikin Money Flow (CMF)", link: "/indicators/Cmf" },
            { text: "Chaikin Oscillator", link: "/indicators/ChaikinOsc" },
            { text: "Chande Momentum Oscillator (CMO)", link: "/indicators/Cmo" },
            { text: "Chandelier Exit", link: "/indicators/Chandelier" },
            { text: "Choppiness Index", link: "/indicators/Chop" },
            { text: "Commodity Channel Index (CCI)", link: "/indicators/Cci" },
            { text: "ConnorsRSI", link: "/indicators/ConnorsRsi" },
            { text: "Correlation Coefficient", link: "/indicators/Correlation" },
            { text: "Detrended Price Oscillator (DPO)", link: "/indicators/Dpo" },
            { text: "Doji", link: "/indicators/Doji" },
            { text: "Donchian Channels", link: "/indicators/Donchian" },
            { text: "Double Exponential Moving Average (DEMA)", link: "/indicators/Dema" },
            { text: "Ehlers Fisher Transform", link: "/indicators/FisherTransform" },
            { text: "Elder-ray Index", link: "/indicators/ElderRay" },
            { text: "Endpoint Moving Average (EPMA)", link: "/indicators/Epma" },
            { text: "Exponential Moving Average (EMA)", link: "/indicators/Ema" },
            { text: "Force Index", link: "/indicators/ForceIndex" },
            { text: "Fractal Chaos Bands (FCB)", link: "/indicators/Fcb" },
            { text: "Gator Oscillator", link: "/indicators/Gator" },
            { text: "Heikin Ashi", link: "/indicators/HeikinAshi" },
            { text: "Hilbert Transform Instantaneous Trendline", link: "/indicators/HtTrendline" },
            { text: "Hull Moving Average (HMA)", link: "/indicators/Hma" },
            { text: "Hurst Exponent", link: "/indicators/Hurst" },
            { text: "Ichimoku Cloud", link: "/indicators/Ichimoku" },
            { text: "Kaufman's Adaptive Moving Average (KAMA)", link: "/indicators/Kama" },
            { text: "Keltner Channels", link: "/indicators/Keltner" },
            { text: "Klinger Volume Oscillator (KVO)", link: "/indicators/Kvo" },
            { text: "Linear Regression Slope", link: "/indicators/Slope" },
            { text: "MESA Adaptive Moving Average (MAMA)", link: "/indicators/Mama" },
            { text: "Marubozu", link: "/indicators/Marubozu" },
            { text: "McGinley Dynamic", link: "/indicators/Dynamic" },
            { text: "Money Flow Index (MFI)", link: "/indicators/Mfi" },
            { text: "Moving Average Convergence Divergence (MACD)", link: "/indicators/Macd" },
            { text: "Moving Average Envelopes", link: "/indicators/MaEnvelopes" },
            { text: "On-Balance Volume (OBV)", link: "/indicators/Obv" },
            { text: "Parabolic SAR", link: "/indicators/ParabolicSar" },
            { text: "Pivot Points", link: "/indicators/PivotPoints" },
            { text: "Pivots", link: "/indicators/Pivots" },
            { text: "Price Momentum Oscillator (PMO)", link: "/indicators/Pmo" },
            { text: "Price Relative Strength (PRS)", link: "/indicators/Prs" },
            { text: "Price Volume Oscillator (PVO)", link: "/indicators/Pvo" },
            { text: "Quote Part", link: "/indicators/QuotePart" },
            { text: "Rate of Change (ROC)", link: "/indicators/Roc" },
            { text: "Rate of Change with Bands", link: "/indicators/RocWb" },
            { text: "Relative Strength Index (RSI)", link: "/indicators/Rsi" },
            { text: "Renko Charts", link: "/indicators/Renko" },
            { text: "Rolling Pivots", link: "/indicators/RollingPivots" },
            { text: "STARC Bands", link: "/indicators/StarcBands" },
            { text: "Schaff Trend Cycle (STC)", link: "/indicators/Stc" },
            { text: "Simple Moving Average (SMA)", link: "/indicators/Sma" },
            { text: "Smoothed Moving Average (SMMA)", link: "/indicators/Smma" },
            { text: "Standard Deviation", link: "/indicators/StdDev" },
            { text: "Standard Deviation Channels", link: "/indicators/StdDevChannels" },
            { text: "Stochastic Momentum Index (SMI)", link: "/indicators/Smi" },
            { text: "Stochastic Oscillator", link: "/indicators/Stoch" },
            { text: "Stochastic RSI", link: "/indicators/StochRsi" },
            { text: "SuperTrend", link: "/indicators/SuperTrend" },
            { text: "T3 Moving Average", link: "/indicators/T3" },
            { text: "TRIX", link: "/indicators/Trix" },
            { text: "Triple Exponential Moving Average (TEMA)", link: "/indicators/Tema" },
            { text: "True Strength Index (TSI)", link: "/indicators/Tsi" },
            { text: "Ulcer Index", link: "/indicators/UlcerIndex" },
            { text: "Ultimate Oscillator", link: "/indicators/Ultimate" },
            { text: "Volatility Stop", link: "/indicators/VolatilityStop" },
            { text: "Volume Weighted Average Price (VWAP)", link: "/indicators/Vwap" },
            { text: "Volume Weighted Moving Average (VWMA)", link: "/indicators/Vwma" },
            { text: "Vortex Indicator (VI)", link: "/indicators/Vortex" },
            { text: "Weighted Moving Average (WMA)", link: "/indicators/Wma" },
            { text: "Williams Alligator", link: "/indicators/Alligator" },
            { text: "Williams Fractal", link: "/indicators/Fractal" },
            { text: "Williams Percent Range (%R)", link: "/indicators/WilliamsR" },
            { text: "ZigZag", link: "/indicators/ZigZag" }
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
