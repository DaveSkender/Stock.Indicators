/**
 * Maps PascalCase indicator keys (as used in doc page `indicator-key` props)
 * to the UIIDs expected by the stock-charts API and @faciaquo/indy-charts.
 * Keys not in this map are passed through unchanged.
 */
export const UIID_MAP: Record<string, string> = {
  Aroon: 'AROON UP/DOWN',
  AtrStop: 'ATR-STOP-HL',
  Awesome: 'AO',
  BollingerBands: 'BB',
  Chandelier: 'CHEXIT-LONG',
  ChaikinOsc: 'CHAIKIN',
  ConnorsRsi: 'CRSI',
  DcPeriods: 'DCPERIOD',
  Dynamic: 'DYN',
  ElderRay: 'ELDER-RAY',
  FisherTransform: 'FISHER',
  ForceIndex: 'FORCE',
  HtTrendline: 'HT Trendline',
  MaEnvelopes: 'MA-ENV',
  ParabolicSar: 'PSAR',
  StarcBands: 'STARC',
  StdDev: 'STDEV',
  StdDevChannels: 'STDEV-CH',
  Stoch: 'STO',
  UlcerIndex: 'ULCER',
  VolatilityStop: 'VOL-STOP',
  ZigZag: 'ZIGZAG-HL',
}
