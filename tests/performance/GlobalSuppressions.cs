using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "Required for BenchmarkDotNet")]

[assembly: SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "Required for BenchmarkDotNet")]
