# File Reorganization Plan

**Status**: Draft  
**Author**: Copilot Agent  
**Date**: December 28, 2024  
**Related Issue**: Major File Renaming Refactor

---

## OBJECTIVE

Reorganize the Stock.Indicators library to follow .NET naming conventions where file names match class/interface/enum names, using dot notation for partial classes. The target structure will improve discoverability, maintainability, and align with Microsoft's library design guidelines.

### Target Folder Structure

```text
src/                                  # Source code root
├── Common/                           # Shared functionality (formerly _common)
│   ├── BufferLists/                  # Buffer list infrastructure
│   ├── Candles/                      # Candle pattern functionality
│   ├── Catalog/                      # Indicator catalog system
│   │   └── Schema/                   # Catalog schema types
│   │       └── Enums/                # Catalog-specific enums
│   ├── Enums/                        # Core library enums
│   ├── Exceptions/                   # Custom exceptions
│   ├── Extensions/                   # Extension methods
│   │   ├── Generics/                 # Generic extensions
│   │   └── Math/                     # Math extensions
│   ├── Interfaces/                   # Core interfaces
│   ├── Models/                       # Core models
│   ├── QuotePart/                    # Quote part functionality
│   ├── Quotes/                       # Quote types and operations
│   ├── StreamHub/                    # Streaming infrastructure
│   │   └── Providers/                # Stream providers
│   └── Validation/                   # Validation utilities
├── Indicators/                       # Indicator implementations
│   ├── A-D/                          # Indicators A-D (alphabetical)
│   ├── E-K/                          # Indicators E-K
│   ├── M-R/                          # Indicators M-R
│   └── S-Z/                          # Indicators S-Z
└── [root files]                      # Project files, global configs

tests/                                # Test projects root
├── Indicators/                       # Indicator tests (formerly indicators)
│   ├── Base/                         # Base test classes (formerly _base)
│   ├── Common/                       # Common test utilities (formerly _common)
│   ├── Precision/                    # Precision tests (formerly _precision)
│   ├── TestData/                     # Test data (formerly _testdata)
│   ├── Tools/                        # Test tools (formerly _tools)
│   ├── A-D/                          # Tests for indicators A-D
│   ├── E-K/                          # Tests for indicators E-K
│   ├── M-R/                          # Tests for indicators M-R
│   └── S-Z/                          # Tests for indicators S-Z
├── Integration/                      # Integration tests
│   ├── Common/                       # Integration test utilities
│   └── Indicators/                   # Indicator integration tests
└── PublicApi/                        # Public API tests (formerly public-api)
    ├── Customizable/                 # Customizable API tests
    └── Indicators/                   # Indicator API tests
```

---

## FILE NAMING RULES

Follow these conventions for all file naming:

### 1. Match Type Names

- **Rule**: Each file name MUST match the primary type (class, interface, enum, record, struct) it contains
- **Example**: `public class EmaHub` → `EmaHub.cs`
- **Exception**: Partial classes use dot notation to indicate their functional area

### 2. Partial Classes Use Dot Notation

- **Rule**: Partial class files use `{TypeName}.{FunctionalArea}.cs` format
- **Examples**:
  - `Ema.StaticSeries.cs` - Static series implementation
  - `Ema.StreamHub.cs` - Streaming hub implementation
  - `Ema.BufferList.cs` - Buffer list implementation
  - `Ema.Catalog.cs` - Catalog metadata
  - `Ema.Models.cs` - Result models and types
  - `Ema.Utilities.cs` - Utility methods
  - `Quote.Validation.cs` - Validation methods
  - `Quote.Converters.cs` - Conversion methods

### 3. One Type Per File

- **Rule**: Each file contains exactly ONE public type (class, interface, enum, record, or struct)
- **Exception**: Nested types may reside in the same file as their parent
- **Action Required**: Files with multiple types (e.g., `Enums.cs`, `IIncrementFrom.cs`) must be split

### 4. Extension Method Classes

- **Rule**: Extension methods should be organized into `{Area}Extensions.cs` files
- **Examples**:
  - `PruningExtensions.cs` - Pruning extension methods
  - `SeekingExtensions.cs` - Seeking/finding extensions
  - `SortingExtensions.cs` - Sorting extensions
  - `TransformingExtensions.cs` - Transformation extensions
  - `StringOutExtensions.cs` - String output formatting extensions
  - `MathExtensions.cs` - Mathematical extensions

### 5. Interface Naming

- **Rule**: Interface files follow standard .NET convention with `I` prefix
- **Example**: `public interface IReusable` → `IReusable.cs`
- **Multiple Related Interfaces**: When closely related, consider organizing in subdirectories
  - `StreamHub/IStreamHub.cs`
  - `StreamHub/IStreamObserver.cs`
  - `StreamHub/IStreamObservable.cs`

### 6. Enum Naming

- **Rule**: Each enum gets its own file matching the enum name
- **Example**: `public enum CandlePart` → `CandlePart.cs`
- **Organization**: Group related enums in subdirectories (e.g., `Enums/`, `Catalog/Schema/Enums/`)

### 7. Exception Naming

- **Rule**: Exception classes follow .NET convention with `Exception` suffix
- **Example**: `public class InvalidQuotesException` → `InvalidQuotesException.cs`
- **Organization**: Place in `Common/Exceptions/` directory

### 8. PascalCase for File Names

- **Rule**: All file names use PascalCase (UpperCamelCase)
- **Example**: `ChainProvider.cs`, `RollingWindowMax.cs`, `StringOutExtensions.cs`

### 9. Avoid Generic Terms

- **Rule**: Avoid overly generic file names like `Enums.cs`, `Models.cs`, `Utilities.cs`
- **Better**: Use specific names or partition by functional area
- **Exception**: `{IndicatorName}.Models.cs` is acceptable as it's scoped to the indicator

### 10. Directory Names

- **Rule**: Directory names use PascalCase and are plural when containing multiple related items
- **Examples**: `Indicators/`, `Extensions/`, `Enums/`, `Interfaces/`
- **Exception**: Indicator alphabetical groupings remain as is (e.g., `A-D/`, `E-K/`)

---

## MIGRATION PLAN

> Phases and tasks are prioritized based on logical implementation sequence, minimizing disruption and ensuring testability at each step.

### Phase 1: Foundation - Common Infrastructure Split

**Goal**: Separate multi-type files in `_common/` into individual files

**Priority**: High - Establishes the foundation for all other changes

- [ ] Task 1.1: Split `Enums.cs` into individual enum files
  - Create `Common/Enums/Act.cs` (internal enum)
  - Create `Common/Enums/CandlePart.cs`
  - Create `Common/Enums/Direction.cs`
  - Create `Common/Enums/EndType.cs`
  - Create `Common/Enums/Match.cs`
  - Create `Common/Enums/MaType.cs`
  - Create `Common/Enums/OutType.cs`
  - Create `Common/Enums/PeriodSize.cs`
  - Delete `Enums.cs`
  - Update all using statements in dependent files

- [ ] Task 1.2: Split `IIncrementFrom.cs` into individual interface files
  - Create `Common/BufferLists/IIncrementFromChain.cs`
  - Create `Common/BufferLists/IIncrementFromPairs.cs`
  - Create `Common/BufferLists/IIncrementFromQuote.cs`
  - Delete `IIncrementFrom.cs`

- [ ] Task 1.3: Split `IStreamObservable.cs` into individual interface files
  - Create `Common/StreamHub/IChainProvider.cs`
  - Create `Common/StreamHub/IQuoteProvider.cs`
  - Create `Common/StreamHub/IStreamObservable.cs` (keep as single interface)
  - Delete original multi-interface file

- [ ] Task 1.4: Split `IStreamObserver.cs` into individual interface files
  - Create `Common/StreamHub/IStreamObserver.cs`
  - Create `Common/StreamHub/IPairsObserver.cs`
  - Delete original multi-interface file

- [ ] Task 1.5: Reorganize model files with multiple types
  - Split `Quote.Models.cs` → `IQuote.cs` and `Quote.cs`
  - Split `Candles.Models.cs` → `CandleProperties.cs` and `CandleResult.cs`
  - Review all other `*.Models.cs` files for multi-type violations

- [ ] Task 1.6: Run full test suite to verify no regressions

### Phase 2: Extension Methods Consolidation

**Goal**: Refactor static utility classes into properly named extension classes

**Priority**: High - Improves discoverability and follows .NET conventions

- [ ] Task 2.1: Rename and consolidate generic extensions
  - Rename `Pruning.cs` → `PruningExtensions.cs`
  - Rename `Seek.cs` → `SeekingExtensions.cs`
  - Rename `Sorting.cs` → `SortingExtensions.cs`
  - Rename `Transforms.cs` → `TransformingExtensions.cs`
  - Rename class `Seeking` → `SeekingExtensions`
  - Rename class `Transforming` → `TransformingExtensions`

- [ ] Task 2.2: Consolidate `StringOut` partial classes
  - Merge `StringOut.List.cs` and `StringOut.Type.cs` → `StringOutExtensions.cs`
  - Update class name from `StringOut` to `StringOutExtensions`

- [ ] Task 2.3: Reorganize math extensions
  - Rename `DeMath.cs` → `DeMathExtensions.cs`
  - Rename `NullMath.cs` → `NullMathExtensions.cs`
  - Rename `Numerical.cs` → `NumericalExtensions.cs`
  - Consider consolidating into `MathExtensions.cs` if closely related

- [ ] Task 2.4: Reorganize reusable extensions
  - Rename `Reusable.Utilities.cs` → `ReusableExtensions.cs`

- [ ] Task 2.5: Run targeted tests for affected areas

### Phase 3: Directory Reorganization

**Goal**: Rename and restructure directories for clarity and consistency

**Priority**: Medium - Improves navigation without changing functionality

- [ ] Task 3.1: Rename `_common/` → `Common/`
  - Update namespace declarations from `Skender.Stock.Indicators` (stay the same)
  - Update project file references
  - Update all path references in documentation

- [ ] Task 3.2: Reorganize `Common/` subdirectories
  - Create `Common/Extensions/Generics/` - Move generic extension files
  - Create `Common/Extensions/Math/` - Move math extension files
  - Create `Common/Interfaces/` - Move core interfaces (`ISeries.cs`, `IReusable.cs`)
  - Create `Common/Exceptions/` - Move exception files
  - Move `BinarySettings.cs` → `Common/Models/BinarySettings.cs`

- [ ] Task 3.3: Rename indicator category directories
  - Rename `a-d/` → `A-D/`
  - Rename `e-k/` → `E-K/`
  - Rename `m-r/` → `M-R/`
  - Rename `s-z/` → `S-Z/`
  - Create parent `Indicators/` directory and move all under it

- [ ] Task 3.4: Reorganize test directories
  - Rename `tests/indicators/` → `tests/Indicators/`
  - Rename `tests/indicators/_base/` → `tests/Indicators/Base/`
  - Rename `tests/indicators/_common/` → `tests/Indicators/Common/`
  - Rename `tests/indicators/_precision/` → `tests/Indicators/Precision/`
  - Rename `tests/indicators/_testdata/` → `tests/Indicators/TestData/`
  - Rename `tests/indicators/_tools/` → `tests/Indicators/Tools/`
  - Rename `tests/public-api/` → `tests/PublicApi/`
  - Update test namespaces to follow new directory structure

- [ ] Task 3.5: Run full test suite to verify directory changes

### Phase 4: Indicator File Organization

**Goal**: Ensure consistent naming patterns for all indicators

**Priority**: Medium - Most files already follow the pattern

- [ ] Task 4.1: Audit all indicator directories for compliance
  - Verify each indicator follows `{Name}.{FunctionalArea}.cs` pattern
  - Identify any non-compliant files
  - Create inventory of files needing renaming

- [ ] Task 4.2: Standardize interface files
  - Verify all `I{IndicatorName}.cs` files exist and follow naming
  - Consolidate any standalone interfaces into proper files

- [ ] Task 4.3: Review `.Models.cs` files for single-type compliance
  - Audit all `*.Models.cs` files
  - Split files containing multiple result types
  - Example: If `Macd.Models.cs` has `MacdResult` and `MacdSettings`, split appropriately

- [ ] Task 4.4: Run indicator-specific test suites

### Phase 5: Catalog and Schema Organization

**Goal**: Clean up catalog system file organization

**Priority**: Low - Working well but can be improved

- [ ] Task 5.1: Review `Catalog/` directory structure
  - Verify `CatalogListingBuilder.cs` naming
  - Verify `ListingExecutionBuilder.cs` naming
  - Verify `ListingExecutor.cs` naming
  - Ensure schema files follow conventions

- [ ] Task 5.2: Verify enum organization in `Catalog/Schema/Enums/`
  - Confirm `Category.cs`, `ResultType.cs`, `Style.cs` follow naming

- [ ] Task 5.3: Run catalog-related tests

### Phase 6: Special Cases and Edge Cases

**Goal**: Address unique situations and exceptions

**Priority**: Low - Handle after main reorganization

- [ ] Task 6.1: Review and rename remaining multi-type files
  - `Quote.StreamHub.cs` (contains `QuoteHub` and `BaseProvider<T>`)
  - Split if beneficial or document as acceptable exception

- [ ] Task 6.2: Review partial classes spanning multiple functional areas
  - Ensure dot notation is consistent and meaningful
  - Document any special cases in code comments

- [ ] Task 6.3: Review obsolete code organization
  - `Obsolete.V3.Indicators.cs` - acceptable as consolidated obsolete items
  - `Obsolete.V3.Other.cs` - acceptable as consolidated obsolete items
  - Consider whether to split in future major versions

- [ ] Task 6.4: Update build and tooling configurations
  - Verify `.csproj` file references
  - Update any build scripts or automation
  - Update IDE project settings if needed

### Phase 7: Documentation and Finalization

**Goal**: Update all documentation to reflect new organization

**Priority**: Medium - Must be done before release

- [ ] Task 7.1: Update contributor documentation
  - Update file organization guidelines
  - Update contribution guide with new naming conventions
  - Create migration guide for contributors with pending PRs

- [ ] Task 7.2: Update API documentation
  - Verify XML documentation references are correct
  - Update any file path references in docs
  - Regenerate API documentation if auto-generated

- [ ] Task 7.3: Update build and CI/CD documentation
  - Update any scripts that reference file paths
  - Update deployment documentation
  - Verify continuous integration still works

- [ ] Task 7.4: Create migration notes for external developers
  - Document breaking changes (if any namespace changes)
  - Provide guidance for updating custom indicators
  - Update changelog with reorganization details

- [ ] Task 7.5: Final validation
  - Run complete test suite (unit, integration, regression)
  - Run performance benchmarks to verify no degradation
  - Manual smoke testing of key scenarios
  - Code review of all changes

---

## APPENDIX

### A.1: Current Directory Structure

```text
src/                                  # Source code
├── _common/                          # Common functionality
│   ├── BufferLists/
│   ├── Candles/
│   ├── Catalog/
│   │   └── Schema/
│   │       └── Enums/
│   ├── Generics/
│   ├── Math/
│   ├── QuotePart/
│   ├── Quotes/
│   ├── Reusable/
│   ├── StreamHub/
│   │   └── Providers/
│   └── Validation/
├── a-d/                              # Indicators A-D
│   ├── Adl/
│   ├── Adx/
│   ├── Alligator/
│   ├── [... 22 more indicators]
├── e-k/                              # Indicators E-K
│   ├── ElderRay/
│   ├── Ema/
│   ├── Epma/
│   ├── [... 12 more indicators]
├── m-r/                              # Indicators M-R
│   ├── MaEnvelopes/
│   ├── Macd/
│   ├── Mama/
│   ├── [... 14 more indicators]
└── s-z/                              # Indicators S-Z
    ├── Slope/
    ├── Sma/
    ├── SmaAnalysis/
    └── [... 24 more indicators]

tests/                                # Tests
├── indicators/                       # Indicator tests
│   ├── _base/                        # Base test classes
│   ├── _common/                      # Common test utilities
│   ├── _precision/                   # Precision tests
│   ├── _testdata/                    # Test data
│   ├── _tools/                       # Test tools
│   ├── a-d/
│   ├── e-k/
│   ├── m-r/
│   └── s-z/
├── integration/                      # Integration tests
│   ├── _common/
│   └── indicators/
└── public-api/                       # Public API tests
    ├── customizable/
    └── indicators/
```

### A.2: Current State Analysis

#### What's Working Well

1. **Indicator Organization**: The alphabetical categorization (a-d, e-k, m-r, s-z) works well for managing 70+ indicators
2. **Partial Class Pattern**: Using dot notation for partial classes (e.g., `Ema.StaticSeries.cs`) is clear and follows .NET conventions
3. **Functional Separation**: Clear separation between series, stream, buffer, catalog, and models
4. **Test Organization**: Test structure mirrors source structure, making tests easy to locate
5. **Common Directory**: Consolidation of shared functionality in `_common/` is logical
6. **Namespace Consistency**: Single namespace `Skender.Stock.Indicators` keeps public API clean

#### What's Not Working Well

1. **Underscore Prefix**: Leading underscores in directory names (`_common/`, `_base/`) are not idiomatic in .NET
2. **Multiple Types Per File**: Files like `Enums.cs` (7 enums), `IIncrementFrom.cs` (3 interfaces) violate single-type principle
3. **Generic File Names**: Names like `Enums.cs`, `Utilities.cs` don't indicate what types they contain
4. **Inconsistent Extension Classes**: Some use class name matching file name, others don't (e.g., `Seeking` vs `Seek.cs`)
5. **Mixed Case Directories**: Test directories use lowercase (`a-d/`) while .NET convention is PascalCase
6. **Extension Method Organization**: Extension methods scattered across files named by functionality rather than as `*Extensions.cs`
7. **Interface Organization**: Related interfaces split across files could be better organized
8. **Model File Ambiguity**: `.Models.cs` suffix doesn't indicate which model(s) are in the file

### A.3: Top 10 Problems to Solve (Prioritized)

1. **Multi-Type Files** (High Impact)
   - **Problem**: `Enums.cs`, `IIncrementFrom.cs`, and other files contain multiple types
   - **Impact**: Violates .NET conventions, makes types hard to find, complicates navigation
   - **Solution**: Split into one file per type

2. **Underscore Directory Prefixes** (High Impact)
   - **Problem**: `_common/`, `_base/`, `_testdata/` use leading underscores
   - **Impact**: Not idiomatic for .NET, suggests private/hidden when they're not
   - **Solution**: Rename to PascalCase without underscores (e.g., `Common/`, `Base/`)

3. **Extension Method Naming** (Medium Impact)
   - **Problem**: Extension classes don't follow `*Extensions` naming convention
   - **Impact**: Harder to identify extension methods in IntelliSense and search
   - **Solution**: Rename to `PruningExtensions.cs`, `SeekingExtensions.cs`, etc.

4. **Inconsistent Static Class Naming** (Medium Impact)
   - **Problem**: Static class names don't always match file names (e.g., `Seeking` class in `Seek.cs`)
   - **Impact**: Confusion when file name differs from type name
   - **Solution**: Align class names with file names

5. **Generic Utility File Names** (Medium Impact)
   - **Problem**: Files like `Utilities.cs` don't indicate their specific purpose
   - **Impact**: Must open file to know what's inside
   - **Solution**: Use specific names or partition by function

6. **Interface File Organization** (Low Impact)
   - **Problem**: Multiple related interfaces in single files makes individual interfaces harder to reference
   - **Impact**: Reduced discoverability, harder to link to specific interface docs
   - **Solution**: One interface per file

7. **Lower Case Directory Names** (Low Impact)
   - **Problem**: `a-d/`, `e-k/`, etc. use lowercase in tests and src
   - **Impact**: Inconsistent with .NET PascalCase convention
   - **Solution**: Rename to `A-D/`, `E-K/`, etc.

8. **Directory Organization Depth** (Low Impact)
   - **Problem**: Current structure is flat with alphabetical grouping
   - **Impact**: Could benefit from functional grouping within indicators
   - **Solution**: Consider `Indicators/` parent directory for clarity

9. **Model File Naming Ambiguity** (Low Impact)
   - **Problem**: `*.Models.cs` suffix doesn't indicate which specific types are inside
   - **Impact**: Must open file to see if it contains desired model
   - **Solution**: Split multi-model files or document naming standard

10. **Test Directory Alignment** (Low Impact)
    - **Problem**: Test directory structure uses different casing and conventions than src
    - **Impact**: Slightly harder to map test files to source files
    - **Solution**: Align test directory naming with source conventions

### A.4: File Naming Patterns by Count

Based on analysis of 623 C# files in `src/`:

- **Pattern: `{Indicator}.StaticSeries.cs`** - ~70 files (one per indicator)
- **Pattern: `{Indicator}.StreamHub.cs`** - ~50 files (indicators with streaming)
- **Pattern: `{Indicator}.BufferList.cs`** - ~40 files (indicators with buffering)
- **Pattern: `{Indicator}.Models.cs`** - ~70 files (result models)
- **Pattern: `{Indicator}.Catalog.cs`** - ~70 files (catalog metadata)
- **Pattern: `{Indicator}.Utilities.cs`** - ~45 files (utility methods)
- **Pattern: `I{Indicator}.cs`** - ~45 files (indicator interfaces)
- **Pattern: Partial class with dots** - ~320 files total
- **Pattern: Generic utility classes** - ~20 files (need renaming)
- **Pattern: Multiple types per file** - ~12 files (need splitting)

### A.5: Namespace Considerations

**Current**: All files use `namespace Skender.Stock.Indicators;`

**Recommendation**: Keep single namespace for public API simplicity

- **Pros**: Clean public API, no breaking changes, easy to use
- **Cons**: Doesn't reflect file organization
- **Decision**: Maintain current single namespace - file organization is for developer experience, not API design

**Rationale**: .NET library design guidelines prioritize clean public APIs over internal organization. File structure aids development; namespace structure aids consumption.

### A.6: Estimated Effort

- **Phase 1**: 8-12 hours (Foundation split) - High complexity due to dependency updates
- **Phase 2**: 4-6 hours (Extension methods) - Medium complexity, mostly renames
- **Phase 3**: 6-8 hours (Directory reorganization) - Medium complexity, many file moves
- **Phase 4**: 4-6 hours (Indicator audit) - Low complexity if compliant
- **Phase 5**: 2-3 hours (Catalog) - Low complexity
- **Phase 6**: 3-4 hours (Special cases) - Variable complexity
- **Phase 7**: 4-6 hours (Documentation) - Medium complexity, critical for adoption

**Total Estimated Effort**: 31-45 hours

**Risk Factors**:

- Test breakage from namespace or path changes
- Build script dependencies on file paths
- External developer impact if namespaces change
- Merge conflicts with active development

**Mitigation**:

- Execute in feature branch with frequent test runs
- Coordinate with core contributors
- Use automated refactoring tools where possible
- Create comprehensive migration documentation

---

## REFERENCES

- [.NET Library Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/)
- [General Naming Conventions](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/general-naming-conventions)
- [Names of Assemblies and DLLs](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-assemblies-and-dlls)

---
Last updated: December 28, 2024
