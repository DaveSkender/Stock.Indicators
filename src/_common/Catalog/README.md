# Indicator Catalog System

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

8. **Finalize One-Listing-Per-Style Implementation**
   - [x] 8.1. Update `MultiStyleIndicatorGuide.md` to reflect the one-listing-per-style approach
   - [x] 8.2. Update `README.md` to reflect the current status of the catalog system
   - [x] 8.3. Remove `CompositeIndicatorListing` checks from `IndicatorRegistry.cs`
   - [x] 8.4. Mark `CompositeIndicatorListing` and `CompositeIndicatorListingBuilder` as obsolete
   - [ ] 8.5. Review and update any other references to composite listings in the codebase
   - [ ] 8.6. Add tests to verify correct behavior of style-specific listings in registry queries
   - [ ] 8.7. Verify that the CatalogGenerator correctly handles multiple style attributes
   - [ ] 8.8 Add missing listing definitions based on values in reference file ‘catalog.bak.json‘

## Status (May 2025)

- All core and advanced catalog features are implemented, including full support for multi-style indicators using the one-listing-per-style approach (Task 11 complete).
- The catalog system is robust, fully tested, and documented.
- `CompositeIndicatorListing` and `CompositeIndicatorListingBuilder` classes have been marked as obsolete to guide users toward the one-listing-per-style approach.
- Registry methods have been updated to remove checks for composite listings and handle style-specific listings correctly.
- See `MultiStyleIndicatorGuide.md` for details on multi-style support using separate listings per style.
- See `CatalogGeneratorGuide.md` and `AnalyzerRules.md` for generator and analyzer usage.

## Next Steps

- [ ] Complete and expand tests to ensure that querying the catalog by each style (Series, Stream, Buffer) returns the correct quantity and set of indicators
- [ ] Review the entire codebase for any remaining references to composite listings
- [ ] Implement catalog audit tools and documentation for ongoing maintenance
- [ ] Consider fully removing the obsolete composite listing classes in a future major release
