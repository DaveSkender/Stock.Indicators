# Stock Indicators

[![NuGet package](https://img.shields.io/nuget/v/skender.stock.indicators?color=blue&logo=NuGet&label=NuGet%20Package)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![Nuget](https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![build status](https://img.shields.io/azure-devops/build/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/21/master?logo=AzureDevops&label=Build%20Status)](https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=21&branchName=master)
[![code coverage](https://img.shields.io/azure-devops/coverage/skender/stock.indicators/21?logo=AzureDevops&label=Code%20Coverage)](https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=21&branchName=master&view=codecoverage-tab)
[![CodeQL](https://github.com/DaveSkender/Stock.Indicators/workflows/CodeQL/badge.svg)](https://github.com/DaveSkender/Stock.Indicators/security/code-scanning)

[Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) is a .NET library that produces [stock indicators](https://daveskender.github.io/Stock.Indicators/docs/INDICATORS.html).  Send in historical stock price quotes and get back desired technical indicators (such as moving average, relative strength, stochastic oscillator, parabolic SAR, etc).  Nothing more.

It can be used in any kind of stock analysis software.  We had private trading algorithms and charts in mind when originally creating this open library.

Explore more information:

- [List of indicators and overlays](https://daveskender.github.io/Stock.Indicators/docs/INDICATORS.html)
- [Getting started](#getting-started)
- [Guide and Pro tips](https://daveskender.github.io/Stock.Indicators/docs/GUIDE.html)
- [Contributing guidelines](docs/CONTRIBUTING.md#content)
- [Release notes](https://github.com/DaveSkender/Stock.Indicators/releases)
- [Demo site](https://stock-charts.azurewebsites.net) (a stock chart that uses this library)
- [Contact us](#contact-us)

## Samples

![image](docs/examples.png)

See [full list of indicators and overlays](https://daveskender.github.io/Stock.Indicators/docs/INDICATORS.html).

## Getting started

### Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project.  See [more help](https://www.google.com/search?q=install+nuget+package) for installing packages.

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# package manager example
Install-Package Skender.Stock.Indicators
```

### Example usage

```csharp
using Skender.Stock.Indicators;

[..]  // prerequisite: acquire quote history from your own source

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);
```

See [individual indicator pages](https://daveskender.github.io/Stock.Indicators/docs/INDICATORS.html) for specific guidance.

## Frameworks targeted

- .NET Core 3.1
- .NET Standard 2.0, 2.1
- .NET Framework 4.6.1

## Contributing

[![board status](https://dev.azure.com/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/69f29c08-2257-4429-9cea-1629abcd3064/_apis/work/boardbadge/a1dfc6ae-7836-4b56-a849-9a48698252c2)](https://dev.azure.com/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/_boards/board/t/69f29c08-2257-4429-9cea-1629abcd3064/Microsoft.RequirementCategory/)

This NuGet package is an open-source project.  If you want to report or contribute bug fixes, new indicators, or feature requests, please review our [contributing guidelines](docs/CONTRIBUTING.md#content).

## Contact us

Contact us through the NuGet [Contact Owners](https://www.nuget.org/packages/Skender.Stock.Indicators) method or [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with your question if it is publicly relevant.  For extended chat, join us on our public [Microsoft Teams site](https://teams.microsoft.com/l/team/19%3a1b9209d6959e442694a28c91250ad708%40thread.skype/conversations?groupId=199d1962-4cdd-46fb-998b-7e24d0f225cf&tenantId=089fffad-8315-4180-8556-a1105149f47e).
