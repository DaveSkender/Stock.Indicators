# Result classes analysis: ISeries vs IReusable

This document lists all result classes that implement `ISeries` instead of `IReusable`, along with recommended properties for the `Value` property if they were to implement `IReusable`.

## Executive summary

- **Total result classes implementing ISeries (not IReusable):** 16
- **Total result classes implementing IReusable:** 63
- **CandleResult:** Also implements ISeries (1 additional class)

## Understanding the interfaces

### ISeries interface

```csharp
public interface ISeries
{
    DateTime Timestamp { get; }
}
```

The `ISeries` interface only requires a `Timestamp` property and is the base interface for all time-series data.

### IReusable interface

```csharp
public interface IReusable : ISeries
{
    [JsonIgnore]
    double Value { get; }
}
```

The `IReusable` interface extends `ISeries` and adds a `Value` property that identifies a single chainable value. This property is used to pass values to chained indicators and is marked with `[JsonIgnore]` so it doesn't appear in serialized output.

## Result classes implementing ISeries only

The following result classes implement `ISeries` but not `IReusable`. Each entry includes a recommendation for which property should be used as the `Value` if the class were converted to implement `IReusable`.

### 1. PivotPointsResult

**File:** `src/m-r/PivotPoints/PivotPoints.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `PP` (decimal?) - Pivot Point
- `S1, S2, S3, S4` (decimal?) - Support levels 1-4
- `R1, R2, R3, R4` (decimal?) - Resistance levels 1-4

**Recommended Value property:** `PP`

**Rationale:** The pivot point (PP) is the central reference value from which all support and resistance levels are calculated. It represents the primary price level for the indicator.

---

### 2. PivotsResult

**File:** `src/m-r/Pivots/Pivots.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `HighPoint` (decimal?)
- `LowPoint` (decimal?)
- `HighLine` (decimal?)
- `LowLine` (decimal?)
- `HighTrend` (PivotTrend?)
- `LowTrend` (PivotTrend?)

**Recommended Value property:** `HighPoint` or `LowPoint` (context-dependent)

**Rationale:** This indicator tracks both high and low pivots. The choice of Value would depend on the use case:

- For bullish strategies: `HighPoint`
- For bearish strategies: `LowPoint`
- Alternative: Could use an average of `(HighPoint + LowPoint) / 2` if both are present

**Note:** This is a complex case where a single Value may not be meaningful. Consider if this should implement IReusable at all.

---

### 3. RollingPivotsResult

**File:** `src/m-r/RollingPivots/RollingPivots.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `PP` (decimal?) - Pivot Point
- `S1, S2, S3, S4` (decimal?) - Support levels 1-4
- `R1, R2, R3, R4` (decimal?) - Resistance levels 1-4

**Recommended Value property:** `PP`

**Rationale:** Same as PivotPointsResult - the pivot point (PP) is the central reference value.

---

### 4. KeltnerResult

**File:** `src/e-k/Keltner/Keltner.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `UpperBand` (double?)
- `Centerline` (double?)
- `LowerBand` (double?)
- `Width` (double?)
- `Atr` (double?) - internal only

**Recommended Value property:** `Centerline`

**Rationale:** The centerline is the primary EMA value that the bands are calculated from. Similar to Bollinger Bands which uses `PercentB` as its Value, but the centerline is more fundamental for chaining purposes.

---

### 5. DonchianResult

**File:** `src/a-d/Donchian/Donchian.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `UpperBand` (decimal?)
- `Centerline` (decimal?)
- `LowerBand` (decimal?)
- `Width` (decimal?)

**Recommended Value property:** `Centerline`

**Rationale:** The centerline represents the middle of the Donchian Channel and is the most appropriate single value for chaining.

---

### 6. AtrStopResult

**File:** `src/a-d/AtrStop/AtrStop.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `AtrStop` (double?) - Trailing stop line (includes both buy and sell stops)
- `BuyStop` (double?) - Stop line to close short position
- `SellStop` (double?) - Stop line to close long position
- `Atr` (double?) - Average True Range

**Recommended Value property:** `AtrStop`

**Rationale:** The `AtrStop` is the combined trailing stop line and represents the primary indicator value.

---

### 7. AlligatorResult

**File:** `src/a-d/Alligator/Alligator.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `Jaw` (double?)
- `Teeth` (double?)
- `Lips` (double?)

**Recommended Value property:** `Lips` or average of all three

**Rationale:** The Alligator indicator consists of three lines representing different smoothed moving averages:

- Jaw (slowest, 13-period SMMA shifted 8 periods)
- Teeth (medium, 8-period SMMA shifted 5 periods)
- Lips (fastest, 5-period SMMA shifted 3 periods)

The `Lips` line is the fastest-responding and most recent, making it a good candidate. Alternatively, an average `(Jaw + Teeth + Lips) / 3` could represent the overall Alligator position.

**Note:** This is complex as all three lines are significant. Consider if this should implement IReusable at all.

---

### 8. GatorResult

**File:** `src/e-k/Gator/Gator.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `Upper` (double?)
- `Lower` (double?)
- `UpperIsExpanding` (bool?)
- `LowerIsExpanding` (bool?)

**Recommended Value property:** `Upper` or `(Upper - Lower)`

**Rationale:** The Gator Oscillator measures the absolute difference between the Alligator's Jaw and Teeth (Upper) and between Teeth and Lips (Lower). Options:

- Use `Upper` as the primary value
- Use the difference `(Upper - Lower)` to represent the overall oscillator state
- Use the absolute value `Math.Abs(Upper) + Math.Abs(Lower)` to represent total distance

**Note:** This is complex as both Upper and Lower are significant. Consider if this should implement IReusable at all.

---

### 9. FractalResult

**File:** `src/e-k/Fractal/Fractal.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `FractalBear` (decimal?)
- `FractalBull` (decimal?)

**Recommended Value property:** Context-dependent

**Rationale:** Fractals identify turning points with separate bearish (high) and bullish (low) values. There is no single primary value. Options:

- Use `FractalBull` for bullish strategies
- Use `FractalBear` for bearish strategies
- Use an average if both are present

**Note:** This is a complex case where a single Value may not be meaningful. Consider if this should implement IReusable at all.

---

### 10. FcbResult

**File:** `src/e-k/Fcb/Fcb.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `UpperBand` (decimal?)
- `LowerBand` (decimal?)

**Recommended Value property:** `(UpperBand + LowerBand) / 2` (calculated centerline)

**Rationale:** Fractal Channel Bands consist of upper and lower bands based on fractal highs and lows. The midpoint between the bands would be the most meaningful single value.

**Note:** This would require adding a calculated property or computing the average in the Value getter.

---

### 11. IchimokuResult

**File:** `src/e-k/Ichimoku/Ichimoku.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `TenkanSen` (decimal?) - Conversion line
- `KijunSen` (decimal?) - Base line
- `SenkouSpanA` (decimal?) - Leading span A
- `SenkouSpanB` (decimal?) - Leading span B
- `ChikouSpan` (decimal?) - Lagging span

**Recommended Value property:** `KijunSen` or `TenkanSen`

**Rationale:** Ichimoku Cloud has five components, making it complex to choose a single value. Options:

- `KijunSen` (Base Line): The medium-term equilibrium, most stable
- `TenkanSen` (Conversion Line): The short-term equilibrium, more responsive
- Average of `(SenkouSpanA + SenkouSpanB) / 2`: The cloud midpoint

Most commonly, traders focus on the relationship between price and the Kijun-sen, making it the best candidate.

**Note:** This is very complex. Consider if this should implement IReusable at all.

---

### 12. SuperTrendResult

**File:** `src/s-z/SuperTrend/SuperTrend.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `SuperTrend` (decimal?)
- `UpperBand` (decimal?)
- `LowerBand` (decimal?)

**Recommended Value property:** `SuperTrend`

**Rationale:** The `SuperTrend` property is the primary indicator value - it's either the upper or lower band depending on the trend direction.

---

### 13. StdDevChannelsResult

**File:** `src/s-z/StdDevChannels/StdDevChannels.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `Centerline` (double?)
- `UpperChannel` (double?)
- `LowerChannel` (double?)
- `BreakPoint` (bool)

**Recommended Value property:** `Centerline`

**Rationale:** The centerline is the linear regression line from which the channels are calculated, making it the primary value.

---

### 14. VortexResult

**File:** `src/s-z/Vortex/Vortex.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `Pvi` (double?) - Positive Vortex Indicator
- `Nvi` (double?) - Negative Vortex Indicator

**Recommended Value property:** `Pvi - Nvi` (calculated difference)

**Rationale:** The Vortex Indicator uses two lines (positive and negative) to identify trend direction. The crossover and difference between them are significant. Options:

- Use `Pvi` for bullish focus
- Use `Nvi` for bearish focus  
- Use `Pvi - Nvi` to represent the oscillator state

**Note:** This would require adding a calculated property. Consider if this should implement IReusable at all.

---

### 15. StarcBandsResult

**File:** `src/s-z/StarcBands/StarcBands.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `UpperBand` (double?)
- `Centerline` (double?)
- `LowerBand` (double?)

**Recommended Value property:** `Centerline`

**Rationale:** The centerline is the SMA from which the ATR-based bands are calculated, making it the primary value.

---

### 16. MaEnvelopeResult

**File:** `src/m-r/MaEnvelopes/MaEnvelopes.Models.cs`

**Properties:**

- `Timestamp` (DateTime)
- `Centerline` (double?)
- `UpperEnvelope` (double?)
- `LowerEnvelope` (double?)

**Recommended Value property:** `Centerline`

**Rationale:** The centerline is the moving average from which the percentage-based envelopes are calculated, making it the primary value.

---

## Other classes implementing ISeries

### CandleResult

**File:** `src/_common/Candles/CandleResult.cs`

**Properties:**

- `Timestamp` (DateTime)
- `Price` (decimal?)
- `Match` (Match enum)
- `Candle` (CandleProperties)

**Recommended Value property:** `Price`

**Rationale:** This represents candlestick pattern analysis results. The `Price` property is the identified pattern price level.

**Note:** CandleResult is a special case representing pattern matching rather than a calculated indicator.

---

## Result classes implementing IReusable

For reference, here are all 63 result classes that currently implement `IReusable`, showing what property they use as their `Value`:

| Class | File | Current Value Property |
| ----- | ---- | ---------------------- |
| RocWbResult | m-r/RocWb/RocWb.Models.cs | Roc |
| RsiResult | m-r/Rsi/Rsi.Models.cs | Rsi |
| PmoResult | m-r/Pmo/Pmo.Models.cs | Pmo |
| MacdResult | m-r/Macd/Macd.Models.cs | Macd |
| PrsResult | m-r/Prs/Prs.Models.cs | Prs |
| ObvResult | m-r/Obv/Obv.Models.cs | Obv |
| MfiResult | m-r/Mfi/Mfi.Models.cs | Mfi |
| MamaResult | m-r/Mama/Mama.Models.cs | Mama |
| PvoResult | m-r/Pvo/Pvo.Models.cs | Pvo |
| ParabolicSarResult | m-r/ParabolicSar/ParabolicSar.Models.cs | Sar |
| RocResult | m-r/Roc/Roc.Models.cs | Roc |
| CmfResult | a-d/Cmf/Cmf.Models.cs | Cmf |
| BetaResult | a-d/Beta/Beta.Models.cs | Beta |
| DpoResult | a-d/Dpo/Dpo.Models.cs | Dpo |
| BollingerBandsResult | a-d/BollingerBands/BollingerBands.Models.cs | PercentB |
| AwesomeResult | a-d/Awesome/Awesome.Models.cs | Oscillator |
| DynamicResult | a-d/Dynamic/Dynamic.Models.cs | Dynamic |
| CorrResult | a-d/Correlation/Correlation.Models.cs | Correlation |
| AdlResult | a-d/Adl/Adl.Models.cs | Adl |
| AroonResult | a-d/Aroon/Aroon.Models.cs | Oscillator |
| DemaResult | a-d/Dema/Dema.Models.cs | Dema |
| ChopResult | a-d/Chop/Chop.Models.cs | Chop |
| AtrResult | a-d/Atr/Atr.Models.cs | Atrp |
| AlmaResult | a-d/Alma/Alma.Models.cs | Alma |
| ChandelierResult | a-d/Chandelier/Chandelier.Models.cs | ChandelierExit |
| ChaikinOscResult | a-d/ChaikinOsc/ChaikinOsc.Models.cs | Oscillator |
| BopResult | a-d/Bop/Bop.Models.cs | Bop |
| CmoResult | a-d/Cmo/Cmo.Models.cs | Cmo |
| AdxResult | a-d/Adx/Adx.Models.cs | Adx |
| CciResult | a-d/Cci/Cci.Models.cs | Cci |
| ConnorsRsiResult | a-d/ConnorsRsi/ConnorsRsi.Models.cs | ConnorsRsi |
| HurstResult | e-k/Hurst/Hurst.Models.cs | HurstExponent |
| FisherTransformResult | e-k/FisherTransform/FisherTransform.Models.cs | Fisher |
| HmaResult | e-k/Hma/Hma.Models.cs | Hma |
| ElderRayResult | e-k/ElderRay/ElderRay.Models.cs | (BullPower + BearPower) |
| EmaResult | e-k/Ema/Ema.Models.cs | Ema |
| KvoResult | e-k/Kvo/Kvo.Models.cs | Oscillator |
| ForceIndexResult | e-k/ForceIndex/ForceIndex.Models.cs | ForceIndex |
| HtlResult | e-k/HtTrendline/HtTrendline.Models.cs | Trendline |
| KamaResult | e-k/Kama/Kama.Models.cs | Kama |
| EpmaResult | e-k/Epma/Epma.Models.cs | Epma |
| ZigZagResult | s-z/ZigZag/ZigZag.Models.cs | ZigZag |
| StdDevResult | s-z/StdDev/StdDev.Models.cs | StdDev |
| TrResult | s-z/Tr/Tr.Models.cs | Tr |
| WilliamsResult | s-z/WilliamsR/WilliamsR.Models.cs | WilliamsR |
| StochResult | s-z/Stoch/Stoch.Models.cs | Oscillator |
| StcResult | s-z/Stc/Stc.Models.cs | Stc |
| UltimateResult | s-z/Ultimate/Ultimate.Models.cs | Ultimate |
| SmmaResult | s-z/Smma/Smma.Models.cs | Smma |
| TrixResult | s-z/Trix/Trix.Models.cs | Trix |
| SmaResult | s-z/Sma/Sma.Models.cs | Sma |
| VwmaResult | s-z/Vwma/Vwma.Models.cs | Vwma |
| UlcerIndexResult | s-z/UlcerIndex/UlcerIndex.Models.cs | UlcerIndex |
| StochRsiResult | s-z/StochRsi/StochRsi.Models.cs | StochRsi |
| T3Result | s-z/T3/T3.Models.cs | T3 |
| TsiResult | s-z/Tsi/Tsi.Models.cs | Tsi |
| VolatilityStopResult | s-z/VolatilityStop/VolatilityStop.Models.cs | Sar |
| WmaResult | s-z/Wma/Wma.Models.cs | Wma |
| TemaResult | s-z/Tema/Tema.Models.cs | Tema |
| SmiResult | s-z/Smi/Smi.Models.cs | Smi |
| SlopeResult | s-z/Slope/Slope.Models.cs | Slope |
| VwapResult | s-z/Vwap/Vwap.Models.cs | Vwap |
| QuotePart | _common/QuotePart/QuotePart.Models.cs | Value (direct) |

Note: Most `Value` properties use the `.Null2NaN()` extension method to convert `null` to `double.NaN`.

---

## Recommendations summary

### Clear candidates for IReusable conversion

These result classes have obvious single-value properties that should be used as `Value`:

1. **PivotPointsResult** → `PP`
2. **RollingPivotsResult** → `PP`
3. **KeltnerResult** → `Centerline`
4. **DonchianResult** → `Centerline`
5. **AtrStopResult** → `AtrStop`
6. **SuperTrendResult** → `SuperTrend`
7. **StdDevChannelsResult** → `Centerline`
8. **StarcBandsResult** → `Centerline`
9. **MaEnvelopeResult** → `Centerline`

### Complex cases requiring discussion

These result classes have multiple significant properties and may not benefit from IReusable conversion:

1. **PivotsResult** - Tracks both high and low pivots
2. **AlligatorResult** - Three equally important lines (Jaw, Teeth, Lips)
3. **GatorResult** - Upper and Lower oscillators with expansion flags
4. **FractalResult** - Separate bear and bull fractals
5. **FcbResult** - Only upper and lower bands (no explicit centerline)
6. **IchimokuResult** - Five components with no clear primary value
7. **VortexResult** - Two lines (Pvi and Nvi) with their difference being significant

**Recommendation:** Consider whether these complex indicators should implement IReusable at all, or if they should remain ISeries-only to avoid forcing a potentially misleading single-value representation.

---

## Pattern analysis

### Value property patterns in IReusable implementations

From the existing 63 IReusable implementations:

1. **Single primary value** (most common): Value points to the main indicator property (RSI → Rsi, MACD → Macd, SMA → Sma)
2. **Percentage/oscillator** (Bollinger Bands): Value points to `PercentB` rather than the centerline
3. **Calculated value** (Elder Ray): Value is calculated as `(BullPower + BearPower).Null2NaN()`
4. **Direct value** (QuotePart): Value is passed through directly

### Decimal vs double

Most result properties use `double?` for numeric values, but some older indicators (PivotPoints, RollingPivots, Fractal, Ichimoku, SuperTrend, Donchian, Fcb) use `decimal?`. The `Value` property in IReusable is `double`, which would require conversion for decimal-based indicators.

---

## Next steps

1. **Review architectural decision**: Should all result classes implement IReusable, or is it acceptable for complex multi-value indicators to remain ISeries-only?

2. **Prioritize conversions**: Start with the clear candidates that have obvious primary values.

3. **Address decimal→double conversion**: Determine the approach for indicators using decimal properties (cast, convert, or refactor to double).

4. **Update catalog**: If indicators are converted to IReusable, update their catalog entries to mark the `IsReusable` flag.

5. **Documentation**: Update indicator documentation to reflect which property is used as the chainable Value.

---

Last updated: January 5, 2026
