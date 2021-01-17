// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "Required syntax for BenchmarkDotNet",
    Scope = "member",
    Target = "~F:Tests.Performance.MarkFunctions.Periods")]

[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "These cannot be static for performance tests.")]
