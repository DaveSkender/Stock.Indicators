<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  indicatorKey: string
}>()

const src = computed(() => `/data/${props.indicatorKey}.json`)
</script>

<template>
  <ClientOnly>
    <IndicatorChart :src="src" :indicator-key="indicatorKey" />
  </ClientOnly>
</template>
