---
title: Creating custom indicators
description: Learn how to create your own custom technical indicators using the Stock Indicators for .NET library as a foundation. This guide shows you how to build custom indicators that integrate seamlessly with the library's existing features.
---

# Creating custom indicators

At some point in your journey, you may want to create your own custom indicators. The following guide shows you how to create custom indicators that work seamlessly with this library.

::: tip
Working example code is available in the [CustomIndicatorsLibrary](https://github.com/DaveSkender/Stock.Indicators/tree/main/docs/examples/CustomIndicatorsLibrary) project.
:::

## Creating a custom indicator

### Step 1: Create the result class

Create your results class by implementing the `IReusable` interface or inheriting from existing result patterns. This allows your custom indicator to be chainable with other indicators.

```csharp
using Skender.Stock.Indicators;

namespace Custom.Indicators;

// Custom results class
public record AtrWmaResult : IReusable
{
    public DateTime Timestamp { get; init; }
    public double? AtrWma { get; init; }
    
    // Required for IReusable interface (enables chaining)
    double IReusable.Value => AtrWma.Null2NaN();
}
```

### Step 2: Create your custom indicator

Create your custom algorithm following the same patterns as the main library.

```csharp
using Skender.Stock.Indicators;

namespace Custom.Indicators;

public static class CustomIndicators
{
    /// <summary>
    /// ATR-weighted moving average (custom indicator example)
    /// </summary>
    /// <param name="quotes">Historical quotes</param>
    /// <param name="lookbackPeriods">Lookback period</param>
    /// <returns>Collection of AtrWmaResult</returns>
    public static IReadOnlyList<AtrWmaResult> ToAtrWma(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 10)
    {
        // Validate parameters
        quotes.ThrowIfNull();
        
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods),
                "Lookback periods must be greater than 0.");
        }

        // Sort quotes
        List<IQuote> quotesList = quotes.ToSortedList();

        // Check for sufficient quotes
        if (quotesList.Count < lookbackPeriods)
        {
            return [];
        }

        // Initialize results
        List<AtrWmaResult> results = new(quotesList.Count);

        // Get ATR values (prerequisite indicator)
        IReadOnlyList<AtrResult> atrResults = quotesList.ToAtr(lookbackPeriods);

        // Calculate custom indicator
        for (int i = 0; i < quotesList.Count; i++)
        {
            IQuote q = quotesList[i];

            AtrWmaResult r = new()
            {
                Timestamp = q.Timestamp
            };

            // Calculate only after warmup period
            if (i >= lookbackPeriods - 1)
            {
                double sumWma = 0;
                double sumAtr = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    double close = (double)quotesList[p].Close;
                    double? atr = atrResults[p].Atr;

                    if (atr.HasValue)
                    {
                        sumWma += atr.Value * close;
                        sumAtr += atr.Value;
                    }
                }

                r = r with 
                { 
                    AtrWma = sumAtr != 0 ? sumWma / sumAtr : null 
                };
            }

            results.Add(r);
        }

        return results;
    }
}
```

### Step 3: Use your custom indicator

Use your custom indicator just like the built-in indicators:

```csharp
using Skender.Stock.Indicators;
using Custom.Indicators;

// Get historical quotes
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("MSFT");

// Calculate custom indicator
IReadOnlyList<AtrWmaResult> results = quotes.ToAtrWma(10);

// Use results
foreach (AtrWmaResult r in results)
{
    Console.WriteLine(
        $"ATR WMA on {r.Timestamp:d} was {r.AtrWma:N4}");
}
```

## Advanced patterns

### Chainable indicators

By implementing `IReusable`, your custom indicator can be chained with other indicators:

```csharp
// Chain your custom indicator with RSI
var rsiOfAtrWma = quotes
    .ToAtrWma(10)
    .ToRsi(14);
```

### Using multiple indicator styles

You can also implement your custom indicator in other styles:

- **Buffer list style** - For incremental processing
- **Stream hub style** - For real-time data feeds

See the [Guide](/guide) for more information about different indicator styles.

### Best practices

When creating custom indicators:

1. **Validate inputs** - Always validate parameters and quotes
2. **Handle edge cases** - Check for insufficient data, null values
3. **Follow naming conventions** - Use `To{IndicatorName}` pattern
4. **Implement IReusable** - Enable chaining with other indicators
5. **Add XML documentation** - Document parameters and return values
6. **Test thoroughly** - Verify calculations against reference data

## Example projects

For complete working examples, see:

- [CustomIndicatorsLibrary](https://github.com/DaveSkender/Stock.Indicators/tree/main/docs/examples/CustomIndicatorsLibrary) - Shows how to create custom indicators
- [CustomIndicatorsUsage](https://github.com/DaveSkender/Stock.Indicators/tree/main/docs/examples/CustomIndicatorsUsage) - Shows how to use custom indicators

## See also

- [Utilities and helpers](/utilities/) - Tools for working with quotes and results
- [Guide](/guide) - General usage patterns and indicator styles
- [Contributing guidelines](/contributing) - How to contribute indicators to the library
