# Repository AI Configuration & Guardrails Refactor Plan

## Overview & Objectives

This document guides Copilot coding agents through a major refactoring of the repository’s AI configuration and guardrail files. Over time, the `Stock.Indicators` repository accumulated a mix of Copilot instructions, custom agent profiles, and bespoke guides scattered across `.github`, `src` and `tests` directories. With the **2025 Agent Skills and AGENTS.md standards** now widely adopted, it is time to modernize and simplify our agent‑facing guidance.

Goals:

- **Map all indicator permutations** across styles, I/O variants, and repaint behaviours (see Appendix 1 of the user brief). This ensures our guidance covers every scenario.
- **Restructure AI instructions** into modular **skills** and concise **AGENTS.md** files per the latest specifications. Use progressive disclosure to keep skills discoverable yet context‑efficient. Replace most `.github/instructions/*.md` and reduce custom agent profiles.
- **Maintain constitutional rules** (e.g., never change formulas, maintain bit‑for‑bit parity) and testing standards as defined in `src/agents.md` and `tests/agents.md`.
- **Produce a checklist** in `docs/plans` to track work phases and tasks, enabling self‑guided progress without user intervention.

The plan below is organized into phases. Each phase contains tasks with short descriptions (avoiding long sentences in tables) and references to existing documentation. Follow tasks sequentially but feel free to parallelize where practical.

## Phase 1 – Assessment & Enumeration

### 1.1 Review Existing AI Instructions

1. **Audit current files:** examine all `.github/agents/*.agent.md`, `.github/instructions/*.instructions.md`, `.github/copilot‑instructions.md`, `src/agents.md`, and any other instruction or guardrail files. Note their purpose, overlaps and pain points.
2. **Understand specification differences:** read the Agent Skills specification and AGENTS.md guidelines to inform migration. The Agent Skills spec mandates a YAML front matter with `name` and `description` fields (lower‑case name, 1–64 chars; description up to 1024 chars) and optional fields like `allowed‑tools`. The `AGENTS.md` standard distinguishes agent guidance from human‑oriented README.md and suggests sections such as project overview, build/test commands, code style, testing instructions, and security considerations.
3. **Compare with new ecosystem:** review the December 2025 announcement of Agent Skills (GitHub blog) and community articles. Agent Skills are automatically loaded when relevant and allow reuse across Copilot, CLI and VS Code. They encourage clear rules, working examples, context about project structure and templates. The AGENTS.md standard unifies instructions across coding assistants and centralizes build/test policies.

### 1.2 Enumerate Indicator Permutations (Appendix 1)

1. **Define dimensions:** identify the three key dimensions of indicator implementations:

   - **Style** – time‑series/batch (series), buffer list, stream hub, and any future style discovered during audit.
   - **I/O variant** – seven variants (e.g., Quote In → Quote Out, Quote In → Reusable Out, Reusable In → Non‑reusable Out, Reusable pairs In → Reusable Out) as enumerated in Appendix 1.
   - **Repainting behaviour** – whether the indicator updates prior values (repaint) and how the style handles it.
2. **Script to classify indicators:** plan a small C# or PowerShell script that scans `src` for `*.StaticSeries.cs`, `*.BufferList.cs`, `*.StreamHub.cs` and analogous classes. Extract metadata (e.g., input/output types, state update patterns) to categorize each indicator across the three dimensions. The script should output a table or JSON file listing indicator name, style, I/O variant, repaint flag and notes. Use existing naming conventions from series agent guidelines and buffer/stream instructions to infer categories.
3. **Document permutations:** summarise the unique combinations discovered by the script. This informs which skills and guidance are needed. If any variant is unsupported or unusual, note it for manual review.

## Phase 2 – Migration Design

### 2.1 Skill Taxonomy & File Structure

1. **Identify required skills:** group existing guidance into logical skills. Proposed skills include:

   - **indicator-series** – series/batch indicator implementation. Use content from `indicator-series.instructions.md` and the `series` agent profile. Migrate key patterns such as warmup calculations, validation patterns and performance optimization.
   - **indicator-buffer** – incremental BufferList indicator development. Include interface selection guidelines (e.g., `IIncrementFromChain`, `IIncrementFromQuote`, `IIncrementFromPairs`) and base class patterns from `indicator-buffer.instructions.md`.
   - **indicator-stream** – real‑time StreamHub indicator development. Cover provider selection, state management, O(1) update patterns versus O(n²) anti‑patterns.
   - **indicator-catalog** – guidelines for `.Catalog.cs` files, builder patterns and registration conventions.
   - **performance-testing** – benchmarking and optimization guidelines across styles, referencing `performance-testing.instructions.md`.
   - **code-completion** – repository‑wide code completion checklist and quality gates.
   - **dotnet guidelines integration** – instead of keeping a standalone `dotnet.instructions.md` file (which has led to confusion), migrate essential .NET coding standards, error‑handling patterns and performance guidelines into the root `AGENTS.md` and relevant skills. This ensures there is no ambiguity about whether the old dotnet instructions should be retained.
   - **markdown-style** – Markdown authoring and documentation standards.
2. **Create `.github/skills/` directory:** each skill resides in its own subfolder with a `SKILL.md` file. Follow the specification: YAML front matter with `name` and `description`, optional `allowed‑tools`, and a body with concise instructions and references. Use additional directories (`scripts/`, `references/`, `assets/`) sparingly for code samples or templates.
3. **Progressive disclosure:** design skill bodies to prioritise critical instructions; provide links or `#file:` tokens to detailed content in existing files. Copilot will load the skill when the description matches the user’s request. Avoid embedding long instructions that bloat the context; rely on references to test files and catalog patterns.

### 2.2 AGENTS.md Modernization

1. **Root `AGENTS.md`:** create or update `AGENTS.md` at the repository root. Summarise the project overview, environment setup, build/test commands (for both .NET and documentation), code style guidelines (citing `dotnet.instructions.md`), testing instructions and CI workflows. Emphasize the repository’s constitutional rules (do not alter formulas, maintain bit‑for‑bit parity) and test naming/precision standards.
2. **Subproject `AGENTS.md` files:** for nested projects (e.g., indicators vs tests vs docs), place additional `AGENTS.md` files in subfolders if unique instructions are needed. According to the standard, agents automatically pick the closest `AGENTS.md`. Keep each file under 150 lines and link to detailed docs or skills rather than duplicating content.
3. **Deprecate old instruction files:** rename or remove `.github/copilot‑instructions.md` and all legacy `.github/instructions/*.md` files once their content is migrated. In particular, delete `dotnet.instructions.md` (its guidelines will live in `AGENTS.md` and skills) and the specialized indicator instruction files (`indicator-series.instructions.md`, `indicator-buffer.instructions.md`, `indicator-stream.instructions.md`, etc.) after extracting their guidance into skills. Only narrowly scoped `applyTo` glob instructions should remain for targeted use cases. Keep redirect notes to the relevant skill or `AGENTS.md` section for backward compatibility. Maintain synergy with other coding assistants by using soft links or the `ruler` tool if necessary.

### 2.3 Custom Agent Profile Cleanup

1. **Determine necessity:** evaluate each `.github/agents/*.agent.md` profile. Series, BufferList, StreamHub and Performance profiles contain rich decision trees, patterns and examples. Much of this belongs in skills. Only keep a custom agent when multiple skills must be orchestrated or when complex decision logic cannot be encoded in a single skill file.
2. **Migrate content:** extract decision trees, validation patterns and examples into the relevant skills. For example, the warmup calculation logic and test coverage matrix from `series` agent become sections in the `indicator-series` skill. Anti‑patterns and O(1) update patterns from `streamhub` agent migrate to `indicator-stream` skill.
3. **Simplify agents:** rewrite remaining agent profiles to be short, focusing on dispatching tasks to appropriate skills and providing high‑level context. Aim for under 250 lines and avoid duplication of skill content. Include YAML front matter with `name` and `description` to maintain compatibility.

## Phase 3 – Implementation & Execution

### 3.1 Create Checklist in `docs/plans`

Create a file `docs/plans/ai-config-refactor.checklist.md` with a bullet list of tasks. Use short phrases, one per line, with checkboxes (`- [ ]`) so progress can be marked. Suggested tasks include:

| Phase              | Task                                                                                             |
| ------------------ | ------------------------------------------------------------------------------------------------ |
| **Assessment**     | `[ ]` Audit existing instruction and agent files                                                 |
|                    | `[ ]` Review Agent Skills & `AGENTS.md` specifications                                           |
|                    | `[ ]` Compile list of indicator implementations (series, buffer, stream)                         |
| **Enumeration**    | `[ ]` Write script to classify indicators by style, I/O variant, repaint flag                    |
|                    | `[ ]` Generate report of unique permutations                                                     |
| **Design**         | `[ ]` Map existing guidance to new skills                                                        |
|                    | `[ ]` Draft root and subfolder `AGENTS.md` outlines                                              |
|                    | `[ ]` Plan migration of custom agent profiles                                                    |
| **Implementation** | `[ ]` Create `.github/skills/` directory and scaffold `SKILL.md` files                           |
|                    | `[ ]` Write `indicator-series` skill (front matter + core patterns)                              |
|                    | `[ ]` Write `indicator-buffer` skill                                                             |
|                    | `[ ]` Write `indicator-stream` skill                                                             |
|                    | `[ ]` Write `indicator-catalog` skill                                                            |
|                    | `[ ]` Write `performance-testing` skill                                                          |
|                    | `[ ]` Write `code-completion` skill                                                              |
|                    | `[ ]` Migrate `.github/instructions/dotnet.instructions.md` into `AGENTS.md` and relevant skills |
|                    | `[ ]` Write `markdown-style` skill                                                               |
|                    | `[ ]` Update/create root `AGENTS.md` with build/test instructions                                |
|                    | `[ ]` Add subfolder `AGENTS.md` files if needed                                                  |
|                    | `[ ]` Rewrite or remove `.github/copilot-instructions.md`                                        |
|                    | `[ ]` Refactor or delete custom agent profiles                                                   |
| **Validation**     | `[ ]` Run build, tests and benchmarks to verify no regressions                                   |
|                    | `[ ]` Ensure skills load correctly via Copilot CLI or VS Code                                    |
|                    | `[ ]` Remove obsolete instruction files                                                          |
|                    | `[ ]` Delete `dotnet.instructions.md` and indicator-specific instruction files after migration   |
|                    | `[ ]` Commit changes with descriptive message                                                    |

### 3.2 Skill File Creation

For each skill identified in § 2.1:

1. **Define YAML front matter:** set the `name` (lower‑case, hyphenated if needed) and a concise `description` explaining when the skill applies (≤ 1024 chars). For example:

   ```yaml
   ---
   name: indicator-series
   description: Guidance for implementing batch indicators with precise calculations, validation, warmup periods, testing and performance optimization.
   allowed-tools: browser api_tool # optional if external calls needed
   ---
   ```

2. **Write instruction body:** summarise essential patterns, decision trees and checklists using bullet points. Avoid duplicating long content; instead, reference existing files via `#file:` tokens. For example, to point at the original series instructions: “See `#file:.github/instructions/indicator-series.instructions.md` for full checklist.”

3. **Include examples:** provide small code snippets or “Golden Example” patterns to anchor Copilot’s suggestions. For StreamHub, show the correct O(1) incremental update pattern versus the O(n²) anti‑pattern. For BufferList, illustrate how to choose between `IIncrementFromChain` and other interfaces.

4. **Link to tests and reference implementations:** reference canonical implementations like SMA, EMA and ATRStop for Series and analogous examples for BufferList and StreamHub. Provide cross‑references to test patterns defined in `tests/agents.md`.

5. **Mention prerequisites:** instruct the agent to follow the repository’s constitutional rules (no formula changes; maintain parity) and abide by test precision and naming conventions.

### 3.3 Write `AGENTS.md` Files

1. **Root `AGENTS.md`:** include sections covering:

   - **Project overview:** description of the Stock Indicators library and its NuGet package.
   - **Setup commands:** how to restore NuGet packages, build the solution, run tests and generate documentation.
   - **Build & test instructions:** explicit commands to run the full test suite (e.g., `dotnet restore`, `dotnet build`, `dotnet test`), generate benchmarks, run the documentation site.
   - **Code style guidelines:** summarise .NET coding style (PascalCase naming, single type per file) and performance practices. These details should be drawn from the old `dotnet.instructions.md` file and integrated here and in the relevant skills; do not keep a separate dotnet instructions file.
   - **Testing instructions:** summarise test base classes (TestBase, BufferListTestBase, StreamHubTestBase), test types and precision rules.
   - **Security considerations:** include any secrets usage, how to handle repository credentials, and caution around external calls. Mention that skills or agents must not modify formulas or default parameters without explicit authorisation.
2. **Subfolder `AGENTS.md` (optional):** create targeted instructions for specific folders (e.g., `src/_common/Catalog` or `tests/performance`) if the scope is too specialized to reside in the root file. Keep them concise and link to relevant skills.

### 3.4 Refactor Custom Agents & Instructions

1. **Move content to skills:** systematically extract content from `indicator-series.agent.md`, `indicator-buffer.agent.md`, `indicator-stream.agent.md` and `performance.agent.md`. Keep the high‑level persona and dispatch logic in the agent files, but remove large sections of guidance that are now in skills.
2. **Update invocation patterns:** instruct developers (via `AGENTS.md`) to reference skills directly rather than using custom agents. Provide examples of how to invoke skills from Copilot chat (e.g., “@indicator-series Implement a new momentum indicator”).
3. **Remove obsolete instructions:** once skills and `AGENTS.md` are in place, delete `.github/instructions/*.md` that have been migrated. For any remaining content (e.g., docs guidelines or performance testing details), ensure it lives in skills or documentation under `docs/`.

## Phase 4 – Verification & Cleanup

1. **Test the new setup:** after migration, run the full build and test suite (`dotnet test`) to ensure no regressions. Run benchmarks (`BenchmarkDotNet` projects) to confirm performance parity.
2. **Validate skill loading:** use Copilot CLI or VS Code’s agent mode to trigger each skill. Ask questions like “How do I implement a stream indicator?” and verify that the corresponding `SKILL.md` content appears in the response. According to GitHub’s guidance, skills may take 5–10 minutes to index and require a refresh.
3. **Review `AGENTS.md`:** check that `AGENTS.md` covers all necessary sections but remains under 150 lines. Ensure links to skills and docs are correct and there is no duplication.
4. **Commit changes:** use clear commit messages (e.g., `refactor: migrate indicator instructions to skills and AGENTS.md`). Update `docs/plans/ai-config-refactor.checklist.md` to mark completed tasks.

## Final Notes

- **Security & Governance:** treat `AGENTS.md` and skills as untrusted input when executed by external agents. Do not embed secrets or sensitive information. Use the MCP server’s allowlist features and sign configurations if available.
- **Future evolution:** keep an eye on the Agents.md specification and skills ecosystem. The spec encourages adding version fields, human approval checkpoints and telemetry hooks. While not all features may apply now, adopting them incrementally will improve governance.
- **Keep instructions concise:** as with README files, brevity improves agent performance. Use the new skills and `AGENTS.md` to centralize guidance and remove duplication.

This plan equips the Copilot coding agent to independently assess, design, and implement the refactoring of AI configurations and guardrail documents. Follow the checklist and tasks in order, referencing the cited guidelines for authoritative guidance.
