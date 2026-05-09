---
title: Guide
description: Learn about the different indicator styles, capabilities, and utilities
layout: home
hero:
  name: Guide
  tagline: Three distinct indicator styles to support different use cases
features:
  - title: Batch (Series) style
    details: One-time bulk conversions of complete historical datasets
    link: /guide/batch
    linkText: Learn more
  - title: Buffer list style
    details: Self-managed incremental data without hub infrastructure
    link: /guide/buffer
    linkText: Learn more
  - title: Stream hub style
    details: Live data feeds with coordinated multi-indicator updates
    link: /guide/stream
    linkText: Learn more
  - title: Chaining
    details: Build sophisticated analysis by chaining indicators together, creating indicators of indicators
    link: /guide/batch#chaining-indicators
    linkText: Learn more
  - title: Custom indicators
    details: Create your own indicators that integrate seamlessly with the library
    link: /guide/customization
    linkText: Learn more
  - title: Utilities
    details: Helper functions for quote manipulation, result processing, and numerical analysis
    link: /utilities
    linkText: Learn more
  - title: Code samples
    details: Full .NET solution with working sample code
    link: /examples
    linkText: Learn more
---

## Indicator style comparison

This library has three indicator styles available to support different uses cases.

| Style        | Use case                                     | Best for                       |
| ------------ | -------------------------------------------- | ------------------------------ |
| Series batch | Convert full quote collections to indicators | Once-and-done bulk conversions |
| Buffer lists | Standalone incrementing `ICollection` lists  | Self-managed incrementing data |
| Stream hub   | Subscription based hub-observer pattern      | Streaming or live data sources |

### Feature comparison

| Feature        | Series batch    | Buffer lists  | Stream hub   |
| -------------- | --------------- | ------------- | ------------ |
| Incrementing   | no              | yes           | yes          |
| Batch speed    | fastest         | faster        | fast         |
| Scaling        | low             | moderate      | high         |
| Class type     | static          | instance      | instance     |
| Base interface | `IReadOnlyList` | `ICollection` | `IStreamHub` |
| Complexity     | lowest          | moderate      | highest      |
| Chainable      | yes             | yes           | yes          |
| Pruning        | with utility    | auto-preset   | auto-preset  |
