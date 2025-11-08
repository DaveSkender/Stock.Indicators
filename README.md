<!-- markdownlint-disable MD041 -->
[![image](https://raw.githubusercontent.com/DaveSkender/Stock.Indicators/main/docs/assets/social-banner.png)](https://dotnet.StockIndicators.dev/)

[![GitHub Stars](https://img.shields.io/github/stars/DaveSkender/Stock.Indicators?logo=github&label=Stars)](https://github.com/DaveSkender/Stock.Indicators)
[![NuGet package](https://img.shields.io/nuget/v/skender.stock.indicators?color=blue&logo=NuGet&label=NuGet)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![NuGet](https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads)](https://www.nuget.org/packages/Skender.Stock.Indicators)

> [!IMPORTANT]
> This branch contains **vNext (v3)** code that is under development. For the currently released stable version (v2), please see the [`main` branch](https://github.com/DaveSkender/Stock.Indicators/blob/main/README.md).

# Stock Indicators for .NET

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that produces financial market technical indicators.  Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc.  Nothing more.

Build your technical analysis, trading algorithms, machine learning, charting, or other intelligent market software with this library and your own [OHLCV](https://dotnet.stockindicators.dev/guide/#historical-quotes) price quotes sources for equities, commodities, forex, cryptocurrencies, and others.  [Stock Indicators for Python](https://python.stockindicators.dev/) is also available.

## Streaming Support

v3 introduces comprehensive **streaming capabilities** for real-time and incremental data processing. Most indicators now support three calculation styles:

- **Series** - Traditional batch processing for complete historical datasets
- **BufferList** - Incremental calculations with efficient buffer management
- **StreamHub** - Real-time processing with observable patterns and state management

Quick example using streaming:

```csharp
// Create a quote hub for streaming quotes
QuoteHub<Quote> quoteHub = new();

// Subscribe indicators to the hub
EmaHub<Quote> emaHub = quoteHub.ToEma(20);
RsiHub<Quote> rsiHub = quoteHub.ToRsi(14);

// Stream quotes as they arrive
foreach (Quote quote in liveQuotes)
{
    quoteHub.Add(quote);
    
    // Access real-time results
    EmaResult emaResult = emaHub.Results.LastOrDefault();
    RsiResult rsiResult = rsiHub.Results.LastOrDefault();
}
```

Visit our project site for more information:

- [Overview](https://dotnet.stockindicators.dev/)
- [Indicators and overlays](https://dotnet.stockindicators.dev/indicators/)
- [Guide and Pro tips](https://dotnet.stockindicators.dev/guide/)
- [Demo site](https://charts.stockindicators.dev/) (a stock chart)
- [Release notes](https://github.com/DaveSkender/Stock.Indicators/releases)
- [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions)
- [Contributing](https://dotnet.stockindicators.dev/contributing/)
