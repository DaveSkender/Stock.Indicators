<script setup lang="ts">
import { data as contributors } from '../contributors.data'
</script>

<template>
  <ul class="contributors-list">
    <li v-for="contributor in contributors" :key="contributor.login" class="contributor-item">
      <a :href="contributor.url" :title="`${contributor.login} - ${contributor.contributions} contributions`" target="_blank" rel="noopener noreferrer">
        <img
          :src="contributor.avatar"
          :alt="`${contributor.login} avatar`"
          width="75"
          height="75"
          class="contributor-avatar"
          loading="lazy"
        />
      </a>
    </li>
  </ul>
</template>

<style scoped>
.contributors-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  list-style: none;
  padding: 0;
  margin: 16px 0;
}

.contributor-item {
  display: inline-block;
  margin: 0;
}

.contributor-avatar {
  border-radius: 50%;
  transition: transform 0.2s ease;
  display: block;
}

.contributor-avatar:hover {
  transform: scale(1.1);
}
</style>
