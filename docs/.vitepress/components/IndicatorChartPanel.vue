<script setup lang="ts">
const props = defineProps<{
  indicatorKey: string
}>()
</script>

<template>
  <ClientOnly>
    <StockIndicatorChart :indicator="indicatorKey" />
  </ClientOnly>
</template>
