# Stock Indicators

[![NuGet package](https://img.shields.io/nuget/v/skender.stock.indicators?color=green&label=NuGet%20Package)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![build status](https://img.shields.io/azure-devops/build/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/18/master?label=Build%20Status)](https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=18&branchName=master)
[![code coverage](https://img.shields.io/azure-devops/coverage/skender/stock.indicators/18?label=Code%20Coverage)](https://dev.azure.com/skender/Stock.Indicators/_build/latest?definitionId=18&&branchName=master&view=codecoverage-tab)
[![board status](https://dev.azure.com/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/69f29c08-2257-4429-9cea-1629abcd3064/_apis/work/boardbadge/a1dfc6ae-7836-4b56-a849-9a48698252c2)](https://dev.azure.com/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/_boards/board/t/69f29c08-2257-4429-9cea-1629abcd3064/Microsoft.RequirementCategory/)

[Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) is a multi-targeting framework .NET library that produces stock indicators.  Send in historical stock price quotes and get back desired technical indicators (such as moving average, relative strength, parabolic SAR, etc).  Nothing more.

## Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project.  See [more help](https://www.google.com/search?q=install+nuget+package) for installing packages.

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# PowerShell example
Install-Package Skender.Stock.Indicators
```

## Example usage

```csharp
using Skender.Stock.Indicators;

[..]

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);
```

## Indicators and overlays

- [Aroon Oscillator](/Indicators/Aroon/README.md)
- [Average Directional Index (ADX)](/Indicators/AvgDirectional/README.md)
- [Average True Range (ATR)](/Indicators/AvgTrueRange/README.md)
- [Beta Coefficient](/Indicators/Beta/README.md)
- [Bollinger Bands](/Indicators/BollingerBands/README.md)
- [Chandelier Exit](/Indicators/Chandelier/README.md)
- [Commodity Channel Index (CCI)](/Indicators/Cci/README.md)
- [Correlation Coefficient](/Indicators/Correlation/README.md)
- [Exponential Moving Average](/Indicators/Ema/README.md)
- [Heikin-Ashi](/Indicators/HeikinAshi/README.md)
- [Moving Average Convergence/Divergence (MACD)](/Indicators/Macd/README.md)
- [Parabolic SAR](/Indicators/ParabolicSar/README.md)
- [Relative Strength Index (RSI)](/Indicators/Rsi/README.md)
- [Simple Moving Average](/Indicators/Sma/README.md)
- [Standard Deviation](/Indicators/StandardDev/README.md)
- [Stochastic Oscillator](/Indicators/Stochastic/README.md)
- [Stochastic RSI](/Indicators/StochasticRsi/README.md)
- [Ulcer Index](/Indicators/Ulcer/README.md)

## Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You can get historical quotes from your favorite stock data provider.
Historical data is an `IEnumerable` of the `Quote` class.  [More info...](/GUIDE.md#Quote)

For additional configuration parameters, default values are provided when there is an industry standard.
You can, of course, override these and provide your own values.

## Frameworks targeted

- .NET Core 3.1
- .NET Standard 2.0, 2.1
- .NET Framework 4.6.1

## Demo

- Stock indicators can be used in any kind of stock analysis software.  We had private trading algorithms and charts in mind when originally creating this open library.
- Check out our [demo site](https://stock-charts.azurewebsites.net/) to see an example usage in a stock chart.

## FAQ

**Where can I get historical data?**

There are many places to get stock market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, try [Alpha Vantage](https://www.alphavantage.co).

**Do you have any documentation?**

Yes.  The documentation site is a GitHub Pages site here: [https://daveskender.github.io/Stock.Indicators](https://daveskender.github.io/Stock.Indicators).  It is automatically generated from the README.md files in this repository, so you can navigate from the above Indicators links too.

**More questions?**  Contact us through the NuGet [Contact Owners](https://www.nuget.org/packages/Skender.Stock.Indicators) method or [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with your question if it is publicly relevant.

## Contributing

This NuGet package is an open-source project.  If you want to report or contribute bug fixes, new indicators, or feature requests, please review our [contributing guidelines](CONTRIBUTING.md).
