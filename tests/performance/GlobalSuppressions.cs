// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "Required for BenchmarkDotNet")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1401:Fields should be private",
    Justification = "Required for BenchmarkDotNet",
    Scope = "member",
    Target = "~F:Tests.Performance.InternalsPerformance.Periods")]

[assembly: SuppressMessage("Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "Required for BenchmarkDotNet",
    Scope = "member",
    Target = "~F:Tests.Performance.InternalsPerformance.Periods")]
