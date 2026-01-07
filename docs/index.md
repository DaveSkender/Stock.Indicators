---
title: Stock Indicators for .NET
titleTemplate: Transform price quotes into trading insights
layout: home
isHome: true

hero:
  name: stock indicators <small>for .NET</small>
  tagline: Transform price quotes into trade indicators and market insights.
  actions:
    - theme: brand
      text: get started
      link: /guide/
    - theme: alt
      text: features
      link: /features/
    - theme: alt
      text: indicators
      link: /indicators/
features:
  - title: Indicators and overlays
    link: /indicators
    details: Reference documentation
  - title: Guide and Pro tips
    link: /guide
    details: Getting started usage guide
  - title: Utilities
    link: /utilities
    details: and helper functions
  - title: Demo site
    link: https://charts.stockindicators.dev
    details: An interactive stock chart
  - title: Examples
    link: /examples
    details: Runnable code samples
  - title: Release notes
    link: https://github.com/DaveSkender/Stock.Indicators/releases
    details: Version changelog
  - title: Discussions
    link: https://github.com/DaveSkender/Stock.Indicators/discussions
    details: Community conversations
  - title: Contributing guide
    link: /contributing
    details: Repo developer guide
---

<p style="display:flex; justify-content:left; gap:1rem; margin-top: 2rem; flex-wrap:wrap;">
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Get the NuGet package."><img src="https://img.shields.io/nuget/v/skender.stock.indicators?logo=NuGet&label=NuGet&color=blue&cacheSeconds=259200" alt="NuGet Package" /></a>
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Read more about package downloads."><img src="https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads&cacheSeconds=259200" alt="Downloads" /></a>
</p>

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that transforms historical price quotes into technical indicators. Get moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, and [many other indicators](/indicators).

Build trading algorithms, charting applications, machine learning models, or market analysis tools with your own [OHLCV](/guide#historical-quotes) price quotes from any market: equities, commodities, forex, or cryptocurrencies. A [Python version](https://python.stockindicators.dev/) is also available.

## Industry-standard indicators with extensibility

Access a comprehensive library of battle-tested technical indicators used by traders worldwide. Extend functionality by creating your own [custom indicators](/customization) that integrate seamlessly with the library.

<ClientOnly>
  <div class="home-charts-stack">
    <IndicatorChart src="/data/Alma.json" />
    <IndicatorChart src="/data/AtrStop.json" />
    <IndicatorChart src="/data/Aroon.json" />
  </div>
</ClientOnly>

## Simple, intuitive API

```csharp
// example: calculate 20-period simple moving average
IReadOnlyList<SmaResult> results = quotes.ToSma(20);
```

See more [usage examples](/guide#example-usage).

## Powerful chaining for advanced analysis

Chain indicators together for sophisticated technical analysis: create indicators of indicators, calculate [slope](/indicators/Slope) (direction) of any result, or apply [moving averages](/indicators/Sma) to indicator outputs.

```csharp
// example: calculate RSI of On-Balance Volume
IReadOnlyList<RsiResult> results
  = quotes
    .ToObv()
    .ToRsi(14);

// example: use custom candle price variants
IReadOnlyList<EmaResult> results
  = quotes
    .Use(CandlePart.HL2)
    .ToEma(20);
```

## Incrementally add data with buffer lists <Badge type="warning" text="preview" />

For scenarios where quotes arrive one at a time, buffer lists provide efficient incremental processing without recalculating the entire history.

```csharp
// create list
SmaList<SmaResult> smaList = new(lookbackPeriods: 20);

// add new quotes incrementally
smaList.Add(newQuote);
```

Buffer lists maintain internal state and automatically manage the warmup period, making them ideal for basic live data feeds and incremental updates.

## Streaming hubs with observer patterns <Badge type="warning" text="preview" />

Hubs provides a reactive, subscription-based pattern for streaming market data with automatic cascading calculations for advances scenarios.

```csharp
// create provider with chain of indicators
QuoteHub provider = new QuoteHub();
StreamHub smaHub = provider.ToSmaHub(20);
StreamHub rsiHub = sma.ToRsiHub(14);  // RSI of SMA

// publish quotes - observers auto-update in cascade
provider.Add(newQuote);

// consume downstream hubs indicators
IReadOnlyList<RsiResult> = rsiHub.Results;
```

The observer cascade ensures that when a new quote arrives, all chained indicators update automatically in the correct sequence.

See the [guide](/guide) and the [full list of indicators and overlays](/indicators) for more information.

## Optimized for modern .NET frameworks

Our [NuGet library](https://www.nuget.org/packages/Skender.Stock.Indicators) directly targets all actively [supported Microsoft .NET platforms](https://dotnet.microsoft.com/platform/support/policy) for peak performance.

- .NET 10.0, 9.0, 8.0

The compiled library package is [Common Language Specification (CLS) compliant](https://docs.microsoft.com/en-us/dotnet/standard/common-type-system) and can be used in other programming languages, including Python and everything in the .NET universe.

## Licensed for everyone

<a href="https://opensource.org/licenses/Apache-2.0"><img src="https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square&cacheSeconds=259200" alt="Apache 2.0 license badge" /></a>

This repository uses the standard Apache 2.0 open-source license. Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## Share your ideas with the community

**Need help?** Have ideas? [Start a new discussion, ask a question 💬](https://github.com/DaveSkender/Stock.Indicators/discussions), or [submit an issue](https://github.com/DaveSkender/Stock.Indicators/issues) if it is publicly relevant. You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).

## Give back with patronage

Thank you for your support! This software is crafted with care by unpaid enthusiasts who 💖 all forms of encouragement. If you or your organization use this library or like what we're doing, add a ⭐ on the [GitHub Repo](https://github.com/DaveSkender/Stock.Indicators) as a token of appreciation.

If you want to buy me a beer or are interested in ongoing support as a patron, [become a sponsor](https://github.com/sponsors/facioquo). Patronage motivates continued maintenance and evolution of open-source projects, and to inspire new ones.

## Contribute to help others

This NuGet package is an open-source project [on GitHub](https://github.com/DaveSkender/Stock.Indicators). If you want to report bugs or contribute fixes, new indicators, or new features, please review our [contributing guidelines](/contributing) and [the backlog](https://github.com/users/DaveSkender/projects/1).

Special thanks to all of our community code contributors!

<Contributors />

Visit our [GitHub repository](https://github.com/DaveSkender/Stock.Indicators) to see the full list.

» see our [full list of indicators and overlays](/indicators)
