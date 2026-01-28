---
title: Numerical methods
description: Mathematical analysis functions for custom indicators.
---

# Numerical methods

Mathematical analysis functions including slope calculation and standard deviation. These are the same internal tools used by the library's built-in indicators.

## Slope

Calculate the slope of a linear regression line from paired x and y values.

### Syntax

```csharp
double slope = Numerix.Slope(double[] xValues, double[] yValues);
```

### Parameters

**xValues** - Array of x-axis values (independent variable)

**yValues** - Array of y-axis values (dependent variable)

### Returns

**double** - The slope of the best-fit line through the data points.

### Usage

```csharp
double[] xValues = { 1, 2, 5, 4 };
double[] yValues = { 4, 7, 8, 1 };
double slope = Numerix.Slope(xValues, yValues);

Console.WriteLine($"Slope: {slope:F4}");
```

### Common use cases

#### Trend detection

Calculate the trend direction of price data:

```csharp
// get last 20 close prices
var closes = quotes.TakeLast(20).Select(q => (double)q.Close).ToArray();
var periods = Enumerable.Range(1, 20).Select(i => (double)i).ToArray();

double trend = Numerix.Slope(periods, closes);

if (trend > 0)
  Console.WriteLine("Uptrend detected");
else if (trend < 0)
  Console.WriteLine("Downtrend detected");
```

#### Custom linear regression indicator

Build a custom linear regression indicator:

```csharp
public static IReadOnlyList<LinRegResult> ToLinearRegression(
  this IEnumerable<IQuote> quotes, int period)
{
  var quotesList = quotes.ToList();
  var results = new List<LinRegResult>();
  
  for (int i = period - 1; i < quotesList.Count; i++)
  {
    var window = quotesList.Skip(i - period + 1).Take(period);
    var closes = window.Select(q => (double)q.Close).ToArray();
    var periods = Enumerable.Range(1, period).Select(p => (double)p).ToArray();
    
    var slope = Numerix.Slope(periods, closes);
    
    results.Add(new LinRegResult {
      Timestamp = quotesList[i].Timestamp,
      Slope = slope
    });
  }
  
  return results;
}
```

## Standard deviation

Calculate the standard deviation of a collection of values.

### Syntax

```csharp
double sd = values.StdDev();
```

### Parameters

**values** - An array or enumerable collection of double values.

### Returns

**double** - The standard deviation of the values.

### Usage

```csharp
double[] values = { 1, 2, 3, 4 };
double sd = values.StdDev();

Console.WriteLine($"Standard Deviation: {sd:F4}");
```

### Common use cases

#### Volatility measurement

Measure price volatility over a period:

```csharp
// calculate 20-period volatility
var closes = quotes
  .TakeLast(20)
  .Select(q => (double)q.Close)
  .ToArray();

double volatility = closes.StdDev();
Console.WriteLine($"Volatility: {volatility:F2}");
```

#### Custom Bollinger Bands

Build custom Bollinger Bands using standard deviation:

```csharp
public static IReadOnlyList<BollingerResult> ToCustomBollinger(
  this IEnumerable<IQuote> quotes, int period, double stdDevMultiplier)
{
  var quotesList = quotes.ToList();
  var results = new List<BollingerResult>();
  
  for (int i = period - 1; i < quotesList.Count; i++)
  {
    var window = quotesList.Skip(i - period + 1).Take(period);
    var closes = window.Select(q => (double)q.Close).ToArray();
    
    var sma = closes.Average();
    var stdDev = closes.StdDev();
    
    results.Add(new BollingerResult {
      Timestamp = quotesList[i].Timestamp,
      Middle = sma,
      Upper = sma + (stdDev * stdDevMultiplier),
      Lower = sma - (stdDev * stdDevMultiplier)
    });
  }
  
  return results;
}
```

#### Z-Score calculation

Calculate Z-Scores for statistical analysis:

```csharp
var closes = quotes.Select(q => (double)q.Close).ToArray();
var mean = closes.Average();
var stdDev = closes.StdDev();

// calculate Z-Score for each close
var zScores = closes.Select(c => (c - mean) / stdDev);
```

## Performance considerations

::: tip Performance
Both `Slope()` and `StdDev()` are optimized for performance and are used internally by the library's built-in indicators. However, for large datasets or frequent calculations, consider caching results when possible.
:::

## Related utilities

- [Helper utilities overview](/utilities/helpers/)
- [NullMath](/utilities/helpers/nullmath) - Null-safe mathematical operations
- [Customization guide](/customization) - Build custom indicators
- [Slope indicator](/indicators/Slope) - Built-in linear regression indicator
- [Standard Deviation indicator](/indicators/StdDev) - Built-in standard deviation indicator
