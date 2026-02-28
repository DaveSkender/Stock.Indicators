# Testing patterns

## FluentAssertions

```csharp
result.Value.Should().BeApproximately(expected, Money6);
results.Should().HaveCount(502);
act.Should().Throw<ArgumentOutOfRangeException>();
act.Should().Throw<ArgumentNullException>().WithParameterName("quotes");
```

## Precision constants

Defined in `TestBaseWithPrecision`. Use for `BeApproximately()` with manually calculated values only:

| Constant | Value | Use case |
| -------- | ----- | -------- |
| `Money4` | 0.00005 | Standard spot check tolerance |
| `Money5` | 0.000005 | Higher precision |
| `Money6` | 0.0000005 | Most calculations |
| `Money8` | 0.000000005 | Very high precision |
| `Money10` | 0.00000000005 | Full-dataset regression validation |
| `Money12` | 0.0000000000005 | Maximum precision regression |

## Series parity

Use `IsExactly()` (NOT `Should().Be()`):

```csharp
buffer.IsExactly(quotes.ToIndicator(14));
```

## BufferList constraints

Inherit `BufferListTestBase`.

`IIncrementFromQuote` → implement `ITestQuoteBufferList`:

- `PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()`
- `Clear_WithState_ResetsState()`
- `AddQuote_IncrementsResults()`
- `AddQuotesBatch_IncrementsResults()`
- `QuotesCtor_OnInstantiation_IncrementsResults()`

`IIncrementFromChain` → implement `ITestChainBufferList` (all above, plus):

- `AddReusableItem_IncrementsResults()`
- `AddReusableItemBatch_IncrementsResults()`
- `AddDateAndValue_IncrementsResults()`

`ITestCustomBufferListCache` (when non-standard Queue history caching is used):

- `CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()`

## StreamHub constraints

Inherit `StreamHubTestBase`. Abstract method (compile error if missing):

- `ToStringOverride_ReturnsExpectedName()`

Implement ONE observer interface:

- `ITestChainObserver`: `ChainObserver_ChainedProvider_MatchesSeriesExactly()` + inherits all `ITestQuoteObserver` methods
- `ITestQuoteObserver`: `QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()`, `WithCachePruning_MatchesSeriesExactly()`

If hub acts as chain provider, also implement `ITestChainProvider`:

- `ChainProvider_MatchesSeriesExactly()`
