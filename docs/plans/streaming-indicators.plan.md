# Streaming Indicators implementation plan

This document tracks remaining work and architectural direction for the v3 streaming indicators implementation.

**Status (2026-05-24, after swarm review).** Streaming framework is implementation-complete and ships-ready in principle, but a second-pass review (Architect, Inspector, Tester, Stercorator, Researcher, Designer, Skills-auditor) surfaced two classes of work that should land before v3.0 stable: (1) **guidance and documentation alignment** — a contributor-facing skill teaches a non-compiling API, root `AGENTS.md` references a directory that no longer exists, and several net-new v3 features are undocumented in skills; (2) **test coverage hardening** — late-arrival and cache-pruning tests are inconsistent across the 55 `RollbackState` overrides, and there is no contract-level rollback-equivalence test. The recommendation is **ship v3.0 stable after a focused 2–3 day quality pass** covering these items, plus the three previously-identified release gates (baseline refresh, branch migration, community feedback).

**Coverage (verified 2026-05-24 via `CatalogShouldHaveExactStyleCounts`):**

- Series listings: 85 (84 indicators + `QuotePart`)
- BufferList listings: 79 (78 indicators + `QuotePart`)
- StreamHub listings: 79 (78 indicators + `QuotePart`)
- Streaming docs coverage: 78 of 79 (`QuotePart` BufferList/StreamHub variants not yet documented on the utilities page — see D009)
- Non-streamable (Series only): Beta, Correlation, Prs, RenkoAtr, StdDevChannels, ZigZag

**Related plans**: [Branching Strategy Migration](branching-strategy.plan.md) (required for v3.0 stable release), [File Reorganization](file-reorg.plan.md) (deferred to v3.1).

**Related guidance** (cross-reference for contributors and AI agents):
`AGENTS.md` (root), `src/AGENTS.md`, `tests/AGENTS.md`, `docs/AGENTS.md`, `.agents/skills/indicator-stream/SKILL.md`, `.agents/skills/indicator-buffer/SKILL.md`, `.agents/skills/indicator-catalog/SKILL.md`, `.agents/skills/performance-testing/SKILL.md`. Some of these have alignment issues — see Guidance doc alignment section below.

---

## Recommendation — Ship v3.0 stable after a 2–3 day quality pass

After the swarm review, the recommendation upgrades from "ship as-is after release gates" to **"ship after a focused quality pass"**. The architecture is sound (Architect verdict: no blockers, four v3.1 refactors queued). The implementation is correct (Tester verdict: parity is strong; rigor gaps are addressable). But:

- A new contributor or AI agent following `.agents/skills/indicator-catalog/SKILL.md` today would write code that doesn't compile (`_catalog.Add(...)` vs the actual `_listings.Add(...)`). This is fixable in 30 minutes and worth fixing before stable.
- Of the 55 hubs that override `RollbackState`, only ~20–30 have explicit late-arrival tests. The framework's most-fragile invariant (state correctness after rollback) is inconsistently validated. A generic rollback-equivalence contract test would catch silent regressions in any hub.
- Stale guidance (`.specify/` reference in root `AGENTS.md`, missing AggregatorHub documentation, irrelevant Vue-ecosystem skills) creates noise for both human and AI contributors.

None of this requires changing the architecture. All of it can ship inside a v3.0 stable release window.

### Why ship after the quality pass (not delay further)

1. **Architectural completeness.** The dual-track model (`BufferList` for synchronous incremental compute; `StreamHub` for live observer chains with rollback and fault tracking) is justified — Architect (F5) and Researcher (F3) both confirm it sits at or ahead of industry practice (ta4j is pull-only; TA-Lib/Tulip are batch-only). Coverage is 100% of streamable indicators (78 + `QuotePart`). The 6 non-streamable indicators are correctly excluded for documented algorithmic reasons.

2. **Critical performance issues resolved.** P004–P014 StreamHub fixes are merged in code (verified by reading source). The only remaining BufferList "critical" items (P015 Slope, P016 Alligator, P017 Adx) are at or near algorithmic floors **except** P015, where the Researcher persona flagged that streaming OLS with four running sums (Σx, Σy, Σxy, Σx²) is O(1) per update — this is the lower bound and should be verified against the current implementation before reclassifying as "research." See PV001 below.

3. **Framework hardening since last plan update.**
   - Thread-safety for live feeds (PR #1927)
   - Self-recursion guard during rebuild (`_isRebuilding` flag in `StreamHub.cs:23`)
   - Centralized `RollbackState(int)` signature (PR #1978)
   - Aggregator hubs for quote/tick quantization (PR #1875) — net-new feature
   - Stream cache validation and pruning tests (PR #1937, #2002)
   - Read-only cache exposure (`AsReadOnly()`) — note: this returns a live view, not an immutable snapshot; see DOC-ARCH-3.

### What still has to happen before tagging v3.0.0

Grouped into four buckets. Total estimated effort 2–3 working days plus benchmark runtime.

| Bucket | Items | Estimated effort |
| ------ | ----- | ---------------- |
| Release gates | Baseline refresh, branch migration, community feedback window | 1 day + runtime |
| Guidance alignment | G001–G007 below | 4–6 hours |
| Cleanup pass | T203, T230–T234 below | 4–6 hours |
| Test hardening | TC001–TC006 below | 1 day |

### v3.1+ direction — what's worth tackling next

After v3.0 ships, the highest-leverage streaming work is (in priority order):

1. **`Reinitialize()` abstraction (T205).** Replaces replay-on-`OnAdd` with subclass-controlled fast-path bulk init via Series static methods. TODO is already at `src/_common/StreamHub/StreamHub.cs:230`.
2. **Retire `BaseProvider<T>` (Architect F1, Stercorator F1).** Introduce `StreamSource<T>` as a root base separate from `StreamHub<TIn,TOut>`; collapse the workaround. Non-breaking internal refactor — too risky to land in a release-hardening window, perfect for v3.1.
3. **`RollbackState(int)` contract clarification and audit (Architect F2).** XML-document precise index semantics, audit the 55 overrides against the formalized contract. Lightweight v3.0 doc work + v3.1 broader audit.
4. **Private cache lock + routed mutation methods (Architect F3).** Replace `lock (Cache)` with a private monitor, expose `AppendResult`/`ReplaceAt`/`TruncateFrom`. Non-breaking.
5. **`JoinHub<TLeft,TRight,TOut>` primitive (Architect F7, Researcher F8).** Re-enable Beta/Correlation/Prs streaming via an explicit zip-by-timestamp semantic (Rx `Zip`/`CombineLatest` analog). PairsProvider revert (PR #1821) attempted this without a designed primitive.
6. **Rx and `IAsyncEnumerable<T>` adapters (Researcher F1, F2).** `ToObservable()` and `ConsumeAsync(IAsyncEnumerable<TIn>, CancellationToken)` extensions so hubs interoperate with the rest of .NET's streaming ecosystem.
7. **Shared "increment kernels" expansion (Inspector F1).** `Ema.Increment`, `Sma.Increment`, `Tr.Increment`, `Atr.Increment` already exist and are used in ~34 of 160 streaming files (~21%). The pattern works; standardize to ≥80% adoption to halve the duplication tax between `*.StreamHub.cs` and `*.BufferList.cs` siblings.

Medium-priority enhancements (composite naming E010, MaEnvelopes remaining MA types T214, BufferList configuration E009, ADX DMI properties E007–E008, Hurst Anis-Lloyd T215, ISeries.UnixDate T226) can land independently in v3.1 minor releases.

---

## v3.0 Work Remaining

### A. Release gates — non-implementation

- [ ] **RG001 — Refresh performance baselines** (1 hour + runtime). `tools/performance/baselines/*.json` is dated 2026-02-28, pre-P004/P009/P010/P011/P012/P013/P014 and pre-centralized-RollbackState. Run `cd tools/performance && dotnet run -c Release`; copy `BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json` to `baselines/`; optionally tag `baseline-v3.0.0.json`. **Without this, CI regression detection compares against stale numbers.**

- [ ] **RG002 — Get and incorporate final community feedback** (ongoing). Time-boxed by maintainer.

- [ ] **RG003 — Execute branching strategy migration** (10–16 hours). See [branching-strategy.plan.md](branching-strategy.plan.md). `origin/main` is ~25+ commits behind `origin/v3`; PR #1014 is the merge vehicle.

### B. Guidance doc alignment (new section — swarm finding)

Pure documentation fixes. Together ~4–6 hours. None require code changes.

- [ ] **G001 — Fix `indicator-catalog` SKILL.md to match real API** (30 min). **Severity: critical** — current example will produce non-compiling code.
  - **Evidence**: `.agents/skills/indicator-catalog/SKILL.md:105–107` instructs `_catalog.Add(Ema.SeriesListing);` but `src/_common/Catalog/Catalog.Listings.cs:58–63` uses `_listings.Add(...)`. Grep confirms `_catalog.Add` exists in exactly one file in the repo — the SKILL.md itself.
  - **Action**: Replace `_catalog.Add` with `_listings.Add`; correct registration order to `Buffer → Series → Stream` per indicator (alphabetical grouping); add a one-line note about the ordering convention.

- [ ] **G002 — Remove `.specify/` reference from root `AGENTS.md`** (15 min).
  - **Evidence**: `AGENTS.md:48` lists `.specify/` in repo layout; directory does not exist (`ls .specify` fails).
  - **Action**: Remove the line. Replace any "consult open specs" guidance with a pointer to `docs/plans/streaming-indicators.plan.md`. Add `docs/plans/` to the repo-layout block with a one-line description.

- [ ] **G003 — Document AggregatorHub, TickHub, BaseProvider, thread-safety in `indicator-stream` skill** (2–3 hours).
  - **Evidence**: Grep of `.agents/skills/**/SKILL.md` for `Aggregator|TickHub|BaseProvider|CacheLock` returns nothing. Yet `src/_common/Quotes/Quote.AggregatorHub.cs`, `Tick.AggregatorHub.cs`, `Tick.StreamHub.cs` exist as v3-new features (PR #1875); `BaseProvider<T>` is the bootstrap pattern for self-rooted hubs.
  - **Action**: Add an "Aggregator hubs" subsection to `.agents/skills/indicator-stream/SKILL.md` (or `references/aggregator-hubs.md`). Add a `BaseProvider`/self-rooted-hub row to the provider-selection table. Add a one-paragraph "Thread safety contract" noting `CacheLock` and `_isRebuilding` invariants. Pairs naturally with D009 (QuotePart streaming docs).

- [ ] **G004 — Regenerate `src/_common/README.md` directory listing from reality** (30 min).
  - **Evidence**: `src/_common/README.md:14` lists `BufferLists/IIncrementFrom.cs` (does not exist; actual files are `IIncrementFromChain.cs` + `IIncrementFromQuote.cs`); the `StreamHub/` listing omits `CircularDoubleBuffer.cs`, `HubCollection.cs`, `IChainProvider.cs`, `IQuoteProvider.cs`, `IStreamObservable.cs`, `IStreamObserver.cs`, `StreamHub.Observable.cs`, `StreamHub.Observer.cs`.
  - **Action**: Refresh tree; add one-line purpose for each new entry.

- [ ] **G005 — Prune unused community skills and reconcile `skills-lock.json`** (1–2 hours).
  - **Evidence**: `skills-lock.json` tracks 1 skill (`vitepress`). `ls .agents/skills/` shows 27 entries including `nuxt/`, `pinia/`, `slidev/`, `turborepo/`, `unocss/`, `vue/`, `vueuse-functions/`, `vue-router-best-practices/`, `vue-testing-best-practices/`, `tsdown/`, `pnpm/`, `web-design-guidelines/`. This is a .NET library with a VitePress docs site — no Nuxt app, no Pinia store, no Slidev decks, no Turborepo monorepo.
  - **Action**: Prune skills irrelevant to a .NET library + VitePress docs site (keep `vitepress`, `vite`, `vitest`, `markdown`, `code-completion`, `documentation`, `indicator-*`, `performance-testing`, `testing-standards`). Reconcile `skills-lock.json` to track what remains. Update root `AGENTS.md` skills index to match. Agents currently waste context loading irrelevant framework guidance.

- [ ] **G006 — Align `indicator-stream` SKILL performance target with measured reality** (15 min).
  - **Evidence**: `.agents/skills/indicator-stream/SKILL.md:18` asserts "Target: StreamHub ≤ 1.5x slower than Series" but `tools/performance/PERFORMANCE_ANALYSIS.md` documents real 1.5x–11x range, and the MA family is acceptable at 7–11x per PERFORMANCE_ANALYSIS.md ("Pattern 2").
  - **Action**: Replace the bare target with the tiered targets/acceptable/critical bands from PERFORMANCE_ANALYSIS.md, citing it as the source of truth.

- [ ] **G007 — Add cross-references between plan and guidance** (30 min).
  - **Evidence**: `docs/plans/streaming-indicators.plan.md` does not reference any skill or `AGENTS.md`; conversely no skill or `AGENTS.md` references this plan. This document is the design source of truth but is invisible from the contributor entry points.
  - **Action**: This file already added a "Related guidance" section at the top. Reciprocate by adding one line each to `indicator-stream/SKILL.md`, `indicator-buffer/SKILL.md`, and `indicator-catalog/SKILL.md` pointing here. Root `AGENTS.md` should mention `docs/plans/` exists.

### C. Pre-v3.0 cleanup pass

- [ ] **T203 — Remove `EnablePreviewFeatures=true` from project configuration** (30 min, **promoted from "optional"**).
  - **Evidence**: `src/Indicators.csproj:11` enables preview features for the `field` keyword used at `src/_common/BufferLists/BufferList.cs:54,56`. `field` shipped GA in C# 14 / .NET 10 (project targets `net10.0;net9.0;net8.0` with `LangVersion=latest`).
  - **Action**: Remove `<EnablePreviewFeatures>` and `<GenerateRequiresPreviewFeaturesAttribute>`; verify full build across all three target frameworks (with .NET 10 SDK installed, `LangVersion=latest` = C# 14 and `field` works regardless of TFM). Removal eliminates preview-feature noise propagating to downstream consumers.

- [ ] **T230 — Untrack `Stock.Indicators.sln.DotSettings.user`** (5 min).
  - **Evidence**: `Stock.Indicators.sln.DotSettings.user` is checked in at repo root (4 KB ReSharper user state with recent-files list). `.gitignore:47` already contains `*.user`, so future per-developer files are correctly ignored — only the legacy committed file needs removal from the index.
  - **Action**: Run `git rm --cached Stock.Indicators.sln.DotSettings.user`.

- [ ] **T231 — Delete `tools/performance/baselines/before-fixes/`** (10 min, decision needed).
  - **Evidence**: 22 historical JSON snapshots tracked in git. Their value (regression detection against pre-v3-fixes baseline) ends when v3.0 ships.
  - **Action**: Two options — (a) delete and rely on git history + tagged commit `baseline/v3-streaming-prefixes`; (b) keep through v3.0 release and delete in first v3.1 patch. Pick (a) unless there's an external CI dependency.

- [ ] **T232 — Delete empty `src/GlobalSuppressions.cs`** (5 min).
  - **Evidence**: 9-line file with only a `using` directive, no `[assembly: SuppressMessage(...)]` declarations. Triggers `IDE0005` (unused) under aggressive analyzer settings.
  - **Action**: Delete the file. Re-add only when a real suppression is needed.

- [ ] **T233 — Audit `#pragma warning disable` for staleness** (30 min).
  - **Evidence**: 7 occurrences in `src/`. After API churn (renames in `Obsolete.V3.*`), some pragmas may protect already-deleted code paths.
  - **Action**: For each pragma, verify the warning still fires without it; remove unneeded pragmas.

- [ ] **T234 — TODO triage in `src/`** (30 min).
  - **Evidence**: Grep finds multiple TODOs including two in `src/_common/StreamHub/StreamHub.cs:230,234` ("make reinitialization abstract", "race condition between rebuild and subscribe").
  - **Action**: For each TODO, either close (delete the comment if no longer relevant), promote to a plan item, or convert to an `#if DEBUG` assertion. Specifically: `StreamHub.cs:230` is already represented by T205 in v3.1+ — link or remove the TODO; `StreamHub.cs:234` race-condition concern needs to either be closed (with explanation) or filed as a Tester investigation.

### D. Test coverage hardening (new section — Tester swarm finding)

Together ~1 working day. Some items can be parallelized across multiple test PRs.

- [x] **TC001 — Generic rollback-equivalence contract test** (3–4 hours). **High leverage.** *(PR #2010)*
  - **Why**: 55 indicators override `RollbackState(int)`. Per-indicator drift in semantics (Architect F2: index ambiguity) is the highest-risk silent-failure mode.
  - **Action**: Add a parameterized test in `tests/indicators/_common/StreamHub/` that iterates every registered StreamHub: feed N quotes, snapshot cache; feed M more then rollback past the boundary, then re-feed the original tail; assert final cache equals a fresh hub fed the full sequence. Use the catalog as the indicator iterator.
  - Out-of-scope gaps surfaced during implementation are tracked as TC-V31-4 and TC-V31-5.

- [ ] **TC002 — Late-arrival tests for indicators with custom `RollbackState`** (3–4 hours).
  - **Evidence**: `tests/indicators/m-r/Macd/Macd.StreamHub.Tests.cs` has no late-arrival test despite Macd's three-stage cascade (EMA fast, EMA slow, signal EMA); `Stoch.StreamHub.Tests.cs:31` has `Increment` but no out-of-order injection. Macd's signal line is exactly the kind of cascaded state where a rollback bug silently produces wrong values.
  - **Action**: Add `LateArrival` test to every indicator with a custom `RollbackState` override (use Ema's `LateInbound` test as template). Prioritize multi-stage state: Macd, Stoch, Adx, SuperTrend, Chandelier, Renko, Keltner, BollingerBands, Atr, AtrStop, Vortex. TC001 partially covers this generically; TC002 catches per-indicator edge cases the generic test cannot exercise.

- [ ] **TC003 — Bounded-value invariant test** (1–2 hours).
  - **Why**: WilliamsR boundary clamping (T202) was added with tests, but the pattern wasn't generalized. RSI, Stoch %K/%D, Aroon, AroonOsc, MFI, UltimateOscillator, ConnorsRsi all have documented value ranges that are not systematically asserted.
  - **Action**: Add a `BoundedIndicatorInvariant.Tests.cs` enumerating indicators with documented ranges; assert every non-null result is in range across the standard quote set.

- [ ] **TC004 — Expand Macd StreamHub coverage to match Ema** (2 hours).
  - **Evidence**: Macd has 3 StreamHub tests; Ema has 11. Macd is structurally more complex.
  - **Action**: Add `LateInbound`, `Reset`, `MaxCacheSize`, `ChainObserver`, and parameter-variant tests to Macd. Audit all cascaded-state indicators for similar gaps.

- [ ] **TC005 — Quote aggregator boundary tests** (2 hours).
  - **Evidence**: `tests/indicators/_common/Quotes/Quote.Aggregate.Tests.cs` covers happy-path bucketing and invalid-period rejection. Missing: gap-fill on/off variants, late tick crossing a closed bucket boundary, partial-bucket emission on stream end.
  - **Action**: Add `Quote.Aggregate.LateArrival`, `Quote.Aggregate.GapFill`, `Quote.Aggregate.PartialBucket` tests. Pattern-match `TickHub.Tests.cs:60` (`WithCachePruning`) for consistency.

- [ ] **TC006 — Sharpen catalog assertions (consolidates T219)** (1 hour).
  - **Evidence**: `tests/indicators/_common/Catalog/Catalog.Metrics.Tests.cs:33–34` uses `bufferCount.Should().BeGreaterThan(5)` and `streamCount.Should().BeGreaterThan(10)` while `seriesCount.Should().Be(85)` is exact.
  - **Action**: Replace `BeGreaterThan(...)` with `Be(79)` for both Buffer and Stream; replace `totalCount.Should().BeGreaterThan(100)` with `Be(243)`.

### E. Architecture documentation (new section — Architect + Inspector + Researcher findings)

Pure docs work — no code changes. Together ~3–4 hours. Lands as small markdown PRs.

- [ ] **DOC-ARCH-1 — ADR for the dual-track `BufferList` + `StreamHub` model** (1 hour).
  - **Why**: The dual model is principled (Architect F5, Researcher F3) but the rationale is not written down. Future maintainers may try to unify them. Inspector F3 also calls for the plan to lead with simplicity decisions instead of per-indicator state.
  - **Action**: Author MADR-format ADR under `docs/decisions/` (or wherever ADRs live) documenting why both styles exist, when each is appropriate, and what the "shared increment kernel" path looks like for v3.1+. Cite Researcher's industry comparison (ta4j is pull-only; TA-Lib/Tulip are batch-only; library is ahead).

- [ ] **DOC-ARCH-2 — Document `RollbackState(int)` index contract precisely** (1 hour).
  - **Why**: 55 overrides interpret `fromIndex` inconsistently (Architect F2). Adding XML docs to `IStreamHub.RollbackState` defining "the cache position that will be the next to be (re)written; existing entries at `[fromIndex, Count)` have already been removed before this is invoked" gives subclass authors a precise reference.
  - **Action**: Add XML doc to `src/_common/StreamHub/IStreamHub.cs` and `src/_common/StreamHub/StreamHub.cs:377`. Note that an audit of the 55 overrides against the formalized contract is a v3.1 task.

- [ ] **DOC-ARCH-3 — Document `Results` live-view semantics** (30 min).
  - **Why**: `List<T>.AsReadOnly()` returns a `ReadOnlyCollection<T>` wrapping the **live** backing list. Consumers cannot mutate it but enumeration during a concurrent `Add` will throw `InvalidOperationException` (Architect F4). #1585 closed the deviant-mutation hole but not the read-during-write hole.
  - **Action**: Update XML doc on `IStreamHub.Results` and `StreamHub.cs:103` to explicitly state "live read-only view; enumerate within `.ToList()` if upstream may emit during iteration." Reserve `Snapshot()` method addition for v3.1.

- [ ] **DOC-ARCH-4 — Boilerplate budget for new streamable indicators** (30 min).
  - **Why**: Without a stated cost ("a new streamable indicator costs N files and ≤ M LOC of ceremony excluding math"), contributors cannot detect overengineering creep (Inspector F4).
  - **Action**: Add a one-paragraph "Cost of a new streamable indicator" section to `src/AGENTS.md`. Use Ema as the baseline reference.

- [ ] **DOC-ARCH-5 — Result type convention** (30 min).
  - **Why**: 79 result records use `public record` with `Timestamp` first and `double?` for warmup-period values, but the convention is conventional only (Inspector F8).
  - **Action**: One paragraph in `src/AGENTS.md` codifying the convention. Cite Ema as canonical.

- [ ] **DOC-ARCH-6 — No analyzer suppressions in streaming source** (15 min).
  - **Why**: Inspector F7 asks to verify and codify that simplicity claims are not artificial.
  - **Action**: After T233 cleanup completes, add a one-line statement to `src/AGENTS.md`: "No analyzer suppressions are used in `_common/StreamHub/` or `_common/BufferLists/`."

- [ ] **DOC-ARCH-7 — Prior art comparison** (15 min).
  - **Why**: Researcher F3–F5 surfaced that the library is ahead of ta4j (pull-only), TA-Lib/Tulip (batch-only), and aligned with Pine Script bar-by-bar — useful framing for marketing and for justifying the dual-track architecture to future contributors.
  - **Action**: Brief subsection in the new ADR (DOC-ARCH-1) or in `docs/migration.md`.

### F. v3.0 performance verification (Researcher F7)

- [ ] **PV001 — Verify Slope BufferList uses O(1) four-sum rolling update** (1–2 hours, **may promote/demote P015**).
  - **Why**: Researcher F7 (citing Wikipedia simple linear regression and stats.stackexchange/6920) shows that streaming OLS is provably O(1) per update by maintaining running sums of Σx, Σy, Σxy, Σx². On window slide, subtract the leaving point and add the entering point. This is the lower bound.
  - **Current implementation**: `src/s-z/Slope/Slope.BufferList.cs` uses a pre-computed `sumSqX` constant and mathematical `sumX`, but **still iterates the buffer** (avgY first pass, then deviations second pass).
  - **Action**: Refactor to maintain four running sums; eliminate the per-emission buffer iteration. If the refactor lands cleanly, P015 changes from "research candidate at 3.41x" to "fixed, retest baseline." If it's mathematically equivalent to current implementation due to Line repaint constraints, document why and reclassify firmly.

### G. Documentation gaps (existing)

- [ ] **D009 — Document QuotePart streaming variants** (1–2 hours).
  - **File**: `docs/utilities/quotes/use-alternate-price.md` (or new page under `docs/utilities/quotes/`).
  - **Problem**: `QuotePart` has full StreamHub + BufferList implementations (`QuotePart.StreamHub.cs`, `QuotePart.BufferList.cs`) but the utilities page documents only the Series form. Sole gap making coverage 78/79 instead of 79/79.
  - **Action**: Add BufferList (`.ToQuotePartList(...)`) and StreamHub (`.ToQuotePartHub(...)`) sections following the same pattern as indicator pages.

### H. Critical BufferList performance — reclassified (re-examined this pass)

P015 status now depends on PV001 outcome. P016 and P017 confirmed at algorithmic floors.

- [ ] **P015** — Slope BufferList: outcome depends on PV001.
- [ ] **P016** — Alligator BufferList (research candidate, was 2–4 hours).
  - **Current**: 2.16x slower than Series (18,570 ns vs 8,609 ns).
  - **Code state**: `src/e-k/Alligator/Alligator.BufferList.cs` uses incremental SMMA with median-price queue; overhead is triple-SMMA fanout + median-price computation per quote.
  - **Recommendation**: Merge with P003 in v3.1+. Ship v3.0 at 2.16x.

- [ ] **P017** — Adx BufferList (research candidate, was 3–4 hours).
  - **Current**: 2.08x slower than Series (31,348 ns vs 15,088 ns).
  - **Recommendation**: Merge with v3.1+ research. Ship v3.0 at 2.08x.

### I. Low-priority test items (existing)

- [ ] **T216** — ConnorsRsi `RemoveWarmupPeriods` calculation review (2–3 hours).
- [ ] **T217** — CMO zero price change test (1–2 hours).

### J. Infrastructure — deferred but listed for context

- [ ] **File reorganization** — [#1810](https://github.com/DaveSkender/Stock.Indicators/issues/1810). See [file-reorg.plan.md](file-reorg.plan.md). ~500 file renames, 55–87 hours, deferred to v3.1.

---

## v3.1+ Enhancements — Deferred Work

### Framework architecture improvements (new — Architect findings)

- [ ] **ARCH-V31-1 — Retire `BaseProvider<T>`, introduce `StreamSource<T>`** (4–6 hours).
  - Source: Architect F1, Stercorator F1.
  - Internal refactor; no public API change. Resolves the self-flagged TODO at `src/_common/StreamHub/Providers/BaseProvider.cs`.

- [ ] **ARCH-V31-2 — Replace `lock (Cache)` with private monitor + routed mutation methods** (4 hours).
  - Source: Architect F3.
  - Introduce `_cacheLock = new object()`, route all mutations through `AppendResult`/`ReplaceAt`/`TruncateFrom`. Eliminates public-field-as-monitor anti-pattern.

- [ ] **ARCH-V31-3 — `Snapshot()` method for immutable cache copies** (2 hours).
  - Source: Architect F4.
  - Keep `Results` as the live view (with documented hazards); add `Snapshot()` returning `IReadOnlyList<TOut>` taken under the cache lock.

- [ ] **ARCH-V31-4 — `JoinHub<TLeft,TRight,TOut>` for multi-input streaming** (8–12 hours).
  - Source: Architect F7, Researcher F8.
  - Designed primitive for two-input indicators (Beta, Correlation, Prs). Configurable zip-by-timestamp vs combine-latest semantic. PairsProvider revert (PR #1821) failed because no primitive was designed first.

- [ ] **ARCH-V31-5 — `IObservable<T>` adapter for Rx interop** (3–4 hours).
  - Source: Researcher F1.
  - Add `ToObservable()` extension on each hub. Does not retrofit Rx as the base — the `OnRebuild` semantic is genuinely outside Rx's model. Just an adapter.

- [ ] **ARCH-V31-6 — `IAsyncEnumerable<T>` and `Channels` adapters** (3–4 hours).
  - Source: Researcher F2.
  - `StreamHubExtensions.ConsumeAsync(IAsyncEnumerable<TIn>, CancellationToken)` and `Results.ToAsyncEnumerable()`. Lets hubs interoperate with SignalR / gRPC server-streaming / Kafka consumers without boilerplate.

- [ ] **ARCH-V31-7 — Standardize shared "increment kernels" usage** (8–12 hours).
  - Source: Inspector F1, verified observation.
  - Today `Ema.Increment`, `Sma.Increment`, `Tr.Increment`, `Atr.Increment` are used in ~34 of 160 streaming files (~21%). Migrate remaining StreamHub + BufferList siblings to delegate to shared kernels. Target ≥80% adoption.

- [ ] **ARCH-V31-8 — Shared rolling-window primitives** (6–8 hours).
  - Source: Inspector F2.
  - `RollingMin`, `RollingMax`, `RollingSum`, `RegressionAccumulator` as shared utilities. Today indicators like Chandelier and Slope reinvent these per surface.

### Test coverage v3.1+

- [ ] **TC-V31-1 — Concurrency test suite for StreamHub** (4–6 hours).
  - Source: Tester F4. Production hardening (PR #1927) shipped; tests are the safety net for future regressions. Parallel `Add` from N threads, interleaved late-arrivals, concurrent observer subscribe/dispose.

- [ ] **TC-V31-2 — Tighten `Catalog.Listings.Tests.cs` per-indicator metadata assertions** (3–4 hours).
  - Source: Tester F7. Loose `NotBeEmpty`/`NotBeNull` should become exact `Results.Count`/`Parameters.Count` per indicator.

- [ ] **TC-V31-3 — BufferList `MaxListSize` runtime trimming test** (1–2 hours).
  - Source: Tester F8. Mirror TickHub's `WithCachePruning` pattern for BufferList.

- [ ] **TC-V31-4 — Aggregator hub rollback-equivalence coverage** (2 hours).
  - Source: TC001 follow-up. `QuoteAggregatorHub` and `TickAggregatorHub` override `RollbackState` but are not in the catalog, so TC001 does not exercise them. Decide whether to catalog-register them or add two hand-built rollback-equivalence cases to `StreamHub.RollbackContract.Tests.cs`.

- [ ] **TC-V31-5 — Compound-hub inner↔outer rollback interaction** (3 hours).
  - Source: TC001 follow-up. For compound hubs (StochRsi, Stc, ConnorsRsi, Gator, etc.) `Rebuild` on the outer hub does not exercise the inner hub's `RollbackState`. Inner is still validated via its own listing's TC001 row; the cross-hub interaction is not. Add a targeted test only if a real compound-hub rollback bug surfaces.

### Framework / performance

- [ ] **T205 — `Reinitialize()` optimization** (6–8 hours) **[highest leverage]**.
  - File: `src/_common/StreamHub/StreamHub.cs:230` (TODO in source).

- [ ] **P001** — Moving Average family framework overhead (research). Acceptable for intended use (~40,000 quotes/sec).
- [ ] **P002** — Slope BufferList research (merged with P015 outcome from PV001).
- [ ] **P003** — Alligator/Gator BufferList research (merged with P016).
- [ ] **P006** — Prs streaming support — depends on ARCH-V31-4 `JoinHub` design.

### Series batch processing optimizations

See [Issue #1259](https://github.com/DaveSkender/Stock.Indicators/issues/1259). Series-side; not streaming.

- [ ] **S001** — Rolling SMA optimization for Series (2–5x for SMA-dependent indicators).
- [ ] **S002** — SMA warmup optimization in EMA family (10–30% for EMA, DEMA, TEMA, T3, MACD).
- [ ] **S004** — Span-based window operations (5–15%).
- [ ] **S005** — `RollingWindowMax/Min` array-based optimization (10–20% for Chandelier, Donchian, Stoch).
- ~~S003 — Array allocation~~ (ON HOLD; PR #1838 unmeasurable improvement).

### Streaming feature enhancements

- [ ] **T206** — `StreamHub.OnAdd` array return pattern (4–6 hours). Evaluate batch-emission need.
- [ ] **T208** — `Quote.Date` property removal (2–3 hours). Breaking change, major version.
- [ ] **T210** — Pivots streaming rewrite (6–8 hours). Enhancement.
- [ ] **T214** — MaEnvelopes ALMA/EPMA/HMA support for StreamHub (8–12 hours).
- [ ] **T215** — Hurst Anis-Lloyd corrected R/S implementation (8–12 hours).
- [ ] **T212** — Catalog `NotImplementedException` alternative (2–3 hours).
- [ ] **T220** — `StringOut` index range support (3–4 hours, test utility).
- [ ] **T221** — StreamHub stackoverflow test coverage expansion (ongoing).
- [ ] **T223** — Renko StreamHub alternative testing approach (4–6 hours).
- [ ] **T224** — Performance benchmark external data cache model (6–8 hours).
- [ ] **T225** — Style comparison benchmark representative indicators (2–3 hours).
- [ ] **T226** — `ISeries.UnixDate` property (3–4 hours, interface change).
- [ ] **T227** — `QuotePart.Use` vs `ToQuotePart` deprecation decision (1–2 hours).
- [ ] **T228** — `IQuotePart` rename to `IBarPartHub` evaluation (2–3 hours).

### Cleanup deferred to v3.1+

- [ ] **T235 (was F4 from Stercorator)** — Schedule `Obsolete.V3.Indicators.cs` and `Obsolete.V3.Other.cs` removal with CHANGELOG entry. They use `error: true` shims (compile errors with helpful messages); zero runtime utility, just better error messages during migration. Sunset in v3.1 or v3.2 with documented removal milestone.

- [ ] **G007-followup — Reconcile `tests/AGENTS.md` layout** (30 min).
  - **Evidence**: `tests/AGENTS.md:8–10` lists `indicators/`, `other/`, `performance/`. Actual `ls tests/` shows `indicators/`, `integration/`, `other/`, `performance/`, `public-api/`. Missing `integration/` and `public-api/` from the AGENTS doc.
  - **Action**: Update to reflect real layout; clarify the relationship between `tests/performance/` and `tools/performance/` (both exist; benchmarks vs assertions).

### Infrastructure & code quality

- [ ] **#1533** — Consistent test method naming conventions (~280 test classes).
- [ ] **#1739** — Doc site framework migration: VitePress migration largely complete on v3 (PRs #1981, #1991, #1992). Close once `main` carries the new site.

### Complex research (v3.2+)

- [ ] **T170 / #1692 / E001–E003** — ZigZag streaming. Pivot detection requires bounded lookahead vs append-only — fundamental tension.

- [ ] **RES-V32-1 — Watermark / grace-period for late data** (Researcher F6).
  - Source: Apache Flink and Kafka Streams watermarks. Today the library has two modes only: accept-and-rollback or reject. A `StreamHubSettings.LatenessGrace : int` (bars) would let high-frequency consumers reject arrivals older than `Cache.Last.Timestamp - Grace`. Default behavior unchanged.

- [ ] **RES-V32-2 — `RollbackContext` struct vs bare `int`** (Architect F2 follow-up).
  - After v3.1 doc clarification (DOC-ARCH-2 + ARCH-V31-1/2), evaluate enriching the signature with intent (`Reason: enum { LateArrival, ExplicitRemove, Rebuild }`) so subclasses can distinguish causes.

- [ ] **RES-V32-3 — Pine Script-style "rebuild current open bar only" fast path** (Researcher F5).
  - For same-timestamp updates, only recompute from the open bar's prefix; for older out-of-order, opt-in `AllowLateArrivals` with documented repaint cost.

- [ ] **#1323 / #1259** — Heap allocation reduction (struct Quote, ArrayPool, Span). Coordinated v3.2 push.

- [ ] **Review [Discussion #1018](https://github.com/DaveSkender/Stock.Indicators/discussions/1018)** — community feedback on state rollback.

- [ ] **E004–E006** — QuoteHub intra-period update semantics.
- [ ] **E007–E008** — ADX DMI property expansion.
- [ ] **E009** — BufferList configuration ([Issue #1831](https://github.com/DaveSkender/Stock.Indicators/issues/1831)).
- [ ] **E010** — Composite naming for chained indicators.

---

## Completed in the v3.0 cycle (appendix)

Retained for traceability. All items below are merged and verified as of 2026-05-24.

### Correctness & framework

- **#1585** — QuoteHub self-healing limitation: cache exposures wrapped in `AsReadOnly()` (note: this prevents *mutation* but exposes a live view — see DOC-ARCH-3 above)
- **T200** — TEMA/DEMA StreamHub layered EMA state optimization
- **T201** — Stochastic SMMA re-initialization on NaN (PR #1852)
- **T202** — WilliamsR boundary rounding precision (boundary clamping + tests)
- **T204** — StochRsi `Remove()` auto-healing evaluation (PR #1842)
- **T207** — Removed redundant `RemoveWarmupPeriods` overrides for Epma, Hurst, Mfi, Stoch, Vwap (PR #1842)
- **T209** — PivotPoints `ToList()` removal (PR #1842)
- **T211** — `ListingExecutor` simplified to use `IQuote` interface (PR #1842)
- **T218** — Precision analysis test obsolescence review (clarified value of `BoundaryTests`)
- **T222** — StreamHub cache management exact-value verification (Series parity is canonical)
- **T229** — ATR utilities incremental method made public

### Performance — StreamHub

All items implemented in source; baselines pending refresh (RG001).

- **P004** — ForceIndex StreamHub O(n²) → O(1) incremental update with rolling sum (PR #1860)
- **P005** — Slope StreamHub: cached slope/intercept state (PR #1859); 43% overhead reduction
- **P007** — Roc StreamHub: investigation closed; inherent framework cost
- **P008** — PivotPoints StreamHub: investigation closed; within acceptable range
- **P009** — Gator StreamHub: AlligatorHub SMMA state caching + `RollbackState` (PR #1986)
- **P010** — Ultimate StreamHub: sliding-window queues (PR #1977)
- **P011** — Adl StreamHub: rolling-total state management (PR #1978 centralized signature)
- **P012** — Pmo StreamHub: layered EMA state O(1) updates (PR #1979)
- **P013** — Smi StreamHub: `RollingWindowMax/Min` deques replaced with fixed-size circular arrays
- **P014** — Chandelier StreamHub: refactored to `ChainHub<IQuote, …>` (PR #1988)

### New streaming features

- **Aggregator hubs for quote/tick quantization** (PR #1875) — `QuoteAggregatorHub`, `TickHub`, `Tick.AggregatorHub`. Net-new streaming capability.

### Documentation & site

- **D007** — Migration guide updates: `docs/migration.md` has full streaming section
- **D008** — SmaAnalysis and Tr indicator doc pages created (PR #1989)
- **PRs #1981, #1991, #1992** — Website 3-pillar IA reorganization; VitePress with Vue chart components
- **PRs #1976, #2005** — Streaming plan updates and missing BufferList/StreamHub doc sections for Alligator, AtrStop, Tsi
- **T213** — Performance documentation consolidated into single `tools/performance/PERFORMANCE_ANALYSIS.md`

---

Last updated: 2026-05-25
