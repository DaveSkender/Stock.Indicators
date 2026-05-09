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

// Stock.Charts color scheme (Material Design M2)
const ChartColors = {
  StandardRed: '#DD2C00',
  StandardOrange: '#EF6C00',
  StandardGreen: '#2E7D32',
  StandardBlue: '#1E88E5',
  StandardPurple: '#8E24AA',
  StandardGrayTransparent: '#9E9E9E50',
  DarkGray: '#616161CC',
  DarkGrayTransparent: '#61616110',
  ThresholdGrayTransparent: '#42424280',
  ThresholdRed: '#B71C1C70',
  ThresholdRedTransparent: '#B71C1C20',
  ThresholdGreen: '#1B5E2070',
  ThresholdGreenTransparent: '#1B5E2020'
}

// Mapping from result file names to chart display names and value fields
// Format: { resultFileName: { displayName, chartType, thresholds, fields: [{ name, jsonKey, type?, color? }] } }
const INDICATOR_CONFIG = {
  'adl.standard.json': {
    displayName: 'Adl',
    chartType: 'oscillator',
    fields: [{ name: 'ADL', jsonKey: 'adl', type: 'line', color: ChartColors.StandardBlue }]
  },
  'adx.standard.json': {
    displayName: 'Adx',
    chartType: 'oscillator',
    thresholds: [
      { value: 40, color: ChartColors.ThresholdGrayTransparent, style: 'dash' },
      { value: 20, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'ADX', jsonKey: 'adx', type: 'line', color: ChartColors.StandardBlue },
      { name: '+DI', jsonKey: 'pdi', type: 'line', color: ChartColors.StandardGreen },
      { name: '-DI', jsonKey: 'mdi', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'alligator.standard.json': {
    displayName: 'Alligator',
    chartType: 'overlay',
    fields: [
      { name: 'Jaw', jsonKey: 'jaw', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Teeth', jsonKey: 'teeth', type: 'line', color: ChartColors.StandardRed },
      { name: 'Lips', jsonKey: 'lips', type: 'line', color: ChartColors.StandardGreen }
    ]
  },
  'alma.standard.json': {
    displayName: 'Alma',
    chartType: 'overlay',
    fields: [{ name: 'ALMA', jsonKey: 'alma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'aroon.standard.json': {
    displayName: 'Aroon',
    chartType: 'oscillator',
    thresholds: [
      { value: 70, color: ChartColors.ThresholdGrayTransparent, style: 'solid' },
      { value: 50, color: ChartColors.ThresholdGrayTransparent, style: 'dash' },
      { value: 30, color: ChartColors.ThresholdGrayTransparent, style: 'solid' }
    ],
    fields: [
      { name: 'Aroon Up', jsonKey: 'aroonUp', type: 'line', color: ChartColors.StandardGreen },
      { name: 'Aroon Down', jsonKey: 'aroonDown', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'atr.standard.json': {
    displayName: 'Atr',
    chartType: 'oscillator',
    fields: [{ name: 'ATR', jsonKey: 'atr', type: 'line', color: ChartColors.StandardBlue }]
  },
  'atr-stop.standard.json': {
    displayName: 'AtrStop',
    chartType: 'overlay',
    fields: [{ name: 'ATR Stop', jsonKey: 'atrStop', type: 'dots', color: ChartColors.StandardPurple }]
  },
  'awesome.standard.json': {
    displayName: 'Awesome',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'Awesome', jsonKey: 'oscillator', type: 'baseline', color: ChartColors.StandardBlue }]
  },
  'bb.standard.json': {
    displayName: 'BollingerBands',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.DarkGray, lineWidth: 1 },
      { name: 'Middle', jsonKey: 'sma', type: 'line', color: ChartColors.DarkGray, lineWidth: 1, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.DarkGray, lineWidth: 1 }
    ]
  },
  'beta.standard.json': {
    displayName: 'Beta',
    chartType: 'oscillator',
    thresholds: [
      { value: 1, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'Beta', jsonKey: 'beta', type: 'line', color: ChartColors.StandardBlue }]
  },
  'bop.standard.json': {
    displayName: 'Bop',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'BOP', jsonKey: 'bop', type: 'baseline', color: ChartColors.StandardBlue }]
  },
  'cci.standard.json': {
    displayName: 'Cci',
    chartType: 'oscillator',
    thresholds: [
      { value: 100, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 0, color: ChartColors.DarkGrayTransparent, style: 'dash' },
      { value: -100, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [{ name: 'CCI', jsonKey: 'cci', type: 'line', color: ChartColors.StandardBlue }]
  },
  'chaikin-osc.standard.json': {
    displayName: 'ChaikinOsc',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'Chaikin', jsonKey: 'oscillator', type: 'baseline', color: ChartColors.StandardBlue }]
  },
  'chexit.standard.json': {
    displayName: 'Chandelier',
    chartType: 'overlay',
    fields: [
      { name: 'Long', jsonKey: 'exitLong', type: 'line', color: ChartColors.StandardOrange },
      { name: 'Short', jsonKey: 'exitShort', type: 'line', color: ChartColors.StandardOrange, lineStyle: 'dash' }
    ]
  },
  'chop.standard.json': {
    displayName: 'Chop',
    chartType: 'oscillator',
    thresholds: [
      { value: 61.8, color: ChartColors.DarkGrayTransparent, style: 'dash' },
      { value: 38.2, color: ChartColors.DarkGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'CHOP', jsonKey: 'chop', type: 'line', color: ChartColors.StandardBlue }]
  },
  'cmf.standard.json': {
    displayName: 'Cmf',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'CMF', jsonKey: 'cmf', type: 'line', color: ChartColors.StandardBlue }]
  },
  'cmo.standard.json': {
    displayName: 'Cmo',
    chartType: 'oscillator',
    fields: [{ name: 'CMO', jsonKey: 'cmo', type: 'line', color: ChartColors.StandardBlue }]
  },
  'corr.standard.json': {
    displayName: 'Correlation',
    chartType: 'oscillator',
    fields: [{ name: 'Correlation', jsonKey: 'correlation', type: 'line', color: ChartColors.StandardBlue }]
  },
  'crsi.standard.json': {
    displayName: 'ConnorsRsi',
    chartType: 'oscillator',
    thresholds: [
      { value: 90, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 10, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [{ name: 'CRSI', jsonKey: 'connorsRsi', type: 'line', color: ChartColors.StandardBlue }]
  },
  'dema.standard.json': {
    displayName: 'Dema',
    chartType: 'overlay',
    fields: [{ name: 'DEMA', jsonKey: 'dema', type: 'line', color: ChartColors.StandardBlue }]
  },
  'donchian.standard.json': {
    displayName: 'Donchian',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 },
      { name: 'Center', jsonKey: 'centerline', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 }
    ]
  },
  'dpo.standard.json': {
    displayName: 'Dpo',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'DPO', jsonKey: 'dpo', type: 'line', color: ChartColors.StandardBlue }]
  },
  'dynamic.standard.json': {
    displayName: 'Dynamic',
    chartType: 'overlay',
    fields: [{ name: 'Dynamic', jsonKey: 'dynamic', type: 'line', color: ChartColors.StandardBlue }]
  },
  'elder-ray.standard.json': {
    displayName: 'ElderRay',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'Bull Power', jsonKey: 'bullPower', type: 'line', color: ChartColors.StandardGreen },
      { name: 'Bear Power', jsonKey: 'bearPower', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'ema.standard.json': {
    displayName: 'Ema',
    chartType: 'overlay',
    fields: [{ name: 'EMA', jsonKey: 'ema', type: 'line', color: ChartColors.StandardBlue }]
  },
  'epma.standard.json': {
    displayName: 'Epma',
    chartType: 'overlay',
    fields: [{ name: 'EPMA', jsonKey: 'epma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'fcb.standard.json': {
    displayName: 'Fcb',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.StandardGreen },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'fisher.standard.json': {
    displayName: 'FisherTransform',
    chartType: 'oscillator',
    fields: [
      { name: 'Fisher', jsonKey: 'fisher', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Trigger', jsonKey: 'trigger', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'force.standard.json': {
    displayName: 'ForceIndex',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'Force', jsonKey: 'forceIndex', type: 'area', color: ChartColors.StandardBlue }]
  },
  'fractal.standard.json': {
    displayName: 'Fractal',
    chartType: 'overlay',
    fields: [
      { name: 'Bull', jsonKey: 'fractalBull', type: 'dots', color: ChartColors.StandardRed, lineWidth: 3 },
      { name: 'Bear', jsonKey: 'fractalBear', type: 'dots', color: ChartColors.StandardGreen, lineWidth: 3 }
    ]
  },
  'gator.standard.json': {
    displayName: 'Gator',
    chartType: 'oscillator',
    fields: []  // Disabled: multiple histogram series not supported with proper stacking
  },
  'heikinashi.standard.json': {
    displayName: 'HeikinAshi',
    fields: [] // This is a candle type, not overlay
  },
  'hma.standard.json': {
    displayName: 'Hma',
    chartType: 'overlay',
    fields: [{ name: 'HMA', jsonKey: 'hma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'htl.standard.json': {
    displayName: 'HtTrendline',
    chartType: 'overlay',
    fields: [
      { name: 'Trendline', jsonKey: 'trendline', type: 'line', color: ChartColors.StandardBlue },
      { name: 'SmoothPrice', jsonKey: 'smoothPrice', type: 'line', color: ChartColors.StandardOrange }
    ]
  },
  'htl-dcperiods.custom.json': {
    displayName: 'DcPeriods',
    chartType: 'oscillator',
    sourceFile: 'htl.standard.json',
    fields: [
      { name: 'DC Periods', jsonKey: 'dcPeriods', type: 'histogram', color: ChartColors.StandardPurple }
    ]
  },
  'hurst.standard.json': {
    displayName: 'Hurst',
    chartType: 'oscillator',
    thresholds: [
      { value: 0.5, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [{ name: 'Hurst', jsonKey: 'hurstExponent', type: 'line', color: ChartColors.StandardBlue }]
  },
  'ichimoku.standard.json': {
    displayName: 'Ichimoku',
    chartType: 'overlay',
    fields: [
      { name: 'Tenkan', jsonKey: 'tenkanSen', type: 'line', color: ChartColors.StandardBlue, lineWidth: 2 },
      { name: 'Kijun', jsonKey: 'kijunSen', type: 'line', color: ChartColors.StandardPurple, lineWidth: 2 },
      { name: 'Senkou A', jsonKey: 'senkouSpanA', type: 'line', color: ChartColors.ThresholdGreen, lineWidth: 1.5 },
      { name: 'Senkou B', jsonKey: 'senkouSpanB', type: 'line', color: ChartColors.ThresholdRed, lineWidth: 1.5 }
    ]
  },
  'kama.standard.json': {
    displayName: 'Kama',
    chartType: 'overlay',
    fields: [{ name: 'KAMA', jsonKey: 'kama', type: 'line', color: ChartColors.StandardBlue }]
  },
  'keltner.standard.json': {
    displayName: 'Keltner',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 },
      { name: 'Center', jsonKey: 'centerline', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 }
    ]
  },
  'kvo.standard.json': {
    displayName: 'Kvo',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'KVO', jsonKey: 'oscillator', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'ma-env.standard.json': {
    displayName: 'MaEnvelopes',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperEnvelope', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 },
      { name: 'Center', jsonKey: 'centerline', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerEnvelope', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 }
    ]
  },
  'macd.standard.json': {
    displayName: 'Macd',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.DarkGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'MACD', jsonKey: 'macd', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed },
      { name: 'Histogram', jsonKey: 'histogram', type: 'histogram', color: ChartColors.StandardGrayTransparent }
    ]
  },
  'mama.standard.json': {
    displayName: 'Mama',
    chartType: 'overlay',
    fields: [
      { name: 'MAMA', jsonKey: 'mama', type: 'line', color: ChartColors.StandardBlue },
      { name: 'FAMA', jsonKey: 'fama', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'mfi.standard.json': {
    displayName: 'Mfi',
    chartType: 'oscillator',
    thresholds: [
      { value: 80, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 20, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [{ name: 'MFI', jsonKey: 'mfi', type: 'line', color: ChartColors.StandardBlue }]
  },
  'obv.standard.json': {
    displayName: 'Obv',
    chartType: 'oscillator',
    fields: [{ name: 'OBV', jsonKey: 'obv', type: 'line', color: ChartColors.StandardBlue }]
  },
  'psar.standard.json': {
    displayName: 'ParabolicSar',
    chartType: 'overlay',
    fields: [{ name: 'SAR', jsonKey: 'sar', type: 'dots', color: ChartColors.StandardPurple, lineWidth: 2 }]
  },
  'pivots.standard.json': {
    displayName: 'Pivots',
    chartType: 'overlay',
    fields: [
      { name: 'High Point', jsonKey: 'highPoint', type: 'dots', color: ChartColors.StandardRed, lineWidth: 3 },
      { name: 'Low Point', jsonKey: 'lowPoint', type: 'dots', color: ChartColors.StandardGreen, lineWidth: 3 }
    ]
  },
  'pmo.standard.json': {
    displayName: 'Pmo',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'PMO', jsonKey: 'pmo', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'prs.standard.json': {
    displayName: 'Prs',
    chartType: 'oscillator',
    fields: [{ name: 'PRS', jsonKey: 'prs', type: 'line', color: ChartColors.StandardBlue }]
  },
  'pvo.standard.json': {
    displayName: 'Pvo',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'PVO', jsonKey: 'pvo', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed },
      { name: 'Histogram', jsonKey: 'histogram', type: 'histogram', color: ChartColors.StandardGrayTransparent }
    ]
  },
  'renko.standard.json': {
    displayName: 'Renko',
    fields: [] // This is a candle type, not overlay
  },
  'roc.standard.json': {
    displayName: 'Roc',
    chartType: 'oscillator',
    fields: [{ name: 'ROC', jsonKey: 'roc', type: 'line', color: ChartColors.StandardBlue }]
  },
  'roc-wb.standard.json': {
    displayName: 'RocWb',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'ROC', jsonKey: 'roc', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.StandardRed, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.StandardGreen, lineStyle: 'dash' }
    ]
  },
  'rolling-pivots.standard.json': {
    displayName: 'RollingPivots',
    chartType: 'overlay',
    fields: [
      { name: 'PP', jsonKey: 'pp', type: 'line', color: ChartColors.StandardBlue },
      { name: 'S1', jsonKey: 's1', type: 'line', color: ChartColors.StandardGreen },
      { name: 'R1', jsonKey: 'r1', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'rsi.standard.json': {
    displayName: 'Rsi',
    chartType: 'oscillator',
    thresholds: [
      { value: 70, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 30, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [{ name: 'RSI', jsonKey: 'rsi', type: 'line', color: ChartColors.StandardBlue }]
  },
  'slope.standard.json': {
    displayName: 'Slope',
    chartType: 'oscillator',
    fields: [{ name: 'Slope', jsonKey: 'slope', type: 'line', color: ChartColors.StandardBlue }]
  },
  'sma.standard.json': {
    displayName: 'Sma',
    chartType: 'overlay',
    fields: [{ name: 'SMA', jsonKey: 'sma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'smi.standard.json': {
    displayName: 'Smi',
    chartType: 'oscillator',
    thresholds: [
      { value: 40, color: ChartColors.ThresholdRed, style: 'dash' },
      { value: -40, color: ChartColors.ThresholdGreen, style: 'dash' }
    ],
    fields: [
      { name: 'SMI', jsonKey: 'smi', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'smma.standard.json': {
    displayName: 'Smma',
    chartType: 'overlay',
    fields: [{ name: 'SMMA', jsonKey: 'smma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'starc.standard.json': {
    displayName: 'StarcBands',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 },
      { name: 'Center', jsonKey: 'centerline', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 }
    ]
  },
  'stc.standard.json': {
    displayName: 'Stc',
    chartType: 'oscillator',
    thresholds: [
      { value: 75, color: ChartColors.ThresholdGreen, style: 'solid' },
      { value: 25, color: ChartColors.ThresholdRed, style: 'solid' }
    ],
    fields: [{ name: 'STC', jsonKey: 'stc', type: 'line', color: ChartColors.StandardBlue }]
  },
  'stdev.standard.json': {
    displayName: 'StdDev',
    chartType: 'oscillator',
    fields: [{ name: 'StdDev', jsonKey: 'stdDev', type: 'line', color: ChartColors.StandardBlue }]
  },
  'stdev-channels.standard.json': {
    displayName: 'StdDevChannels',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperChannel', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 },
      { name: 'Center', jsonKey: 'centerline', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1, lineStyle: 'dash' },
      { name: 'Lower', jsonKey: 'lowerChannel', type: 'line', color: ChartColors.StandardOrange, lineWidth: 1 }
    ]
  },
  'stoch.standard.json': {
    displayName: 'Stoch',
    chartType: 'oscillator',
    thresholds: [
      { value: 80, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 20, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [
      { name: '%K', jsonKey: 'k', type: 'line', color: ChartColors.StandardBlue },
      { name: '%D', jsonKey: 'd', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'stoch-rsi.standard.json': {
    displayName: 'StochRsi',
    chartType: 'oscillator',
    thresholds: [
      { value: 80, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 20, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [
      { name: 'StochRSI', jsonKey: 'stochRsi', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'supertrend.standard.json': {
    displayName: 'SuperTrend',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'line', color: ChartColors.StandardRed },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'line', color: ChartColors.StandardGreen }
    ]
  },
  't3.standard.json': {
    displayName: 'T3',
    chartType: 'overlay',
    fields: [{ name: 'T3', jsonKey: 't3', type: 'line', color: ChartColors.StandardBlue }]
  },
  'tema.standard.json': {
    displayName: 'Tema',
    chartType: 'overlay',
    fields: [{ name: 'TEMA', jsonKey: 'tema', type: 'line', color: ChartColors.StandardBlue }]
  },
  'trix.standard.json': {
    displayName: 'Trix',
    chartType: 'oscillator',
    thresholds: [
      { value: 0, color: ChartColors.ThresholdGrayTransparent, style: 'dash' }
    ],
    fields: [
      { name: 'TRIX', jsonKey: 'trix', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'tsi.standard.json': {
    displayName: 'Tsi',
    chartType: 'oscillator',
    thresholds: [
      { value: 25, color: ChartColors.ThresholdRed, style: 'dash' },
      { value: -25, color: ChartColors.ThresholdGreen, style: 'dash' }
    ],
    fields: [
      { name: 'TSI', jsonKey: 'tsi', type: 'line', color: ChartColors.StandardBlue },
      { name: 'Signal', jsonKey: 'signal', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'ulcer.standard.json': {
    displayName: 'UlcerIndex',
    chartType: 'oscillator',
    fields: [{ name: 'Ulcer', jsonKey: 'ulcerIndex', type: 'line', color: ChartColors.StandardBlue }]
  },
  'uo.standard.json': {
    displayName: 'Ultimate',
    chartType: 'oscillator',
    thresholds: [
      { value: 70, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: 30, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [{ name: 'Ultimate', jsonKey: 'ultimate', type: 'line', color: ChartColors.StandardBlue }]
  },
  'vol-stop.standard.json': {
    displayName: 'VolatilityStop',
    chartType: 'overlay',
    fields: [
      { name: 'Upper', jsonKey: 'upperBand', type: 'dots', color: ChartColors.StandardRed, lineWidth: 2 },
      { name: 'Lower', jsonKey: 'lowerBand', type: 'dots', color: ChartColors.StandardGreen, lineWidth: 2 }
    ]
  },
  'vortex.standard.json': {
    displayName: 'Vortex',
    chartType: 'oscillator',
    fields: [
      { name: '+VI', jsonKey: 'pvi', type: 'line', color: ChartColors.StandardGreen },
      { name: '-VI', jsonKey: 'nvi', type: 'line', color: ChartColors.StandardRed }
    ]
  },
  'vwap.standard.json': {
    displayName: 'Vwap',
    chartType: 'overlay',
    fields: [{ name: 'VWAP', jsonKey: 'vwap', type: 'line', color: ChartColors.StandardBlue }]
  },
  'vwma.standard.json': {
    displayName: 'Vwma',
    chartType: 'overlay',
    fields: [{ name: 'VWMA', jsonKey: 'vwma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'willr.standard.json': {
    displayName: 'WilliamsR',
    chartType: 'oscillator',
    thresholds: [
      { value: -20, color: ChartColors.ThresholdRed, style: 'dash', fill: 'above', fillColor: ChartColors.ThresholdRedTransparent },
      { value: -80, color: ChartColors.ThresholdGreen, style: 'dash', fill: 'below', fillColor: ChartColors.ThresholdGreenTransparent }
    ],
    fields: [{ name: 'Williams %R', jsonKey: 'williamsR', type: 'line', color: ChartColors.StandardBlue }]
  },
  'wma.standard.json': {
    displayName: 'Wma',
    chartType: 'overlay',
    fields: [{ name: 'WMA', jsonKey: 'wma', type: 'line', color: ChartColors.StandardBlue }]
  },
  'zigzag.standard.json': {
    displayName: 'ZigZag',
    chartType: 'overlay',
    fields: [{ name: 'ZigZag', jsonKey: 'zigZag', type: 'line', color: ChartColors.StandardBlue }]
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

  // Build series data with styling
  const series = config.fields.map(field => {
    const seriesData = {
      name: field.name,
      type: field.type || 'line'
    }

    // Add color after type (but NOT for conditional coloring histograms)
    if (field.color && !field.colorConditional) {
      seriesData.color = field.color
    }
    if (field.lineWidth) seriesData.lineWidth = field.lineWidth
    if (field.lineStyle) seriesData.lineStyle = field.lineStyle
    
    // Add data last - with conditional coloring for histograms if needed
    if (field.colorConditional && field.type === 'histogram') {
      seriesData.data = results.map(r => {
        const value = r[field.jsonKey] ?? null
        const dataPoint = {
          timestamp: r.timestamp,
          value: value
        }
        // Add color based on value (green above zero, red below)
        if (value !== null && !isNaN(value)) {
          dataPoint.color = value >= 0 ? ChartColors.StandardGreen : ChartColors.StandardRed
        }
        return dataPoint
      })
    } else {
      seriesData.data = results.map(r => ({
        timestamp: r.timestamp,
        value: r[field.jsonKey] ?? null
      }))
    }

    return seriesData
  })

  // Build metadata with optional chartType and thresholds
  const metadata = {
    symbol: 'S&P 500',
    timeframe: 'Daily',
    indicator: config.displayName
  }

  if (config.chartType) {
    metadata.chartType = config.chartType
  }

  if (config.thresholds && config.thresholds.length > 0) {
    metadata.thresholds = config.thresholds
  }

  return {
    metadata: metadata,
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
    // Use sourceFile if specified, otherwise use resultFile
    const sourceFile = config.sourceFile || resultFile
    const resultPath = join(RESULTS_DIR, sourceFile)

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

      // Write output file with trailing newline
      const outputFile = join(OUTPUT_DIR, `${config.displayName}.json`)
      writeFileSync(outputFile, JSON.stringify(chartData, null, 2) + '\n')
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
