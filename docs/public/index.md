---
title: Stock Indicators for .NET
description: Transform financial market prices into technical analysis insights with this best in class C# NuGet library. Go further with chaining and custom indicators.
permalink: /
---

# Stock Indicators for .NET

<p style="text-align:center;">
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Get the NuGet package." class="not-mobile"><img src="https://img.shields.io/nuget/v/skender.stock.indicators?logo=NuGet&label=NuGet%20Package&color=blue&cacheSeconds=259200" alt="" width="170" height="20" /></a>
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Read more about package downloads." class="not-mobile"><img src="https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads&cacheSeconds=259200" alt="" width="130" height="20" /></a>
</p>

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that produces financial market technical indicators. Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc. Nothing more.

Build your technical analysis, trading algorithms, machine learning, charting, or other intelligent market software with this library and your own OHLCV price quotes sources for equities, commodities, forex, cryptocurrencies, and others. [Stock Indicators for Python](https://python.stockindicators.dev/) is also available.

Explore more information:

- [Indicators and overlays](/indicators)
- [Guide and Pro tips](/guide)
- [Utilities and helper functions](/utilities)
- [Demo site](https://charts.stockindicators.dev/) (a stock chart)
- [Example usage code](https://github.com/DaveSkender/Stock.Indicators/tree/main/docs/examples)
- [Release notes](https://github.com/DaveSkender/Stock.Indicators/releases)
- [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions)
- [Contributing guidelines](/contributing)

## Reputable and extensible indicators

You'll get all of the industry standard indicators out-of-the-box. Additionally, you can create compatible [custom indicators](https://github.com/DaveSkender/Stock.Indicators/tree/main/docs/examples/CustomIndicatorsLibrary).

![sample indicators shown in chart](examples.webp)

## Easy to use in your application

```csharp
// example: get 20-period simple moving average
IReadOnlyList<SmaResult> results = quotes.GetSma(20);
```

See more [usage examples](/guide).

## Use chaining for unique insights

Optional chaining enables advanced uses cases; such as, indicator of indicators, slope (direction) of any result, or moving average of an indicator.

```csharp
// example: advanced chaining (RSI of OBV)
IReadOnlyList<RsiResult> results
  = quotes
    .GetObv()
    .GetRsi(14);

// example: use any candle variant
IReadOnlyList<EmaResult> results
  = quotes
    .Use(CandlePart.HL2)
    .GetEma(20);
```

See the [guide](/guide) and the [full list of indicators and overlays](/indicators) for more information.

## Optimized for modern .NET frameworks

Our [NuGet library](https://www.nuget.org/packages/Skender.Stock.Indicators) directly targets all current frameworks for peak performance.

- .NET 9.0, 8.0, 6.0

The compiled library package is [Common Language Specification (CLS) compliant](https://docs.microsoft.com/en-us/dotnet/standard/common-type-system) and can be used in other programming languages, including Python and everything in the .NET universe.

## Licensed for everyone

[![Apache 2.0 license badge](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square&cacheSeconds=259200)](https://opensource.org/licenses/Apache-2.0)

This repository uses the standard Apache 2.0 open-source license. Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## Share your ideas with the community

**Need help?** Have ideas? [Start a new discussion, ask a question 💬](https://github.com/DaveSkender/Stock.Indicators/discussions), or [submit an issue](https://github.com/DaveSkender/Stock.Indicators/issues) if it is publicly relevant. You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).

## Give back with patronage

Thank you for your support! This software is crafted with care by unpaid enthusiasts who ❤️ all forms of encouragement. If you or your organization use this library or like what we're doing, add a ⭐ on the [GitHub Repo](https://github.com/DaveSkender/Stock.Indicators) as a token of appreciation.

If you want to buy me a beer or are interested in ongoing support as a patron, [become a sponsor](https://github.com/sponsors/facioquo). Patronage motivates continued maintenance and evolution of open-source projects, and to inspire new ones.

## Contribute to help others

This NuGet package is an open-source project [on GitHub](https://github.com/DaveSkender/Stock.Indicators). If you want to report bugs or contribute fixes, new indicators, or new features, please review our [contributing guidelines](/contributing) and [the backlog](https://github.com/users/DaveSkender/projects/1).

Special thanks to all of our community code contributors!

[View contributors on GitHub](https://github.com/DaveSkender/Stock.Indicators/graphs/contributors)

» see our [full list of indicators and overlays](/indicators)
