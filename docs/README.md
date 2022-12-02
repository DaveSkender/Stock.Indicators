# Stock Indicators for .NET

<a href="https://www.nuget.org/packages/Skender.Stock.Indicators"><img src="https://img.shields.io/nuget/v/skender.stock.indicators?color=blue&logo=NuGet&label=NuGet%20Package" width="155" height="20"></a>
<a href="https://www.nuget.org/packages/Skender.Stock.Indicators"><img src="https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads" width="125" height="20"></a>
<a href="https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=21&branchName=main&view=codecoverage-tab"><img src="https://img.shields.io/azure-devops/coverage/skender/stock.indicators/21/main?logo=AzureDevOps&label=Test%20Coverage" width="149" height="20"></a>

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that produces financial market technical indicators.  Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc.  Nothing more.

It can be used in any market analysis software using standard [OHLCV]({{site.baseurl}}/guide/#historical-quotes) price quotes for equities, commodities, forex, cryptocurrencies, and others.  We had private trading algorithms, machine learning, and charting systems in mind when originally creating this library.  [Stock Indicators for Python](https://python.stockindicators.dev/) is also available.

Explore more information:

- [Indicators and overlays]({{site.baseurl}}/indicators/#content)
- [Guide and Pro tips]({{site.baseurl}}/guide/#content)
- [Utilities and helper functions]({{site.baseurl}}/utilities/#content)
- [Demo site](https://charts.stockindicators.dev/) (a stock chart)
- [Example usage code]({{site.baseurl}}/examples/#content)
- [Release notes]({{site.github.repository_url}}/releases)
- [Discussions]({{site.github.repository_url}}/discussions)
- [Contributing guidelines]({{site.baseurl}}/contributing/#content)

## Samples

![image](examples.webp)

### Basic usage

```csharp
using Skender.Stock.Indicators;

[..]  // prerequisite: get historical quotes from your own source

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = quotes.GetSma(20);
```

### Advanced usage

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

## Frameworks targeted

- .NET 7.0, 6.0
- .NET Standard 2.1, 2.0

The compiled library package is [Common Language Specification (CLS) compliant](https://docs.microsoft.com/en-us/dotnet/standard/common-type-system) and can be used in other programming languages, including Python and everything in the .NET universe.

## License

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](https://opensource.org/licenses/Apache-2.0)

This repository uses the standard Apache 2.0 open-source license.  Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## :phone: Contact us

[Start a new discussion, ask a question]({{site.github.repository_url}}/discussions), or [submit an issue]({{site.github.repository_url}}/issues) if it is publicly relevant.  You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).

## :heart: Patronage

This software is crafted with care by unpaid enthusiasts who appreciate all forms of encouragement.  If you or your organization use any of my projects or like what we're doing, please add a :star: on the [GitHub Repo]({{site.github.repository_url}}) as a token of appreciation.  If you want to buy me a beer or are interest in ongoing support as a patron, [become a sponsor](https://github.com/sponsors/DaveSkender).  Patronage motivates continued maintenance and evolution of open-source projects, and to inspire new ones.  Thank you for your support!

## :octocat: Contributing

This NuGet package is an open-source project.  If you want to report bugs or contribute fixes, new indicators, or new features, please review our [contributing guidelines]({{site.baseurl}}/contributing/#content) and [the backlog](https://github.com/users/DaveSkender/projects/1).

Special thanks to all of our community code contributors!

<ul class="list-style-none">
{% for contributor in site.github.contributors %}
  <li class="d-inline-block">
     <a href="{{ contributor.html_url }}"><img src="{{ contributor.avatar_url }}&s=75" width="75" height="75" class="circle" alt="{{ contributor.login }}" /></a>
  </li>
{% endfor %}
</ul>
