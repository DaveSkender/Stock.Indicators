---
title: Contributing guidelines
description: >-
  This NuGet package is an open-source project.
  Learn how to contribute issues, fixes, new indicators, new features, or to our discussions.
permalink: /contributing/
relative_path: pages/contributing.md
layout: page
---

# Contributing guidelines

[![Codacy quality grade](https://app.codacy.com/project/badge/Grade/012497adc00847eca9ee91a58d00cc4f)](https://app.codacy.com/gh/DaveSkender/Stock.Indicators/dashboard)
[![Codacy code coverage](https://app.codacy.com/project/badge/Coverage/012497adc00847eca9ee91a58d00cc4f)](https://app.codacy.com/gh/DaveSkender/Stock.Indicators/dashboard)

**Thanks for taking the time to contribute!**

This project is simpler than most, so it's a good place to start contributing to the open source community, even if you're a newbie.

We are accepting these sorts of changes and requests:

- Bug reports and fixes
- Usability improvements
- Documentation updates
- New reputable "by the book" indicators and overlays

We are not accepting things that should be done in your own wrapper code:

- Personal customizations and preferences
- Modified or augmented outputs that are not intrinsic

If you have general interest in contributing, but are not sure where to start, please [contact us](#contact-info) and we can help to find work in an area of interest.

## Reporting bugs and feature requests

If you suspect a problem, please [report a bug Issue](https://github.com/DaveSkender/Stock.Indicators/issues/new?labels=bug&template=bug_report.md) with a detailed description of the problem, steps to reproduce, code samples, and any reference materials.  For enhancements, [create a feature Issue](https://github.com/DaveSkender/Stock.Indicators/issues/new?labels=enhancement&template=feature_request.md).

Use the [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions) area for general ideation and help/usage questions.

## Project management

- Planned work is managed in [the backlog](https://github.com/users/DaveSkender/projects/1).
- Work items are primarily entered as Notes (not Issues), except where an issue or feature is user reported.  With that said, Notes can be converted to Issues if in-progress and collaborative discussion is needed.

## Developing

- Read this first: [A Step by Step Guide to Making Your First GitHub Contribution](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940).  I also have a discussion [on Forking](https://github.com/DaveSkender/Stock.Indicators/discussions/503) if you have questions.
- If you want to work on something specific, please mention your intention on the related [Issue](https://github.com/DaveSkender/Stock.Indicators/issues).  If an Issue does not exist for your contribution, please create one before starting.  This will help us reserve that feature and avoid duplicative efforts.
- If you are adding a new indicator, the easiest way to do this is to copy the folder of an existing indicator and rename everything using the same naming conventions and taxonomy.  All new indicators should include [tests](#testing).
- Do not commingle multiple contributions on different topics.  Please keep changes small and separate.

## Testing

- Review the `tests/indicators` folder for examples of unit tests.  Just copy one of these.
- New indicators should be tested against manually calculated, proven, accurate results.  It is helpful to include your manual calculations spreadsheet in the appropriate indicator test folder when [submitting changes](#submitting-changes).
- Historical Stock Quotes are automatically added to unit test methods.  A `Data.Quotes.xlsx` Excel file is included in the `tests/_common` folder that is an exact copy of what is used in the unit tests.  Use a copy of this file for your manual calculations to ensure that it is correct.  Do not commit changes to the original file.
- We expect all unit tests to execute successfully and all Errors and Warning resolved before you submit your code.
- Failed builds or unit testing will block acceptance of your Pull Request when submitting changes.

### Performance benchmarking

Running the performance benchmark application in `Release` mode will produce [benchmark performance data](https://dotnet.stockindicators.dev/performance/) that we include on our documentation site.

```bash
# from /tests/performance folder
# run all performance benchmarks
dotnet run -c Release

# run individual performance benchmark
dotnet run -c Release --filter *.GetAdx
```

## Documentation

This site uses [Jekyll](https://jekyllrb.com) construction with _Front Matter_.
Our documentation site code is in the `docs` folder.
Build the site locally to test that it works properly.
See [Ruby Jekyll documentation](https://jekyllrb.com/docs) for initial setup.

```bash
# from /docs folder
bundle install
bundle exec jekyll serve --livereload
```

The site will be available at `http://127.0.0.1:4000`.

When adding or updating indicators:

- Add or update the `/docs/_indicators/` documentation files.
- Page image assets go here: `/docs/assets/` and can be optimized to `webp` format using [ImageMagick](https://imagemagick.org) or the [cwebp Encoder CLI](https://developers.google.com/speed/webp/docs/cwebp) and a command like `cwebp -resize 832 0 -q 100 examples.png -o examples-832.webp`

### Accessibility testing

- use Lighthouse in Chrome, or
- build the site locally (see above), then:

```bash
# from /docs folder
npx pa11y-ci \
  --yes
  --sitemap http://127.0.0.1:4000/sitemap.xml \
  --config ./.pa11yci
```

## Submitting changes

By submitting changes to this repo you are also acknowledging and agree to the terms in both the [Developer Certificate of Origin (DCO) 1.1](https://developercertificate.org) and the [Apache 2.0 license](https://opensource.org/licenses/Apache-2.0).  These are standard open-source terms and conditions.

When ready, submit a [Pull Request](https://help.github.com/pull-requests) with a clear description of what you've done and why it's important.

### Pull Request naming convention

Pull Request titles must follow the [Conventional Commits](https://www.conventionalcommits.org) format: `type: Subject` where:

- `type` is one of: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert (lowercase)
- `Subject` starts with an uppercase letter

Examples: `feat: Add RSI indicator`, `fix: Resolve calculation error in MACD`, `docs: Update API documentation`

Always write a clear log message for your commits. One-line messages are fine for most changes.

After a Pull Request is reviewed, accepted, and [squash] merged to the default branch, we may batch changes before publishing a new package version to the [public NuGet repository](https://www.nuget.org/packages/Skender.Stock.Indicators).  Please be patient with turnaround time.

## Code reviews and administration

If you want to contribute administratively, do code reviews, or provide general user support, we're also currently seeking a few core people to help.  Please [contact us](#contact-info) if interested.

## Standards and design guidelines

- [Guiding principles for this project](https://github.com/DaveSkender/Stock.Indicators/discussions/648)
- [.NET Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines)
- [NuGet Best Practices](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/nuget)
- [Semantic Version 2.0](https://semver.org)

## GitHub Copilot and AI development

This repository is optimized for GitHub Copilot and coding agents with:

- **Custom agent instructions** in `AGENTS.md` files (root and subdirectories) providing repository context, coding patterns, and domain knowledge
- **Scoped instruction files** in `.github/instructions/` for targeted guidance by file type and folder
- **Enhanced VS Code settings** in `.vscode/settings.json` with Copilot-specific configurations for optimal suggestions
- **Development container** in `.devcontainer/devcontainer.json` for consistent development environment setup
- **MCP server configurations** in `.vscode/mcp.json` for extended AI tools for developing capabilities with financial mathematics and .NET performance analysis

When using GitHub Copilot:

- Follow the established patterns documented in the AGENTS.md files and instruction files
- Ensure all financial calculations maintain decimal precision
- Include comprehensive unit tests for any new indicators
- Validate mathematical accuracy against reference implementations

## Versioning

We use [GitVersion](https://gitversion.net) for automated [semantic versioning](https://semver.org). Version numbers are automatically generated based on branch names, commit messages, and Git history.

<!-- markdownlint-disable MD060 -->
| Component | Format    | Description |
| --------- | --------- | ----------- |
| Major     | `x.-.-`   | Breaking changes. Use `+semver: major` in commit message. |
| Minor     | `-.x.-`   | New features, backward compatible. Use `+semver: minor` in commit message. |
| Patch     | `-.-.x`   | Bug fixes, documentation. Use `+semver: patch` (or default auto-increment). |
| Suffix    | `-label.N` | Pre-release identifier. Automatically set based on branch and build type. |
<!-- markdownlint-enable MD060 -->

### Branch-based versioning

GitVersion automatically determines version suffixes based on the branch:

- **main branch**: Produces `2.x.x` versions
  - CI builds: `2.7.1-ci.3+5` (includes build metadata)
  - Production: `2.7.1` (no suffix)
- **v3 branch**: Produces `3.x.x-preview.N` versions (always has preview suffix)
- **Feature branches**: `x.x.x-{branch-name}.N` (branch name becomes suffix)

### Controlling version increments

Add semver tags to PR merge commit messages to control version bumps:

- `+semver: major` → increments major version (breaking changes)
- `+semver: minor` → increments minor version (new features)
- `+semver: patch` → increments patch version (bug fixes, default behavior)
- `+semver: none` → no version increment

Example merge commit: `feat: Add new indicator (+semver: minor)`

### Package deployment and tagging

Packages are deployed via GitHub Actions workflow:

**Automatic CI builds** (push to main or v3):

- Published to GitHub Packages
- Version includes build metadata (e.g., `2.7.1-ci.3+5`)
- Old CI packages auto-cleanup after 3 days (keeps 5 most recent)

**Manual production releases**:

- Published to nuget.org (main branch only for stable versions)
- Creates Git tag (e.g., `v2.7.1`)
- Creates GitHub Release
- Version determined by `preview` parameter:
  - `preview: false` → stable version (e.g., `2.7.1`)
  - `preview: true` → preview version (e.g., `3.0.0-preview.2`)

### Version examples

| Scenario | Branch | Version | Notes |
| :------- | :----- | :------ | :---- |
| CI build | main | `2.7.1-ci.3+5` | Auto-published to GitHub Packages |
| CI build | v3 | `3.0.0-preview.2+12` | Auto-published to GitHub Packages |
| Feature branch | feature/rsi | `2.8.0-rsi.1+3` | Feature branch build |
| Manual preview | v3 | `3.0.0-preview.2` | Manual deploy with preview=true |
| Manual production | main | `2.7.1` | Manual deploy with preview=false |

For technical details, see:

- GitVersion configuration: [`src/gitversion.yml`](https://github.com/DaveSkender/Stock.Indicators/blob/main/src/gitversion.yml)
- Deployment workflow: [`.github/workflows/deploy-package.yml`](https://github.com/DaveSkender/Stock.Indicators/blob/main/.github/workflows/deploy-package.yml)

## License

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

This repository uses a standard Apache 2.0 open-source license.  It enables open-source community development by protecting the project and contributors from certain legal risks while allowing the widest range of uses, including in closed source software.  Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## Contact info

[Start a new discussion](https://github.com/DaveSkender/Stock.Indicators/discussions) or [submit an issue](https://github.com/DaveSkender/Stock.Indicators/issues) if it is publicly relevant.  You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).

Thanks,
Dave Skender
