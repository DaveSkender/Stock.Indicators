# Gator Oscillator

 Created by Bill Williams, the Gator Oscillator is an expanded oscillator view of Williams Alligator's three moving averages.



Created by Bill Williams, the Gator Oscillator is an expanded oscillator view of [Williams Alligator](Alligator.md#content)'s three moving averages.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/385 "Community discussion about this indicator")

![chart for Gator Oscillator]()

```csharp
// C# usage syntax
IReadOnlyList<GatorResult> results =
  quotes.GetGator();

// with custom Alligator configuration
IReadOnlyList<GatorResult> results = quotes
  .GetAlligator([see Alligator docs])
  .GetGator();
```

## Historical quotes requirements

If using default settings, you must have at least 121 periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods. Since this uses a smoothing technique, we recommend you use at least 271 data points prior to the intended usage date for better precision.  If using a custom Alligator configuration, see [Alligator documentation](./Alligator/#historical-quotes-requirements) for historical quotes requirements.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<GatorResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first 10-20 periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first 150 periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### GatorResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Upper`** _`double`_ - Absolute value of Alligator `Jaw-Teeth`

**`Lower`** _`double`_ - Absolute value of Alligator `Lips-Teeth`

**`UpperIsExpanding`** _`bool`_ - Upper value is growing

**`LowerIsExpanding`** _`bool`_ - Lower value is growing

### Utilities

- [.Condense()](../utilities.md#condense)
- [.Find(lookupDate)](../utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../utilities.md#remove-warmup-periods)

See [Utilities and helpers](../utilities.md#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HLC3)
    .GetGator();
```

Results **cannot** be further chained with additional transforms.
