# Scripts

Utility scripts for repository maintenance and quality assurance.

## StreamHub Audit Script

**File**: `audit-streamhub.sh`

**Purpose**: Validates StreamHub test coverage, interface compliance, and provider history testing completeness. Implements validation requirements from tasks T173, T175-T185 in the streaming indicators development plan.

### Usage

```bash
# From repository root
bash tools/scripts/audit-streamhub.sh
```

### What It Checks

#### T173: Test Coverage

- All StreamHub implementations (`src/**/*.StreamHub.cs`) have corresponding test files
- Test file naming convention matches implementation files
- Reports missing test files if any exist

#### T175-T179: Interface Compliance

- All test classes inherit from `StreamHubTestBase`
- Tests implement correct observer interfaces:
  - `ITestQuoteObserver` - For quote provider compatibility
  - `ITestChainObserver` - For chainable indicators (inherits ITestQuoteObserver)
  - `ITestPairsObserver` - For dual-stream indicators
- Tests implement provider interface when applicable:
  - `ITestChainProvider` - For indicators that can be chained
- Validates exactly one observer interface is implemented (except ChainObserver which includes QuoteObserver)
- Validates PairsObserver is not combined with other observer interfaces

#### T180-T183: Provider History Testing

Checks that test methods include comprehensive provider history mutation testing:

**QuoteObserver tests** must include:

- `Insert()` operation (late arrival scenario)
- `Remove()` operation (history mutation)
- Duplicate quote handling

**ChainProvider tests** must include:

- `Insert()` operation (late arrival scenario)
- `Remove()` operation (history mutation)

#### T184-T185: Test Base Class Review

- Validates `StreamHubTestBase` exists
- Confirms test interfaces are defined
- Checks helper methods are available

### Output

The script provides color-coded output:

- 🟢 Green: Passing checks
- 🟡 Yellow: Warnings (non-critical issues)
- 🔴 Red: Failures (critical issues)

Example:

```text
========================================
StreamHub Audit - Tasks T173, T175-T185
========================================

=== T173: Validating Test Coverage ===
StreamHub implementations: 81
Corresponding test files found: 81
Missing test files: 0

=== T175-T179: Test Interface Compliance ===
Interface compliance: PASS
Required test methods: PASS

=== T180-T183: Provider History Testing ===
Provider history testing issues: 41
  ⚠ Sma: ChainProvider test missing Insert/Remove operations
  ⚠ Roc: ChainProvider test missing Insert/Remove operations
  ...

=== T184-T185: Test Base Class Review ===
✓ StreamHubTestBase exists
✓ Test interfaces defined: 4
✓ Helper methods available: Yes

========================================
Audit Summary
========================================
Total StreamHub implementations: 81
Test files found: 81

Issues found:
  ⚠ Provider history testing gaps: 41
```

### Exit Codes

- `0` - Success (no critical issues, warnings allowed)
- `1` - Failure (missing test files or interface compliance issues)

### Integration

The script can be integrated into CI/CD pipelines to enforce quality standards:

```yaml
- name: Audit StreamHub Tests
  run: bash tools/scripts/audit-streamhub.sh
```

### Fixing Issues

When the audit identifies issues, follow these patterns:

#### ChainProvider Test Pattern (Canonical Reference)

See `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` - `ChainProvider_MatchesSeriesExactly` method.

**Required elements**:

1. Skip quote 80 during initial loop (late arrival scenario)
2. Add Insert operation: `quoteHub.Insert(Quotes[80]);`
3. Add Remove operation: `quoteHub.Remove(Quotes[removeAtIndex]);`
4. Add duplicate quote sending for robustness testing
5. Use `RevisedQuotes` (not `Quotes`) for Series comparison after mutations
6. Assert count is `length - 1` (after removal)

**Example**:

```csharp
[TestMethod]
public void ChainProvider_MatchesSeriesExactly()
{
    const int emaPeriods = 20;
    const int smaPeriods = 10;
    int length = Quotes.Count;

    QuoteHub quoteHub = new();
    SmaHub observer = quoteHub
        .ToEmaHub(emaPeriods)
        .ToSmaHub(smaPeriods);

    // Emulate adding quotes with skip logic
    for (int i = 0; i < length; i++)
    {
        if (i == 80) { continue; }  // Skip for late arrival
        Quote q = Quotes[i];
        quoteHub.Add(q);
        if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
    }

    // Late arrival
    quoteHub.Insert(Quotes[80]);

    // Delete
    quoteHub.Remove(Quotes[removeAtIndex]);

    // Compare with RevisedQuotes (excludes removeAtIndex)
    IReadOnlyList<SmaResult> seriesList = RevisedQuotes
        .ToEma(emaPeriods)
        .ToSma(smaPeriods);

    // Assert
    observer.Results.Should().HaveCount(length - 1);
    observer.Results.Should().BeEquivalentTo(seriesList);

    observer.Unsubscribe();
    quoteHub.EndTransmission();
}
```

### Related Documentation

- Streaming indicators plan: `docs/plans/streaming-indicators.plan.md`
- StreamHub development guidelines: `.github/instructions/indicator-stream.instructions.md`
- Test base classes: `tests/indicators/_base/StreamHubTestBase.cs`

---
Last updated: December 27, 2025
