---
title: Stock Indicators for .NET
description: >-
  Transform financial market prices into technical analysis insights with this best in class C# NuGet library.
  Go further with chaining and custom indicators.
permalink: /
layout: base
lazy-images: true
---

<p style="text-align:center;">
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Get the NuGet package." class="not-mobile">
  <img src="https://img.shields.io/nuget/v/skender.stock.indicators?logo=NuGet&label=NuGet%20Package&color=blue&cacheSeconds=259200"
  alt="" width="170" height="20" />
</a>
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators" aria-label="Read more about package downloads." class="not-mobile">
  <img src="https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads&cacheSeconds=259200"
  alt="" width=130 height="20" />
</a>
</p>

<h1 style="display:none;">{{ page.title }}</h1>

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that produces financial market technical indicators.  Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc.  Nothing more.

Build your private technical analysis, trading algorithms, machine learning, charting, or other intelligent market software with this library and your own [OHLCV]({{site.baseurl}}/guide/#historical-quotes) price quotes sources for equities, commodities, forex, cryptocurrencies, and others.  [Stock Indicators for Python](https://python.stockindicators.dev/) is also available.

Explore more information:

- [Indicators and overlays]({{site.baseurl}}/indicators/#content)
- [Guide and Pro tips]({{site.baseurl}}/guide/#content)
- [Utilities and helper functions]({{site.baseurl}}/utilities/#content)
- [Demo site](https://charts.stockindicators.dev/) (a stock chart)
- [Example usage code]({{site.baseurl}}/examples/#content)
- [Release notes]({{site.github.repository_url}}/releases)
- [Discussions]({{site.github.repository_url}}/discussions)
- [Contributing guidelines]({{site.baseurl}}/contributing/#content)

## Reputable and extensible indicators

You'll get all of the industry standard indicators out-of-the-box.  Additionally, you can create compatible [custom indicators]({{site.baseurl}}/custom-indicators/#content).

<img alt="sample indicators shown in chart"
  data-src="examples.webp"
  style="aspect-ratio:900/748;"
  class = "lazyload" />

## Easy to use in your application

```csharp
// example: get 20-period simple moving average
IEnumerable<SmaResult> results = quotes.GetSma(20);
```

See more [usage examples]({{site.baseurl}}/guide/#example-usage).

## Use chaining for unique insights

Optional chaining enables advanced uses cases; such as, indicator of indicators, [slope]({{site.baseurl}}/indicators/Slope/#content) (direction) of any result, or [moving average]({{site.baseurl}}/indicators/#moving-average) of an indicator.

```csharp
// example: advanced chaining (RSI of OBV)
IEnumerable<RsiResult> results
  = quotes
    .GetObv()
    .GetRsi(14);

// example: use any candle variant
IEnumerable<EmaResult> results
  = quotes
    .Use(CandlePart.HL2)
    .GetEma(20);
```

See the [guide]({{site.baseurl}}/guide/#content) and the [full list of indicators and overlays]({{site.baseurl}}/indicators/#content) for more information.

## Optimized for modern .NET frameworks

Our [NuGet library](https://www.nuget.org/packages/Skender.Stock.Indicators) directly targets all current frameworks for peak performance, including the .NET Standard for older framework compatibility.

- .NET 8.0, 7.0, 6.0
- .NET Standard 2.1, 2.0

The compiled library package is [Common Language Specification (CLS) compliant](https://docs.microsoft.com/en-us/dotnet/standard/common-type-system) and can be used in other programming languages, including Python and everything in the .NET universe.

## Licensed for everyone

<a href="https://opensource.org/licenses/Apache-2.0"><img src="https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square&cacheSeconds=259200" alt="Apache 2.0 license badge" width="124" height="20" class="lazyload" /></a>

This repository uses the standard Apache 2.0 open-source license.  Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## Share your ideas with the community

**Need help?**  Have ideas?  [Start a new discussion, ask a question &#128172;]({{site.github.repository_url}}/discussions), or [submit an issue]({{site.github.repository_url}}/issues) if it is publicly relevant.  You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).

## Give back with patronage

Thank you for your support!  This software is crafted with care by unpaid enthusiasts who &#128150; all forms of encouragement.  If you or your organization use this library or like what we're doing, add a &#11088; on the [GitHub Repo]({{site.github.repository_url}}) as a token of appreciation.

If you want to buy me a beer or are interested in ongoing support as a patron, [become a sponsor](https://github.com/sponsors/DaveSkender).  Patronage motivates continued maintenance and evolution of open-source projects, and to inspire new ones.

## Contribute to help others

This NuGet package is an open-source project [on GitHub](https://github.com/DaveSkender/Stock.Indicators).  If you want to report bugs or contribute fixes, new indicators, or new features, please review our [contributing guidelines]({{site.baseurl}}/contributing/#content) and [the backlog](https://github.com/users/DaveSkender/projects/1).

Special thanks to all of our community code contributors!

<ul class="list-style-none">
{% for contributor in site.github.contributors %}
  <li class="d-inline-block">
     <a href="{{ contributor.html_url }}" width="75" height="75"><img data-src="{{ contributor.avatar_url }}&s=75" width="75" height="75" class="circle lazyload" alt="{{ contributor.login }} avatar" /></a>
  </li>
{% endfor %}
</ul>

&#187; see our [full list of indicators and overlays]({{site.baseurl}}/indicators/#content)
