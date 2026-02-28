---
name: testing-standards
description: Testing conventions for Stock Indicators. Use for test naming (MethodName_StateUnderTest_ExpectedBehavior), FluentAssertions patterns, precision requirements, and test base class selection.
---

# Testing standards

## Base class selection

| Style | Base Class |
| ----- | ---------- |
| Series | `StaticSeriesTestBase` |
| Buffer | `BufferListTestBase` |
| Stream | `StreamHubTestBase` |
| Other | `TestBase` |

## Test naming

Pattern: `MethodName_StateUnderTest_ExpectedBehavior`

## Required abstract methods

Compile errors if missing. Additional tests are developer discretion.

**Series** (`StaticSeriesTestBase`):

- `DefaultParameters_ReturnsExpectedResults()`
- `BadQuotes_DoesNotFail()`
- `NoQuotes_ReturnsEmpty()`

**Buffer** (`BufferListTestBase`):

- `PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()`
- `Clear_WithState_ResetsState()`
- Plus interface methods from `ITestQuoteBufferList` or `ITestChainBufferList` (see [patterns reference](references/patterns.md))

**Stream** (`StreamHubTestBase`):

- `ToStringOverride_ReturnsExpectedName()`
- Plus interface methods from `ITestQuoteObserver`, `ITestChainObserver`, and/or `ITestChainProvider` (see [patterns reference](references/patterns.md))

## Test data

`Data.GetDefault()` — 502 quotes. Use consistently across all tests.

See [references/patterns.md](references/patterns.md) for FluentAssertions patterns, precision constants, and full BufferList/StreamHub interface method lists.
