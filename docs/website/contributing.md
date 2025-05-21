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

If you suspect a problem, please [report a bug Issue](https://github.com/DaveSkender/Stock.Indicators/issues/new?labels=bug&template=bug_report.md) with a detailed description of the problem, steps to reproduce, code samples, and any reference materials. For enhancements, [create a feature Issue](https://github.com/DaveSkender/Stock.Indicators/issues/new?labels=enhancement&template=feature_request.md).

Use the [Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions) area for general ideation and help/usage questions.

## Project management

- Planned work is managed in [the backlog](https://github.com/users/DaveSkender/projects/1).
- Work items are primarily entered as Notes (not Issues), except where an issue or feature is user reported. With that said, Notes can be converted to Issues if in-progress and collaborative discussion is needed.

## Developing

- Read this first: [A Step by Step Guide to Making Your First GitHub Contribution](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940). I also have a discussion [on Forking](https://github.com/DaveSkender/Stock.Indicators/discussions/503) if you have questions.
- If you want to work on something specific, please mention your intention on the related [Issue](https://github.com/DaveSkender/Stock.Indicators/issues). If an Issue does not exist for your contribution, please create one before starting. This will help us reserve that feature and avoid duplicative efforts.
- If you are adding a new indicator, the easiest way to do this is to copy the folder of an existing indicator and rename everything using the same naming conventions and taxonomy. All new indicators should include [tests](#testing).
- Do not commingle multiple contributions on different topics. Please keep changes small and separate.

## Branch strategy and pull requests

- This project uses a GitHub Flow strategy, with `main` branch always being the most current working version.
- Do your work on a feature branch, derived from `main`, with a meaningful name. For example: `feature/new-indicators` or `fix/typo-in-readme`
- Keep your branch fresh by pulling the latest changes and rebasing often.
- Submit a Pull Request to the `main` branch when ready for review.

## Pull request guidelines

- An admin will review Pull Requests and provide feedback.
- Keep the size and scope reasonable.
- Include tests and documentation updates with your changes.
- Adhere to the coding standards and patterns already established in the codebase.

## Testing

- All stock indicators have unit tests.
- All tests must pass.
- We target .NET 6.0 and higher.
- Test data is automatically shared between derived indicators.
- We have a common test base for most indicators in the `Test.Utilities` namespace.

## Contact info

Have questions?

- [Start a Discussion](https://github.com/DaveSkender/Stock.Indicators/discussions)
- Contact us by email: [hello@skender.com](mailto:hello@skender.com?subject=[GitHub]%20Stock%20Indicators)
- Message [@daveskender](https://twitter.com/messages/compose?recipient_id=27475431&text=re:%20Stock.Indicators%20for%20.NET) on Twitter/X
