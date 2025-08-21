# Stock Indicators v3 Migration Guide

This guide provides a comprehensive migration path from v2 to v3 of the Stock Indicators library. Version 3 introduces significant improvements including streaming support, immutable record types, and a more consistent API surface.

## 🚀 Quick Start

The most common changes you'll need to make:

1. **Change method names**: `GetSma()` → `ToSma()`
2. **Update Use() calls**: `quotes.Use()` → `quotes.Use(CandlePart.Close)`
3. **Handle record types**: Results are now immutable records instead of classes

## 📋 Breaking Changes Summary

### API Method Naming
- **All indicator methods**: `GetX()` → `ToX()`
- **Examples**: `GetSma()` → `ToSma()`, `GetRsi()` → `ToRsi()`, `GetEma()` → `ToEma()`

### Quote and Result Types
- **Quote type**: Now an immutable `record` instead of `class`
- **IQuote.Date**: Renamed to `IQuote.Timestamp`
- **All result types**: Now immutable `record` types instead of `sealed class`
- **IReusableResult**: Renamed to `IReusable`

### Utility Methods
- **Use() method**: Parameter `candlePart` is now required (no default)
- **Use() return type**: Returns `QuotePart` instead of tuple
- **Removed utilities**: `SyncSeries()`, `Find()`, `FindIndex()`, `GetBaseQuote()`

## 🔄 Step-by-Step Migration

### 1. Update Indicator Method Names

**Before (v2):**
```csharp
var smaResults = quotes.GetSma(20);
var rsiResults = quotes.GetRsi(14);
var emaResults = quotes.GetEma(10);
```

**After (v3):**
```csharp
var smaResults = quotes.ToSma(20);
var rsiResults = quotes.ToRsi(14);
var emaResults = quotes.ToEma(10);
```

### 2. Update Use() Method Calls

**Before (v2):**
```csharp
// Default to Close price
var closePrices = quotes.Use();

// Explicit parameter
var highPrices = quotes.Use(CandlePart.High);
```

**After (v3):**
```csharp
// Must specify candlePart explicitly
var closePrices = quotes.Use(CandlePart.Close);

// Same for other parts
var highPrices = quotes.Use(CandlePart.High);
```

### 3. Handle Record Types

**Before (v2):**
```csharp
// Results were mutable classes
foreach (var result in smaResults)
{
    result.Sma = ModifyValue(result.Sma); // This was allowed
}
```

**After (v3):**
```csharp
// Results are immutable records
foreach (var result in smaResults)
{
    // result.Sma = value; // ❌ Compilation error - immutable
    var newValue = ModifyValue(result.Sma); // ✅ Read-only access
}
```

### 4. Update Quote Initialization

**Before (v2):**
```csharp
var quote = new Quote
{
    Date = DateTime.Now,      // ❌ Old property name
    Open = 100,
    High = 105,
    Low = 99,
    Close = 102,
    Volume = 1000
};
```

**After (v3):**
```csharp
var quote = new Quote
{
    Timestamp = DateTime.Now, // ✅ New property name
    Open = 100,
    High = 105,
    Low = 99,
    Close = 102,
    Volume = 1000
};
```

### 5. Update Custom Quote Types

**Before (v2):**
```csharp
public class MyQuote : IQuote
{
    public DateTime Date { get; set; }     // ❌ Old property name
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
```

**After (v3):**
```csharp
public class MyQuote : IQuote, IReusable  // ✅ Implement IReusable
{
    public DateTime Timestamp { get; set; } // ✅ New property name
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
    
    // Required by IReusable
    public double Value => (double)Close;   // ✅ Add Value property
}
```

### 6. Update Property Access

**Before (v2):**
```csharp
// UlcerIndex had different property name
foreach (var result in ulcerResults)
{
    Console.WriteLine(result.UI); // ❌ Old property name
}
```

**After (v3):**
```csharp
// Property was renamed for consistency
foreach (var result in ulcerResults)
{
    Console.WriteLine(result.UlcerIndex); // ✅ New property name
}
```

### 7. Replace Removed Utilities

**Before (v2):**
```csharp
// These utilities were removed
var synced = quotes.SyncSeries(otherQuotes, SyncType.Prepend);
var found = results.Find(date);
var index = results.FindIndex(date);
var baseQuotes = quotes.GetBaseQuote();
```

**After (v3):**
```csharp
// Use alternative approaches
// For synchronization - implement custom logic or use LINQ
var synced = /* custom sync implementation */;

// For finding - use LINQ
var found = results.FirstOrDefault(x => x.Timestamp == date);
var index = results.ToList().FindIndex(x => x.Timestamp == date);

// GetBaseQuote was removed - use Use() instead
var quoteParts = quotes.Use(CandlePart.Close);
```

## 🎯 Common Migration Patterns

### Chaining Operations
```csharp
// v3 supports better chaining with QuotePart
var result = quotes
    .Use(CandlePart.Close)
    .ToSma(20)
    .ToRsi(14);
```

### Working with Results
```csharp
// Results are now records - use with expressions for modifications
var modifiedResult = originalResult with { Sma = newValue };
```

### Null Handling
```csharp
// v2: Nullable properties
if (result.Sma.HasValue)
{
    var value = result.Sma.Value;
}

// v3: NaN for missing values
if (!double.IsNaN(result.Sma))
{
    var value = result.Sma;
}
```

## ⚠️ Important Notes

1. **Immutability**: All result types are now immutable. You cannot modify properties after creation.

2. **Performance**: Record types may have different performance characteristics. Test thoroughly if performance is critical.

3. **Breaking Changes**: This is a major version update with breaking changes. Update all references at once.

4. **Obsolete Warnings**: Many v2 methods are marked obsolete but still work. Plan to migrate them gradually.

5. **Custom Types**: If you have custom quote or result types, they must implement the new interfaces correctly.

## 🧪 Testing Your Migration

After migration, ensure:

1. **All compilation errors are resolved**
2. **Unit tests pass** 
3. **Results are mathematically identical** to v2 (except for type differences)
4. **Performance is acceptable** for your use case
5. **No obsolete warnings remain** in your code

## 📚 Additional Resources

- Check the [API documentation](https://dotnet.stockindicators.dev) for complete reference
- Review the [performance benchmarks](https://dotnet.stockindicators.dev/performance/) for v3
- See the [GitHub releases](https://github.com/DaveSkender/Stock.Indicators/releases) for detailed changelog
