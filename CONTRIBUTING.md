# Contributing to Stock Indicators for .NET

[![Codacy quality grade](https://app.codacy.com/project/badge/Grade/012497adc00847eca9ee91a58d00cc4f)](https://app.codacy.com/gh/DaveSkender/Stock.Indicators/dashboard)
[![Codacy code coverage](https://app.codacy.com/project/badge/Coverage/012497adc00847eca9ee91a58d00cc4f)](https://app.codacy.com/gh/DaveSkender/Stock.Indicators/dashboard)

**Thanks for taking the time to contribute!** 🎉

This project is simpler than most, making it a great place to start contributing to open source, even if you're new to it.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Branching and Naming Conventions](#branching-and-naming-conventions)
- [Types of Contributions](#types-of-contributions)
- [Reporting Issues](#reporting-issues)
- [Submitting Changes](#submitting-changes)
- [Testing](#testing)
- [Documentation](#documentation)
- [Project Management](#project-management)
- [Labels and Project Boards](#labels-and-project-boards)
- [Security](#security)
- [License](#license)

## Code of Conduct

This project adheres to a [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you're expected to uphold this code.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- Git
- An IDE (Visual Studio, VS Code, or JetBrains Rider)

### First-time Setup

1. **Fork and clone the repository**
   ```bash
   git clone https://github.com/YOUR-USERNAME/Stock.Indicators.git
   cd Stock.Indicators
   ```

2. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/DaveSkender/Stock.Indicators.git
   ```

3. **Build and test**
   ```bash
   dotnet restore
   dotnet build --configuration Release
   dotnet test
   ```

If you're new to GitHub contributions, read [A Step by Step Guide to Making Your First GitHub Contribution](https://codeburst.io/a-step-by-step-guide-to-making-your-first-github-contribution-5302260a2940).

## Development Setup

### Building the Project

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build --configuration Release

# Build with warnings as errors (like CI)
dotnet build --configuration Release -warnAsError
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/indicators/Tests.Indicators.csproj

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run performance tests
dotnet test tests/performance/Tests.Performance.csproj
```

### Working with Examples

```bash
# Run console example
dotnet run --project docs/examples/ConsoleApp

# Test examples
dotnet test tests/examples/Tests.Examples.csproj
```

## Branching and Naming Conventions

### Primary Branch

- **`main`** - The primary development branch
- All pull requests should target `main`
- Releases are tagged and deployed from `main`

### Branch Naming

Use descriptive branch names that indicate the type and scope of changes:

```
feature/add-stochastic-rsi
fix/sma-null-handling
docs/update-readme
chore/update-dependencies
```

### Commit Messages

Write clear, concise commit messages:

```
Add Stochastic RSI indicator

- Implement StochasticRsi calculation method
- Add comprehensive unit tests
- Update documentation with usage examples
```

For version control, we use semantic versioning with GitVersion:
- `+semver: major` - Breaking changes
- `+semver: minor` - New features (default for new indicators)
- `+semver: patch` - Bug fixes and minor changes

## Types of Contributions

### ✅ We Accept

- **Bug reports and fixes**
- **Usability improvements**
- **Documentation updates**
- **New reputable "by the book" indicators and overlays**
- **Performance optimizations**
- **Test improvements**

### ❌ We Don't Accept

- Personal customizations and preferences
- Modified or augmented outputs that aren't intrinsic to the indicator
- Breaking changes without strong justification
- Features that should be implemented in wrapper code

## Reporting Issues

### Bug Reports

Use our [bug report template](.github/ISSUE_TEMPLATE/bug_report.yml) and include:

- **Clear description** of the issue and expected behavior
- **Steps to reproduce** the problem
- **Code sample** that demonstrates the issue
- **Environment details** (library version, .NET version, OS)
- **Error messages** or stack traces
- **Additional context** like screenshots or related issues

### Feature Requests

Use our [feature request template](.github/ISSUE_TEMPLATE/feature_request.yml) and include:

- **Problem statement** - What are you trying to solve?
- **Proposed solution** - Describe your preferred approach
- **Alternative solutions** - What else have you considered?
- **Reference materials** - Links, papers, or documentation
- **Additional context** - Urgency, willingness to contribute, etc.

### Before Reporting

- Check [existing issues](https://github.com/DaveSkender/Stock.Indicators/issues)
- Review [community discussions](https://github.com/DaveSkender/Stock.Indicators/discussions)
- Read [Help! Results don't match TradingView!](https://github.com/DaveSkender/Stock.Indicators/discussions/801)

## Submitting Changes

### Pull Request Process

1. **Create an issue** first (unless it's a trivial fix)
2. **Work on a feature branch** from your fork
3. **Follow coding standards** and include tests
4. **Update documentation** if needed
5. **Submit a pull request** using our [PR template](.github/PULL_REQUEST_TEMPLATE.md)

### Pull Request Guidelines

- **Link to related issues** in the PR description
- **Keep changes small and focused** - one feature/fix per PR
- **Include tests** for new functionality
- **Update documentation** for user-facing changes
- **Ensure CI passes** before requesting review

### Review Process

- All submissions require review before merging
- We may suggest changes, improvements, or alternatives
- Once approved, PRs are squash-merged to `main`
- Changes are batched before publishing to NuGet

## Testing

### Test Structure

- **`tests/indicators/`** - Unit tests for indicators
- **`tests/other/`** - Tests for utilities and edge cases
- **`tests/performance/`** - Performance benchmarks
- **`tests/examples/`** - Tests for example code

### Writing Tests

When adding new indicators:

```csharp
[TestMethod]
public void GetSma_Standard()
{
    // Test standard cases
    var results = quotes.GetSma(20).ToList();
    
    // Assertions for expected values
    Assert.AreEqual(502, results.Count);
    Assert.IsNull(results[18].Sma);
    Assert.AreEqual(214.5250m, Math.Round(results[19].Sma.Value, 4));
}

[TestMethod]
public void GetSma_BadData()
{
    // Test error conditions
    Assert.ThrowsException<ArgumentOutOfRangeException>(
        () => quotes.GetSma(0));
}
```

### Performance Tests

Include performance tests for new indicators:

```csharp
[TestMethod]
public void GetSma_Performance()
{
    var results = TestData.GetDefault(100_000)
        .GetSma(50)
        .ToList();
        
    Assert.AreEqual(100_000, results.Count);
}
```

## Documentation

### Code Documentation

- Use XML documentation comments for public APIs
- Keep comments concise and focused on usage
- Include examples for complex methods

### User Documentation

- Update the main documentation site when adding features
- Follow the existing style and structure
- Test examples to ensure they work

## Project Management

- **Planned work** is managed in [our backlog](https://github.com/users/DaveSkender/projects/1)
- **Work items** are primarily Notes, converted to Issues when needed
- **Discussions** are used for ideation and Q&A

## Labels and Project Boards

### Issue Labels

- **`bug`** - Something isn't working correctly
- **`enhancement`** - New feature or improvement request
- **`documentation`** - Documentation improvements needed
- **`good first issue`** - Suitable for newcomers
- **`help wanted`** - Extra attention/help needed
- **`performance`** - Performance-related improvements
- **`question`** - Further information requested
- **`wontfix`** - This will not be worked on

### Pull Request Labels

- **`breaking-change`** - Contains breaking changes
- **`new-indicator`** - Adds a new technical indicator
- **`bug-fix`** - Fixes a reported bug
- **`documentation`** - Documentation-only changes

### Project Boards

We use [GitHub Projects](https://github.com/users/DaveSkender/projects/1) to track:

- **Backlog** - Planned features and improvements
- **In Progress** - Currently being worked on
- **Review** - Awaiting code review or testing
- **Done** - Completed work

## Security

### Reporting Security Issues

Report security vulnerabilities privately via [GitHub Security Advisories](https://github.com/DaveSkender/Stock.Indicators/security/advisories/new).

### Security Scanning

- **Code scanning** is enabled via GitHub Advanced Security
- **Secret scanning** prevents accidental credential commits
- **Dependency scanning** monitors for vulnerable packages
- **Codacy** provides additional code quality and security analysis

### Response Process

1. Security issues are reviewed within 48 hours
2. Fixes are prioritized and developed privately
3. Patches are released as soon as possible
4. Public disclosure follows responsible disclosure practices

## License

By contributing to this project, you:

- Acknowledge and agree to the [Developer Certificate of Origin (DCO) 1.1](https://developercertificate.org)
- Grant your contributions under the [Apache 2.0 license](LICENSE)

This means your contributions will be freely available under the same open-source terms.

## GitHub Copilot and AI Agent Access

### Repository Configuration for AI Agents

This repository is optimized for GitHub Copilot and coding agents with:

- **Clear documentation** - Comprehensive setup and contribution guidelines
- **Detailed templates** - Structured issue and PR templates for consistent information
- **Automated workflows** - CI/CD pipelines with clear status indicators
- **Standardized labels** - Consistent labeling for issue/PR categorization
- **Security scanning** - Automated vulnerability detection and dependency updates

### Access and Permissions

#### For GitHub Copilot Users
- **Repository access** - Standard read access enables Copilot suggestions
- **Context awareness** - Copilot has access to:
  - Public repository content
  - Issue and PR history
  - Documentation and examples
  - Coding patterns and conventions

#### For Coding Agents
When working with coding agents on this repository:

1. **Fork and clone** - Agents should work from forks for security
2. **Branch strategy** - Use feature branches as documented above
3. **Testing requirements** - All changes must include appropriate tests
4. **Documentation updates** - Update docs for user-facing changes
5. **Security compliance** - Follow security scanning and dependency policies

#### Permission Escalation
If additional access is needed:

1. **Create an issue** describing the access requirements
2. **Tag maintainers** (@DaveSkender) for review
3. **Provide justification** for the requested permissions
4. **Wait for approval** before proceeding

### Best Practices for AI Contributions

#### Code Quality
- Follow existing patterns and conventions
- Include comprehensive tests for new features
- Maintain backward compatibility unless breaking changes are justified
- Use meaningful commit messages and PR descriptions

#### Communication
- Reference related issues in PRs
- Provide clear explanations for complex changes
- Ask questions in discussions when uncertain
- Be responsive to review feedback

#### Security Considerations
- Never commit secrets or sensitive data
- Follow secure coding practices
- Respect dependency scanning alerts
- Report security issues through proper channels

---

## Need Help?

- 💬 [Start a discussion](https://github.com/DaveSkender/Stock.Indicators/discussions) for questions
- 📧 [Contact the maintainers](https://github.com/DaveSkender/Stock.Indicators/discussions) for guidance
- 💎 Consider [sponsoring the project](https://github.com/sponsors/facioquo) for priority support

**Thank you for contributing to Stock Indicators for .NET!** 🚀