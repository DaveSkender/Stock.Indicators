[![image](https://raw.githubusercontent.com/DaveSkender/Stock.Indicators/main/docs/assets/social-banner.png)](https://dotnet.StockIndicators.dev/)

[![GitHub Stars](https://img.shields.io/github/stars/DaveSkender/Stock.Indicators?logo=github&label=Stars)](https://github.com/DaveSkender/Stock.Indicators)
[![NuGet package](https://img.shields.io/nuget/v/skender.stock.indicators?color=blue&logo=NuGet&label=NuGet)](https://www.nuget.org/packages/Skender.Stock.Indicators)
[![NuGet](https://img.shields.io/nuget/dt/skender.stock.indicators?logo=NuGet&label=Downloads)](https://www.nuget.org/packages/Skender.Stock.Indicators)

[![Test Indicators](https://github.com/DaveSkender/Stock.Indicators/actions/workflows/test-indicators.yml/badge.svg)](https://github.com/DaveSkender/Stock.Indicators/actions/workflows/test-indicators.yml)
[![Codacy quality grade](https://app.codacy.com/project/badge/Grade/012497adc00847eca9ee91a58d00cc4f)](https://app.codacy.com/gh/DaveSkender/Stock.Indicators/dashboard)
[![Codacy code coverage](https://app.codacy.com/project/badge/Coverage/012497adc00847eca9ee91a58d00cc4f)](https://app.codacy.com/gh/DaveSkender/Stock.Indicators/dashboard)

# Stock Indicators for .NET

**Stock Indicators for .NET** is a C# [library package](https://www.nuget.org/packages/Skender.Stock.Indicators) that produces financial market technical indicators.  Send in historical price quotes and get back desired indicators such as moving averages, Relative Strength Index, Stochastic Oscillator, Parabolic SAR, etc.  Nothing more.

Build your technical analysis, trading algorithms, machine learning, charting, or other intelligent market software with this library and your own [OHLCV](https://dotnet.stockindicators.dev/guide/#historical-quotes) price quotes sources for equities, commodities, forex, cryptocurrencies, and others.  [Stock Indicators for Python](https://python.stockindicators.dev/) is also available.

## Quick Start

### Installation

```bash
dotnet add package Skender.Stock.Indicators
```

### Basic Usage

```csharp
using Skender.Stock.Indicators;

// Get historical quotes from your data source
IEnumerable<IQuote> quotes = GetHistoricalQuotes();

// Calculate 20-period simple moving average
IEnumerable<SmaResult> results = quotes.GetSma(20);
```

Visit our project site for more information:

- [Overview](https://dotnet.stockindicators.dev/)
- [Indicators and overlays](https://dotnet.stockindicators.dev/indicators/)
- [Guide and Pro tips](https://dotnet.stockindicators.dev/guide/)
- [Demo site](https://charts.stockindicators.dev/) (a stock chart)
- [Release notes](https://github.com/DaveSkender/Stock.Indicators/releases)
- [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions)
- [Contributing](https://dotnet.stockindicators.dev/contributing/)

## Development Setup

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later (we test against .NET 6.0 and 9.0)
- An IDE such as [Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Building the Project

1. **Clone the repository**
   ```bash
   git clone https://github.com/DaveSkender/Stock.Indicators.git
   cd Stock.Indicators
   ```

2. **Restore dependencies and build**
   ```bash
   dotnet restore
   dotnet build --configuration Release
   ```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/indicators/Tests.Indicators.csproj

# Run tests with coverage (requires appropriate tooling)
dotnet test --collect:"XPlat Code Coverage"
```

### Development Branch

- Primary development branch: `main`
- All pull requests should target the `main` branch
- Releases are tagged and deployed from `main`

For detailed development guidelines, see [CONTRIBUTING.md](CONTRIBUTING.md).

## Support

- 📖 [Documentation](https://dotnet.stockindicators.dev/)
- 💬 [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions) - Get help and share ideas
- 🐛 [Issues](https://github.com/DaveSkender/Stock.Indicators/issues) - Report bugs or request features
- 💎 [Sponsorship](https://github.com/sponsors/facioquo) - Support the project
