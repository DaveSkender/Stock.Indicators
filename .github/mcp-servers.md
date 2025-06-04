# MCP Servers Configuration for Stock Indicators for .NET

This directory contains Model Context Protocol (MCP) server configurations to extend GitHub Copilot's capabilities when working with this financial indicators library.

## Available MCP Servers

### 1. Financial Mathematics Server
**Purpose**: Provides mathematical functions and financial calculations context
**Configuration**:
```json
{
  "name": "financial-math",
  "description": "Financial mathematics and statistical functions",
  "capabilities": [
    "statistical_analysis",
    "financial_formulas", 
    "mathematical_validation",
    "precision_arithmetic"
  ]
}
```

### 2. .NET Performance Analyzer
**Purpose**: Analyzes .NET code performance patterns and suggests optimizations
**Configuration**:
```json
{
  "name": "dotnet-performance",
  "description": ".NET performance analysis and optimization",
  "capabilities": [
    "memory_analysis",
    "performance_profiling",
    "span_optimization",
    "linq_performance"
  ]
}
```

### 3. Technical Analysis Domain Expert
**Purpose**: Provides domain knowledge about technical analysis indicators
**Configuration**:
```json
{
  "name": "technical-analysis",
  "description": "Technical analysis domain expertise",
  "capabilities": [
    "indicator_definitions",
    "formula_validation",
    "trading_terminology",
    "market_data_standards"
  ]
}
```

## Setup Instructions

1. **Install MCP Runtime** (when available):
   ```bash
   npm install -g @microsoft/mcp-runtime
   ```

2. **Configure MCP Servers**:
   - Copy the appropriate server configurations to your local MCP directory
   - Ensure server executables are in your PATH
   - Test connectivity: `mcp test-connection financial-math`

3. **GitHub Copilot Integration**:
   - MCP servers will be automatically detected by GitHub Copilot
   - Additional context will be provided for financial and mathematical code
   - Performance suggestions will be enhanced with .NET-specific guidance

## Custom MCP Server Development

For this repository, you can create custom MCP servers to provide:

### Financial Data Server
```typescript
// Example structure for financial data MCP server
interface FinancialDataServer {
  getHistoricalData(symbol: string, period: string): Promise<OhlcData[]>;
  validateIndicatorInputs(indicatorName: string, params: any): boolean;
  getIndicatorReference(name: string): IndicatorReference;
}
```

### Performance Optimization Server  
```typescript
// Example structure for performance MCP server
interface PerformanceServer {
  analyzeMemoryUsage(code: string): MemoryAnalysis;
  suggestSpanOptimizations(method: string): OptimizationSuggestion[];
  benchmarkAlgorithm(algorithm: string): BenchmarkResult;
}
```

## Integration Benefits

With MCP servers configured, GitHub Copilot will have enhanced capabilities:

- **Mathematical Accuracy**: Validation of financial formulas and calculations
- **Performance Awareness**: Suggestions optimized for high-frequency financial data processing
- **Domain Expertise**: Technical analysis terminology and best practices
- **.NET Optimization**: Framework-specific performance patterns and memory management

## Future Enhancements

Planned MCP server integrations:
- Real-time market data feeds for testing
- Backtesting framework integration
- Advanced mathematical libraries (e.g., QuantLib.NET)
- Cloud financial data services (Alpha Vantage, Yahoo Finance)

## Troubleshooting

Common issues and solutions:
- **Server not found**: Verify server executable is in PATH
- **Connection timeout**: Check network permissions and firewall settings
- **Invalid configuration**: Validate JSON syntax in server configurations
- **Performance degradation**: Monitor MCP server resource usage

For detailed setup and troubleshooting, refer to the official MCP documentation.