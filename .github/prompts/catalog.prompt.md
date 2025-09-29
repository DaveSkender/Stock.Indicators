---
description: Create an indicator catalog entry
mode: edit
---

# Catalog entry creation workflow

Create a catalog entry for a Stock Indicators indicator using this workflow:

## Step 1: Analyze the models file

Examine `{IndicatorName}.Models.cs`:

- Does it implement `IReusable` or `ISeries`?
- If `IReusable`: Which property implements the `Value` getter?
- List all result properties
- Identify required parameters from method signatures

## Step 2: Determine default result

**CRITICAL RULE**: `isReusable: true` ONLY for `IReusable.Value` mappings

- **`IReusable` models**: Find `Value` getter (e.g., `Value => Ema.Null2NaN()`), mark ONLY that property as default
- **`ISeries` models**: ALL results get `isReusable: false` (no exceptions)

## Step 3: Build the catalog

### For single-style indicators

```csharp
namespace Skender.Stock.Indicators;

public static partial class {IndicatorName}
{
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("{Full Name}")
            .WithId("{UPPERCASE_ID}")
            .WithStyle(Style.Series)
            .WithCategory(Category.{Category})
            .WithMethodName("To{IndicatorName}")
            .AddParameter<int>("periods", "Periods", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("PropertyName", "Display Name", ResultType.Default, isReusable: true) // Only if IReusable.Value
            .Build();

    // No StreamListing for {INDICATOR_ID}.
    // No BufferListing for {INDICATOR_ID}.
}
```

### For multi-style indicators (preferred DRY pattern)

```csharp
namespace Skender.Stock.Indicators;

public static partial class {IndicatorName}
{
    // {INDICATOR_ID} Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("{Full Name}")
            .WithId("{UPPERCASE_ID}")
            .WithCategory(Category.{Category})
            .WithMethodName("To{IndicatorName}")
            .AddParameter<int>("periods", "Periods", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("PropertyName", "Display Name", ResultType.Default, isReusable: true) // Only if IReusable.Value
            .Build();

    // {INDICATOR_ID} Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // {INDICATOR_ID} Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // {INDICATOR_ID} Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
```

## Step 4: Register in catalog

Add entries to `src/_common/Catalog/Catalog.cs` in the `PopulateCatalog()` method:

```csharp
// {IndicatorName} ({Full Name})
_catalog.Add({IndicatorName}.SeriesListing);
_catalog.Add({IndicatorName}.StreamListing);  // If supported
_catalog.Add({IndicatorName}.BufferListing);  // If supported
```

## Step 5: Create catalog tests

Create `tests/indicators/{folder}/{indicator}/{IndicatorName}.Catalog.Tests.cs`:

```csharp
namespace Catalogging;

[TestClass]
public class {IndicatorName}Tests : TestBase
{
    [TestMethod]
    public void {IndicatorName}SeriesListing()
    {
        var listing = {IndicatorName}.SeriesListing;
        
        listing.Should().NotBeNull();
        listing.Name.Should().Be("{Full Name}");
        listing.Uiid.Should().Be("{UPPERCASE_ID}");
        listing.Style.Should().Be(Style.Series);
        // Test all parameters and results...
    }
    
    // Add tests for Stream/Buffer if supported
}
```

## Step 6: Validate

- [ ] Catalog created with correct structure and properties
- [ ] Registered in `IndicatorCatalog.PopulateCatalog()` method
- [ ] Catalog tests created and passing
- [ ] `dataName` matches model property names exactly
- [ ] `IReusable` models: exactly one `isReusable: true` for Value property
- [ ] `ISeries` models: all results `isReusable: false`
- [ ] Parameters have appropriate constraints
- [ ] Tests validate all metadata correctly

## Quick reference

**Common Categories**: MovingAverage, Oscillator, PriceChannel, Trend, Volume, Volatility

**Parameter Types**:

- `AddParameter<int>()` - integers with min/max
- `AddEnumParameter<T>()` - enum types
- `AddDateParameter()` - DateTime values
- `AddSeriesParameter()` - `IReadOnlyList<T>` inputs

**Critical Rule**: Only `IReusable.Value` mappings get `isReusable: true`. Everything else is `false`.

**Multi-style Pattern**: Use the `CommonListing` pattern with inheritance constructor `new CatalogListingBuilder(CommonListing)` to eliminate duplication. The constructor copies all properties from the base listing, and you only need to override the style.

---
Last updated: September 28, 2025
