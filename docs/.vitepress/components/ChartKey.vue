<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { CHART_SNAPSHOT_PATH } from '../theme/chart-api'
import { CHART_INDICATORS } from '../theme/chart-indicators'

// Names each line of a multi-line chart. The charts library draws one label per
// pane, so names and styles come from the committed catalog snapshot, which can
// lag the live API the chart prefers (facioquo/stock-charts#599 tracks a
// library legend).
const props = defineProps<{ indicator: string }>()

interface Listing {
  uiid: string
  results: Array<{ dataName: string; displayName: string; defaultColor: string; lineType?: string }>
}

const listings = ref<Listing[]>([])

onMounted(async () => {
  try {
    const response = await fetch(`${CHART_SNAPSHOT_PATH}/indicators.json`)
    if (response.ok) listings.value = (await response.json()) as Listing[]
  } catch {
    // The key is an aid; the chart is complete without it.
  }
})

// A display name can carry an unresolved legend template ("FCB([P1]) Upper Band")
// or repeat across lines; the template prefix is dropped and a repeat falls back
// to the property name.
const lines = computed(() => {
  const uiid = CHART_INDICATORS[props.indicator]?.uiid?.toLowerCase()
  const results = listings.value.find((l) => l.uiid.toLowerCase() === uiid)?.results ?? []
  const names = results.map((r) => r.displayName.replace(/^\w+\((?:\[P\d+\],?\s*)*\)\s*/, ''))
  return results.map((r, i) => ({
    key: r.dataName,
    name: names.filter((n) => n === names[i]).length > 1 ? r.dataName : names[i],
    color: r.defaultColor,
    style: r.lineType === 'dash' ? 'dashed' : r.lineType === 'dots' ? 'dotted' : 'solid'
  }))
})
</script>

<template>
  <ul v-if="lines.length > 1" class="chart-key" role="list" aria-label="Chart lines">
    <li v-for="line in lines" :key="line.key">
      <span class="chart-key__swatch" :style="{ borderTopColor: line.color, borderTopStyle: line.style }" aria-hidden="true" />
      {{ line.name }}
    </li>
  </ul>
</template>
