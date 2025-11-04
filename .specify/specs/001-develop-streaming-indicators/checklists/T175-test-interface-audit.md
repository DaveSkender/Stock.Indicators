# T175: StreamHub Test Interface Compliance Audit

**Date**: 2025-11-04  
**Purpose**: Audit all existing StreamHub test classes for proper test interface implementation per `.github/instructions/indicator-stream.instructions.md`

## Summary

- **Total test files audited**: 81
- **✓ Compliant**: 65 (80.2%)
- **✗ Issues found**: 13 (16.0%)
- **Hub not found**: 3 (3.7%)

## Interface Selection Rules (from instruction file)

Per `.github/instructions/indicator-stream.instructions.md`, test interfaces should be selected based on the hub's provider base class:

| Hub Provider Base Class | Required Test Interfaces | Notes |
|-------------------------|-------------------------|-------|
| `ChainProvider<IReusable, TResult>` | `ITestChainObserver`, `ITestChainProvider` | Chainable from reusable values |
| `ChainProvider<IQuote, TResult>` | `ITestQuoteObserver`, `ITestChainProvider` | Chainable from quotes |
| `QuoteProvider<TIn, TResult>` | `ITestQuoteObserver`, `ITestChainProvider` | Quote-only providers |
| `PairsProvider<TIn, TResult>` | `ITestPairsObserver` | Dual-stream indicators (NO quote observer) |
| `StreamHub<IQuote, TResult>` | `ITestQuoteObserver` | Direct quote stream (multi-result, non-chainable) |
| `StreamHub<IReusable, TResult>` | `ITestChainObserver` | Direct reusable stream (multi-result) |

**Important**: `ITestChainObserver` inherits `ITestQuoteObserver`. Do not implement both redundantly.

## Issues Found (13 total)

### Category 1: Missing ITestChainProvider (6 indicators)

These indicators extend ChainProvider or QuoteProvider but are missing the `ITestChainProvider` interface:

- [ ] **BollingerBands** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/a-d/BollingerBands/BollingerBands.StreamHub.Tests.cs`
  - Action: Add both `ITestChainObserver` and `ITestChainProvider`

- [ ] **Cci** (ChainProvider<IQuote>)
  - Current: `ITestQuoteObserver`
  - Expected: `ITestQuoteObserver`, `ITestChainProvider`
  - File: `tests/indicators/a-d/Cci/Cci.StreamHub.Tests.cs`
  - Action: Add `ITestChainProvider`

- [ ] **Epma** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/e-k/Epma/Epma.StreamHub.Tests.cs`
  - Action: Replace `ITestQuoteObserver` with `ITestChainObserver`, add `ITestChainProvider`

- [ ] **Kvo** (ChainProvider<IQuote>)
  - Current: `ITestQuoteObserver`
  - Expected: `ITestQuoteObserver`, `ITestChainProvider`
  - File: `tests/indicators/e-k/Kvo/Kvo.StreamHub.Tests.cs`
  - Action: Add `ITestChainProvider`

- [ ] **Obv** (ChainProvider<IQuote>)
  - Current: `ITestQuoteObserver`
  - Expected: `ITestQuoteObserver`, `ITestChainProvider`
  - File: `tests/indicators/m-r/Obv/Obv.StreamHub.Tests.cs`
  - Action: Add `ITestChainProvider`

- [ ] **Pivots** (ChainProvider<IQuote>)
  - Current: `ITestQuoteObserver`
  - Expected: `ITestQuoteObserver`, `ITestChainProvider`
  - File: `tests/indicators/m-r/Pivots/Pivots.StreamHub.Tests.cs`
  - Action: Add `ITestChainProvider`

### Category 2: Wrong Observer Type (5 indicators)

These ChainProvider<IReusable> indicators should use `ITestChainObserver` instead of `ITestQuoteObserver`:

- [ ] **ForceIndex** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`, `ITestChainProvider`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/e-k/ForceIndex/ForceIndex.StreamHub.Tests.cs`
  - Action: Replace `ITestQuoteObserver` with `ITestChainObserver`

- [ ] **T3** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`, `ITestChainProvider`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/s-z/T3/T3.StreamHub.Tests.cs`
  - Action: Replace `ITestQuoteObserver` with `ITestChainObserver`

- [ ] **Tema** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`, `ITestChainProvider`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/s-z/Tema/Tema.StreamHub.Tests.cs`
  - Action: Replace `ITestQuoteObserver` with `ITestChainObserver`

- [ ] **Ultimate** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`, `ITestChainProvider`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/s-z/Ultimate/Ultimate.StreamHub.Tests.cs`
  - Action: Replace `ITestQuoteObserver` with `ITestChainObserver`

- [ ] **Vwma** (ChainProvider<IReusable>)
  - Current: `ITestQuoteObserver`, `ITestChainProvider`
  - Expected: `ITestChainObserver`, `ITestChainProvider`
  - File: `tests/indicators/s-z/Vwma/Vwma.StreamHub.Tests.cs`
  - Action: Replace `ITestQuoteObserver` with `ITestChainObserver`

### Category 3: Wrong Observer Type for Quote Provider (2 indicators)

These ChainProvider<IQuote> indicators should use `ITestQuoteObserver` instead of `ITestChainObserver`:

- [ ] **Bop** (ChainProvider<IQuote>)
  - Current: `ITestChainObserver`, `ITestChainProvider`
  - Expected: `ITestQuoteObserver`, `ITestChainProvider`
  - File: `tests/indicators/a-d/Bop/Bop.StreamHub.Tests.cs`
  - Action: Replace `ITestChainObserver` with `ITestQuoteObserver`

- [ ] **ChaikinOsc** (ChainProvider<IQuote>)
  - Current: `ITestChainObserver`, `ITestChainProvider`
  - Expected: `ITestQuoteObserver`, `ITestChainProvider`
  - File: `tests/indicators/a-d/ChaikinOsc/ChaikinOsc.StreamHub.Tests.cs`
  - Action: Replace `ITestChainObserver` with `ITestQuoteObserver`

### Category 4: Unexpected ChainProvider (1 indicator)

This indicator extends `StreamHub<IReusable>` directly (not ChainProvider) but implements ChainProvider test interface:

- [ ] **MaEnvelopes** (StreamHub<IReusable>)
  - Current: `ITestChainObserver`, `ITestChainProvider`
  - Expected: `ITestChainObserver`
  - File: `tests/indicators/m-r/MaEnvelopes/MaEnvelopes.StreamHub.Tests.cs`
  - Action: Remove `ITestChainProvider` (hub doesn't support chaining)
  - Note: Review if hub implementation should be ChainProvider instead

### Category 5: Hub Not Found (3 indicators)

Test files exist but corresponding hub implementation not found:

- **Quotes**: `tests/indicators/_common/Quotes/Quote.StreamHub.Tests.cs`
  - Note: This may be intentional (QuoteHub itself, not an indicator hub)
  
- **StochRsi**: `tests/indicators/s-z/StochRsi/StochRsi.StreamHub.Tests.cs`
  - Note: Verify if StreamHub implementation exists or is planned

- **Prs**: Test file shows as `ITestPairsObserver` - verify PairsProvider implementation exists

## Compliant Indicators (65 total - representative sample)

These indicators correctly implement test interfaces per guidelines:

✓ **Ema** (ChainProvider<IReusable>): `ITestChainObserver`, `ITestChainProvider`  
✓ **Sma** (ChainProvider<IReusable>): `ITestChainObserver`, `ITestChainProvider`  
✓ **Rsi** (ChainProvider<IReusable>): `ITestChainObserver`, `ITestChainProvider`  
✓ **Correlation** (PairsProvider): `ITestPairsObserver`  
✓ **Beta** (PairsProvider): `ITestPairsObserver`  
✓ **Adx** (ChainProvider<IQuote>): `ITestQuoteObserver`, `ITestChainProvider`  
✓ **Atr** (ChainProvider<IQuote>): `ITestQuoteObserver`, `ITestChainProvider`  
✓ **Chandelier** (StreamHub<IQuote>): `ITestQuoteObserver`  
✓ **Alligator** (StreamHub<IReusable>): `ITestChainObserver`  

Full list available in `/tmp/audit_results_detailed.txt`

## Next Steps (Task T176)

Task T176 should address the 13 issues identified above:

1. **Priority 1**: Missing ITestChainProvider (Category 1) - 6 indicators
2. **Priority 2**: Wrong observer type for reusable chains (Category 2) - 5 indicators  
3. **Priority 3**: Wrong observer type for quote chains (Category 3) - 2 indicators
4. **Priority 4**: Review MaEnvelopes hub implementation (Category 4) - 1 indicator

Each fix should:
- Update test class declaration to implement correct interfaces
- Ensure corresponding test methods exist (QuoteObserver, ChainObserver, ChainProvider, etc.)
- Verify tests follow canonical patterns from `Ema.StreamHub.Tests.cs`
- Run tests to confirm no regressions

## Verification Commands

After fixes are applied, re-run this audit:

```bash
# Run audit script
python3 /tmp/comprehensive_audit_v2.py

# Verify specific indicator
grep "class.*StreamHubTestBase" tests/indicators/path/to/Indicator.StreamHub.Tests.cs

# Check hub provider type
grep "class.*Hub.*:" src/path/to/Indicator.StreamHub.cs
```

## References

- Instruction file: `.github/instructions/indicator-stream.instructions.md`
- Canonical test pattern: `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs`
- Test base class: `tests/indicators/_base/StreamHubTestBase.cs`
- Test interfaces: `tests/indicators/_base/ITest*.cs`

---

**Audit completed**: 2025-11-04  
**Script used**: `/tmp/comprehensive_audit_v2.py`  
**Raw results**: `/tmp/audit_results_detailed.txt`
