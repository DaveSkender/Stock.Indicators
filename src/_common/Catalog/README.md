# Indicator Catalog implementation plan tracking

This directory contains the schema, attributes, and validation system for the Stock.Indicators catalog. The catalog provides a structured way to access metadata about all indicators in the library across different implementation styles (Series, Stream, and Buffer).

## Implementation Tasks

1. **Base Schema Definition**
   - [x] 1.1. Use existing `IndicatorListing` class with core properties (Name, Uiid, Style, Category)
   - [x] 1.2. Define and document all required and optional properties for catalog listings
   - [x] 1.3. Ensure all indicator types are represented in the catalog (Series, Stream, Buffer with separate listings for multi-style indicators)
   - [x] 1.4. Provide extension points for future catalog metadata
   - [x] 1.5. Use existing enumerations for Style (Series, Stream, Buffer), Category, and ResultType

2. **Attribute Implementation**
   - [x] 2.1. Implement `SeriesIndicatorAttribute`, `StreamIndicatorAttribute`, and `BufferIndicatorAttribute`
   - [x] 2.2. Ensure attributes are used for catalog generation and analyzer validation
   - [x] 2.3. Validate attribute usage and enforce uniqueness of Uiid

3. **Multi-Style Support (Task 11)**
   - [x] 3.1. Implement separate listings for each indicator style (Series, Stream, Buffer)
   - [x] 3.2. Update registry and analyzer to support multiple style-specific listings per indicator class
   - [x] 3.3. Enhance `CatalogGenerator` for automatic detection and generation of style-specific listing properties
   - [x] 3.4. Fix duplicate listing property issues in generator
   - [x] 3.5. Add comprehensive unit tests for multi-style listings and style-based queries
   - [x] 3.6. Update documentation and guides for multi-style support using one-listing-per-style approach
   - [x] 3.7. Ensure catalog search, filtering, and registration work for all supported styles
   - [x] 3.8. Validate that multi-style indicators appear correctly in style-specific queries

4. **Analyzer and Validation**
   - [x] 4.1. Analyzer rules for missing/incorrect listings, parameters, and results
   - [x] 4.2. Analyzer support for multi-style indicators with separate listings
   - [x] 4.3. Validate catalog completeness and uniqueness at build time

5. **Testing and Examples**
   - [x] 5.1. Unit tests for all catalog and registry features
   - [x] 5.2. Example code for custom and multi-style indicators using one-listing-per-style approach
   - [x] 5.3. Add/expand tests to ensure correct quantities are returned for each style (Series, Stream, Buffer) when queried separately
   - [ ] 5.4. Add tests for edge cases: duplicate Uiid, missing styles, and mixed manual/auto registration

6. **Documentation**
   - [x] 6.1. Guides for catalog usage, builder API, and multi-style indicators using one-listing-per-style approach
   - [x] 6.2. Analyzer rules and troubleshooting
   - [x] 6.3. Document catalog extension and custom metadata

7. **Catalog Completeness and Maintenance**
   - [ ] 7.1. Add a script or tool to audit catalog coverage and style distribution
   - [ ] 7.2. Document process for adding new indicators and updating the catalog
   - [ ] 7.3. Periodically review catalog for consistency and completeness

8. **Phase 1: Implement One-Listing-Per-Style Approach (Replace Composite Listings)**
   - [x] 8.1. Update `CatalogListingGuide.md` to reflect the one-listing-per-style approach
   - [x] 8.2. Update `README.md` to reflect the current status of the catalog system
   - [x] 8.3. Remove `CompositeIndicatorListing` checks from `IndicatorRegistry.cs`
   - [x] 8.4. Mark `CompositeIndicatorListing` and `CompositeIndicatorListingBuilder` as obsolete
   - [x] 8.4a. Completely remove `CompositeIndicatorListing` and `CompositeIndicatorListingBuilder` files and tests
   - [x] 8.5. Review and update any other references to composite listings in the codebase
   - [x] 8.6. Add tests to verify correct behavior of style-specific listings in registry queries
   - [x] 8.7. Verify that the CatalogGenerator correctly handles multiple style attributes
   - [ ] 8.8. **CRITICAL**: Fix decimal parameter formatting in CatalogGenerator (syntax error at line 326/341 with parameter 'F').  When troubleshooting, try to focus on the generator code itself initially, and the implementation of `AddParameter` method.  We've already explored potential issues with ALMA and data and that is not likely the issue.
   - [ ] 8.9. Add missing listing definitions based on values in reference file 'catalog.bak.json'

9. **Phase 2: Enhanced Catalog Features**
   - [ ] 9.1. Implement catalog versioning and migration support
   - [ ] 9.2. Add performance metrics and benchmarking data to catalog listings
   - [ ] 9.3. Implement advanced search and filtering capabilities (regex, fuzzy matching)
   - [ ] 9.4. Add catalog export/import functionality (JSON, XML, CSV formats)
   - [ ] 9.5. Implement catalog comparison tools for version differences

10. **Phase 3: Advanced Validation and Quality Assurance**
    - [ ] 10.1. Implement comprehensive catalog completeness validation
    - [ ] 10.2. Add automated testing for all catalog-generated code
    - [ ] 10.3. Implement performance regression testing for catalog operations
    - [ ] 10.4. Add memory usage analysis for large catalog operations
    - [ ] 10.5. Implement cross-platform compatibility testing

11. **Phase 4: Documentation and Developer Experience**
    - [ ] 11.1. Create interactive catalog browser/explorer tool
    - [ ] 11.2. Generate comprehensive API documentation from catalog metadata
    - [ ] 11.3. Implement catalog-based code generation for client libraries
    - [ ] 11.4. Add visual indicator relationship mapping
    - [ ] 11.5. Create developer onboarding guides with catalog integration

## Status (May 29 2025)

### Phase 1 Progress (95% Complete)

**MAJOR PROGRESS**: Phase 1 cleanup has significantly advanced with critical compilation fixes applied:

#### ✅ COMPLETED

- Fixed critical compilation errors: Reduced from 82+ errors to near-successful builds
- Updated `ExtractResultTypeInfo` method in `CatalogGenerator.cs` to use correct `ResultType` enum values
- Fixed syntax errors in generated `CatalogRegistration.g.cs` file related to parameter generation
- Updated `FormatDefaultValue` method to handle nullable string input properly
- Fixed test compilation errors by updating `CatalogGeneratorTests.cs` to use proper `GeneratorDriver` API
- Updated result type detection tests to expect correct `ResultType` enum values
- Fixed parameter generation logic to use proper dynamic values instead of hardcoded test values
- All unit tests now pass successfully
- Enhanced result type detection for Signal, Oscillator, and Band patterns

#### ⚠️ CRITICAL ISSUE REMAINING

- **Build compilation failure** due to syntax error in generated `CatalogRegistration.g.cs` at line 326/341 with parameter named 'F'
- Likely related to decimal default value formatting (e.g., ALMA indicator with `double offset = 0.85`)
- The `FormatDefaultValue` method may be incorrectly formatting decimal numbers, generating invalid C# syntax

#### 📋 PENDING TASKS

- Task 8.8: Fix decimal/numeric parameter default value formatting issues in the generator
- Task 8.9: Update reference file `catalog.bak.json` with missing listing definitions
- Complete validation of Phase 1 with all compilation errors resolved

#### 🏗️ IMPLEMENTATION NOTES

- All core and advanced catalog features are implemented using the one-listing-per-style approach
- `CompositeIndicatorListing` and related classes have been completely removed from the codebase
- Registry methods updated to handle style-specific listings correctly
- Multi-style indicators (e.g., ADL) successfully implemented with separate attributes
- Comprehensive test suite in place and passing

### Future Phases

- **Phase 2**: Enhanced catalog features including versioning, performance metrics, and advanced search
- **Phase 3**: Advanced validation, quality assurance, and comprehensive testing
- **Phase 4**: Documentation, developer tools, and interactive catalog browser

## Restart Instructions

When resuming Phase 1 work:

1. **Priority 1**: Investigate and fix the decimal formatting issue in `CatalogGenerator.cs`
   - Check the `FormatDefaultValue` method for decimal number formatting
   - Examine the ALMA indicator parameter causing the syntax error
   - Test with other decimal default values to ensure proper C# syntax generation

2. **Priority 2**: Complete Task 8.9 by updating `catalog.bak.json` reference file

3. **Validation**: Run full build and test suite to confirm Phase 1 completion

## Reference Documentation

- See `CatalogListingGuide.md` for details on multi-style support using separate listings per style
- See `CatalogGeneratorGuide.md` and `AnalyzerRules.md` for generator and analyzer usage
- See conversation summary for detailed technical context and code changes made

## Key Files and Changes Made

### Updated Files

- `d:\Repos\Stock.Indicators\tools\generators\Catalogger\CatalogGenerator.cs` (multiple fixes applied)
- `d:\Repos\Stock.Indicators\tests\tools\generators\CatalogGeneratorTests.cs` (compilation fixes)
- `d:\Repos\Stock.Indicators\tests\tools\generators\ResultTypeDetectionTests.cs` (test expectations updated)

### Generated Files with Issues

- `d:\Repos\Stock.Indicators\src\obj\Debug\net9.0\Generators\*.CatalogGenerator\CatalogRegistration.g.cs` (syntax error at line 326/341)

### Source of Current Issue

- `d:\Repos\Stock.Indicators\src\a-d\Alma\Alma.StaticSeries.cs` (ALMA indicator with decimal parameter causing formatting issue)
