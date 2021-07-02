# Stock.Indicators for .NET

[![NuGet package](https://img.shields.io/nuget/v/skender.stock.indicators?color=blue&logo=NuGet&label=NuGet%20Package)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![Nuget](https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![build status](https://img.shields.io/azure-devops/build/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/21/master?logo=AzureDevops&label=Build%20Status)](https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=21&branchName=master)
[![code coverage](https://img.shields.io/azure-devops/coverage/skender/stock.indicators/21/master?logo=AzureDevOps&label=Code%20Coverage)](https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=21&branchName=master&view=codecoverage-tab)
[![CodeQL](https://github.com/DaveSkender/Stock.Indicators/workflows/CodeQL/badge.svg)](https://github.com/DaveSkender/Stock.Indicators/security/code-scanning)

[Stock.Indicators for .NET](https://www.nuget.org/packages/Skender.Stock.Indicators) is a C# library package that produces financial market technical indicators.  Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc.  Nothing more.

It can be used in any market analysis software using standard OHLCV price quotes for equities, commodities, forex, cryptocurrencies, and others.  We had private trading algorithms, machine learning, and charting systems in mind when originally creating this community library.

Explore more information:

- [Indicators and overlays](https://daveskender.github.io/Stock.Indicators/docs/INDICATORS.html)
- [Guide and Pro tips](https://daveskender.github.io/Stock.Indicators/docs/GUIDE.html)
- [Contributing guidelines](https://daveskender.github.io/Stock.Indicators/docs/CONTRIBUTING.html)
- [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions)
- [Release notes](https://github.com/DaveSkender/Stock.Indicators/releases)
- [Demo site](https://stock-charts.azurewebsites.net) (a stock chart)
- [Contact us](#contact-us)

## Samples

![image](docs/examples.png)

### Example usage

```csharp
using Skender.Stock.Indicators;

[..]  // prerequisite: get quote history from your own source

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = history.GetSma(20);
```

See the [guide](https://daveskender.github.io/Stock.Indicators/docs/GUIDE.html) and the [full list of indicators and overlays](https://daveskender.github.io/Stock.Indicators/docs/INDICATORS.html) for more information.

## Frameworks targeted

- .NET 5.0
- .NET Core 3.1
- .NET Standard 2.0, 2.1
- .NET Framework 4.6.1

The compiled library package is [Common Language Specification (CLS) compliant](https://docs.microsoft.com/en-us/dotnet/standard/common-type-system) and can be used in other programming languages, including Python and everything in the .NET universe.

## Contributing

This NuGet package is an open-source community project.  If you want to report bugs or contribute fixes, new indicators, or new features, please review our [contributing guidelines](docs/CONTRIBUTING.md#content) and [the backlog](https://github.com/DaveSkender/Stock.Indicators/projects/1).

## License

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

This repository uses the standard Apache 2.0 open-source license.  Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## :heart: Patronage

If you or your organization use any of my projects or like what I’m doing, please add a :star: on the [GitHub Repo](https://github.com/DaveSkender/Stock.Indicators) as a token of appreciation.
If you want to buy me a beer or are interest in ongoing support as a patron, [become a sponsor](https://github.com/sponsors/DaveSkender).
Patronage motivates continued maintenance and evolution of open-source projects, and to inspire new ones.
Thank you for your support!

## :phone: Contact us

[Start a new feature discussion, ask a question](https://github.com/DaveSkender/Stock.Indicators/discussions), or [submit an issue](https://github.com/DaveSkender/Stock.Indicators/issues) if it is publicly relevant.  You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).
