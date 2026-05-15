<script setup lang="ts">
import { computed } from 'vue'
import { UIID_MAP } from '../utils/uiid-map'

const props = defineProps<{
  indicatorKey: string
}>()

const uiid = computed(() => UIID_MAP[props.indicatorKey] ?? props.indicatorKey)
</script>

<template>
  <ClientOnly>
    <StockIndicatorChart :indicator="uiid" />
  </ClientOnly>
</template>
