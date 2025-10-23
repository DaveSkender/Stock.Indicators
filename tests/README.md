# Testing

Tests are split into different projects for isolation of purpose.

```bash
# runs all unit
# and integration tests
dotnet test
```

> When developing locally, we recommend that you normally _unload_ the external test projects shown below, except when testing externalities.

## Unit tests

> `indicators/Tests.Indicators.csproj` unit tests library

Our primary full unit test project covers the entire utility of the library.  In most IDE, you can [manually select](https://learn.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022#manually-select-the-run-settings-file) the `tests/tests.unit.runsettings` for isolation for local IDE dev/test efficiency, or use the _unload_ approach described above.

```bash
# CLI equivalent
dotnet test --settings tests/tests.unit.runsettings
```

## Performance tests

> `tools/performance/Tests.Performance.csproj` benchmark tests

Running the `Tests.Performance` console application in `Release` mode will produce [benchmark performance data](https://dotnet.stockindicators.dev/performance/) that we include on our documentation site.

```bash
# run all performance benchmarks (~15-20 minutes)
dotnet run -c Release

# run individual performance benchmark
dotnet run -c Release --filter *.ToAdx

# run cohorts of performance benchmarks
dotnet run -c Release --filter **
```

```bash
# to see all cohorts
dotnet run --list
...
# Available Benchmarks:
  #0 BufferList
  #1 SeriesIndicators
  #2 StreamExternal
  #3 StreamIndicators
  #4 Utility
  #5 UtilityMaths

# to see all tests
dotnet run --list flat
```

## Integration tests

All integration tests are marked with `[TestCategory("Integration")]` and are located in separate test projects (`Tests.Integration` and `Tests.PublicApi`).

To run just the integration tests, you must explicitly use its configuration:

```bash
# CLI equivalent
dotnet test --settings tests/tests.integration.runsettings
```

Since we assume tests are non-integration tests by default, set the category attribute on any new test classes that contain integration tests. This can be applied uniquely to `[TestMethod]` as well.

```csharp
[TestClass, TestCategory("Integration")]
public class MyIntegrationTests : TestBase
...
```

### Public API tests

> `public-api/Tests.PublicApi.csproj` E2E library external tests

### Live integration tests

> `integration/Tests.Integration.csproj` connected to Live 3rd-party APIs

## Linting and code analyzers

### Roslynator

<https://josefpihrt.github.io/docs/roslynator/cli/commands/analyze>
<https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules>

```bash
Usage: roslynator analyze [<PROJECT|SOLUTION>] [options]

Arguments:
  <PROJECT|SOLUTION>  Path to one or more project/solution files.

Options:
  -a, --analyzer-assemblies <PATH>             Define one or more paths to an analyzer assembly or a directory that should be searched recursively for analyzer assemblies.
      --culture <CULTURE_ID>                   Defines culture that should be used to display diagnostic message.
      --exclude <GLOB>                         Space separated list of glob patterns to exclude files, folders, solutions or projects.
      --execution-time                         Indicates whether to measure execution time of each analyzer.
      --file-log <FILE_PATH>                   Path to a file that should store output.
      --file-log-verbosity <LEVEL>             Verbosity of the file log. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed] and diag[nostic].
  -h, --help                                   Show command line help.
      --ignore-analyzer-references             Indicates whether analyzers that are referenced in a project should be ignored.
      --ignore-compiler-diagnostics            Indicates whether to display compiler diagnostics.
      --ignored-diagnostics <DIAGNOSTIC_ID>    Defines diagnostics that should not be reported.
      --ignored-projects <PROJECT_NAME>        Defines projects that should not be analyzed.
      --include <GLOB>                         Space separated list of glob patterns to include files, folders, solutions or projects.
      --language <LANGUAGE>                    Defines project language. Allowed values are cs[harp] or v[isual-]b[asic].
  -m, --msbuild-path <DIRECTORY_PATH>          Defines a path to MSBuild directory.
  -o, --output <FILE_PATH>                     Defines path to file that will store reported diagnostics. The format of the file is determined by the --output-format option, with the default being xml.
      --output-format                          Defines the file format of the report written to file. Supported options are: gitlab and xml, with xml the default if no option is provided.
      --projects <PROJECT_NAME>                Defines projects that should be analyzed.
  -p, --properties <NAME=VALUE>                Defines one or more MSBuild properties.
      --report-not-configurable                Indicates whether diagnostics with 'NotConfigurable' tag should be reported.
      --report-suppressed-diagnostics          Indicates whether suppressed diagnostics should be reported.
      --severity-level <LEVEL>                 Defines minimally required severity for a diagnostic. Allowed values are hidden, info (default), warning or error.
      --supported-diagnostics <DIAGNOSTIC_ID>  Defines diagnostics that should be reported.
  -v, --verbosity <LEVEL>                      Verbosity of the log. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed] and diag[nostic].

roslynator fix \
--properties TargetFramework=net9.0 \
--verbosity detailed \
--severity-level hidden \
--ignored-diagnostics IDE0008 IDE0010 IDE0045 IDE0046 IDE0047 IDE0054 IDE2003 RCS1228 \
IDE0320, RCS1238
# IDE0320 fixes okay, only ignored due to significant changes
# RCS1238 reverts IDE0045 in a problematic way

roslynator analyze \
--properties TargetFramework=net9.0 \
--verbosity normal \
--severity-level hidden \
--language csharp

CS8019  Unnecessary using directive
CS8933  The using directive appeared previously as global using
IDE0005 Using directive is unnecessary.
IDE0008 Use explicit type
# IDE0010 Add missing cases
IDE0021 Use block body for constructor
IDE0022 Use expression body for method
IDE0028 Simplify collection initialization
# IDE0045 Use conditional expression for assignment (if statement simplified)
# IDE0046 Use conditional expression for return
# IDE0047 Remove unnecessary parentheses
IDE0052 Remove unread private members
IDE0072 Add missing cases
IDE0078 Use pattern matching
IDE0082 'typeof' can be converted to 'nameof'
IDE0320 Make anonymous function static        # doesn't seem right
# IDE2003 Blank line required between block and subsequent statement
RCS1006 Merge 'else' with nested 'if'
RCS1031 Remove unnecessary braces in switch section
RCS1043 Remove 'partial' modifier from type with a single part
# IDE0054 'Use '++' operator'
RCS1055 Unnecessary semicolon at the end of declaration
RCS1061 Merge 'if' with nested 'if'
RCS1070 Remove redundant default switch section
RCS1124 Inline local variable
RCS1129 Remove redundant field initialization
RCS1140 Add exception to documentation comment
RCS1141 Add 'param' element to documentation comment
RCS1142 Add 'typeparam' element to documentation comment
RCS1151 Remove redundant cast
RCS1161 Enum should declare explicit values
RCS1168 Parameter name differs from base name
RCS1181 Convert comment to documentation comment
RCS1189 Add or remove region name
RCS1201 Use method chaining
RCS1211 Remove unnecessary 'else'
# RCS1228 Unused element in a documentation comment
RCS1238 Avoid nested ?: operators           # reverts IDE0045
```
