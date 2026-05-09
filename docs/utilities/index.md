---
title: Utilities and helpers
description: The Stock Indicators for .NET library includes utilities to help you use and transform historical prices quotes and indicator results, and to create custom indicators.
---

# Utilities and helper functions

The library provides utilities and helper functions to work with historical quotes, indicator results, and numerical analysis.

<div class="utility-categories">

<a href="/utilities/quotes/" class="category-card">
  <div class="card-icon">📊</div>
  <h2>Quote utilities</h2>
  <p>Prepare and transform historical price quotes before using them with indicators</p>
  <span class="card-count">5 utilities</span>
</a>

<a href="/utilities/results/" class="category-card">
  <div class="card-icon">🎯</div>
  <h2>Result utilities</h2>
  <p>Work with indicator results after calculation and analysis</p>
  <span class="card-count">4 utilities</span>
</a>

<a href="/utilities/helpers/" class="category-card">
  <div class="card-icon">🔧</div>
  <h2>Helper utilities</h2>
  <p>Numerical analysis tools for creating custom indicators</p>
  <span class="card-count">2 utilities</span>
</a>

<a href="/utilities/catalog" class="category-card">
  <div class="card-icon">📚</div>
  <h2>Indicator catalog</h2>
  <p>Programmatic access to indicator metadata for dynamic UIs</p>
  <span class="card-count">1 utility</span>
</a>

</div>

<style scoped>
.utility-categories {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 1.5rem;
  margin: 2rem 0;
}

.category-card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 2rem 1.5rem;
  background: var(--vp-c-bg-soft);
  border: 2px solid var(--vp-c-divider);
  border-radius: 12px;
  text-decoration: none;
  transition: all 0.3s ease;
  min-height: 200px;
}

.category-card:hover {
  border-color: var(--vp-c-brand);
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.card-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  filter: grayscale(20%);
  transition: filter 0.3s ease;
}

.category-card:hover .card-icon {
  filter: grayscale(0%);
}

.category-card h2 {
  margin: 0 0 0.75rem 0;
  padding: 0;
  font-size: 1.5rem;
  color: var(--vp-c-brand);
  border: none;
}

.category-card p {
  margin: 0;
  font-size: 0.95rem;
  color: var(--vp-c-text-2);
  line-height: 1.5;
  flex-grow: 1;
}

.card-count {
  display: inline-block;
  margin-top: 1rem;
  padding: 0.25rem 0.75rem;
  font-size: 0.85rem;
  color: var(--vp-c-text-2);
  background: var(--vp-c-bg);
  border-radius: 12px;
  border: 1px solid var(--vp-c-divider);
}
</style>
