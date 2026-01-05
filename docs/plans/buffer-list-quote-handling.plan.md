# BufferList quote handling resilience

This document analyzes the feasibility of adding limited healing capabilities to BufferList for handling duplicate or newer quote arrivals.

## TL;DR

Recommendation: Implement limited last-point healing for BufferList

BufferList should support **last-point-only healing** to handle duplicate timestamps and quote corrections at the most recent position. This provides meaningful resilience for real-time streaming use cases without the complexity of full historical state reconstruction that StreamHub provides.

Specifically:

- **Duplicate detection**: Detect when incoming quote has same timestamp as last result
- **Last-point update**: Update the last result and internal state when duplicate/correction arrives
- **No historical healing**: Do not support late-arriving quotes in middle of sequence (out of scope for BufferList design)
- **Stateful rollback**: Each BufferList implementation provides rollback method to restore state to position before last result

This balances BufferList's design goal of simplicity with practical resilience needs for streaming data feeds.

## Plan with tasks

If approved, implement last-point healing for BufferList:

- [ ] **Phase 1: Framework implementation** (4-6 hours)
  - [ ] Add `UpdateLast(TResult item)` protected method to `BufferList<T>` base class
  - [ ] Add abstract `RollbackLastState()` method to `BufferList<T>` for derived classes to implement
  - [ ] Add timestamp comparison logic to `AddInternal()` for duplicate detection
  - [ ] Update documentation for `BufferList<T>` base class

- [ ] **Phase 2: Pilot implementation** (2-3 hours)
  - [ ] Implement `RollbackLastState()` for RsiList (simple stateful example)
  - [ ] Implement `RollbackLastState()` for MacdList (complex multi-buffer example)
  - [ ] Add unit tests for duplicate/correction handling in both indicators

- [ ] **Phase 3: Comprehensive rollout** (12-16 hours)
  - [ ] Implement `RollbackLastState()` for remaining 77 BufferList indicators
  - [ ] Add duplicate handling tests for all indicators
  - [ ] Update BufferListTestBase with abstract test requirement
  - [ ] Update indicator documentation with healing behavior notes

- [ ] **Phase 4: Validation and documentation** (2-3 hours)
  - [ ] Run full test suite to verify mathematical precision maintained
  - [ ] Performance benchmarks to ensure no overhead for non-duplicate path
  - [ ] Update AGENTS.md and indicator-buffer skill with healing guidance
  - [ ] Add examples to documentation showing duplicate handling

Total estimated effort: 20-28 hours

## Appendix A: Analysis

### Current state

**BufferList architecture:**

BufferList is designed as a simpler alternative to StreamHub for incremental indicator calculations:

- **Single-direction**: Only appends new results via `AddInternal()`, no built-in update or removal
- **Stateful**: Maintains internal buffers (Queues) and running state variables (EMAs, sums, previous values)
- **Sequential dependency**: Each new result depends on complete prior state being correct
- **No timestamp validation**: Assumes chronological order, no duplicate detection

**Key differences from StreamHub:**

| Feature | StreamHub | BufferList |
|---------|-----------|------------|
| Duplicate detection | Yes (IsOverflowing) | No |
| Late arrival handling | Yes (rebuilds from timestamp) | No |
| State rollback | Yes (RollbackState) | No |
| Update semantics | Replace + rebuild observers | Append only |
| Complexity | High (observer pattern, caching) | Low (stateful queue) |
| Performance | 5-8x overhead | Near-Series performance |

**Problem scope:**

Real-time data feeds commonly send:

1. **Duplicate timestamps**: Same quote arrives twice (network retry, feed quirk)
2. **Quote corrections**: Updated values for most recent timestamp (price correction, volume update)
3. **Late arrivals**: Historical quote arrives after newer quotes (out of scope for BufferList)

Currently, BufferList silently accepts duplicates/corrections as new sequential quotes, corrupting internal state.

### Recommended approaches

#### Approach 1: Last-point-only healing (RECOMMENDED)

**Design:**

- Detect duplicate timestamp on incoming quote
- If timestamp matches last result, rollback internal state and update last result
- Only heal the most recent point (no historical healing)
- Each indicator implements `RollbackLastState()` to restore state

**Pros:**

- Addresses 80%+ of real-world streaming issues (most duplicates/corrections are at tail)
- Minimal complexity - no historical reconstruction needed
- Preserves BufferList's performance advantage
- Clear contract: "heal last point only"
- Stateful rollback is indicator-specific and well-defined

**Cons:**

- Does not handle late arrivals in middle of sequence
- Requires each of 79 BufferList implementations to add rollback logic
- Slightly more complex than current append-only model

**Implementation sketch:**

```csharp
// BufferList.cs base class
protected void AddInternal(TResult item)
{
    // Check for duplicate timestamp at last position
    if (Count > 0 && item.Timestamp == this[Count - 1].Timestamp)
    {
        // Rollback derived class state
        RollbackLastState();
        
        // Update last result in place
        UpdateInternal(Count - 1, item);
    }
    else
    {
        // Normal append
        _internalList.Add(item);
        
        if (_internalList.Count > MaxListSize)
        {
            PruneList();
        }
    }
}

protected abstract void RollbackLastState();
```

```csharp
// RsiList implementation example
protected override void RollbackLastState()
{
    // Remove last item from buffer
    if (_buffer.Count > 0)
    {
        // Recreate buffer without last item
        Queue<(double, double)> tempBuffer = new(_buffer);
        tempBuffer.Dequeue(); // Remove oldest (first in)
        _buffer.Clear();
        foreach (var item in tempBuffer)
        {
            _buffer.Enqueue(item);
        }
    }
    
    // Restore previous EMA state would require keeping prior state
    // (this is the complexity tradeoff)
}
```

**Complexity assessment:**

Each indicator needs different rollback logic:

- **Simple (25-30 indicators)**: Single queue, restore by removing last item (SMA, EMA, RSI)
- **Medium (30-35 indicators)**: Multiple queues, restore each buffer (MACD, Stoch, ADX)
- **Complex (15-20 indicators)**: Stateful variables need prior values cached (Parabolic SAR, Ichimoku)

#### Approach 2: Full healing with timestamp validation

**Design:**

- Detect any duplicate or out-of-order timestamp
- Rebuild entire indicator state from scratch when timestamp violation detected
- Similar to StreamHub's rebuild mechanism

**Pros:**

- Handles all timestamp scenarios (duplicates, corrections, late arrivals)
- Mathematically precise - always correct state
- Consistent with StreamHub behavior

**Cons:**

- Defeats BufferList's purpose (simplicity and performance)
- O(n) rebuild cost on every duplicate/correction
- Requires full historical quote retention (not available in BufferList)
- Complex implementation - essentially reimplementing StreamHub

**Verdict:** Not recommended. If users need full healing, use StreamHub.

#### Approach 3: No healing, document limitation

**Design:**

- Keep BufferList append-only
- Document that users must ensure chronological, deduplicated input
- Add validation option to throw on duplicate timestamps

**Pros:**

- Zero implementation effort
- Preserves simplest possible design
- Forces users to clean data upstream

**Cons:**

- Does not address real-world streaming use case
- Puts burden on every user to implement deduplication
- Silent corruption when duplicates arrive (current behavior)
- Adds no value over current state

**Verdict:** Suboptimal user experience. Issue #1018 exists because users need some resilience.

### Feasibility constraints

**Technical feasibility:** High

- `UpdateInternal()` already exists in BufferList base class
- Timestamp comparison is trivial
- Each indicator already manages its own state

**Implementation effort:** Medium

- 79 BufferList implementations need `RollbackLastState()` method
- Each requires analysis of what state to rollback
- Comprehensive test coverage needed

**Performance impact:** Minimal

- Duplicate check is O(1) timestamp comparison
- Rollback only executes on duplicates (rare in practice)
- No impact on non-duplicate path

**Mathematical precision:** Maintained

- Rollback + recompute guarantees same result as if duplicate never arrived
- No approximations or shortcuts

**Design consistency:** Good fit

- BufferList already has stateful update methods (`UpdateInternal`, `RemoveAt`)
- Aligns with "limited healing" positioning vs. StreamHub's full healing
- Clear upgrade path: BufferList for last-point healing, StreamHub for full healing

### Design boundaries

**What BufferList healing SHOULD do:**

- Detect duplicate timestamp at last position
- Update last result when duplicate/correction arrives
- Rollback internal state to before last result
- Maintain mathematical precision
- Document healing limitations clearly

**What BufferList healing SHOULD NOT do:**

- Handle late arrivals in middle of sequence (use StreamHub)
- Rebuild entire history on timestamp violations (use StreamHub)
- Maintain observer pattern for notifications (use StreamHub)
- Support arbitrary quote insertion (use StreamHub)
- Cache historical quotes for rebuilds (use StreamHub)

**Clear contract:**

> BufferList provides **last-point healing only**: duplicate timestamps or corrections at the most recent position are handled by rolling back internal state and updating the last result. Historical late arrivals are not supported - use StreamHub for full healing capabilities.

### Risk assessment

**Low risk:**

- Scoped to last-point only (bounded complexity)
- Each indicator controls its own rollback logic (no cross-cutting state)
- Tests verify mathematical precision maintained
- Performance impact negligible (fast-path unchanged)

**Medium risk:**

- 79 implementations need rollback methods (high effort)
- Complex indicators (Parabolic SAR, Ichimoku) may need state caching
- Incomplete rollback logic could corrupt state

**Mitigation:**

- Start with pilot on 2-3 indicators (RSI, MACD, Stoch)
- Add abstract test requirement for duplicate handling
- Comprehensive test coverage before rollout
- Performance benchmarks to verify no regression

### Alternative: Optional healing mode

Instead of always-on healing, add opt-in flag:

```csharp
public bool EnableLastPointHealing { get; set; } = true;
```

**Pros:**

- Users who guarantee deduplicated input can disable for maximum performance
- Backward compatible (can default to true)

**Cons:**

- More complex API surface
- Testing burden (test both modes)
- Likely premature optimization (duplicate check is O(1))

**Verdict:** Not recommended unless performance testing shows measurable impact.

---
Last updated: January 5, 2026
