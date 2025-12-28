using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "Required for BenchmarkDotNet")]

[assembly: SuppressMessage(
    "Design",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by BenchmarkDotNet via reflection",
    Scope = "type",
    Target = "~T:Performance.ManualTestDirectConfig")]

[assembly: SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Test projects use public class types.")]

[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "Required for BenchmarkDotNet")]
