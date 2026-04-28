<script setup lang="ts">
const props = defineProps<{
  indicatorKey: string
}>()

// Maps PascalCase doc indicator keys to stock-charts API uiids where they differ
const UIID_MAP: Record<string, string> = {
  Aroon: 'AROON UP/DOWN',
  AtrStop: 'ATR-STOP-HL',
  BollingerBands: 'BB',
  Chandelier: 'CHEXIT-LONG',
  ConnorsRsi: 'CRSI',
  DcPeriods: 'DCPERIOD',
  Dynamic: 'DYN',
  ElderRay: 'ELDER-RAY',
  FisherTransform: 'FISHER',
  HtTrendline: 'HT Trendline',
  ParabolicSar: 'PSAR',
  StarcBands: 'STARC',
  StdDev: 'STDEV',
  Stoch: 'STO',
  UlcerIndex: 'ULCER',
  ZigZag: 'ZIGZAG-HL',
}

const uiid = UIID_MAP[props.indicatorKey] ?? props.indicatorKey
</script>

<template>
  <ClientOnly>
    <StockIndicatorChart :indicator="uiid" />
  </ClientOnly>
</template>
