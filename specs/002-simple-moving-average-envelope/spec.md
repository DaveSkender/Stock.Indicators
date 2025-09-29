# Specification: Simple Moving Average Envelope

## Overview

Simple Moving Average Envelope creates upper and lower bands around a Simple Moving Average by applying a percentage offset. This indicator helps identify overbought and oversold conditions by providing dynamic support and resistance levels.

## Mathematical Foundation

### Formula
- **Middle Line**: Simple Moving Average of closing prices
- **Upper Envelope**: SMA × (1 + percentage/100)
- **Lower Envelope**: SMA × (1 - percentage/100)

### Inputs
- **Quotes**: Historical price data (OHLCV)
- **Period**: Lookback period for SMA calculation (default: 20)
- **Percentage**: Envelope percentage offset (default: 2.5%)

### Outputs
- **Upper**: Upper envelope value
- **Sma**: Simple Moving Average (middle line)
- **Lower**: Lower envelope value

## Technical Requirements

### Performance
- Target processing speed: 10,000+ quotes per second
- Memory usage: Minimize allocations, leverage existing SMA calculations
- Streaming support: Yes with buffering strategy

### Precision
- Calculation precision: double (sufficient for percentage calculations)
- Rounding behavior: Standard .NET double precision
- Null handling: Return null during warmup period

## API Design

### Method Signature
```csharp
public static IEnumerable<SmaEnvelopeResult> ToSmaEnvelope<TQuote>(
    this IEnumerable<TQuote> quotes,
    int lookbackPeriods = 20,
    double percentOffset = 2.5)
    where TQuote : IQuote
```

### Result Model
```csharp
public record SmaEnvelopeResult : ISeries
{
    public DateTime Date { get; init; }
    public double? Upper { get; init; }
    public double? Sma { get; init; }
    public double? Lower { get; init; }
}
```

### Validation Rules
- Period validation: minimum 1, maximum practical limit
- Percentage validation: greater than 0, reasonable maximum (e.g., 50%)
- Quote validation: minimum data requirements (period + 1 quotes)

## Validation Criteria

### Reference Implementation
- Source: Manual calculation using existing SMA implementation
- Test data: Standard test quotes from Data.Quotes.xlsx
- Accuracy tolerance: 0.0001 for double precision calculations

### Test Coverage
- Unit tests: All calculation paths and parameter combinations
- Edge cases: Insufficient data, boundary percentage values
- Performance tests: Large datasets, streaming scenarios
- Integration tests: Chaining with other indicators

### Performance Benchmarks
- Target speed: 10,000+ calculations per second
- Memory usage: Minimal additional allocations beyond SMA
- Streaming latency: Real-time processing with <1ms latency

## Documentation Requirements

### XML Documentation
- Complete parameter descriptions with valid ranges
- Usage examples with sample data
- Mathematical references to envelope theory
- Exception handling for invalid parameters

### Usage Examples
```csharp
// Basic usage
var results = quotes.ToSmaEnvelope(lookbackPeriods: 20, percentOffset: 2.5);

// Custom parameters
var results = quotes.ToSmaEnvelope(lookbackPeriods: 14, percentOffset: 1.5);

// Chaining with other indicators
var signals = quotes.ToSmaEnvelope(20, 2.5).Where(x => x.Sma != null);
```

---
Created: 2025-09-29
Specification ID: 002
Status: Draft
