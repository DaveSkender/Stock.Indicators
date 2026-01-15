# Hub-with-Hub Pattern Analysis

This document analyzes all StreamHub implementations that use internal hubs, examining their subscription patterns and potential race conditions.

## Overview

The StochRsiHub race condition (documented in this PR) revealed an architectural pattern that affects multiple indicators. This analysis examines all hubs-with-hubs to identify similar issues.

## StochRsiHub - BROKEN (v3)

**Pattern:** StochRsiHub contains internal RsiHub

**v3 Implementation (BROKEN):**

```csharp
internal StochRsiHub(IChainProvider<IReusable> provider, ...) 
    : base(provider)  // ❌ BROKEN: subscribes to same provider as rsiHub
{
    rsiHub = provider.ToRsiHub(rsiPeriods);  // Both subscribe to same provider
}
```

**Failure Mode:**

- During rebuild (late arrival, removal), `ToIndicator()` accesses `rsiHub.Cache[i]`
- Race condition: If StochRsiHub processes update before rsiHub, `Cache[i]` doesn't exist
- Result: `ArgumentOutOfRangeException` at line 81: `rsiHub.Cache[i]`

**Correct Architecture (post-fix):**

```csharp
internal StochRsiHub(IChainProvider<IReusable> provider, ...) 
    : base(provider.ToRsiHub(rsiPeriods))  // ✅ CORRECT: chains through RsiHub
{
    rsiHub = (RsiHub)ProviderCache;  // Reference to our provider
}
```

Data flow: `provider → RsiHub → StochRsiHub` (proper chaining eliminates race condition)

## ConnorsRsiHub - BROKEN (v3)

**Pattern:** ConnorsRsiHub contains internal RsiHub

**v3 Implementation (BROKEN):**

```csharp
internal ConnorsRsiHub(IChainProvider<IReusable> provider, ...) 
    : base(provider)  // ❌ BROKEN: same pattern as StochRsiHub
{
    rsiHub = provider.ToRsiHub(rsiPeriods);  // Both subscribe to same provider
}
```

**Failure Mode:**

- Identical race condition to StochRsiHub
- Line 61: `double? rsi = rsiHub.Cache[i].Rsi;` can throw `ArgumentOutOfRangeException`
- Occurs during rebuild operations when ConnorsRsiHub processes before rsiHub

**Correct Architecture (post-fix):**

```csharp
internal ConnorsRsiHub(IChainProvider<IReusable> provider, ...) 
    : base(provider.ToRsiHub(rsiPeriods))  // ✅ CORRECT
{
    rsiHub = (RsiHub)ProviderCache;
}
```

## ChandelierHub - BROKEN (v3)

**Pattern:** ChandelierHub contains internal AtrHub

**v3 Implementation (BROKEN):**

```csharp
internal ChandelierHub(IQuoteProvider<IQuote> provider, ...) 
    : base(provider)  // ❌ BROKEN: same pattern
{
    atrHub = provider.ToAtrHub(lookbackPeriods);  // Both subscribe to same provider
}
```

**Failure Mode:**

- Line 76: `double? atr = atrHub.Cache[i].Atr;` can throw `ArgumentOutOfRangeException`
- ChandelierHub even has defensive bounds checking at lines 68-74, suggesting awareness of the issue
- Current code has a comment "System invariant" claiming atrHub processes first, but this is not guaranteed

**Correct Architecture (post-fix):**

```csharp
internal ChandelierHub(IQuoteProvider<IQuote> provider, ...) 
    : base(provider.ToAtrHub(lookbackPeriods))  // ✅ CORRECT
{
    atrHub = (AtrHub)ProviderCache;
}
```

## GatorHub - CORRECT (v3)

**Pattern:** GatorHub subscribes to AlligatorHub

**v3 Implementation (CORRECT):**

```csharp
internal GatorHub(AlligatorHub alligatorHub)
    : base(alligatorHub)  // ✅ CORRECT: subscribes directly to AlligatorHub
{
    // No internal hub created - properly chains
}
```

**Why This Works:**

- Gator doesn't create an internal AlligatorHub
- It receives AlligatorHub as parameter and subscribes to it
- No race condition possible - proper provider chain from the start

**Extension Method Pattern:**

```csharp
public static GatorHub ToGatorHub(this IChainProvider<IReusable> chainProvider)
{
    AlligatorHub alligatorHub = chainProvider.ToAlligatorHub();  // Create hub
    return new GatorHub(alligatorHub);  // Pass as provider
}
```

This is the CORRECT pattern - create the inner hub in the extension method, pass it as the provider to the outer hub's constructor.

## PvoHub - CORRECT (v3)

**Pattern:** PvoHub uses provider transformation in extension method

**v3 Implementation (CORRECT):**

```csharp
public static PvoHub ToPvoHub(this IQuoteProvider<IQuote> quoteProvider, ...)
    => quoteProvider
        .ToQuotePartHub(CandlePart.Volume)  // ✅ Transform provider first
        .ToPvoHub(fastPeriods, slowPeriods, signalPeriods);  // Then create hub
```

**Why This Works:**

- PvoHub itself doesn't create any internal hubs
- Extension method transforms the provider chain before creating PvoHub
- Data flows: `QuoteHub → QuotePartHub → PvoHub`
- No race condition - proper chaining

## Summary

| Hub | Internal Hub | v3 Status | Issue |
|-----|--------------|-----------|-------|
| **StochRsiHub** | RsiHub | ❌ BROKEN | Race condition: `rsiHub.Cache[i]` |
| **ConnorsRsiHub** | RsiHub | ❌ BROKEN | Race condition: `rsiHub.Cache[i]` |
| **ChandelierHub** | AtrHub | ❌ BROKEN | Race condition: `atrHub.Cache[i]` |
| **GatorHub** | AlligatorHub | ✅ CORRECT | Chains properly |
| **PvoHub** | QuotePartHub | ✅ CORRECT | Transforms provider |

## Architectural Patterns

### Anti-Pattern (BROKEN)

```csharp
internal OuterHub(IProvider provider) : base(provider)
{
    innerHub = provider.ToInnerHub();  // ❌ Both subscribe to same provider
}
```

### Correct Pattern 1 - Pass Hub as Provider

```csharp
internal OuterHub(InnerHub innerHub) : base(innerHub)  // ✅ Subscribe to inner hub
{
    // innerHub is our provider, no duplicate subscription
}

// Extension method creates and chains:
public static OuterHub ToOuterHub(this IProvider provider)
{
    InnerHub inner = provider.ToInnerHub();
    return new OuterHub(inner);
}
```

### Correct Pattern 2 - Chain in Base Constructor

```csharp
internal OuterHub(IProvider provider) 
    : base(provider.ToInnerHub())  // ✅ Chain through inner hub
{
    innerHub = (InnerHub)ProviderCache;  // Reference to our provider
}
```

## Recommendation

All three broken hubs (StochRsiHub, ConnorsRsiHub, ChandelierHub) should be fixed using **Correct Pattern 2** to eliminate race conditions and ensure deterministic notification ordering.

---
Last updated: 2026-01-15
