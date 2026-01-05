---
title: Features
description: Learn about the different indicator styles and features available in Stock Indicators for .NET
---

# {{ $frontmatter.title }}

Stock Indicators for .NET provides three distinct indicator styles to support different use cases, from batch processing to real-time streaming.

## Indicator styles

Choose the style that best fits your use case:

| Style | Best for | Learn more |
|-------|----------|------------|
| **Batch (Series)** | One-time bulk conversions of complete historical datasets | [Batch processing →](/features/batch) |
| **Buffer lists** | Self-managed incremental data without hub infrastructure | [Buffer lists →](/features/buffer) |
| **Stream hubs** | Live data feeds with coordinated multi-indicator updates | [Stream hubs →](/features/stream) |

## Feature comparison

| Feature | Series batch | Buffer lists | Stream hub |
|---------|-------------|--------------|------------|
| Incrementing | no | yes | yes |
| Batch speed | fastest | faster | fast |
| Scaling | low | moderate | high |
| Class type | static | instance | instance |
| Base interface | `IReadOnlyList` | `ICollection` | `IStreamHub` |
| Complexity | lowest | moderate | highest |
| Chainable | yes | yes | yes |
| Pruning | with utility | auto-preset | auto-preset |

## Additional features

### Utilities

Helper functions for quote manipulation, result processing, and numerical analysis.

[Explore utilities →](/utilities)

### Custom indicators

Create your own indicators that integrate seamlessly with the library.

[Learn about custom indicators →](/custom-indicators)

### Chaining

Build sophisticated analysis by chaining indicators together, creating indicators of indicators.

[See chaining examples →](/guide#generating-indicator-of-indicators)

---
Last updated: January 5, 2026
