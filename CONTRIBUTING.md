# Contributing to Stock.Indicators

Thanks for taking the time to contribute!

Before contributing, please be aware that we are accepting these sorts of changes:

- Bug reports and fixes
- New generic indicators and overlays (lets say, "by the book" and reputable definitions)

We are not accepting things that should be done in your own extension code:

- Personal customizations and preferences
- Modified or augmented outputs that are not backed by standard definitions

## Reporting bugs

If you are reporting a bug, please submit an Issue with a detailed description of the problem.  Be sure to include steps to reproduce, code samples, and any reference materials.

## Developing

- Read this first: [contributing to an open-source GitHub project](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940)
- If you are adding a new indicator, the easiest way to do this is to copy the folder of an existing indicator and rename everything using the same naming conventions and taxonomy.
- All new indicators should include unit tests.
- Update the main README file if you're adding a new indicator, and the README file for the individual indicator.  This is our only user documentation.
- Increment the Package and Assembly versions in the Indicators project properties (see the Symantic Versioning link below for guidance).
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

## Questions?

Contact me through my GitHub profile (preferred) or submit an Issue with your question if it may be relevant for others.

Thanks,
Dave Skender
