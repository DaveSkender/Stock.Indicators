# Contributing guidelines

[![Board Status](https://dev.azure.com/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/69f29c08-2257-4429-9cea-1629abcd3064/_apis/work/boardbadge/a1dfc6ae-7836-4b56-a849-9a48698252c2)](https://dev.azure.com/skender/5123ca47-74f2-4d67-a5d4-c4d90b8d670a/_boards/board/t/69f29c08-2257-4429-9cea-1629abcd3064/Microsoft.RequirementCategory/)

Thanks for taking the time to contribute!

This project is simpler than most, so it's a good place to start contributing to the open source community, even if you're a newbie.

Before contributing, please be aware that we are accepting these sorts of changes:

- Bug reports and fixes
- New generic indicators and overlays (reputable "by the book" definitions only)

We are not accepting things that should be done in your own wrapper code:

- Personal customizations and preferences
- Modified or augmented outputs that are not backed by standard definitions

## Reporting bugs and feature requests

If you are reporting a bug or feature request, please [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with a detailed description of the problem or recommended feature.  For bugs, be sure to include steps to reproduce, code samples, and any reference materials.

## Developing

- Read this first: [contributing to an open-source GitHub project](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940)
- Our backlog is [here](https://dev.azure.com/skender/Stock.Indicators/_boards/board/t/Stock.Indicators)
- If you are adding a new indicator, the easiest way to do this is to copy the folder of an existing indicator and rename everything using the same naming conventions and taxonomy.
- All new indicators should include unit tests.
- Update the `INDICATORS.md` file if you're adding a new indicator, and the `README.md` file for the individual indicator.  This is our only user documentation.
- Do not comingle multiple contributions.  Please keep changes small and separate.

## Testing

- Review the `tests/indicators` folder for examples of unit tests.  Just copy one of these.
- New indicators should be tested against manually calculated, proven, accurate results.  It is helpful to attach your manual calculations to the Pull Request when [submitting changes](#submitting-changes).
- Stock History is automatically added to unit test methods.  A `History.xlsx` Excel file is included in the `test data` folder that is an exact copy of what is used in the unit tests.  Use this for your manual calculations to ensure that it is correct.  Do not commit changes to this Excel file.
- We expect all unit tests to execute successfully and all Errors and Warning resolved before you submit your code.
- Failed builds or unit testing will block acceptance of your Pull Request, when submitting changes.

### Performance benchmarking

- Running the `Tests.Performance` console application in `Release` mode will produce performance reports in the `bin` folder that we periodically include in the repo documentation.  You can find or update the latest results [here](../tests/performance/README.md).

## Submitting changes

Submit a Pull Request with a clear list of what you've done (read more about [pull requests](http://help.github.com/pull-requests/)).

Always write a clear log message for your commits. One-line messages are fine for most changes, but bigger changes that require more explanation should look like this:

    $ git commit -m "A brief summary of the commit
    > 
    > A paragraph describing what changed and its impact."

After a Pull Request is reviewed, accepted, and [squash] merged to master, we may batch changes before publishing a new package version to the [public NuGet repository](https://www.nuget.org/packages/Skender.Stock.Indicators).  Please be patient with turnaround time.

## Code Reviews and Administration

If you want to contribute administratively or help with code reviews, I'm also currently seeking a few core people to help so I don't have to be the only person to *review changes ... including my own!*

## About the NuGet packaging

For the NuGet packaging, we're using:

- [Symantic Version 2.0](https://semver.org/)
- [NuGet best practices](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/nuget)

## About versioning

We use the `GitVersion` tool for versioning.  It is mostly auto generated in the [Azure DevOps build](https://dev.azure.com/skender/Stock.Indicators/_build?definitionId=18).  This only needs to be done on the merge to `master`, so your feature branch does not need to include this as it will get squashed anyway.

- Adding `+semver: major` as a commit message will increment the major x.-.- element
- Adding `+semver: minor` as a commit message will increment the minor -.x.- element
- Adding `+semver: patch` as a commit message will increment the minor -.-.x element.  Patch element auto-increments, so you'd only need to do this to override the next value.

## Contact info

Contact us through the NuGet [Contact Owners](https://www.nuget.org/packages/Skender.Stock.Indicators) method or [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with your question if it is publicly relevant.

Thanks,
Dave Skender
