# Documentation site reorganization plan

**Status**: Draft

## Objective

Reorganize the documentation site around three clearly distinct pillars that match how developers actually use library documentation:

1. **Get started** — a concise onboarding path from installation to first working call
2. **Documentation** — conceptual and usage guides explaining the *why* and *how* (indicator styles, streaming, chaining, customization)
3. **Reference** — API pages looked up by name (indicators, utilities, performance)

A first-time visitor should immediately understand which path is for them. A returning developer should be able to navigate to a reference page and cross-reference relevant concepts in Documentation without hunting through the site.

---

## Information architecture model

```plaintext
┌─────────────────────────────────────────────────────────┐
│  Home                                                   │
│  ┌─────────────┐  ┌─────────────────┐  ┌─────────────┐  │
│  │ Get started │  │     Guide       │  │  Reference  │  │
│  │ /guide/     │  │  /guide/        │  │ /indicators │  │
│  │ getting-    │  │  ├── index      │  │ /utilities/ │  │
│  │ started     │  │  ├── batch      │  │ /performance│  │
│  │             │  │  ├── buffer     │  │             │  │
│  │  Install    │  │  ├── stream     │  │  84 pages   │  │
│  │  First call │  │  └── custom     │  └─────────────┘  │
│  └─────────────┘  └─────────────────┘                   │
└─────────────────────────────────────────────────────────┘
```

| Pillar | URL prefix | Purpose | Visitor intent |
| ------ | ---------- | ------- | -------------- |
| Get started | `/guide/getting-started` | Install, first call, minimal concepts | "I'm new here" |
| Guide | `/guide/` | Conceptual and usage guides | "I want to understand this" |
| Reference | `/indicators/`, `/utilities/`, `/performance` | Look up by name | "I know what I need" |

**Note**: "Getting started" lives inside the `/guide/` section but is surfaced as its own item in both the top navigation bar and the sidebar so first-time visitors always see it at a glance.

**Cross-referencing rule**: Every Reference page links to relevant Documentation pages for conceptual depth. Documentation pages link to Reference pages as concrete examples.

---

## Current state

### Site structure

```plaintext
/docs
├── index.md              ← home (landing, marketing-focused)
├── guide.md              ← 771-line monolith: quick start + deep reference
├── indicators.md         ← indicator category landing
├── indicators/           ← 84 individual indicator pages
├── features/             ← usage pattern docs — misnamed, not in top nav
│   ├── index.md
│   ├── batch.md
│   ├── buffer.md
│   └── stream.md
├── utilities/            ← utility API reference
├── examples/             ← external links only
├── performance.md        ← buried in "More" dropdown
├── customization.md      ← no clear home in the IA
├── migration.md
├── about.md
├── CONTRIBUTING.md       ← user-facing; intentionally in sidebar
├── AGENTS.md             ← internal; should not be a public page
├── PRINCIPLES.md         ← internal; should not be a public page
└── plans/                ← excluded from site build
```

### Current top navigation

```plaintext
Home | Indicators | Guide | More (Migration, v2 Docs)
```

### Key problems

1. **No 3-pillar orientation on home page** — The home page markets the library. It does not orient first-time visitors toward "where to start" vs. "where to look things up".

2. **`features/` is misnamed and invisible** — Contains conceptual guides for the three API styles. Not in the top navigation. Developers looking for "how streaming works" would never look under "Features".

3. **`guide.md` conflates Get started and Documentation** — 771 lines covering installation (Get started) alongside deep threading patterns, memory management, and concurrency (Documentation). Neither pillar is served well.

4. **Reference has no links back to Documentation** — Indicator pages' `## Streaming` section never links to the streaming usage guide. `## Chaining` never links to the chaining conceptual page.

5. **`customization.md` has no pillar home** — The custom indicator guide fits cleanly in Documentation alongside batch/buffer/stream, but sits at the root with no section association.

6. **Performance is buried in "More"** — A primary reference page for users evaluating batch vs. stream. Should surface with the other Reference content.

7. **Sidebar config repeats eight times** — Nine navigation links copy-pasted identically into eight sidebar contexts. Any change requires eight synchronized edits.

8. **Indicator pages: no standard for "streaming not applicable"** — Beta, Correlation, Prs, ZigZag omit `## Streaming` silently. StdDevChannels uses a non-standard heading.

9. **Renko ATR variant uses non-standard headings** — Ad-hoc H2/H3 headings (`### Parameters for ATR`) do not match the standard page structure used across all other indicator pages.

---

## Target state

### Site structure

```plaintext
/docs
├── index.md              ← home — 3-pillar orientation + quick example
├── guide/                ← Guide pillar (renamed from features/)
│   ├── index.md          ← overview + style comparison table
│   ├── getting-started.md ← Get started: install + first call (~200 lines, moved from guide.md)
│   ├── batch.md          ← series / batch style
│   ├── buffer.md         ← buffer list style
│   ├── stream.md         ← stream hub style
│   └── customization.md  ← custom indicators (moved from root)
├── indicators.md         ← Reference landing (category cards)
├── indicators/           ← Reference: 84 indicator pages
├── utilities/            ← Reference: utility API pages
├── performance.md        ← Reference: benchmarks
├── examples/             ← no change (separate effort)
├── migration.md          ← no change
├── CONTRIBUTING.md       ← stays; linked from sidebar as before
└── about.md              ← no change
```

AGENTS.md, PRINCIPLES.md, and README.md excluded from the VitePress build via `srcExclude` in `config.mts`. They are not user documentation.

### Target top navigation

```plaintext
Home | Getting started | Guide ▾           | Reference ▾  | More ▾
                         ├ Overview          ├ Indicators    ├ Migration
                         ├ Batch (Series)    ├ Utilities     ├ Contributing
                         ├ Buffer lists      └ Performance   ├ About
                         ├ Stream hubs                       └ v2 Docs ↗
                         └ Custom indicators
```

"Getting started" → `/guide/getting-started` (top-level link, never in a dropdown)
"Guide" → dropdown expanding the `/guide/` section pages

---

## Implementation phases

Phases are ordered by execution dependency. Complete each phase before starting the next unless marked independent.

---

### Phase 1: Consolidate sidebar configuration

**Goal**: Extract the repeated documentation navigation section into a single shared constant so it only needs to be maintained in one place.

**Current pattern**: The same nine-link list is copy-pasted identically into eight sidebar context blocks in `config.mts`.

**Target pattern** (inside `defineConfig` in `docs/.vitepress/config.mts`):

```typescript
export default defineConfig({
  // ...
  themeConfig: {
    // Define once at top of themeConfig
    // (TypeScript const defined before the defineConfig call, or inline)
    sidebar: (() => {
      const siteNav = {
        text: 'Documentation',
        items: [
          { text: 'Getting started', link: '/guide/getting-started' },
          { text: 'Guide', link: '/guide/' },
          { text: 'Indicators', link: '/indicators' },
          { text: 'Utilities', link: '/utilities/' },
          { text: 'Performance', link: '/performance' },
          { text: 'Migration (v2→v3)', link: '/migration' },
          { text: 'Contributing', link: '/contributing' },
          { text: 'About', link: '/about' },
        ]
      }
      return {
        '/guide':      [siteNav, { text: 'Guide', items: [...] }],
        '/utilities':  [siteNav, { text: 'Utilities', items: [...] }],
        '/indicators': [siteNav, { text: 'Indicators', items: [...] }],
        // etc. — all sidebar contexts reference the same siteNav object
      }
    })(),
  }
})
```

**Files touched:** `docs/.vitepress/config.mts` only

---

### Phase 2: Rename features/ to guide/ and restructure top nav

**Goal**: The Guide pillar has a URL and navigation label that matches its content, appears in the top navigation, and the Get started page moves inside it with its own top-nav entry.

**Tasks:**

- Rename `docs/features/` → `docs/guide/`
- Move `docs/guide.md` → `docs/guide/getting-started.md`
- Move `docs/customization.md` → `docs/guide/customization.md`
- Add URL rewrites in `config.mts` to preserve old paths as permanent redirects:
  - `/features/:path*` → `/guide/:path*`
  - `/guide` → `/guide/getting-started`  ← old get-started URL
  - `/customization` → `/guide/customization`
- Update the top navigation to match the target nav defined in Target state above:
  - Add "Getting started" → `/guide/getting-started` as a top-level nav link
  - Add "Guide" → dropdown with the `/guide/` section pages
- Update sidebar: single context key `/guide` covers both getting-started and guide pages
- Add `getting-started` and `customization` to the `/guide/` sidebar entries
- Grep for all internal links referencing `/features/`, `/customization`, or `/guide` (the old get-started URL) and update them

**Files touched:**

- `docs/.vitepress/config.mts`
- `docs/guide/*.md` (directory rename + moved files)
- Any indicator, utility, or other pages containing `/features/`, `/customization`, or `link: '/guide'` references

---

### Phase 3: Slim guide/getting-started.md to a true Get started page

**Goal**: A developer can install the library and get a working first result by following `guide/getting-started.md` alone, in under five minutes. Deeper conceptual content belongs in the Guide (`/guide/`).

Before removing any section, verify the corresponding `/guide/` page already covers it fully. Port any gaps first.

**Content disposition:**

| Current section | Action |
| --------------- | ------ |
| Installation and setup | Keep |
| Prerequisite data | Keep (condensed) |
| Implementation pattern | Keep — one batch example |
| Indicator styles comparison | Remove → `/guide/` index |
| Example usage (all 3 styles) | Remove → `/guide/batch`, `/guide/buffer`, `/guide/stream` |
| Historical quotes | Keep — foundational concept for the API |
| Chaining | Keep (condensed, 1 example) with link to `/guide/batch#chaining` |
| Candlestick patterns | Keep (brief) |
| Incremental buffer style (deep dive) | Remove → `/guide/buffer` |
| Streaming hub style (deep dive) | Remove → `/guide/stream` |
| Thread safety and concurrency | Remove → `/guide/stream` (verify coverage first) |
| Utilities overview | Keep (brief, link to `/utilities/`) |

**Tasks:**

- Remove the sections marked Remove above
- After each condensed section that links to a deeper page, add a callout: `For a deeper guide, see [Section name](/guide/...)`.

---

### Phase 4: Establish the 3-pillar home page

**Goal**: A visitor landing on the home page immediately sees three clear paths and understands which one is for them.

**Current home**: Hero with library value proposition → feature cards (indicator categories) → code samples → project badges.

**Target home**: Retain the value proposition hero and code samples. Replace the indicator category feature cards with three orientation cards pointing to the three pillars. Move the indicator category cards (Moving averages, Oscillators, etc.) to `indicators.md`, the Reference landing page.

Proposed orientation card layout:

```plaintext
┌──────────────────────┐  ┌──────────────────────┐  ┌──────────────────────┐
│  New here?           │  │  Learning the API?   │  │  Looking something   │
│                      │  │                      │  │  up?                 │
│  Install and run     │  │  Batch, buffer,      │  │                      │
│  your first          │  │  stream, chaining,   │  │  84 indicators with  │
│  indicator in        │  │  and custom          │  │  parameters, result  │
│  minutes.            │  │  indicators.         │  │  types, and examples │
│                      │  │                      │  │                      │
│  → Get started       │  │  → Documentation     │  │  → Indicators        │
└──────────────────────┘  └──────────────────────┘  └──────────────────────┘
```

VitePress home pages support `features:` arrays in frontmatter. Use three feature entries with `link:` pointing to `/guide/getting-started`, `/guide/`, and `/indicators` respectively.

**Files touched:**

- `docs/index.md`
- `docs/indicators.md` (add the category cards removed from home)

---

### Phase 5: Add cross-references from Reference to Documentation

**Goal**: A developer reading an indicator page can reach the relevant conceptual guide in one click.

**Standard cross-references to add at the end of each section:**

| Section | Line to append |
| ------- | -------------- |
| `## Chaining` | `See [Chaining indicators](/guide/batch#chaining) for more.` |
| `## Streaming` (full examples) | `See [Buffer lists](/guide/buffer) and [Stream hubs](/guide/stream) for full usage guides.` |
| `## Streaming` (not applicable) | Link to the most relevant guide page explaining the model |

The `### Historical quotes requirements` subsection already cross-references `/guide#historical-quotes` — update to `/guide/getting-started#historical-quotes` when Phase 2 runs.

The documentation skill template already includes these cross-references using `/features/` URLs (the current path). When this phase runs, update the skill template links to use `/guide/` (post Phase 2 rename) and do a sweep of existing indicator pages to add the links.

---

### Phase 6: Standardize indicator pages — streaming not applicable

**Goal**: Every indicator page has `## Streaming` — either full examples or a clear explanation of why streaming is not supported.

**Affected pages:**

| Page | Current state | Action |
| ---- | ------------- | ------ |
| `Beta.md` | Section missing | Add `## Streaming` with "not applicable" note |
| `Correlation.md` | Section missing | Add `## Streaming` with "not applicable" note |
| `Prs.md` | Section missing | Add `## Streaming` with "not applicable" note |
| `ZigZag.md` | Section missing | Add `## Streaming` with "not applicable" note |
| `StdDevChannels.md` | Heading is `## Streaming and real-time usage` | Rename heading to `## Streaming` only |
| `Renko.md` | `### Streaming limitations for ATR` (subsection) | Handled by Phase 7 |

**Standard "not applicable" wording:**

```markdown
## Streaming

Streaming is not supported for this indicator.
{One sentence stating the architectural reason.}
Use the Series (batch) implementation with periodic recalculation instead.
```

**Reason by indicator:**

- **Beta, Correlation, Prs**: "This indicator requires a second synchronized quote series, which cannot be expressed in the single-series streaming model."
- **ZigZag**: "This indicator requires lookahead to confirm reversal points; output repaints as new data arrives, making incremental results undefined."

> StdDevChannels already has good explanatory prose under its existing section — only the heading needs to change.

---

### Phase 7: Standardize Renko multi-overload structure

**Goal**: Renko documents two distinct overloads using ad-hoc H2/H3 headings that break the standard page structure. Restructure it to match the multi-variant pattern documented in the documentation skill.

**Target structure for `Renko.md`:**

````markdown
# Renko chart

description...

```csharp
// C# usage syntax (fixed brick size)
IReadOnlyList<RenkoResult> results =
  quotes.ToRenko(brickSize, endType);

// C# usage syntax (ATR-derived brick size — Series only)
IReadOnlyList<RenkoResult> results =
  quotes.ToRenkoAtr(atrPeriods, endType);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `endType` | EndType | Applies to both variants. ... |

### Fixed brick size

| param | type | description |
| ----- | ---- | ----------- |
| `brickSize` | decimal | ... |

### ATR-derived brick size

| param | type | description |
| ----- | ---- | ----------- |
| `atrPeriods` | int | ... |

### Historical quotes requirements

**Fixed brick size**: You must have at least 2 periods of `quotes`.

**ATR-derived brick size**: You must have at least `A+100` periods of `quotes`.

`quotes` is a collection of generic `TQuote`...

## Response

[shared — both variants return RenkoResult; existing table unchanged]

## Chaining

[existing chaining content]

## Streaming

[existing buffer + hub examples for fixed-brick variant]

::: warning
`ToRenkoAtr()` does not support streaming.
The ATR brick size is derived from the full dataset and changes as new quotes are added, making incremental output undefined.
Use the Series implementation with periodic recalculation instead.
:::
````

Note: the two variants have different historical quotes requirements and must be stated separately under the shared `### Historical quotes requirements` subsection.

---

### Phase 8: Exclude internal files from site build

**Goal**: AGENTS.md, PRINCIPLES.md, and README.md are not user documentation and should not be built as public pages.

CONTRIBUTING.md is intentionally kept — it is linked from the sidebar under "More" and is user-facing.

**Task**: Add the following to `config.mts` inside `defineConfig`:

```typescript
srcExclude: ['**/AGENTS.md', '**/PRINCIPLES.md', '**/README.md'],
```

**Files touched:** `docs/.vitepress/config.mts` only

---

### Phase 9: Update documentation skill

**Goal**: The `.agents/skills/documentation/SKILL.md` reflects all structural decisions in this plan.

**Already completed** (done during initial skill authoring session):

- Streaming not applicable pattern and standard wording
- Multi-variant overload pattern (Renko-style)
- Dual `<IndicatorChartPanel>` convention for distinct behavioral modes
- Cross-reference links at end of Chaining and Streaming sections (currently using `/features/` URLs)

**Remaining tasks** (depend on prior phases completing):

- After Phase 2: Update site structure table (`features/` → `guide/`, `guide.md` → `guide/getting-started.md`, `customization.md` → `guide/customization.md`) and update all `/features/` cross-reference links in the skill to `/guide/`; update the `Historical quotes requirements` cross-reference from `/guide#historical-quotes` to `/guide/getting-started#historical-quotes`
- After Phase 8: Add note that AGENTS.md, PRINCIPLES.md, README.md are excluded from build and should not be linked from indicator pages

---

## Execution sequence summary

Phases must run in the order listed (1 → 9). Phases 6, 7, and 8 are independent of each other and of Phases 3–5; they may be done in any order after Phase 2 completes.

```plaintext
Phase 1 → Phase 2 → Phase 3 → Phase 4 → Phase 5 → Phase 9
                ↘
                 Phase 6 ┐
                 Phase 7 ├→ Phase 9
                 Phase 8 ┘
```

---

## Out of scope

- `examples/` section content (requires content creation; separate effort)
- VitePress theme or styling changes
- Indicator sidebar category groupings (intentional cross-categorization; do not change)
- HtTrendline dual chart panels (intentional — shows two distinct outputs: trendline and dominant cycle period)
- StdDevChannels dual chart panels (intentional — illustrates the `null` lookback behavioral mode)
- Adding new indicator pages

---

## Success criteria

- [ ] Home page has three orientation cards pointing to Getting started, Guide, and Indicators
- [ ] Top navigation has "Getting started" as a top-level link and "Guide" as a dropdown; both top nav and sidebar surface Getting started prominently
- [ ] `/features/*`, `/customization`, and `/guide` (old) redirect cleanly to `/guide/*`; no broken internal links
- [ ] `guide/getting-started.md` is under 250 lines and reads as a standalone quick-start
- [ ] All indicator pages have `## Streaming` (full examples or standard "not applicable" note)
- [ ] Indicator `## Streaming` and `## Chaining` sections end with a cross-reference link to the relevant Guide page (`/guide/batch`, `/guide/buffer`, `/guide/stream`)
- [ ] Sidebar config has a single shared navigation constant — zero copy-paste repetition
- [ ] AGENTS.md, PRINCIPLES.md, README.md are not accessible as site pages
- [ ] CONTRIBUTING.md remains accessible and linked from the sidebar
- [ ] Site builds without errors: `pnpm run docs:dev` from `docs/`
- [ ] All internal links resolve: `bash .vitepress/test-links.sh`
- [ ] Accessibility tests pass: `bash .vitepress/test-a11y.sh`
