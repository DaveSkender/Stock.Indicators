# Common Utilities and Patterns

This directory contains shared utilities, base classes, and common patterns used across all indicator implementations in the Stock Indicators library.

## Directory Structure

```text
_common/
├── README.md                    # This file
├── BinarySettings.cs            # Binary reader settings
├── BufferLists/                 # BufferList implementation support
│   ├── BufferList.cs            # Base BufferList class
│   ├── BufferList.Utilities.cs # Buffer management utilities
│   └── IIncrementFrom.cs        # Increment interfaces
├── Candles/                     # Candlestick pattern utilities
├── Catalog/                     # Indicator catalog and metadata
├── Enums.cs                     # Shared enumerations
├── Generics/                    # Generic type utilities
├── ISeries.cs                   # Series interface definition
├── Math/                        # Mathematical utilities
│   ├── NullMath.cs              # Null-safe math operations
│   └── Numerical.cs             # Numerical calculations
├── QuotePart/                   # Quote component extraction
├── Quotes/                      # Quote manipulation utilities
├── Reusable/                    # Reusable result types
├── StreamHub/                   # StreamHub base classes and streaming utilities
│   ├── IStreamHub.cs            # StreamHub interface
│   ├── StreamHub.cs             # Base StreamHub implementation
│   ├── StreamHub.Utilities.cs  # StreamHub helper methods
│   └── Providers/               # Stream provider implementations
└── Validation/                  # Input validation helpers
```

## Purpose

This directory provides foundational infrastructure for all indicator implementations:

- **BufferLists/**: Incremental buffer-based indicator processing
- **Candles/**: Candlestick pattern recognition utilities
- **Catalog/**: Indicator metadata and discovery
- **Math/**: Numerical operations and null-safe calculations
- **StreamHub/**: Real-time streaming indicator base classes
- **Validation/**: Input validation and error handling

## Development Guidelines

For detailed implementation guidance, see the scoped instruction files:

- **StreamHub development**: `.github/instructions/indicator-stream.instructions.md`
  - Performance patterns (O(1) state updates, avoiding O(n²) recalculation)
  - State management best practices
  - Testing requirements and regression validation
- **BufferList development**: `.github/instructions/indicator-buffer.instructions.md`
  - Buffer management patterns
  - Incremental processing techniques
  - Interface selection guide
- **Series development**: `.github/instructions/indicator-series.instructions.md`
  - Batch processing patterns (canonical reference implementations)
- **General requirements**: `.github/copilot-instructions.md`
  - Catalog registration
  - Documentation standards
  - Migration guide updates

## Performance Optimization

For streaming and buffer indicators experiencing performance issues, consult:

- **Analysis**: `tools/performance/baselines/PERFORMANCE_REVIEW.md` - Root cause analysis of performance bottlenecks
- **Active work**: `specs/002-fix-streaming-performance/` - Current performance optimization specification
- **Constitution**: `.specify/memory/constitution.md` - Performance First principles

---
Last updated: October 19, 2025
