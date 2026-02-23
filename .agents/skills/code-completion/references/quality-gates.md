# Quality gates reference

## Quick reference

| Gate | Command | VS Code task |
| ---- | ------- | ------------ |
| Lint .NET (fix) | `dotnet tool run roslynator fix --properties TargetFramework=net10.0 --severity-level hidden` | `Lint: .NET Roslynator (fix)` |
| Lint all (fix) | `dotnet format --severity info --no-restore && npx markdownlint-cli2 --fix` | `Lint: All (fix)` |
| Lint markdown (fix) | `npx markdownlint-cli2 --fix` | `Lint: Markdown (fix)` |
| Build | `dotnet build -v minimal --nologo` | `Build: .NET Solution (incremental)` |
| Test | `dotnet test --no-restore --nologo` | `Test: Unit tests` |
| Full check | `dotnet format --severity info --no-restore && dotnet build && dotnet test --no-restore && npx markdownlint-cli2` | Run sequentially |

## Configuration files

Linting:

- Analyzers: `src/Directory.Build.props`
- .NET format: `.editorconfig`
- Roslynator: Global configuration
- Markdown: `.markdownlint-cli2.jsonc`

Build:

- Solution: `Stock.Indicators.sln`
- Project: `src/Indicators.csproj`
- Dependencies: `src/Directory.Packages.props`

Test:

- Unit tests: `tests/tests.unit.runsettings`
- Regression: `tests/tests.regression.runsettings`
- Integration: `tests/tests.integration.runsettings`
