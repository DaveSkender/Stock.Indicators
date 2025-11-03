# Common utilities and patterns

This directory contains shared utilities, base classes, and common patterns used across all indicator implementations in the Stock Indicators library.

## Directory structure

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

- BufferLists/ - Incremental buffer-based indicator processing
- Candles/ - Candlestick pattern recognition utilities
- Catalog/ - Indicator metadata and discovery
- Math/ - Numerical operations and null-safe calculations
- StreamHub/ - Real-time streaming indicator base classes
- Validation/ - Input validation and error handling

## Development guidelines

For detailed implementation guidance, see the scoped instruction files:

- StreamHub development: [.github/instructions/indicator-stream.instructions.md](../../.github/instructions/indicator-stream.instructions.md)
  - Performance patterns (O(1) state updates, avoiding O(n²) recalculation)
  - State management best practices
  - Testing requirements and regression validation
- BufferList development: [.github/instructions/indicator-buffer.instructions.md](../../.github/instructions/indicator-buffer.instructions.md)
  - Buffer management patterns
  - Incremental processing techniques
  - Interface selection guide
- Series development: [.github/instructions/indicator-series.instructions.md](../../.github/instructions/indicator-series.instructions.md)
  - Batch processing patterns (canonical reference implementations)
- General requirements: [.github/copilot-instructions.md](../../.github/copilot-instructions.md)
  - Catalog registration
  - Documentation standards
  - Migration guide updates

## NaN handling policy

The library follows IEEE 754 floating-point standard for NaN (Not-a-Number) handling:

### Core principles

1. Natural propagation - NaN values propagate naturally through calculations (e.g., any operation with NaN produces NaN)
2. Internal representation - Use `double.NaN` internally when a value cannot be calculated
3. External representation - Convert NaN to `null` (via `.NaN2Null()`) only at the final result boundary
4. No rejection - Never reject NaN inputs with validation; allow them to flow through the system

### Implementation guidelines

- RollingWindow utilities - Accept NaN values and return NaN for Min/Max when NaN is present in the window
- Quote validation - Only validate for null/missing quotes, not for NaN values in quote properties (High/Low/Close/etc.)
- Division by zero - Allow natural NaN propagation (e.g., `x / 0.0 = Infinity`, `0.0 / 0.0 = NaN`)
- Zero-range checks - Remove unnecessary guards like `if (range != 0)`; let division produce NaN naturally

### Constitutional alignment

This approach aligns with Constitution §1: Mathematical Precision:

- Maintains numerical correctness (NaN is mathematically correct for undefined values)
- Prevents silent data corruption from substituting invalid placeholders
- Follows established IEEE 754 standard

## Performance optimization

For streaming and buffer indicators experiencing performance issues, consult:

- Analysis: [tools/performance/baselines/PERFORMANCE_REVIEW.md](../../tools/performance/baselines/PERFORMANCE_REVIEW.md) - Root cause analysis of performance bottlenecks
- Active work: [specs/002-fix-streaming-performance/](../../specs/002-fix-streaming-performance/) - Current performance optimization specification
- Constitution: [.specify/memory/constitution.md](../../.specify/memory/constitution.md) - Performance First principles

---
Last updated: November 2, 2025
