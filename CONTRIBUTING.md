# Contributing to Stock.Indicators

Thanks for taking the time to contribute!

Before contributing, please be aware that we are accepting these sorts of changes:

- Bug reports and fixes
- New generic indicators and overlays (lets say, "by the book" and reputable definitions)

We are not accepting things that should be done in your own extension code:

- Personal customizations and preferences
- Modified or augmented outputs that are not backed by standard definitions

## Reporting bug or feature requests

If you are reporting a bug or feature request, please [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with a detailed description of the problem or recommended feature.  For bugs, be sure to include steps to reproduce, code samples, and any reference materials.

## Developing

- Read this first: [contributing to an open-source GitHub project](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940)
- Our backlog is [here](https://dev.azure.com/skender/Stock.Indicators/_boards/board/t/Stock.Indicators)
- If you are adding a new indicator, the easiest way to do this is to copy the folder of an existing indicator and rename everything using the same naming conventions and taxonomy.
- All new indicators should include unit tests.
- Update the main README file if you're adding a new indicator, and the README file for the individual indicator.  This is our only user documentation.
- Do not comingle multiple contributions.  Please keep changes small and separate.

## Testing

- Review the IndicatorTests folder/project for examples of unit tests.  Just copy one of these.
- New indicators should be tested against manually calculated, proven, accurate results.
- Stock History is automatically added to unit test methods.  A `History.xlsx` Excel file is included in the `Test Data` folder that is an exact copy of what is used in the unit tests.  Use this for your manual calculations to ensure that it is correct.  Do not commit changes to this Excel file.
- We expect all unit tests to execute successfully and all Errors and Warning resolved before you submit your code.
- Failed builds or unit testing will result in immediate rejection of your changes.

## Submitting changes

Submit a Pull Request with a clear list of what you've done (read more about [pull requests](http://help.github.com/pull-requests/)).

Always write a clear log message for your commits. One-line messages are fine for small changes, but bigger changes should look like this:

    $ git commit -m "A brief summary of the commit
    > 
    > A paragraph describing what changed and its impact."

After a Pull Request is reviewed, accepted, and merged to master, we may batch changes before publishing a new package version to the public NuGet repository.  Please be patient with turnaround time.

## About the NuGet packaging

For the NuGet packaging, we're using:

- [Symantic Version 2.0](https://semver.org/)
- [NuGet best practices](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/nuget)

## About versioning

- We use the `GitVersion` tool for versioning.  It is mostly auto generated in the [Azure DevOps build](https://dev.azure.com/skender/Stock.Indicators/_build?definitionId=18)
- Adding `+semver: major` as a commit message will increment the major x.-.- element
- Adding `+semver: minor` as a commit message will increment the minor -.x.- element
- Adding `+semver: patch` as a commit message will increment the minor -.-.x element.  Patch element auto-increments, so you'd only need to do this to override the next value.

## Questions?

Contact us through the NuGet [Contact Owners](https://www.nuget.org/packages/Skender.Stock.Indicators) method (preferred) or [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with your question if it is publicly relevant.

Thanks,
Dave Skender
