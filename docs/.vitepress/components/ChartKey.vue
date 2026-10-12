<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { CHART_INDICATORS } from '../theme/chart-indicators'

// Names each line of a multi-line chart. The charts library draws one label per
// pane, so the line names and colors come from the same catalog snapshot the
// chart renders from (facioquo/stock-charts#599 tracks a library legend).
const props = defineProps<{ indicator: string }>()

interface Listing {
  uiid: string
  results: Array<{ displayName: string; defaultColor: string }>
}

const listings = ref<Listing[]>([])

onMounted(async () => {
  try {
    const response = await fetch(`${import.meta.env.BASE_URL}data/chart-api/indicators.json`)
    if (response.ok) listings.value = (await response.json()) as Listing[]
  } catch {
    // The key is an aid; the chart is complete without it.
  }
})

const lines = computed(() => {
  const uiid = CHART_INDICATORS[props.indicator]?.uiid?.toLowerCase()
  return listings.value.find((l) => l.uiid.toLowerCase() === uiid)?.results ?? []
})
</script>

<template>
  <ul v-if="lines.length > 1" class="chart-key" aria-label="Chart lines">
    <li v-for="line in lines" :key="line.displayName">
      <span class="chart-key__swatch" :style="{ background: line.defaultColor }" aria-hidden="true" />
      {{ line.displayName }}
    </li>
  </ul>
</template>
