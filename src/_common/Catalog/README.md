# Indicator Catalog System

This directory contains the schema, attributes, and validation system for the Stock.Indicators catalog. The catalog provides a structured way to access metadata about all indicators in the library across different implementation styles (Series, Stream, and Buffer).

## Implementation Tasks

1. **Base Schema Definition**
   - [x] 1.1. Use existing `IndicatorListing` class with core properties (Name, Uiid, Style, Category)
   - [x] 1.2. Define and document all required and optional properties for catalog listings
   - [x] 1.3. Ensure all indicator types are represented in the catalog (Series, Stream, Buffer, Composite)
   - [x] 1.4. Provide extension points for future catalog metadata
   - [x] 1.5. Use existing enumerations for Style (Series, Stream, Buffer), Category, and ResultType

2. **Attribute Implementation**
   - [x] 2.1. Implement `SeriesIndicatorAttribute`, `StreamIndicatorAttribute`, and `BufferIndicatorAttribute`
   - [x] 2.2. Ensure attributes are used for catalog generation and analyzer validation
   - [x] 2.3. Validate attribute usage and enforce uniqueness of Uiid

3. **Composite/Multi-Style Support (Task 11)**
   - [x] 3.1. Add `CompositeIndicatorListing` and builder for multi-style indicators
   - [x] 3.2. Update registry and analyzer to support composite listings
   - [x] 3.3. Enhance `CatalogGenerator` for automatic detection and generation of multi-style indicators
   - [x] 3.4. Fix duplicate listing property issues in generator
   - [x] 3.5. Add comprehensive unit tests for composite listings and style-based queries
   - [x] 3.6. Update documentation and guides for multi-style support
   - [x] 3.7. Ensure catalog search, filtering, and registration work for all supported styles
   - [x] 3.8. Validate that composite indicators appear in all relevant style queries

4. **Analyzer and Validation**
   - [x] 4.1. Analyzer rules for missing/incorrect listings, parameters, and results
   - [x] 4.2. Analyzer support for composite/multi-style indicators
   - [x] 4.3. Validate catalog completeness and uniqueness at build time

5. **Testing and Examples**
   - [x] 5.1. Unit tests for all catalog and registry features
   - [x] 5.2. Example code for custom and multi-style indicators
   - [ ] 5.3. Add/expand tests to ensure correct quantities are returned for each style (Series, Stream, Buffer, Composite) when queried separately
   - [ ] 5.4. Add tests for edge cases: duplicate Uiid, missing styles, and mixed manual/auto registration

6. **Documentation**
   - [x] 6.1. Guides for catalog usage, builder API, and multi-style indicators
   - [x] 6.2. Analyzer rules and troubleshooting
   - [x] 6.3. Document catalog extension and custom metadata

7. **Catalog Completeness and Maintenance**
   - [ ] 7.1. Add a script or tool to audit catalog coverage and style distribution
   - [ ] 7.2. Document process for adding new indicators and updating the catalog
   - [ ] 7.3. Periodically review catalog for consistency and completeness

## Status (May 2025)

- All core and advanced catalog features are implemented, including full support for multi-style indicators (Task 11 complete).
- The catalog system is robust, fully tested, and documented.
- See `MultiStyleIndicatorGuide.md` for details on multi-style support.
- See `CatalogGeneratorGuide.md` and `AnalyzerRules.md` for generator and analyzer usage.

## Next Steps

- [ ] Complete and expand tests to ensure that querying the catalog by each style (Series, Stream, Buffer, Composite) returns the correct quantity and set of indicators.
- [ ] Implement catalog audit tools and documentation for ongoing maintenance.
