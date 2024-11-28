# Test application

This is only used for testing API breaking changes. It contains instantiations of all public classes and methods in the library.

## How the test application exercises the public API

The test application in `tests/external/application/Program.cs` exercises the public API by instantiating and using all public classes and methods. It includes the following steps:

1. Load sample data
2. Exercise the public API by calling methods like `GetAdl`, `GetObv`, `GetPrs`, `GetRoc`, `GetStdDev`, and `GetTrix`
3. Print the results to the console

The `tests/external/application/Test.Application.csproj` references both Skender.Stock.Indicators v2.6.0 and the new Indicators project to ensure compatibility and provide obsolete warnings to guide necessary changes.
