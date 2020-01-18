# Stock Indicators

[Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) is a multi-targeting framework .NET library that produces stock indicators.  Send in historical stock price quotes and get back desired technical indicators (such as moving average, relative strength, parabolic SAR, etc).  Nothing more.

![Build Status](https://dev.azure.com/skender/Stock.Indicators/_apis/build/status/Stock.Indicators?branchName=master)

## Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) package into your Project.  See [more help](https://www.google.com/search?q=install+nuget+package) for installing NuGet packages;

``` PowerShell
PM> Install-Package Skender.Stock.Indicators
```

## Example usage

``` C#
using Skender.Stock.Indicators;

[..]

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);
```

## How to use for individual indicators

- [Average Directional Index (ADX)](/Indicators/AvgDirectional/README.md)
- [Average True Range (ATR)](/Indicators/AvgTrueRange/README.md)
- [Bollinger Bands](/Indicators/BollingerBands/README.md)
- [Commodity Channel Index (CCI)](/Indicators/Cci/README.md)
- [Exponential Moving Average](/Indicators/Ema/README.md)
- [Heikin-Ashi](/Indicators/HeikinAshi/README.md)
- [Moving Average Convergence/Divergence (MACD)](/Indicators/Macd/README.md)
- [Parabolic SAR](/Indicators/ParabolicSar/README.md)
- [Relative Strength Index (RSI)](/Indicators/Rsi/README.md)
- [Simple Moving Average](/Indicators/Sma/README.md)
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

## Q&A

**Where can I get historical data?**

There are many places to get stock market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, try [Alpha Vantage](https://www.alphavantage.co).

## Contributing

This NuGet package is an open-source project.  If you want to report or contribute bug fixes, new indicators, or feature requests, please review our [contributing guidelines](CONTRIBUTING.md).
