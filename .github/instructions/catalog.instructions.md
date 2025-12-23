---
description: Indicator Catalog entry definition
applyTo: '**/src/**/*.Catalog.cs,**/tests/indicators/**/*.Catalog.Tests.cs'
---

# Catalog file conventions

## Structure requirements

- All catalog files must follow the pattern `{IndicatorName}.Catalog.cs`
- Must be placed in the specific `src` folder alongside the indicator source code (e.g., `src/e-k/Ema/Ema.Catalog.cs`)
- Must be in the same directory as the corresponding `{IndicatorName}.Models.cs` file and other indicator files
- Must use the `CatalogListingBuilder` pattern for defining indicator metadata

## Naming conventions

- Class name must be a static partial class matching the indicator name (e.g., `public static partial class Ema`)
- Use consistent naming for internal static readonly fields:
  - `SeriesListing` for basic series-style indicators
  - `StreamListing` for streaming/incremental indicators  
  - `BufferListing` for buffer-style indicators
- Use "No StreamListing" and "No BufferListing" comments when those styles are not supported

## Builder pattern requirements

- Use `IndicatorDefinitionBuilder()` to construct listings
- **CRITICAL**: `.WithMethodName()` must be specified in the **style-specific listings** (SeriesListing, StreamListing, BufferListing), NOT in the CommonListing
- Required methods in CommonListing (in order):
  1. `WithName()` - Full display name of the indicator
  2. `WithId()` - Uppercase abbreviated identifier
  3. `WithCategory()` - Appropriate category from Category enum
  4. Parameters (if any) using `AddParameter<T>()`, `AddEnumParameter<T>()`, `AddDateParameter()`, or `AddSeriesParameter()`
  5. Results using `AddResult()`
  6. `Build()`
- Required methods in style-specific listings (in order):
  1. `WithStyle()` - Style.Series, Style.Stream, or Style.Buffer
  2. `WithMethodName()` - Method name for automation (e.g., "ToEma" for Series, "ToEmaHub" for Stream, "ToEmaList" for Buffer)
  3. `Build()`

## Method naming conventions

Each style has a specific method naming pattern:

- **Series**: `To{IndicatorName}` (e.g., `ToEma`, `ToRocWb`, `ToRsi`)
- **Stream**: `To{IndicatorName}Hub` (e.g., `ToEmaHub`, `ToRocWbHub`, `ToRsiHub`)
- **Buffer**: `To{IndicatorName}List` (e.g., `ToEmaList`, `ToRocWbList`, `ToRsiList`)

The method name MUST match the actual extension method name in the indicator's implementation.

## Complete example

Here's a complete example showing the correct pattern with CommonListing and style-specific listings:

```csharp
public static partial class Ema
{
    /// <summary>
    /// EMA Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Exponential Moving Average")
            .WithId("EMA")
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Period", description: "Number of periods for the EMA calculation", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// EMA Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToEma")
            .Build();

    /// <summary>
    /// EMA Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName("ToEmaHub")
            .Build();

    /// <summary>
    /// EMA Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName("ToEmaList")
            .Build();
}
```

Note: `.WithMethodName()` appears ONLY in the style-specific listings, and each style has its own unique method name following the naming conventions.

## Parameter guidelines

- Use `AddParameter<T>()` for basic types (int, double, bool, etc.)
- Include descriptive display names and descriptions
- Set `isRequired: true` for mandatory parameters
- Provide appropriate `defaultValue`, `minimum`, and `maximum` constraints
- Use `AddEnumParameter<T>()` for enum types
- Use `AddDateParameter()` for DateTime parameters
- Use `AddSeriesParameter()` for `IReadOnlyList<T> where T : IReusable`

## Advanced parameter scenarios

- **Optional parameters**: Use `isRequired: false` and provide sensible `defaultValue`
- **Nullable parameters**: Use nullable types (e.g., `AddParameter<double?>()`) when the parameter can be null
- **Constrained parameters**: Always set appropriate `minimum` and `maximum` values for numeric parameters
- **Enum parameters**: Use `AddEnumParameter<T>()` and provide a sensible `defaultValue` from the enum
- **Complex parameters**: For indicators requiring series inputs, use `AddSeriesParameter()`

## Result guidelines

- Use `AddResult(dataName, displayName, ResultType.Default, isReusable)`
- The `dataName` must exactly match the property name in the corresponding Models file
- Set `isReusable: true` ONLY for the property that maps to `IReusable.Value` (when the model implements `IReusable`)
- For models implementing `ISeries` (not `IReusable`), ALL results should have `isReusable: false`
- Use descriptive `displayName` values for UI display
- There should be exactly one and only one result with `isReusable: true` per indicator, and only when the model implements `IReusable`

## Default result identification rules

**Critical**: Only set `isReusable: true` for results that correspond to the `IReusable.Value` property implementation.

**For `IReusable` models**:

- Find the property that implements the `Value` getter (e.g., `public double Value => Ema.Null2NaN();`)
- Only that property's corresponding result should have `isReusable: true`
- There must be exactly one and only one result with `isReusable: true`

**For `ISeries` models (not `IReusable`)**:

- ALL results must have `isReusable: false`
- No exceptions - `ISeries` models never have a default result

**Examples**:

- `EmaResult.Ema` → `isReusable: true` (maps to `Value => Ema.Null2NaN()`)
- `GatorResult.Upper` → `isReusable: false` (`ISeries` model - no default allowed)
- `KeltnerResult.Centerline` → `isReusable: false` (`ISeries` model - no default allowed)

## Validation and error prevention

- **Property name matching**: The `dataName` in `AddResult()` must match the exact case and spelling of the property in the Models file
- **Duplicate validation**: The builder validates against duplicate parameter names and result names
- **Default result validation**: The builder enforces that multiple results require at least one default when dealing with `IReusable` models
- **Interface consistency**: Verify the model's interface (`IReusable` vs `ISeries`) determines default result behavior

## Category guidelines

Choose the most appropriate category:

- **MovingAverage**: EMA, SMA, HMA, TEMA, etc.
- **Oscillator**: RSI, Stochastic, MACD, Williams %R, etc.
- **PriceChannel**: Bollinger Bands, Keltner Channels, Donchian Channels
- **Trend**: ADX, Aroon, Parabolic SAR, Supertrend
- **Volume**: OBV, Chaikin Money Flow, Volume Weighted indicators
- **Volatility**: ATR, Standard Deviation, VIX-related indicators

## Style implementation patterns

- **Series only**: Most indicators only need Series style (simplest implementation)
- **Series + Stream**: Indicators that support real-time updates (like EMA, SMA)
- **Series + Buffer**: Indicators that can efficiently handle incremental data with buffering
- **All three styles**: Complex indicators that support all processing modes (rare)

## Common anti-patterns to avoid

- **❌ CRITICAL**: Putting `.WithMethodName()` in CommonListing instead of style-specific listings
- **❌ CRITICAL**: Using the wrong indicator's method name (e.g., "ToRocWb" in EMA's SeriesListing)
- **❌ CRITICAL**: Inconsistent method naming across styles (e.g., "ToEma", "ToRocWbHub", "ToEmaList")
- Setting `isReusable: true` for `ISeries` models (never allowed)
- Multiple `isReusable: true` results in `IReusable` models (only one allowed)
- Missing parameter constraints on numeric values
- Inconsistent naming between Models properties and catalog results
- Overly complex parameter descriptions (keep concise but clear)
- Missing required parameters or marking optional parameters as required

## Technical implementation notes

- The catalog listings are used by automation systems to generate API calls
- Default results determine which property is used for indicator chaining
- Parameter validation occurs at runtime based on the catalog constraints
- Method names should follow the "To{IndicatorName}" pattern for consistency
- IDs should be uppercase and concise but recognizable

## Catalog registration requirements

- **Manual registration**: Each catalog entry must be manually added to `src/_common/Catalog/Catalog.cs` in the `PopulateCatalog()` method
- **Registration pattern**: Add entries in alphabetical order by indicator name
- **Multiple styles**: Register each supported style (Series, Stream, Buffer) as separate entries
- **Format**: Use the pattern `_catalog.Add({IndicatorName}.{StyleName}Listing);`

## Required test files

- **Catalog tests**: Create `{IndicatorName}.Catalog.Tests.cs` in the `tests/indicators/{folder}/{indicator}/` directory
- **Test structure**: Must test all supported styles (Series, Stream, Buffer)
- **Test inheritance**: Inherit from `TestBase` and use the `[TestClass]` attribute
- **Test methods**: Include tests for each listing's metadata, parameters, and results
- **Validation tests**: Include tests that validate catalog metadata matches direct method calls

## Test implementation requirements

- **Namespace**: Use `namespace Catalogging;` for all catalog tests
- **Test methods**: One test method per style (e.g., `EmaSeriesListing()`, `EmaStreamListing()`, `EmaBufferListing()`)
- **Assertions**: Verify all properties including Name, Uiid, Style, Category, Parameters, and Results
- **Parameter validation**: Check parameter names, types, default values, and constraints
- **Result validation**: Verify result property names, display names, and `isReusable` flags
- **Integration test**: Include test comparing catalog-driven calls with direct method calls

---
Last updated: October 13, 2025
