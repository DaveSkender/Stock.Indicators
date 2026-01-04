---
layout: home
isHome: true
title: Stock Indicators for .NET
titleTemplate: Transform price quotes into trading insights

hero:
  name: Stock Indicators <small>for .NET</small>
  tagline: Transform price quotes into trading insights
  actions:
    - theme: brand
      text: Get Started
      link: /guide/
    - theme: alt
      text: Indicators
      link: /indicators/
---

<p style="display:flex; justify-content:left; gap:1rem; margin-top: 2rem; flex-wrap:wrap;">
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Get the NuGet package."><img src="https://img.shields.io/nuget/v/skender.stock.indicators?logo=NuGet&label=NuGet&color=blue&cacheSeconds=259200" alt="NuGet Package" /></a>
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Read more about package downloads."><img src="https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads&cacheSeconds=259200" alt="Downloads" /></a>
</p>

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that produces financial market technical indicators. Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc. Nothing more.

Build your technical analysis, trading algorithms, machine learning, charting, or other intelligent market software with this library and your own [OHLCV](/guide#historical-quotes) price quotes sources for equities, commodities, forex, cryptocurrencies, and others. [Stock Indicators for Python](https://python.stockindicators.dev/) is also available.

Explore more information:

- [Indicators and overlays](/indicators)
- [Guide and Pro tips](/guide)
- [Utilities and helper functions](/utilities)
- [Demo site](https://charts.stockindicators.dev/) (a stock chart)
- [Example usage code](/examples/)
- [Release notes](https://github.com/DaveSkender/Stock.Indicators/releases)
- [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions)
- [Contributing guidelines](/contributing)

## Reputable and extensible indicators

You'll get all of the industry standard indicators out-of-the-box. Additionally, you can create compatible [custom indicators](/examples/CustomIndicatorsLibrary/).

<ClientOnly>
  <div class="home-charts-stack">
    <IndicatorChart src="/data/BollingerBands.json" />
    <IndicatorChart src="/data/Stoch.json" />
    <IndicatorChart src="/data/Macd.json" />
  </div>
</ClientOnly>

## Easy to use in your application

```csharp
// example: get 20-period simple moving average
IReadOnlyList<SmaResult> results = quotes.ToSma(20);
```

See more [usage examples](/guide#example-usage).

## Use chaining for unique insights

Optional chaining enables advanced uses cases; such as, indicator of indicators, [slope](/indicators/Slope) (direction) of any result, or [moving average](/indicators#moving-averages) of an indicator.

```csharp
// example: advanced chaining (RSI of OBV)
IReadOnlyList<RsiResult> results
  = quotes
    .ToObv()
    .ToRsi(14);

// example: use any candle variant
IReadOnlyList<EmaResult> results
  = quotes
    .Use(CandlePart.HL2)
    .ToEma(20);
```

## Incrementally add data with buffer lists

For scenarios where quotes arrive one at a time, buffer lists provide efficient incremental processing without recalculating the entire history.

```csharp
// create list
SmaList<SmaResult> smaList = new(lookbackPeriods: 20);

// add new quotes incrementally
smaList.Add(newQuote);
```

Buffer lists maintain internal state and automatically manage the warmup period, making them ideal for basic live data feeds and incremental updates.

## Streaming hubs with observer patterns

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
