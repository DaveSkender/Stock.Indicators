# Indicator Catalog System

This directory contains the schema, attributes, and validation system for the Stock.Indicators catalog. The catalog provides a structured way to access metadata about all indicators in the library across different implementation styles (Series, Stream, and Buffer).

## Implementation Tasks

1. **Base Schema Definition**
   - [ ] 1.1. Use existing `IndicatorListing` class with core properties (Name, Uiid, Style, Category)
   - [ ] 1.2. Enhance `Parameters` and `Results` collections in IndicatorListing
   - [ ] 1.3. Use existing `IndicatorParameter` schema with properties: ParameterName, DisplayName, DataType, Description, IsRequired, DefaultValue
   - [ ] 1.4. Use existing `IndicatorResult` schema with properties: DisplayName, DataName, DataType, IsDefault
   - [ ] 1.5. Use existing enumerations for Style (Series, Stream, Buffer), Category, and ResultType

2. **Attribute Implementation**
   - [ ] 2.1. Use existing base `IndicatorAttribute` abstract class with Id property
   - [ ] 2.2. Use existing `SeriesIndicatorAttribute` derived from IndicatorAttribute
   - [ ] 2.3. Use existing `StreamIndicatorAttribute` derived from IndicatorAttribute
   - [ ] 2.4. Use existing `BufferIndicatorAttribute` derived from IndicatorAttribute
   - [ ] 2.5. Apply attribute documentation and usage examples

3. **Fluent Builder Pattern**
   - [ ] 3.1. Design `IndicatorListingBuilder` with fluent interface for readability
   - [ ] 3.2. Implement strongly-typed parameter and result addition methods
   - [ ] 3.3. Add overloads to support optional properties and defaults
   - [ ] 3.4. Include validation in the build process
   - [ ] 3.5. Document builder usage with examples

4. **Static Catalog Definitions**
   - [ ] 4.1. Define `Listing` static property pattern for indicator classes
   - [ ] 4.2. Create standard template for indicator listings using the fluent builder pattern
   - [ ] 4.3. Implement sample `Ema.Listing` with complete metadata
   - [ ] 4.4. Ensure parameters match actual method signatures
   - [ ] 4.5. Document the registration pattern in comments

5. **Catalog Registration System**
   - [ ] 5.1. Create `IndicatorRegistry` static class with thread-safe singleton pattern
   - [ ] 5.2. Implement `Register` method to store indicator listings
   - [ ] 5.3. Implement `RegisterAuto` method using reflection to discover attributed classes
   - [ ] 5.4. Add `RegisterCatalog` convention methods with automatic parameter discovery
   - [ ] 5.5. Add support for XML documentation integration to pull parameter descriptions

6. **Public Catalog API**
   - [ ] 6.1. Implement `GetCatalog()` method with optional style filtering
   - [ ] 6.2. Implement `GetIndicator()` method to retrieve specific listings
   - [ ] 6.3. Add search and filtering capabilities
   - [ ] 6.4. Ensure thread-safe initialization
   - [ ] 6.5. Create extension methods for common queries

7. **Analyzer Rules Development**
   - [ ] 7.1. **SID001**: Missing Indicator Listing
     - [ ] 7.1.1. Detect classes with indicator attributes but no Listing property
     - [ ] 7.1.2. Report informational diagnostic with fix suggestions

   - [ ] 7.2. **SID002**: Missing Parameters
     - [ ] 7.2.1. Compare implementation parameters with listing parameters
     - [ ] 7.2.2. Detect parameters present in methods/constructors but missing from listing
     - [ ] 7.2.3. Report informational diagnostic for each missing parameter

   - [ ] 7.3. **SID003**: Extraneous Parameters
     - [ ] 7.3.1. Detect parameters present in listing but missing from implementation
     - [ ] 7.3.2. Report informational diagnostic for each extraneous parameter

   - [ ] 7.4. **SID004**: Parameter Type Mismatch
     - [ ] 7.4.1. Compare parameter types between implementation and listing
     - [ ] 7.4.2. Handle type name variations and nullable types
     - [ ] 7.4.3. Report informational diagnostic for each type mismatch

   - [ ] 7.5. **SID005**: Missing Results
     - [ ] 7.5.1. Extract expected result properties from return types
     - [ ] 7.5.2. Compare with results defined in listing
     - [ ] 7.5.3. Report informational diagnostic for missing results

8. **Source Generator Implementation**
   - [ ] 8.1. Create `CatalogGenerator` class (stub file already exists)
   - [ ] 8.2. Implement syntax receiver to collect attributed members
   - [ ] 8.3. Extract indicator metadata from attributes
   - [ ] 8.4. Generate automatic catalog registration code
   - [ ] 8.5. Add support for custom registration methods

9. **Documentation and Examples**
   - [ ] 9.1. Document catalog usage patterns in XML comments
   - [ ] 9.2. Add examples of retrieving and using catalog data
   - [ ] 9.3. Document analyzer rules and how to fix reported issues
   - [ ] 9.4. Create sample implementations for different indicator styles
   - [ ] 9.5. Add documentation for fluent builder API

10. **Cleanup and Final Refinements**
    - [ ] 10.1. Remove unneeded rules from `IndicatorStyleRules` and affiliated files
    - [ ] 10.2. Remove other unneeded code from `tools\generators\Generators.csproj`
    - [ ] 10.3. Optimize registration process for performance
    - [ ] 10.4. Add unit tests for the catalog system
    - [ ] 10.5. Final review and documentation updates

## Fluent Builder Pattern

To make catalog definitions more readable and maintainable, the plan includes a fluent builder API:

```csharp
// Example of the enhanced fluent builder pattern for indicator listings
public static class Ema
{
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Series)
        .WithCategory(Category.Trend)
        .AddParameter<int>("lookbackPeriods", "Lookback Period", 
            description: "Number of periods for the EMA calculation",
            isRequired: true)
        .AddParameter<decimal?>("smoothingFactor", "Smoothing Factor", 
            description: "Optional custom smoothing factor")
        // Auto-validates that parameter types match implementation
        .AddResult("Ema", "EMA", ResultType.Decimal, isDefault: true)
        // Support for extension methods to add common result patterns
        .AddPriceHlcResult() 
        .Build();
}
```

## Convention Guidelines

- [ ] Each indicator with attributes must have a static Listing property
- [ ] Parameters in Listing must match method/constructor parameters exactly
- [ ] Result definitions must accurately reflect indicator output properties
- [ ] Use consistent naming across all catalog entries

## Analyzer Usage

Analyzers provide informational diagnostics when:

- [ ] An indicator has attributes but no Listing property
- [ ] Parameters in implementation don't match Listing parameters
- [ ] Results in implementation don't match Listing results

These analyzers run at build time and report issues that need to be fixed to maintain an accurate catalog.
