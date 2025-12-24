# StreamHub Tests Checklist

Use this checklist to verify StreamHub tests meet project standards.

- [ ] Inherits `StreamHubTestBase`
- [ ] Implements exactly one observer interface:
  - [ ] `ITestChainObserver` OR
  - [ ] `ITestQuoteObserver` OR
  - [ ] `ITestPairsObserver`
- [ ] Implements at most one provider interface:
  - [ ] `ITestChainProvider`
- [ ] Comprehensive rollback validation present (follow EMA hub test pattern):
  - [ ] Prefill warmup window before subscribing
  - [ ] Stream in-order including a few duplicates
  - [ ] Insert a late historical quote and verify recalculation parity
  - [ ] Remove a historical quote and verify recalculation parity
  - [ ] Compare results to Series with strict ordering
  - [ ] Clean up with `Unsubscribe()` and `EndTransmission()`
- [ ] Dual-stream indicators only:
  - [ ] Use `ITestPairsObserver` (no quote observer)
  - [ ] Validate timestamp sync and sufficient data checks
- [ ] Reset/state behavior covered (`Reset()`, reinitialize)
- [ ] Bad/insufficient data and boundary period tests included
- [ ] Performance placeholder present in benchmarks project if applicable

---

**Source**: Migrated from .specify/specs/001-develop-streaming-indicators/checklists/stream-hub-tests.md  
**Last updated**: December 24, 2025
