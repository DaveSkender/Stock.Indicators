# BufferList Tests Checklist

Use this checklist to verify BufferList tests meet project standards.

- [ ] Test class inherits `BufferListTestBase` (not `TestBase`)
- [ ] Implements the correct test interface(s):
  - [ ] `ITestChainBufferList` OR
  - [ ] `ITestQuoteBufferList`
  - [ ] `ITestCustomBufferListCache` when using custom non-`Queue<T>` caches (for example, `List<T>`)
- [ ] All five required tests are present and passing:
  - [ ] AddQuote_IncrementsResults()
  - [ ] AddQuotesBatch_IncrementsResults()
  - [ ] QuotesCtor_OnInstantiation_IncrementsResults()
  - [ ] Clear_WithState_ResetsState()
  - [ ] PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
- [ ] If applicable, interface-specific tests are implemented and passing (for chain paths):
  - [ ] AddReusableItem_IncrementsResults()
  - [ ] AddReusableItemBatch_IncrementsResults()
  - [ ] AddDateAndValue_IncrementsResults() (chain)
- [ ] Results strictly match Series outputs for the same inputs
- [ ] Edge cases covered (insufficient data, parameter bounds, reset)
- [ ] Performance placeholder present in benchmarks project if applicable

---

**Source**: Migrated from .specify/specs/001-develop-streaming-indicators/checklists/buffer-list-tests.md  
**Last updated**: December 31, 2025
