#!/usr/bin/env node
/**
 * Chart Data Generator
 *
 * Generates JSON data files for the IndicatorChart component by combining
 * candle data from test quotes with indicator results from test data.
 *
 * Usage: node scripts/generate-chart-data.mjs
 */

import { readFileSync, writeFileSync, mkdirSync, existsSync, readdirSync } from 'fs'
import { join, dirname, basename } from 'path'
import { fileURLToPath } from 'url'

const __dirname = dirname(fileURLToPath(import.meta.url))
const ROOT_DIR = join(__dirname, '..')
const TEST_DATA_DIR = join(ROOT_DIR, '..', 'tests', 'indicators', '_testdata')
const QUOTES_FILE = join(TEST_DATA_DIR, 'quotes', 'default.csv')
const RESULTS_DIR = join(TEST_DATA_DIR, 'results')
const OUTPUT_DIR = join(ROOT_DIR, '.vitepress', 'public', 'data')

// Mapping from result file names to chart display names and value fields
// Format: { resultFileName: { displayName, fields: [{ name, jsonKey, type? }] } }
const INDICATOR_CONFIG = {
  'adl.standard.json': {
    displayName: 'Adl',
    fields: [{ name: 'ADL', jsonKey: 'adl', type: 'line' }]
  },
  'adx.standard.json': {
    displayName: 'Adx',
    fields: [
      { name: 'ADX', jsonKey: 'adx', type: 'line' },
      { name: '+DI', jsonKey: 'pdi', type: 'line' },
      { name: '-DI', jsonKey: 'mdi', type: 'line' }
    ]
  },
  'alligator.standard.json': {
    displayName: 'Alligator',
    fields: [
      { name: 'Jaw', jsonKey: 'jaw', type: 'line' },
      { name: 'Teeth', jsonKey: 'teeth', type: 'line' },
      { name: 'Lips', jsonKey: 'lips', type: 'line' }
    ]
  },
  'alma.standard.json': {
    displayName: 'Alma',
    fields: [{ name: 'ALMA', jsonKey: 'alma', type: 'line' }]
  },
  'aroon.standard.json': {
    displayName: 'Aroon',
    fields: [
      { name: 'Aroon Up', jsonKey: 'aroonUp', type: 'line' },
      { name: 'Aroon Down', jsonKey: 'aroonDown', type: 'line' }
    ]
  },
  'atr.standard.json': {
    displayName: 'Atr',
    fields: [{ name: 'ATR', jsonKey: 'atr', type: 'line' }]
  },
  'atr-stop.standard.json': {
    displayName: 'AtrStop',
    fields: [{ name: 'ATR Stop', jsonKey: 'atrStop', type: 'line' }]
  },
  'awesome.standard.json': {
    displayName: 'Awesome',
    fields: [{ name: 'Awesome', jsonKey: 'oscillator', type: 'histogram' }]
  },
  'bb.standard.json': {
    displayName: 'BollingerBands',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line' },
      { name: 'Middle', jsonKey: 'sma', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line' }
    ]
  },
  'beta.standard.json': {
    displayName: 'Beta',
    fields: [{ name: 'Beta', jsonKey: 'beta', type: 'line' }]
  },
  'bop.standard.json': {
    displayName: 'Bop',
    fields: [{ name: 'BOP', jsonKey: 'bop', type: 'histogram' }]
  },
  'cci.standard.json': {
    displayName: 'Cci',
    fields: [{ name: 'CCI', jsonKey: 'cci', type: 'line' }]
  },
  'chaikin-osc.standard.json': {
    displayName: 'ChaikinOsc',
    fields: [{ name: 'Chaikin', jsonKey: 'oscillator', type: 'histogram' }]
  },
  'chexit.standard.json': {
    displayName: 'Chandelier',
    fields: [
      { name: 'Long', jsonKey: 'exitLong', type: 'line' },
      { name: 'Short', jsonKey: 'exitShort', type: 'line' }
    ]
  },
  'chop.standard.json': {
    displayName: 'Chop',
    fields: [{ name: 'CHOP', jsonKey: 'chop', type: 'line' }]
  },
  'cmf.standard.json': {
    displayName: 'Cmf',
    fields: [{ name: 'CMF', jsonKey: 'cmf', type: 'line' }]
  },
  'cmo.standard.json': {
    displayName: 'Cmo',
    fields: [{ name: 'CMO', jsonKey: 'cmo', type: 'line' }]
  },
  'corr.standard.json': {
    displayName: 'Correlation',
    fields: [{ name: 'Correlation', jsonKey: 'correlation', type: 'line' }]
  },
  'crsi.standard.json': {
    displayName: 'ConnorsRsi',
    fields: [{ name: 'CRSI', jsonKey: 'connorsRsi', type: 'line' }]
  },
  'dema.standard.json': {
    displayName: 'Dema',
    fields: [{ name: 'DEMA', jsonKey: 'dema', type: 'line' }]
  },
  'donchian.standard.json': {
    displayName: 'Donchian',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line' },
      { name: 'Center', jsonKey: 'centerline', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line' }
    ]
  },
  'dpo.standard.json': {
    displayName: 'Dpo',
    fields: [{ name: 'DPO', jsonKey: 'dpo', type: 'line' }]
  },
  'dynamic.standard.json': {
    displayName: 'Dynamic',
    fields: [{ name: 'Dynamic', jsonKey: 'dynamic', type: 'line' }]
  },
  'elder-ray.standard.json': {
    displayName: 'ElderRay',
    fields: [
      { name: 'Bull Power', jsonKey: 'bullPower', type: 'histogram' },
      { name: 'Bear Power', jsonKey: 'bearPower', type: 'histogram' }
    ]
  },
  'ema.standard.json': {
    displayName: 'Ema',
    fields: [{ name: 'EMA', jsonKey: 'ema', type: 'line' }]
  },
  'epma.standard.json': {
    displayName: 'Epma',
    fields: [{ name: 'EPMA', jsonKey: 'epma', type: 'line' }]
  },
  'fcb.standard.json': {
    displayName: 'Fcb',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line' }
    ]
  },
  'fisher.standard.json': {
    displayName: 'FisherTransform',
    fields: [
      { name: 'Fisher', jsonKey: 'fisher', type: 'line' },
      { name: 'Trigger', jsonKey: 'trigger', type: 'line' }
    ]
  },
  'force.standard.json': {
    displayName: 'ForceIndex',
    fields: [{ name: 'Force', jsonKey: 'forceIndex', type: 'histogram' }]
  },
  'fractal.standard.json': {
    displayName: 'Fractal',
    fields: [
      { name: 'Bull', jsonKey: 'fractalBull', type: 'line' },
      { name: 'Bear', jsonKey: 'fractalBear', type: 'line' }
    ]
  },
  'gator.standard.json': {
    displayName: 'Gator',
    fields: [
      { name: 'Upper', jsonKey: 'upper', type: 'histogram' },
      { name: 'Lower', jsonKey: 'lower', type: 'histogram' }
    ]
  },
  'heikinashi.standard.json': {
    displayName: 'HeikinAshi',
    fields: [] // This is a candle type, not overlay
  },
  'hma.standard.json': {
    displayName: 'Hma',
    fields: [{ name: 'HMA', jsonKey: 'hma', type: 'line' }]
  },
  'htl.standard.json': {
    displayName: 'HtTrendline',
    fields: [
      { name: 'Trendline', jsonKey: 'trendline', type: 'line' },
      { name: 'SmoothPrice', jsonKey: 'smoothPrice', type: 'line' }
    ]
  },
  'hurst.standard.json': {
    displayName: 'Hurst',
    fields: [{ name: 'Hurst', jsonKey: 'hurstExponent', type: 'line' }]
  },
  'ichimoku.standard.json': {
    displayName: 'Ichimoku',
    fields: [
      { name: 'Tenkan', jsonKey: 'tenkanSen', type: 'line' },
      { name: 'Kijun', jsonKey: 'kijunSen', type: 'line' },
      { name: 'Senkou A', jsonKey: 'senkouSpanA', type: 'line' },
      { name: 'Senkou B', jsonKey: 'senkouSpanB', type: 'line' }
    ]
  },
  'kama.standard.json': {
    displayName: 'Kama',
    fields: [{ name: 'KAMA', jsonKey: 'kama', type: 'line' }]
  },
  'keltner.standard.json': {
    displayName: 'Keltner',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line' },
      { name: 'Center', jsonKey: 'centerline', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line' }
    ]
  },
  'kvo.standard.json': {
    displayName: 'Kvo',
    fields: [
      { name: 'KVO', jsonKey: 'oscillator', type: 'histogram' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'ma-env.standard.json': {
    displayName: 'MaEnvelopes',
    fields: [
      { name: 'Upper', jsonKey: 'upperEnvelope', type: 'line' },
      { name: 'Center', jsonKey: 'centerline', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerEnvelope', type: 'line' }
    ]
  },
  'macd.standard.json': {
    displayName: 'Macd',
    fields: [
      { name: 'MACD', jsonKey: 'macd', type: 'line' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' },
      { name: 'Histogram', jsonKey: 'histogram', type: 'histogram' }
    ]
  },
  'mama.standard.json': {
    displayName: 'Mama',
    fields: [
      { name: 'MAMA', jsonKey: 'mama', type: 'line' },
      { name: 'FAMA', jsonKey: 'fama', type: 'line' }
    ]
  },
  'mfi.standard.json': {
    displayName: 'Mfi',
    fields: [{ name: 'MFI', jsonKey: 'mfi', type: 'line' }]
  },
  'obv.standard.json': {
    displayName: 'Obv',
    fields: [{ name: 'OBV', jsonKey: 'obv', type: 'line' }]
  },
  'psar.standard.json': {
    displayName: 'ParabolicSar',
    fields: [{ name: 'SAR', jsonKey: 'sar', type: 'line' }]
  },
  'pivots.standard.json': {
    displayName: 'Pivots',
    fields: [
      { name: 'High Point', jsonKey: 'highPoint', type: 'line' },
      { name: 'Low Point', jsonKey: 'lowPoint', type: 'line' }
    ]
  },
  'pmo.standard.json': {
    displayName: 'Pmo',
    fields: [
      { name: 'PMO', jsonKey: 'pmo', type: 'line' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'prs.standard.json': {
    displayName: 'Prs',
    fields: [{ name: 'PRS', jsonKey: 'prs', type: 'line' }]
  },
  'pvo.standard.json': {
    displayName: 'Pvo',
    fields: [
      { name: 'PVO', jsonKey: 'pvo', type: 'histogram' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'renko.standard.json': {
    displayName: 'Renko',
    fields: [] // This is a candle type, not overlay
  },
  'roc.standard.json': {
    displayName: 'Roc',
    fields: [{ name: 'ROC', jsonKey: 'roc', type: 'line' }]
  },
  'roc-wb.standard.json': {
    displayName: 'RocWb',
    fields: [
      { name: 'ROC', jsonKey: 'roc', type: 'line' },
      { name: 'Upper', jsonKey: 'upperBand', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line' }
    ]
  },
  'rolling-pivots.standard.json': {
    displayName: 'RollingPivots',
    fields: [
      { name: 'PP', jsonKey: 'pp', type: 'line' },
      { name: 'S1', jsonKey: 's1', type: 'line' },
      { name: 'R1', jsonKey: 'r1', type: 'line' }
    ]
  },
  'rsi.standard.json': {
    displayName: 'Rsi',
    fields: [{ name: 'RSI', jsonKey: 'rsi', type: 'line' }]
  },
  'slope.standard.json': {
    displayName: 'Slope',
    fields: [{ name: 'Slope', jsonKey: 'slope', type: 'line' }]
  },
  'sma.standard.json': {
    displayName: 'Sma',
    fields: [{ name: 'SMA', jsonKey: 'sma', type: 'line' }]
  },
  'smi.standard.json': {
    displayName: 'Smi',
    fields: [
      { name: 'SMI', jsonKey: 'smi', type: 'line' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'smma.standard.json': {
    displayName: 'Smma',
    fields: [{ name: 'SMMA', jsonKey: 'smma', type: 'line' }]
  },
  'starc.standard.json': {
    displayName: 'StarcBands',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line' },
      { name: 'Center', jsonKey: 'centerline', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line' }
    ]
  },
  'stc.standard.json': {
    displayName: 'Stc',
    fields: [{ name: 'STC', jsonKey: 'stc', type: 'line' }]
  },
  'stdev.standard.json': {
    displayName: 'StdDev',
    fields: [{ name: 'StdDev', jsonKey: 'stdDev', type: 'line' }]
  },
  'stdev-channels.standard.json': {
    displayName: 'StdDevChannels',
    fields: [
      { name: 'Upper', jsonKey: 'upperChannel', type: 'line' },
      { name: 'Center', jsonKey: 'centerline', type: 'line' },
      { name: 'Lower', jsonKey: 'lowerChannel', type: 'line' }
    ]
  },
  'stoch.standard.json': {
    displayName: 'Stoch',
    fields: [
      { name: '%K', jsonKey: 'k', type: 'line' },
      { name: '%D', jsonKey: 'd', type: 'line' }
    ]
  },
  'stoch-rsi.standard.json': {
    displayName: 'StochRsi',
    fields: [
      { name: 'StochRSI', jsonKey: 'stochRsi', type: 'line' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'supertrend.standard.json': {
    displayName: 'SuperTrend',
    fields: [{ name: 'SuperTrend', jsonKey: 'superTrend', type: 'line' }]
  },
  't3.standard.json': {
    displayName: 'T3',
    fields: [{ name: 'T3', jsonKey: 't3', type: 'line' }]
  },
  'tema.standard.json': {
    displayName: 'Tema',
    fields: [{ name: 'TEMA', jsonKey: 'tema', type: 'line' }]
  },
  'trix.standard.json': {
    displayName: 'Trix',
    fields: [
      { name: 'TRIX', jsonKey: 'trix', type: 'line' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'tsi.standard.json': {
    displayName: 'Tsi',
    fields: [
      { name: 'TSI', jsonKey: 'tsi', type: 'line' },
      { name: 'Signal', jsonKey: 'signal', type: 'line' }
    ]
  },
  'ulcer.standard.json': {
    displayName: 'UlcerIndex',
    fields: [{ name: 'Ulcer', jsonKey: 'ulcerIndex', type: 'line' }]
  },
  'uo.standard.json': {
    displayName: 'Ultimate',
    fields: [{ name: 'Ultimate', jsonKey: 'ultimate', type: 'line' }]
  },
  'vol-stop.standard.json': {
    displayName: 'VolatilityStop',
    fields: [{ name: 'Stop', jsonKey: 'stop', type: 'line' }]
  },
  'vortex.standard.json': {
    displayName: 'Vortex',
    fields: [
      { name: '+VI', jsonKey: 'pvi', type: 'line' },
      { name: '-VI', jsonKey: 'nvi', type: 'line' }
    ]
  },
  'vwap.standard.json': {
    displayName: 'Vwap',
    fields: [{ name: 'VWAP', jsonKey: 'vwap', type: 'line' }]
  },
  'vwma.standard.json': {
    displayName: 'Vwma',
    fields: [{ name: 'VWMA', jsonKey: 'vwma', type: 'line' }]
  },
  'willr.standard.json': {
    displayName: 'WilliamsR',
    fields: [{ name: 'Williams %R', jsonKey: 'williamsR', type: 'line' }]
  },
  'wma.standard.json': {
    displayName: 'Wma',
    fields: [{ name: 'WMA', jsonKey: 'wma', type: 'line' }]
  },
  'zigzag.standard.json': {
    displayName: 'ZigZag',
    fields: [{ name: 'ZigZag', jsonKey: 'zigZag', type: 'line' }]
  }
}

/**
 * Parse CSV file into array of quote objects
 */
function parseQuotesCsv(csvContent) {
  const lines = csvContent.trim().split('\n')
  const headers = lines[0].split(',')

  return lines.slice(1).map(line => {
    const values = line.split(',')
    return {
      timestamp: new Date(values[0]).toISOString(),
      open: parseFloat(values[1]),
      high: parseFloat(values[2]),
      low: parseFloat(values[3]),
      close: parseFloat(values[4]),
      volume: parseFloat(values[5]) || 0
    }
  })
}

/**
 * Generate chart data JSON for a specific indicator
 */
function generateChartData(quotes, results, config) {
  // Create timestamp lookup for quotes
  const quoteLookup = new Map()
  quotes.forEach(q => {
    const dateKey = q.timestamp.split('T')[0]
    quoteLookup.set(dateKey, q)
  })

  // Find overlapping timestamps
  const resultTimestamps = results.map(r => r.timestamp.split('T')[0])
  const overlappingQuotes = []

  for (const ts of resultTimestamps) {
    if (quoteLookup.has(ts)) {
      overlappingQuotes.push(quoteLookup.get(ts))
    }
  }

  // Build series data
  const series = config.fields.map(field => {
    return {
      name: field.name,
      type: field.type || 'line',
      data: results.map(r => ({
        timestamp: r.timestamp,
        value: r[field.jsonKey] ?? null
      }))
    }
  })

  return {
    metadata: {
      symbol: 'S&P 500',
      timeframe: 'Daily',
      indicator: config.displayName
    },
    candles: overlappingQuotes,
    series: series
  }
}

/**
 * Main entry point
 */
function main() {
  console.log('Stock Indicators Chart Data Generator')
  console.log('=====================================\n')

  // Create output directory
  if (!existsSync(OUTPUT_DIR)) {
    mkdirSync(OUTPUT_DIR, { recursive: true })
    console.log(`Created output directory: ${OUTPUT_DIR}`)
  }

  // Load quotes
  console.log(`Loading quotes from: ${QUOTES_FILE}`)
  const quotesContent = readFileSync(QUOTES_FILE, 'utf-8')
  const quotes = parseQuotesCsv(quotesContent)
  console.log(`Loaded ${quotes.length} quotes\n`)

  // Process each configured indicator
  let generated = 0
  let skipped = 0

  for (const [resultFile, config] of Object.entries(INDICATOR_CONFIG)) {
    const resultPath = join(RESULTS_DIR, resultFile)

    // Skip if result file doesn't exist
    if (!existsSync(resultPath)) {
      console.log(`⚠ Skipping ${config.displayName}: result file not found`)
      skipped++
      continue
    }

    // Skip indicators without fields (candle types)
    if (config.fields.length === 0) {
      console.log(`⚠ Skipping ${config.displayName}: no overlay fields defined`)
      skipped++
      continue
    }

    try {
      // Load results
      const resultsContent = readFileSync(resultPath, 'utf-8')
      const results = JSON.parse(resultsContent)

      // Generate chart data
      const chartData = generateChartData(quotes, results, config)

      // Write output file
      const outputFile = join(OUTPUT_DIR, `${config.displayName}.json`)
      writeFileSync(outputFile, JSON.stringify(chartData, null, 2))
      console.log(`✓ Generated: ${config.displayName}.json (${chartData.candles.length} candles, ${chartData.series.length} series)`)
      generated++
    } catch (error) {
      console.error(`✗ Error processing ${config.displayName}:`, error.message)
      skipped++
    }
  }

  console.log(`\n=====================================`)
  console.log(`Generated: ${generated} files`)
  console.log(`Skipped: ${skipped} files`)
  console.log(`Output: ${OUTPUT_DIR}`)
}

main()
