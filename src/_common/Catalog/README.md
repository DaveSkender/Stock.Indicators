# Indicator Catalog System

This directory contains the schema, attributes, and validation system for the Stock.Indicators catalog. The catalog provides a structured way to access metadata about all indicators in the library across different implementation styles (Series, Stream, and Buffer).

## Implementation Tasks

1. **Base Schema Definition**
   - [x] 1.1. Use existing `IndicatorListing` class with core properties (Name, Uiid, Style, Category)
   - [x] 1.2. Enhance `Parameters` and `Results` collections in IndicatorListing
   - [x] 1.3. Use existing `IndicatorParameter` schema with properties: ParameterName, DisplayName, DataType, Description, IsRequired, DefaultValue
   - [x] 1.4. Use existing `IndicatorResult` schema with properties: DisplayName, DataName, DataType, IsDefault
   - [x] 1.5. Use existing enumerations for Style (Series, Stream, Buffer), Category, and ResultType

2. **Attribute Implementation**
   - [x] 2.1. Use existing base `IndicatorAttribute` abstract class with Id property
   - [x] 2.2. Use existing `SeriesIndicatorAttribute` derived from IndicatorAttribute
   - [x] 2.3. Use existing `StreamIndicatorAttribute` derived from IndicatorAttribute
   - [x] 2.4. Use existing `BufferIndicatorAttribute` derived from IndicatorAttribute
   - [x] 2.5. Apply attribute documentation and usage examples

3. **Fluent Builder Pattern**
   - [x] 3.1. Design `IndicatorListingBuilder` with fluent interface for readability
   - [x] 3.2. Implement strongly-typed parameter and result addition methods
   - [x] 3.3. Add overloads to support optional properties and defaults
   - [x] 3.4. Include validation in the build process
   - [x] 3.5. Document builder usage with examples

4. **Static Catalog Definitions**
   - [x] 4.1. Define `Listing` static property pattern for indicator classes
   - [x] 4.2. Create standard template for indicator listings using the fluent builder pattern
   - [x] 4.3. Implement sample `Ema.Listing` with complete metadata
   - [x] 4.4. Ensure parameters match actual method signatures
   - [x] 4.5. Document the registration pattern in comments

5. **Catalog Registration System**
   - [x] 5.1. Create `IndicatorRegistry` static class with thread-safe singleton pattern
   - [x] 5.2. Implement `Register` method to store indicator listings
   - [x] 5.3. Implement `RegisterAuto` method using reflection to discover attributed classes
   - [x] 5.4. Add `RegisterCatalog` convention methods with automatic parameter discovery
   - [x] 5.5. Add support for XML documentation integration to pull parameter descriptions

6. **Public Catalog API**
   - [x] 6.1. Implement `GetCatalog()` method with optional style filtering
   - [x] 6.2. Implement `GetIndicator()` method to retrieve specific listings
   - [x] 6.3. Add search and filtering capabilities
   - [x] 6.4. Ensure thread-safe initialization
   - [x] 6.5. Create extension methods for common queries

7. **Analyzer Rules Development**
   - [x] 7.1. **SID001**: Missing Indicator Listing
     - [x] 7.1.1. Detect classes with indicator attributes but no Listing property
     - [x] 7.1.2. Report informational diagnostic with fix suggestions

   - [x] 7.2. **SID002**: Missing Parameters
     - [x] 7.2.1. Compare implementation parameters with listing parameters
     - [x] 7.2.2. Detect parameters present in methods/constructors but missing from listing
     - [x] 7.2.3. Report informational diagnostic for each missing parameter

   - [x] 7.3. **SID003**: Extraneous Parameters
     - [x] 7.3.1. Detect parameters present in listing but missing from implementation
     - [x] 7.3.2. Report informational diagnostic for each extraneous parameter

   - [x] 7.4. **SID004**: Parameter Type Mismatch
     - [x] 7.4.1. Compare parameter types between implementation and listing
     - [x] 7.4.2. Handle type name variations and nullable types
     - [x] 7.4.3. Report informational diagnostic for each type mismatch

   - [x] 7.5. **SID005**: Missing Results
     - [x] 7.5.1. Extract expected result properties from return types
     - [x] 7.5.2. Compare with results defined in listing
     - [x] 7.5.3. Report informational diagnostic for missing results

8. **Source Generator Implementation**
   - [x] 8.1. Create `CatalogGenerator` class (stub file already exists)
   - [x] 8.2. Implement syntax receiver to collect attributed members
   - [x] 8.3. Extract indicator metadata from attributes
   - [x] 8.4. Generate automatic catalog registration code
   - [x] 8.5. Add support for custom registration methods

9. **Documentation and Examples**
   - [x] 9.1. Document catalog usage patterns in XML comments
   - [x] 9.2. Add examples of retrieving and using catalog data
   - [x] 9.3. Document analyzer rules and how to fix reported issues
   - [x] 9.4. Create sample implementations for different indicator styles:
     - [x] 9.4.1. Series indicator implementation example with proper attribute usage
     - [x] 9.4.2. Stream indicator implementation example with proper attribute usage
     - [x] 9.4.3. Buffer indicator implementation example with proper attribute usage
   - [x] 9.5. Add documentation for fluent builder API
   - [x] 9.6. Document CatalogGenerator usage and troubleshooting:
     - [x] 9.6.1. How to add attributes to indicator methods
     - [x] 9.6.2. When to manually create a Listing vs using the generator
     - [x] 9.6.3. How to resolve common generation errors (CS0111, CS0102, etc.)

10. **Cleanup and Final Refinements**
    - [x] 10.1. Remove unneeded `IND***` rules from `DiagnosticDescriptors.cs`
    - [x] 10.2. Remove other unneeded code from `tools\generators\Generators.csproj`
    - [x] 10.3. Optimize registration process for performance
    - [x] 10.4. Add unit tests for the catalog system
    - [x] 10.5. Fix duplicate code generation in CatalogGenerator:
        - [x] 10.5.1. Enhance HasExistingListing detection to properly filter classes with existing Listing properties
        - [x] 10.5.2. Fix missing partial modifier for HeikinAshi class
        - [x] 10.5.3. Add explicit filtering of duplicate methods with the same attribute
    - [x] 10.6. Enhance `CatalogGenerator` for better metadata extraction:
        - [x] 10.6.1. Improve result type detection from method return types
        - [x] 10.6.2. Add XML documentation parsing for better parameter descriptions
        - [x] 10.6.3. Implement smarter Style detection for different indicator types
    - [x] 10.7. Final review and documentation updates

11. **Multi-Style Indicator Support**
    - [ ] 11.1. Enhance CatalogGenerator to handle multiple style variants in a single class:
        - [ ] 11.1.1. Detect all style variants of an indicator (Series/Stream/Buffer)
        - [ ] 11.1.2. Generate style-specific listings when multiple variants exist
        - [ ] 11.1.3. Use naming pattern like `SeriesListing`, `StreamListing`, `BufferListing` for variants
    - [ ] 11.2. Create a consolidated catalog approach:
        - [ ] 11.2.1. Implement `CompositeIndicatorListing` to aggregate multiple style variants
        - [ ] 11.2.2. Allow manual creation of composite listings with style-specific sections
        - [ ] 11.2.3. Add detection of composite listings to avoid duplicate generation
    - [ ] 11.3. Update analyzer rules for multi-style indicators:
        - [ ] 11.3.1. Enhance SID001 to recognize composite listings
        - [ ] 11.3.2. Add SID006 rule to check consistency across style variants
        - [ ] 11.3.3. Update parameter/result validation to account for style-specific parameters

12. **Future Enhancements** (Optional)
    - [ ] 12.1. Further improve code coverage with additional test scenarios
    - [ ] 12.2. Add benchmark tests for registration performance
    - [ ] 12.3. Consider adding runtime configuration options for catalog system
    - [ ] 12.4. Explore advanced metadata extraction techniques for better API discoverability

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

- [x] Each indicator with attributes must have a static Listing property
- [x] Parameters in Listing must match method/constructor parameters exactly
- [x] Result definitions must accurately reflect indicator output properties (basic implementation complete, refinement needed)
- [x] Use consistent naming across all catalog entries
- [x] Indicator specific tests should be placed alongside their other tests
- [x] Tests should use FluentAssertions assertion style
- [x] Enum parameter definitions should not have minimum or maximum values
- [x] Fix all code analysis warnings and suggestions as we go, but ignore `IND***` types

## Analyzer Usage

Analyzers provide informational diagnostics when:

- [x] An indicator has attributes but no Listing property
- [x] Parameters in implementation don't match Listing parameters
- [x] Results in implementation don't match Listing results

These analyzers run at build time and report issues that need to be fixed to maintain an accurate catalog.

## Current Status and Next Steps

### Completed Milestones

- Base schema definition and attributes implementation is complete
- Fluent builder pattern and static catalog definitions are functional
- Catalog registration system and public API are implemented
- Analyzer rules are developed and working
- Source Generator implementation is functionally complete
- Documentation and examples are complete with comprehensive guides for:
  - Catalog usage patterns and examples
  - Different indicator style implementations
  - Analyzer rules and troubleshooting
  - Fluent builder API usage
  - CatalogGenerator usage and error resolution

### Current Status

All planned functionality for the catalog system has been successfully implemented:

- Fixed HeikinAshi partial class issue by adding the partial modifier
- Enhanced CatalogGenerator filtering to prevent duplicate code generation:
  - Improved HasExistingListing detection to properly identify existing Listing properties
  - Added explicit filtering to skip classes with existing implementations
  - Implemented per-method filtering to handle multiple attributed methods in a single class
- Added comprehensive unit tests for the catalog system:
  - Tests for basic catalog generator functionality
  - Tests for result type detection
  - Tests for XML documentation parsing
- Enhanced metadata extraction:
  - Improved result type detection based on return types and property patterns
  - Added XML documentation parsing for better descriptions
  - Implemented smarter style detection for different indicator types

### Future Improvements

Some areas for potential future enhancement include:

1. **Comprehensive Code Coverage**
   - Add more test scenarios for edge cases
   - Implement boundary testing for analyzer rules

2. **Performance Optimization**
   - Add benchmark tests for registration performance
   - Optimize memory usage during catalog loading

3. **Extended Functionality**
   - Add runtime configuration options for catalog system
   - Implement additional metadata extraction techniques

4. **Multi-Style Indicator Support**
   - Enhance CatalogGenerator to handle multiple style variants (series, stream, buffer) within a single indicator class:
     - Detect all style variants of an indicator (Series/Stream/Buffer) even if they share the same class and similar method names
     - Generate style-specific listings (e.g., `SeriesListing`, `StreamListing`, `BufferListing`) when multiple variants exist in the same class
     - Avoid generating duplicate `Listing` properties that cause CS0102/CS0111 errors
   - Create a consolidated catalog approach:
     - Implement a `CompositeIndicatorListing` to aggregate multiple style variants for a single indicator
     - Allow manual creation of composite listings with style-specific sections
     - Add detection of composite listings to avoid duplicate generation
   - Update analyzer rules for multi-style indicators:
     - Enhance SID001 to recognize composite listings
     - Add a new rule (e.g., SID006) to check consistency across style variants
     - Update parameter/result validation to account for style-specific differences

**Note:**
The current build errors are primarily due to multiple indicator styles (series, stream, buffer) being implemented as static extension methods within the same class, often with similar or identical method names. When the CatalogGenerator attempts to generate a static `Listing` property for each variant, it results in duplicate member errors (CS0102, CS0111). Resolving this will require the enhancements described above to support multi-style indicators in a unified and error-free way.
