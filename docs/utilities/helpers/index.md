---
title: Helper utilities
description: Numerical analysis utilities for creating custom indicators.
---

# Helper utilities

Numerical analysis tools and utilities for creating custom indicators. These are the same internal tools used by the library's built-in indicators.

<div class="utility-cards">

## [Numerical methods](/utilities/helpers/numerical-methods)

Mathematical analysis functions including slope calculation and standard deviation.

```csharp
double slope = Numerix.Slope(xValues, yValues);
double sd = values.StdDev();
```

[See more →](/utilities/helpers/numerical-methods)

## [NullMath](/utilities/helpers/nullmath)

Null-safe mathematical operations that handle null values gracefully.

```csharp
decimal? abs = NullMath.Abs(value);
decimal? rounded = NullMath.Round(value, 2);
```

[See more →](/utilities/helpers/nullmath)

</div>

<style scoped>
.utility-cards {
  display: grid;
  gap: 1.5rem;
  margin: 2rem 0;
}

.utility-cards h2 {
  margin: 0;
  font-size: 1.35rem;
  padding: 1.25rem;
  background: var(--vp-c-bg-soft);
  border: 1px solid var(--vp-c-divider);
  border-left: 4px solid var(--vp-c-brand);
  border-radius: 8px;
  transition: all 0.2s ease;
}

.utility-cards h2:hover {
  border-left-color: var(--vp-c-brand-dark);
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.utility-cards h2 a {
  text-decoration: none;
  color: var(--vp-c-brand);
}

.utility-cards p {
  margin: 0.75rem 0 0 0;
  padding: 0 1.25rem 1.25rem 1.25rem;
  color: var(--vp-c-text-2);
  line-height: 1.6;
}

.utility-cards p strong {
  display: block;
  margin-top: 0.5rem;
  color: var(--vp-c-text-1);
  font-size: 0.9rem;
}
</style>
