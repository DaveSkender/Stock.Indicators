# Contributing guidelines

Thanks for taking the time to contribute!

This project is simpler than most, so it's a good place to start contributing to the open source community, even if you're a newbie.

We are accepting these sorts of changes and requests:

- Bug reports and fixes
- Usability improvements
- Documentation updates
- New reputable "by the book" indicators and overlays

We are not accepting things that should be done in your own wrapper code:

- Personal customizations and preferences
- Modified or augmented outputs that are not instrinsic

## Reporting bugs and feature requests

If you are reporting a bug or suspect a problem, please [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with a detailed description of the problem + include steps to reproduce, code samples, and any reference materials.  For new features, add a new Issue with the `enhancement` label.

## Project management

- Planned work is managed in [the backlog](https://github.com/DaveSkender/Stock.Indicators/projects/1).
- Work items are primarily [entered as Notes](https://docs.github.com/en/free-pro-team@latest/github/managing-your-work-on-github/adding-notes-to-a-project-board) (not Issues), except where an issue or feature is user reported.  With that said, Notes can be converted to Issues if in-progress and collaborative discussion is needed.
- Use the [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions) area for general ideation and unrelated questions.

## Developing

- Read this first: [contributing to an open-source GitHub project](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940)
- If you are adding a new indicator, the easiest way to do this is to copy the folder of an existing indicator and rename everything using the same naming conventions and taxonomy.  All new indicators should include unit and performance tests.
- Update the `INDICATORS.md` file if you're adding a new indicator, and the `README.md` file for the individual indicator.  This is our only user documentation.
- Do not comingle multiple contributions.  Please keep changes small and separate.

## Testing

- Review the `tests/indicators` folder for examples of unit tests.  Just copy one of these.
- New indicators should be tested against manually calculated, proven, accurate results.  It is helpful to include your manual calculations spreadsheet in the appropriate indicator folder when [submitting changes](#submitting-changes).
- Stock History is automatically added to unit test methods.  A `History.xlsx` Excel file is included in the `test data` folder that is an exact copy of what is used in the unit tests.  Use this for your manual calculations to ensure that it is correct.  Do not commit changes to this Excel file.
- We expect all unit tests to execute successfully and all Errors and Warning resolved before you submit your code.
- Failed builds or unit testing will block acceptance of your Pull Request, when submitting changes.

### Performance benchmarking

Running the `Tests.Performance` console application in `Release` mode will produce performance data that we periodically include in the repo documentation.  You can find the latest results [here](../tests/performance/README.md).

```bash
# run all performance benchmarks
dotnet run -c Release

# run individual performance benchmark
dotnet run -c Release --filter *.GetAdx
```

## Submitting changes

Submit a Pull Request with a clear list of what you've done (read more about [pull requests](http://help.github.com/pull-requests)).

Always write a clear log message for your commits. One-line messages are fine for most changes, but bigger changes that require more explanation should look like this:

```bash
git commit -m "A brief summary of the commit
>
> A paragraph describing what changed and its impact."
```

After a Pull Request is reviewed, accepted, and [squash] merged to master, we may batch changes before publishing a new package version to the [public NuGet repository](https://www.nuget.org/packages/Skender.Stock.Indicators).  Please be patient with turnaround time.

## Code reviews and administration

If you want to contribute administratively, do code reviews, or provide general user support, we're also currently seeking a few core people to help.  Please [contact us](#contact-info) if interested.

## Standards and design guidelines

- [.NET Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines)
- [NuGet Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/nuget)
- [Semantic Version 2.0](https://semver.org)

## About versioning

We use the `GitVersion` tool for [semantic versioning](https://semver.org).  It is mostly auto generated in the [Azure DevOps build](https://dev.azure.com/skender/Stock.Indicators/_build?definitionId=21).

Type | Format | Description
------------ | ------ | -----------
Major | `x.-.-` | A significant deviation with breaking changes.
Minor | `-.x.-` | A new feature, usually new non-breaking change, such as adding an indicator.  Small breaking changes may occur here and are denoted in the [release notes](https://github.com/DaveSkender/Stock.Indicators/releases).
Patch | `-.-.x` | A small bug fix, chore, or documentation change.
Increment | `-.-.-+x` | Intermediate commits between releases.

This only needs to be done on the merge to `master` when the Pull Request is committed, so your feature branch does not need to include this as it will get squashed anyway.

- Adding `+semver: major` as a commit message will increment the major x.-.- element
- Adding `+semver: minor` as a commit message will increment the minor -.x.- element
- Adding `+semver: patch` as a commit message will increment the minor -.-.x element.  Patch element auto-increments, so you'd only need to do this to override the next value.

A manual Git `tag`, in accordance with the above schema, is introduced when deploying to package managers and is reflected in the [Releases](https://github.com/DaveSkender/Stock.Indicators/releases).

## License

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

This repository uses a standard Apache 2.0 open-source license.  It enables open-source community development by protecting the project and contributors from certain legal risks while allowing the widest range of uses, including in closed source software.  Please review the [license](https://opensource.org/licenses/Apache-2.0) before using or contributing to the software.

## Contact info

[Start a new feature discussion, ask a question](https://github.com/DaveSkender/Stock.Indicators/discussions), or [submit an issue](https://github.com/DaveSkender/Stock.Indicators/issues) if it is publicly relevant.  You can also direct message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431).

Thanks,
Dave Skender
